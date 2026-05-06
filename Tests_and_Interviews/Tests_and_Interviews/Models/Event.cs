namespace Tests_and_Interviews.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Event class represents an event organized by a company, containing properties such as photo, title, description, start and end dates, location, host company information, and a list of collaborators.
    /// It is mapped to the "events" table in the database, with a primary key of Id that is not auto-generated. The Event class provides constructors for initializing event instances and an overridden ToString method for easy representation of event details.
    /// </summary>
    [Table("events")]
    public class Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="eventPhoto"> event photo generated path </param>
        /// <param name="eventTitle"> event title </param>
        /// <param name="eventDescription"> event description </param>
        /// <param name="eventStartDate"> event starting date </param>
        /// <param name="eventEndDate"> event ending date </param>
        /// <param name="eventLocation"> event location </param>
        /// <param name="eventHostID"> id of the company who created the event</param>
        public Event(string eventPhoto, string eventTitle, string eventDescription, DateTime eventStartDate, DateTime eventEndDate, string eventLocation, int eventHostID)
        {
            this.Photo = eventPhoto;
            this.Title = eventTitle;
            this.Description = eventDescription;
            this.StartDate = eventStartDate;
            this.EndDate = eventEndDate;
            this.Location = eventLocation;
            this.HostCompanyId = eventHostID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class with default values for all properties.
        /// This constructor allows for creating an empty event instance that can be populated with specific details later on.
        /// </summary>
        public Event()
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier for the event. This property is marked as the primary key and is mapped to the "event_id" column in the database.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("event_id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the photo associated with the event. This property is mapped to the "photo" column in the database and is of type nvarchar(max) to allow for storing a URL or base64-encoded string representing the photo.
        /// </summary>
        [Column("photo", TypeName = "nvarchar(max)")]
        public string? Photo { get; set; }

        /// <summary>
        /// Gets or sets the title of the event. This property is mapped to the "title" column in the database and has a maximum length of 200 characters.
        /// </summary>
        [Column("title", TypeName = "nvarchar(200)")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the event. This property is mapped to the "description" column in the database and is of type nvarchar(max) to allow for storing a detailed description of the event.
        /// </summary>
        [Column("description", TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the start date of the event. This property is mapped to the "start_date" column in the database and is of type date, representing the date on which the event begins.
        /// </summary>
        [Column("start_date", TypeName = "date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the event. This property is mapped to the "end_date" column in the database and is of type date, representing the date on which the event ends.
        /// </summary>
        [Column("end_date", TypeName = "date")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the location of the event. This property is mapped to the "location" column in the database and has a maximum length of 300 characters.
        /// </summary>
        [Column("location", TypeName = "nvarchar(300)")]
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the host company that created the event. This property is mapped to the "host_company_id" column in the database and represents a foreign key relationship to the Company entity.
        /// </summary>
        [Column("host_company_id")]
        public int HostCompanyId { get; set; }

        /// <summary>
        /// Gets or sets the host company that created the event. This property represents a navigation property to the Company entity, allowing for access to the company's details and related information.
        /// </summary>
        public Company? HostCompany { get; set; } = null;

        /// <summary>
        /// Gets or sets the date and time when the event was posted. This property is mapped to the "posted_at" column in the database and is of type datetime, representing the timestamp of when the event was created or made public.
        /// </summary>
        [Column("posted_at", TypeName = "datetime")]
        public DateTime PostedAt { get; set;  }

        /// <summary>
        /// Gets or sets the collection of collaborators associated with the event. This property represents a navigation property to a collection of Collaborator entities, allowing for access to the details of each collaborator such as their name, role, and contact information.
        /// </summary>
        public ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();

        /// <summary>
        /// Returns a string representation of the event, including its photo, title, description, start and end dates, location, host company ID, and collaborators. This method is overridden to provide a meaningful representation of the event's details when the ToString method is called.
        /// </summary>
        /// <returns>A string representation of the event.</returns>
        public override string ToString()
        {
            return "Event: " + this.Photo + " " + this.Title + " " + this.Description + " " +
                this.StartDate.ToString() + " " + this.EndDate.ToString() + " " + this.Location + " " + this.HostCompanyId.ToString() +
                " " + this.Collaborators.ToString() + "\n";
        }
    }
}
