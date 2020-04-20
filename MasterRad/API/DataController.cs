using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.Models;
using MasterRad.Models.Configuration;
using MasterRad.Repositories;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Linq;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.ProfessorOrStudent)]
    public class DataController : BaseUserMapController
    {
        private readonly IMicrosoftSQL _microsoftSQLService;

        public DataController
        (
            IMicrosoftSQL microsoftSQLService, 
            IUserRepository userRepo,
            IOptions<SqlServerAdminConnection> adminConnectionConf
        ) : base(userRepo, adminConnectionConf)
        {
            _microsoftSQLService = microsoftSQLService;
        }

        [HttpPost, Route("insert")]
        public ActionResult<Result<bool>> InsertRecord([FromBody] DataCreateRQ body)
        {
            var conn = GetSqlConnection(body.DatabaseName);
            return _microsoftSQLService.InsertRecord(body.SchemaName, body.TableName, body.ValuesNew, conn);
        }

        [HttpPost, Route("update")]
        public ActionResult<Result<bool>> UpdateRecord([FromBody] DataUpdateRQ body)
        {
            var conn = GetSqlConnection(body.DatabaseName);

            var count = _microsoftSQLService.Count(body.SchemaName, body.TableName, body.ValuesUnmodified, conn);
            if (count != 1)
                return Result<bool>.Fail($"The change would affect {count} records.");

            return _microsoftSQLService.UpdateRecord(body.SchemaName, body.TableName, body.ValueNew, body.ValuesUnmodified, conn);
        }

        [HttpPost, Route("delete")]
        public ActionResult<Result<bool>> DeleteRecord([FromBody] DataDeleteRQ body)
        {
            var conn = GetSqlConnection(body.DatabaseName);
            return _microsoftSQLService.DeleteRecord(body.SchemaName, body.TableName, body.ValuesUnmodified, conn);
        }

        [HttpGet, Route("read/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<TableDTO> ReadTable([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            var conn = GetSqlConnection(dbName);
            var queryResult = _microsoftSQLService.ReadTable(schemaName, tableName, conn);
            return queryResult.Tables.Single();
        }
    }
}
