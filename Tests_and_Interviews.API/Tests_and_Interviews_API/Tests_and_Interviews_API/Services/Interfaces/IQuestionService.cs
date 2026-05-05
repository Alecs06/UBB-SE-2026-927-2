namespace Tests_and_Interviews_API.Services.Interfaces
{
    using Tests_and_Interviews_API.Models.Core;

    /// <summary>
    /// Service layer for Question CRUD operations.
    /// Sits between the controller and the repository.
    /// </summary>
    public interface IQuestionService
    {
        /// <summary>
        /// Retrieves a list of questions associated with the specified test.
        /// </summary>
        /// <param name="testId">The unique identifier of the test for which to retrieve questions.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of questions linked to
        /// the specified test. The list is empty if no questions are found.</returns>
        public Task<List<Question>> GetQuestionsByTest(int testId);

        /// <summary>
        /// Retrieves a list of interview questions associated with the specified position.
        /// </summary>
        /// <param name="positionId">The unique identifier of the position for which to retrieve interview questions. Must be a valid position
        /// ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of questions for the
        /// specified position. The list is empty if no questions are found.</returns>
        public Task<List<Question>> GetInterviewQuestionsByPosition(int positionId);

        /// <summary>
        /// Asynchronously retrieves a question by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the question to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the question with the specified
        /// identifier.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a question with the specified identifier does not exist.</exception>
        public Task<Question> GetQuestionByIdAsync(int id);

        /// <summary>
        /// Asynchronously adds a new question to the data store.
        /// </summary>
        /// <param name="question">The question to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added question.</returns>
        public Task<Question> AddQuestionAsync(Question question);

        /// <summary>
        /// Updates an existing question with the specified identifier using the provided question data.
        /// </summary>
        /// <param name="id">The unique identifier of the question to update.</param>
        /// <param name="question">The updated question data to apply. The <see cref="Question.Id"/> property is ignored and will be set to the
        /// specified <paramref name="id"/>.</param>
        /// <returns>A <see cref="Question"/> object representing the updated question.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a question with the specified <paramref name="id"/> does not exist.</exception>
        public Task<Question> UpdateQuestionAsync(int id, Question question);

        /// <summary>
        /// Asynchronously deletes the question with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the question to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the question
        /// was successfully deleted.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a question with the specified <paramref name="id"/> does not exist.</exception
        public Task<bool> DeleteQuestionAsync(int id);
    }
}
