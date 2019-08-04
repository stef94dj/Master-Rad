using MasterRad.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTOs
{
    public class UpdateDescriptionRQ : UpdateDTO
    {
        public string Description { get; set; }
    }
    public class UpdateDescriptionRS { }
}
