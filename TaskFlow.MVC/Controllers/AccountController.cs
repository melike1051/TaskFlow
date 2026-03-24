using Microsoft.AspNetCore.Mvc;
using TaskFlow.MVC.Models.Auth;
using TaskFlow.MVC.Services;

namespace TaskFlow.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.LoginAsync(model);

            if (result == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            HttpContext.Session.SetString("JWT", result.Token);
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetString("Username", model.Username);
            TempData["SuccessMessage"] = $"Welcome back, {model.Username}.";

            if (result.Role == "Admin")
                return RedirectToAction("AllTasks", "Admin");

            return RedirectToAction("MyTasks", "Task");
        }

        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _authService.RegisterAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", "We couldn't create your account. Please try a different username.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Your account is ready. You can sign in now.";
            return RedirectToAction("Login");
        }

        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out safely.";
            return RedirectToAction("Login");
        }
    }
}
