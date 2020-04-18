using MasterRad.Models.ViewModels;
using MasterRad.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MasterRad.Controllers
{
    [Authorize(Roles = UserRole.Professor)]
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
