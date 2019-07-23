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
    public class SchemaController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IConfiguration _config;
        private readonly IDbTemplateRepository _dbTemplateRepo;

        public SchemaController(
            IMicrosoftSQL microsoftSQLService,
            IConfiguration config,
            IDbTemplateRepository dbTemplateRepo
        )
        {
            _microsoftSQLService = microsoftSQLService;
            _config = config;
            _dbTemplateRepo = dbTemplateRepo;
        }

        [HttpGet, Route("get")]
        public ActionResult CreateDatabase()
        {
            throw new NotImplementedException();
            //return Ok(result);
        }

    }
}
