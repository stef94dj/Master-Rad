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
    public class SetupController : Controller
    {
        private readonly IDbTemplateRepository _dbTemplateRepo;
        private readonly ITaskRepository _taskRepo;

        public SetupController(
            IDbTemplateRepository dbTemplateRepo,
            ITaskRepository taskRepo
            )
        {
            _dbTemplateRepo = dbTemplateRepo;
            _taskRepo = taskRepo;
        }

        public IActionResult Model(int templateId)
        {
            var templateEntity = _dbTemplateRepo.Get(templateId);

            var vm = new TemplateModelVM() //AutoMapper
            {
                Id = templateEntity.Id,
                TimeStamp = Convert.ToBase64String(templateEntity.TimeStamp),
                TemplateName = templateEntity.Name,
                DatabaseName = templateEntity.NameOnServer
            };

            return View(vm);
        }

        public IActionResult ModifyTemplateData(int templateId)
        {
            var templateEntity = _dbTemplateRepo.Get(templateId);

            var vm = new ModifyDataVM()
            {
                DatabaseName = $"Template '{templateEntity.Name}'",
                NameOnServer = templateEntity.NameOnServer
            };

            return View("ModifyData", vm);
        }

        public IActionResult ModifyTaskData(int taskId)
        {
            var taskEntity = _taskRepo.Get(taskId);

            var vm = new ModifyDataVM()
            {
                DatabaseName = $"Task '{taskEntity.Name}'",
                NameOnServer = taskEntity.NameOnServer
            };

            return View("ModifyData", vm);
        }

        public IActionResult Templates()
        {
            return View();
        }

        public IActionResult Tasks()
        {
            return View();
        }
    }
}
