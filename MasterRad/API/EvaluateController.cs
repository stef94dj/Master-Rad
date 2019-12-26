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

        public EvaluateController
        (
            IEvaluator evaluatorService,
            ISynthesisRepository synthesisRepository,
            IMicrosoftSQL microsoftSQLService,
            IQueue queue,
            ISignalR<JobProgressHub> signalR
        )
        {
            _evaluatorService = evaluatorService;
            _synthesisRepository = synthesisRepository;
            _microsoftSQLService = microsoftSQLService;
            _queue = queue;
            _signalR = signalR;
        }

        [HttpGet, Route("get/papers/synthesis/{testId}")]
        public ActionResult<IEnumerable<SynthesisTestStudentEntity>> EvaluationData([FromRoute] int testId)
            => Ok(_synthesisRepository.GetPapers(testId));

        private async Task EvaluateSynthesisPaper(string jobId, int testId, int studentId, bool useSecretData)
        {
            var sts = _synthesisRepository.GetEvaluationData(testId, studentId);

            var setStatusSuccess = _synthesisRepository.SetStatus(sts.SynthesisPaper.Id, useSecretData, EvaluationProgress.Evaluating);
            if (setStatusSuccess)
                await _signalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                {
                    id = studentId,
                    secret = false,
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

            var saveSuccess = _synthesisRepository.SaveEvaluation(sts.SynthesisPaper.Id, useSecretData, result);
            if (saveSuccess)
                await _signalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                {
                    id = studentId,
                    secret = false,
                    status = (int)(result.Pass ? EvaluationProgress.Passed : EvaluationProgress.Passed)
                });

            var deleteSuccess = _microsoftSQLService.DeleteDatabaseIfExists(cloneDataNameOnServer);
            if (!deleteSuccess)
                Console.WriteLine($"NOT implemented: log delete database {cloneDataNameOnServer} failed");

            if (!saveSuccess)
                throw new SynthesisEvaluationException(testId, studentId, "Failed to save evaluation result");
        }

        [HttpGet, Route("analysis/{id}")]
        public ActionResult<Result<bool>> EvaluateAnalysisPaper([FromRoute] int id)
        {
            var res = _evaluatorService.EvaluateAnalysisPaper(id);
            return Ok(res);
        }

        [HttpPost, Route("Start/Evaluation/Synthesis")]
        public async Task<ActionResult<Result<bool>>> StartSynthesisEvaluationAsync([FromBody] StartTestEvalulation model)
        {
            var jobId = $"evaluate_synthesis_{model.TestId}";
            foreach (var studentId in model.StudentIds)
            {
                _queue.QueueAsyncTask(() => PerformBackgroundJob(jobId, studentId, true));

                //_queue.QueueAsyncTask(() => EvaluateSynthesisPaper(jobId, model.TestId, studentId, true));
                //_queue.QueueAsyncTask(() => EvaluateSynthesisPaper(jobId, model.TestId, studentId, false));

                await _signalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                {
                    id = studentId,
                    secret = true,
                    status = (int)EvaluationProgress.Queued
                });

                _queue.QueueAsyncTask(() => PerformBackgroundJob(jobId, studentId, false));
                await _signalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                {
                    id = studentId,
                    secret = false,
                    status = (int)EvaluationProgress.Queued
                });
            }

            var res = Result<bool>.Fail("");
            return Ok(res);
        }

        private async Task PerformBackgroundJob(string jobId, int studentId, bool secret)
        {
            for (int i = 0; i <= 100; i += 1)
            {
                var status = (int)EvaluationProgress.NotEvaluated;
                if (i % 2 == 0)
                    status = (int)EvaluationProgress.Evaluating;

                await _signalR.SendMessageAsync(jobId, "synthesisEvaluationUpdate", new
                {
                    id = studentId,
                    secret,
                    status
                });

                await Task.Delay(1000);
            }
        }
    }
}
