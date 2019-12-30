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
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly ISignalR<JobProgressHub> _signalR;
        private readonly IQueue _queue;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EvaluateController
        (
            IEvaluator evaluatorService,
            ISynthesisRepository synthesisRepository,
            IMicrosoftSQL microsoftSQLService,
            IQueue queue,
            ISignalR<JobProgressHub> signalR,
            IServiceScopeFactory serviceScopeFactory
        )
        {
            _evaluatorService = evaluatorService;
            _synthesisRepository = synthesisRepository;
            _microsoftSQLService = microsoftSQLService;
            _queue = queue;
            _signalR = signalR;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet, Route("get/papers/synthesis/{testId}")]
        public ActionResult<IEnumerable<SynthesisTestStudentEntity>> EvaluationData([FromRoute] int testId)
            => Ok(_synthesisRepository.GetPapers(testId));

        [HttpPost, Route("Start/Evaluation/Synthesis")]
        public async Task<ActionResult> StartSynthesisEvaluationAsync([FromBody] StartTestEvalulation model)
        {
            var jobId = $"evaluate_synthesis_{model.TestId}";
            foreach (var request in model.EvaluationRequests)
            {
                _queue.QueueAsyncTask(() => EvaluateSynthesisPaper(_serviceScopeFactory, jobId, model.TestId, request.StudentId, request.UseSecretData));
                await _signalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                {
                    id = request.StudentId,
                    secret = request.UseSecretData,
                    status = (int)EvaluationProgress.Queued
                });
            }

            return Ok();
        }

        private async Task EvaluateSynthesisPaper(IServiceScopeFactory serviceScopeFactory, string jobId, int testId, int studentId, bool useSecretData)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var logRepository = scope.ServiceProvider.GetService<ILogRepository>();
                try
                {
                    var synthesisRepository = scope.ServiceProvider.GetService<ISynthesisRepository>();
                    var sts = synthesisRepository.GetEvaluationData(testId, studentId);

                    var setStatusSuccess = synthesisRepository.SetStatus(sts.SynthesisPaper, useSecretData, EvaluationProgress.Evaluating);
                    if (setStatusSuccess)
                        await _signalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                        {
                            id = studentId,
                            secret = useSecretData,
                            status = (int)EvaluationProgress.Evaluating
                        });
                    else
                        throw new SynthesisEvaluationException(testId, studentId, $"Failed to set status to {EvaluationProgress.Evaluating}");

                    var originalDataNameOnServer = useSecretData ? sts.SynthesisTest.Task.NameOnServer : sts.SynthesisTest.Task.Template.NameOnServer;
                    var cloneDataNameOnServer = DatabaseNameHelper.SynthesisTestEvaluation(studentId, testId, useSecretData);

                    var solutionScript = sts.SynthesisTest.Task.SolutionSqlScript;
                    var solutionFormat = sts.SynthesisTest.Task.SolutionColumns.Select(sc => sc.ColumnName);
                    var studentScript = sts.SynthesisPaper.SqlScript;

                    var cloneSuccess = _microsoftSQLService.CloneDatabase(originalDataNameOnServer, cloneDataNameOnServer, false);
                    if (!cloneSuccess)
                        throw new SynthesisEvaluationException(testId, studentId, $"Failed to clone database '{originalDataNameOnServer}' into '{cloneDataNameOnServer}'");

                    SynthesisEvaluationResult result;
                    var studentResult = _microsoftSQLService.ExecuteSQLAsAdmin(studentScript, cloneDataNameOnServer);
                    if (studentResult.Messages.Any())
                    {
                        result = new SynthesisEvaluationResult() { Pass = false, FailReason = $"Sql execution failed with errors:{string.Join(", ", studentResult.Messages)}" };
                    }
                    else
                    {
                        var teacherResult = _microsoftSQLService.ExecuteSQLAsAdmin(solutionScript, originalDataNameOnServer);
                        if (teacherResult.Messages.Any())
                            throw new SynthesisEvaluationException(testId, studentId, "Failed to execute solution script");

                        result = _evaluatorService.EvaluateSynthesisPaper(studentResult, teacherResult, solutionFormat);
                    }

                    var saveResultSuccess = synthesisRepository.SaveEvaluation(sts.SynthesisPaper, useSecretData, result);
                    if (saveResultSuccess)
                        await _signalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                        {
                            id = studentId,
                            secret = useSecretData,
                            status = (int)(result.Pass ? EvaluationProgress.Passed : EvaluationProgress.Failed)
                        });

                    var deleteSuccess = _microsoftSQLService.DeleteDatabaseIfExists(cloneDataNameOnServer);
                    if (!deleteSuccess)
                        throw new SynthesisEvaluationException(testId, studentId, $"Delete database {cloneDataNameOnServer} failed");

                    if (!saveResultSuccess)
                        throw new SynthesisEvaluationException(testId, studentId, "Failed to save evaluation result");
                }
                catch(Exception ex)
                {
                    logRepository.Log(ex);
                }
            }
        }

        [HttpGet, Route("analysis/{id}")]
        public ActionResult<Result<bool>> EvaluateAnalysisPaper([FromRoute] int id)
        {
            var res = _evaluatorService.EvaluateAnalysisPaper(id);
            return Ok(res);
        }
    }
}
