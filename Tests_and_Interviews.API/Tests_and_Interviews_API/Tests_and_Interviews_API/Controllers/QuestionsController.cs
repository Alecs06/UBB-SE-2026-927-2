namespace Tests_and_Interviews_API.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Tests_and_Interviews_API.DTOs;
    using Tests_and_Interviews_API.Mappers;
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _service;

        public QuestionsController(IQuestionService service)
        {
            this._service = service;
        }

        [HttpGet("test/{testId}")]
        public async Task<ActionResult<List<QuestionDto>>> GetByTest(int testId)
        {
            List<Question> questions = await this._service.GetQuestionsByTest(testId);

            return Ok(questions.Select(question => question.ToDto()));
        }

        [HttpGet("position/{positionId}")]
        public async Task<ActionResult<List<QuestionDto>>> GetByPositionId(int positionId)
        {
            List<Question> questions = await this._service.GetInterviewQuestionsByPosition(positionId);

            return Ok(questions.Select(question => question.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetById(int id)
        {
            try
            {
                Question question = await this._service.GetQuestionByIdAsync(id);

                return Ok(question.ToDto());
            } catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost()]
        public async Task<ActionResult<QuestionDto>> Create([FromBody] QuestionDto dto)
        {
            Question created = await this._service.AddQuestionAsync(dto.ToEntity());

            return Ok(created.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<QuestionDto>> Update(int id, [FromBody] QuestionDto dto)
        {
            try
            {
                Question updated = await this._service.UpdateQuestionAsync(id, dto.ToEntity());

                return Ok(updated.ToDto());
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                bool deleted = await this._service.DeleteQuestionAsync(id);

                if (deleted)
                {
                    return Ok( new { message = "Question deleted successfully" });
                }

                return BadRequest();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
