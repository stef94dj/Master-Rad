using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class CreateExerciseInstanceRQ
    {
        public string Name { get; set; }
        public int TemplateId { get; set; }
    }
}
