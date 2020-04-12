using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class AnalysisTestEntity : BaseTestEntity
    {
        public int STS_SynthesisTestId { get; set; }
        public Guid STS_StudentId { get; set; }
        [ForeignKey("STS_StudentId,STS_SynthesisTestId")]
        public SynthesisTestStudentEntity SynthesisTestStudent { get; set; }

        public List<AnalysisTestStudentEntity> AnalysisTestStudents{ get; set; }
    }
}
