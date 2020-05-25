using MasterRad.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class UpdateIsPublicRQ : UpdateDTO
    {
        public bool IsPublic { get; set; }
    }
}
