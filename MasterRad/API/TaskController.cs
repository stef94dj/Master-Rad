using MasterRad.DTOs;
using MasterRad.Entities;
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
        {
            var result = _taskRepo.Get();
            return Ok(result);
        }

        [HttpPost, Route("Create")]
        public ActionResult CreateTask([FromBody] CreateTaskRQ body)
        {
            var templateEntity = _templateRepo.Get(body.TemplateId);

            var taskNameOnServer = $"Task_{body.Name}".Replace("\t", "_").Replace(" ", "_");
            var cloneSuccess = _microsoftSQLService.CloneDatabase(templateEntity.NameOnServer, taskNameOnServer);
            if (!cloneSuccess)
                return Ok(new JsonResult(new { Message = "Unable to clone database" }));

            var result = _taskRepo.Create(body, taskNameOnServer);
            return Ok(result);
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var result = _taskRepo.UpdateDescription(body);
            return Ok(result);
        }

        [HttpPost, Route("Update/Name")]
        public ActionResult UpdateName([FromBody] UpdateNameRQ body)
        {
            var result = _taskRepo.UpdateName(body);
            return Ok(result);
        }

        [HttpPost, Route("Update/Solution")]
        public ActionResult UpdateSolution([FromBody] UpdateTaskSolutionRQ body)
        {
            var result = _taskRepo.UpdateSolution(body);
            return Ok(result);
        }
    }
}
