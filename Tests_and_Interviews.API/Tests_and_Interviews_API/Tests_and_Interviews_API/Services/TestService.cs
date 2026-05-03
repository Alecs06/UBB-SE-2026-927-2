namespace Tests_and_Interviews_API.Services
{
    using Tests_and_Interviews_API.DTOs;
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Repositories.Interfaces;
    using Tests_and_Interviews_API.Services.Interfaces;

    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;

        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public async Task<List<TestSummaryDto>> GetAllTestsAsync()
        {
            var tests = await _testRepository.GetAllAsync();
            return tests.Select(MapToSummary).ToList();
        }

        public async Task<TestDetailDto?> GetTestByIdAsync(int id)
        {
            var test = await _testRepository.FindByIdAsync(id);
            return test is null ? null : MapToDetail(test);
        }

        public async Task<List<TestSummaryDto>> GetTestsByCategoryAsync(string category)
        {
            var tests = await _testRepository.FindTestsByCategoryAsync(category);
            return tests.Select(MapToSummary).ToList();
        }

        public async Task<TestDetailDto> CreateTestAsync(CreateTestDto dto)
        {
            var test = new Test
            {
                Title = dto.Title,
                Category = dto.Category,
                CreatedAt = DateTime.UtcNow,
                Questions = dto.Questions.Select(q => new Question
                {
                    QuestionText = q.QuestionText,
                    QuestionTypeString = q.QuestionType,
                    QuestionScore = q.QuestionScore,
                    QuestionAnswer = q.QuestionAnswer,
                    OptionsJson = q.OptionsJson,
                }).ToList()
            };

            var created = await _testRepository.CreateAsync(test);
            return MapToDetail(created);
        }

        public async Task<TestDetailDto?> UpdateTestAsync(int id, UpdateTestDto dto)
        {
            var test = new Test
            {
                Id = id,
                Title = dto.Title,
                Category = dto.Category,
            };

            var updated = await _testRepository.UpdateAsync(test);
            return updated is null ? null : MapToDetail(updated);
        }

        public async Task<bool> DeleteTestAsync(int id)
        {
            return await _testRepository.DeleteAsync(id);
        }

        // Private mapping helpers

        private static TestSummaryDto MapToSummary(Test t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Category = t.Category,
            CreatedAt = t.CreatedAt,
            QuestionCount = t.Questions.Count,
        };

        private static TestDetailDto MapToDetail(Test t) => new()
        {
            Id = t.Id,
            Title = t.Title,
            Category = t.Category,
            CreatedAt = t.CreatedAt,
            Questions = t.Questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                QuestionType = q.QuestionTypeString,
                QuestionScore = q.QuestionScore,
                QuestionAnswer = q.QuestionAnswer,
                OptionsJson = q.OptionsJson,
            }).ToList()
        };
    }
}
