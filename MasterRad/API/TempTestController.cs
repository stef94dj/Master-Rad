using MasterRad.DTO.RQ;
using MasterRad.Entities;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    /*public*/ class TempTestController : BaseController
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly Context _dbContext;

        public TempTestController(IMicrosoftSQL microsoftSQLService, Context dbContext)
        {
            _microsoftSQLService = microsoftSQLService;
            _dbContext = dbContext;
        }

        [HttpPost, Route("msSqlServer/login/create")]
        public ActionResult<string> CreateLogin([FromBody] TestCreateLoginRQ body)
        {
            _microsoftSQLService.CreateSQLServerUser("", "");

            return "test completed";
        }

        [HttpGet, Route("delete/temlpate/{templateId}/{timestamp}")]
        public ActionResult<string> TestDeleteTemplate([FromRoute] int templateId, [FromRoute] byte[] timestamp)
        {
            var tmpEnt = new TemplateEntity() 
            {
                Id = templateId,
                TimeStamp = timestamp
            };

            _dbContext.Templates.Attach(tmpEnt);
            _dbContext.Templates.Remove(tmpEnt);
            _dbContext.SaveChanges();
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
