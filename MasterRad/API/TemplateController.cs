using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Helpers;
using MasterRad.Models;
using MasterRad.Models.DTOs;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly ITemplateRepository _templateRepo;

        public TemplateController(
            IMicrosoftSQL microsoftSQLService,
            ITemplateRepository templateRepo
        )
        {
            _microsoftSQLService = microsoftSQLService;
            _templateRepo = templateRepo;
        }

        [HttpGet, Route("Get")]
        public ActionResult GetTemplates() 
            => Ok(_templateRepo.Get());

        [HttpPost, Route("Create")]
        public ActionResult CreateTemplate([FromBody] CreateTemplateRQ body)
        {
            var dbName = DatabaseNameHelper.TemplateName(body.Name);

            var templateExists = _templateRepo.TemplateExists(body.Name);
            if (templateExists)
                return Ok(Result<TemplateEntity>.Fail($"Template '{body.Name}' already exists in the system"));

            var alreadyRegistered = _templateRepo.DatabaseRegisteredAsTemplate(dbName);
            if (alreadyRegistered)
                return Ok(Result<TemplateEntity>.Fail($"Database '{dbName}' is bound to another template"));

            var existsOnSqlServer = _microsoftSQLService.DatabaseExists(dbName);
            if (existsOnSqlServer)
                return Ok(Result<TemplateEntity>.Fail($"Database '{dbName}' already exists on database server"));

            var dbCreateSuccess = _microsoftSQLService.CreateDatabase(dbName); //CreateDatabase treba da uloguje gresku
            if (!dbCreateSuccess)
                return Ok(Result<TemplateEntity>.Fail($"Failed to create databse '{dbName}' on database server"));

            var result = _templateRepo.Create(body.Name, dbName);
            return Ok(result);
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult UpdateDescription([FromBody] UpdateDescriptionRQ body)
            => Ok(_templateRepo.UpdateDescription(body));

        [HttpPost, Route("Update/Name")]
        public ActionResult UpdateName([FromBody] UpdateNameRQ body)
        {
            var templateExists = _templateRepo.TemplateExists(body.Name);
            if (templateExists)
                return Ok(Result<TemplateEntity>.Fail($"Template '{body.Name}' already exists in the system"));

            var result = _templateRepo.UpdateName(body);
            return Ok(result);
        }

        [HttpPost, Route("Update/Model")]
        public ActionResult<Result<bool>> UpdateModel([FromBody] UpdateTemplateModelRQ body)
        {

            var templateEntity = _templateRepo.Get(body.Id);

            var scriptExeRes = _microsoftSQLService.ExecuteSQLAsAdmin(body.SqlScript, templateEntity.NameOnServer);

            if (scriptExeRes.Messages.Any())
                return Result<bool>.Fail(scriptExeRes.Messages);

            return Ok(Result<bool>.Success(true));
        }
    }
}
