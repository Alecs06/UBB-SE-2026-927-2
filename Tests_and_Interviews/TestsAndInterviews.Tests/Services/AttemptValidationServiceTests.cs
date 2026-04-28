using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsAndInterviews.Tests.Services
{
    using System.Threading.Tasks;
    using Moq;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services;
    using Xunit;

    /// <summary>
    /// Contains unit tests for the <see cref="AttemptValidationService"/> class.
    /// </summary>
    public class AttemptValidationServiceTests
    {
        private static AttemptValidationService MakeAttemptValidationService(ITestAttemptRepository attemptRepository)
        {
            return new AttemptValidationService(attemptRepository);
        }

        private static TestAttempt MakeTestAttempt()
        {
            return new TestAttempt
            {
                Id = 1,
                TestId = 1,
                ExternalUserId = 1,
            };
        }

        [Fact]
        public async Task CanStartTestAsync_WhenNoExistingAttempt_ReturnsTrue()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            mockRepository
                .Setup(repository => repository.FindByUserAndTestAsync(1, 1))
                .ReturnsAsync((TestAttempt?)null);

            var validationService = MakeAttemptValidationService(mockRepository.Object);

            // Act
            bool result = await validationService.CanStartTestAsync(1, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CanStartTestAsync_WhenExistingAttemptFound_ReturnsFalse()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            mockRepository
                .Setup(repository => repository.FindByUserAndTestAsync(1, 1))
                .ReturnsAsync(MakeTestAttempt());

            var validationService = MakeAttemptValidationService(mockRepository.Object);

            // Act
            bool result = await validationService.CanStartTestAsync(1, 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckExistingAttemptsAsync_WhenNoExistingAttempt_DoesNotThrow()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            mockRepository
                .Setup(repository => repository.FindByUserAndTestAsync(1, 1))
                .ReturnsAsync((TestAttempt?)null);

            var validationService = MakeAttemptValidationService(mockRepository.Object);

            // Act
            var exception = await Record.ExceptionAsync(() => validationService.CheckExistingAttemptsAsync(1, 1));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task CheckExistingAttemptsAsync_WhenExistingAttemptFound_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            mockRepository
                .Setup(repository => repository.FindByUserAndTestAsync(1, 1))
                .ReturnsAsync(MakeTestAttempt());

            var validationService = MakeAttemptValidationService(mockRepository.Object);

            // Act and Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(
                () => validationService.CheckExistingAttemptsAsync(1, 1));
        }

        [Fact]
        public async Task CheckExistingAttemptsAsync_WhenExistingAttemptFound_ExceptionMessageContainsUserId()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            mockRepository
                .Setup(repository => repository.FindByUserAndTestAsync(1, 1))
                .ReturnsAsync(MakeTestAttempt());

            var validationService = MakeAttemptValidationService(mockRepository.Object);

            // Act
            var exception = await Record.ExceptionAsync(() => validationService.CheckExistingAttemptsAsync(1, 1));

            // Assert
            Assert.Contains("1", exception!.Message);
        }

        [Fact]
        public async Task CanStartTestAsync_WhenCalled_CallsRepositoryOnce()
        {
            // Arrange
            var mockRepository = new Mock<ITestAttemptRepository>();
            mockRepository
                .Setup(repository => repository.FindByUserAndTestAsync(1, 1))
                .ReturnsAsync((TestAttempt?)null);

            var validationService = MakeAttemptValidationService(mockRepository.Object);

            // Act
            await validationService.CanStartTestAsync(1, 1);

            // Assert
            mockRepository.Verify(
                repository => repository.FindByUserAndTestAsync(1, 1),
                Times.Once);
        }
    }
}
