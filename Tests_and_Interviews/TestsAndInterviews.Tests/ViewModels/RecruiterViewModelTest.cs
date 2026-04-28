// <copyright file="RecruiterViewModelTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TestsAndInterviews.Tests.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services;
    using Tests_and_Interviews.ViewModels;
    using Xunit;

    public class RecruiterViewModelTests
    {
        private readonly Mock<ISlotService> mockSlotService;
        private readonly Mock<IInterviewSessionRepository> mockSessionRepository;

        public RecruiterViewModelTests()
        {
            this.mockSlotService = new Mock<ISlotService>();
            this.mockSessionRepository = new Mock<IInterviewSessionRepository>();

            this.mockSlotService
                .Setup(slotService => slotService.LoadRecruiterVisibleSlotsAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<SlotDto>());

            this.mockSessionRepository
                .Setup(sessionRepository => sessionRepository.GetSessionsByStatusAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<InterviewSession>());
        }

        private RecruiterViewModel CreateViewModel()
        {
            return new RecruiterViewModel(
                this.mockSlotService.Object,
                this.mockSessionRepository.Object);
        }

        [Fact]
        public async Task LoadSlotsAsync_WhenSlotsExist_PopulatesSlots()
        {
            var slots = new List<SlotDto>
            {
                new SlotDto { Id = 1, StartTime = DateTime.Today },
                new SlotDto { Id = 2, StartTime = DateTime.Today.AddHours(1) },
            };
            this.mockSlotService
                .Setup(slotService => slotService.LoadRecruiterVisibleSlotsAsync(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(slots);

            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadSlotsAsync();

            Assert.Equal(2, viewmodel.Slots.Count);
        }

        [Fact]
        public async Task LoadPendingReviewsAsync_WhenSessionsExist_PopulatesPendingReviews()
        {
            var sessions = new List<InterviewSession>
            {
                new InterviewSession { Id = 1 },
                new InterviewSession { Id = 2 },
            };
            this.mockSessionRepository
                .Setup(sessionRepository => sessionRepository.GetSessionsByStatusAsync(It.IsAny<string>()))
                .ReturnsAsync(sessions);

            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadPendingReviewsAsync();

            Assert.Equal(2, viewmodel.PendingReviews.Count);
        }

        [Fact]
        public async Task LoadPendingReviewsAsync_WhenRepositoryThrows_LeavesPendingReviewsEmpty()
        {
            this.mockSessionRepository
                .Setup(sessionRepository => sessionRepository.GetSessionsByStatusAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            var viewmodel = this.CreateViewModel();
            await viewmodel.LoadPendingReviewsAsync();

            Assert.Empty(viewmodel.PendingReviews);
        }

        [Fact]
        public void LoadPendingReviews_WhenCalled_DoesNotThrow()
        {
            var viewmodel = this.CreateViewModel();
            var exception = Record.Exception(() => viewmodel.LoadPendingReviews());

            Assert.Null(exception);
        }

        [Fact]
        public async Task CreateSlotAsync_WhenCalled_CallsServiceAndReloadsSlots()
        {
            var slot = new SlotDto { Id = 1, StartTime = DateTime.Today };
            var viewmodel = this.CreateViewModel();

            await viewmodel.CreateSlotAsync(slot, 60);

            this.mockSlotService.Verify(
                slotService => slotService.CreateRecruiterSlotAsync(slot, 60),
                Times.Once);
            this.mockSlotService.Verify(
                slotService => slotService.LoadRecruiterVisibleSlotsAsync(It.IsAny<int>(), It.IsAny<DateTime>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task DeleteSlotAsync_WhenCalled_CallsServiceAndReloadsSlots()
        {
            var viewmodel = this.CreateViewModel();

            await viewmodel.DeleteSlotAsync(1);

            this.mockSlotService.Verify(
                slotService => slotService.DeleteRecruiterSlotAsync(1),
                Times.Once);
            this.mockSlotService.Verify(
                slotService => slotService.LoadRecruiterVisibleSlotsAsync(It.IsAny<int>(), It.IsAny<DateTime>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task UpdateSlotAsync_WhenCalled_CallsServiceWithCorrectStartTimeAndReloadsSlots()
        {
            var slot = new SlotDto { Id = 1, StartTime = DateTime.Today };
            var newStartTime = new TimeSpan(10, 0, 0);
            var viewmodel = this.CreateViewModel();

            await viewmodel.UpdateSlotAsync(slot, newStartTime, 60);

            this.mockSlotService.Verify(
                slotService => slotService.UpdateRecruiterSlotAsync(
                    slot,
                    DateTime.Today.Date + newStartTime,
                    60),
                Times.Once);
        }

        [Fact]
        public void SelectedDate_WhenChanged_TriggersSlotReload()
        {
            var viewmodel = this.CreateViewModel();
            viewmodel.SelectedDate = DateTime.Today.AddDays(1);

            this.mockSlotService.Verify(
                slotService => slotService.LoadRecruiterVisibleSlotsAsync(It.IsAny<int>(), It.IsAny<DateTime>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public void SelectedDateFormatted_ReturnsCorrectFormat()
        {
            var viewmodel = this.CreateViewModel();
            var date = new DateTime(2025, 4, 14);
            viewmodel.SelectedDate = date;

            Assert.Equal(date.ToString("dddd dd/MM/yyyy"), viewmodel.SelectedDateFormatted);
        }

        [Fact]
        public void OnPropertyChanged_WhenNoListenersAttached_DoesNotThrow()
        {
            var viewmodel = this.CreateViewModel();
            var exception = Record.Exception(() => viewmodel.SelectedDate = DateTime.Today.AddDays(1));

            Assert.Null(exception);
        }
    }
}