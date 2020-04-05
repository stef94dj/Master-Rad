using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO
{
    public class TableDTO
    {
        public TableDTO()
        {
            Columns = new List<ColumnDTO>();
            Rows = new List<List<object>>();
        }

        public List<ColumnDTO> Columns { get; set; }
        public List<List<object>> Rows { get; set; }
    }
}
