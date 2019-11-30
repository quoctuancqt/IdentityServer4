using System;

namespace Product.API.Dtos
{
    public class ProductDto
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
    }
}
