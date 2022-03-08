using AppCore.Business.Models.Results;
using AppCore.Business.Services.Bases;
using Business.Models;
using Business.Models.Filters;
using DataAccess.EntityFramework.Repositories;
using DataAccess.EntityFramework.Repositories.Bases;
using Entities.Entities;
using Entities.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Business.Services
{
    public interface IOrderService : IService<Business.Models.OrderModel>
    {
        Result<List<OrderModel>> GetOrderList(OrdersFilterModel filter = null);
    }

    public class OrderService : IOrderService
    {
        private readonly OrderRepositoryBase _orderRepository;
        private readonly UserDetailRepositoryBase _userDetailRepository;
        private readonly ProductRepositoryBase _productRepository;
        private readonly UserRepositoryBase _userRepository;
        private readonly CountryRepositoryBase _countryRepository;
        private readonly CityRepositoryBase _cityRepository;
        private readonly CategoryRepositoryBase _categoryRepository;
        private readonly ProductOrderRepositoryBase _productOrderRepository;

        public OrderService(OrderRepositoryBase orderRepository, UserDetailRepositoryBase userDetailRepository, ProductRepositoryBase productRepository, UserRepositoryBase userRepository, CountryRepositoryBase countryRepository, CityRepositoryBase cityRepository, CategoryRepositoryBase categoryRepository, ProductOrderRepositoryBase productOrderRepository)
        {
            _orderRepository = orderRepository;
            _userDetailRepository = userDetailRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _categoryRepository = categoryRepository;
            _productOrderRepository = productOrderRepository;
        }

        public Result Add(Business.Models.OrderModel model)
        {
            try
            {
                // her bir kullanıcının alındı durumunda tek bir siparişi olmalı
                Order existingEntity = _orderRepository.EntityQuery().SingleOrDefault(o => o.UserId == model.UserId && o.Status == OrderStatus.Received);
                if (existingEntity != null)
                    return new ErrorResult("User has previously given orders!");

                Order entity = new Order()
                {
                    Date = model.Date,
                    Status = model.Status,
                    UserId = model.UserId,
                    ProductOrders = model.ProductOrders.Select(po => new ProductOrder()
                    {
                        ProductId = po.ProductId
                    }).ToList()
                };
                _orderRepository.Add(entity);
                return new SuccessResult("Order received successfully.");
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result Delete(int id)
        {
            Business.Models.OrderModel model = new Business.Models.OrderModel()
            {
                Id = id,
                Status = OrderStatus.Canceled
            };
            var result = Update(model);
            if (result.Status == ResultStatus.Success)
                result.Message = "Order canceled successfully.";
            return result;
        }

        public void Dispose()
        {
            _orderRepository.Dispose();
        }

        /*
        select o.Guid OrderID, o.Date OrderDate, o.Status OrderStatus, 
        u.UserName, ud.EMail UserEMail, co.Name UserCountryName, ci.Name UserCityName,
        ud.Address UserAddress, p.Name ProductName, p.UnitPrice ProductUnitPrice,
        p.StockAmount ProductStockAmount, p.ExpirationDate ProductExpirationDate, 
        c.Name ProductCategoryName
        from ETradeOrders o 
        inner join ETradeUsers u on o.UserId = u.Id
        inner join ETradeUserDetails ud on ud.Id = u.UserDetailId
        inner join ETradeCountries co on ud.CountryId = co.Id
        inner join ETradeCities ci on ud.CityId = ci.Id
        inner join ETradeProductOrders po on po.OrderId = o.Id
        inner join ETradeProducts p on p.Id = po.ProductId
        inner join ETradeCategories c on p.CategoryId = c.Id
        */
        public IQueryable<Business.Models.OrderModel> Query()
        {
            var orderQuery = _orderRepository.EntityQuery();
            var userQuery = _userRepository.EntityQuery();
            var userDetailQuery = _userDetailRepository.EntityQuery();
            var countryQuery = _countryRepository.EntityQuery();
            var cityQuery = _cityRepository.EntityQuery();
            var productQuery = _productRepository.EntityQuery();
            var categoryQuery = _categoryRepository.EntityQuery();
            var productOrderQuery = _productOrderRepository.EntityQuery();
            var query = from o in orderQuery
                        join u in userQuery
                        on o.UserId equals u.Id
                        join ud in userDetailQuery
                        on u.UserDetailId equals ud.Id
                        join co in countryQuery
                        on ud.CountryId equals co.Id
                        join ci in cityQuery
                        on ud.CityId equals ci.Id
                        join po in productOrderQuery
                        on o.Id equals po.OrderId
                        join p in productQuery
                        on po.ProductId equals p.Id
                        join c in categoryQuery
                        on p.CategoryId equals c.Id
                        select new Business.Models.OrderModel()
                        {
                            Id = o.Id,
                            OrderId = "O" + o.Id,
                            Date = o.Date,
                            DateText = o.Date.ToString("MM/dd/yyyy hh:mm:ss", new CultureInfo("en-US")),
                            Status = o.Status,
                            OrderColor = o.Status == OrderStatus.Received ? "bg-warning" : (o.Status == OrderStatus.Completed ? "bg-success text-white" : "bg-danger text-white"),
                            User = new Business.Models.UserModel()
                            {
                                UserName = u.UserName,
                                UserDetail = new Business.Models.UserDetailModel()
                                {
                                    EMail = ud.EMail,
                                    Address = ud.Address,
                                    Country = new Business.Models.CountryModel()
                                    {
                                        Name = co.Name
                                    },
                                    City = new Business.Models.CityModel()
                                    {
                                        Name = ci.Name
                                    }
                                }
                            },
                            // join'lerde ProductOrders gibi kolleksiyon referansları kullanılamaz,
                            // bu yüzden ProductOrderJoin adında tek bir referans oluşturulmuş ve kullanılmıştır.
                            ProductOrderJoin = new Business.Models.ProductOrderModel()
                            {
                                Product = new Business.Models.ProductModel()
                                {
                                    Name = p.Name,
                                    StockAmount = p.StockAmount,
                                    UnitPrice = p.UnitPrice,
                                    UnitPriceText = "$" + p.UnitPrice.ToString(new CultureInfo("en-US")),
                                    ExpirationDate = p.ExpirationDate,
                                    ExpirationDateText = p.ExpirationDate.HasValue ? p.ExpirationDate.Value.ToString("MM/dd/yyyy", new CultureInfo("en-US")) : "",
                                    Category = new Business.Models.CategoryModel()
                                    {
                                        Name = c.Name
                                    }
                                }
                            }
                        };
            query = query.OrderBy(q => q.User.UserName).OrderByDescending(q => q.Date)
                .ThenBy(q => q.Status).ThenBy(q => q.ProductOrderJoin.Product.Category.Name).ThenBy(q => q.ProductOrderJoin.Product.Name);
            return query;
        }

        public Result Update(Business.Models.OrderModel model)
        {
            try
            {
                Order entity = _orderRepository.EntityQuery().SingleOrDefault(o => o.Id == model.Id);
                if (entity == null)
                    return new ErrorResult("Order not found!");
                entity.Status = model.Status;
                _orderRepository.Update(entity);
                return new SuccessResult("Order completed successfully.");
            }
            catch (Exception exc)
            {
                return new ExceptionResult(exc);
            }
        }

        public Result<List<OrderModel>> GetOrderList(OrdersFilterModel filter = null)
        {
            try
            {
                IQueryable<OrderModel> query = Query();
                if (filter != null)
                {
                    if (!string.IsNullOrWhiteSpace(filter.OrderId))
                    {
                        query = query.Where(q => q.OrderId.ToUpper().Contains(filter.OrderId.ToUpper().Trim()));
                    }
                    if (!string.IsNullOrWhiteSpace(filter.UserName))
                    {
                        query = query.Where(q => q.User.UserName.ToLower().Contains(filter.UserName.ToLower().Trim()));
                    }
                    if (!string.IsNullOrWhiteSpace(filter.DateBeginText))
                    {
                        DateTime beginDate = DateTime.Parse(filter.DateBeginText + " 00:00:00", new CultureInfo("en-US"));
                        query = query.Where(q => q.Date >= beginDate);
                    }
                    if (!string.IsNullOrWhiteSpace(filter.DateEndText))
                    {
                        DateTime endDate = DateTime.Parse(filter.DateEndText + " 23:59:59", new CultureInfo("en-US"));
                        query = query.Where(q => q.Date <= endDate);
                    }
                    if (filter.OrderStatusValues != null && filter.OrderStatusValues.Count > 0)
                    {
                        query = query.Where(q => filter.OrderStatusValues.Contains((int)q.Status));
                    }
                }
                return new SuccessResult<List<OrderModel>>(query.ToList());
            }
            catch (Exception exc)
            {
                return new ExceptionResult<List<OrderModel>>(exc);
            }
        }
    }
}
