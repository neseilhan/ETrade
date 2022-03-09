using AppCore.Business.Models.Results;
using Business.Models;
using Business.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MvcWebUI.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ICountryService _countryService;
        private readonly ICityService _cityService;

        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountService accountService, ICountryService countryService, ICityService cityService,
            ILogger<AccountsController> logger)
        {
            _accountService = accountService;
            _countryService = countryService;
            _cityService = cityService;

            _logger = logger;
        }

        public IActionResult Register()
        {
            _logger.Log(LogLevel.Debug, "Accounts Controller -> Register HttpGet Action executed.");
            // IIS Express'te Output, Project'te konsol ekranlarında
            // uygulama ~\Properties\launchSettings.json dosyasında tanımlanmış MvcWebUI (Project - Production) modunda çalıştırıldığında
            // appsettings.json dosyasında Logging -> LogLevel -> Default değeri Information olarak ayarlandığından
            // bu log ekrana gelmez,
            // uygulama ~\Properties\launchSettings.json dosyasında tanımlanmış IIS Express (IISExpress - Development) modunda çalıştırıldığında
            // appsettings.Development.json dosyasında Logging -> LogLevel -> Default değeri Debug olarak ayarlandığından
            // bu log ekrana gelir.

            // Aşağıdan yukarıya gösterilme sırasına göre log seviyeleri: Sadece belirtilen seviye ve üstündeki seviyeler gösterilir.
            // Trace -> Debug -> Information -> Warning -> Error -> Critical

            var countriesResult = _countryService.GetCountries();
            if (countriesResult.Status == ResultStatus.Exception)
                throw new Exception(countriesResult.Message);
            ViewBag.Countries = new SelectList(countriesResult.Data, "Id", "Name");
            var model = new UserRegisterModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserRegisterModel model)
        {
            _logger.LogDebug("Accounts Controller -> Register HttpPost Action executed.");

            if (ModelState.IsValid)
            {
                var result = _accountService.Register(model);
                if (result.Status == ResultStatus.Exception)
                    throw new Exception(result.Message);
                if (result.Status == ResultStatus.Success)
                    return RedirectToAction("Login");

                // result.Status == ResultStatus.Error:
                ModelState.AddModelError("", result.Message);
            }
            var countriesResult = _countryService.GetCountries();
            if (countriesResult.Status == ResultStatus.Exception)
                throw new Exception(countriesResult.Message);
            ViewBag.Countries = new SelectList(countriesResult.Data, "Id", "Name", model.UserDetail.CountryId);
            var citiesResult = _cityService.GetCities(model.UserDetail.CountryId);
            if (citiesResult.Status == ResultStatus.Exception)
                throw new Exception(citiesResult.Message);
            ViewBag.Cities = new SelectList(citiesResult.Data, "Id", "Name", model.UserDetail.CityId);
            return View(model);
        }

        public IActionResult Login()
        {
            _logger.LogDebug("Accounts Controller -> Login HttpGet Action executed.");

            var model = new UserLoginModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            _logger.LogDebug("Accounts Controller -> Login HttpPost Action executed.");

            if (ModelState.IsValid)
            {
                var result = _accountService.Login(model);
                if (result.Status == ResultStatus.Exception)
                {
                    _logger.LogError(result.Message);

                    throw new Exception(result.Message);
                }

                if (result.Status == ResultStatus.Error)
                {
                    _logger.LogWarning(result.Message);

                    ViewBag.Message = result.Message;
                    return View(model);
                }

                _logger.LogInformation(result.Message);

                var user = result.Data;

                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role.Name),
                    new Claim(ClaimTypes.Sid, user.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogDebug("Accounts Controller -> Logout HttpGet Action executed.");

            //HttpContext.Session.Remove("cart"); // sadece cart key'ine ait session'ı temizler
            HttpContext.Session.Clear(); // tüm key'lere ait session'ları temizler
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            if (User.Identity.IsAuthenticated)
                _logger.LogCritical("Accounts Controller -> AccessDenied HttpGet Action executed by user: " + User.Identity.Name);
            else
                _logger.LogCritical("Accounts Controller -> AccessDenied HttpGet Action executed.");

            return View();
        }
    }
}
