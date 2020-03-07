using Coravel.Queuing.Interfaces;
using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Exceptions;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluateController : Controller
    {
        private readonly IEvaluator _evaluatorService;
        private readonly ISynthesisRepository _synthesisRepository;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly ISignalR<SynthesisProgressHub> _synthesisSignalR;
        private readonly ISignalR<AnalysisProgressHub> _analysisSignalR;
        private readonly IQueue _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _config;

        public EvaluateController
        (
            IEvaluator evaluatorService,
            ISynthesisRepository synthesisRepository,
            IAnalysisRepository analysisRepository,
            IMicrosoftSQL microsoftSQLService,
            IQueue queue,
            ISignalR<SynthesisProgressHub> synthesisSignalR,
            ISignalR<AnalysisProgressHub> analysisSignalR,
            IServiceScopeFactory serviceScopeFactory,
            IConfiguration config
        )
        {
            _evaluatorService = evaluatorService;
            _synthesisRepository = synthesisRepository;
            _analysisRepository = analysisRepository;
            _microsoftSQLService = microsoftSQLService;
            _queue = queue;
            _synthesisSignalR = synthesisSignalR;
            _analysisSignalR = analysisSignalR;
            _serviceScopeFactory = serviceScopeFactory;
            _config = config;
        }

        [HttpGet, Route("get/papers/synthesis/{testId}")]
        public ActionResult<IEnumerable<SynthesisTestStudentEntity>> SynthesisEvaluationData([FromRoute] int testId)
            => Ok(_synthesisRepository.GetPapers(testId));

        [HttpPost, Route("Start/Evaluation/Synthesis")]
        public async Task<ActionResult> StartSynthesisEvaluationAsync([FromBody] StartTestEvalulationSynthesis model)
        {
            var jobId = $"evaluate_synthesis_{model.TestId}";
            foreach (var request in model.EvaluationRequests)
            {
                _queue.QueueAsyncTask(() => EvaluateSynthesisPaper(_serviceScopeFactory, jobId, model.TestId, request.StudentId, request.UseSecretData));

                //SET STATUS IN DB TO Queued -> To be implemented (remove synthesis paper entity first): posalji timeStamp sa FE + novi setStatus endpoint koji sam pravi entity

                await _synthesisSignalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                {
                    id = request.StudentId,
                    secret = request.UseSecretData,
                    status = (int)EvaluationProgress.Queued
                });
            }

            return Ok();
        }

        [NonAction]
        private async Task EvaluateSynthesisPaper(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, int studentId, bool useSecretData)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var logRepository = scope.ServiceProvider.GetService<ILogRepository>();
                try
                {

                    var synthesisRepository = scope.ServiceProvider.GetService<ISynthesisRepository>();
                    var sts = synthesisRepository.GetEvaluationData(testId, studentId);

                    #region Record_Start
                    var setStatusSuccess = synthesisRepository.SaveProgress(sts, useSecretData, EvaluationProgress.Evaluating);
                    if (setStatusSuccess)
                        await _synthesisSignalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                        {
                            id = studentId,
                            secret = useSecretData,
                            status = (int)EvaluationProgress.Evaluating
                        });
                    else
                        throw new SynthesisEvaluationException(useSecretData, testId, studentId, $"Failed to set status to {EvaluationProgress.Evaluating}");
                    #endregion

                    #region Prepare_Data
                    var originalDataNameOnServer = useSecretData ? sts.SynthesisTest.Task.NameOnServer : sts.SynthesisTest.Task.Template.NameOnServer;
                    var cloneDataNameOnServer = NameHelper.SynthesisTestEvaluation(studentId, testId, useSecretData);

                    var solutionScript = sts.SynthesisTest.Task.SolutionSqlScript;
                    var solutionFormat = sts.SynthesisTest.Task.SolutionColumns.Select(sc => sc.ColumnName);
                    var studentScript = sts.SqlScript;
                    #endregion

                    #region Clone_DB
                    var cloneSuccess = _microsoftSQLService.CloneDatabase(originalDataNameOnServer, cloneDataNameOnServer, false);
                    if (!cloneSuccess)
                        throw new SynthesisEvaluationException(useSecretData, testId, studentId, $"Failed to clone database '{originalDataNameOnServer}' into '{cloneDataNameOnServer}'");
                    #endregion

                    #region Evaluate
                    EvaluationResult result = null;
                    var studentResult = _microsoftSQLService.ExecuteSQLAsAdmin(studentScript, cloneDataNameOnServer);
                    if (studentResult.Messages.Any())
                        result = new EvaluationResult() { Pass = false, Message = $"Student Sql execution failed with errors:'{string.Join(", ", studentResult.Messages)}'" };

                    if (result == null)
                    {
                        var teacherResult = _microsoftSQLService.ExecuteSQLAsAdmin(solutionScript, originalDataNameOnServer);
                        if (teacherResult.Messages.Any())
                            throw new SynthesisEvaluationException(useSecretData, testId, studentId, $"Failed to execute solution script: '{string.Join(", ", teacherResult.Messages)}'");

                        result = _evaluatorService.EvaluateQueryOutputs(studentResult, teacherResult, solutionFormat, true);
                    }
                    #endregion

                    #region Record_End_And_CleanUp
                    var saveResultSuccess = synthesisRepository.SaveProgress(sts, useSecretData, result.PassStatus, result.Message);
                    if (saveResultSuccess)
                        await _synthesisSignalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                        {
                            id = studentId,
                            secret = useSecretData,
                            status = (int)(result.Pass ? EvaluationProgress.Passed : EvaluationProgress.Failed)
                        });

                    var deleteSuccess = _microsoftSQLService.DeleteDatabaseIfExists(cloneDataNameOnServer);

                    if (!deleteSuccess)
                        throw new SynthesisEvaluationException(useSecretData, testId, studentId, $"Delete database {cloneDataNameOnServer} failed");

                    if (!saveResultSuccess)
                        throw new SynthesisEvaluationException(useSecretData, testId, studentId, "Failed to save evaluation result");
                    #endregion
                }
                catch (Exception ex)
                {
                    logRepository.Log(ex);
                }
            }
        }

        [HttpGet, Route("get/papers/analysis/{testId}")]
        public ActionResult<IEnumerable<AnalysisTestStudentEntity>> AnalysisEvaluationData([FromRoute] int testId)
            => Ok(_analysisRepository.GetPapers(testId));

        [HttpPost, Route("Start/Evaluation/Analysis")]
        public async Task<ActionResult> StartAnalysisEvaluationAsync([FromBody] StartTestEvalulationAnalysis model)
        {
            var jobId = $"evaluate_analysis_{model.TestId}";
            foreach (var studentId in model.StudentIds)
            {
                _queue.QueueAsyncTask(() => EvaluateAnalysisPaper(_serviceScopeFactory, jobId, model.TestId, studentId));
                
                //SET STATUS IN DB TO Queued
                
                await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                {
                    id = studentId,
                    type = (int)AnalysisEvaluationType.PrepareData,
                    status = (int)EvaluationProgress.Queued
                });
            }

            return Ok();
        }

        [NonAction]
        private async Task EvaluateAnalysisPaper(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, int studentId)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {
                    var type = AnalysisEvaluationType.PrepareData;

                    var analysisRepository = scope.ServiceProvider.GetService<IAnalysisRepository>();
                    var ats = analysisRepository.GetEvaluationData(testId, studentId);

                    #region Record_Start
                    var setStatusSuccess = analysisRepository.SaveProgress(ats, type, EvaluationProgress.Evaluating);
                    if (setStatusSuccess)
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type,
                            status = (int)EvaluationProgress.Evaluating
                        });
                    else
                        throw new AnalysisEvaluationException(type, testId, studentId, $"Failed to set status to {EvaluationProgress.Evaluating}");
                    #endregion

                    #region Find_Data
                    var sts = ats.AnalysisTest.SynthesisTestStudent;
                    var studentSolutionSql = sts.SqlScript;
                    var task = sts.SynthesisTest.Task;
                    var teacherSolutionSql = task.SolutionSqlScript;
                    var solutionFormat = task.SolutionColumns.Select(sc => sc.ColumnName);
                    #endregion

                    #region Clone_DB
                    var originalDataNameOnServer = ats.InputNameOnServer;
                    var cloneDataNameOnServer = NameHelper.AnalysisTestEvaluation(studentId, testId, type);
                    var cloneSuccess = _microsoftSQLService.CloneDatabase(originalDataNameOnServer, cloneDataNameOnServer, false);
                    if (!cloneSuccess)
                        throw new AnalysisEvaluationException(type, testId, studentId, $"Failed to clone database '{originalDataNameOnServer}' into '{cloneDataNameOnServer}'");
                    #endregion

                    #region Get_Outputs
                    EvaluationResult result = new EvaluationResult() { Pass = true, Message = "Data successfully prepared." };

                    var studentOutput = _microsoftSQLService.ExecuteSQLAsAdmin(studentSolutionSql, cloneDataNameOnServer);
                    if (studentOutput.Messages.Any())
                        result = new EvaluationResult() { Pass = false, Message = $"Student Sql execution failed with errors:{string.Join(", ", studentOutput.Messages)}" };

                    QueryExecuteRS teacherOutput = null;
                    if (result.Pass)
                    {
                        teacherOutput = _microsoftSQLService.ExecuteSQLAsAdmin(teacherSolutionSql, ats.InputNameOnServer);
                        if (teacherOutput.Messages.Any())
                            result = new EvaluationResult() { Pass = false, Message = $"Failed to execute solution script: '{string.Join(", ", teacherOutput.Messages)}'" };
                    }
                    #endregion

                    #region Invoke_New_Jobs
                    if (result.Pass)
                    {
                        _queue.QueueAsyncTask(() => EvaluateAnalysisInput(_serviceScopeFactory, jobId, testId, studentId, studentOutput, teacherOutput, solutionFormat));
                        analysisRepository.SaveProgress(ats, AnalysisEvaluationType.FailingInput, EvaluationProgress.Queued);
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type = (int)AnalysisEvaluationType.FailingInput,
                            status = (int)EvaluationProgress.Queued
                        });

                        _queue.QueueAsyncTask(() => EvaluateAnalysisOutput(_serviceScopeFactory, jobId, testId, studentId, studentOutput, AnalysisEvaluationType.QueryOutput, solutionFormat));
                        analysisRepository.SaveProgress(ats, AnalysisEvaluationType.QueryOutput, EvaluationProgress.Queued);
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type = (int)AnalysisEvaluationType.QueryOutput,
                            status = (int)EvaluationProgress.Queued
                        });

                        _queue.QueueAsyncTask(() => EvaluateAnalysisOutput(_serviceScopeFactory, jobId, testId, studentId, teacherOutput, AnalysisEvaluationType.CorrectOutput, solutionFormat));
                        analysisRepository.SaveProgress(ats, AnalysisEvaluationType.CorrectOutput, EvaluationProgress.Queued);
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type = (int)AnalysisEvaluationType.CorrectOutput,
                            status = (int)EvaluationProgress.Queued
                        });
                    }
                    #endregion

                    #region Save_And_CleanUp
                    var saveResultSuccess = analysisRepository.SaveProgress(ats, type, result.PassStatus, result.Message);
                    await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                    {
                        id = studentId,
                        type = (int)AnalysisEvaluationType.PrepareData,
                        status = result.Pass ? (int)EvaluationProgress.Passed : (int)EvaluationProgress.Failed
                    });
                    var deleteSuccess = _microsoftSQLService.DeleteDatabaseIfExists(cloneDataNameOnServer);

                    if (!deleteSuccess)
                        throw new AnalysisEvaluationException(type, testId, studentId, $"Delete database '{cloneDataNameOnServer}' failed");

                    if (!saveResultSuccess)
                        throw new AnalysisEvaluationException(type, testId, studentId, "Failed to save evaluation result");
                    #endregion
                }
                catch (Exception ex)
                {
                    var logRepository = scope.ServiceProvider.GetService<ILogRepository>();
                }
            }
        }

        [NonAction]
        private async Task EvaluateAnalysisInput(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, int studentId, QueryExecuteRS studentQueryOutput, QueryExecuteRS teacherQueryOutput, IEnumerable<string> solutionFormat)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {
                    var type = AnalysisEvaluationType.FailingInput;

                    var analysisRepository = scope.ServiceProvider.GetService<IAnalysisRepository>();
                    var ats = analysisRepository.GetEvaluationData(testId, studentId);

                    #region Record_Start
                    var setStatusSuccess = analysisRepository.SaveProgress(ats, type, EvaluationProgress.Evaluating);
                    if (setStatusSuccess)
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type,
                            status = (int)EvaluationProgress.Evaluating
                        });
                    else
                        throw new AnalysisEvaluationException(type, testId, studentId, $"Failed to set status to {EvaluationProgress.Evaluating}");
                    #endregion

                    var result = _evaluatorService.EvaluateQueryOutputs(studentQueryOutput, teacherQueryOutput, solutionFormat, false);

                    #region Record_End
                    var saveResultSuccess = analysisRepository.SaveProgress(ats, type, result.PassStatus, result.Message);
                    if (saveResultSuccess)
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type,
                            status = (int)(result.Pass ? EvaluationProgress.Passed : EvaluationProgress.Failed)
                        });

                    if (!saveResultSuccess)
                        throw new AnalysisEvaluationException(type, testId, studentId, "Failed to save evaluation result");
                    #endregion
                }
                catch (Exception ex)
                {
                    var logRepository = scope.ServiceProvider.GetService<ILogRepository>();
                }
            }
        }

        [NonAction]
        private async Task EvaluateAnalysisOutput(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, int studentId, QueryExecuteRS actualOutput, AnalysisEvaluationType type, IEnumerable<string> solutionFormat)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {
                    var analysisRepository = scope.ServiceProvider.GetService<IAnalysisRepository>();
                    var ats = analysisRepository.GetEvaluationData(testId, studentId);

                    #region Record_Start
                    var setStatusSuccess = analysisRepository.SaveProgress(ats, type, EvaluationProgress.Evaluating);
                    if (setStatusSuccess)
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type,
                            status = (int)EvaluationProgress.Evaluating
                        });
                    else
                        throw new AnalysisEvaluationException(type, testId, studentId, $"Failed to set status to {EvaluationProgress.Evaluating}");
                    #endregion

                    #region Prepare_Data
                    var mainDbName = _config.GetValue<string>("DbAdminConnection:DbName");
                    var providedOutputTableName = "";
                    switch (type)
                    {
                        case AnalysisEvaluationType.QueryOutput:
                            providedOutputTableName = ats.StudentOutputNameOnServer;
                            break;
                        case AnalysisEvaluationType.CorrectOutput:
                            providedOutputTableName = ats.TeacherOutputNameOnServer;
                            break;
                        default:
                            throw new AnalysisEvaluationException(type, testId, studentId, $"Invalid type of test '{type}'");
                    }
                    #endregion

                    #region Evaluate
                    var providedOutput = _microsoftSQLService.ReadTable(mainDbName, "dbo", providedOutputTableName);
                    if (providedOutput.Messages.Any())
                        throw new AnalysisEvaluationException(type, testId, studentId, $"Failed to read provided output table '{providedOutputTableName}'");

                    var result = _evaluatorService.EvaluateQueryOutputs(providedOutput, actualOutput, solutionFormat, true);
                    #endregion

                    #region Record_End
                    var saveResultSuccess = analysisRepository.SaveProgress(ats, type, result.PassStatus, result.Message);
                    if (saveResultSuccess)
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type,
                            status = (int)(result.Pass ? EvaluationProgress.Passed : EvaluationProgress.Failed)
                        });

                    if (!saveResultSuccess)
                        throw new AnalysisEvaluationException(type, testId, studentId, "Failed to save evaluation result");
                    #endregion

                }
                catch (Exception ex)
                {
                    var logRepository = scope.ServiceProvider.GetService<ILogRepository>();
                }
            }
        }
    }
}
