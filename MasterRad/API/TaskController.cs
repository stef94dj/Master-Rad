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
        private readonly IDbTemplateRepository _templateRepo;
        private readonly IMicrosoftSQL _microsoftSQLService;

        public TaskController(
            ITaskRepository taskRepo,
            IDbTemplateRepository templateRepo,
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

            var taskNameOnServer = $"Tsk_{body.Name}".Replace("\t", "_").Replace(" ", "_");
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

        [HttpPost, Route("Update/Template")]
        public ActionResult UpdateTemplate([FromBody] UpdateTaskTemplateRQ body)
        {
            var templateEntity = _templateRepo.Get(body.TemplateId);
            var taskEntity = _taskRepo.Get(body.Id); //mozda dupli entitet kod update-a (u get metodi treba _context.Tasks.AsNoTracking().Where...)
            var oldTaskNameOnServer = taskEntity.NameOnServer;

            var taskNameOnServer = $"Tsk_{taskEntity.Name}".Replace("\t", "_").Replace(" ", "_");
            var deleteSuccess = _microsoftSQLService.DeleteDatabaseIfExists(taskNameOnServer);
            if (!deleteSuccess)
                return Ok(new JsonResult(new { Message = "Unable to drop current database from sql server" }));

            var cloneSuccess = _microsoftSQLService.CloneDatabase(templateEntity.NameOnServer, taskNameOnServer);
            if (!cloneSuccess)
            {
                //LOG $"Update task template failed - current database {taskNameOnServer} was remove but the system was unable to create a new one";
                return Ok(new JsonResult(new { Message = "Unable to clone database" }));
            }

            var result = _taskRepo.UpdateTemplate(body); //sta ako kloniranje uspe a ne uspe upis u bazu (task ostaje vezan na stari template)

            return Ok(result);
        }

        [HttpPost, Route("Update/Solution")]
        public ActionResult UpdateSolution([FromBody] UpdateTaskTemplateRQ body)
        {
            throw new NotImplementedException();
        }

        [HttpPost, Route("Update/Solution")]
        public ActionResult UpdateSolution([FromBody] UpdateTaskTemplateRQ body)
        {
            throw new NotImplementedException();
        }
    }
}
