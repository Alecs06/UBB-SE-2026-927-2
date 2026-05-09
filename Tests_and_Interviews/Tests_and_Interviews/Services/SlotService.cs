// <copyright file="SlotService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Helpers;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models;
    using Tests_and_Interviews.Models.Enums;

    /// <summary>
    /// Provides operations for managing recruiter slots, including retrieval and creation of slots.
    /// </summary>
    public class SlotService : ISlotService
    {
        private readonly HttpClient http;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlotService"/> class.
        /// </summary>
        public SlotService()
        {
            this.http = ApiClient.Http;
        }

        public SlotService(HttpClient httpClient)
        {
            this.http = httpClient ?? ApiClient.Http;
        }

        /// <summary>
        /// Asynchronously retrieves a list of slots for the specified recruiter and date, including both
        /// existing free and occupied slots and free 30-minutes slots between 8:00 and 18:00.
        /// </summary>
        /// <param name="recruitedId">The unique identifier of the recruiter.</param>
        /// <param name="date">The date for which to load the slots.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of slot DTOs for the specified
        /// recruiter and date.</returns>
        public async Task<List<SlotDto>> LoadRecruiterVisibleSlotsAsync(int recruitedId, DateTime date)
        {
            HttpResponseMessage response = await this.http.GetAsync($"recruiter/{recruitedId}/date?date={date:O}");
            response.EnsureSuccessStatusCode();
            List<SlotDto>? dtos = await response.Content.ReadFromJsonAsync<List<SlotDto>>();
            List<Slot> existing = dtos?.Select(dto => dto.ToEntity()).ToList() ?? new List<Slot>();

            var visibleSlots = new List<Slot>();
            var currentTime = date.AddHours(8);
            var endOfDay = date.AddHours(18);
            while (currentTime < endOfDay)
            {
                var overlappingSlot = existing.FirstOrDefault(s =>
                    s.StartTime < currentTime.AddMinutes(30) && s.EndTime > currentTime);
                if (overlappingSlot != null)
                {
                    visibleSlots.Add(overlappingSlot);
                    currentTime = overlappingSlot.EndTime;
                }
                else
                {
                    visibleSlots.Add(new Slot
                    {
                        RecruiterId = recruitedId,
                        StartTime = currentTime,
                        EndTime = currentTime.AddMinutes(30),
                        Duration = 30,
                        Status = SlotStatus.Free,
                        InterviewType = string.Empty,
                    });
                    currentTime = currentTime.AddMinutes(30);
                }
            }
            return visibleSlots.Select(slot => slot.ToDto()).ToList();
        }

        /// <summary>
        /// Creates a new recruiter slot with the specified start time and duration.
        /// </summary>
        /// <param name="baseSlot">The base slot containing the start time.</param>
        /// <param name="duration">The duration of the slot in minutes.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CreateRecruiterSlotAsync(SlotDto baseSlot, int duration)
        {
            var newSlot = new Slot
            {
                RecruiterId = Env.RECRUITER_ID,
                StartTime = baseSlot.StartTime,
                EndTime = baseSlot.StartTime.AddMinutes(duration),
                Duration = duration,
                Status = SlotStatus.Free,
                InterviewType = "Available",
            };
            HttpResponseMessage response = await this.http.PostAsJsonAsync("slots", newSlot.ToDto());
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Asynchronously deletes a recruiter slot with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the recruiter slot to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public async Task DeleteRecruiterSlotAsync(int id)
        {
            HttpResponseMessage response = await this.http.DeleteAsync($"slots/{id}");
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Updates a recruiter's slot with new start time and duration asynchronously.
        /// </summary>
        /// <param name="initialSlot">The initial slot to update.</param>
        /// <param name="startTime">The new start time for the slot.</param>
        /// <param name="duration">The new duration of the slot in minutes.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        /// <exception cref="Exception">Thrown when the start time is outside the allowed hours of 8 to 18.</exception>
        public async Task UpdateRecruiterSlotAsync(SlotDto initialSlot, DateTime startTime, int duration)
        {
            if (startTime.Hour < 8 || startTime.Hour > 18)
            {
                throw new Exception("Slots should be between hours 8 and 18.");
            }
            var newSlot = new Slot
            {
                Id = initialSlot.Id,
                RecruiterId = initialSlot.RecruiterId,
                StartTime = startTime,
                EndTime = startTime.AddMinutes(duration),
            };
            HttpResponseMessage response = await this.http.PutAsJsonAsync(
                $"slots/{newSlot.Id}",
                newSlot.ToDto());
            response.EnsureSuccessStatusCode();
        }
    }
}