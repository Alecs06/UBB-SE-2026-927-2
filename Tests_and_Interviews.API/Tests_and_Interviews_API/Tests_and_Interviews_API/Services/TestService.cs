// <copyright file="TestService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews_API.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Repositories.Interfaces;
    using Tests_and_Interviews_API.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing tests.
    /// </summary>
    public class TestService : ITestService
    {
        private readonly ITestRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access test data. Cannot be null.</param>
        public TestService(ITestRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Asynchronously retrieves all test entities from the data store.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all test entities. The
        /// list will be empty if no tests are found.</returns>
        public async Task<List<Test>> GetAll()
        {
            return await this._repository.GetTestsASync();
        }

        /// <summary>
        /// Asynchronously retrieves a list of all available category names.
        /// </summary>
        /// <returns>A list of strings containing the names of all categories. The list is empty if no categories are found.</returns>
        public async Task<List<string>> GetCategories()
        {
            return await this._repository.GetAllCategories();
        }

        /// <summary>
        /// Asynchronously retrieves the test with the specified identifier, including its associated questions.
        /// </summary>
        /// <param name="id">The unique identifier of the test.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the test, or null if not found.</returns>
        public async Task<Test?> FindByIdAsync(int id)
        {
            return await this._repository.FindByIdAsync(id);
        }

        /// <summary>
        /// Asynchronously retrieves all tests belonging to the specified category, including their associated questions.
        /// </summary>
        /// <param name="category">The category to filter tests by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of tests in the specified category.</returns>
        public async Task<List<Test>> FindTestsByCategoryAsync(string category)
        {
            return await this._repository.FindTestsByCategoryAsync(category);
        }

        /// <summary>
        /// Asynchronously adds a new test entity to the data store.
        /// </summary>
        /// <param name="test">The test entity to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added test entity.</returns>
        public async Task<Test> AddTestASync(Test test)
        {
            test.CreatedAt = DateTime.UtcNow;
            await this._repository.AddAsync(test);

            return test;
        }

        /// <summary>
        /// Asynchronously updates an existing test with the specified identifier using the provided test data.
        /// </summary>
        /// <param name="id">The unique identifier of the test to update.</param>
        /// <param name="test">The test data to apply to the existing test. The <see cref="Test.Id"/> property is ignored and will be set
        /// to the specified <paramref name="id"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Test"/>
        /// instance.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a test with the specified <paramref name="id"/> does not exist.</exception>
        public async Task<Test> UpdateTestAsync(int id, Test test)
        {
            Test? initialTest = await this._repository.FindByIdAsync(id);
            if (initialTest == null) {
                throw new KeyNotFoundException("Test to update not found.");
            }

            test.Id = id;
            await this._repository.UpdateAsync(test);

            return test;
        }

        /// <summary>
        /// Asynchronously deletes the test with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the test to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the test was
        /// successfully deleted; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a test with the specified <paramref name="id"/> does not exist.</exception>
        public async Task<bool> DeleteTestAsync(int id)
        {
            Test? initialTest = await this._repository.FindByIdAsync(id);
            if (initialTest == null) {
                throw new KeyNotFoundException("Test to update not found.");
            }

            try {
                await this._repository.DeleteAsync(id);
                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}