using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Entities
{
    public abstract class BaseEntityInsertOnly<TPrimaryKey>
    {
        public abstract TPrimaryKey Id { get; set; }
        [Timestamp]
        public virtual byte[] TimeStamp { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public abstract class BaseEntity<TPrimaryKey> : BaseEntityInsertOnly<TPrimaryKey>
    {
        public DateTime? DateModified { get; set; }
        public Guid ModifiedBy { get; set; }
    }

    public abstract class BaseManyToManyEntity
    {
        [Timestamp]
        public virtual byte[] TimeStamp { get; set; }
        public DateTime? DateCreated { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}
