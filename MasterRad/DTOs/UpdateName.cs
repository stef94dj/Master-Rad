using MasterRad.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.DTOs
{
    public class UpdateNameRQ : UpdateDTO
    {
        public string Name { get; set; }
    }
    public class UpdateNameRS { }
}
