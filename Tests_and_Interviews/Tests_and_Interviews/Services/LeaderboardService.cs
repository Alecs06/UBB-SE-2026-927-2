// <copyright file="LeaderboardService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Services.Interfaces;

    /// <inheritdoc cref="ILeaderboardService"/>
    public class LeaderboardService : ILeaderboardService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardService"/> class.
        /// </summary>
        public LeaderboardService()
        {
        }

        /// <inheritdoc />
        public async Task RecalculateLeaderboardAsync(int testId)
        {
            HttpResponseMessage attemptsResponse = await ApiClient.Http.GetAsync($"testattempts/valid/bytest/{testId}");
            attemptsResponse.EnsureSuccessStatusCode();
            List<TestAttemptDto>? attemptDtos = await attemptsResponse.Content.ReadFromJsonAsync<List<TestAttemptDto>>();
            List<TestAttempt> attempts = attemptDtos?.Select(dto => dto.ToEntity()).ToList() ?? new List<TestAttempt>();

            HttpResponseMessage deleteResponse = await ApiClient.Http.DeleteAsync($"leaderboard/bytest/{testId}");
            deleteResponse.EnsureSuccessStatusCode();

            var entries = new List<LeaderboardEntry>();
            for (int i = 0; i < attempts.Count; i++)
            {
                var attempt = attempts[i];
                entries.Add(new LeaderboardEntry
                {
                    TestId = attempt.TestId,
                    UserId = attempt.ExternalUserId!.Value,
                    NormalizedScore = attempt.PercentageScore!.Value,
                    RankPosition = i + 1,
                    TieBreakPriority = i + 1,
                    LastRecalculationAt = DateTime.UtcNow,
                });
            }

            if (entries.Count > 0)
            {
                List<LeaderboardEntryDto> entryDtos = entries.Select(e => e.ToDto()).ToList();
                HttpResponseMessage saveResponse = await ApiClient.Http.PostAsJsonAsync("leaderboard", entryDtos);
                saveResponse.EnsureSuccessStatusCode();
            }
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> GetTopThreeAsync(int testId)
        {
            await this.RecalculateLeaderboardAsync(testId);
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"leaderboard/bytest/{testId}/top/3");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<LeaderboardEntry>();

            }

            response.EnsureSuccessStatusCode();
            List<LeaderboardEntryDto>? dtos = await response.Content.ReadFromJsonAsync<List<LeaderboardEntryDto>>();
            return dtos?.Select(dto => dto.ToEntity()).ToList() ?? new List<LeaderboardEntry>();
        }

        /// <inheritdoc />
        public async Task<LeaderboardEntry?> GetUserRankingAsync(int userId, int testId)
        {
            await this.RecalculateLeaderboardAsync(testId);
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"leaderboard/bytest/{testId}/byuser/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            LeaderboardEntryDto? dto = await response.Content.ReadFromJsonAsync<LeaderboardEntryDto>();
            return dto?.ToEntity();
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> GetFullLeaderboardAsync(int testId)
        {
            await this.RecalculateLeaderboardAsync(testId);
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"leaderboard/bytest/{testId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<LeaderboardEntry>();
            }

            response.EnsureSuccessStatusCode();
            List<LeaderboardEntryDto>? dtos = await response.Content.ReadFromJsonAsync<List<LeaderboardEntryDto>>();
            return dtos?.Select(dto => dto.ToEntity()).ToList() ?? new List<LeaderboardEntry>();
        }
    }
}