using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public class Table
    {
        public Table()
        {
            Columns = new List<string>();
            Rows = new List<List<object>>();
        }

        public List<string> Columns { get; set; }
        public List<List<object>> Rows { get; set; }
    }
}
