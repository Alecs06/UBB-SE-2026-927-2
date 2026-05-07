namespace Tests_and_Interviews_API.Services.Interfaces
{
    using System.Collections.Generic;
    using Tests_and_Interviews_API.Models;

    /// <summary>
    /// Defines operations for managing job postings.
    /// </summary>
    public interface IJobsService
    {
        /// <summary>
        /// Retrieves all job postings.
        /// </summary>
        /// <returns>A list of all job postings.</returns>
        IEnumerable<JobPosting> GetAllJobs();

        /// <summary>
        /// Retrieves all available skills.
        /// </summary>
        /// <returns>A read-only list of all skills.</returns>
        IReadOnlyList<Skill> GetAllSkills();

        /// <summary>
        /// Adds a new job posting to the data store.
        /// </summary>
        /// <param name="jobPosting">The job posting to add. Cannot be null.</param>
        /// <param name="companyId">The unique identifier of the company adding the job.</param>
        /// <param name="skillLinks">The list of skill links associated with the job posting.</param>
        /// <returns>The unique identifier of the newly added job posting.</returns>
        int AddJob(JobPosting jobPosting, int companyId, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks);

        /// <summary>
        /// Retrieves all skills linked to a specific job posting.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting.</param>
        /// <returns>A read-only list of skill links with required percentages.</returns>
        IReadOnlyList<(int SkillId, int RequiredPercentage)> GetSkillsByJob(int jobId);
    }
}