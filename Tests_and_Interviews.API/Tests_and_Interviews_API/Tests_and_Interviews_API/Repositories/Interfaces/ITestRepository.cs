namespace Tests_and_Interviews_API.Repositories.Interfaces
{
    using Tests_and_Interviews_API.Models.Core;
    /// <summary>
    /// Repository interface for Tests.
    /// </summary>
    public interface ITestRepository
    {
        Task<List<Test>> GetAllAsync();
        Task<Test?> FindByIdAsync(int id);
        Task<List<Test>> FindTestsByCategoryAsync(string category);
        Task<Test> CreateAsync(Test test);
        Task<Test?> UpdateAsync(Test test);
        Task<bool> DeleteAsync(int id);
    }
}
