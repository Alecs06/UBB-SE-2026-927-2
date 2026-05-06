// <copyright file="TestPageViewModelTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TestsAndInterviews.Tests.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Moq;
    using Xunit;

    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services;
    using Tests_and_Interviews.Services.Interfaces;
    using Tests_and_Interviews.ViewModels;

    public class TestPageViewModelTests
    {
        private readonly Mock<IUserRepository> mockUserRepository;

        private readonly Mock<ITestRepository> mockTestRepository;

        private readonly Mock<IQuestionRepository> mockQuestionRepository;

        private readonly Mock<ITestAttemptRepository> mockAttemptRepository;

        private readonly Mock<IAnswerRepository> mockAnswerRepository;

        private readonly Mock<ITestService> mockTestService;

        private readonly Mock<IDataProcessingService> mockDataProcessingService;

        public TestPageViewModelTests()
        {
            this.mockUserRepository = new Mock<IUserRepository>();
            this.mockTestRepository = new Mock<ITestRepository>();
            this.mockQuestionRepository = new Mock<IQuestionRepository>();
            this.mockAttemptRepository = new Mock<ITestAttemptRepository>();
            this.mockAnswerRepository = new Mock<IAnswerRepository>();
            this.mockTestService = new Mock<ITestService>();
            this.mockDataProcessingService = new Mock<IDataProcessingService>();

            this.mockQuestionRepository
                .Setup(questionRepository => questionRepository.FindByTestIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Question>());
        }

        [Fact]
        public async Task LoadAsync_WhenTestNotFound_DoesNotLoadQuestions()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync((Test?)null);

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.Empty(viewModel.Questions);
        }

        [Fact]
        public async Task LoadAsync_WhenTestFound_SetsTestTitle()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "C# Basics",
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.Equal("C# Basics", viewModel.TestTitle);
        }

        [Fact]
        public async Task LoadAsync_WhenUserIdIsZero_LooksUpUserByName()
        {
            var users = new List<User>
            {
                new User(5, "Alice Johnson", string.Empty),
            };

            this.mockUserRepository
                .Setup(userRepository => userRepository.GetAllAsync())
                .ReturnsAsync(users);

            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 0);

            Assert.Equal(5, viewModel.UserId);
        }

        [Fact]
        public async Task LoadAsync_WhenUserIdIsZeroAndAliceNotFound_SetsUserIdToZero()
        {
            this.mockUserRepository
                .Setup(userRepository => userRepository.GetAllAsync())
                .ReturnsAsync(new List<User>());

            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 0);

            Assert.Equal(0, viewModel.UserId);
        }

        [Fact]
        public async Task LoadAsync_WhenAlreadyAttempted_SetsAlreadyAttemptedTrue()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockTestService
                .Setup(testService => testService.StartTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("Already attempted"));

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.True(viewModel.AlreadyAttempted);
        }

        [Fact]
        public async Task LoadAsync_WhenStartTestThrowsGenericException_ContinuesLoading()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockTestService
                .Setup(testService => testService.StartTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Generic error"));

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.False(viewModel.AlreadyAttempted);
        }

        [Fact]
        public async Task LoadAsync_WhenQuestionsExist_PopulatesQuestions()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockQuestionRepository
                .Setup(questionRepository => questionRepository.FindByTestIdAsync(1))
                .ReturnsAsync(new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        QuestionText = "What is C#?",
                        QuestionTypeString = "TEXT",
                    },
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.Single(viewModel.Questions);
        }

        [Fact]
        public async Task LoadAsync_WhenQuestionIsInterview_SkipsIt()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockQuestionRepository
                .Setup(questionRepository => questionRepository.FindByTestIdAsync(1))
                .ReturnsAsync(new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        QuestionTypeString = "INTERVIEW",
                    },
                    new Question
                    {
                        Id = 2,
                        QuestionTypeString = "TEXT",
                    },
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.Single(viewModel.Questions);
        }

        [Fact]
        public async Task LoadAsync_WhenSingleChoiceWithOptionsJson_PopulatesOptions()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockQuestionRepository
                .Setup(questionRepository => questionRepository.FindByTestIdAsync(1))
                .ReturnsAsync(new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        QuestionTypeString = "SINGLE_CHOICE",
                        OptionsJson = "[\"Option A\",\"Option B\"]",
                    },
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.Equal(2, viewModel.Questions[0].Options.Count);
        }

        [Fact]
        public async Task LoadAsync_WhenSingleChoiceWithNoOptionsJson_UsesDefaults()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockQuestionRepository
                .Setup(questionRepository => questionRepository.FindByTestIdAsync(1))
                .ReturnsAsync(new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        QuestionTypeString = "SINGLE_CHOICE",
                        OptionsJson = null,
                    },
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.Equal(6, viewModel.Questions[0].Options.Count);
        }

        [Fact]
        public async Task LoadAsync_WhenMultipleChoiceWithOptionsJson_PopulatesOptions()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockQuestionRepository
                .Setup(questionRepository => questionRepository.FindByTestIdAsync(1))
                .ReturnsAsync(new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        QuestionTypeString = "MULTIPLE_CHOICE",
                        OptionsJson = "[\"Option A\",\"Option B\",\"Option C\"]",
                    },
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.Equal(3, viewModel.Questions[0].Options.Count);
        }

        [Fact]
        public async Task LoadAsync_WhenStartTestThrowsExceptionWithInnerException_ContinuesLoading()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockTestService
                .Setup(testService => testService.StartTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Outer", new Exception("Inner")));

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            Assert.False(viewModel.AlreadyAttempted);
        }

        [Fact]
        public void StopTimer_WhenTimerIsNull_DoesNotThrow()
        {
            var viewModel = this.CreateViewModel();

            var exception = Record.Exception(() => viewModel.StopTimer());

            Assert.Null(exception);
        }

        [Fact]
        public async Task SubmitAsync_WhenAttemptNotFound_ReturnsZero()
        {
            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByUserAndTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((TestAttempt?)null);

            var viewModel = this.CreateViewModel();

            var result = await viewModel.SubmitAsync();

            Assert.Equal(0f, result);
        }

        [Fact]
        public async Task SubmitAsync_WhenAttemptFound_CallsSubmitTestAsync()
        {
            var attempt = new TestAttempt { Id = 1 };

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByUserAndTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockDataProcessingService
                .Setup(dataProcessingService => dataProcessingService.ProcessFinalizedAttemptAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var viewModel = this.CreateViewModel();

            await viewModel.SubmitAsync();

            this.mockTestService.Verify(
                testService => testService.SubmitTestAsync(attempt.Id),
                Times.Once);
        }

        [Fact]
        public async Task SubmitAsync_WhenAnsweredQuestionsExist_SavesAnswers()
        {
            var attempt = new TestAttempt { Id = 1 };

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByUserAndTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockDataProcessingService
                .Setup(dataProcessingService => dataProcessingService.ProcessFinalizedAttemptAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var viewModel = this.CreateViewModel();

            var answeredQuestion = new QuestionViewModel
            {
                QuestionId = 1,
                Type = QuestionType.TEXT,
                TextAnswer = "My answer",
            };

            viewModel.Questions.Add(answeredQuestion);

            await viewModel.SubmitAsync();

            this.mockAnswerRepository.Verify(
                answerRepository => answerRepository.SaveAsync(It.IsAny<Answer>()),
                Times.Once);
        }

        [Fact]
        public async Task SubmitAsync_WhenUnansweredQuestionsExist_DoesNotSaveAnswers()
        {
            var attempt = new TestAttempt { Id = 1 };

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByUserAndTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockDataProcessingService
                .Setup(dataProcessingService => dataProcessingService.ProcessFinalizedAttemptAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var viewModel = this.CreateViewModel();

            var unansweredQuestion = new QuestionViewModel
            {
                QuestionId = 1,
                Type = QuestionType.TEXT,
                TextAnswer = string.Empty,
            };

            viewModel.Questions.Add(unansweredQuestion);

            await viewModel.SubmitAsync();

            this.mockAnswerRepository.Verify(
                answerRepository => answerRepository.SaveAsync(It.IsAny<Answer>()),
                Times.Never);
        }

        [Fact]
        public async Task AnsweredCount_WhenQuestionIsAnswered_UpdatesCount()
        {
            this.mockTestRepository
                .Setup(testRepository => testRepository.FindByIdAsync(1))
                .ReturnsAsync(new Test
                {
                    Id = 1,
                    Title = "Test",
                });

            this.mockQuestionRepository
                .Setup(questionRepository => questionRepository.FindByTestIdAsync(1))
                .ReturnsAsync(new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        QuestionTypeString = "TEXT",
                    },
                });

            var viewModel = this.CreateViewModel();

            await viewModel.LoadAsync(1, 1);

            viewModel.Questions[0].TextAnswer = "My answer";

            Assert.Equal(1, viewModel.AnsweredCount);
        }

        [Fact]
        public async Task SubmitAsync_WhenFinalAttemptFound_ReturnsScore()
        {
            var attempt = new TestAttempt
            {
                Id = 1,
                Score = 85m,
            };

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByUserAndTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockDataProcessingService
                .Setup(dataProcessingService => dataProcessingService.ProcessFinalizedAttemptAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var viewModel = this.CreateViewModel();

            var result = await viewModel.SubmitAsync();

            Assert.Equal(85f, result);
        }

        [Fact]
        public async Task SubmitAsync_WhenFinalAttemptIsNull_ReturnsZero()
        {
            var attempt = new TestAttempt { Id = 1 };

            this.mockAttemptRepository
                .SetupSequence(attemptRepository => attemptRepository.FindByUserAndTestAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(attempt)
                .ReturnsAsync((TestAttempt?)null);

            this.mockAttemptRepository
                .Setup(attemptRepository => attemptRepository.FindByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(attempt);

            this.mockDataProcessingService
                .Setup(dataProcessingService => dataProcessingService.ProcessFinalizedAttemptAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var viewModel = this.CreateViewModel();

            var result = await viewModel.SubmitAsync();

            Assert.Equal(0f, result);
        }

        [Fact]
        public void TotalCount_ReturnsQuestionsCount()
        {
            var viewModel = this.CreateViewModel();

            viewModel.Questions.Add(new QuestionViewModel { Type = QuestionType.TEXT });
            viewModel.Questions.Add(new QuestionViewModel { Type = QuestionType.TEXT });

            Assert.Equal(2, viewModel.TotalCount);
        }

        [Fact]
        public void TimerDisplay_ReturnsFormattedTime()
        {
            var viewModel = this.CreateViewModel();

            Assert.Matches(@"^\d{2}:\d{2}$", viewModel.TimerDisplay);
        }

        [Fact]
        public void Notify_WhenNoListenersAttached_DoesNotThrow()
        {
            var viewModel = this.CreateViewModel();

            var exception = Record.Exception(() => viewModel.TestTitle = "Test");

            Assert.Null(exception);
        }

        private TestPageViewModel CreateViewModel()
        {
            return new TestPageViewModel(
                this.mockUserRepository.Object,
                this.mockTestRepository.Object,
                this.mockQuestionRepository.Object,
                this.mockAttemptRepository.Object,
                this.mockAnswerRepository.Object,
                this.mockTestService.Object,
                this.mockDataProcessingService.Object);
        }
    }
}