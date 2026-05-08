// <copyright file="TestService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Services.Interfaces;

    /// <summary>
    /// TestService class provides methods to manage the lifecycle of a test attempt,
    /// including starting a test, submitting answers, and retrieving available tests.
    /// </summary>
    public class TestService : ITestService
    {
        private readonly IGradingService gradingService;
        private readonly ITimerService timerService;
        private readonly IAttemptValidationService validationService;
        private readonly IDataProcessingService dataProcessingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestService"/> class with the specified dependencies.
        /// </summary>
        /// <param name="gradingService">The service responsible for grading answers.</param>
        /// <param name="timerService">The service responsible for managing test timers.</param>
        /// <param name="validationService">The service responsible for validating test attempts.</param>
        /// <param name="dataProcessingService">The service responsible for processing test data.</param>
        public TestService(
            IGradingService gradingService,
            ITimerService timerService,
            IAttemptValidationService validationService,
            IDataProcessingService dataProcessingService)
        {
            this.gradingService = gradingService;
            this.timerService = timerService;
            this.validationService = validationService;
            this.dataProcessingService = dataProcessingService;
        }

        /// <summary>
        /// Asynchronously starts a test attempt for a given user and test. It checks for existing attempts.
        /// </summary>
        /// <param name="userId">The ID of the user starting the test.</param>
        /// <param name="testId">The ID of the test to be attempted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartTestAsync(int userId, int testId)
        {
            await this.validationService.CheckExistingAttemptsAsync(userId, testId);
            var attempt = new TestAttempt
            {
                TestId = testId,
                ExternalUserId = userId,
                Status = TestStatus.IN_PROGRESS.ToString(),
                StartedAt = DateTime.UtcNow,
            };
            attempt.Start();
            HttpResponseMessage saveResponse = await ApiClient.Http.PostAsJsonAsync("testattempts", attempt.ToDto());
            saveResponse.EnsureSuccessStatusCode();
            TestAttemptDto? savedDto = await saveResponse.Content.ReadFromJsonAsync<TestAttemptDto>();
            int attemptId = savedDto!.ToEntity().Id;
            this.timerService.StartTimer(attemptId);
        }

        /// <summary>
        /// Asynchronously submits a test attempt by grading the answers and calculating the final score.
        /// </summary>
        /// <param name="attemptId">The ID of the test attempt to be submitted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SubmitTestAsync(int attemptId)
        {
            HttpResponseMessage attemptResponse = await ApiClient.Http.GetAsync($"testattempts/{attemptId}");
            attemptResponse.EnsureSuccessStatusCode();
            TestAttemptDto? attemptDto = await attemptResponse.Content.ReadFromJsonAsync<TestAttemptDto>();
            if (attemptDto == null)
            {
                throw new InvalidOperationException($"Attempt {attemptId} not found.");
            }
            TestAttempt attempt = attemptDto.ToEntity();

            HttpResponseMessage answersResponse = await ApiClient.Http.GetAsync($"answers/byattempt/{attemptId}");
            answersResponse.EnsureSuccessStatusCode();
            List<AnswerDto>? answerDtos = await answersResponse.Content.ReadFromJsonAsync<List<AnswerDto>>();
            List<Answer> answers = answerDtos?.Select(dto => dto.ToEntity()).ToList() ?? new List<Answer>();

            foreach (var answer in answers)
            {
                if (answer.Question == null)
                {
                    continue;
                }
                switch (answer.Question.Type)
                {
                    case QuestionType.SINGLE_CHOICE:
                        this.gradingService.GradeSingleChoice(answer.Question, answer);
                        break;
                    case QuestionType.MULTIPLE_CHOICE:
                        this.gradingService.GradeMultipleChoice(answer.Question, answer);
                        break;
                    case QuestionType.TEXT:
                        this.gradingService.GradeText(answer.Question, answer);
                        break;
                    case QuestionType.TRUE_FALSE:
                        this.gradingService.GradeTrueFalse(answer.Question, answer);
                        break;
                }
            }

            this.gradingService.CalculateFinalScore(attempt);
            attempt.Submit();
            HttpResponseMessage updateResponse = await ApiClient.Http.PutAsJsonAsync(
                $"testattempts/{attempt.Id}",
                attempt.ToDto());
            updateResponse.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Asynchronously retrieves the next available test for a given category.
        /// </summary>
        /// <param name="category">The category of the test to retrieve.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Test?> GetNextAvailableTestAsync(string category)
        {
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"tests/bycategory/{category}");
            response.EnsureSuccessStatusCode();
            List<TestDto>? dtos = await response.Content.ReadFromJsonAsync<List<TestDto>>();
            if (dtos == null || dtos.Count == 0)
            {
                return null;
            }
            return dtos[0].ToEntity();
        }

        /// <summary>
        /// Submits an attempt for the specified user and test using the provided answers. This method will persist
        /// the answers, finalize the attempt (grading), process the finalized attempt and return the final score.
        /// </summary>
        /// <param name="userId">The ID of the user submitting the attempt.</param>
        /// <param name="testId">The ID of the test being submitted.</param>
        /// <param name="answers">The collection of answers provided by the user.</param>
        /// <returns>The final score as a float.</returns>
        public async Task<float> SubmitAttemptAsync(int userId, int testId, IEnumerable<AnswerDto> answers)
        {
            HttpResponseMessage attemptResponse = await ApiClient.Http.GetAsync($"testattempts/byuser/{userId}/bytest/{testId}");
            if (!attemptResponse.IsSuccessStatusCode)
            {
                return 0f;
            }
            TestAttemptDto? attemptDto = await attemptResponse.Content.ReadFromJsonAsync<TestAttemptDto>();
            if (attemptDto == null)
            {
                return 0f;
            }
            int attemptId = attemptDto.ToEntity().Id;

            foreach (var answerDto in answers)
            {
                if (string.IsNullOrEmpty(answerDto.Value))
                {
                    continue;
                }
                var answer = new Answer
                {
                    AttemptId = attemptId,
                    QuestionId = answerDto.QuestionId,
                    Value = answerDto.Value,
                };
                HttpResponseMessage saveResponse = await ApiClient.Http.PostAsJsonAsync("answers", answer.ToDto());
                saveResponse.EnsureSuccessStatusCode();
            }

            await this.SubmitTestAsync(attemptId);
            await this.dataProcessingService.ProcessFinalizedAttemptAsync(attemptId);

            HttpResponseMessage finalResponse = await ApiClient.Http.GetAsync($"testattempts/byuser/{userId}/bytest/{testId}");
            if (!finalResponse.IsSuccessStatusCode)
            {
                return 0f;
            }
            TestAttemptDto? finalDto = await finalResponse.Content.ReadFromJsonAsync<TestAttemptDto>();
            return finalDto != null ? (float)(finalDto.ToEntity().Score ?? 0m) : 0f;
        }

        public async Task<List<Test>> FindTestsByCategoryAsync(string category)
        {
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"tests/bycategory/{category}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<Test>();
            }

            response.EnsureSuccessStatusCode();
            List<TestDto>? testsDto = await response.Content.ReadFromJsonAsync<List<TestDto>>();
            return testsDto!.Select(session => session.ToEntity()).ToList();
        }

        public async Task<Test> FindByIdAsync(int id)
        {
            HttpResponseMessage response = await ApiClient.Http.GetAsync($"tests/{id}");
            response.EnsureSuccessStatusCode();
            TestDto? testDto = await response.Content.ReadFromJsonAsync<TestDto>();
            return testDto!.ToEntity();
        }
    }
}