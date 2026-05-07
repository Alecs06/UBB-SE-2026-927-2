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
    }
}