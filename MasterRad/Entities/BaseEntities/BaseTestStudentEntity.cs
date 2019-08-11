using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public abstract class BaseTestStudentEntity
    {
        public int StudentId { get; set; }
        public StudentEntity Student { get; set; }

        [Required, MaxLength(255)]
        public string NameOnServer { get; set; }
    }
}
