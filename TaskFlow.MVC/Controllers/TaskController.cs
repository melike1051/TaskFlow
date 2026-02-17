using Microsoft.AspNetCore.Mvc;
using TaskFlow.MVC.Models.Tasks;
using TaskFlow.MVC.Services;

namespace TaskFlow.MVC.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<IActionResult> MyTasks()
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            var tasks = await _taskService.GetMyTasksAsync();
            return View(tasks);
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _taskService.CreateTaskAsync(model);

            if (!success)
            {
                ModelState.AddModelError("", "Task creation failed");
                return View(model);
            }

            return RedirectToAction("MyTasks");
        }

       
        [HttpGet]
        public IActionResult Edit(int id, string title, string description, DateTime dueDate, int status)
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            var model = new TaskEditViewModel
            {
                Id = id,
                Title = title,
                Description = description,
                DueDate = dueDate,
                Status = status
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _taskService.UpdateTaskAsync(model.Id, model);

            if (!success)
            {
                ModelState.AddModelError("", "Task update failed");
                return View(model);
            }

            return RedirectToAction("MyTasks");
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("JWT") == null)
            return RedirectToAction("Login", "Account");

            await _taskService.DeleteTaskAsync(id);

             return RedirectToAction("MyTasks");
        }

    }
}
