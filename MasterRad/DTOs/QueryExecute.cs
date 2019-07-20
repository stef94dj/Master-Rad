using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Table = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>;

namespace MasterRad.DTOs
{
    public class QueryExecuteRQ
    {
        public string DatabaseName { get; set; }
        public string SQLQuery { get; set; }
    }

    public class QueryExecuteRS
    {
        public QueryExecuteRS(List<string> messages, List<Table> tables, int? rowsAffected)
        {
            Messages = messages;
            Tables = tables;
            RowsAffected = rowsAffected;
        }
        public List<string> Messages { get; set; }
        public int? RowsAffected { get; set; }
        public List<Table> Tables { get; set; }
    }
}
