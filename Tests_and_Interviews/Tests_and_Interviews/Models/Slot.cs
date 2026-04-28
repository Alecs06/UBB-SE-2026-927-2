// <copyright file="Slot.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories;

    /// <summary>
    /// Represents a time slot for scheduling interviews. Includes details about timing, participants, status, and
    /// UI-related properties.
    /// </summary>
    /// <remarks>Implements INotifyPropertyChanged to support data binding in UI scenarios. Provides properties for
    /// formatted display, selection state, and color customization based on selection.</remarks>
    public class Slot : INotifyPropertyChanged
    {
        private bool isDaySelected;

        private bool isSlotSelected;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the recruiter.
        /// </summary>
        public int RecruiterId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the candidate.
        /// </summary>
        public int CandidateId { get; set; }

        /// <summary>
        /// Gets or sets the start time of the slot.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the slot.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the current status of the slot.
        /// </summary>
        public SlotStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the type of interview.
        /// </summary>
        public string InterviewType { get; set; } = string.Empty;

        /// <summary>
        /// Gets the start time formatted as a 24-hour string (HH:mm).
        /// </summary>
        public string FormattedTime => this.StartTime.ToString("HH:mm");

        /// <summary>
        /// Gets the time range in 24-hour format as a string, combining the start and end times.
        /// </summary>
        public string TimeRange => $"{this.StartTime:HH:mm} - {this.EndTime:HH:mm}";

        /// <summary>
        /// Gets the day of the month and abbreviated month name from the start time, formatted as "dd MMM".
        /// </summary>
        public string DayFormatted => this.StartTime.ToString("dd MMM");

        /// <summary>
        /// Gets the number of rows spanned based on the duration, each row equivalent to 30 minutes, with a minimum value of 1.
        /// </summary>
        public int RowSpan => this.Duration > 0 ? this.Duration / 30 : 1;

        /// <summary>
        /// Gets a value indicating whether the slot is currently occupied by a candidate.
        /// </summary>
        public bool IsOccupied => this.Status == SlotStatus.Occupied;

        /// <summary>
        /// Gets a value indicating whether the slot is free to occupy.
        /// </summary>
        public bool IsAvailable => this.Status == SlotStatus.Free;

        /// <summary>
        /// Gets or sets a value indicating whether the day is selected.
        /// </summary>
        public bool IsDaySelected
        {
            get => this.isDaySelected;
            set
            {
                this.isDaySelected = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.BackgroundColor));
                this.OnPropertyChanged(nameof(this.ForegroundColor));
            }
        }

        /// <summary>
        /// Gets the background color based on whether the day is selected.
        /// </summary>
        public string BackgroundColor => this.IsDaySelected ? "#6367FF" : "#C9BEFF";

        /// <summary>
        /// Gets the foreground color based on whether the day is selected.
        /// </summary>
        public string ForegroundColor => this.IsDaySelected ? "White" : "Black";

        /// <summary>
        /// Gets the slot color based on the selection state.
        /// </summary>
        public string SlotColor => this.IsSlotSelected ? "#8494FF" : "#FFDBFD";

        /// <summary>
        /// Gets or sets a value indicating whether the item is hidden.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the slot is selected.
        /// </summary>
        public bool IsSlotSelected
        {
            get => this.isSlotSelected;
            set
            {
                this.isSlotSelected = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.SlotColor));
            }
        }

        /// <summary>
        /// Locks the slot for the specified candidate and updates its status in the repository.
        /// </summary>
        /// <param name="candidateId">The identifier of the candidate to lock the slot for.</param>
        /// <exception cref="InvalidOperationException">Thrown when the slot is not available.</exception>
        public void Lock(int candidateId)
        {
            if (!this.IsAvailable)
            {
                throw new InvalidOperationException("Slot is not available");
            }

            this.Status = SlotStatus.Occupied;
            this.CandidateId = candidateId;

            var repo = new SlotRepository();
            repo.Update(this);
        }

        /// <summary>
        /// Releases the slot by setting its status to free and clearing the candidate identifier.
        /// </summary>
        public void Release()
        {
            this.Status = SlotStatus.Free;
            this.CandidateId = 0;
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <remarks>Intended for use in property setters to notify listeners of property value changes.</remarks>
        /// <param name="name">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}