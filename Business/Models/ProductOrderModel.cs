using AppCore.Records.Bases;

namespace Business.Models
{
    public class ProductOrderModel : RecordBase
    {
        public int OrderId { get; set; }
        public OrderModel Order { get; set; }
        public int ProductId { get; set; }
        public ProductModel Product { get; set; }
    }
}
