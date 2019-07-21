using MasterRad.DTOs;
using MasterRad.Models;
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
    public class DataController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IConfiguration _config;
        //private readonly IJsonHelper _jsonHelper;

        public DataController(IMicrosoftSQL microsoftSQLService, IConfiguration config)
        {
            _microsoftSQLService = microsoftSQLService;
            _config = config;
        }

        [HttpPost, Route("insert")]
        public ActionResult<Result<bool>> InsertData([FromHeader] string token, [FromBody] DataInsertRQ body)
        {
            var connParams = new ConnectionParams()
            {
                DbName = body.DatabaseName,
                Login = _config.GetSection("DbAdminConnection:Login").Value,
                Password = _config.GetSection("DbAdminConnection:Password").Value
            };

            var userName = string.Empty; //_profileService.GetUserName(token);
            var tableName = $"{body.TableName}_{userName}";

            var res = _microsoftSQLService.InsertRecord(tableName, body.DataRecord, connParams);
            return Ok(res);
        }

        [HttpGet, Route("read/{dbName}/{tableName}")]
        public ActionResult<Result<bool>> ReadTable([FromRoute] string dbName, [FromRoute] string tableName)
        {
            var res = _microsoftSQLService.ReadTable(dbName, tableName);
            return Ok(res);
        }
    }
}
