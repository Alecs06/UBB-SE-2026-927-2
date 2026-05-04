namespace TestsAndInterviews.API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Tests_and_Interviews_API.DTOs;
    using Tests_and_Interviews_API.Services.Interfaces;

    [ApiController]
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestsController(ITestService testService)
        {
            _testService = testService;
        }

        /// <summary>
        /// GET api/tests
        /// Returns all tests (summary only, no questions).
        /// Optional filter: api/tests?category=Programming
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TestSummaryDto>>> GetAll([FromQuery] string? category)
        {
            if (!string.IsNullOrWhiteSpace(category))
            {
                var byCategory = await _testService.GetTestsByCategoryAsync(category);
                return Ok(byCategory);
            }

            var tests = await _testService.GetAllTestsAsync();
            return Ok(tests);
        }

        /// <summary>
        /// GET api/tests/id
        /// Returns a single test with its full list of questions.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TestDetailDto>> GetById(int id)
        {
            var test = await _testService.GetTestByIdAsync(id);
            if (test is null)
                return NotFound($"Test with id {id} was not found.");

            return Ok(test);
        }

        /// <summary>
        /// POST api/tests
        /// Creates a new test. Questions can be included in the body.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TestDetailDto>> Create([FromBody] CreateTestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _testService.CreateTestAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// PUT api/tests/id
        /// Updates the title and category of an existing test.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TestDetailDto>> Update(int id, [FromBody] UpdateTestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _testService.UpdateTestAsync(id, dto);
            if (updated is null)
                return NotFound($"Test with id {id} was not found.");

            return Ok(updated);
        }

        /// <summary>
        /// DELETE api/tests/id
        /// Deletes a test and all its associated questions.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _testService.DeleteTestAsync(id);
            if (!deleted)
                return NotFound($"Test with id {id} was not found.");

            return NoContent();
        }
    }
}
