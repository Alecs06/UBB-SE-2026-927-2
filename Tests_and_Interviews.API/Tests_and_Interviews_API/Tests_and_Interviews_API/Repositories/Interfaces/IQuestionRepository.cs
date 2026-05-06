namespace Tests_and_Interviews_API.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Tests_and_Interviews_API.Models.Core;

    /// <summary>
    /// IQuestionRepository interface provides methods to perform CRUD operations on the Questions.
    /// </summary>
    public interface IQuestionRepository
    {
        /// <summary>
        /// Asynchronously retrieves a list of questions along with their associated answers for a given test ID.
        /// </summary>
        /// <param name="testId">The id of the test one want to find.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<Question>> FindByTestIdAsync(int testId);

        /// <summary>
        /// Asynchronously retrieves a list of interview questions for a specific position ID.
        /// This method executes a SQL query to fetch questions that are categorized as "INTERVIEW" type and are associated with the given position ID. The results are mapped to a list of Question objects, which are then returned to the caller.
        /// </summary>
        /// <param name="positionId">The ID of the position for which to retrieve interview questions.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation, containing a list of <see cref="Question"/> objects.</returns>
        Task<List<Question>> GetInterviewQuestionsByPositionAsync(int positionId);

        /// <summary>
        /// Asynchronously retrieves a question by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the question to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the question with the specified
        /// identifier, or <see langword="null"/> if no matching question is found.</returns>
        Task<Question?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously adds a new question to the data store.
        /// </summary>
        /// <param name="question">The question to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        Task AddASync(Question question); 

        /// <summary>
        /// Asynchronously updates the specified question in the data store.
        /// </summary>
        /// <param name="question">The question entity to update. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task UpdateAsync(Question question); 

        /// <summary>
        /// Asynchronously deletes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task DeleteAsync(int id); 
    }
}
