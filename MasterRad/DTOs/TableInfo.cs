using MasterRad.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    
    public class TableInfoRS
    {
        public TableInfoRS(string name)
        {
            Name = name;
            Columns = new List<ColumnInfo>();
            Constraints = new List<ConstraintInfo>();
        }
        public string Name { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public List<ConstraintInfo> Constraints { get; set; }
    }
}
