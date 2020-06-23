using MasterRad.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RS
{
    
    public class TableInfoRS
    {
        public TableInfoRS(string name)
        {
            Name = name;
            Columns = new List<ColumnInfoDTO>();
            //Constraints = new List<ConstraintDTO>();
        }
        public string Name { get; set; }
        public List<ColumnInfoDTO> Columns { get; set; }
        public List<ConstraintDTO> Constraints { get; set; }
    }
}
