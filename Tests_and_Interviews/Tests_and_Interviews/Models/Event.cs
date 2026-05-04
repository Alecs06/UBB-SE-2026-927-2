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

    [Table("events")]
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("event_id")]
        public int Id { get; set; }

        [Column("photo", TypeName = "nvarchar(max)")]
        public string Photo { get; set; }

        [Column("title", TypeName = "nvarchar(200)")]
        public string Title { get; set; }

        [Column("description", TypeName = "nvarchar(max)")]

        public string Description { get; set; }

        [Column("start_date", TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column("end_date", TypeName = "date")]
        public DateTime EndDate { get; set; }

        [Column("location", TypeName = "nvarchar(300)")]
        public string Location { get; set; }

        [Column("host_company_id")]
        public int HostCompanyId { get; set; }
        public Company HostCompany { get; set; } = null!;

        [Column("posted_at", TypeName = "datetime")]
        public DateTime PostedAt { get; set;  }
        public ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();


        /// <summary>
        /// Event constructor
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

        public Event()
        {
        }

        public override string ToString()
        {
            return "Event: " + Photo + " " + Title + " " + Description + " " +
                StartDate.ToString() + " " + EndDate.ToString() + " " + Location + " " + HostCompanyId.ToString() +
                " " + Collaborators.ToString() + "\n";
        }
    }
}
