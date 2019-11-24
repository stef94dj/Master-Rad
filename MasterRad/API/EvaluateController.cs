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

            var nameOnServer = "NOT IMPLEMENTED - clone of task instance";

            //exec students sql
            var studentResult = _microsoftSQLService.ExecuteSQLAsAdmin(sts.SynthesisPaper.SqlScript, nameOnServer);

            //execute teachers sql
            var teacherResult = _microsoftSQLService.ExecuteSQLAsAdmin(sts.SynthesisTest.Task.SolutionSqlScript, nameOnServer);

            //get solution format
            var solutionFormat = sts.SynthesisTest.Task.SolutionColumns.Select(sc => sc.ColumnName);

            //evaluate
            var res = _evaluatorService.EvaluateSynthesisPaper(studentResult, teacherResult, solutionFormat);
            return Ok(res);
        }

        [HttpGet, Route("analysis/{id}")]
        public ActionResult<Result<bool>> EvaluateAnalysisPaper([FromRoute] int id)
        {
            var res = _evaluatorService.EvaluateAnalysisPaper(id);
            return Ok(res);
        }
    }
}
