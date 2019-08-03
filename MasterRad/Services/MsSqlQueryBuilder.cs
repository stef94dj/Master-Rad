using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Services
{
    public interface IMsSqlQueryBuilder
    {
        string ToQuery(string sqlType, string value);
    }

    public class MsSqlQueryBuilder : IMsSqlQueryBuilder
    {
        public string ToQuery(string sqlType, string value)
        {
            var res = string.Empty;
            switch (sqlType)
            {
                case "nchar":
                case "nvarchar":
                    res = $"N'{value}'";
                    break;
                case "binary":
                case "varbinary":
                case "timestamp":
                case "rowversion":
                    res = value;
                    break;
                default:
                    res = $"'{value}'";
                    break;
            }
            return res;
        }
    }
}
