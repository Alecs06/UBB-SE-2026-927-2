// <copyright file="TestsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews_API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Tests_and_Interviews_API.Dtos;
    using Tests_and_Interviews_API.Mappers;
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ITestService _service;

        public TestsController(ITestService service)
        {
            this._service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestDto>> FindById(int id)
        {
            Test? test = await this._service.FindByIdAsync(id);

            if (test == null)
            {
                return NotFound();
            }

            return Ok(test.ToDto());
        }

        [HttpGet("bycategory/{category}")]
        public async Task<ActionResult<List<TestDto>>> FindByCategory(string category)
        {
            List<Test> tests = await this._service.FindTestsByCategoryAsync(category);

            return Ok(tests.Select(t => t.ToDto()).ToList());
        }

        /// <summary>
        /// Starts a test attempt for the specified user and test.
        /// </summary>
        [HttpPost("start")]
        public async Task<ActionResult> StartTest([FromBody] StartTestDto dto)
        {
            try
            {
                await this._service.StartTestAsync(dto.UserId, dto.TestId);
                return this.Ok();
            }
            catch (InvalidOperationException e)
            {
                return this.Conflict(e.Message);
            }
        }

        /// <summary>
        /// Submits a test attempt with answers and returns the final score.
        /// </summary>
        [HttpPost("submit-attempt")]
        public async Task<ActionResult<float>> SubmitAttempt([FromBody] SubmitAttemptDto dto)
        {
            float score = await this._service.SubmitAttemptAsync(
                dto.UserId, dto.TestId, dto.Answers);
            return this.Ok(score);
        }
    }
}