using MasterRad.Attributes;
using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using MasterRad.DTO.RS.TableRow;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.Professor)]
    public class TemplateController : BaseController
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly ITemplateRepository _templateRepo;
        private readonly ITaskRepository _taskRepo;
        private readonly IMsGraph _msGraph;

        public TemplateController
        (
            IMicrosoftSQL microsoftSQLService,
            ITemplateRepository templateRepo,
            ITaskRepository taskRepo,
            IMsGraph msGraph
        )
        {
            _microsoftSQLService = microsoftSQLService;
            _templateRepo = templateRepo;
            _taskRepo = taskRepo;
            _msGraph = msGraph;
        }

        [AjaxMsGraphProxy]
        [HttpPost, Route("Search")]
        public async Task<ActionResult<PageRS<TemplateDTO>>> GetTemplatesAsync([FromBody] SearchPaginatedRQ body)
        {
            var entities = _templateRepo.GetPaginated(body, out int pageCnt, out int pageNo);

            #region Get_CreatedBy_Users_Details
            var createdByIds = entities.Select(e => e.CreatedBy);
            var createdByDetails = await _msGraph.GetStudentsByIds(createdByIds);
            #endregion

            #region Map_Result
            var resData = entities.Select(entity =>
            {
                var createdByDetail = createdByDetails.Single(ud => ud.MicrosoftId == entity.CreatedBy);
                return new TemplateDTO(entity, createdByDetail);
            });
            #endregion

            return new PageRS<TemplateDTO>(resData, pageCnt, pageNo);
        }

        [HttpPost, Route("Create")]
        public ActionResult<Result<bool>> CreateTemplate([FromBody] CreateTemplateRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail($"Name cannot be empty.");

            var templateExists = _templateRepo.TemplateExists(body.Name);
            if (templateExists)
                return Result<bool>.Fail($"Template '{body.Name}' already exists.");

            var newDbName = NameHelper.TemplateName();
            var alreadyRegistered = _templateRepo.DatabaseRegisteredAsTemplate(newDbName);
            if (alreadyRegistered)
                return Result<bool>.Fail($"Generated name is already used for another template. Please try again.");

            var existsOnSqlServer = _microsoftSQLService.DatabaseExists(newDbName);
            if (existsOnSqlServer)
                return Result<bool>.Fail($"Generated name is not unique. Please try again.");

            var dbCreateSuccess = _microsoftSQLService.CreateDatabase(newDbName, true);
            if (!dbCreateSuccess)
                return Result<bool>.Fail($"Failed to create databse '{newDbName}' on database server");

            var success = _templateRepo.Create(body.Name, newDbName, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult<Result<bool>> UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var success = _templateRepo.UpdateDescription(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Name")]
        public ActionResult<Result<bool>> UpdateName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail($"Template name cannot be empty.");

            var templateExists = _templateRepo.TemplateExists(body.Name);
            if (templateExists)
                return Result<bool>.Fail($"Template '{body.Name}' already exists in the system");

            var success = _templateRepo.UpdateName(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Toggle/IsPublic")]
        public ActionResult<Result<bool>> UpdateIsPublic([FromBody] UpdateIsPublicRQ body)
        {
            var success = _templateRepo.UpdateIsPublic(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Model")]
        public ActionResult<Result<bool>> UpdateModel([FromBody] UpdateTemplateModelRQ body)
        {
            var templateEntity = _templateRepo.Get(body.Id);

            var scriptExeRes = _microsoftSQLService.ExecuteSQLAsAdmin(body.SqlScript, templateEntity.NameOnServer);

            if (scriptExeRes.Messages.Any())
                return Result<bool>.Fail(scriptExeRes.Messages);

            return Result<bool>.Success(true);
        }

        [HttpPost, Route("Delete")]
        public ActionResult<Result<bool>> DeleteTemplate([FromBody] DeleteEntityRQ body)
        {
            var entity = _templateRepo.Get(body.Id);

            var childCnt = _taskRepo.TemplateChildCount(body.Id);
            if (childCnt > 0)
                return Result<bool>.Fail($"Unable to delete. A total of {childCnt} tasks depend on this template.");

            var success = _microsoftSQLService.DeleteDatabaseIfExists(entity.NameOnServer);

            if (!success)
                return Result<bool>.Fail("Failed to delete database instance.");

            success = _templateRepo.DeleteTemplate(body.Id, body.TimeStamp);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed delete record.");
        }
    }
}
