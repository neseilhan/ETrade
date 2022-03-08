using AppCore.Records.Bases;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Business.Models
{
    public class OrderModel : RecordBase
    {
        [DisplayName("Order ID")]
        public string OrderId { get; set; }

        public DateTime Date { get; set; }

        [DisplayName("Order Date")]
        public string DateText { get; set; }

        [DisplayName("Order Status")]
        public OrderStatus Status { get; set; }

        public int UserId { get; set; }
        public UserModel User { get; set; }
        public List<ProductOrderModel> ProductOrders { get; set; }

        /// <summary>
        /// OrderService'de order join query'sinde kullanılmak için eklendi.
        /// </summary>
        public ProductOrderModel ProductOrderJoin { get; set; }

        public string OrderColor { get; set; }
    }
}
