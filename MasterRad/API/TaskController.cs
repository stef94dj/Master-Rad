using MasterRad.Attributes;
using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using MasterRad.DTO.RS.TableRow;
using MasterRad.Entities;
using MasterRad.Helpers;
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
    public class TaskController : BaseController
    {
        private readonly ITaskRepository _taskRepo;
        private readonly ITemplateRepository _templateRepo;
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IMsGraph _msGraph;

        public TaskController(
            ITaskRepository taskRepo,
            ITemplateRepository templateRepo,
            IMicrosoftSQL microsoftSQLService,
            IMsGraph msGraph
        )
        {
            _taskRepo = taskRepo;
            _templateRepo = templateRepo;
            _microsoftSQLService = microsoftSQLService;
            _msGraph = msGraph;
        }

        [AjaxMsGraphProxy]
        [HttpPost, Route("Search")]
        public async Task<ActionResult<PageRS<TaskDTO>>> GetTasksAsync([FromBody] SearchPaginatedRQ body)
        { 
            var entities = _taskRepo.GetPaginated(body, out int pageCnt, out int pageNo);

            #region Get_CreatedBy_Users_Details
            var createdByIds = entities.Select(e => e.CreatedBy);
            var createdByDetails = await _msGraph.GetStudentsByIds(createdByIds);
            #endregion

            #region Map_Result
            var res = entities.Select(te =>
            {
                var createdByDetail = createdByDetails.Single(ud => ud.MicrosoftId == te.CreatedBy);
                return new TaskDTO(te, createdByDetail);
            });
            #endregion

            return new PageRS<TaskDTO>(res, pageCnt, pageNo);
        }

        [HttpPost, Route("Create")]
        public ActionResult<Result<bool>> CreateTask([FromBody] CreateTaskRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            if (body.TemplateId <= 0)
                return Result<bool>.Fail("A template must be selected.");

            var taskExists = _taskRepo.TaskExists(body.Name);
            if (taskExists)
                return Result<bool>.Fail($"Task '{body.Name}' already exists.");

            var newDbName = NameHelper.TaskName();

            var alreadyRegistered = _taskRepo.DatabaseRegisteredAsTask(newDbName);
            if (alreadyRegistered)
                return Result<bool>.Fail($"Generated name is already used for another task. Please try again.");

            var existsOnSqlServer = _microsoftSQLService.DatabaseExists(newDbName);
            if (existsOnSqlServer)
                return Result<bool>.Fail($"Generated name is not unique. Please try again.");

            var templateEntity = _templateRepo.Get(body.TemplateId);
            var cloneSuccess = _microsoftSQLService.CloneDatabase(templateEntity.NameOnServer, newDbName, true);
            if (!cloneSuccess)
                return Result<bool>.Fail($"Failed to clone database '{templateEntity.NameOnServer}' into '{newDbName}'.");

            var success = _taskRepo.Create(body, newDbName, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult<Result<bool>> UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var success = _taskRepo.UpdateDescription(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Name")]
        public ActionResult<Result<bool>> UpdateName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Result<bool>.Fail("Name cannot be empty.");

            var taskExists = _taskRepo.TaskExists(body.Name);
            if (taskExists)
                return Result<bool>.Fail($"Task '{body.Name}' already exists.");

            var success = _taskRepo.UpdateName(body, UserId);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Solution")]
        public ActionResult<TaskEntity> UpdateSolution([FromBody] UpdateTaskSolutionRQ body)
            => _taskRepo.UpdateSolution(body, UserId);
    }
}
