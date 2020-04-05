using MasterRad.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models
{
    public class CreateTable
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public IEnumerable<ColumnDTO> Columns { get; set; }
    }
}
