using AppCore.Business.Models.Results;
using Business.Models;
using Business.Models.Filters;
using Business.Services;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcWebUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcWebUI.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "User")]
        public IActionResult Receive()
        {
            if (HttpContext.Session.GetString("cart") == null)
            {
                return RedirectToAction("Index", "Products");
            }
            List<CartModel> cartModel = JsonConvert.DeserializeObject<List<CartModel>>(HttpContext.Session.GetString("cart"));
            HttpContext.Session.Remove("cart");
            OrderModel orderModel = new OrderModel()
            {
                Date = DateTime.Now,
                Status = OrderStatus.Received,
                UserId = cartModel.FirstOrDefault().UserId,
                ProductOrders = cartModel.Select(c => new ProductOrderModel()
                {
                    ProductId = c.ProductId
                }).ToList()
            };
            var result = _orderService.Add(orderModel);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            TempData["OrdersMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index(OrdersFilterModel filter = null)
        {
            var result = _orderService.GetOrderList(filter);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            List<OrderModel> orders = result.Data;
            OrdersIndexViewModel viewModel = new OrdersIndexViewModel()
            {
                Orders = orders,
                Filter = filter ?? new OrdersFilterModel()
            };
            return View(viewModel);
        }

        public IActionResult Cancel(int? id)
        {
            if (id == null)
                return View("NotFound");
            var result = _orderService.Delete(id.Value);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            TempData["OrdersMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Complete(int? id)
        {
            if (id == null)
                return View("NotFound");
            var result = _orderService.Update(new Business.Models.OrderModel() { Id = id.Value, Status = OrderStatus.Completed });
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            TempData["OrdersMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
