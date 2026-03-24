using System.Net.Http.Json;
using TaskFlow.MVC.Models.Users;

namespace TaskFlow.MVC.Services
{
    public class UserService
    {
        private readonly ApiClient _apiClient;

        public UserService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<UserManagementViewModel>> GetUsersAsync()
        {
            var client = _apiClient.CreateClient();
            var response = await client.GetAsync("/api/users");
            if (!response.IsSuccessStatusCode)
                return [];

            return await response.Content.ReadFromJsonAsync<List<UserManagementViewModel>>() ?? [];
        }

        public async Task<(bool Success, string Message)> UpdateRoleAsync(int id, string role)
        {
            var client = _apiClient.CreateClient();
            var response = await client.PutAsJsonAsync($"/api/users/{id}/role", new { role });

            if (response.IsSuccessStatusCode)
                return (true, "User role updated successfully.");

            var error = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(error) ? "Could not update the user role." : error);
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            var client = _apiClient.CreateClient();
            var response = await client.DeleteAsync($"/api/users/{id}");

            if (response.IsSuccessStatusCode)
                return (true, "User deleted successfully.");

            var error = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(error) ? "Could not delete the user." : error);
        }
    }
}
