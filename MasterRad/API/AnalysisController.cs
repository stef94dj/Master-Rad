using MasterRad.Attributes;
using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using MasterRad.DTO.RS.TableRow;
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
    [Authorize(Roles = UserRole.Professor)]
    public class AnalysisController : BaseController
    {
        private readonly IAnalysisRepository _analysisRepo;
        private readonly IMsGraph _msGraph;

        public AnalysisController
        (
            IAnalysisRepository analysisRepository,
            IMsGraph msGraph
        )
        {
            _analysisRepo = analysisRepository;
            _msGraph = msGraph;
        }

        [AjaxMsGraphProxy]
        [HttpPost, Route("Search")]
        public async Task<ActionResult<PageRS<AnalysisTestDTO>>> GetTestsAsync([FromBody] SearchPaginatedRQ body)
        {
            var entities = _analysisRepo.GetPaginated(body, out int pageCnt, out int pageNo);

            #region Get_CreatedBy_Users_Details
            var createdByIds = entities.Select(e => e.CreatedBy);
            var createdByDetails = await _msGraph.GetStudentsByIds(createdByIds);
            #endregion

            #region Get_Student_Users_Details
            var studentIds = entities.Select(e => e.STS_StudentId);
            var studentDetails = await _msGraph.GetStudentsByIds(studentIds);
            #endregion

            #region Map_Result
            var res = entities.Select(entity =>
            {
                var createdByDetail = createdByDetails.Single(ud => ud.MicrosoftId == entity.CreatedBy);
                var studentDetail = studentDetails.Single(ud => ud.MicrosoftId == entity.STS_StudentId);
                return new AnalysisTestDTO(entity, createdByDetail, studentDetail);
            });
            #endregion

            return new PageRS<AnalysisTestDTO>(res, pageCnt, pageNo);
        }

        [HttpPost, Route("create/test")]
        public ActionResult<bool> CreateTest([FromBody] AnalysisCreateRQ body)
           => _analysisRepo.Create(body, UserId);

        [HttpPost, Route("update/name")]
        public ActionResult<Result<bool>> UpdateTestName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            var testExists = _analysisRepo.TestExists(body.Name);
            if (testExists)
                return Result<bool>.Fail($"Test '{body.Name}' already exists.");

            var success = _analysisRepo.UpdateName(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("status/next")]
        public ActionResult<Result<bool>> GoToNextStatus([FromBody] UpdateDTO body)
        {
            var success = _analysisRepo.StatusNext(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Delete")]
        public ActionResult<Result<bool>> DeleteAnalysisTest([FromBody] DeleteEntityRQ body)
        {
            var entity = _analysisRepo.Get(body.Id);

            if (entity.Status > TestStatus.Scheduled)
            {
                var statusText = entity.Status.ToString().ToLower();
                return Result<bool>.Fail($"Unable to delete. Delete of '{statusText}' test is not allowed");
            }

            var assignedCount = _analysisRepo.AssignedStudentsCount(body.Id);
            if (assignedCount > 0)
                return Result<bool>.Fail($"Unable to delete. A total of {assignedCount} students are assigned to this test.");

            var success = _analysisRepo.DeleteTest(body.Id, body.TimeStamp);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to delete record.");
        }
    }
}
