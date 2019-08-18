using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class StudentEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? DateCreated { get; set; }

        public List<SynthesisTestStudentEntity> SynthesisTestStudents { get; set; }
        public List<AnalysisTestStudentEntity> AnalysisTestStudents { get; set; }

        [EmailAddress, Required, MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(255)]
        public string FirstName { get; set; }
        [MaxLength(255)]
        public string LastName { get; set; }
    }
}
