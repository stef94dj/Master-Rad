using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class Cell
    {
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }
        public string Value { get; set; }
    }

    public abstract class DataBaseRQ
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
    }

    public class DataCreateRQ : DataBaseRQ
    {
        public List<Cell> ValuesNew { get; set; }
    }

    public class DataDeleteRQ : DataBaseRQ
    {
        public List<Cell> ValuesUnmodified { get; set; }
    }

    public class DataUpdateRQ : DataBaseRQ
    {
        public Cell ValueNew { get; set; }
        public List<Cell> ValuesUnmodified { get; set; }
    }
}
