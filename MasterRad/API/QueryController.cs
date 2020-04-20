using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using System.Linq;
using MasterRad.Models;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using MasterRad.Repositories;
using MasterRad.Models.Configuration;
using Microsoft.Extensions.Options;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.ProfessorOrStudent)]
    public class QueryController : BaseUserMapController
    {
        private readonly IMicrosoftSQL _microsoftSQLService;

        public QueryController
        (
            IMicrosoftSQL microsoftSQLService,
            IUserRepository userRepo,
            IOptions<SqlServerAdminConnection> adminConnectionConf
        ): base(userRepo, adminConnectionConf)
        {
            _microsoftSQLService = microsoftSQLService;
        }

        [HttpPost, Route("execute")]
        public ActionResult<Result<QueryExecuteRS>> Execute([FromBody] QueryExecuteRQ body)
        {
            var conn = GetSqlConnection(body.DatabaseName);
            var sqlRes = _microsoftSQLService.ExecuteSQL(body.SQLQuery, conn);

            #region Build_Response
            if (sqlRes.NoMessages)
                return Result<QueryExecuteRS>.Success(sqlRes);
            else
                return Result<QueryExecuteRS>.Fail(sqlRes.Messages);
            #endregion
        }
    }
}
