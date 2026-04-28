using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsAndInterviews.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services;
    using Xunit;

    /// <summary>
    /// Contains unit tests for the <see cref="TimerService"/> class.
    /// </summary>
    public class TimerServiceTests
    {
        private static TimerService MakeTimerService(ITestAttemptRepository attemptRepository)
        {
            return new TimerService(attemptRepository);
        }

        [Fact]
        public void CheckExpiration_WhenTimerNotStarted_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            var timerService = MakeTimerService(mockRepository.Object);

            // Act
            bool result = timerService.CheckExpiration(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CheckExpiration_WhenTimerJustStarted_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            var timerService = MakeTimerService(mockRepository.Object);
            timerService.StartTimer(1);

            // Act
            bool result = timerService.CheckExpiration(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetExpiredAttemptIds_WhenNoTimersStarted_ReturnsEmptyList()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            var timerService = MakeTimerService(mockRepository.Object);

            // Act
            List<int> result = timerService.GetExpiredAttemptIds();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetExpiredAttemptIds_WhenTimerJustStarted_DoesNotIncludeAttempt()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            var timerService = MakeTimerService(mockRepository.Object);
            timerService.StartTimer(2);

            // Act
            List<int> result = timerService.GetExpiredAttemptIds();

            // Assert
            Assert.DoesNotContain(2, result);
        }

        [Fact]
        public async Task ExpireTestAsync_WhenCalled_UpdatesAttemptInRepository()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            var timerService = MakeTimerService(mockRepository.Object);
            timerService.StartTimer(1);

            // Act
            await timerService.ExpireTestAsync(1);

            // Assert
            mockRepository.Verify(
                repository => repository.UpdateAsync(It.IsAny<TestAttempt>()),
                Times.Once);
        }

        [Fact]
        public async Task ExpireTestAsync_WhenCalled_SetsStatusToCompleted()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            TestAttempt? updatedAttempt = null;
            mockRepository
                .Setup(repository => repository.UpdateAsync(It.IsAny<TestAttempt>()))
                .Callback<TestAttempt>(attempt => updatedAttempt = attempt);

            var timerService = MakeTimerService(mockRepository.Object);
            timerService.StartTimer(1);

            // Act
            await timerService.ExpireTestAsync(1);

            // Assert
            Assert.Equal(TestStatus.COMPLETED.ToString(), updatedAttempt!.Status);
        }

        [Fact]
        public async Task ExpireTestAsync_WhenCalled_SetsCompletedAt()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            TestAttempt? updatedAttempt = null;
            mockRepository
                .Setup(repository => repository.UpdateAsync(It.IsAny<TestAttempt>()))
                .Callback<TestAttempt>(attempt => updatedAttempt = attempt);

            var timerService = MakeTimerService(mockRepository.Object);
            timerService.StartTimer(1);

            // Act
            await timerService.ExpireTestAsync(1);

            // Assert
            Assert.NotNull(updatedAttempt!.CompletedAt);
        }

        [Fact]
        public async Task ExpireTestAsync_WhenCalled_RemovesTimerFromActiveTimers()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            var timerService = MakeTimerService(mockRepository.Object);
            timerService.StartTimer(1);

            // Act
            await timerService.ExpireTestAsync(1);

            // Assert
            Assert.False(timerService.CheckExpiration(1));
        }

        [Fact]
        public void StartTimer_WhenCalledTwiceForSameAttempt_OverwritesPreviousTimer()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            var timerService = MakeTimerService(mockRepository.Object);

            // Act
            timerService.StartTimer(1);
            timerService.StartTimer(1);

            // Assert
            Assert.False(timerService.CheckExpiration(1));
        }
    }
}
