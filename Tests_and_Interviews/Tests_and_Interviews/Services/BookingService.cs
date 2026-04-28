// <copyright file="BookingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services.Interfaces;

    /// <summary>
    /// Provides booking-related operations for managing interview slot reservations and session creation.
    /// </summary>
    public class BookingService : IBookingService
    {
        private const int MINIMUMPOSITIONID = 0;
        private const int MINIMUMINTERVIEWSCORE = 0;

        private readonly ISlotRepository slotRepo;
        private readonly IInterviewSessionRepository interviewRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class with the specified repositories.
        /// </summary>
        /// <param name="slotRepository">The slot repository to be used by the service.</param>
        /// <param name="interviewSessionRepository">The interview session repository to be used by the service.</param>
        public BookingService(ISlotRepository slotRepository, IInterviewSessionRepository interviewSessionRepository)
        {
            this.slotRepo = slotRepository;
            this.interviewRepo = interviewSessionRepository;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class with default repositories.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public BookingService()
            : this(new SlotRepository(), new InterviewSessionRepository())
        {
        }

        /// <summary>
        /// Gets the available slots for a given recruiter at a given date.
        /// </summary>
        /// <param name="recruiterId"> Id of the recruiter.</param>
        /// <param name="date"> The date for which to retrieve available slots.</param>
        /// <returns> A list of available slots for the specified recruiter and date.</returns>
        public List<Slot> GetAvailableSlots(int recruiterId, DateTime date)
        {
            return this.slotRepo
                .GetSlots(recruiterId, date)
                .Where(slot => slot.Status == SlotStatus.Free)
                .OrderBy(slot => slot.StartTime)
                .ToList();
        }

        /// <summary>
        /// Gets all available slots for a given recruiter, regardless of the date.
        /// </summary>
        /// <param name="recruiterId"> Id of the recruiter.</param>
        /// <returns> A list of all available slots for the specified recruiter.</returns>
        public List<Slot> GetAvailableSlotsByRecruiterId(int recruiterId)
        {
            return this.slotRepo
                .GetAllSlots(recruiterId)
                .Where(slot => slot.Status == SlotStatus.Free)
                .OrderBy(slot => slot.StartTime)
                .ToList();
        }

        /// <summary>
        /// Confirms a booking for a candidate by updating the slot's status to occupied and creating a new interview session.
        /// </summary>
        /// <param name="candidateId"> Id of the candidate.</param>
        /// <param name="slot"> The slot to be booked.</param>
        /// <exception cref="Exception"> Thrown when the slot is not found or is no longer available.</exception>
        public void ConfirmBooking(int candidateId, Slot slot)
        {
            if (slot == null)
            {
                throw new Exception("Slot not found");
            }

            if (slot.Status != SlotStatus.Free)
            {
                throw new Exception("This slot is no longer available");
            }

            slot.Status = SlotStatus.Occupied;
            slot.CandidateId = candidateId;
            slot.InterviewType = string.Empty;

            this.slotRepo.Update(slot);

            InterviewSession newInterviewSession = new InterviewSession
            {
                SessionId = slot.Id,
                PositionId = MINIMUMPOSITIONID,
                ExternalUserId = candidateId,
                InterviewerId = slot.RecruiterId,
                DateStart = slot.StartTime.ToUniversalTime(),
                Video = string.Empty,
                Status = "Scheduled",
                Score = MINIMUMINTERVIEWSCORE,
            };

            this.interviewRepo.Add(newInterviewSession);
        }
    }
}