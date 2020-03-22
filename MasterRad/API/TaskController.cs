using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Models.DTOs;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Professor")]
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


        //Authenthication required by default on every endpoint (Startup.cs RequireAuthenticatedUser)
        //[AllowAnonymous] - disable authenthication requirement on specific endpoint

        //Authorisation:
        //[Authorize(Roles = "Professor")][Authorize(Roles = "Student")] - Require both roles
        //[Authorize(Roles = "Professor,Student")]- Require either role
        [HttpGet, Route("Get")]
        public ActionResult<IEnumerable<TaskEntity>> GetTasks()
            => _taskRepo.Get(); //var isTeacher = User.IsInRole("Professor"); var isStudent = User.IsInRole("Student");

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

            var success = _taskRepo.Create(body, newDbName);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult<Result<bool>> UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var success = _taskRepo.UpdateDescription(body);
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

            var success = _taskRepo.UpdateName(body);
            if (success)
                return Result<bool>.Success(true);
            else
                return Result<bool>.Fail("Failed to save changes.");
        }

        [HttpPost, Route("Update/Solution")]
        public ActionResult<TaskEntity> UpdateSolution([FromBody] UpdateTaskSolutionRQ body)
            => _taskRepo.UpdateSolution(body);
    }
}
