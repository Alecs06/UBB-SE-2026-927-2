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
    }
}