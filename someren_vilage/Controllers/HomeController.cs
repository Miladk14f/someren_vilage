using Microsoft.AspNetCore.Mvc;
using someren_vilage.Models;
using System.Diagnostics;

namespace someren_vilage.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }
    }
}
