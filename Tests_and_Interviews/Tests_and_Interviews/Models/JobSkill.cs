namespace Tests_and_Interviews.Models
{
    using System;

    public class JobSkill
    {
        public Skill Skill { get; set; }
        public JobPosting Job { get; set; }
        public int RequiredPercentage { get; set; }
    }
}
