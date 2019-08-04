using MasterRad.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.DTOs
{
    public class UpdateTemplateModelRQ : IdentifyDTO
    {
        public string SqlScript { get; set; }
    }
    public class UpdateTemplateModelRS { }
}
