using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterRad.DTOs;
using MasterRad.Models;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MasterRad.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IConfiguration _config;

        public QueryController(IMicrosoftSQL microsoftSQLService, IConfiguration config)
        {
            _microsoftSQLService = microsoftSQLService;
            _config = config;
        }

        [HttpPost, Route("execute")]
        public ActionResult Execute([FromBody] QueryExecuteRQ body)
        {
            var connParams = new ConnectionParams()
            {
                DbName = body.DatabaseName,
                Login = _config.GetSection("DbAdminConnection:Login").Value,
                Password = _config.GetSection("DbAdminConnection:Password").Value
            };

            var result = _microsoftSQLService.ExecuteSQL(body.SQLQuery, connParams);

            return Ok(result);
        }

        
    }
}
