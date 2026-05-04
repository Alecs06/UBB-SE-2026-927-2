namespace Tests_and_Interviews.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("job_skills")]
    public class JobSkill
    {
        [Column("skill_id")]
        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;  

        [Column("job_id")]
        public int JobId { get; set; }
        public JobPosting Job { get; set; } = null!;

        [Column("required_percentage")]
        public int RequiredPercentage { get; set; }
    }
}
