namespace Tests_and_Interviews_API.Dtos
{
    /// <summary>
    /// Represents the association between a job posting and a required skill.
    /// </summary>
    public class JobSkillDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the skill.
        /// </summary>
        public int SkillId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the job posting.
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Gets or sets the required proficiency percentage for the skill.
        /// </summary>
        public int RequiredPercentage { get; set; }
    }
}