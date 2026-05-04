namespace Tests_and_Interviews.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("collaborators")]
    public class Collaborator
    {
        [Column("event_id")]
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        [Column("company_id")]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
    }
}