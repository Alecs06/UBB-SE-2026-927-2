// <copyright file="MainTestViewModelTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TestsAndInterviews.Tests.ViewModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml;
    using Moq;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.ViewModels;
    using Xunit;

    public class MainTestViewModelTests
    {
        private readonly Mock<ITestRepository> mockTestRepository;

        public MainTestViewModelTests()
        {
            this.mockTestRepository = new Mock<ITestRepository>();

            this.mockTestRepository
                .Setup(testRepository => testRepository.FindTestsByCategoryAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<Test>());
        }

        private MainTestViewModel CreateViewModel()
        {
            return new MainTestViewModel(this.mockTestRepository.Object);
        }

        [Fact]
        public async Task LoadTestsAsync_WhenTestsExist_PopulatesTests()
        {
            var tests = new List<Test>
            {
                new Test
                {
                    Id = 1,
                    Title = "C# Basics",
                    Category = "Programming",
                    Questions = new List<Question>
                    {
                        new Question { QuestionTypeString = "SINGLE_CHOICE" },
                    },
                },
            };

            this.mockTestRepository
                .Setup(testRepository => testRepository.FindTestsByCategoryAsync("Programming"))
                .ReturnsAsync(tests);

            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadTestsAsync();

            Assert.Single(viewmodel.Tests);
            Assert.Equal("C# Basics", viewmodel.Tests[0].Title);
            Assert.Equal("SINGLE/CHOICE", viewmodel.Tests[0].QuestionTypeLabel);
        }

        [Fact]
        public async Task LoadTestsAsync_WhenTestHasNoQuestions_SetsTypeLabelToMixed()
        {
            var tests = new List<Test>
            {
                new Test
                {
                    Id = 1,
                    Title = "Empty Test",
                    Category = "Programming",
                    Questions = new List<Question>(),
                },
            };

            this.mockTestRepository
                .Setup(testRepository => testRepository.FindTestsByCategoryAsync("Programming"))
                .ReturnsAsync(tests);

            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadTestsAsync();

            Assert.Equal("MIXED", viewmodel.Tests[0].QuestionTypeLabel);
        }

        [Fact]
        public async Task LoadTestsAsync_WhenNoTestsExist_LeavesTestsEmpty()
        {
            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadTestsAsync();

            Assert.Empty(viewmodel.Tests);
        }

        [Fact]
        public async Task LoadTestsAsync_SetsIsLoadingFalseWhenComplete()
        {
            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadTestsAsync();

            Assert.False(viewmodel.IsLoading);
        }

        [Fact]
        public async Task LoadTestsAsync_WhenQuestionsIsNull_SetsTypeLabelToMixed()
        {
            var tests = new List<Test>
            {
                new Test
                {
                    Id = 1,
                    Title = "Null Questions Test",
                    Category = "Programming",
                    Questions = null,
                },
            };

            this.mockTestRepository
                .Setup(testRepository => testRepository.FindTestsByCategoryAsync("Programming"))
                .ReturnsAsync(tests);

            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadTestsAsync();

            Assert.Equal("MIXED", viewmodel.Tests[0].QuestionTypeLabel);
        }

        [Fact]
        public async Task NoTestsVisible_WhenNoTestsAndNotLoading_ReturnsVisible()
        {
            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadTestsAsync();

            Assert.Equal(Visibility.Visible, viewmodel.NoTestsVisible);
        }

        [Fact]
        public async Task NoTestsVisible_WhenTestsExist_ReturnsCollapsed()
        {
            var tests = new List<Test>
            {
                new Test { Id = 1, Title = "Test", Category = "Programming", Questions = new List<Question>() },
            };

            this.mockTestRepository
                .Setup(testRepository => testRepository.FindTestsByCategoryAsync("Programming"))
                .ReturnsAsync(tests);

            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadTestsAsync();

            Assert.Equal(Visibility.Collapsed, viewmodel.NoTestsVisible);
        }

        [Fact]
        public void NoTestsVisible_WhenIsLoadingTrue_ReturnsCollapsed()
        {
            var viewmodel = this.CreateViewModel();
            viewmodel.IsLoading = true;

            Assert.Equal(Visibility.Collapsed, viewmodel.NoTestsVisible);
        }

        [Fact]
        public void SelectedTest_WhenSet_MarksNewTestAsSelected()
        {
            var viewmodel = this.CreateViewModel();
            var testCard = new TestCardViewModel { TestId = 1 };

            viewmodel.SelectedTest = testCard;

            Assert.True(testCard.IsSelected);
        }

        [Fact]
        public void SelectedTest_WhenChanged_DeselectsPreviousTest()
        {
            var viewmodel = this.CreateViewModel();
            var firstCard = new TestCardViewModel { TestId = 1 };
            var secondCard = new TestCardViewModel { TestId = 2 };

            viewmodel.SelectedTest = firstCard;
            viewmodel.SelectedTest = secondCard;

            Assert.False(firstCard.IsSelected);
            Assert.True(secondCard.IsSelected);
        }

        [Fact]
        public void SelectedTest_WhenSetToNull_DoesNotThrow()
        {
            var viewmodel = this.CreateViewModel();
            var exception = Record.Exception(() => viewmodel.SelectedTest = null);

            Assert.Null(exception);
        }

        [Fact]
        public void SelectedTest_WhenGet_ReturnsCurrentValue()
        {
            var viewmodel = this.CreateViewModel();
            var testCard = new TestCardViewModel { TestId = 1 };
            viewmodel.SelectedTest = testCard;

            Assert.Equal(testCard, viewmodel.SelectedTest);
        }

        [Fact]
        public void OnPropertyChanged_WhenNoListenersAttached_DoesNotThrow()
        {
            var viewmodel = this.CreateViewModel();
            var exception = Record.Exception(() => viewmodel.IsLoading = true);

            Assert.Null(exception);
        }
    }
}