using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Business.Models.Ordering;
using AppCore.Business.Models.Paging;
using AppCore.Business.Models.Results;
using Business.Models.Filters;
using Business.Services.Bases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcWebUI.Models;
using MvcWebUI.Settings;
using OfficeOpenXml;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsReportAjaxController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductsReportAjaxController(IProductService productService, ICategoryService categoryService, IHttpContextAccessor contextAccessor)
        {
            _productService = productService;
            _categoryService = categoryService;

            _httpContextAccessor = contextAccessor;
        }

        //public IActionResult Index()
        public IActionResult Index(int? categoryId)
        {
            //var productsFilter = new ProductsReportFilterModel();
            var productsFilter = new ProductsReportFilterModel()
            {
                CategoryId = categoryId
            };

            // Sayfalama
            #region Paging
            var page = new PageModel()
            {
                RecordsPerPageCount = AppSettings.RecordsPerPageCount
            };
            #endregion

            // Sıralama
            #region Ordering
            var order = new OrderModel()
            {
                Expression = "Category Name",
                DirectionAscending = true
            };
            #endregion

            var result = _productService.GetProductsReport(productsFilter, page, order);
            if (result.Status == ResultStatus.Exception)
                throw new Exception(result.Message);
            var productsReport = result.Data;

            #region Paging
            double recordsCount = page.RecordsCount; // filtrelenmiş veya filtrelenmemiş toplam kayıt sayısı
            double recordsPerPageCount = page.RecordsPerPageCount; // sayfa başına kayıt sayısı
            double totalPageCount = Math.Ceiling(recordsCount / recordsPerPageCount); // toplam sayfa sayısı
            List<SelectListItem> pageSelectListItems = new List<SelectListItem>();
            if (totalPageCount == 0)
            {
                pageSelectListItems.Add(new SelectListItem()
                {
                    Value = "1",
                    Text = "1"
                });
            }
            else
            {
                for (int pageNumber = 1; pageNumber <= totalPageCount; pageNumber++)
                {
                    pageSelectListItems.Add(new SelectListItem()
                    {
                        Value = pageNumber.ToString(),
                        Text = pageNumber.ToString()
                    });
                }
            }
            #endregion

            var viewModel = new ProductsReportAjaxIndexViewModel()
            {
                ProductsReport = productsReport,
                ProductsFilter = productsFilter,
                Pages = new SelectList(pageSelectListItems, "Value", "Text"),
                TotalRecordsCount = Convert.ToInt32(recordsCount),
                OrderByExpression = order.Expression,
                OrderByDirectionAscending = order.DirectionAscending

                // Categories artık view component üzerinden kullanıldığı için tekrar doldurmaya gerek yok
                //Categories = new SelectList(_categoryService.Query().ToList(), "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(ProductsReportAjaxIndexViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Sayfalama
                #region Paging
                var page = new PageModel()
                {
                    PageNumber = viewModel.PageNumber,
                    RecordsPerPageCount = AppSettings.RecordsPerPageCount
                };
                #endregion

                // Sıralama
                #region Ordering
                var order = new OrderModel()
                {
                    Expression = viewModel.OrderByExpression,
                    DirectionAscending = viewModel.OrderByDirectionAscending
                };
                #endregion

                var result = _productService.GetProductsReport(viewModel.ProductsFilter, page, order);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                viewModel.ProductsReport = result.Data;

                #region Paging
                double recordsCount = page.RecordsCount; // filtrelenmiş veya filtrelenmemiş toplam kayıt sayısı
                double recordsPerPageCount = page.RecordsPerPageCount; // sayfa başına kayıt sayısı
                double totalPageCount = Math.Ceiling(recordsCount / recordsPerPageCount); // toplam sayfa sayısı
                List<SelectListItem> pageSelectListItems = new List<SelectListItem>();
                if (totalPageCount == 0)
                {
                    pageSelectListItems.Add(new SelectListItem()
                    {
                        Value = "1",
                        Text = "1"
                    });
                }
                else
                {
                    for (int pageNumber = 1; pageNumber <= totalPageCount; pageNumber++)
                    {
                        pageSelectListItems.Add(new SelectListItem()
                        {
                            Value = pageNumber.ToString(),
                            Text = pageNumber.ToString()
                        });
                    }
                }
                #endregion

                viewModel.Pages = new SelectList(pageSelectListItems, "Value", "Text", viewModel.PageNumber);
                viewModel.TotalRecordsCount = Convert.ToInt32(recordsCount);
            }

            // Categories artık view component üzerinden kullanıldığı için tekrar doldurmaya gerek yok
            //viewModel.Categories = new SelectList(_categoryService.Query().ToList(), "Id", "Name", viewModel.ProductsFilter.CategoryId);

            return PartialView("_ProductsReport", viewModel);
        }

        public async Task Export()
        {
            try
            {
                var result = _productService.GetProductsReport();
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage = new ExcelPackage();
                ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Products Report");

                // 1. satır: sütun başlıkları
                excelWorksheet.Cells["A1"].Value = "Category";
                excelWorksheet.Cells["B1"].Value = "Product";
                excelWorksheet.Cells["C1"].Value = "Unit Price";
                excelWorksheet.Cells["D1"].Value = "Stock Amount";
                excelWorksheet.Cells["E1"].Value = "Expiration Date";

                // 2. satırdan itibaren veriler
                if (result.Data != null && result.Data.Count > 0)
                {
                    for (int row = 0; row < result.Data.Count; row++)
                    {
                        excelWorksheet.Cells["A" + (row + 2)].Value = result.Data[0].CategoryName;
                        excelWorksheet.Cells["B" + (row + 2)].Value = result.Data[0].ProductName;
                        excelWorksheet.Cells["C" + (row + 2)].Value = result.Data[0].UnitPriceText;
                        excelWorksheet.Cells["D" + (row + 2)].Value = result.Data[0].StockAmount;
                        excelWorksheet.Cells["E" + (row + 2)].Value = result.Data[0].ExpirationDateText;
                    }
                }

                excelWorksheet.Cells["A:AZ"].AutoFitColumns();

                var excelData = excelPackage.GetAsByteArray();
                _httpContextAccessor.HttpContext.Response.Headers.Clear();
                _httpContextAccessor.HttpContext.Response.Clear();
                _httpContextAccessor.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                _httpContextAccessor.HttpContext.Response.Headers.Add("content-length", excelData.Length.ToString());
                _httpContextAccessor.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=\"ProductsReport.xlsx\"");
                await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(excelData, 0, excelData.Length);
                _httpContextAccessor.HttpContext.Response.Body.Flush();
                _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}
