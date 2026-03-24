using Microsoft.AspNetCore.Mvc;
using TaskFlow.MVC.Models.Tasks;
using TaskFlow.MVC.Models.Users;
using TaskFlow.MVC.Services;

namespace TaskFlow.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly TaskService _taskService;
        private readonly UserService _userService;

        public AdminController(TaskService taskService, UserService userService)
        {
            _taskService = taskService;
            _userService = userService;
        }

        public async Task<IActionResult> AllTasks(
            string? search,
            int? status,
            string sortBy = "dueDate",
            string sortDirection = "asc",
            int page = 1)
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            var tasks = await _taskService.GetAllTasksAsync();
            var filteredTasks = ApplyTaskFilters(tasks, search, status, sortBy, sortDirection);
            const int pageSize = 8;
            var pagedTasks = filteredTasks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new TaskListPageViewModel
            {
                Title = "All Tasks",
                Subtitle = "Review workload across users and keep execution balanced.",
                EmptyTitle = "No tasks found",
                EmptyDescription = "No tasks match the selected filters right now.",
                ShowOwnerColumn = true,
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

        public async Task<IActionResult> Users(
            string? search,
            string role = "All",
            string sortBy = "username",
            string sortDirection = "asc",
            int page = 1)
        {
            if (HttpContext.Session.GetString("JWT") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            var users = await _userService.GetUsersAsync();
            var currentUsername = HttpContext.Session.GetString("Username") ?? string.Empty;
            var filteredUsers = ApplyUserFilters(users, search, role, sortBy, sortDirection, currentUsername);
            const int pageSize = 8;
            var pagedUsers = filteredUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new UserManagementPageViewModel
            {
                Items = pagedUsers,
                Search = search?.Trim() ?? string.Empty,
                Role = role,
                SortBy = sortBy,
                SortDirection = sortDirection,
                Page = page,
                PageSize = pageSize,
                TotalCount = filteredUsers.Count,
                AdminCount = users.Count(user => user.Role == "Admin"),
                UserCount = users.Count(user => user.Role == "User")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(UpdateUserRoleViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            var (success, message) = await _userService.UpdateRoleAsync(model.Id, model.Role);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return Unauthorized();

            var (success, message) = await _userService.DeleteUserAsync(id);
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = message;

            return RedirectToAction("Users");
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
                    task.Description.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    task.Username.Contains(searchValue, StringComparison.OrdinalIgnoreCase));
            }

            if (status.HasValue)
                query = query.Where(task => task.Status == status.Value);

            var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant(), descending) switch
            {
                ("title", true) => query.OrderByDescending(task => task.Title),
                ("title", false) => query.OrderBy(task => task.Title),
                ("status", true) => query.OrderByDescending(task => task.Status),
                ("status", false) => query.OrderBy(task => task.Status),
                ("user", true) => query.OrderByDescending(task => task.Username),
                ("user", false) => query.OrderBy(task => task.Username),
                ("duedate", true) => query.OrderByDescending(task => task.DueDate),
                _ => query.OrderBy(task => task.DueDate)
            };

            return query.ToList();
        }

        private static List<UserManagementViewModel> ApplyUserFilters(
            IEnumerable<UserManagementViewModel> users,
            string? search,
            string role,
            string sortBy,
            string sortDirection,
            string currentUsername)
        {
            var query = users.AsQueryable();
            var searchValue = search?.Trim();

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(user =>
                    user.Username.Contains(searchValue, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.Equals(role, "All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(user => user.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
            }

            var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant(), descending) switch
            {
                ("role", true) => query.OrderByDescending(user => user.Role),
                ("role", false) => query.OrderBy(user => user.Role),
                ("tasks", true) => query.OrderByDescending(user => user.TaskCount),
                ("tasks", false) => query.OrderBy(user => user.TaskCount),
                ("username", true) => query.OrderByDescending(user => user.Username),
                _ => query.OrderBy(user => user.Username)
            };

            var result = query.ToList();
            foreach (var user in result)
            {
                user.CanDelete = !user.Username.Equals(currentUsername, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }
    }
}
