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
        public ActionResult<IEnumerable<string>> GetDatabaseNames([FromHeader] string token)
        {
            //var tables = _profileService.GetDatabaseNames(token);

            return Ok(new List<string>() { "tableName1", "tableName2", "tableName3" });
        }

        [HttpGet, Route("tables/{dbName}")]
        public ActionResult<IEnumerable<string>> GetTableNames([FromHeader] string token, [FromRoute] string dbName)
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

        [HttpGet, Route("columns/{dbName}/{tableName}")]
        public ActionResult<IEnumerable<ColumnInfo>> GetColumnsData([FromHeader] string token, [FromRoute] string dbName, [FromRoute] string tableName)
        {
            //var conn = _profileService.GetConnectionsParams(token, dbName)
            var conn = new ConnectionParams()
            {
                DbName = dbName,
                Login = "sa",
                Password = "Tnation2019"
            };

            var result = _microsoftSQLService.GetColumnsData(tableName, conn);
            return Ok(result);
        }
    }
}
