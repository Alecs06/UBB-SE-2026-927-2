namespace Tests_and_Interviews_API.Repositories
{
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Repositories.Interfaces;

    /// <summary>
    /// MOCK REPOSITORY FOR TESTING CONTROLLER TODO
    /// </summary>
    public class QuestionRepository: IQuestionRepository
    {
        private readonly List<Question> _questions = new();

        public QuestionRepository()
        {
            // Seed with some sample questions for testing
            _questions.Add(new Question
            {
                Id = 1,
                TestId = 10,
                PositionId = 5,
                QuestionText = "What is polymorphism?",
                QuestionTypeString = "INTERVIEW"
            });

            _questions.Add(new Question
            {
                Id = 2,
                TestId = 10,
                PositionId = 5,
                QuestionText = "Explain dependency injection.",
                QuestionTypeString = "INTERVIEW"
            });

            _questions.Add(new Question
            {
                Id = 3,
                TestId = 11,
                PositionId = 7,
                QuestionText = "What is encapsulation?",
                QuestionTypeString = "TEST"
            });
        }

    public Task<List<Question>> FindByTestIdAsync(int testId)
        {
            var result = _questions
                .Where(q => q.TestId == testId)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<List<Question>> GetInterviewQuestionsByPositionAsync(int positionId)
        {
            var result = _questions
                .Where(q => q.PositionId == positionId && q.QuestionTypeString == "INTERVIEW")
                .ToList();

            return Task.FromResult(result);
        }

        public Task<Question?> GetByIdAsync(int id)
        {
            var question = _questions.FirstOrDefault(q => q.Id == id);
            return Task.FromResult(question);
        }

        public Task AddASync(Question question)
        {
            question.Id = _questions.Count == 0 ? 1 : _questions.Max(q => q.Id) + 1;
            _questions.Add(question);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(Question question)
        {
            var existing = _questions.FirstOrDefault(q => q.Id == question.Id);
            if (existing == null)
                return Task.CompletedTask; // no exception, service will handle null

            // Replace the object
            _questions.Remove(existing);
            _questions.Add(question);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var existing = _questions.FirstOrDefault(q => q.Id == id);
            if (existing != null)
                _questions.Remove(existing);

            return Task.CompletedTask;
        }
    }
}