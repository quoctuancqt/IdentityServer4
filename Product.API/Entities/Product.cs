using Core.Domain;

namespace Product.API.Entities
{
    public class Product : BaseEntity, IBaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
    }
}
