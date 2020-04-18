using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using System.Linq;
using MasterRad.Models;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.ProfessorOrStudent)]
    public class QueryController : BaseController
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IConfiguration _config;

        public QueryController(IMicrosoftSQL microsoftSQLService, IConfiguration config)
        {
            _microsoftSQLService = microsoftSQLService;
            _config = config;
        }

        [HttpPost, Route("execute")]
        public ActionResult<Result<QueryExecuteRS>> Execute([FromBody] QueryExecuteRQ body)
        {
            var connParams = _microsoftSQLService.GetAdminConnParams(body.DatabaseName);

            var scriptExeRes = _microsoftSQLService.ExecuteSQL(body.SQLQuery, connParams);

            if (scriptExeRes.Messages.Any())
                return Result<QueryExecuteRS>.Fail(scriptExeRes.Messages);

            return Result<QueryExecuteRS>.Success(scriptExeRes);
        }
    }
}
