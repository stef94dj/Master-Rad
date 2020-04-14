using MasterRad.Attributes;
using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.Entities;
using MasterRad.Models;
using MasterRad.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : BaseController
    {
        private readonly IAnalysisRepository _analysisRepository;

        public AnalysisController(IAnalysisRepository analysisRepository)
        {
            _analysisRepository = analysisRepository;
        }

        [AjaxMsGraphProxy]
        [HttpGet, Route("get")]
        public ActionResult<IEnumerable<AnalysisTestEntity>> GetTests()
            => Ok(_analysisRepository.Get());

        [HttpPost, Route("create/test")]
        public ActionResult<bool> CreateTest([FromBody] AnalysisCreateRQ body)
           => _analysisRepository.Create(body, UserId);

        [HttpPost, Route("update/name")]
        public ActionResult<Result<bool>> UpdateTestName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            var testExists = _analysisRepository.TestExists(body.Name);
            if (testExists)
                return Result<bool>.Fail($"Test '{body.Name}' already exists.");

            var success = _analysisRepository.UpdateName(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("status/next")]
        public ActionResult<bool> GoToNextStatus([FromBody] UpdateDTO body)
            => _analysisRepository.StatusNext(body, UserId);
    }
}
