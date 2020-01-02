using MasterRad.DTOs;
using MasterRad.Entities;
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

        [HttpGet, Route("Solution/Format/{testId}")]
        public ActionResult<IEnumerable<string>> GetSolutionFormat([FromRoute] int testId)
            => Ok(_synthesisRepository.GetSolutionFormat(testId));

        [HttpPost, Route("create/test")]
        public ActionResult<bool> CreateTest([FromBody] SynthesisCreateRQ body)
            => Ok(_synthesisRepository.Create(body));

        [HttpPost, Route("delete/test")]
        public ActionResult<SynthesisTestEntity> DeleteTest([FromBody] DeleteDTO body)
        {
            _synthesisRepository.Delete(body);
            return Ok();
        }

        [HttpPost, Route("update/name")]
        public ActionResult<bool> UpdateTestName([FromBody] UpdateNameRQ body)
            => Ok(_synthesisRepository.UpdateName(body));

        [HttpPost, Route("status/next")]
        public ActionResult<bool> GoToNextStatus([FromBody] UpdateDTO body)
            => Ok(_synthesisRepository.StatusNext(body));

        [HttpPost, Route("Submit/Answer")]
        public ActionResult<SynthesisPaperEntity> SubmitAnswer([FromBody] AnswerSynthesisRQ body)
        {
            if (!_userService.IsAssigned(body.TestId))
                return Unauthorized();

            if (!_synthesisRepository.HasAnswer(body.TestId, _userService.UserId))
                return Ok(_synthesisRepository.SubmitAnswer(body.TestId, _userService.UserId, body.SqlScript));
            else
                return Ok(_synthesisRepository.UpdateAnswer(body.SynthesisPaperId, body.SynthesisPaperTimeStamp, body.SqlScript));
        }
    }
}
