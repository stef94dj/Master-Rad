using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterRad.Entities
{
    public class ExerciseInstanceEntity : BaseTestStudentEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  int Id { get; set; }

        public int TemplateId { get; set; }
        public TemplateEntity Template { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string NameOnServer { get; set; }

        public string SqlScript { get; set; }
    }
}
