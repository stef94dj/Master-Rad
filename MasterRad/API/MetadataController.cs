using MasterRad.DTO;
using MasterRad.DTO.RS;
using MasterRad.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MasterRad.API
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRole.ProfessorOrStudent)]
    public class MetadataController : BaseController
    {
        private readonly IMicrosoftSQL _microsoftSQLService;

        public MetadataController(IMicrosoftSQL microsoftSQLService)
        {
            _microsoftSQLService = microsoftSQLService;
        }

        [HttpGet, Route("tables/{dbName}")]
        public ActionResult<IEnumerable<string>> GetTableNames([FromRoute] string dbName)
        {
            var conn = _microsoftSQLService.GetAdminConnParams(dbName);

            var tableNames = _microsoftSQLService.GetTableNames(conn);

            //var username = "rY"; //_profileService.GetUserName(token);

            //var result = tableNames.Where(tn => tn.ToLower().EndsWith(username.ToLower()));

            return Ok(tableNames);
        }

        [HttpGet, Route("table-names/column-names/{dbName}")]
        public ActionResult<IEnumerable<TableWithColumns>> GetTableNamesWithColumnNames([FromRoute] string dbName)
        {
            var conn = _microsoftSQLService.GetAdminConnParams(dbName);
            return Ok(_microsoftSQLService.GetTableNamesWithColumnNames(conn));
        }

        [HttpGet, Route("columns/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<IEnumerable<ColumnInfoDTO>> GetColumnsData([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            //var conn = _profileService.GetConnectionsParams(token, dbName)
            var conn = _microsoftSQLService.GetAdminConnParams(dbName);
            return Ok(_microsoftSQLService.GetColumnsData(schemaName, tableName, conn));
        }

        [HttpGet, Route("identity_columns/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<IEnumerable<string>> GetIdentityColumns([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            var conn = _microsoftSQLService.GetAdminConnParams(dbName);
            return Ok(_microsoftSQLService.GetIdentityColumns(schemaName, tableName, conn));
        }

        [HttpGet, Route("constraints/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<IEnumerable<ColumnInfoDTO>> GetConstraintData([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            var conn = _microsoftSQLService.GetAdminConnParams(dbName);
            return Ok(_microsoftSQLService.GetConstraintData(schemaName, tableName, conn));
        }

        [HttpGet, Route("explore/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<TableInfoRS> ExploreTable([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            var res = new TableInfoRS($"{schemaName}.{tableName}");
            var conn = _microsoftSQLService.GetAdminConnParams(dbName);

            res.Columns = _microsoftSQLService.GetColumnsData(schemaName, tableName, conn).ToList();
            res.Constraints = _microsoftSQLService.GetConstraintData(schemaName, tableName, conn).ToList();

            return res;
        }
    }
}
