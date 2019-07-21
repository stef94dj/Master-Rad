using MasterRad.DTOs;
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
    public class DatabaseController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IConfiguration _config;
        private readonly IDbTemplateRepository _dbTemplateRepo;
        //private readonly IJsonHelper _jsonHelper;

        public DatabaseController(
            IMicrosoftSQL microsoftSQLService,
            IConfiguration config,
            IDbTemplateRepository dbTemplateRepo
        )
        {
            _microsoftSQLService = microsoftSQLService;
            _config = config;
            _dbTemplateRepo = dbTemplateRepo;
        }

        [HttpPost, Route("Create")]
        public ActionResult CreateDatabase([FromBody] DatabaseCreateRQ body)
        {
            //VERIFIKACIJA
            var sql = @"CREATE DATABASE [NovaBaza] CONTAINMENT = NONE ON  PRIMARY  ( NAME = N'NovaBaza', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\NovaBaza.mdf', SIZE = 270336KB, MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB ) LOG ON ( NAME = N'NovaBaza_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\NovaBaza_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )";
            var result = _microsoftSQLService.ExecuteSQLAsAdmin(sql); //body.SQLScript

            _dbTemplateRepo.Insert(body.SQLScript, "NovaBaza");
            return Ok(result);
        }

    }
}
