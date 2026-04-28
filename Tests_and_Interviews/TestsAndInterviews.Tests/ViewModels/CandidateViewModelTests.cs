// <copyright file="CandidateViewModelTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TestsAndInterviews.Tests.ViewModels
{
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Helpers;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services.Interfaces;
    using Tests_and_Interviews.ViewModels;
    using Xunit;

    public class CandidateViewModelTests
    {
        private readonly Mock<IBookingService> mockBookingService;
        private readonly Mock<IInterviewSessionRepository> mockSessionRepository;
        private readonly Mock<INotificationService> mockNotificationService;
        private readonly Mock<ISlotRepository> mockSlotRepository;

        public CandidateViewModelTests()
        {
            this.mockBookingService = new Mock<IBookingService>();
            this.mockSessionRepository = new Mock<IInterviewSessionRepository>();
            this.mockNotificationService = new Mock<INotificationService>();
            this.mockSlotRepository = new Mock<ISlotRepository>();

            this.mockSessionRepository
                .Setup(repository => repository.GetScheduledSessionsAsync())
                .ReturnsAsync(new List<InterviewSession>());
        }

        private CandidateViewModel CreateViewModel()
        {
            return new CandidateViewModel(
                this.mockBookingService.Object,
                this.mockSessionRepository.Object,
                this.mockNotificationService.Object,
                this.mockSlotRepository.Object);
        }

        private CandidateViewModel CreateViewModelWithCompanyAndSlot(Company company, Slot slot)
        {
            this.mockBookingService
                .Setup(b => b.GetAvailableSlotsByRecruiterId(company.RecruiterId))
                .Returns(new List<Slot> { slot });

            var viewmodel = this.CreateViewModel();
            viewmodel.ScheduleInterviewCommand.Execute(company);
            viewmodel.SelectSlotForInterviewCommand.Execute(slot);
            return viewmodel;
        }

        [Fact]
        public void ScheduleInterviewCommand_SetsIsBookingVisibleAndSelectedCompany()
        {
            var company = new Company { RecruiterId = 1 };
            this.mockBookingService
                .Setup(bookingService => bookingService.GetAvailableSlotsByRecruiterId(1))
                .Returns(new List<Slot>());

            var viewmodel = this.CreateViewModel();
            viewmodel.ScheduleInterviewCommand.Execute(company);

            Assert.True(viewmodel.IsBookingVisible);
            Assert.Equal(company, viewmodel.SelectedCompany);
        }

        [Fact]
        public void ScheduleInterviewCommand_WhenObjectIsNotCompany_DoesNothing()
        {
            var viewmode = this.CreateViewModel();
            viewmode.ScheduleInterviewCommand.Execute("not a company");

            Assert.False(viewmode.IsBookingVisible);
        }

        [Fact]
        public void ScheduleInterviewCommand_WhenSelectedCompanyIsNull_DoesNotLoadSlots()
        {
            var viewmodel = this.CreateViewModel();
            viewmodel.SelectedDay = DateTime.Today;

            this.mockBookingService.Verify(
                bookingService => bookingService.GetAvailableSlotsByRecruiterId(It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public void SelectSlotCommand_SetsSelectedSlotAndDeselectsOthers()
        {
            var company = new Company { RecruiterId = 1 };
            var slot1 = new Slot { StartTime = DateTime.Today, Status = SlotStatus.Free };
            var slot2 = new Slot { StartTime = DateTime.Today.AddHours(1), Status = SlotStatus.Free };
            this.mockBookingService
                .Setup(bookingService => bookingService.GetAvailableSlotsByRecruiterId(1))
                .Returns(new List<Slot> { slot1, slot2 });

            var viewmodel = this.CreateViewModel();
            viewmodel.ScheduleInterviewCommand.Execute(company);
            viewmodel.SelectSlotForInterviewCommand.Execute(slot1);
            viewmodel.SelectSlotForInterviewCommand.Execute(slot2);

            Assert.Equal(slot2, viewmodel.SelectedSlot);
            Assert.False(slot1.IsSlotSelected);
            Assert.True(slot2.IsSlotSelected);
        }

        [Fact]
        public void SelectSlotCommand_WhenObjectIsNotSlot_DoesNothing()
        {
            var viewmodel = this.CreateViewModel();
            viewmodel.SelectSlotForInterviewCommand.Execute("not a slot");

            Assert.Null(viewmodel.SelectedSlot);
        }

        [Fact]
        public void SelectDayCommand_SetsSelectedDayAndClearsSelectedSlot()
        {
            var company = new Company { RecruiterId = 1 };
            var slot = new Slot { StartTime = DateTime.Today.AddDays(1), Status = SlotStatus.Free };
            var viewmodel = this.CreateViewModelWithCompanyAndSlot(company, slot);

            viewmodel.SelectDayForInterviewCommand.Execute(slot);

            Assert.Equal(slot.StartTime.Date, viewmodel.SelectedDay.Date);
            Assert.Null(viewmodel.SelectedSlot);
        }

        [Fact]
        public void SelectDayCommand_WhenObjectIsNotSlot_DoesNothing()
        {
            var viewmodel = this.CreateViewModel();
            var originalDay = viewmodel.SelectedDay;
            viewmodel.SelectDayForInterviewCommand.Execute("not a slot");

            Assert.Equal(originalDay, viewmodel.SelectedDay);
        }

        [Fact]
        public void LoadNextDaysCommand_AndPreviousDaysCommand_PaginateCorrectly()
        {
            var company = new Company { RecruiterId = 1 };
            var slots = new List<Slot>
            {
                new Slot { StartTime = DateTime.Today, Status = SlotStatus.Free },
                new Slot { StartTime = DateTime.Today.AddDays(1), Status = SlotStatus.Free },
                new Slot { StartTime = DateTime.Today.AddDays(2), Status = SlotStatus.Free },
                new Slot { StartTime = DateTime.Today.AddDays(3), Status = SlotStatus.Free },
            };
            this.mockBookingService
                .Setup(bookingService => bookingService.GetAvailableSlotsByRecruiterId(1))
                .Returns(slots);

            var viewmodel = this.CreateViewModel();
            viewmodel.ScheduleInterviewCommand.Execute(company);

            var firstVisible = new List<Slot>(viewmodel.VisibleDays)[0].StartTime;
            viewmodel.LoadNextDaysCommand.Execute(null);
            var afterNext = new List<Slot>(viewmodel.VisibleDays)[0].StartTime;
            viewmodel.LoadPreviousDaysCommand.Execute(null);
            var afterPrev = new List<Slot>(viewmodel.VisibleDays)[0].StartTime;

            Assert.NotEqual(firstVisible, afterNext);
            Assert.Equal(firstVisible, afterPrev);
        }

        [Fact]
        public void LoadNextDaysCommand_WhenAtEnd_DoesNotAdvance()
        {
            var viewmodel = this.CreateViewModel();
            var visibleBefore = new List<Slot>(viewmodel.VisibleDays).Count;
            viewmodel.LoadNextDaysCommand.Execute(null);

            Assert.Equal(visibleBefore, new List<Slot>(viewmodel.VisibleDays).Count);
        }

        [Fact]
        public void LoadPreviousDaysCommand_WhenAtStart_DoesNotGoBack()
        {
            var viewmodel = this.CreateViewModel();
            viewmodel.LoadPreviousDaysCommand.Execute(null);

            Assert.Empty(viewmodel.VisibleDays);
        }

        [Fact]
        public async Task ConfirmInterviewCommand_WhenSlotAndCompanySelected_ConfirmsAndHidesBooking()
        {
            var company = new Company { CompanyName = "Google", JobTitle = "Dev", RecruiterId = 1 };
            var slot = new Slot { StartTime = DateTime.Today, EndTime = DateTime.Today.AddHours(1), Status = SlotStatus.Free };
            var viewmodel = this.CreateViewModelWithCompanyAndSlot(company, slot);
            viewmodel.MatchedCompanies.Add(company);

            viewmodel.ConfirmInterviewCommand.Execute(null);
            await Task.Delay(100);

            this.mockBookingService.Verify(bookingService => bookingService.ConfirmBooking(It.IsAny<int>(), slot), Times.Once);
            Assert.False(viewmodel.IsBookingVisible);
            Assert.DoesNotContain(company, viewmodel.MatchedCompanies);
        }

        [Fact]
        public void ConfirmInterviewCommand_WhenNoSlotSelected_DoesNotCallConfirmBooking()
        {
            var viewmodel = this.CreateViewModel();
            viewmodel.ConfirmInterviewCommand.Execute(null);

            this.mockBookingService.Verify(bookingService => bookingService.ConfirmBooking(It.IsAny<int>(), It.IsAny<Slot>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmInterviewCommand_WhenNotificationFails_StillCompletesBooking()
        {
            var company = new Company { CompanyName = "Google", JobTitle = "Dev", RecruiterId = 1 };
            var slot = new Slot { StartTime = DateTime.Today, EndTime = DateTime.Today.AddHours(1), Status = SlotStatus.Free };
            this.mockNotificationService
                .Setup(notificationService => notificationService.ShowBookingConfirmed(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Throws(new Exception("Notification failed"));

            var viewmodel = this.CreateViewModelWithCompanyAndSlot(company, slot);
            viewmodel.MatchedCompanies.Add(company);
            viewmodel.ConfirmInterviewCommand.Execute(null);
            await Task.Delay(100);

            Assert.False(viewmodel.IsBookingVisible);
        }

        [Fact]
        public void ConfirmInterviewCommand_WhenNoCompanySelected_DoesNotCallConfirmBooking()
        {
            var slot = new Slot { Status = SlotStatus.Free };
            var viewmodel = this.CreateViewModel();
            viewmodel.SelectedSlot = slot;

            viewmodel.ConfirmInterviewCommand.Execute(null);

            this.mockBookingService.Verify(
                bookingService => bookingService.ConfirmBooking(It.IsAny<int>(), It.IsAny<Slot>()),
                Times.Never);
        }

        [Fact]
        public void JoinInterviewCommand_WhenObjectIsNull_DoesNothing()
        {
            var viewmodel = this.CreateViewModel();
            var exception = Record.Exception(() => viewmodel.JoinInterviewCommand.Execute(null));

            Assert.Null(exception);
        }

        [Fact]
        public async Task CancelInterviewCommand_WhenSessionExists_DeletesSession()
        {
            var session = new InterviewSession { Id = 1 };
            this.mockSessionRepository
                .Setup(sessionRepository => sessionRepository.GetInterviewSessionByIdAsync(1))
                .ReturnsAsync(session);

            var viewmodel = this.CreateViewModel();
            viewmodel.CancelInterviewCommand.Execute(session);
            await Task.Delay(100);

            this.mockSessionRepository.Verify(sessionRepository => sessionRepository.Delete(session), Times.Once);
        }

        [Fact]
        public async Task CancelInterviewCommand_WhenSessionNotFound_DoesNotDelete()
        {
            var session = new InterviewSession { Id = 99 };
            this.mockSessionRepository
                .Setup(sessionRepository => sessionRepository.GetInterviewSessionByIdAsync(99))
                .ReturnsAsync((InterviewSession?)null);

            var viewmodel = this.CreateViewModel();
            viewmodel.CancelInterviewCommand.Execute(session);
            await Task.Delay(100);

            this.mockSessionRepository.Verify(sessionRepository => sessionRepository.Delete(It.IsAny<InterviewSession>()), Times.Never);
        }

        [Fact]
        public void CancelInterviewCommand_WhenObjectIsNotSession_DoesNothing()
        {
            var viewmodel = this.CreateViewModel();
            viewmodel.CancelInterviewCommand.Execute("not a session");

            this.mockSessionRepository.Verify(sessionRepository => sessionRepository.Delete(It.IsAny<InterviewSession>()), Times.Never);
        }

        [Fact]
        public async Task CancelInterviewCommand_WhenRepositoryThrows_DoesNotCrash()
        {
            var session = new InterviewSession { Id = 1 };
            this.mockSessionRepository
                .Setup(sessionRepository => sessionRepository.GetInterviewSessionByIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            var viewmodel = this.CreateViewModel();
            viewmodel.CancelInterviewCommand.Execute(session);
            await Task.Delay(100);

            this.mockSessionRepository.Verify(sessionRepository => sessionRepository.Delete(It.IsAny<InterviewSession>()), Times.Never);
        }

        [Fact]
        public async Task LoadInterviewSessions_WhenSessionsExist_PopulatesInterviewSessions()
        {
            this.mockSessionRepository
                .Setup(sessionRepository => sessionRepository.GetScheduledSessionsAsync())
                .ReturnsAsync(new List<InterviewSession> { new InterviewSession { Id = 1 } });

            var viewmodel = this.CreateViewModel();
            await Task.Delay(100);

            Assert.Single(viewmodel.InterviewSessions);
        }

        [Fact]
        public async Task LoadInterviewSessions_WhenRepositoryThrows_LeavesSessionsEmpty()
        {
            this.mockSessionRepository
                .Setup(sessionRepository => sessionRepository.GetScheduledSessionsAsync())
                .ThrowsAsync(new Exception("Database error"));

            var viewmodel = this.CreateViewModel();
            await Task.Delay(100);

            Assert.Empty(viewmodel.InterviewSessions);
        }

        [Fact]
        public void LoadAvailableSlotsCommand_WhenExecuted_UpdatesMatchedCompanies()
        {
            var viewmodel = this.CreateViewModel();
            viewmodel.LoadAvailableSlotsCommand.Execute(null);

            Assert.Equal(2, viewmodel.MatchedCompanies.Count);
        }

        [Fact]
        public void OnPropertyChanged_WhenNoListenersAttached_DoesNotThrow()
        {
            var viewmodel = this.CreateViewModel();
            var exception = Record.Exception(() => viewmodel.SelectedSlot = new Slot());

            Assert.Null(exception);
        }
    }
}