using AppCore.Business.Models.Results;
using Business.Services.Bases;
using DataAccess.EntityFramework.Contexts;
using Entities.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.Enums;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MvcWebUI.Settings;

namespace MvcWebUI.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        //private readonly ETradeContext _context;

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        //public ProductsController(ETradeContext context)
        //{
        //    _context = context;
        //}
        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [AllowAnonymous]
        //public IActionResult Index()
        public IActionResult Index(string message = null, int? id = null)

           
        {

            var query = _productService.Query();
            var model = query.ToList();
            ViewData["ProductsMessage"] = message;
            ViewBag.ProductId = id;
            return View(model);

            // Index aksiyonunun hata aldığındaki davranışını görebilmek için:
            //throw new Exception("Test Exception!");
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                //return NotFound();
                return View("NotFound");
            }

            var query = _productService.Query();

            var model = query.SingleOrDefault(p => p.Id == id.Value);

            if (model == null)
            {
                //return NotFound();
                return View("NotFound");
            }

            return View(model);
        }



        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var query = _categoryService.Query();
            ViewBag.Categories = new SelectList(query.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]


        public IActionResult Create(ProductModel product, IFormFile image) // .NET Core: IFormFile, .NET Framework: HttpPostedFileBase
        {
            Result productResult;
            IQueryable<CategoryModel> categoryQuery;
            if (ModelState.IsValid)
            {

                #region Dosya validasyonu
                string fileName = null;
                string fileExtension = null;
                string filePath = null; // sunucuda dosyayı kaydedeceğim yol
                bool saveFile = false; // flag
                if (image != null && image.Length > 0)
                {
                    fileName = image.FileName; // asusrog.jpg
                    fileExtension = Path.GetExtension(fileName); // .jpg
                    string[] appSettingsAcceptedImageExtensions = AppSettings.AcceptedImageExtensions.Split(',');
                    bool acceptedImageExtension = false; // flag
                    foreach (string appSettingsAcceptedImageExtension in appSettingsAcceptedImageExtensions)
                    {
                        if (fileExtension.ToLower() == appSettingsAcceptedImageExtension.ToLower().Trim())
                        {
                            acceptedImageExtension = true;
                            break;
                        }
                    }
                    if (!acceptedImageExtension)
                    {
                        ModelState.AddModelError("", "The image extension is not allowed, the accepted image extensions are " + AppSettings.AcceptedImageExtensions);
                        categoryQuery = _categoryService.Query();
                        ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);
                        return View(product);
                    }

                    // 1 byte = 8 bits
                    // 1 kilobyte = 1024 bytes
                    // 1 megabyte = 1024 kilobytes = 1024 * 1024 bytes
                    double acceptedFileLength = AppSettings.AcceptedImageMaximumLength * Math.Pow(1024, 2); // bytes
                    if (image.Length > acceptedFileLength)
                    {
                        ModelState.AddModelError("", "The image size is not allowed, the accepted image size must be maximum " + AppSettings.AcceptedImageMaximumLength + " MB");
                        categoryQuery = _categoryService.Query();
                        ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);
                        return View(product);
                    }

                    saveFile = true;
                }
                #endregion

                if (saveFile)
                {
                    fileName = Guid.NewGuid() + fileExtension; // x345f-dert5-gfds2-6hjkl.jpg

                    filePath = Path.Combine("wwwroot", "files", "products", fileName); // .NET Core


                }
                product.ImageFileName = fileName;

                productResult = _productService.Add(product);
                if (productResult.Status == ResultStatus.Exception) // exception
                {
                    throw new Exception(productResult.Message);
                }
                if (productResult.Status == ResultStatus.Success) // success
                {
                    if (saveFile)
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew))
                        {
                            image.CopyTo(fileStream);
                        }

                    }

                    //return RedirectToAction("Index");
                    return RedirectToAction(nameof(Index)); // nameof(Index) = "Index"
                }

                ModelState.AddModelError("", productResult.Message);

                categoryQuery = _categoryService.Query();
                ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);
                return View(product);
            }

            // validation error
            categoryQuery = _categoryService.Query();
            ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View("NotFound");

            var productQuery = _productService.Query();
            var product = productQuery.SingleOrDefault(p => p.Id == id.Value);
            if (product == null)
                return View("NotFound");

            var categoryQuery = _categoryService.Query();
            ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(ProductModel product, IFormFile image)
        {
            Result productResult;
            IQueryable<CategoryModel> categoryQuery;
            if (ModelState.IsValid)
            {

                #region Dosya validasyonu
                string fileName = null;
                string fileExtension = null;
                string filePath = null; //sunucuda dosyayı kaydedeceğim yol 
                bool saveFile = false; //flag
                if (image != null && image.Length > 0)
                {
                    fileName = image.FileName; //blabla.jpg
                    fileExtension = Path.GetExtension(fileName); // .jpg
                    string[] appSettingsAcceptedImageExtensions = AppSettings.AcceptedImageExtensions.Split(",");
                    bool acceptedImageExtension = false; //flag
                    foreach (string appSettingsAcceptedImageExtension in appSettingsAcceptedImageExtensions)
                    {
                        if (fileExtension.ToLower() == appSettingsAcceptedImageExtension.ToLower().Trim())
                        {
                            acceptedImageExtension = true;
                            break;
                        }
                    }
                    if (!acceptedImageExtension)
                    {
                        ModelState.AddModelError("", "The image extension is not allowed, the accepted image extensions are " + AppSettings.AcceptedImageExtensions);
                        categoryQuery = _categoryService.Query();
                        ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);
                        return View(product);
                    }

                    double acceptedFileLength = AppSettings.AcceptedImageMaximumLength * Math.Pow(1024, 2); // bytes
                    if (image.Length > acceptedFileLength)
                    {
                        ModelState.AddModelError("", "The image size is not allowed, the accepted image size must be maximum " + AppSettings.AcceptedImageMaximumLength + " MB");
                        categoryQuery = _categoryService.Query();
                        ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);
                        return View(product);
                    }

                    saveFile = true;
                }
                #endregion

                var existingProduct = _productService.Query().SingleOrDefault(p => p.Id == product.Id);

                if (saveFile) // kullanıcı dosya seçtiyse
                {
                    if (string.IsNullOrWhiteSpace(existingProduct.ImageFileName)) // veritabanında bu ürün için daha önce dosya kaydedilmemişse
                    {
                        fileName = Guid.NewGuid() + fileExtension; // yeni dosya adı ile kullanıcının yüklediği dosyanın uzantısını kullan
                    }
                    else // veritabanında bu ürün için daha önce dosya kaydedilmişse kullanıcının yüklediği dosya uzantısını al ve mevcut dosya adını koru
                    {
                        // existingProduct.ImageFileName = x345f-dert5-gfds2-6hjkl.jpg, fileExtension = png
                        int periodIndex = existingProduct.ImageFileName.IndexOf("."); // 23
                        fileName = existingProduct.ImageFileName.Substring(0, periodIndex); // x345f-dert5-gfds2-6hjkl
                        string existingProductImageFileExtension = existingProduct.ImageFileName.Substring(periodIndex); // .jpg
                        if (existingProductImageFileExtension != fileExtension) // mevcut dosya uzantısı ile yeni dosya uzantısı farklıysa
                        {
                            // sunucudaki mevcut dosya uzantısına sahip dosyayı sil
                            filePath = Path.Combine("wwwroot", "files", "products", existingProduct.ImageFileName);
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);
                        }
                        fileName = fileName + fileExtension;
                    }
                }
                else // kullanıcı dosya seçmediyse mevcut dosya adını veritabanında koru
                {
                    fileName = existingProduct.ImageFileName;
                }

                product.ImageFileName = fileName;

                productResult = _productService.Update(product);
                if (productResult.Status == ResultStatus.Exception) // exception
                {
                    throw new Exception(productResult.Message);
                }
                if (productResult.Status == ResultStatus.Success) // success
                {
                    if (saveFile)
                    {
                        filePath = Path.Combine("wwwroot", "files", "products", fileName);
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }
                    }

                    //return RedirectToAction("Index");
                    return RedirectToAction(nameof(Index)); // nameof(Index) = "Index"
                }

                // error
                //ViewBag.Message = productResult.Message;
                ModelState.AddModelError("", productResult.Message);

                categoryQuery = _categoryService.Query();
                ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);
                return View(product);
            }

            // validation error
            categoryQuery = _categoryService.Query();
            ViewBag.Categories = new SelectList(categoryQuery.ToList(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            //if (!User.IsInRole("Admin"))
            if (!User.IsInRole(Roles.Admin.ToString()))
                return RedirectToAction("Login", "Accounts");

            if (!id.HasValue)
                return View("NotFound");

            var existingProduct = _productService.Query().SingleOrDefault(p => p.Id == id.Value);

            var result = _productService.Delete(id.Value);
            if (result.Status == ResultStatus.Success)
            {
                if (!string.IsNullOrWhiteSpace(existingProduct.ImageFileName))
                {
                    string filePath = Path.Combine("wwwroot", "files", "products", existingProduct.ImageFileName);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                return RedirectToAction(nameof(Index));
            }
            throw new Exception(result.Message);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProductImage(int? id)
        {
            if (id == null)
                return View("NotFound");

            var existingProduct = _productService.Query().SingleOrDefault(p => p.Id == id.Value);
            if (!string.IsNullOrWhiteSpace(existingProduct.ImageFileName))
            {
                string filePath = Path.Combine("wwwroot", "files", "products", existingProduct.ImageFileName);
                existingProduct.ImageFileName = null;
                var result = _productService.Update(existingProduct);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            //return RedirectToAction(nameof(Details), new { id = existingProduct.Id }); 
            return View(nameof(Details), existingProduct);
        }

        //private bool ProductExists(int id)
        //{
        //    return _context.Products.Any(e => e.Id == id);
        //}
    }
}
