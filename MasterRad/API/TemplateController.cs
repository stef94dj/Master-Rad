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
            if (string.IsNullOrEmpty(body.Name))
                return Ok(Result<bool>.Fail($"Name cannot be empty."));

            var templateExists = _templateRepo.TemplateExists(body.Name);
            if (templateExists)
                return Ok(Result<bool>.Fail($"Template '{body.Name}' already exists."));

            var newDbName = NameHelper.TemplateName();
            var alreadyRegistered = _templateRepo.DatabaseRegisteredAsTemplate(newDbName);
            if (alreadyRegistered)
                return Ok(Result<bool>.Fail($"Generated name is already used for another template. Please try again."));

            var existsOnSqlServer = _microsoftSQLService.DatabaseExists(newDbName);
            if (existsOnSqlServer)
                return Ok(Result<bool>.Fail($"Generated name is not unique. Please try again."));

            var dbCreateSuccess = _microsoftSQLService.CreateDatabase(newDbName);
            if (!dbCreateSuccess)
                return Ok(Result<bool>.Fail($"Failed to create databse '{newDbName}' on database server"));

            var success = _templateRepo.Create(body.Name, newDbName);
            if (success)
                return Ok(Result<bool>.Success(true));
            else
                return Ok(Result<bool>.Fail("Failed to save changes."));
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var success = _templateRepo.UpdateDescription(body);
            if (success)
                return Ok(Result<bool>.Success(true));
            else
                return Ok(Result<bool>.Fail("Failed to save changes."));
        }

        [HttpPost, Route("Update/Name")]
        public ActionResult UpdateName([FromBody] UpdateNameRQ body)
        {
            if (string.IsNullOrEmpty(body.Name))
                return Ok(Result<bool>.Fail($"Template name cannot be empty."));

            var templateExists = _templateRepo.TemplateExists(body.Name);
            if (templateExists)
                return Ok(Result<bool>.Fail($"Template '{body.Name}' already exists in the system"));

            var success = _templateRepo.UpdateName(body);
            if (success)
                return Ok(Result<bool>.Success(true));
            else
                return Ok(Result<bool>.Fail("Failed to save changes."));
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
