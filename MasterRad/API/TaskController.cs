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

        public TaskController(
            ITaskRepository taskRepo
        )
        {
            _taskRepo = taskRepo;
        }

        [HttpGet, Route("Get")]
        public ActionResult GetTasks()
        {
            var result = _taskRepo.Get();
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
    }
}
