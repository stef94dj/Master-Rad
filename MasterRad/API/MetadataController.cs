using MasterRad.DTOs;
using MasterRad.Models;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetadataController : ControllerBase
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        //private readonly IProfile _profileService;

        public MetadataController(IMicrosoftSQL microsoftSQLService)
        {
            _microsoftSQLService = microsoftSQLService;
            //_profileService = profileService;
        }

        [HttpGet, Route("databases")]
        public ActionResult<IEnumerable<string>> GetDatabaseNames()
        {
            //var tables = _profileService.GetDatabaseNames(token);

            return Ok(new List<string>() { "tableName1", "tableName2", "tableName3" });
        }

        [HttpGet, Route("tables/{dbName}")]
        public ActionResult<IEnumerable<string>> GetTableNames([FromRoute] string dbName)
        {
            var conn = new ConnectionParams()
            {
                DbName = dbName,
                Login = "sa",
                Password = "Tnation2019"
            };

            var tableNames = _microsoftSQLService.GetTableNames(conn);

            //var username = "rY"; //_profileService.GetUserName(token);

            //var result = tableNames.Where(tn => tn.ToLower().EndsWith(username.ToLower()));

            return Ok(tableNames);
        }

        [HttpGet, Route("columns/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<IEnumerable<ColumnInfo>> GetColumnsData([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            //var conn = _profileService.GetConnectionsParams(token, dbName)
            var conn = new ConnectionParams()
            {
                DbName = dbName,
                Login = "sa",
                Password = "Tnation2019"
            };

            var result = _microsoftSQLService.GetColumnsData(schemaName, tableName, conn);
            return Ok(result);
        }

        [HttpGet, Route("identity_columns/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<IEnumerable<string>> GetIdentityColumns([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            var conn = new ConnectionParams()
            {
                DbName = dbName,
                Login = "sa",
                Password = "Tnation2019"
            };

            var result = _microsoftSQLService.GetIdentityColumns(schemaName, tableName, conn);
            return Ok(result);
        }

        [HttpGet, Route("constraints/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<IEnumerable<ColumnInfo>> GetConstraintData([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            var conn = new ConnectionParams()
            {
                DbName = dbName,
                Login = "sa",
                Password = "Tnation2019"
            };

            var result = _microsoftSQLService.GetConstraintData(schemaName, tableName, conn);
            return Ok(result);
        }

        [HttpGet, Route("explore/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<TableInfoRS> ExploreTable ([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            var res = new TableInfoRS($"{schemaName}.{tableName}");
            var conn = new ConnectionParams()
            {
                DbName = dbName,
                Login = "sa",
                Password = "Tnation2019"
            };

            res.Columns = _microsoftSQLService.GetColumnsData(schemaName, tableName, conn).ToList();
            res.Constraints = _microsoftSQLService.GetConstraintData(schemaName, tableName, conn).ToList();

            return Ok(res);
        }
    }
}
