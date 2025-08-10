using Web.Models;
using System.Diagnostics;
using Infrastructure.Data;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RedirectStrategyContext _redirectContext;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _redirectContext = new RedirectStrategyContext();
        }

        //Index-----------------------------------------------
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var role = user?.KullaniciRolu;

            var strategy = _redirectContext.GetStrategy(role);

            return RedirectToAction(strategy.GetActionName(), strategy.GetControllerName());
        }
        //----------------------------------------------------

        //Privacy---------------------------------------------
        public IActionResult Privacy()
        {
            return View();
        }
        //----------------------------------------------------

        //Error-----------------------------------------------
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //----------------------------------------------------
    }
}
