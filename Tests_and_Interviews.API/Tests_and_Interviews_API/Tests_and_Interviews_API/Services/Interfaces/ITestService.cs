namespace Tests_and_Interviews_API.Services.Interfaces
{
    using Tests_and_Interviews_API.DTOs;
    /// <summary>
    /// Service layer for Test CRUD operations.
    /// Sits between the controller and the repository.
    /// </summary>
    public interface ITestService
    {
        Task<List<TestSummaryDto>> GetAllTestsAsync();
        Task<TestDetailDto?> GetTestByIdAsync(int id);
        Task<List<TestSummaryDto>> GetTestsByCategoryAsync(string category);
        Task<TestDetailDto> CreateTestAsync(CreateTestDto dto);
        Task<TestDetailDto?> UpdateTestAsync(int id, UpdateTestDto dto);
        Task<bool> DeleteTestAsync(int id);
    }
}
