using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MasterRad.Models;
using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using MasterRad.DTOs;
using MasterRad.Extensions;

namespace MasterRad.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepo;

        public TaskController(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        public IActionResult ModifyTaskData(int taskId)
        {
            var taskEntity = _taskRepo.Get(taskId);

            var vm = new ModifyDataVM()
            {
                DatabaseName = $"Task '{taskEntity.Name}'",
                NameOnServer = taskEntity.NameOnServer
            };

            return View("~/Views/Shared/ModifyDataView.cshtml", vm);
        }

        public IActionResult ModifyTaskSolution(int taskId)
        {
            var taskEntity = _taskRepo.Get(taskId);

            var vm = new TaskSolutionVM()
            {
                Id = taskEntity.Id,
                TimeStamp = Convert.ToBase64String(taskEntity.TimeStamp),
                TaskName = taskEntity.Name,
                NameOnServer = taskEntity.NameOnServer,
                Solution = taskEntity.SolutionSqlScript
            };

            return View("TaskSolution", vm);
        }
    }
}
