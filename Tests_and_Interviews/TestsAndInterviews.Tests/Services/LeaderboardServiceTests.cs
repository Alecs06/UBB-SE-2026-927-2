namespace Tests_and_Interviews.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services;
    using Xunit;

    public class LeaderboardServiceTests
    {
        #region helpers

        private static (
            Mock<ITestAttemptRepository> attemptRepo,
            Mock<ILeaderboardRepository> leaderboardRepo,
            LeaderboardService leaderboardService)
            
            CreateSetupMocksAndService()
        {
            var attemptRepo = new Mock<ITestAttemptRepository>(MockBehavior.Strict);
            var leaderboardRepo = new Mock<ILeaderboardRepository>(MockBehavior.Strict);

            var leaderboardService = new LeaderboardService(attemptRepo.Object, leaderboardRepo.Object);

            return (attemptRepo, leaderboardRepo, leaderboardService);
        }

        private static TestAttempt MakeAttempt(int testId, int userId, decimal pct, DateTime? completedAt = null) =>
            new TestAttempt
            {
                TestId = testId,
                ExternalUserId = userId,
                PercentageScore = pct,
                Score = pct,
                StartedAt = DateTime.UtcNow.AddMinutes(-30),
                CompletedAt = completedAt ?? DateTime.UtcNow,
            };
        private static LeaderboardEntry MakeEntry(int testId, int userId, decimal score, int rank) =>
            new LeaderboardEntry
            {
                TestId = testId,
                UserId = userId,
                NormalizedScore = score,
                RankPosition = rank,
            };

        private static void SetupRecalculate(
            Mock<ITestAttemptRepository> attemptRepo,
            Mock<ILeaderboardRepository> leaderboardRepo,
            int testId,
            List<TestAttempt> attempts)
        {
            attemptRepo
                .Setup(repository => repository.FindValidAttemptsByTestIdAsync(testId))
                .ReturnsAsync(attempts);

            leaderboardRepo
                .Setup(repository => repository.DeleteByTestIdAsync(testId))
                .Returns(Task.CompletedTask);

            if (attempts.Count > 0)
            {
                leaderboardRepo
                    .Setup(repository => repository.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()))
                    .Returns(Task.CompletedTask);
            }
        }
        #endregion

        #region RecalculateLeaderboard
        [Fact]
        public async Task RecalculateLeaderboard_WithAttempts_DeletesThenSavesEntries()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 1;

            var attempts = new List<TestAttempt>
            {
                MakeAttempt(testId, 10, 90m),
                MakeAttempt(testId, 20, 75m),
            };

            attemptRepo
                .Setup(repository => repository.FindValidAttemptsByTestIdAsync(testId))
                .ReturnsAsync(attempts);

            leaderboardRepo
                .Setup(repository => repository.DeleteByTestIdAsync(testId))
                .Returns(Task.CompletedTask);

            List<LeaderboardEntry>? saved = null;
            leaderboardRepo
                .Setup(repository => repository.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()))
                .Callback<List<LeaderboardEntry>>(e => saved = e)
                .Returns(Task.CompletedTask);

            await leaderboardService.RecalculateLeaderboardAsync(testId);

            // i know there should only be one verify or assert but it doesn't make sense to only have one 
            leaderboardRepo.Verify(repository => repository.DeleteByTestIdAsync(testId), Times.Once);
            leaderboardRepo.Verify(r => r.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()), Times.Once);
            Assert.Equal(2, saved.Count);
        }

        [Fact]
        public async Task RecalculateLeaderboard_AssignsRanksInOrder()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 2;

            var attempts = new List<TestAttempt>
            {
                MakeAttempt(testId, 1, 95m),
                MakeAttempt(testId, 2, 80m),
                MakeAttempt(testId, 3, 60m),
            };

            attemptRepo
                .Setup(repository => repository.FindValidAttemptsByTestIdAsync(testId))
                .ReturnsAsync(attempts);
            leaderboardRepo
                .Setup(repository => repository.DeleteByTestIdAsync(testId))
                .Returns(Task.CompletedTask);

            List<LeaderboardEntry>? saved = null;
            leaderboardRepo
                .Setup(repository => repository.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()))
                .Callback<List<LeaderboardEntry>>(entry => saved = entry)
                .Returns(Task.CompletedTask);

            await leaderboardService.RecalculateLeaderboardAsync(testId);

            Assert.Equal(1, saved[0].RankPosition);
            Assert.Equal(2, saved[1].RankPosition);
            Assert.Equal(3, saved[2].RankPosition);
        }

        [Fact]
        public async Task RecalculateLeaderboard_MapsAttemptFieldsToEntryCorrectly()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 3;
            const int userId = 10;
            const decimal pct = 88.5m;

            var attempts = new List<TestAttempt> { MakeAttempt(testId, userId, pct) };

            attemptRepo
                .Setup(repository => repository.FindValidAttemptsByTestIdAsync(testId))
                .ReturnsAsync(attempts);
            leaderboardRepo
                .Setup(repository => repository.DeleteByTestIdAsync(testId))
                .Returns(Task.CompletedTask);

            List<LeaderboardEntry>? saved = null;
            leaderboardRepo
                .Setup(repository => repository.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()))
                .Callback<List<LeaderboardEntry>>(entry => saved = entry)
                .Returns(Task.CompletedTask);

            var before = DateTime.UtcNow;
            await leaderboardService.RecalculateLeaderboardAsync(testId);
            var after = DateTime.UtcNow;

            var entry = saved[0];
            Assert.Equal(testId, entry.TestId);
            Assert.Equal(userId, entry.UserId);
            Assert.Equal(pct, entry.NormalizedScore);
            Assert.Equal(1, entry.RankPosition);
            Assert.Equal(1, entry.TieBreakPriority);
            Assert.InRange(entry.LastRecalculationAt, before, after);
        }

        [Fact]
        public async Task RecalculateLeaderboard_WithNoAttempts_DeletesButDoesNotSave()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 4;

            attemptRepo
                .Setup(repository => repository.FindValidAttemptsByTestIdAsync(testId))
                .ReturnsAsync(new List<TestAttempt>());

            leaderboardRepo
                .Setup(repository => repository.DeleteByTestIdAsync(testId))
                .Returns(Task.CompletedTask);

            await leaderboardService.RecalculateLeaderboardAsync(testId);

            leaderboardRepo.Verify(repository => repository.DeleteByTestIdAsync(testId), Times.Once);
            leaderboardRepo.Verify(repository => repository.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()), Times.Never);
        }

        [Fact]
        public async Task RecalculateLeaderboard_DeleteAlwaysCalledBeforeSave()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 5;
            var callOrder = new List<string>();

            attemptRepo
                .Setup(repository => repository.FindValidAttemptsByTestIdAsync(testId))
                .ReturnsAsync(new List<TestAttempt> { MakeAttempt(testId, 99, 50m) });

            leaderboardRepo
                .Setup(repository => repository.DeleteByTestIdAsync(testId))
                .Callback(() => callOrder.Add("delete"))
                .Returns(Task.CompletedTask);

            leaderboardRepo
                .Setup(repository => repository.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()))
                .Callback<List<LeaderboardEntry>>(_ => callOrder.Add("save"))
                .Returns(Task.CompletedTask);

            await leaderboardService.RecalculateLeaderboardAsync(testId);

            Assert.Equal(new[] { "delete", "save" }, callOrder);
        }
        #endregion

        #region Test GetTopThree

        [Fact]
        public async Task GetTopThree_RecalculatesThenReturnsTopThree()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 10;

            var attempts = new List<TestAttempt>
            {
                MakeAttempt(testId, 1, 95m),
                MakeAttempt(testId, 2, 85m),
                MakeAttempt(testId, 3, 60m),
                MakeAttempt(testId, 4, 75m),
            };

            var topThree = new List<LeaderboardEntry>
            {
                MakeEntry(testId, 1, 95m, 1),
                MakeEntry(testId, 2, 85m, 2),
                MakeEntry(testId, 3, 75m, 3),
            };

            attemptRepo
                .Setup(repository => repository.FindValidAttemptsByTestIdAsync(testId))
                .ReturnsAsync(attempts);
            leaderboardRepo
                .Setup(repository => repository.DeleteByTestIdAsync(testId))
                .Returns(Task.CompletedTask);
            leaderboardRepo
                .Setup(repository => repository.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()))
                .Returns(Task.CompletedTask);
            leaderboardRepo
                .Setup(repository => repository.FindTopByTestIdAsync(testId, 3))
                .ReturnsAsync(topThree);

            var result = await leaderboardService.GetTopThreeAsync(testId);

            Assert.Equal(3, result.Count);
            Assert.Equal(1, result[0].RankPosition);
            leaderboardRepo.Verify(r => r.FindTopByTestIdAsync(testId, 3), Times.Once);
        }

        [Fact]
        public async Task GetTopThree_WithFewerThanThreeAttempts_ReturnsWhatExists()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 11;

            attemptRepo
                .Setup(repository => repository.FindValidAttemptsByTestIdAsync(testId))
                .ReturnsAsync(new List<TestAttempt> { MakeAttempt(testId, 1, 70m) });
            leaderboardRepo
                .Setup(repository => repository.DeleteByTestIdAsync(testId))
                .Returns(Task.CompletedTask);
            leaderboardRepo
                .Setup(repository => repository.SaveRangeAsync(It.IsAny<List<LeaderboardEntry>>()))
                .Returns(Task.CompletedTask);

            var singleEntry = new List<LeaderboardEntry> { MakeEntry(testId, 1, 70m, 1) };
            leaderboardRepo
                .Setup(repository => repository.FindTopByTestIdAsync(testId, 3))
                .ReturnsAsync(singleEntry);

            var result = await leaderboardService.GetTopThreeAsync(testId);

            Assert.Single(result);
        }

        #endregion

        #region Get User Ranking

        [Fact]
        public async Task GetUserRanking_RecalculatesThenReturnsEntry()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 20;
            const int userId = 5;

            SetupRecalculate(attemptRepo, leaderboardRepo, testId,
                new List<TestAttempt> { MakeAttempt(testId, userId, 82m) });

            var expectedEntry = MakeEntry(testId, userId, 82m, 1);
            leaderboardRepo
                .Setup(repository => repository.FindUserEntryAsync(userId, testId))
                .ReturnsAsync(expectedEntry);

            var result = await leaderboardService.GetUserRankingAsync(userId, testId);

            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetUserRanking_WhenUserHasNoEntry_ReturnsNull()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 21;
            const int userId = 99;

            SetupRecalculate(attemptRepo, leaderboardRepo, testId, new List<TestAttempt>());

            leaderboardRepo
                .Setup(repo => repo.FindUserEntryAsync(userId, testId))
                .ReturnsAsync((LeaderboardEntry?)null);

            var result = await leaderboardService.GetUserRankingAsync(userId, testId);

            Assert.Null(result);
        }

        #endregion

        #region GetFullLeaderboard

        [Fact]
        public async Task GetFullLeaderboard_RecalculatesThenReturnsAll()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 30;

            var attempts = new List<TestAttempt>
            {
                MakeAttempt(testId, 1, 91m),
                MakeAttempt(testId, 2, 78m),
                MakeAttempt(testId, 3, 55m),
                MakeAttempt(testId, 4, 30m),
            };

            SetupRecalculate(attemptRepo, leaderboardRepo, testId, attempts);

            var fullBoard = new List<LeaderboardEntry>
            {
                MakeEntry(testId, 1, 91m, 1),
                MakeEntry(testId, 2, 78m, 2),
                MakeEntry(testId, 3, 55m, 3),
                MakeEntry(testId, 4, 30m, 4)
            };

            leaderboardRepo
                .Setup(r => r.FindByTestIdAsync(testId))
                .ReturnsAsync(fullBoard);

            var result = await leaderboardService.GetFullLeaderboardAsync(testId);

            Assert.Equal(4, result.Count);
            leaderboardRepo.Verify(r => r.FindByTestIdAsync(testId), Times.Once);
        }

        [Fact]
        public async Task GetFullLeaderboard_WhenNoAttempts_ReturnsEmptyList()
        {
            var (attemptRepo, leaderboardRepo, leaderboardService) = CreateSetupMocksAndService();
            const int testId = 31;

            SetupRecalculate(attemptRepo, leaderboardRepo, testId, new List<TestAttempt>());

            leaderboardRepo
                .Setup(r => r.FindByTestIdAsync(testId))
                .ReturnsAsync(new List<LeaderboardEntry>());

            var result = await leaderboardService.GetFullLeaderboardAsync(testId);

            Assert.Empty(result);
        }
        #endregion
    }
}