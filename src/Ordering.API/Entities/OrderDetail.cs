using Core.Domain;

namespace Ordering.API
{
    public class OrderDetail : BaseEntity, IBaseEntity
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
