namespace Tests_and_Interviews.Web.Services
{
    using System.Net.Http.Json;
    using Tests_and_Interviews.Web.Models;

    /// <summary>
    /// Calls the API authentication endpoints.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly HttpClient http;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="http">The HTTP client used to call the API.</param>
        public AuthService(HttpClient http)
        {
            this.http = http;
        }

        /// <inheritdoc/>
        public async Task<AuthResponseModel?> LoginAsync(string email, string password)
        {
            var payload = new { Email = email, Password = password };
            HttpResponseMessage response =
                await this.http.PostAsJsonAsync("api/auth/login", payload);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponseModel>();
        }

        /// <inheritdoc/>
        public async Task<AuthResponseModel?> RegisterAsync(
            string name, string email, string password, string role)
        {
            var payload = new
            {
                Name = name,
                Email = email,
                Password = password,
                Role = role,
            };

            HttpResponseMessage response =
                await this.http.PostAsJsonAsync("api/auth/register", payload);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponseModel>();
        }
    }
}