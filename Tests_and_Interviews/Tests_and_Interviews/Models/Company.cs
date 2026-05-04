namespace Tests_and_Interviews.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("companies")]
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // no IDENTITY in SQL
        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("company_name", TypeName = "nvarchar(255)")]
        public string Name { get; set; } = string.Empty;

        [Column("about_us", TypeName = "nvarchar(max)")]
        public string? AboutUs { get; set; }

        [Column("profile_picture_url", TypeName = "nvarchar(max)")]
        public string? ProfilePicturePath { get; set; }

        [Column("logo_picture_url", TypeName = "nvarchar(max)")]
        public string CompanyLogoPath { get; set; } = string.Empty;

        [Column("location", TypeName = "nvarchar(300)")]
        public string? Location { get; set; }

        [Column("email", TypeName = "nvarchar(100)")]
        public string? Email { get; set; }

        [Column("posted_jobs_count")]
        public int PostedJobsCount { get; set; }

        [Column("collaborators_count")]
        public int CollaboratorsCount { get; set; }

        [NotMapped]
        private Game? _game;

        [NotMapped]
        public Game? Game
        {
            get => _game;
            set => _game = value;
        }

        [Column("buddy_name", TypeName = "nvarchar(255)")]
        public string? BuddyName { get; set; }

        [Column("avatar_id")]
        public int? AvatarId { get; set; }

        [Column("final_quote", TypeName = "nvarchar(max)")]
        public string? FinalQuote { get; set; }

        [Column("buddy_description", TypeName = "nvarchar(255)")]
        public string? BuddyDescription { get; set; }

        [Column("scen_1_text", TypeName = "nvarchar(max)")]
        public string? Scen1Text { get; set; }

        [Column("scen1_answer1", TypeName = "nvarchar(max)")]
        public string? Scen1Answer1 { get; set; }

        [Column("scen1_answer2", TypeName = "nvarchar(max)")]
        public string? Scen1Answer2 { get; set; }

        [Column("scen1_answer3", TypeName = "nvarchar(max)")]
        public string? Scen1Answer3 { get; set; }

        [Column("scen1_reaction1", TypeName = "nvarchar(max)")]
        public string? Scen1Reaction1 { get; set; }

        [Column("scen1_reaction2", TypeName = "nvarchar(max)")]
        public string? Scen1Reaction2 { get; set; }

        [Column("scen1_reaction3", TypeName = "nvarchar(max)")]
        public string? Scen1Reaction3 { get; set; }

        [Column("scen2_text", TypeName = "nvarchar(max)")]
        public string? Scen2Text { get; set; }

        [Column("scen2_answer1", TypeName = "nvarchar(max)")]
        public string? Scen2Answer1 { get; set; }

        [Column("scen2_answer2", TypeName = "nvarchar(max)")]
        public string? Scen2Answer2 { get; set; }

        [Column("scen2_answer3", TypeName = "nvarchar(max)")]
        public string? Scen2Answer3 { get; set; }

        [Column("scen2_reaction1", TypeName = "nvarchar(max)")]
        public string? Scen2Reaction1 { get; set; }

        [Column("scen2_reaction2", TypeName = "nvarchar(max)")]
        public string? Scen2Reaction2 { get; set; }

        [Column("scen2_reaction3", TypeName = "nvarchar(max)")]
        public string? Scen2Reaction3 { get; set; }

        // --- Navigation properties ---
        // These don't map to columns, EF uses them to understand relationships

        public ICollection<JobPosting> Jobs { get; set; } = new List<JobPosting>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Collaborator> Collaborators { get; set; } = new List<Collaborator>();

        public Company() { }

        public Company(
            string name,
            string aboutUs,
            string pfpUrl,
            string logoUrl,
            string location,
            string email,
            int companyId = 1,
            int postedJobsCount = 0,
            int collaboratorsCount = 0)
        {
            this.CompanyId = companyId;
            this.Name = name ?? string.Empty;
            this.AboutUs = aboutUs ?? string.Empty;
            this.ProfilePicturePath = pfpUrl ?? string.Empty;
            this.CompanyLogoPath = logoUrl ?? string.Empty;
            this.Location = location ?? string.Empty;
            this.Email = email ?? string.Empty;
            this.PostedJobsCount = postedJobsCount;
            this.CollaboratorsCount = collaboratorsCount;
        }

        public override string ToString()
        {
            return $"Company[{this.CompanyId}]: {this.Name}, {this.Email}";
        }
    }
}