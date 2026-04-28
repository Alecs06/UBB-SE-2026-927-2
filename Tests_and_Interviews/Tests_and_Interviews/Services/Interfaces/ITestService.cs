namespace Tests_and_Interviews.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Models.Core;

    /// <summary>
    /// ITestService interface provides methods to manage the lifecycle of test attempts, including starting a test, submitting a test, and retrieving the next available test for a given category.
    /// </summary>
    public interface ITestService
    {
        /// <summary>
        /// Asynchronously starts a test attempt for a given user and test. It checks for existing attempts,
        /// </summary>
        /// <param name="userId">The ID of the user starting the test.</param>
        /// <param name="testId">The ID of the test to be attempted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task StartTestAsync(int userId, int testId);

        /// <summary>
        /// Asynchronously submits a test attempt by grading the answers and calculating the final score. It retrieves
        /// </summary>
        /// <param name="attemptId">The ID of the test attempt to be submitted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SubmitTestAsync(int attemptId);

        /// <summary>
        /// Asynchronously retrieves the next available test for a given category. It queries the test repository for tests
        /// </summary>
        /// <param name="category">The category of the test to retrieve.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Test?> GetNextAvailableTestAsync(string category);
    }
}
