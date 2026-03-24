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

        public async Task<IActionResult> MyTasks(
            string? search,
            int? status,
            string sortBy = "dueDate",
            string sortDirection = "asc",
            int page = 1)
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            var tasks = await _taskService.GetMyTasksAsync();
            var filteredTasks = ApplyTaskFilters(tasks, search, status, sortBy, sortDirection);
            const int pageSize = 6;
            var pagedTasks = filteredTasks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new TaskListPageViewModel
            {
                Title = "My Tasks",
                Subtitle = "Track upcoming work, monitor progress, and keep deadlines visible.",
                EmptyTitle = "No tasks match this view",
                EmptyDescription = "Try clearing filters or create a new task to start your workflow.",
                ShowOwnerColumn = false,
                Items = pagedTasks,
                Search = search?.Trim() ?? string.Empty,
                Status = status,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                PageSize = pageSize,
                TotalCount = filteredTasks.Count,
                TodoCount = tasks.Count(task => task.Status == 0),
                InProgressCount = tasks.Count(task => task.Status == 1),
                DoneCount = tasks.Count(task => task.Status == 2)
            };

            return View(viewModel);
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
                ModelState.AddModelError("", "We couldn't create the task. Please review the details and try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Task created successfully.";
            return RedirectToAction("MyTasks");
        }

       
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            var model = await _taskService.GetTaskForEditAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "The task could not be loaded.";
                return RedirectToAction("MyTasks");
            }

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
                ModelState.AddModelError("", "We couldn't save your changes. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Task updated successfully.";
            return RedirectToAction("MyTasks");
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            var success = await _taskService.DeleteTaskAsync(id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success
                ? "Task deleted successfully."
                : "The task could not be deleted.";

            return RedirectToAction("MyTasks");
        }

        private static List<TaskViewModel> ApplyTaskFilters(
            IEnumerable<TaskViewModel> tasks,
            string? search,
            int? status,
            string sortBy,
            string sortDirection)
        {
            var query = tasks.AsQueryable();
            var searchValue = search?.Trim();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(task =>
                    task.Title.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    task.Description.Contains(searchValue, StringComparison.OrdinalIgnoreCase));
            }

            if (status.HasValue)
            {
                query = query.Where(task => task.Status == status.Value);
            }

            var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant(), descending) switch
            {
                ("title", true) => query.OrderByDescending(task => task.Title),
                ("title", false) => query.OrderBy(task => task.Title),
                ("status", true) => query.OrderByDescending(task => task.Status),
                ("status", false) => query.OrderBy(task => task.Status),
                ("duedate", true) => query.OrderByDescending(task => task.DueDate),
                _ => query.OrderBy(task => task.DueDate)
            };

            return query.ToList();
        }
    }
}
