using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Services
{
    public interface IMsSqlQueryBuilder
    {
        string ToQuery(string sqlType, string value);
        string ToDisplay(string sqlType, object value);
    }

    public class MsSqlQueryBuilder : IMsSqlQueryBuilder
    {
        public string ToQuery(string sqlType, string value)
        {
            var res = string.Empty;
            if ("null".Equals(value.ToLower()))
                return "NULL";

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

        public string ToDisplay(string sqlType, object value)
        {
            var res = string.Empty;
            switch (sqlType)
            {
                case "date":
                    res = ((DateTime)value).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    break;
                case "time":
                    res = ((TimeSpan)value).ToString(@"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
                    //res = value.TrimEnd('0').TrimEnd('.');
                    break;
                case "smalldatetime":
                    res = ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    break;
                case "datetime":
                    res = ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);
                    break;
                case "datetime2":
                    res = ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture);
                    break;
                case "datetimeoffset":
                    res = ((DateTimeOffset)value).ToString("yyyy-MM-ddTHH:mm:ss.fffffffK", CultureInfo.InvariantCulture);
                    break;
                case "binary":
                case "varbinary":
                case "timestamp":
                case "rowversion":
                    res = $"0x{BitConverter.ToString((byte[])value).Replace("-", "")}";
                    break;
                default:
                    res = Convert.ToString(value, CultureInfo.InvariantCulture);
                    break;
            }
            return res;
        }
    }
}
