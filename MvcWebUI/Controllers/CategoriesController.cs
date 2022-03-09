using AppCore.Business.Models.Results;
using Business.Services.Bases;
using DataAccess.EntityFramework.Contexts;
using Entities.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Business.Models;
using Microsoft.AspNetCore.Authorization;

namespace MvcWebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public IActionResult Index()
        {
            var query = _categoryService.Query();
            var model = query.ToList();
            return View(model);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            var model = new CategoryModel();
            return View(model);
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryService.Add(category);
                if (result.Status == ResultStatus.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                if (result.Status == ResultStatus.Error)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(category);
                }
                throw new Exception(result.Message);
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var query = _categoryService.Query();

            var category = query.SingleOrDefault(c => c.Id == id.Value);

            if (category == null)
            {
                return View("NotFound");
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                var result = _categoryService.Update(category);
                if (result.Status == ResultStatus.Success)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (result.Status == ResultStatus.Error)
                {
                    ModelState.AddModelError("", result.Message);
                    return View(category);
                }

                throw new Exception(result.Message);
            }
            return View(category);
        }

        // POST: Categories/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var deleteResult = _categoryService.Delete(id);
            if (deleteResult.Status == ResultStatus.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            if (deleteResult.Status == ResultStatus.Error)
            {
                ModelState.AddModelError("", deleteResult.Message);
                var categoryQuery = _categoryService.Query();

                var category = categoryQuery.SingleOrDefault(c => c.Id == id);
                return View("Edit", category);
            }
            throw new Exception(deleteResult.Message); // deleteResult.Status = Exception
        }

        #region ResponseCache
        // ResponseCache çıktıların ön bellekte saklanarak belirli bir süre boyunca bu çıktıların ön bellekten sonuç olarak döndürülmesini sağlar.
        // Özellikle sabit veya zamanla çok az değişen çıktılar için kullanılır.
        // Controller bazında da tanımlanabilir.
        // Controller bazında tanımlandığında eğer herhangi bir aksiyon için devre dışı bırakmak istersek Location = OutputCacheLocation.None yapabiliriz.
        // Unutulmamalı ki action'larda tanımlanan filter'lar controller'larda tanımlanan filter'ları, controller'larda tanımlanan filter'lar da global tanımlanan filter'ları ezer.
        // Duration zorunludur, saniye cinsindendir.
        // İstenirse ResponseCache için Startup.cs ConfigureServices() methodunda aşağıdaki şekilde bir profil oluşturulup
        // action veya controller üzerinde [ResponseCache(CacheProfileName = "Default5Seconds")] şeklinde kullanılabilir.
        /*
        services.AddMvc(options =>
        {
            options.CacheProfiles.Add("Default5Seconds",
                new CacheProfile()
                {
                    Duration = 5
                });
        });
        */
        // ResponseCache 2 farklı lokasyonda cache'leme yapabilir:
        // 1) Client'ın kullandığı web browser,
        // 2) Web proxy sunucusu.

        [ResponseCache(Duration = 5)]
        public ActionResult GetDateTimeContent() // her 5 saniyede bir cache yenilenecektir.
        {
            return Content(string.Format($"Date and Time: {DateTime.Now.ToString("T")}"));
        }

        [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Client, NoStore = false)]
        public ActionResult GetUserName() // burada Location için web sunucusu seçilmesi hata çünkü başka biri login olduğunda cache'den eski login kullanıcı adı gelecek.
                                          // NoStore proxy sunucusunda depolama yapmaması gerektiğini belirtiyor. Hassas veriler için true set edilmelidir.
        {
            return Content(string.Format($"Name: {User.Identity.Name}"));
        }

        //[ResponseCache(Duration = 360, VaryByQueryKeys = new string[] { "none" })]
        //[ResponseCache(Duration = 360, VaryByQueryKeys = new string[] { "*" })]
        //[ResponseCache(Duration = 360, VaryByQueryKeys = new string[] { "id" })]
        [ResponseCache(Duration = 360, VaryByQueryKeys = new string[] { "id", "name" })]

        public ActionResult GetUserDetails(int id, string name) // VaryByQueryKeys özelliğini hangi parametreleri cache sisteminden yalıtmak istiyorsak yani hangi parametreler
                                                                // değiştiğinde cache'i yenilemek istiyorsak o zaman kullanıyoruz.
                                                                // VaryByQueryKeys = "*": Eğer VaryByQueryKeys belirtmezsek default'tur. Yani tüm parametreler için veriyi her zaman 
                                                                // tekrar getirir ve cache'e atar.
                                                                // VaryByQueryKeys = "none": Hangi parametre değişirse değişsin belirtilen süre boyunca veriyi cache'de getirir.
        {
            return Content(string.Format($"Date and Time: {DateTime.Now.ToString("T")}, Id: {id}, Name: {name}"));
        }
        #endregion
    }
}
