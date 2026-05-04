namespace Tests_and_Interviews.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("skills")]
    public class Skill
    {
        [Key]
        [Column("skill_id")]
        public int SkillId { get; set; }

        [Column("skill_name", TypeName = "nvarchar(255)")]
        public string SkillName { get; set; } = string.Empty;

        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}
