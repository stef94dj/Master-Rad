using MasterRad.DTO;
using MasterRad.DTO.RQ;
using MasterRad.Models;
using MasterRad.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MasterRad.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : Controller
    {
        private readonly IMicrosoftSQL _microsoftSQLService;
        private readonly IConfiguration _config;
        //private readonly IJsonHelper _jsonHelper;

        public DataController(IMicrosoftSQL microsoftSQLService, IConfiguration config)
        {
            _microsoftSQLService = microsoftSQLService;
            _config = config;
        }

        [HttpPost, Route("insert")]
        public ActionResult<Result<bool>> InsertRecord([FromBody] DataCreateRQ body)
        {
            var connParams = _microsoftSQLService.GetAdminConnParams(body.DatabaseName);
            return _microsoftSQLService.InsertRecord(body.SchemaName, body.TableName, body.ValuesNew, connParams);
        }

        [HttpPost, Route("update")]
        public ActionResult<Result<bool>> UpdateRecord([FromBody] DataUpdateRQ body)
        {
            var connParams = _microsoftSQLService.GetAdminConnParams(body.DatabaseName);

            //var userName = string.Empty; //_profileService.GetUserName(token); 
            //var tableName = $"{body.TableName}_{userName}";

            var count = _microsoftSQLService.Count(body.SchemaName, body.TableName, body.ValuesUnmodified, connParams);
            if (count != 1)
                return Result<bool>.Fail($"The change would affect {count} records.");

            return _microsoftSQLService.UpdateRecord(body.SchemaName, body.TableName, body.ValueNew, body.ValuesUnmodified, connParams);
        }

        [HttpPost, Route("delete")]
        public ActionResult<Result<bool>> DeleteRecord([FromBody] DataDeleteRQ body)
        {
            var connParams = _microsoftSQLService.GetAdminConnParams(body.DatabaseName);

            //var userName = string.Empty; //_profileService.GetUserName(token); 
            //var tableName = $"{body.TableName}_{userName}";

            return _microsoftSQLService.DeleteRecord(body.SchemaName, body.TableName, body.ValuesUnmodified, connParams);
        }

        [HttpGet, Route("read/{dbName}/{schemaName}/{tableName}")]
        public ActionResult<TableDTO> ReadTable([FromRoute] string dbName, [FromRoute] string schemaName, [FromRoute] string tableName)
        {
            var connParams = _microsoftSQLService.GetAdminConnParams(dbName);

            var queryResult = _microsoftSQLService.ReadTable(dbName, schemaName, tableName); //ovo ide kao admin - connParams?
            return queryResult.Tables.Single();
        }
    }
}
