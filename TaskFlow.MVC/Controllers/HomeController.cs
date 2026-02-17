using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
