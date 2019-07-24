using MasterRad.DTOs;
using MasterRad.Entities;
using MasterRad.Models;
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
    public class TemplateController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IConfiguration _config;
        private readonly IDbTemplateRepository _dbTemplateRepo;

        public TemplateController(
            IMicrosoftSQL microsoftSQLService,
            IConfiguration config,
            IDbTemplateRepository dbTemplateRepo
        )
        {
            _microsoftSQLService = microsoftSQLService;
            _config = config;
            _dbTemplateRepo = dbTemplateRepo;
        }

        [HttpGet, Route("Get")]
        public ActionResult GetTemplates()
        {
            var result = _dbTemplateRepo.Templates();
            return Ok(result);
        }

        [HttpPost, Route("Create")]
        public ActionResult CreateTemplate([FromBody] DatabaseCreateRQ body)
        {
            var result = _dbTemplateRepo.Create(body.Name);
            return Ok(result);
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var result = _dbTemplateRepo.UpdateDescription(body);
            return Ok(result);
        }

        [HttpPost, Route("Update/Name")]
        public ActionResult UpdateName([FromBody] UpdateNameRQ body)
        {
            var result = _dbTemplateRepo.UpdateName(body);
            return Ok(result);
        }

        [HttpPost, Route("Update/SqlScript")]
        public ActionResult<Result<DbTemplateEntity>> UpdateSqlScript([FromBody] SetSqlScriptRQ body)
        {
            var creatingDatabases = new List<string>() { "DatabaseNameaoidaiosdowqd" }; //queryService.GetCreatingDatabases();
            if (creatingDatabases.Count() != 1)
                return Ok(Result<DbTemplateEntity>.Fail($"The script should create exactly 1 database. Detected creating {creatingDatabases.Count()} databases."));

            var dbName = creatingDatabases.Single();

            var existsInDatabase = _dbTemplateRepo.DatabaseExists(dbName);
            if (existsInDatabase)
                return Ok(Result<DbTemplateEntity>.Fail($"Database name '{dbName}' already exists in the system"));

            var existsOnSqlServer = _microsoftSQLService.DatabaseExists(dbName);
            if (existsOnSqlServer)
                return Ok(Result<DbTemplateEntity>.Fail($"Database name '{dbName}' already exists on the database server"));

            var createResult = _microsoftSQLService.CreateDatabaseFromScript(dbName, body.SqlScript);
            if (!createResult.IsSuccess())
                return Ok(Result<DbTemplateEntity>.Fail(createResult.Errors));

            var result = _dbTemplateRepo.UpdateSqlScript(body, dbName);
            return Ok(result);
        }
    }
}
