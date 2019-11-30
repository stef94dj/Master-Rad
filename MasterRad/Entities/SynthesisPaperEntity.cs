using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public class SynthesisPaperEntity : BaseEntity<int>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public string SqlScript { get; set; }

        public bool? PassPublicTest { get; set; }
        public string PublicTestFailReason { get; set; }

        public bool? PassSecretTest { get; set; }
        public string SecretTestFailReason { get; set; }

        public int STS_SynthesisTestId { get; set; }
        public int STS_StudentId { get; set; }
        [ForeignKey("STS_StudentId,STS_SynthesisTestId")]
        public SynthesisTestStudentEntity SynthesisTestStudent { get; set; }

        public List<AnalysisTestEntity> AnalysisTests { get; set; }
    }
}
