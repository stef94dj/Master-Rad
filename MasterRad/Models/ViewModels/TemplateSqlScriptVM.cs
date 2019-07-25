using MasterRad.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models.ViewModels
{
    public class TemplateSqlScriptVM
    {
        public int Id { get; set; }
        public string TimeStamp { get; set; }
        public string TemplateName { get; set; }
        public string SqlScript { get; set; }
    }
}
