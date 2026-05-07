namespace Tests_and_Interviews_API.Services
{
    using System.Collections.Generic;
    using Tests_and_Interviews_API.Models;
    using Tests_and_Interviews_API.Repositories.Interfaces;
    using Tests_and_Interviews_API.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing job postings.
    /// </summary>
    public class JobsService : IJobsService
    {
        private readonly IJobsRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobsService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access job posting data. Cannot be null.</param>
        public JobsService(IJobsRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Retrieves all job postings.
        /// </summary>
        /// <returns>A list of all job postings.</returns>
        public IEnumerable<JobPosting> GetAllJobs()
        {
            return this._repository.GetAllJobs();
        }

        /// <summary>
        /// Retrieves all available skills.
        /// </summary>
        /// <returns>A read-only list of all skills.</returns>
        public IReadOnlyList<Skill> GetAllSkills()
        {
            return this._repository.GetAllSkills();
        }

        /// <summary>
        /// Adds a new job posting to the data store.
        /// </summary>
        /// <param name="jobPosting">The job posting to add. Cannot be null.</param>
        /// <param name="companyId">The unique identifier of the company adding the job.</param>
        /// <param name="skillLinks">The list of skill links associated with the job posting.</param>
        /// <returns>The unique identifier of the newly added job posting.</returns>
        public int AddJob(JobPosting jobPosting, int companyId, IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks)
        {
            return this._repository.AddJob(jobPosting, companyId, skillLinks);
        }

        /// <summary>
        /// Retrieves all skills linked to a specific job posting.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job posting.</param>
        /// <returns>A read-only list of skill links with required percentages.</returns>
        public IReadOnlyList<(int SkillId, int RequiredPercentage)> GetSkillsByJob(int jobId)
        {
            JobPosting? job = this._repository.GetAllJobs().FirstOrDefault(j => j.JobId == jobId);
            if (job == null || job.JobSkills == null)
            {
                return new List<(int, int)>();
            }
            return job.JobSkills
                .Select(js => (js.SkillId, js.RequiredPercentage))
                .ToList();
        }
    }
}