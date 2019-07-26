using MasterRad.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.DTOs
{
    public class UpdateDescriptionRQ : UpdateDTO
    {
        public string Description { get; set; }
    }
    public class UpdateDescriptionRS { }
}
