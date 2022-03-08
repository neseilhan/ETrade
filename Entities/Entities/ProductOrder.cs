using AppCore.Records.Bases;

namespace Entities.Entities
{
    public class ProductOrder : RecordBase
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
