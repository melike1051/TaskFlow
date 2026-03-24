using System.Net.Http.Json;
using TaskFlow.MVC.Models.Tasks;

namespace TaskFlow.MVC.Services
{
    public class TaskService
    {
        private readonly ApiClient _apiClient;

        public TaskService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

       
        public async Task<List<TaskViewModel>> GetMyTasksAsync()
        {
            var client = _apiClient.CreateClient();
            var response = await client.GetAsync("/api/tasks/my");

            if (!response.IsSuccessStatusCode)
                return new List<TaskViewModel>();

            return await response.Content
                .ReadFromJsonAsync<List<TaskViewModel>>()
                ?? new List<TaskViewModel>();
        }

        public async Task<TaskEditViewModel?> GetTaskForEditAsync(int id)
        {
            var client = _apiClient.CreateClient();
            var response = await client.GetAsync($"/api/tasks/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            var task = await response.Content.ReadFromJsonAsync<TaskViewModel>();
            if (task == null)
                return null;

            return new TaskEditViewModel
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status
            };
        }

        
        public async Task<bool> CreateTaskAsync(TaskCreateViewModel model)
        {
            var client = _apiClient.CreateClient();
            var response = await client.PostAsJsonAsync("/api/tasks", model);
            return response.IsSuccessStatusCode;
        }

        
        public async Task<bool> UpdateTaskAsync(int id, TaskEditViewModel model)
        {
            var client = _apiClient.CreateClient();
            var response = await client.PutAsJsonAsync($"/api/tasks/{id}", model);
            return response.IsSuccessStatusCode;
        }

        
        public async Task<List<TaskViewModel>> GetAllTasksAsync()
        {
            var client = _apiClient.CreateClient();
            var response = await client.GetAsync("/api/tasks");

            if (!response.IsSuccessStatusCode)
                return new List<TaskViewModel>();

            var apiTasks = await response.Content
                .ReadFromJsonAsync<List<AdminTaskDto>>();

            if (apiTasks == null)
                return new List<TaskViewModel>();

            return apiTasks.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Status = t.Status,
                Username = t.User.Username
            }).ToList();
        }

        
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var client = _apiClient.CreateClient();
            var response = await client.DeleteAsync($"/api/tasks/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
