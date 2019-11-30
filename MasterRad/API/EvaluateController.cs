using MasterRad.DTOs;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
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

        public EvaluateController
        (
            IEvaluator evaluatorService,
            ISynthesisRepository synthesisRepository,
            IMicrosoftSQL microsoftSQLService
        )
        {
            _evaluatorService = evaluatorService;
            _synthesisRepository = synthesisRepository;
            _microsoftSQLService = microsoftSQLService;
        }

        [HttpPost]
        public ActionResult<Result<bool>> EvaluateSynthesisPaper([FromBody] EvaluateSynthesisTest model)
        {
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
    }
}
