// <copyright file="BookingServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TestsAndInterviews.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services;
    using Xunit;

    public class BookingServiceTests
    {
        private readonly Mock<ISlotRepository> mockSlotRepository;
        private readonly Mock<IInterviewSessionRepository> mockInterviewSessionRepository;
        private readonly BookingService bookingService;

        public BookingServiceTests()
        {
            this.mockSlotRepository = new Mock<ISlotRepository>();
            this.mockInterviewSessionRepository = new Mock<IInterviewSessionRepository>();
            this.bookingService = new BookingService(); // Adjusted to use parameterless constructor
        }

        [Fact]
        public async Task GetAvailableSlots_ReturnsOnlyFreeSlots()
        {
            var date = DateTime.Today;
            var slots = new List<Slot>
            {
                new Slot { StartTime = date, Status = SlotStatus.Free },
                new Slot { StartTime = date, Status = SlotStatus.Occupied },
            };
            this.mockSlotRepository
                .Setup(slotRepository => slotRepository.GetSlotsAsync(1, date))
                .ReturnsAsync(slots);

            var result = await this.bookingService.GetAvailableSlots(1, date);

            Assert.Single(result);
            Assert.All(result, slot => Assert.Equal(SlotStatus.Free, slot.Status));
        }

        [Fact]
        public async Task GetAvailableSlots_ReturnsSlotsOrderedByStartTime()
        {
            var date = DateTime.Today;
            var slots = new List<Slot>
            {
                new Slot { StartTime = date.AddHours(3), Status = SlotStatus.Free },
                new Slot { StartTime = date.AddHours(1), Status = SlotStatus.Free },
            };
            this.mockSlotRepository
                .Setup(slotRepository => slotRepository.GetSlotsAsync(1, date))
                .ReturnsAsync(slots);

            var result = await this.bookingService.GetAvailableSlots(1, date);

            Assert.Equal(date.AddHours(1), result[0].StartTime);
            Assert.Equal(date.AddHours(3), result[1].StartTime);
        }

        [Fact]
        public async Task GetAvailableSlotsByRecruiterId_ReturnsOnlyFreeSlots()
        {
            var slots = new List<Slot>
            {
                new Slot { StartTime = DateTime.Today, Status = SlotStatus.Free },
                new Slot { StartTime = DateTime.Today, Status = SlotStatus.Occupied },
            };
            this.mockSlotRepository
                .Setup(slotRepository => slotRepository.GetAllSlotsAsync(1))
                .ReturnsAsync(slots);

            var result = await this.bookingService.GetAvailableSlotsByRecruiterId(1);

            Assert.Single(result);
            Assert.All(result, slot => Assert.Equal(SlotStatus.Free, slot.Status));
        }

        [Fact]
        public async Task GetAvailableSlotsByRecruiterId_ReturnsSlotsOrderedByStartTime()
        {
            var slots = new List<Slot>
            {
                new Slot { StartTime = DateTime.Today.AddHours(3), Status = SlotStatus.Free },
                new Slot { StartTime = DateTime.Today.AddHours(1), Status = SlotStatus.Free },
            };
            this.mockSlotRepository
                .Setup(slotRepository => slotRepository.GetAllSlotsAsync(1))
                .ReturnsAsync(slots);

            var result = await this.bookingService.GetAvailableSlotsByRecruiterId(1);

            Assert.Equal(DateTime.Today.AddHours(1), result[0].StartTime);
            Assert.Equal(DateTime.Today.AddHours(3), result[1].StartTime);
        }

        [Fact]
        public async Task ConfirmBooking_WhenSlotIsNull_ThrowsException()
        {
            var exception = await Record.ExceptionAsync(() => this.bookingService.ConfirmBooking(1, null));
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task ConfirmBooking_WhenSlotIsNotFree_ThrowsException()
        {
            var slot = new Slot { Status = SlotStatus.Occupied };
            var exceptionThrown = false;

            try
            {
                await this.bookingService.ConfirmBooking(1, slot);
            }
            catch (Exception)
            {
                exceptionThrown = true;
            }

            Assert.True(exceptionThrown, "Expected an Exception to be thrown.");
        }

        [Fact]
        public async Task ConfirmBooking_WhenSlotIsFree_UpdatesSlotAndCreatesSession()
        {
            var slot = new Slot
            {
                Id = 5,
                RecruiterId = 2,
                StartTime = DateTime.Today,
                Status = SlotStatus.Free,
            };

            var addedSessions = new List<InterviewSession>();
            this.mockInterviewSessionRepository
                .Setup(interviewSessionRepository => interviewSessionRepository.Add(It.IsAny<InterviewSession>()))
                .Callback<InterviewSession>(addedSessions.Add);

            await this.bookingService.ConfirmBooking(1, slot);

            Assert.Equal(SlotStatus.Occupied, slot.Status);
            Assert.Equal(1, slot.CandidateId);
            Assert.Equal(string.Empty, slot.InterviewType);
            this.mockSlotRepository.Verify(
                slotRepository => slotRepository.Update(slot),
                Times.Once);
            Assert.Single(addedSessions);
            Assert.Equal(1, addedSessions[0].ExternalUserId);
            Assert.Equal(2, addedSessions[0].InterviewerId);
            Assert.Equal("Scheduled", addedSessions[0].Status);
        }
    }
}