using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public abstract class BaseTestStudentEntity : BaseManyToManyEntity
    {
        public Guid StudentId { get; set; }
        public AzureSqlUserMapEntity Student { get; set; }
    }
}
