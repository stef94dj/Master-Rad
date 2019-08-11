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
        public List<SynthesisPaperStudentEntity> SynthesisPaperStudents { get; set; }

        [EmailAddress, Required]
        public string Email { get; set; }
    }
}
