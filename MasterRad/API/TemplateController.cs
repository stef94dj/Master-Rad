using MasterRad.Entities;
using MasterRad.Models;
using MasterRad.Models.DTOs;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IDbTemplateRepository _templateRepo;

        public TemplateController(
            IMicrosoftSQL microsoftSQLService,
            IDbTemplateRepository templateRepo
        )
        {
            _microsoftSQLService = microsoftSQLService;
            _templateRepo = templateRepo;
        }

        [HttpGet, Route("Get")]
        public ActionResult GetTemplates()
        {
            var result = _templateRepo.Get();
            return Ok(result);
        }

        [HttpPost, Route("Create")]
        public ActionResult CreateTemplate([FromBody] CreateTemplateRQ body)
        {
            var result = _templateRepo.Create(body.Name);
            return Ok(result);
        }

        [HttpPost, Route("Update/Description")]
        public ActionResult UpdateDescription([FromBody] UpdateDescriptionRQ body)
        {
            var result = _templateRepo.UpdateDescription(body);
            return Ok(result);
        }

        [HttpPost, Route("Update/Name")]
        public ActionResult UpdateName([FromBody] UpdateNameRQ body)
        {
            var result = _templateRepo.UpdateName(body);
            return Ok(result);
        }

        [HttpPost, Route("Update/SqlScript")]
        public ActionResult<Result<DbTemplateEntity>> UpdateSqlScript([FromBody] SetSqlScriptRQ body)
        {
            _microsoftSQLService.ExecuteSQLAsAdmin(body.SqlScript);
            //var creatingDatabases = new List<string>() { "DatabaseNameaoidaiosdowqd" }; //queryService.GetCreatingDatabases();
            //if (creatingDatabases.Count() != 1)
            //    return Ok(Result<DbTemplateEntity>.Fail($"The script should create exactly 1 database. Detected creating {creatingDatabases.Count()} databases."));

            //var dbName = creatingDatabases.Single();

            var dbName = body.DbName;

            var existsInDatabase = _templateRepo.DatabaseExists(dbName);
            if (existsInDatabase)
                return Ok(Result<DbTemplateEntity>.Fail($"Database name '{dbName}' already exists in the system"));

            var existsOnSqlServer = _microsoftSQLService.DatabaseExists(dbName);
            if (existsOnSqlServer)
                return Ok(Result<DbTemplateEntity>.Fail($"Database name '{dbName}' already exists on the database server"));

            var createResult = _microsoftSQLService.CreateDatabaseFromScript(dbName, body.SqlScript);
            if (!createResult.IsSuccess())
                return Ok(Result<DbTemplateEntity>.Fail(createResult.Errors));

            var result = _templateRepo.UpdateSqlScript(body, dbName);
            return Ok(result);
        }
    }
}
