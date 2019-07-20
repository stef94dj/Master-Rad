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
                Login = _config.GetSection("DbConnection:AdminLogin").Value,
                Password = _config.GetSection("DbConnection:AdminPassword").Value
            };

            var result = _microsoftSQLService.ExecuteSQL(body.SQLQuery, connParams);

            return Ok(result);
        }

        [HttpPost, Route("Database/Create")]
        public ActionResult CreateDatabase([FromBody] QueryExecuteRQ body)
        {
            throw new Exception("Test exception");

            var connParams = new ConnectionParams()
            {
                DbName = body.DatabaseName,
                Login = _config.GetSection("DbConnection:Login").Value,
                Password = _config.GetSection("DbConnection:Password").Value
            };

            var sql = @"CREATE DATABASE [NovaBaza] CONTAINMENT = NONE ON  PRIMARY  ( NAME = N'NovaBaza', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\NovaBaza.mdf', SIZE = 270336KB, MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB ) LOG ON ( NAME = N'NovaBaza_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\NovaBaza_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )";

            var result = _microsoftSQLService.ExecuteSQL(sql, connParams); //body.SQLQuery

            return Ok(result);
        }
    }
}
