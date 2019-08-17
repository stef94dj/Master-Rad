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
        {
            var res = _synthesisRepository.Get();
            return Ok(res);
        }

        [HttpGet, Route("Solution/Format/{testId}")]
        public ActionResult<IEnumerable<string>> GetSolutionFormat([FromRoute] int testId)
        {
            var res = _synthesisRepository.GetSolutionFormat(testId);
            return Ok(res);
        }

        [HttpPost, Route("create/test")]
        public ActionResult<SynthesisTestEntity> CreateTest([FromBody] SynthesisCreateRQ body)
        {

            try
            {
                throw new NotImplementedException();
                //clone DB and return name on server FOR EACH STUDENT
                //pass the list of pairs (nameOnServer, studentId) to _synthesisRepository.Create
            }
            catch (Exception) { };

            var res = _synthesisRepository.Create(body);
            return Ok(res);
        }

        [HttpPost, Route("delete/test")]
        public ActionResult<SynthesisTestEntity> DeleteTest([FromBody] DeleteDTO body)
        {
            _synthesisRepository.Delete(body);
            return Ok();
        }

        [HttpPost, Route("update/name")]
        public ActionResult<SynthesisTestEntity> UpdateTestName([FromBody] UpdateNameRQ body)
        {
            var res = _synthesisRepository.UpdateName(body);
            return Ok(res);
        }

        [HttpPost, Route("status/next")]
        public ActionResult<SynthesisTestEntity> GoToNextStatus([FromBody] UpdateDTO body)
        {
            var res = _synthesisRepository.StatusNext(body);
            return Ok(res);
        }

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
