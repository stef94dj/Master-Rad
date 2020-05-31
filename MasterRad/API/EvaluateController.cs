using Coravel.Queuing.Interfaces;
using MasterRad.Attributes;
using MasterRad.DTO;
using MasterRad.DTO.RS;
using MasterRad.DTO.RS.TableRow;
using MasterRad.Exceptions;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Models.Configuration;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.Professor)]
    public class EvaluateController : BaseController
    {
        private readonly IEvaluator _evaluatorService;
        private readonly ISynthesisRepository _synthesisRepo;
        private readonly IAnalysisRepository _analysisRepo;
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly ISignalR<SynthesisProgressHub> _synthesisSignalR;
        private readonly ISignalR<AnalysisProgressHub> _analysisSignalR;
        private readonly IQueue _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SqlServerAdminConnection _sqlServerAdmin;
        private readonly IMsGraph _msGraph;

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
            IOptions<SqlServerAdminConnection> sqlServerAdmin,
            IMsGraph msGraph
        )
        {
            _evaluatorService = evaluatorService;
            _synthesisRepo = synthesisRepository;
            _analysisRepo = analysisRepository;
            _microsoftSQLService = microsoftSQLService;
            _queue = queue;
            _synthesisSignalR = synthesisSignalR;
            _analysisSignalR = analysisSignalR;
            _serviceScopeFactory = serviceScopeFactory;
            _sqlServerAdmin = sqlServerAdmin.Value;
            _msGraph = msGraph;
        }

        #region Synthesis
        [AjaxMsGraphProxy]
        [HttpGet, Route("get/papers/synthesis/{testId}")]
        public async Task<ActionResult<IEnumerable<SynthesisEvaluationItemDTO>>> SynthesisEvaluationDataAsync([FromRoute] int testId)
        {
            var entities = _synthesisRepo.GetPapers(testId);

            #region Get_Student_Details
            var studentIds = entities.Select(e => e.StudentId);
            var studentDetails = await _msGraph.GetStudentsByIds(studentIds);
            #endregion

            #region Map_Result
            var res = entities.Select(entity =>
            {
                var studentDetail = studentDetails.Single(ud => ud.MicrosoftId == entity.StudentId);
                return new SynthesisEvaluationItemDTO(entity, studentDetail);
            });
            #endregion

            return Ok(res);
        }

        [HttpPost, Route("Start/Evaluation/Synthesis")]
        public async Task<ActionResult> StartSynthesisEvaluationAsync([FromBody] StartTestEvalulationSynthesis model)
        {
            var userId = UserId;
            var jobId = $"evaluate_synthesis_{model.TestId}";
            foreach (var request in model.EvaluationRequests)
            {
                _queue.QueueAsyncTask(() => EvaluateSynthesisPaper(_serviceScopeFactory, jobId, model.TestId, request.StudentId, request.UseSecretData, userId));

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
        private async Task EvaluateSynthesisPaper(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, Guid studentId, bool useSecretData, Guid userId)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {

                    var synthesisRepository = scope.ServiceProvider.GetService<ISynthesisRepository>();
                    var sts = synthesisRepository.GetEvaluationData(testId, studentId);

                    #region Record_Start
                    var setStatusSuccess = synthesisRepository.SaveProgress(sts, useSecretData, EvaluationProgress.Evaluating, userId);
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
                    var taskEntity = sts.SynthesisTest.Task;
                    var dbNameOnServer = useSecretData ? taskEntity.NameOnServer : taskEntity.Template.NameOnServer;
                    var solutionScript = taskEntity.SolutionSqlScript;
                    var solutionFormat = taskEntity.SolutionColumns.Select(sc => sc.ColumnName);
                    var studentScript = sts.SqlScript;
                    #endregion

                    #region Evaluate
                    EvaluationResult result = null;

                    var studentResult = _microsoftSQLService.ExecuteSQLAsReadOnlyAdmin(studentScript, dbNameOnServer);
                    if (studentResult.Messages.Any())
                        result = new EvaluationResult() { Pass = false, Message = $"Student Sql execution failed with errors:'{string.Join(", ", studentResult.Messages)}'" };

                    if (result == null)
                    {
                        var teacherResult = _microsoftSQLService.ExecuteSQLAsReadOnlyAdmin(solutionScript, dbNameOnServer);
                        if (teacherResult.Messages.Any())
                            throw new SynthesisEvaluationException(useSecretData, testId, studentId, $"Failed to execute solution script: '{string.Join(", ", teacherResult.Messages)}'");

                        result = _evaluatorService.EvaluateQueryOutputs(studentResult, teacherResult, solutionFormat, true);
                    }
                    #endregion

                    #region Signal_End_And_Save_Result
                    var saveResultSuccess = synthesisRepository.SaveProgress(sts, useSecretData, result.PassStatus, userId, result.Message);
                    if (saveResultSuccess)
                        await _synthesisSignalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                        {
                            id = studentId,
                            secret = useSecretData,
                            status = (int)(result.Pass ? EvaluationProgress.Passed : EvaluationProgress.Failed)
                        });

                    if (!saveResultSuccess)
                        throw new SynthesisEvaluationException(useSecretData, testId, studentId, "Failed to save evaluation result");
                    #endregion
                }
                catch (Exception ex)
                {
                    var logRepo = scope.ServiceProvider.GetService<ILogRepository>();
                    logRepo.Log(ex);
                }
            }
        }

        [HttpGet, Route("get/diff/synthesis/{testId}/{studentId}")]
        public ActionResult GetSynthesisDiff([FromRoute] int testId, [FromRoute] Guid studentId)
        {
            var cols = new List<string>();
            var cells = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                cols.Add($"col {i}");
                cells.Add($"val {i}");
            }
            var res = new
            {
                columns = cols.ToArray(),
                row = cells.ToArray()
            };
            return Ok(res);
        }
        #endregion

        #region Analysis
        [AjaxMsGraphProxy]
        [HttpGet, Route("get/papers/analysis/{testId}")]
        public async Task<ActionResult<IEnumerable<AnalysisEvaluationItemDTO>>> AnalysisEvaluationDataAsync([FromRoute] int testId)
        {
            var entities = _analysisRepo.GetPapers(testId);

            #region Get_CreatedBy_Users_Details
            var studentIds = entities.Select(e => e.StudentId);
            var studentDetails = await _msGraph.GetStudentsByIds(studentIds);
            #endregion

            #region Get_Student_Details
            var res = entities.Select(entity =>
            {
                var studentDetail = studentDetails.Single(ud => ud.MicrosoftId == entity.StudentId);
                return new AnalysisEvaluationItemDTO(entity, studentDetail);
            });
            #endregion

            return Ok(res);
        }

        [HttpPost, Route("Start/Evaluation/Analysis")]
        public async Task<ActionResult> StartAnalysisEvaluationAsync([FromBody] StartTestEvalulationAnalysis model)
        {
            var userId = UserId;
            var jobId = $"evaluate_analysis_{model.TestId}";
            foreach (var studentId in model.StudentIds)
            {
                _queue.QueueAsyncTask(() => EvaluateAnalysisPaper(_serviceScopeFactory, jobId, model.TestId, studentId, userId));

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
        private async Task EvaluateAnalysisPaper(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, Guid studentId, Guid userId)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {
                    var type = AnalysisEvaluationType.PrepareData;

                    var analysisRepository = scope.ServiceProvider.GetService<IAnalysisRepository>();
                    var ats = analysisRepository.GetEvaluationData(testId, studentId);

                    #region Record_Start
                    var setStatusSuccess = analysisRepository.SaveProgress(ats, type, EvaluationProgress.Evaluating, userId);
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

                    #region Get_Outputs
                    EvaluationResult result = new EvaluationResult() { Pass = true, Message = "Data successfully prepared." };

                    var studentOutput = _microsoftSQLService.ExecuteSQLAsReadOnlyAdmin(studentSolutionSql, ats.InputNameOnServer);
                    if (studentOutput.Messages.Any())
                        result = new EvaluationResult() { Pass = false, Message = $"Student Sql execution failed with errors:{string.Join(", ", studentOutput.Messages)}" };

                    QueryExecuteRS teacherOutput = null;
                    if (result.Pass)
                    {
                        teacherOutput = _microsoftSQLService.ExecuteSQLAsReadOnlyAdmin(teacherSolutionSql, ats.InputNameOnServer);
                        if (teacherOutput.Messages.Any())
                            result = new EvaluationResult() { Pass = false, Message = $"Failed to execute solution script: '{string.Join(", ", teacherOutput.Messages)}'" };
                    }
                    #endregion

                    #region Invoke_New_Jobs
                    if (result.Pass)
                    {
                        _queue.QueueAsyncTask(() => EvaluateAnalysisInput(_serviceScopeFactory, jobId, testId, studentId, userId, studentOutput, teacherOutput, solutionFormat));
                        analysisRepository.SaveProgress(ats, AnalysisEvaluationType.FailingInput, EvaluationProgress.Queued, userId);
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type = (int)AnalysisEvaluationType.FailingInput,
                            status = (int)EvaluationProgress.Queued
                        });

                        _queue.QueueAsyncTask(() => EvaluateAnalysisOutput(_serviceScopeFactory, jobId, testId, studentId, userId, studentOutput, AnalysisEvaluationType.QueryOutput, solutionFormat));
                        analysisRepository.SaveProgress(ats, AnalysisEvaluationType.QueryOutput, EvaluationProgress.Queued, userId);
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type = (int)AnalysisEvaluationType.QueryOutput,
                            status = (int)EvaluationProgress.Queued
                        });

                        _queue.QueueAsyncTask(() => EvaluateAnalysisOutput(_serviceScopeFactory, jobId, testId, studentId, userId, teacherOutput, AnalysisEvaluationType.CorrectOutput, solutionFormat));
                        analysisRepository.SaveProgress(ats, AnalysisEvaluationType.CorrectOutput, EvaluationProgress.Queued, userId);
                        await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                        {
                            id = studentId,
                            type = (int)AnalysisEvaluationType.CorrectOutput,
                            status = (int)EvaluationProgress.Queued
                        });
                    }
                    #endregion

                    #region Signal_End_And_Save_Result
                    var saveResultSuccess = analysisRepository.SaveProgress(ats, type, result.PassStatus, userId, result.Message);
                    await _analysisSignalR.SendMessageAsync(jobId, "analysisEvaluationUpdate", new
                    {
                        id = studentId,
                        type = (int)AnalysisEvaluationType.PrepareData,
                        status = result.Pass ? (int)EvaluationProgress.Passed : (int)EvaluationProgress.Failed
                    });

                    if (!saveResultSuccess)
                        throw new AnalysisEvaluationException(type, testId, studentId, "Failed to save evaluation result");
                    #endregion
                }
                catch (Exception ex)
                {
                    var logRepo = scope.ServiceProvider.GetService<ILogRepository>();
                    logRepo.Log(ex);
                }
            }
        }

        [NonAction]
        private async Task EvaluateAnalysisInput(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, Guid studentId, Guid userId, QueryExecuteRS studentQueryOutput, QueryExecuteRS teacherQueryOutput, IEnumerable<string> solutionFormat)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {
                    var type = AnalysisEvaluationType.FailingInput;

                    var analysisRepository = scope.ServiceProvider.GetService<IAnalysisRepository>();
                    var ats = analysisRepository.GetEvaluationData(testId, studentId);

                    #region Record_Start
                    var setStatusSuccess = analysisRepository.SaveProgress(ats, type, EvaluationProgress.Evaluating, userId);
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
                    var saveResultSuccess = analysisRepository.SaveProgress(ats, type, result.PassStatus, userId, result.Message);
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
                    var logRepo = scope.ServiceProvider.GetService<ILogRepository>();
                    logRepo.Log(ex);
                }
            }
        }

        [NonAction]
        private async Task EvaluateAnalysisOutput(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, Guid studentId, Guid userId, QueryExecuteRS actualOutput, AnalysisEvaluationType type, IEnumerable<string> solutionFormat)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {
                    var analysisRepository = scope.ServiceProvider.GetService<IAnalysisRepository>();
                    var ats = analysisRepository.GetEvaluationData(testId, studentId);

                    #region Record_Start
                    var setStatusSuccess = analysisRepository.SaveProgress(ats, type, EvaluationProgress.Evaluating, userId);
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
                    var conn = _microsoftSQLService.GetReadOnlyAdminConnParams(_sqlServerAdmin.OutputTablesDbName);
                    var providedOutput = _microsoftSQLService.ReadTable("dbo", providedOutputTableName, conn);
                    if (providedOutput.Messages.Any())
                        throw new AnalysisEvaluationException(type, testId, studentId, $"Failed to read provided output table '{providedOutputTableName}'");

                    var result = _evaluatorService.EvaluateQueryOutputs(providedOutput, actualOutput, solutionFormat, true);
                    #endregion

                    #region Signal_End_And_Save_Result
                    var saveResultSuccess = analysisRepository.SaveProgress(ats, type, result.PassStatus, userId, result.Message);
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
                    var logRepo = scope.ServiceProvider.GetService<ILogRepository>();
                    logRepo.Log(ex);
                }
            }
        }
        #endregion
    }
}
