using Business.Models;
using Business.Models.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MvcWebUI.Models
{
    public class OrdersIndexViewModel
    {
        public List<OrderModel> Orders { get; set; }
        public OrdersFilterModel Filter { get; set; }

        MultiSelectList _orderStatusMultiSelectList;
        public MultiSelectList OrderStatusMultiSelectList
        {
            get
            {
                _orderStatusMultiSelectList = new MultiSelectList(Filter.OrderStatuses, "Value", "Text");
                return _orderStatusMultiSelectList;
            }
        }
    }
}
