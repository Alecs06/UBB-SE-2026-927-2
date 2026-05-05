namespace Tests_and_Interviews_API.Services
{
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Repositories.Interfaces;
    using Tests_and_Interviews_API.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing questions, including retrieval, creation, update, and deletion of question
    /// entities.
    /// </summary>
    /// <remarks>This service acts as an abstraction over the question repository, enabling consumers to
    /// interact with question data without needing to access the underlying data store directly.</remarks>
    public class QuestionService: IQuestionService
    {
        private readonly IQuestionRepository _repository;

        /// <summary>
        /// Initializes a new instance of the QuestionService class using the specified question repository.
        /// </summary>
        /// <param name="repository">The repository used to access and manage question data. Cannot be null.</param>
        public QuestionService(IQuestionRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Retrieves a list of questions associated with the specified test.
        /// </summary>
        /// <param name="testId">The unique identifier of the test for which to retrieve questions.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of questions linked to
        /// the specified test. The list is empty if no questions are found.</returns>
        public async Task<List<Question>> GetQuestionsByTest(int testId)
        {
            return await this._repository.FindByTestIdAsync(testId);
        }

        /// <summary>
        /// Retrieves a list of interview questions associated with the specified position.
        /// </summary>
        /// <param name="positionId">The unique identifier of the position for which to retrieve interview questions. Must be a valid position
        /// ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of questions for the
        /// specified position. The list is empty if no questions are found.</returns>
        public async Task<List<Question>> GetInterviewQuestionsByPosition(int positionId)
        {
            return await this._repository.GetInterviewQuestionsByPositionAsync(positionId);
        }

        /// <summary>
        /// Asynchronously retrieves a question by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the question to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the question with the specified
        /// identifier.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a question with the specified identifier does not exist.</exception>
        public async Task<Question> GetQuestionByIdAsync(int id)
        {
            Question? question = await this._repository.GetByIdAsync(id);

            if (question == null)
            {
                throw new KeyNotFoundException("Question not found.");
            }

            return question;
        }

        /// <summary>
        /// Asynchronously adds a new question to the data store.
        /// </summary>
        /// <param name="question">The question to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added question.</returns>
        public async Task<Question> AddQuestionAsync(Question question)
        {
            await this._repository.AddASync(question);

            return question;
        }

        /// <summary>
        /// Updates an existing question with the specified identifier using the provided question data.
        /// </summary>
        /// <param name="id">The unique identifier of the question to update.</param>
        /// <param name="question">The updated question data to apply. The <see cref="Question.Id"/> property is ignored and will be set to the
        /// specified <paramref name="id"/>.</param>
        /// <returns>A <see cref="Question"/> object representing the updated question.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a question with the specified <paramref name="id"/> does not exist.</exception>
        public async Task<Question> UpdateQuestionAsync(int id, Question question)
        {
            Question? initialQuestion = await this._repository.GetByIdAsync(id);

            if (initialQuestion == null)
            {
                throw new KeyNotFoundException("Question to update not found.");
            }

            question.Id = id;
            await this._repository.UpdateAsync(question);

            return question;
        }

        /// <summary>
        /// Asynchronously deletes the question with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the question to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the question
        /// was successfully deleted.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a question with the specified <paramref name="id"/> does not exist.</exception>
        public async Task<bool> DeleteQuestionAsync(int id)
        {
            Question? question = await this._repository.GetByIdAsync(id);

            if (question == null)
            {
                throw new KeyNotFoundException("Question to delete not found.");
            }

            await this._repository.DeleteAsync(id);

            return true;
        }
    }
}
