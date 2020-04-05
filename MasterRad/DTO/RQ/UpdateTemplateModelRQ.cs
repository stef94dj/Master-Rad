using MasterRad.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.DTO.RQ
{
    public class UpdateTemplateModelRQ : IdentifyDTO
    {
        public string SqlScript { get; set; }
    }
}
