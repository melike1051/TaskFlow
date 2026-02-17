using System.Net.Http.Json;
using TaskFlow.MVC.Models.Auth;

namespace TaskFlow.MVC.Services
{
    public class AuthService
    {
        private readonly ApiClient _apiClient;

        public AuthService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<AuthResponseViewModel?> LoginAsync(LoginViewModel model)
        {
            var client = _apiClient.CreateClient();

            var response = await client.PostAsJsonAsync(
                "/api/auth/login", model);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<AuthResponseViewModel>();
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            var client = _apiClient.CreateClient();

            var response = await client.PostAsJsonAsync(
                "/api/auth/register", model);

            return response.IsSuccessStatusCode;
        }
    }
}
