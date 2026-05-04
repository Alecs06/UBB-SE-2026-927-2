namespace Tests_and_Interviews.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Tests_and_Interviews.Models.Core;

    [Table("applicants")]
    public class Applicant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("applicant_id")]
        public int ApplicantId { get; set; }

        [Column("job_id")]
        public int JobId { get; set; }
        public JobPosting Job { get; set; } = null!;

        [Column("user_id")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Grades start as null until evaluated

        [Column("app_test_grade", TypeName = "decimal(5,2)")]
        public decimal? AppTestGrade { get; set; }
        [Column("cv_grade", TypeName = "decimal(5,2)")]
        public decimal? CvGrade { get; set; }
        [Column("company_test_grade", TypeName = "decimal(5,2)")]
        public decimal? CompanyTestGrade { get; set; }
        [Column("interview_grade", TypeName = "decimal(5,2)")]
        public decimal? InterviewGrade { get; set; }

        // "Failed", "On Hold", "Accepted", "Recommended"
        [Column("application_status", TypeName = "nvarchar(50)")]
        public string? ApplicationStatus { get; set; } = null;

        [Column("applied_at", TypeName = "datetime")]
        public DateTime AppliedAt { get; set; }

        [Column("recommended_from_company_id")]
        public int? RecommendedFromCompanyId { get; set; }
        public Company? RecommendedFromCompany { get; set; }

        [Column("cv_file_url", TypeName = "nvarchar(500)")]
        public string? CvFileUrl { get; set; }
    }
}
