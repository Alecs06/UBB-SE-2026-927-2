// <copyright file="ITestService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews_API.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tests_and_Interviews_API.Models.Core;

    /// <summary>
    /// Defines operations for managing tests.
    /// </summary>
    public interface ITestService
    {
        /// <summary>
        /// Asynchronously retrieves the test with the specified identifier, including its associated questions.
        /// </summary>
        /// <param name="id">The unique identifier of the test.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the test, or null if not found.</returns>
        Task<Test?> FindByIdAsync(int id);

        /// <summary>
        /// Asynchronously retrieves all tests belonging to the specified category, including their associated questions.
        /// </summary>
        /// <param name="category">The category to filter tests by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of tests in the specified category.</returns>
        Task<List<Test>> FindTestsByCategoryAsync(string category);
    }
}