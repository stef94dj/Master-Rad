using MasterRad.DTOs;
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

        [HttpGet, Route("synthesis/{synthesisTestStudentId}")]
        public ActionResult<Result<bool>> EvaluateSynthesisPaper([FromRoute] int synthesisTestStudentId)
        {
            var sts = _synthesisRepository.GetEvaluationData();

            //get solution format
            var solutionFormat = sts.SynthesisTest.Task.SolutionColumns.Select(sc => sc.ColumnName);

            var templateDatabaseNameOnServer = "clone template db";
            //exec students sql
            var studentTemplateResult = _microsoftSQLService.ExecuteSQLAsAdmin(sts.SynthesisPaper.SqlScript, templateDatabaseNameOnServer);
            //execute teachers sql
            var teacherTemplateResult = _microsoftSQLService.ExecuteSQLAsAdmin(sts.SynthesisTest.Task.SolutionSqlScript, templateDatabaseNameOnServer);
            //evaluate
            var templateEvalRes = _evaluatorService.EvaluateSynthesisPaper(studentTemplateResult, teacherTemplateResult, solutionFormat);

            var taskDatabaseNameOnServer = "clone task db";
            //exec students sql
            var studentTaskResult = _microsoftSQLService.ExecuteSQLAsAdmin(sts.SynthesisPaper.SqlScript, taskDatabaseNameOnServer);
            //execute teachers sql
            var tacherTaskResult = _microsoftSQLService.ExecuteSQLAsAdmin(sts.SynthesisTest.Task.SolutionSqlScript, taskDatabaseNameOnServer);
            //evaluate
            var taskEvalRes = _evaluatorService.EvaluateSynthesisPaper(studentTaskResult, tacherTaskResult, solutionFormat);

            //store evaluate results

            throw new NotImplementedException();
            return Ok(true);
        }

        [HttpGet, Route("analysis/{id}")]
        public ActionResult<Result<bool>> EvaluateAnalysisPaper([FromRoute] int id)
        {
            var res = _evaluatorService.EvaluateAnalysisPaper(id);
            return Ok(res);
        }
    }
}
