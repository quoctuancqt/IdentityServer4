using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Base
{
    public class EntityBase : IEntity, IAudit
    {
        [Key]
        public virtual string Id { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual DateTime? CreatedDate { get; set; }

        public virtual string UpdatedBy { get; set; }

        public virtual DateTime? UpdatedAt { get; set; }
    }
}
