// <copyright file="AttemptValidationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Services.Interfaces;

    /// <summary>
    /// AttemptValidationService class provides methods to validate if a user can start a test attempt and to check for existing attempts.
    /// </summary>
    public class AttemptValidationService : IAttemptValidationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttemptValidationService"/> class.
        /// </summary>
        public AttemptValidationService()
        {
        }

        /// <summary>
        /// Asynchronously checks if a user can start a test attempt by verifying if there are
        /// any existing attempts for the given user and test.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to start the test.</param>
        /// <param name="testId">The ID of the test the user is attempting to start.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CanStartTestAsync(int userId, int testId)
        {
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"testattempts/byuser/{userId}/bytest/{testId}");
            if (!response.IsSuccessStatusCode)
            {
                return true;
            }
            TestAttemptDto? dto = await response.Content.ReadFromJsonAsync<TestAttemptDto>();
            return dto == null;
        }

        /// <summary>
        /// Asynchronously checks for existing test attempts for a given user and test,
        /// and throws an exception if an attempt already exists.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to start the test.</param>
        /// <param name="testId">The ID of the test the user is attempting to start.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when an existing attempt is found for the user and test.</exception>
        public async Task CheckExistingAttemptsAsync(int userId, int testId)
        {
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"testattempts/byuser/{userId}/bytest/{testId}");
            if (!response.IsSuccessStatusCode)
            {
                return;
            }
            TestAttemptDto? dto = await response.Content.ReadFromJsonAsync<TestAttemptDto>();
            if (dto == null)
            {
                return;
            }
            throw new System.InvalidOperationException(
                $"User {userId} has already attempted test {testId}.");
        }
    }
}