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

        public SetupController(IDbTemplateRepository dbTemplateRepo)
        {
            _dbTemplateRepo = dbTemplateRepo;
        }

        public IActionResult Database(int templateId)
        {
            var templateEntity = _dbTemplateRepo.Get(templateId);

            var vm = new TemplateSqlScriptVM() //AutoMapper
            {
                Id = templateEntity.Id,
                TimeStamp = Convert.ToBase64String(templateEntity.TimeStamp),
                TemplateName = templateEntity.Name,
                SqlScript = templateEntity?.SqlScript ?? string.Empty,
            };

            return View(vm);
        }

        public IActionResult ModifyData(int templateId)
        {
            var templateEntity = _dbTemplateRepo.Get(templateId);

            var vm = new ModifyDataVM()
            {
                TemplateName = templateEntity.Name,
                NameOnServer = templateEntity.NameOnServer
            };

            return View(vm);
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
