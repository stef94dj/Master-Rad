using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class AnalysisTestStudentEntity : BaseTestStudentEntity
    {
        public int AnalysisTestId { get; set; }
        public AnalysisTestEntity AnalysisTest { get; set; }

        [Required, MaxLength(255)]
        public string InputNameOnServer { get; set; }

        public string TeacherOutputNameOnServer { get; set; }
        public string StudentOutputNameOnServer { get; set; }
        public bool TakenTest { get; set; }
    }
}
