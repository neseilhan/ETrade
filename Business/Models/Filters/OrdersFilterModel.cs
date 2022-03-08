using Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Business.Models.Filters
{
    public class OrdersFilterModel
    {
        [DisplayName("Date")]
        public string DateBeginText { get; set; }

        public string DateEndText { get; set; }

        [DisplayName("User")]
        public string UserName { get; set; }

        List<OrderStatusModel> _orderStatuses;
        public List<OrderStatusModel> OrderStatuses
        {
            get
            {
                _orderStatuses = new List<OrderStatusModel>();
                Array values = Enum.GetValues(typeof(OrderStatus));
                for (int i = 0; i < values.Length; i++)
                {
                    _orderStatuses.Add(new OrderStatusModel()
                    {
                        Value = ((int)values.GetValue(i)).ToString(),
                        Text = values.GetValue(i).ToString()
                    });
                }
                return _orderStatuses;
            }
        }

        [DisplayName("Status")]
        public List<int> OrderStatusValues { get; set; }

        [DisplayName("Order ID")]
        public string OrderId { get; set; }
    }
}
