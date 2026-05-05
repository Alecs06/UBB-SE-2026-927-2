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
    using Tests_and_Interviews.Services.Interfaces;
    using Xunit;

    /// <summary>
    /// Contains unit tests for the <see cref="TestService"/> class.
    /// </summary>
    public class TestServiceTests
    {
        private readonly Mock<ITestRepository> mockTestRepository;
        private readonly Mock<ITestAttemptRepository> mockAttemptRepository;
        private readonly Mock<IAnswerRepository> mockAnswerRepository;
        private readonly Mock<IGradingService> mockGradingService;
        private readonly Mock<ITimerService> mockTimerService;
        private readonly Mock<IAttemptValidationService> mockValidationService;
        private readonly Mock<IDataProcessingService> mockDataProcessingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestServiceTests"/> class.
        /// </summary>
        public TestServiceTests()
        {
            this.mockTestRepository = new Mock<ITestRepository>();
            this.mockAttemptRepository = new Mock<ITestAttemptRepository>();
            this.mockAnswerRepository = new Mock<IAnswerRepository>();
            this.mockGradingService = new Mock<IGradingService>();
            this.mockTimerService = new Mock<ITimerService>();
            this.mockValidationService = new Mock<IAttemptValidationService>();
            this.mockDataProcessingService = new Mock<IDataProcessingService>();
        }

        private TestService MakeTestService()
        {
            return new TestService(
                this.mockTestRepository.Object,
                this.mockAttemptRepository.Object,
                this.mockAnswerRepository.Object,
                this.mockGradingService.Object,
                this.mockTimerService.Object,
                this.mockValidationService.Object,
                this.mockDataProcessingService.Object);
        }

        // StartTestAsync tests

        [Fact]
        public async Task StartTestAsync_WhenCalled_CallsValidationService()
        {
            // Arrange
            var testService = this.MakeTestService();

            // Act
            await testService.StartTestAsync(1, 1);

            // Assert
            this.mockValidationService.Verify(
                validationService => validationService.CheckExistingAttemptsAsync(1, 1),
                Times.Once);
        }

        [Fact]
        public async Task StartTestAsync_WhenCalled_SavesNewAttempt()
        {
            // Arrange
            var testService = this.MakeTestService();

            // Act
            await testService.StartTestAsync(1, 1);

            // Assert
            this.mockAttemptRepository.Verify(
                attemptRepository => attemptRepository.SaveAsync(It.IsAny<TestAttempt>()),
                Times.Once);
        }

        [Fact]
        public async Task StartTestAsync_WhenCalled_StartsTimer()
        {
            // Arrange
            var testService = this.MakeTestService();

            // Act
            await testService.StartTestAsync(1, 1);

            // Assert
            this.mockTimerService.Verify(
                timerService => timerService.StartTimer(It.IsAny<int>()),
                Times.Once);
        }

        [Fact]
        public async Task StartTestAsync_WhenCalled_SetsStatusToInProgress()
        {
            // Arrange
            var testService = this.MakeTestService();
            TestAttempt? savedAttempt = null;
            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.SaveAsync(It.IsAny<TestAttempt>()))
                .Callback<TestAttempt>(attempt => savedAttempt = attempt);

            // Act
            await testService.StartTestAsync(1, 1);

            // Assert
            Assert.Equal(TestStatus.IN_PROGRESS.ToString(), savedAttempt!.Status);
        }

        [Fact]
        public async Task StartTestAsync_WhenValidationThrows_DoesNotSaveAttempt()
        {
            // Arrange
            this.mockValidationService
                .Setup(validationService => validationService.CheckExistingAttemptsAsync(1, 1))
                .ThrowsAsync(new System.InvalidOperationException("Already attempted"));

            var testService = this.MakeTestService();

            // Act and Assert
            await Assert.ThrowsAsync<System.InvalidOperationException>(
                () => testService.StartTestAsync(1, 1));

            this.mockAttemptRepository.Verify(
                attemptRepository => attemptRepository.SaveAsync(It.IsAny<TestAttempt>()),
                Times.Never);
        }

        // SubmitTestAsync tests

        [Fact]
        public async Task SubmitTestAsync_WhenCalled_RetrievesAnswers()
        {
            // Arrange
            this.mockAnswerRepository
                .Setup(answerRepository => answerRepository.FindByAttemptAsync(1))
                .ReturnsAsync([]);

            var testService = this.MakeTestService();

            // Act
            await testService.SubmitTestAsync(1);

            // Assert
            this.mockAnswerRepository.Verify(
                answerRepository => answerRepository.FindByAttemptAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task SubmitTestAsync_WhenCalled_UpdatesAttempt()
        {
            // Arrange
            this.mockAnswerRepository
                .Setup(answerRepository => answerRepository.FindByAttemptAsync(1))
                .ReturnsAsync([]);

            var testService = this.MakeTestService();

            // Act
            await testService.SubmitTestAsync(1);

            // Assert
            this.mockAttemptRepository.Verify(
                attemptRepository => attemptRepository.UpdateAsync(It.IsAny<TestAttempt>()),
                Times.Once);
        }

        [Fact]
        public async Task SubmitTestAsync_WhenCalled_CalculatesFinalScore()
        {
            // Arrange
            this.mockAnswerRepository
                .Setup(answerRepository => answerRepository.FindByAttemptAsync(1))
                .ReturnsAsync([]);

            var testService = this.MakeTestService();

            // Act
            await testService.SubmitTestAsync(1);

            // Assert
            this.mockGradingService.Verify(
                gradingService => gradingService.CalculateFinalScore(It.IsAny<TestAttempt>()),
                Times.Once);
        }

        [Fact]
        public async Task SubmitTestAsync_WhenAnswerHasSingleChoiceQuestion_GradesSingleChoice()
        {
            // Arrange
            var answers = new List<Answer>
            {
                new Answer
                {
                    Value = "1",
                    Question = new Question
                    {
                        QuestionTypeString = QuestionType.SINGLE_CHOICE.ToString(),
                        QuestionAnswer = "1",
                        QuestionScore = 4f,
                    },
                },
            };

            this.mockAnswerRepository
                .Setup(answerRepository => answerRepository.FindByAttemptAsync(1))
                .ReturnsAsync(answers);

            var testService = this.MakeTestService();

            // Act
            await testService.SubmitTestAsync(1);

            // Assert
            this.mockGradingService.Verify(
                gradingService => gradingService.GradeSingleChoice(
                    It.IsAny<Question>(),
                    It.IsAny<Answer>()),
                Times.Once);
        }

        [Fact]
        public async Task SubmitTestAsync_WhenAnswerHasMultipleChoiceQuestion_GradesMultipleChoice()
        {
            // Arrange
            var answers = new List<Answer>
            {
                new Answer
                {
                    Value = "[0,1]",
                    Question = new Question
                    {
                        QuestionTypeString = QuestionType.MULTIPLE_CHOICE.ToString(),
                        QuestionAnswer = "[0,1]",
                        QuestionScore = 4f,
                    },
                },
            };

            this.mockAnswerRepository
                .Setup(answerRepository => answerRepository.FindByAttemptAsync(1))
                .ReturnsAsync(answers);

            var testService = this.MakeTestService();

            // Act
            await testService.SubmitTestAsync(1);

            // Assert
            this.mockGradingService.Verify(
                gradingService => gradingService.GradeMultipleChoice(
                    It.IsAny<Question>(),
                    It.IsAny<Answer>()),
                Times.Once);
        }

        [Fact]
        public async Task SubmitTestAsync_WhenAnswerHasTextQuestion_GradesText()
        {
            // Arrange
            var answers = new List<Answer>
            {
                new Answer
                {
                    Value = "polymorphism",
                    Question = new Question
                    {
                        QuestionTypeString = QuestionType.TEXT.ToString(),
                        QuestionAnswer = "polymorphism",
                        QuestionScore = 4f,
                    },
                },
            };

            this.mockAnswerRepository
                .Setup(answerRepository => answerRepository.FindByAttemptAsync(1))
                .ReturnsAsync(answers);

            var testService = this.MakeTestService();

            // Act
            await testService.SubmitTestAsync(1);

            // Assert
            this.mockGradingService.Verify(
                gradingService => gradingService.GradeText(
                    It.IsAny<Question>(),
                    It.IsAny<Answer>()),
                Times.Once);
        }

        [Fact]
        public async Task SubmitTestAsync_WhenAnswerHasTrueFalseQuestion_GradesTrueFalse()
        {
            // Arrange
            var answers = new List<Answer>
            {
                new Answer
                {
                    Value = "true",
                    Question = new Question
                    {
                        QuestionTypeString = QuestionType.TRUE_FALSE.ToString(),
                        QuestionAnswer = "true",
                        QuestionScore = 4f,
                    },
                },
            };

            this.mockAnswerRepository
                .Setup(answerRepository => answerRepository.FindByAttemptAsync(1))
                .ReturnsAsync(answers);

            var testService = this.MakeTestService();

            // Act
            await testService.SubmitTestAsync(1);

            // Assert
            this.mockGradingService.Verify(
                gradingService => gradingService.GradeTrueFalse(
                    It.IsAny<Question>(),
                    It.IsAny<Answer>()),
                Times.Once);
        }

        [Fact]
        public async Task SubmitTestAsync_WhenAnswerHasNullQuestion_SkipsGrading()
        {
            // Arrange
            var answers = new List<Answer>
            {
                new Answer { Value = "1", Question = null },
            };

            this.mockAnswerRepository
                .Setup(answerRepository => answerRepository.FindByAttemptAsync(1))
                .ReturnsAsync(answers);

            var testService = this.MakeTestService();

            // Act
            await testService.SubmitTestAsync(1);

            // Assert
            this.mockGradingService.Verify(
                gradingService => gradingService.GradeSingleChoice(
                    It.IsAny<Question>(),
                    It.IsAny<Answer>()),
                Times.Never);
        }

        // GetNextAvailableTestAsync tests

        [Fact]
        public async Task GetNextAvailableTestAsync_WhenTestsExist_ReturnsFirstTest()
        {
            // Arrange
            var tests = new List<Test>
            {
                new Test { Id = 1, Title = "C++ Basics", Category = "Programming" },
                new Test { Id = 2, Title = "C++ Advanced", Category = "Programming" },
            };

            this.mockTestRepository
                .Setup(testRepository => testRepository.FindTestsByCategoryAsync("Programming"))
                .ReturnsAsync(tests);

            var testService = this.MakeTestService();

            // Act
            var result = await testService.GetNextAvailableTestAsync("Programming");

            // Assert
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetNextAvailableTestAsync_WhenNoTestsExist_ReturnsNull()
        {
            // Arrange
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindTestsByCategoryAsync("Programming"))
                .ReturnsAsync([]);

            var testService = this.MakeTestService();

            // Act
            var result = await testService.GetNextAvailableTestAsync("Programming");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetNextAvailableTestAsync_WhenCalled_CallsRepositoryWithCorrectCategory()
        {
            // Arrange
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindTestsByCategoryAsync("Database"))
                .ReturnsAsync([]);

            var testService = this.MakeTestService();

            // Act
            await testService.GetNextAvailableTestAsync("Database");

            // Assert
            this.mockTestRepository.Verify(
                testRepository => testRepository.FindTestsByCategoryAsync("Database"),
                Times.Once);
        }
    }
}
