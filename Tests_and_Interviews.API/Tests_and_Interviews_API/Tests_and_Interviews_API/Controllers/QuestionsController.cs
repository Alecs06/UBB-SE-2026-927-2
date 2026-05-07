namespace Tests_and_Interviews_API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Tests_and_Interviews_API.Dtos;
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

        [HttpGet("bytest/{testId}")]
        public async Task<ActionResult<List<QuestionDto>>> GetByTestId(int testId)
        {
            List<Question> questions = await this._service.GetQuestionsByTestIdAsync(testId);

            return Ok(questions.Select(q => q.ToDto()).ToList());
        }

        [HttpGet("byposition/{positionId}")]
        public async Task<ActionResult<List<QuestionDto>>> GetByPosition(int positionId)
        {
            List<Question> questions = await this._service.GetInterviewQuestionsByPositionAsync(positionId);

            return Ok(questions.Select(q => q.ToDto()).ToList());
        }
    }
}