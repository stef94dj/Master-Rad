using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class AnalysisTestStudentEntity: BaseTestStudentEntity
    {
        public int AnalysisTestId { get; set; }
        public AnalysisTestEntity AnalysisTest{ get; set; }
        
        public AnalysisPaperEntity AnalysisPaper { get; set; }
    }
}
