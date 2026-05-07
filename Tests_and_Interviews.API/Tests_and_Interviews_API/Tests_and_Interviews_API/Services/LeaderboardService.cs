namespace Tests_and_Interviews_API.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Repositories.Interfaces;
    using Tests_and_Interviews_API.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing leaderboard entries.
    /// </summary>
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access leaderboard data. Cannot be null.</param>
        public LeaderboardService(ILeaderboardRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Asynchronously retrieves all leaderboard entries for the specified test.
        /// </summary>
        /// <param name="testId">The unique identifier of the test.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of leaderboard entries for the specified test.</returns>
        public async Task<List<LeaderboardEntry>> FindByTestIdAsync(int testId)
        {
            return await this._repository.FindByTestIdAsync(testId);
        }

        /// <summary>
        /// Asynchronously retrieves the top leaderboard entries for the specified test.
        /// </summary>
        /// <param name="testId">The unique identifier of the test.</param>
        /// <param name="limit">The maximum number of entries to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of top leaderboard entries for the specified test.</returns>
        public async Task<List<LeaderboardEntry>> FindTopByTestIdAsync(int testId, int limit)
        {
            return await this._repository.FindTopByTestIdAsync(testId, limit);
        }

        /// <summary>
        /// Asynchronously retrieves the leaderboard entry for the specified user and test.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="testId">The unique identifier of the test.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the leaderboard entry for the specified user and test, or null if not found.</returns>
        public async Task<LeaderboardEntry?> FindUserEntryAsync(int userId, int testId)
        {
            return await this._repository.FindUserEntryAsync(userId, testId);
        }

        /// <summary>
        /// Asynchronously deletes all leaderboard entries for the specified test.
        /// </summary>
        /// <param name="testId">The unique identifier of the test.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteByTestIdAsync(int testId)
        {
            await this._repository.DeleteByTestIdAsync(testId);
        }

        /// <summary>
        /// Asynchronously saves a list of leaderboard entries to the data store.
        /// </summary>
        /// <param name="entries">The list of leaderboard entries to save. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SaveRangeAsync(List<LeaderboardEntry> entries)
        {
            await this._repository.SaveRangeAsync(entries);
        }
    }
}