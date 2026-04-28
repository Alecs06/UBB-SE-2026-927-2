// <copyright file="LeaderboardService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Repositories;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services.Interfaces;

    /// <inheritdoc cref="ILeaderboardService"/>
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ITestAttemptRepository testAttemptRepository;
        private readonly ILeaderboardRepository leaderboardRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardService"/> class.
        /// </summary>
        /// <param name="testAttemptRepository">Repository for test attempts.</param>
        /// <param name="leaderboardRepository">Repository for leaderboard.</param>
        ///
        public LeaderboardService(ITestAttemptRepository testAttemptRepository, ILeaderboardRepository leaderboardRepository)
        {
            this.testAttemptRepository = testAttemptRepository;
            this.leaderboardRepository = leaderboardRepository;
        }

        /// <inheritdoc />
        public async Task RecalculateLeaderboardAsync(int testId)
        {
            var attempts = await this.testAttemptRepository.FindValidAttemptsByTestIdAsync(testId);

            await this.leaderboardRepository.DeleteByTestIdAsync(testId);

            var entries = new List<LeaderboardEntry>();

            for (int i = 0; i < attempts.Count; i++)
            {
                var attempt = attempts[i];

                entries.Add(new LeaderboardEntry
                {
                    TestId = attempt.TestId,
                    UserId = attempt.ExternalUserId.Value,
                    NormalizedScore = attempt.PercentageScore!.Value,
                    RankPosition = i + 1,
                    TieBreakPriority = i + 1,
                    LastRecalculationAt = DateTime.UtcNow,
                });
            }

            if (entries.Count > 0)
            {
                await this.leaderboardRepository.SaveRangeAsync(entries);
            }
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> GetTopThreeAsync(int testId)
        {
            await this.RecalculateLeaderboardAsync(testId);
            return await this.leaderboardRepository.FindTopByTestIdAsync(testId, 3);
        }

        /// <inheritdoc />
        public async Task<LeaderboardEntry?> GetUserRankingAsync(int userId, int testId)
        {
            await this.RecalculateLeaderboardAsync(testId);
            return await this.leaderboardRepository.FindUserEntryAsync(userId, testId);
        }

        /// <inheritdoc />
        public async Task<List<LeaderboardEntry>> GetFullLeaderboardAsync(int testId)
        {
            await this.RecalculateLeaderboardAsync(testId);
            return await this.leaderboardRepository.FindByTestIdAsync(testId);
        }
    }
}