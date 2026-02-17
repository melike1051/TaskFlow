using Microsoft.AspNetCore.Mvc;
using TaskFlow.MVC.Services;

namespace TaskFlow.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly TaskService _taskService;

        public AdminController(TaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<IActionResult> AllTasks()
        {
            
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            var tasks = await _taskService.GetAllTasksAsync();
            return View(tasks);
        }
    }
}
