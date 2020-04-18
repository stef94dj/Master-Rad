using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MasterRad.Controllers
{
    [Authorize(Roles = UserRole.Professor)]
    public class TaskController : BaseController
    {
        private readonly ITaskRepository _taskRepo;

        public TaskController(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
        }

        public IActionResult ModifyTaskData(int taskId)
        {
            var taskEntity = _taskRepo.Get(taskId);

            var vm = new ModifyDatabaseVM()
            {
                DatabaseName = $"Task '{taskEntity.Name}'",
                NameOnServer = taskEntity.NameOnServer
            };

            return View("~/Views/Shared/ModifyDatabaseView.cshtml", vm);
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
