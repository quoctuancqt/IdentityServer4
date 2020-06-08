using Domain.Base;

namespace Domain
{
    public class Product : EntityBase, IAudit, IEntity
    {
        public string Name { get; set; }
    }
}
