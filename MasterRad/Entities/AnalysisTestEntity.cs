using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class AnalysisTestEntity : BaseTestEntity
    {
        public int SynthesisPaperId { get; set; }
        public SynthesisPaperEntity SynthesisPaper { get; set; }

        public List<AnalysisTestStudentEntity> AnalysisTestStudents{ get; set; }
    }
}
