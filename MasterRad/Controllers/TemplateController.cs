using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MasterRad.Models;
using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using MasterRad.DTO;
using MasterRad.Extensions;

namespace MasterRad.Controllers
{
    public class TemplateController : BaseController
    {
        private readonly ITemplateRepository _templateRepo;

        public TemplateController(ITemplateRepository templateRepo)
        {
            _templateRepo = templateRepo;
        }

        public IActionResult Model(int templateId)
        {
            var templateEntity = _templateRepo.Get(templateId);

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
            var templateEntity = _templateRepo.Get(templateId);

            var vm = new ModifyDatabaseVM()
            {
                DatabaseName = $"Template '{templateEntity.Name}'",
                NameOnServer = templateEntity.NameOnServer
            };

            return View("~/Views/Shared/ModifyDatabaseView.cshtml", vm);
        }
    }
}
