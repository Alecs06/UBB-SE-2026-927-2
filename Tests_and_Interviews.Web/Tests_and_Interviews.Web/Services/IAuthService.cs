namespace Tests_and_Interviews.Web.Services
{
    using Tests_and_Interviews.Web.Models;

    /// <summary>
    /// Defines authentication operations against the API.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Sends login credentials to the API and returns the auth response.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's plain-text password.</param>
        /// <returns>An <see cref="AuthResponseModel"/> if successful, null otherwise.</returns>
        Task<AuthResponseModel?> LoginAsync(string email, string password);

        /// <summary>
        /// Sends registration data to the API and returns the auth response.
        /// </summary>
        /// <param name="name">The user's display name.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's plain-text password.</param>
        /// <param name="role">The user's selected role.</param>
        /// <returns>An <see cref="AuthResponseModel"/> if successful, null otherwise.</returns>
        Task<AuthResponseModel?> RegisterAsync(string name, string email, string password, string role);
    }
}