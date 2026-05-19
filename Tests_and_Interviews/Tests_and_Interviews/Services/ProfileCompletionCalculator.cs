// <copyright file="ProfileCompletionCalculator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Services.Interfaces;
    using Tests_and_Interviews.Dtos;

    public class ProfileCompletionCalculator : IProfileCompletionCalculator
    {
        private const int TotalRequiredTasksCount = 5;
        private const int MinimumRequiredPostedJobs = 5;
        private const int MinimumRequiredCollaborators = 2;
        private const int PercentageMultiplier = 100;
        private const int EmptyCount = 0;

        private const string TaskUploadPictureText = "Upload company picture";
        private const string TaskAddDescriptionText = "Add company description";
        private const string TaskPostJobsText = "Post at least 5 jobs";
        private const string TaskAddCollaboratorsText = "Add 2 collaborators";
        private const string TaskCompleteMiniGameText = "Complete mini-game";

        private readonly HttpClient http;

        public ProfileCompletionCalculator()
        {
            this.http = ApiClient.Http;
        }

        public ProfileCompletionCalculator(HttpClient httpClient)
        {
            this.http = httpClient ?? ApiClient.Http;
        }

        public (int percentage, List<string> remainingTasks) Calculate(Company company)
        {
            int completedTasksCount = EmptyCount;
            var remainingTasksList = new List<string>();

            if (!string.IsNullOrEmpty(company.ProfilePicturePath))
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskUploadPictureText);
            }

            if (!string.IsNullOrEmpty(company.AboutUs))
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskAddDescriptionText);
            }

            if (company.PostedJobsCount >= MinimumRequiredPostedJobs)
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskPostJobsText);
            }

            if (company.CollaboratorsCount >= MinimumRequiredCollaborators)
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskAddCollaboratorsText);
            }

            if (IsMiniGameComplete(company.Game))
            {
                completedTasksCount++;
            }
            else
            {
                remainingTasksList.Add(TaskCompleteMiniGameText);
            }

            return ((completedTasksCount * PercentageMultiplier) / TotalRequiredTasksCount, remainingTasksList);
        }

        /// <inheritdoc />
        public async Task<(List<string> skillNames, List<int> percents)> GetSkillsTop3Async(int companyId)
        {
            HttpResponseMessage response = await this.http.GetAsync($"companystats/{companyId}/skills/top3");

            if (!response.IsSuccessStatusCode)
                return (new List<string>(), new List<int>());

            var result = await response.Content.ReadFromJsonAsync<SkillsTop3Result>();
            return result != null ? (result.SkillNames, result.Percents) : (new List<string>(), new List<int>());
        }

        /// <inheritdoc />
        public async Task<string> ApplicantsMessage(int companyId)
        {
            HttpResponseMessage response = await this.http.GetAsync($"companystats/{companyId}/applicantsmessage");

            if (!response.IsSuccessStatusCode)
                return string.Empty;

            return await response.Content.ReadAsStringAsync();
        }

        private static bool IsMiniGameComplete(Game game)
        {
            return game != null && game.IsPublished;
        }
    }
}