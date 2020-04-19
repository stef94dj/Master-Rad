using MasterRad.DTO.RQ;
using MasterRad.DTO.RS;
using System.Linq;
using MasterRad.Models;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using MasterRad.Repositories;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.ProfessorOrStudent)]
    public class QueryController : BaseController
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IUserRepository _userRepo;

        public QueryController
        (
            IMicrosoftSQL microsoftSQLService, 
            IUserRepository userRepo
        )
        {
            _microsoftSQLService = microsoftSQLService;
            _userRepo = userRepo;
        }

        [HttpPost, Route("execute")]
        public ActionResult<Result<QueryExecuteRS>> Execute([FromBody] QueryExecuteRQ body)
        {
            QueryExecuteRS sqlRes;
            if (User.IsInRole(UserRole.Professor))
            {
                sqlRes = _microsoftSQLService.ExecuteSQLAsAdmin(body.SQLQuery, body.DatabaseName);
            }
            else
            {
                var userEntity = _userRepo.Get(UserId);
                var conn = new ConnectionParams(body.DatabaseName, userEntity.SqlUsername, userEntity.SqlPassword);
                sqlRes = _microsoftSQLService.ExecuteSQL(body.SQLQuery, conn);
            }

            #region Build_Response
            if (sqlRes.IsSelectSuccess)
                return Result<QueryExecuteRS>.Success(sqlRes);
            else
                return Result<QueryExecuteRS>.Fail(sqlRes.Messages);
            #endregion
        }
    }
}
