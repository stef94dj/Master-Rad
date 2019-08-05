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
            Columns = new List<Column>();
            Rows = new List<List<object>>();
        }

        public List<Column> Columns { get; set; }
        public List<List<object>> Rows { get; set; }
    }

    public class Column
    {
        public Column(string name, string sqlType)
        {
            Name = name;
            SqlType = sqlType;
        }

        public string Name { get; set; }
        public string SqlType { get; set; }
        public bool? IsPrimaryKey { get; set; }
    }
}
