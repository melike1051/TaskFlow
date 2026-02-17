using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace TaskFlow.MVC.Services
{
    public class ApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _contextAccessor;

        public ApiClient(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _contextAccessor = contextAccessor;
        }

        public HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("TaskFlowAPI");

            var token = _contextAccessor.HttpContext?
                .Session.GetString("JWT");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }
}
