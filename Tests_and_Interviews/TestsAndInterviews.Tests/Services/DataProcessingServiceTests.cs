namespace Tests_and_Interviews.Tests.Services
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services;
    using Xunit;

    public class DataProcessingServiceTests
    {
        #region helpers
        private static (
            Mock<IUserRepository> userRepo,
            Mock<ITestAttemptRepository> attemptRepo,
            Mock<ITestRepository> testRepo,
            DataProcessingService DataProcessingService)
            CreateMockData()
        {
            var userRepo = new Mock<IUserRepository>(MockBehavior.Strict);
            var attemptRepo = new Mock<ITestAttemptRepository>(MockBehavior.Strict);
            var testRepo = new Mock<ITestRepository>(MockBehavior.Strict);
            var dataProcessingService = new DataProcessingService(userRepo.Object, attemptRepo.Object, testRepo.Object);
            return (userRepo, attemptRepo, testRepo, dataProcessingService);
        }

        private static TestAttempt MakeValidAttempt() =>
            new TestAttempt
            {
                Id = 1,
                TestId = 10,
                ExternalUserId = 5,
                Score = 80m,
                Status = "COMPLETED",
                CompletedAt = DateTime.UtcNow,
            };

        private static User MakeUser() =>
            new User(5, string.Empty, string.Empty);

        private static Test MakeRecentTest() =>
            new Test { Id = 10, CreatedAt = DateTime.UtcNow };

        private static Test MakeExpiredTest() =>
            new Test { Id = 10, CreatedAt = DateTime.UtcNow.AddMonths(-4) };

        /// <summary>
        /// Sets up all dependencies needed to reach the happy path.
        /// Individual tests break exactly one thing on top of this.
        /// </summary>
        private static void SetupValidFlow(
            Mock<IUserRepository> userRepo,
            Mock<ITestAttemptRepository> attemptRepo,
            Mock<ITestRepository> testRepo,
            TestAttempt attempt,
            User user,
            Test test)
        {
            attemptRepo
                .Setup(repo => repo.FindByIdAsync(attempt.Id))
                .ReturnsAsync(attempt);
            userRepo
                .Setup(repo => repo.GetByIdAsync(attempt.ExternalUserId!.Value))
                .ReturnsAsync(user);
            testRepo
                .Setup(repo => repo.FindByIdAsync(attempt.TestId))
                .ReturnsAsync(test);
            attemptRepo
                .Setup(repo => repo.UpdateAsync(attempt))
                .ReturnsAsync(attempt);
        }
        #endregion

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenAttemptNotFound_ReturnsFalse()
        {
            var (_, attemptRepo, _, dataProcessingService) = CreateMockData();

            attemptRepo
                .Setup(repo => repo.FindByIdAsync(1))
                .ReturnsAsync((TestAttempt?)null);

            var result = await dataProcessingService.ProcessFinalizedAttemptAsync(1);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenExternalUserIdIsNull_ReturnsFalse()
        {
            var (_, attemptRepo, _, dataProcessingService) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.ExternalUserId = null;

            attemptRepo
                .Setup(repo => repo.FindByIdAsync(attempt.Id))
                .ReturnsAsync(attempt);
            attemptRepo
                .Setup(repo => repo.UpdateAsync(attempt))
                .ReturnsAsync(attempt);

            var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenUserNotFound_ReturnsFalse()
        {
            var (userRepo, attemptRepo, _, dataProcessingService) = CreateMockData();
            var attempt = MakeValidAttempt();

            attemptRepo
                .Setup(repo => repo.FindByIdAsync(attempt.Id))
                .ReturnsAsync(attempt);
            userRepo
                .Setup(repo => repo.GetByIdAsync(attempt.ExternalUserId!.Value))
                .ReturnsAsync((User?)null);
            attemptRepo
                .Setup(repo => repo.UpdateAsync(attempt))
                .ReturnsAsync(attempt);

            var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenTestNotFound_ReturnsFalse()
        {
            var (userRepo, attemptRepo, testRepo, dataProcessingService) = CreateMockData();
            var attempt = MakeValidAttempt();

            attemptRepo
                .Setup(repo => repo.FindByIdAsync(attempt.Id))
                .ReturnsAsync(attempt);
            userRepo
                .Setup(repo => repo.GetByIdAsync(attempt.ExternalUserId!.Value))
                .ReturnsAsync(MakeUser());
            testRepo
                .Setup(repo => repo.FindByIdAsync(attempt.TestId))
                .ReturnsAsync((Test?)null);
            attemptRepo
                .Setup(repo => repo.UpdateAsync(attempt))
                .ReturnsAsync(attempt);

            var result = await dataProcessingService.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenCompletedAtIsNull_ReturnsFalse()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.CompletedAt = null;

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenStatusIsNull_ReturnsFalse()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.Status = null;

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenStatusIsWhitespace_ReturnsFalse()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.Status = "   ";

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenStatusIsNotCompleted_ReturnsFalse()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.Status = "IN_PROGRESS";

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenScoreIsNegative_ReturnsFalse()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.Score = -1m;

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenScoreExceedsMaximum_ReturnsFalse()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.Score = 101m;

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenTestIsExpired_ReturnsFalse()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeExpiredTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenAttemptIsValid_ReturnsTrue()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenAttemptIsValid_SetsIsValidatedTrue()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.True(attempt.IsValidated);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenAttemptIsValid_SetsPercentageScore()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.Score = 80m;

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.Equal(80m, attempt.PercentageScore);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenAttemptIsValid_ClearsRejectionReason()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.RejectionReason = "previously rejected";

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.Null(attempt.RejectionReason);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenAttemptIsValid_ClearsRejectedAt()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.RejectedAt = DateTime.UtcNow.AddDays(-1);

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.Null(attempt.RejectedAt);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenValidationFails_SetsIsValidatedFalse()
        {
            var (_, attemptRepo, _, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.ExternalUserId = null;

            attemptRepo
                .Setup(repo => repo.FindByIdAsync(attempt.Id))
                .ReturnsAsync(attempt);
            attemptRepo
                .Setup(repo => repo.UpdateAsync(attempt))
                .ReturnsAsync(attempt);

            await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.False(attempt.IsValidated);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenValidationFails_ClearsPercentageScore()
        {
            var (_, attemptRepo, _, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.ExternalUserId = null;
            attempt.PercentageScore = 80m;

            attemptRepo
                .Setup(repo => repo.FindByIdAsync(attempt.Id))
                .ReturnsAsync(attempt);
            attemptRepo
                .Setup(repo => repo.UpdateAsync(attempt))
                .ReturnsAsync(attempt);

            await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.Null(attempt.PercentageScore);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenValidationFails_SetsRejectedAt()
        {
            var (_, attemptRepo, _, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.ExternalUserId = null;

            attemptRepo
                .Setup(repo => repo.FindByIdAsync(attempt.Id))
                .ReturnsAsync(attempt);
            attemptRepo
                .Setup(repo => repo.UpdateAsync(attempt))
                .ReturnsAsync(attempt);

            var before = DateTime.UtcNow;
            await sut.ProcessFinalizedAttemptAsync(attempt.Id);
            var after = DateTime.UtcNow;

            Assert.InRange(attempt.RejectedAt!.Value, before, after);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenValidationFails_CallsUpdate()
        {
            var (_, attemptRepo, _, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.ExternalUserId = null;

            attemptRepo
                .Setup(repo => repo.FindByIdAsync(attempt.Id))
                .ReturnsAsync(attempt);
            attemptRepo
                .Setup(repo => repo.UpdateAsync(attempt))
                .ReturnsAsync(attempt);

            await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            attemptRepo.Verify(repo => repo.UpdateAsync(attempt), Times.Once);
        }

        [Fact]
        public async Task ProcessFinalizedAttempt_WhenStatusIsCompletedLowercase_ReturnsTrue()
        {
            var (userRepo, attemptRepo, testRepo, sut) = CreateMockData();
            var attempt = MakeValidAttempt();
            attempt.Status = "completed";

            SetupValidFlow(userRepo, attemptRepo, testRepo, attempt, MakeUser(), MakeRecentTest());

            var result = await sut.ProcessFinalizedAttemptAsync(attempt.Id);

            Assert.True(result);
        }
    }
}