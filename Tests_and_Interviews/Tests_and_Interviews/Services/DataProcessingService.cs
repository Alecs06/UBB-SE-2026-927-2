// <copyright file="DataProcessingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Services.Interfaces;

    /// <inheritdoc cref="IDataProcessingService"/>
    public class DataProcessingService : IDataProcessingService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataProcessingService"/> class.
        /// </summary>
        public DataProcessingService()
        {
        }

        /// <inheritdoc/>
        public async Task<bool> ProcessFinalizedAttemptAsync(int attemptId)
        {
            HttpResponseMessage attemptResponse = await ApiClient.Http.GetAsync($"testattempts/{attemptId}");
            attemptResponse.EnsureSuccessStatusCode();
            TestAttemptDto? attemptDto = await attemptResponse.Content.ReadFromJsonAsync<TestAttemptDto>();
            if (attemptDto == null)
            {
                return false;
            }
            TestAttempt attempt = attemptDto.ToEntity();

            string? validationError = await this.ValidateAttemptAsync(attempt);
            if (validationError != null)
            {
                attempt.IsValidated = false;
                attempt.PercentageScore = null;
                attempt.RejectionReason = validationError;
                attempt.RejectedAt = DateTime.UtcNow;
                HttpResponseMessage rejectResponse = await ApiClient.Http.PutAsJsonAsync(
                    $"testattempts/{attempt.Id}",
                    attempt.ToDto());
                rejectResponse.EnsureSuccessStatusCode();
                return false;
            }

            attempt.IsValidated = true;
            attempt.PercentageScore = this.ConvertToPercentageScore(attempt.Score.GetValueOrDefault());
            attempt.RejectionReason = null;
            attempt.RejectedAt = null;
            HttpResponseMessage updateResponse = await ApiClient.Http.PutAsJsonAsync(
                $"testattempts/{attempt.Id}",
                attempt.ToDto());
            updateResponse.EnsureSuccessStatusCode();
            return true;
        }

        /// <summary>
        /// Performs a series of business rule checks to ensure the attempt is eligible for processing.
        /// </summary>
        /// <param name="attempt">The <see cref="TestAttempt"/> entity to evaluate.</param>
        /// <returns>A string containing the rejection reason if invalid; otherwise, <c>null</c>.</returns>
        private async Task<string?> ValidateAttemptAsync(TestAttempt attempt)
        {
            if (attempt.ExternalUserId == null)
            {
                return "User does not exist.";
            }

            HttpResponseMessage userResponse = await ApiClient.Http.GetAsync($"users/{attempt.ExternalUserId.Value}");
            if (!userResponse.IsSuccessStatusCode)
            {
                return "User does not exist.";
            }
            UserDto? userDto = await userResponse.Content.ReadFromJsonAsync<UserDto>();
            if (userDto == null)
            {
                return "User does not exist.";
            }

            HttpResponseMessage testResponse = await ApiClient.Http.GetAsync($"tests/{attempt.TestId}");
            if (!testResponse.IsSuccessStatusCode)
            {
                return "Test does not exist.";
            }
            TestDto? testDto = await testResponse.Content.ReadFromJsonAsync<TestDto>();
            if (testDto == null)
            {
                return "Test does not exist.";
            }
            Test test = testDto.ToEntity();

            if (attempt.CompletedAt == null)
            {
                return "Attempt is incomplete. Missing completion time.";
            }
            if (string.IsNullOrWhiteSpace(attempt.Status))
            {
                return "Attempt status is missing.";
            }
            if (!string.Equals(attempt.Status, "COMPLETED", StringComparison.OrdinalIgnoreCase))
            {
                return "Attempt is not eligible for leaderboard because status is not COMPLETED.";
            }
            if (attempt.Score < 0 || attempt.Score > 100)
            {
                return "Attempt score is invalid.";
            }
            if (!this.IsTestStillValidForLeaderboard(test))
            {
                return "Test is no longer valid for leaderboard inclusion.";
            }
            return null;
        }

        /// <summary>
        /// Determines if a test is still eligible for the leaderboard based on its creation date.
        /// Currently enforces a 3-month validity window.
        /// </summary>
        private bool IsTestStillValidForLeaderboard(Test test)
        {
            return test.CreatedAt.AddMonths(3) >= DateTime.UtcNow;
        }

        /// <summary>
        /// Normalizes the raw score into a percentage format.
        /// </summary>
        private decimal ConvertToPercentageScore(decimal originalScore)
        {
            return originalScore / 100m * 100m;
        }
    }
}