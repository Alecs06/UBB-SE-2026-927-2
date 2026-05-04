namespace Tests_and_Interviews.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("jobs")]
    public class JobPosting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("job_id")]
        public int JobId { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        [Column("photo", TypeName = "nvarchar(max)")]
        public string Photo { get; set; }
        [Column("job_title", TypeName = "nvarchar(255)")]
        public string JobTitle { get; set; }
        [Column("industry_field", TypeName = "nvarchar(255)")]
        public string IndustryField { get; set; }// *dropdown menu w options: IT, Business, Healthcare, Education, etc.
        [Column("job_type", TypeName = "nvarchar(255)")]
        public string JobType { get; set; }// *Type: dropdown menu with options - can be multiple choice (part-time, full-time, volunteer, internship, remote, hybrid, etc)
        [Column("experience_level", TypeName = "nvarchar(255)")]    
        public string ExperienceLevel { get; set; }// *Experience level: dropdown menu with options (internship, entry level, mid-senior level, director, executive)
        [Column("start_date", TypeName = "date")]
        public DateTime? StartDate { get; set; }
        [Column("end_date", TypeName = "date")]
        public DateTime? EndDate { get; set; }

        [Column("job_description", TypeName = "nvarchar(max)")]
        public string JobDescription { get; set; }
        [Column("job_location", TypeName = "nvarchar(255)")]
        public string JobLocation { get; set; }

        [Column("available_positions")]
        public int AvailablePositions { get; set; }

        [Column("posted_at", TypeName = "datetime")]
        public DateTime? PostedAt { get; set; }// *Automatically getTime()

        [Column("salary")]
        public int? Salary { get; set; }// <0
        [Column("amount_payed")]
        public int? AmountPayed { get; set; }// <=0 (0 by default)
        [Column("deadline", TypeName = "date")]
        public DateTime? Deadline { get; set; }
        public System.Collections.Generic.ICollection<JobSkill> JobSkills { get; set; } = new System.Collections.Generic.List<JobSkill>();

        // *Required skills: checkboxes with different skills options (Python, Java, C++, etc.) and a corresponding percentage representing the minimum required knowledge for the job;
    }
}
