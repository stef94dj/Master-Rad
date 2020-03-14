using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Models.DTOs;
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
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepo;
        private readonly ITemplateRepository _templateRepo;
        private readonly IMicrosoftSQL _microsoftSQLService;

        public TaskController(
            ITaskRepository taskRepo,
            ITemplateRepository templateRepo,
            IMicrosoftSQL microsoftSQLService
        )
        {
            _taskRepo = taskRepo;
            _templateRepo = templateRepo;
            _microsoftSQLService = microsoftSQLService;
        }

        [HttpGet, Route("Get")]
        public ActionResult GetTasks()
            => Ok(_taskRepo.Get());

        [HttpPost, Route("Create")]
        public ActionResult CreateTask([FromBody] CreateTaskRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Ok(Result<bool>.Fail("Name cannot be empty."));

            if (body.TemplateId <= 0)
                return Ok(Result<bool>.Fail("A template must be selected."));

            var taskExists = _taskRepo.TaskExists(body.Name);
            if (taskExists)
                return Ok(Result<bool>.Fail($"Task '{body.Name}' already exists."));

            var newDbName = NameHelper.TaskName();

            var alreadyRegistered = _taskRepo.DatabaseRegisteredAsTask(newDbName);
            if (alreadyRegistered)
                return Ok(Result<bool>.Fail($"Generated name is already used for another task. Please try again."));

            var existsOnSqlServer = _microsoftSQLService.DatabaseExists(newDbName);
            if (existsOnSqlServer)
                return Ok(Result<bool>.Fail($"Generated name is not unique. Please try again."));

            var templateEntity = _templateRepo.Get(body.TemplateId);
            var cloneSuccess = _microsoftSQLService.CloneDatabase(templateEntity.NameOnServer, newDbName, true);
            if (!cloneSuccess)
                return Ok(Result<bool>.Fail($"Failed to clone database '{templateEntity.NameOnServer}' into '{newDbName}'."));

            var success = _taskRepo.Create(body, newDbName);
            if (success)
                return Ok(Result<bool>.Success(true));
            else
                return Ok(Result<bool>.Fail("Failed to save changes."));
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var success = _taskRepo.UpdateDescription(body);
            if (success)
                return Ok(Result<bool>.Success(true));
            else
                return Ok(Result<bool>.Fail("Failed to save changes."));
        }

        [HttpPost, Route("Update/Name")]
        public ActionResult UpdateName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Ok(Result<bool>.Fail("Name cannot be empty."));

            var taskExists = _taskRepo.TaskExists(body.Name);
            if (taskExists)
                return Ok(Result<bool>.Fail($"Task '{body.Name}' already exists."));

            var success = _taskRepo.UpdateName(body);
            if (success)
                return Ok(Result<bool>.Success(true));
            else
                return Ok(Result<bool>.Fail("Failed to save changes."));
        }

        [HttpPost, Route("Update/Solution")]
        public ActionResult UpdateSolution([FromBody] UpdateTaskSolutionRQ body)
            => Ok(_taskRepo.UpdateSolution(body));
    }
}
