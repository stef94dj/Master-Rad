using MasterRad.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.DTOs
{
    public class UpdateTaskSolutionRQ : UpdateDTO
    {
        public int TemplateId { get; set; }
    }
    public class UpdateTaskSolutionRS { }
}
