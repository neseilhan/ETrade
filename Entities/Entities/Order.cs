using AppCore.Records.Bases;
using Entities.Enums;
using System;
using System.Collections.Generic;

namespace Entities.Entities
{
    public class Order : RecordBase
    {
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<ProductOrder> ProductOrders { get; set; }
    }
}
