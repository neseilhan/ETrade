using System.ComponentModel;

namespace Business.Models
{
    public class CartGroupByModel
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        public double TotalUnitPrice { get; set; }

        [DisplayName("Total Unit Price")]
        public string TotalUnitPriceText { get; set; }

        [DisplayName("Total Count")]
        public int TotalCount { get; set; }

        public bool IsSum { get; set; } = false;
    }
}
