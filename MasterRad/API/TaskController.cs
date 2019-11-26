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
            => Ok(_taskRepo.Get());

        [HttpPost, Route("Create")]
        public ActionResult CreateTask([FromBody] CreateTaskRQ body)
        {
            var templateEntity = _templateRepo.Get(body.TemplateId);

            var taskNameOnServer = $"Task_{body.Name}".Replace("\t", "_").Replace(" ", "_");
            var cloneSuccess = _microsoftSQLService.CloneDatabase(templateEntity.NameOnServer, taskNameOnServer, true);
            if (!cloneSuccess)
                return Ok(new JsonResult(new { Message = "Unable to clone database" }));

            var result = _taskRepo.Create(body, taskNameOnServer);
            return Ok(result);
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult UpdateDescription([FromBody] UpdateDescriptionRQ body)
            => Ok(_taskRepo.UpdateDescription(body));

        [HttpPost, Route("Update/Name")]
        public ActionResult UpdateName([FromBody] UpdateNameRQ body)
            => Ok(_taskRepo.UpdateName(body));

        [HttpPost, Route("Update/Solution")]
        public ActionResult UpdateSolution([FromBody] UpdateTaskSolutionRQ body)
            => Ok(_taskRepo.UpdateSolution(body));
    }
}
