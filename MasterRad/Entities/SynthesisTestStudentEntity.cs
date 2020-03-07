using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class SynthesisTestStudentEntity: BaseTestStudentEntity
    {
        public int SynthesisTestId { get; set; }
        public SynthesisTestEntity SynthesisTest { get; set; }

        [Required, MaxLength(255)]
        public string NameOnServer { get; set; }

        public bool TakenTest { get; set; }

        public string SqlScript { get; set; }

        public List<SynthesisEvaluationEntity> EvaluationProgress { get; set; }

        public List<AnalysisTestEntity> AnalysisTests { get; set; }
    }
}
