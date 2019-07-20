using MasterRad.DTOs;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly Context _dbContext;

        public TestController(IMicrosoftSQL microsoftSQLService, Context dbContext)
        {
            _microsoftSQLService = microsoftSQLService;
            _dbContext = dbContext;
        }

        [HttpPost, Route("msSqlServer/login/create")]
        public ActionResult<string> CreateLogin([FromBody] TestCreateLoginRQ body)
        {
            _microsoftSQLService.CreateSQLServerUser(body.Username);

            return "test completed";
        }

        [HttpPost, Route("msSqlServer/login/assign")]
        public ActionResult<string> AssignLogin([FromBody] TestAssignLoginRQ body)
        {
            _microsoftSQLService.AssignSQLServerUserToDb(body.Username, body.DbName);

            return "test completed";
        }

        [HttpPost, Route("msSqlServer/login/delete")]
        public ActionResult<string> DeleteLogin([FromBody] TestDeleteLoginRQ body)
        {
            _microsoftSQLService.DeleteSQLServerUser(body.Username);

            return "test completed";
        }

    }
}
