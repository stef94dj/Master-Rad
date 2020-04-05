using MasterRad.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class UpdateTaskSolutionRQ : UpdateDTO
    {
        public string SolutionSqlScript { get; set; }
        public List<ColumnDescriptionDTO> Columns { get; set; }
    } 
}
