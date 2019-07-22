﻿using MasterRad.DTOs;
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

        [HttpPost, Route("insert/{dbName}/{tableName}")]
        public ActionResult<Result<bool>> InsertRecord([FromBody] DataCreateRQ body)
        {
            var connParams = new ConnectionParams()
            {
                DbName = body.DatabaseName,
                Login = _config.GetSection("DbAdminConnection:Login").Value,
                Password = _config.GetSection("DbAdminConnection:Password").Value
            };

            //var userName = string.Empty; //_profileService.GetUserName(token); 
            //var tableName = $"{body.TableName}_{userName}";

            var res = _microsoftSQLService.InsertRecord(body.TableName, body.ValuesNew, connParams);
            return Ok(res);
        }

        [HttpPost, Route("update/{dbName}/{tableName}")]
        public ActionResult<Result<bool>> UpdateRecord([FromBody] DataUpdateRQ body)
        {
            var connParams = new ConnectionParams()
            {
                DbName = body.DatabaseName,
                Login = _config.GetSection("DbAdminConnection:Login").Value,
                Password = _config.GetSection("DbAdminConnection:Password").Value
            };

            //var userName = string.Empty; //_profileService.GetUserName(token); 
            //var tableName = $"{body.TableName}_{userName}";

            var res = _microsoftSQLService.UpdateRecord(body.TableName, body.ValuesNew, body.ValuesPrevious, connParams);
            return Ok(res);
        }

        [HttpPost, Route("delete")]
        public ActionResult<Result<bool>> DeleteRecord([FromBody] DataDeleteRQ body)
        {
            var connParams = new ConnectionParams()
            {
                DbName = body.DatabaseName,
                Login = _config.GetSection("DbAdminConnection:Login").Value,
                Password = _config.GetSection("DbAdminConnection:Password").Value
            };

            //var userName = string.Empty; //_profileService.GetUserName(token); 
            //var tableName = $"{body.TableName}_{userName}";

            var res = _microsoftSQLService.DeleteRecord(body.TableName, body.Values, connParams);
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
