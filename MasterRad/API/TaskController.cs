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

            var nameOnServer = "AdventureWorks2017";
            try
            {
                //iz diplomskog
                throw new NotImplementedException($"clone from database with name {templateEntity.NameOnServer} to a db with prefix of user and 'Task'");
                //nameOnServer = templateEntity.NameOnServer, User.Name + "Task" + body.Name;
                //_microsoftSQLService.CloneDatabase(templateEntity.NameOnServer, nameOnServer)
            }
            catch (NotImplementedException){}


            var result = _taskRepo.Create(body, nameOnServer);
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
            var nameOnServer = "AdventureWorks2017";
            try
            {
                //iz diplomskog
                throw new NotImplementedException("clone from database with name {templateEntity.NameOnServer} to a db with prefix of user and 'Task'");
                //nameOnServer = templateEntity.NameOnServer, User.Name + "Task" + body.Name;
                //_microsoftSQLService.CloneDatabase(templateEntity.NameOnServer, nameOnServer)
            }
            catch (NotImplementedException) { }
            //check if database created, if failed return with error

            try
            {
                throw new NotImplementedException("TRY Delete existing databasse");
            }
            catch (NotImplementedException) { }
            //if delete failed, log database not deleted

            var result = _taskRepo.UpdateTemplate(body, nameOnServer);
            return Ok(result);
        }
    }
}
