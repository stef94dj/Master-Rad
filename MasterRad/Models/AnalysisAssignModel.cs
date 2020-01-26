using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Models
{
    public class AnalysisAssignModel
    {
        public int StudentId { get; set; }

        public string Database { get; set; }
        public string StudentOutputTable { get; set; }
        public string TeacherOutputTable { get; set; }
    }
}
