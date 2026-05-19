// <copyright file="TestService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Services.Interfaces;

    /// <summary>
    /// Calls the API for all test-related operations.
    /// </summary>
    public class TestService : ITestService
    {
        private readonly HttpClient http;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestService"/> class.
        /// </summary>
        public TestService()
        {
            this.http = ApiClient.Http;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestService"/> class.
        /// </summary>
        public TestService(HttpClient httpClient)
        {
            this.http = httpClient ?? ApiClient.Http;
        }

        /// <inheritdoc/>
        public async Task StartTestAsync(int userId, int testId)
        {
            var payload = new { UserId = userId, TestId = testId };
            HttpResponseMessage response =
                await this.http.PostAsJsonAsync("tests/start", payload);
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc/>
        public async Task SubmitTestAsync(int attemptId)
        {
            HttpResponseMessage response =
                await this.http.PostAsJsonAsync($"tests/submit/{attemptId}", new { });
            response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc/>
        public async Task<float> SubmitAttemptAsync(
            int userId, int testId, IEnumerable<AnswerDto> answers)
        {
            var payload = new
            {
                UserId = userId,
                TestId = testId,
                Answers = answers,
            };

            HttpResponseMessage response =
                await this.http.PostAsJsonAsync("tests/submit-attempt", payload);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<float>();
        }

        /// <inheritdoc/>
        public async Task<Test?> GetNextAvailableTestAsync(string category)
        {
            HttpResponseMessage response =
                await this.http.GetAsync($"tests/bycategory/{category}");
            response.EnsureSuccessStatusCode();

            List<TestDto>? dtos =
                await response.Content.ReadFromJsonAsync<List<TestDto>>();

            if (dtos == null || dtos.Count == 0)
            {
                return null;
            }

            return dtos[0].ToEntity();
        }

        /// <inheritdoc/>
        public async Task<List<Test>> FindTestsByCategoryAsync(string category)
        {
            HttpResponseMessage response =
                await this.http.GetAsync($"tests/bycategory/{category}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<Test>();
            }

            response.EnsureSuccessStatusCode();
            List<TestDto>? testsDto =
                await response.Content.ReadFromJsonAsync<List<TestDto>>();
            return testsDto!.Select(t => t.ToEntity()).ToList();
        }

        /// <inheritdoc/>
        public async Task<Test> FindByIdAsync(int id)
        {
            HttpResponseMessage response = await this.http.GetAsync($"tests/{id}");
            response.EnsureSuccessStatusCode();
            TestDto? testDto = await response.Content.ReadFromJsonAsync<TestDto>();
            return testDto!.ToEntity();
        }
    }
}