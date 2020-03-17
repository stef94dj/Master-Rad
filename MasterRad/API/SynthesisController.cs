using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Models;
using MasterRad.Models.DTOs;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SynthesisController : ControllerBase
    {
        private readonly IUser _userService;
        private readonly ISynthesisRepository _synthesisRepository;

        public SynthesisController(IUser userService, ISynthesisRepository synthesisRepository)
        {
            _userService = userService;
            _synthesisRepository = synthesisRepository;
        }

        [HttpGet, Route("get")]
        public ActionResult<IEnumerable<SynthesisTestEntity>> GetTests()
            => Ok(_synthesisRepository.Get());

        [HttpPost, Route("create/test")]
        public ActionResult<Result<bool>> CreateTest([FromBody] SynthesisCreateRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            if (body.TaskId <= 0)
                return Result<bool>.Fail("A task must be selected.");

            var testExists = _synthesisRepository.TestExists(body.Name);
            if (testExists)
                return Result<bool>.Fail($"Task '{body.Name}' already exists.");

            var success = _synthesisRepository.Create(body);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("update/name")]
        public ActionResult<Result<bool>> UpdateTestName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            var testExists = _synthesisRepository.TestExists(body.Name);
            if (testExists)
                return Result<bool>.Fail($"Task '{body.Name}' already exists.");

            var success  = _synthesisRepository.UpdateName(body);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("status/next")]
        public ActionResult<bool> GoToNextStatus([FromBody] UpdateDTO body)
            => StatusCode(500); //_synthesisRepository.StatusNext(body);


        [HttpGet, Route("Solution/Format/{testId}")]
        public ActionResult<IEnumerable<string>> GetSolutionFormat([FromRoute] int testId)
            => Ok(_synthesisRepository.GetSolutionFormat(testId));

        [HttpPost, Route("delete/test")]
        public ActionResult<SynthesisTestEntity> DeleteTest([FromBody] DeleteDTO body)
        {
            _synthesisRepository.Delete(body);
            return Ok();
        }

        [HttpPost, Route("Submit/Answer")]
        public ActionResult<byte[]> SubmitAnswer([FromBody] AnswerSynthesisRQ body)
        {
            if (!_userService.IsAssigned(body.TestId))
                return Unauthorized();

            return _synthesisRepository.SubmitAnswer(body.TestId, _userService.UserId, body.TimeStamp, body.SqlScript);
        }
    }
}
