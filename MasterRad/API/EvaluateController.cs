using Coravel.Queuing.Interfaces;
using MasterRad.DTOs;
using MasterRad.Entities;
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
        private readonly IHubContext<JobProgressHub> _hubContext;
        private readonly IQueue _queue;

        public EvaluateController
        (
            IEvaluator evaluatorService,
            ISynthesisRepository synthesisRepository,
            IMicrosoftSQL microsoftSQLService,
            IQueue queue,
            IHubContext<JobProgressHub> hubContext
        )
        {
            _evaluatorService = evaluatorService;
            _synthesisRepository = synthesisRepository;
            _microsoftSQLService = microsoftSQLService;
            _queue = queue;
            _hubContext = hubContext;
        }

        [HttpGet, Route("get/papers/synthesis/{testId}")]
        public ActionResult<IEnumerable<SynthesisTestStudentEntity>> EvaluationData([FromRoute] int testId)
            => Ok(_synthesisRepository.GetPapers(testId));

        [HttpPost]
        public ActionResult<Result<bool>> EvaluateSynthesisPaper([FromBody] EvaluateSynthesisTest model)
        {
            //QUEUED - somwhere else
            //_synthesisRepository.SetStatus(Evaluating); _signalR.SendMessage(new {model.SynthesisTestId, model.StudentId, Evaluating})

            var sts = _synthesisRepository.GetEvaluationData(model.SynthesisTestId, model.StudentId);

            var originalDataNameOnServer = model.EvaluateWithSecretData ? sts.SynthesisTest.Task.NameOnServer : sts.SynthesisTest.Task.Template.NameOnServer;
            var cloneDataNameOnServer = DatabaseNameHelper.SynthesisTestEvaluation(model.StudentId, model.SynthesisTestId, model.EvaluateWithSecretData);

            var solutionScript = sts.SynthesisTest.Task.SolutionSqlScript;
            var solutionFormat = sts.SynthesisTest.Task.SolutionColumns.Select(sc => sc.ColumnName);
            var studentScript = sts.SynthesisPaper.SqlScript;

            var cloneSuccess = _microsoftSQLService.CloneDatabase(originalDataNameOnServer, cloneDataNameOnServer, false);
            if (!cloneSuccess)
                return Result<bool>.Fail("Unexpected error");

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
                    return Result<bool>.Fail("Unexpected error");

                result = _evaluatorService.EvaluateSynthesisPaper(studentResult, teacherResult, solutionFormat);
            }

            var saveSuccess = _synthesisRepository.SaveEvaluation(sts.SynthesisPaper.Id, model.EvaluateWithSecretData, result);
            // _signalR.SendMessage(new { model.SynthesisTestId, model.StudentId, result ? Passed : Failed })

            var deleteSuccess = _microsoftSQLService.DeleteDatabaseIfExists(cloneDataNameOnServer);
            if (!deleteSuccess)
                Console.WriteLine($"NOT implemented: log delete database {cloneDataNameOnServer} failed");

            if (saveSuccess)
                return Ok(Result<bool>.Success(true));
            else
                return Ok(Result<bool>.Fail("Unexpected error"));

        }

        [HttpGet, Route("analysis/{id}")]
        public ActionResult<Result<bool>> EvaluateAnalysisPaper([FromRoute] int id)
        {
            var res = _evaluatorService.EvaluateAnalysisPaper(id);
            return Ok(res);
        }

        [HttpPost, Route("StartProgress")]
        public ActionResult<Result<bool>> StartProgress()
        {
            string jobId = Guid.NewGuid().ToString("N");
            //for (var studentID = 100; studentID < 105; studentID++)
            //{
            //    _queue.QueueAsyncTask(() => PerformBackgroundJob("123", studentID, true));
            //    _queue.QueueAsyncTask(() => PerformBackgroundJob("123", studentID, false));
            //}

            _queue.QueueAsyncTask(() => PerformBackgroundJob("123", 101, true));
            _queue.QueueAsyncTask(() => PerformBackgroundJob("123", 101, false));

            _queue.QueueAsyncTask(() => PerformBackgroundJob("123", 102, true));
            _queue.QueueAsyncTask(() => PerformBackgroundJob("123", 102, false));

            _queue.QueueAsyncTask(() => PerformBackgroundJob("123", 103, true));
            _queue.QueueAsyncTask(() => PerformBackgroundJob("123", 103, false));

            _queue.QueueAsyncTask(() => PerformBackgroundJob("123", 104, true));
            _queue.QueueAsyncTask(() => PerformBackgroundJob("123", 104, false));

            var res = Result<bool>.Fail("");
            return Ok(res);
        }

        private async Task PerformBackgroundJob(string jobId, int studentId, bool secret)
        {
            for (int i = 0; i <= 100; i += 1)
            {
                var status = i % 10;

                if (status > (int)EvaluationProgress.Passed)
                    status = (int)EvaluationProgress.NotEvaluated;

                var progressUpdate = new
                {
                    id = studentId,
                    secret,
                    status
                };

                await _hubContext.Clients.Group(jobId).SendAsync("progress", progressUpdate);

                await Task.Delay(1000);
            }
        }
    }
}
