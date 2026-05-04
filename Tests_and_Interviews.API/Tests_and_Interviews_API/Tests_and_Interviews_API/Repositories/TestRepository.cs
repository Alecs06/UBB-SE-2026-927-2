using Tests_and_Interviews_API.Models.Core;
using Tests_and_Interviews_API.Repositories.Interfaces;

namespace Tests_and_Interviews_API.Repositories
{
    /// <summary>
    /// TODO: GENERATED MOCK REPO!!!! DOESNT DO SHIT!!! ONLY FOR TESTING!!! REPLACE!!!
    /// </summary>
    public class TestRepository : ITestRepository
    {
        private static readonly List<Test> _tests =
        [
            new Test { Id = 1, Title = "C# Fundamentals", Category = "Programming", CreatedAt = DateTime.UtcNow },
            new Test { Id = 2, Title = "SQL Basics", Category = "Database", CreatedAt = DateTime.UtcNow },
        ];

        private static int _nextId = 3;

        public Task<List<Test>> GetAllAsync()
            => Task.FromResult(_tests.ToList());

        public Task<Test?> FindByIdAsync(int id)
            => Task.FromResult(_tests.FirstOrDefault(t => t.Id == id));

        public Task<List<Test>> FindTestsByCategoryAsync(string category)
            => Task.FromResult(_tests.Where(t => t.Category == category).ToList());

        public Task<Test> CreateAsync(Test test)
        {
            test.Id = _nextId++;
            test.CreatedAt = DateTime.UtcNow;
            _tests.Add(test);
            return Task.FromResult(test);
        }

        public Task<Test?> UpdateAsync(Test test)
        {
            var existing = _tests.FirstOrDefault(t => t.Id == test.Id);
            if (existing is null) return Task.FromResult<Test?>(null);

            existing.Title = test.Title;
            existing.Category = test.Category;
            return Task.FromResult<Test?>(existing);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var test = _tests.FirstOrDefault(t => t.Id == id);
            if (test is null) return Task.FromResult(false);

            _tests.Remove(test);
            return Task.FromResult(true);
        }
    }
}