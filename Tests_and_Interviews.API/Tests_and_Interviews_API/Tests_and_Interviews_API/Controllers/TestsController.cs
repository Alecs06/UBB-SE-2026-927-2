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

        [HttpGet]
        public async Task<ActionResult<List<TestDto>>> GetAll()
        {
            List<Test> tests = await this._service.GetAll();

            return Ok(tests.Select(test => test.ToDto()).ToList());
        }

        [HttpGet("categories")]
        public async Task<ActionResult<string>> GetCategories()
        {
            return Ok(await this._service.GetCategories());
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

        [HttpPost]
        public async Task<ActionResult<TestDto>> Create([FromBody] TestDto dto)
        {
            try {
                Test created = await this._service.AddTestASync(dto.ToEntity());

                return Ok(created.ToDto());
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TestDto>> Update (int id, [FromBody] TestDto dto)
        {
            try {
                Test updated = await this._service.UpdateTestAsync(id, dto.ToEntity());

                return Ok(updated.ToDto());
            } catch (KeyNotFoundException ke) {
                return NotFound(ke.Message);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try {
                bool deleted = await this._service.DeleteTestAsync(id);

                if (deleted) {
                    return Ok(new { message = "Test was deleted successfully" });
                }

                return BadRequest();
            } catch (KeyNotFoundException e) {
                return NotFound(e.Message);
            }
        }
    }
}