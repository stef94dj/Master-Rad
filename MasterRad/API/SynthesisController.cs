using MasterRad.Attributes;
using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using MasterRad.DTO.RS.TableRow;
using MasterRad.Entities;
using MasterRad.Models;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.ProfessorOrStudent)]
    public class SynthesisController : BaseController
    {
        private readonly ISynthesisRepository _synthesisRepo;
        private readonly IMsGraph _msGraph;

        public SynthesisController
        (
            ISynthesisRepository synthesisRepository,
            IMsGraph msGraph
        )
        {
            _synthesisRepo = synthesisRepository;
            _msGraph = msGraph;
        }

        [AjaxMsGraphProxy]
        [HttpGet, Route("get")]
        [Authorize(Roles = UserRole.Professor)]
        [Obsolete]
        public async Task<ActionResult<IEnumerable<SynthesisTestDTO>>> GetTestsAsync()
        {
            var entities = _synthesisRepo.Get();

            #region Get_CreatedBy_Users_Details
            var createdByIds = entities.Select(e => e.CreatedBy);
            var createdByDetails = await _msGraph.GetStudentsByIds(createdByIds);
            #endregion

            #region Map_Result
            var res = entities.Select(entity =>
            {
                var createdByDetail = createdByDetails.Single(ud => ud.MicrosoftId == entity.CreatedBy);
                return new SynthesisTestDTO(entity, createdByDetail);
            });
            #endregion

            return Ok(res);
        }

        [AjaxMsGraphProxy]
        [HttpPost, Route("Search")]
        [Authorize(Roles = UserRole.Professor)]
        [Obsolete]
        public async Task<ActionResult<PageRS<SynthesisTestDTO>>> GetTestsAsync([FromBody] SearchPaginatedRQ body)
        {
            var entities = _synthesisRepo.GetPaginated(body, out int pageCnt, out int pageNo);

            #region Get_CreatedBy_Users_Details
            var createdByIds = entities.Select(e => e.CreatedBy);
            var createdByDetails = await _msGraph.GetStudentsByIds(createdByIds);
            #endregion

            #region Map_Result
            var res = entities.Select(entity =>
            {
                var createdByDetail = createdByDetails.Single(ud => ud.MicrosoftId == entity.CreatedBy);
                return new SynthesisTestDTO(entity, createdByDetail);
            });
            #endregion

            return new PageRS<SynthesisTestDTO>(res, pageCnt, pageNo);
        }

        [HttpPost, Route("create/test")]
        [Authorize(Roles = UserRole.Professor)]
        public ActionResult<Result<bool>> CreateTest([FromBody] SynthesisCreateRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            if (body.TaskId <= 0)
                return Result<bool>.Fail("A task must be selected.");

            var testExists = _synthesisRepo.TestExists(body.Name);
            if (testExists)
                return Result<bool>.Fail($"Test '{body.Name}' already exists.");

            var success = _synthesisRepo.Create(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("update/name")]
        [Authorize(Roles = UserRole.Professor)]
        public ActionResult<Result<bool>> UpdateTestName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            var testExists = _synthesisRepo.TestExists(body.Name);
            if (testExists)
                return Result<bool>.Fail($"Test '{body.Name}' already exists.");

            var success = _synthesisRepo.UpdateName(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("status/next")]
        [Authorize(Roles = UserRole.Professor)]
        public ActionResult<bool> GoToNextStatus([FromBody] UpdateDTO body)
            => _synthesisRepo.StatusNext(body, UserId);

        [HttpGet, Route("Solution/Format/{testId}")]
        [Authorize(Roles = UserRole.Student)]
        public ActionResult<IEnumerable<string>> GetSolutionFormat([FromRoute] int testId)
            => Ok(_synthesisRepo.GetSolutionFormat(testId));

        [HttpPost, Route("delete/test")]
        [Authorize(Roles = UserRole.Professor)]
        public ActionResult<SynthesisTestEntity> DeleteTest([FromBody] DeleteDTO body)
        {
            _synthesisRepo.Delete(body);
            return Ok();
        }

        [HttpPost, Route("Submit/Answer")]
        [Authorize(Roles = UserRole.Student)]
        public ActionResult<byte[]> SubmitAnswer([FromBody] AnswerSynthesisRQ body)
        {
            if (!_synthesisRepo.IsAssigned(UserId, body.TestId))
                return Unauthorized();

            return _synthesisRepo.SubmitAnswer(body.TestId, UserId, body.TimeStamp, body.SqlScript);
        }
    }
}
