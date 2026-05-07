namespace Tests_and_Interviews_API.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tests_and_Interviews_API.Models.Core;

    /// <summary>
    /// Defines operations for managing users.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Asynchronously retrieves the user with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user, or null if not found.</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously retrieves all users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all users.</returns>
        Task<List<User>> GetAllAsync();

        /// <summary>
        /// Asynchronously adds a new user to the data store.
        /// </summary>
        /// <param name="user">The user to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync(User user);

        /// <summary>
        /// Asynchronously updates an existing user in the data store.
        /// </summary>
        /// <param name="user">The user with updated values. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateAsync(User user);

        /// <summary>
        /// Asynchronously deletes the user with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
    }
}