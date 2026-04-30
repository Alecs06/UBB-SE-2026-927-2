using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Tests_and_Interviews.Models;

namespace Tests_and_Interviews.Services.Interfaces
{
    public interface IEventsService
    {
        Event AddEvent(string eventPhoto, string eventTitle, string eventDescription, DateTime eventStartDate, DateTime eventEndDate, string eventLocation, int hostId, List<Company> collaborators);
        void DeleteEvent(Event eventToBeRemoved);
        ObservableCollection<Event> GetCurrentEvents(int loggedInUserID);
        ObservableCollection<Event> GetPastEvents(int loggedInUserID);
        void UpdateEvent(int eventIdToBeUpdated, string newEventPhoto, string newEventTitle, string newEventDescription, DateTime newEventStartDate, DateTime newEventEndDate, string newEventLocation);
    }
}