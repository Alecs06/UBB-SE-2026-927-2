namespace Tests_and_Interviews_API.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Repositories.Interfaces;
    using Tests_and_Interviews_API.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing users.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access user data. Cannot be null.</param>
        public UserService(IUserRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Asynchronously retrieves the user with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user, or null if not found.</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await this._repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Asynchronously retrieves all users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all users.</returns>
        public async Task<List<User>> GetAllAsync()
        {
            return await this._repository.GetAllAsync();
        }

        /// <summary>
        /// Asynchronously adds a new user to the data store.
        /// </summary>
        /// <param name="user">The user to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddAsync(User user)
        {
            await this._repository.AddAsync(user);
        }

        /// <summary>
        /// Asynchronously updates an existing user in the data store.
        /// </summary>
        /// <param name="user">The user with updated values. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(User user)
        {
            await this._repository.UpdateAsync(user);
        }

        /// <summary>
        /// Asynchronously deletes the user with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(int id)
        {
            await this._repository.DeleteAsync(id);
        }
    }
}