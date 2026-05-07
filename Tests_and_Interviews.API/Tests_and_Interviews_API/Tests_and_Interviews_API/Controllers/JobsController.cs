namespace Tests_and_Interviews_API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using Tests_and_Interviews_API.Dtos;
    using Tests_and_Interviews_API.Mappers;
    using Tests_and_Interviews_API.Models;
    using Tests_and_Interviews_API.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobsService _service;

        public JobsController(IJobsService service)
        {
            this._service = service;
        }

        [HttpGet]
        public ActionResult<List<JobPostingDto>> GetAllJobs()
        {
            IEnumerable<JobPosting> jobs = this._service.GetAllJobs();

            return Ok(jobs.Select(j => j.ToDto()).ToList());
        }

        [HttpGet("skills")]
        public ActionResult<List<SkillDto>> GetAllSkills()
        {
            IReadOnlyList<Skill> skills = this._service.GetAllSkills();

            return Ok(skills.Select(s => s.ToDto()).ToList());
        }

        [HttpPost]
        public ActionResult<int> AddJob([FromBody] AddJobDto dto)
        {
            JobPosting jobPosting = dto.JobPosting.ToEntity();
            IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks = dto.SkillLinks
                .Select(s => (s.SkillId, s.RequiredPercentage))
                .ToList();

            int jobId = this._service.AddJob(jobPosting, dto.CompanyId, skillLinks);

            return Ok(jobId);
        }
    }
}