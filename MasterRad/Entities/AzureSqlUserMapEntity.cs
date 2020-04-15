using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class AzureSqlUserMapEntity
    {
        [Key]
        public Guid AzureId { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }

        public List<SynthesisTestStudentEntity> SynthesisTestStudents { get; set; }
        public List<AnalysisTestStudentEntity> AnalysisTestStudents { get; set; }

        [Required, MaxLength(255)]
        public string SqlUsername { get; set; }
        [Required, MaxLength(255)]
        public string SqlPassword { get; set; }
    }
}
