// <copyright file="TestService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Services
{
    using System;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services.Interfaces;

    /// <summary>
    /// TestService class provides methods to manage the lifecycle of a test attempt,
    /// including starting a test, submitting answers, and retrieving available tests.
    /// </summary>
    public class TestService : ITestService
    {
        private readonly ITestRepository testRepository;
        private readonly ITestAttemptRepository attemptRepository;
        private readonly IAnswerRepository answerRepository;
        private readonly IGradingService gradingService;
        private readonly ITimerService timerService;
        private readonly IAttemptValidationService validationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestService"/> class with the specified dependencies.
        /// </summary>
        /// <param name="testRepository">The repository for accessing test data.</param>
        /// <param name="attemptRepository">The repository for managing test attempts.</param>
        /// <param name="answerRepository">The repository for managing answers.</param>
        /// <param name="gradingService">The service responsible for grading answers.</param>
        /// <param name="timerService">The service responsible for managing test timers.</param>
        /// <param name="validationService">The service responsible for validating test attempts.</param>
        public TestService(
            ITestRepository testRepository,
            ITestAttemptRepository attemptRepository,
            IAnswerRepository answerRepository,
            IGradingService gradingService,
            ITimerService timerService,
            IAttemptValidationService validationService)
        {
            this.testRepository = testRepository;
            this.attemptRepository = attemptRepository;
            this.answerRepository = answerRepository;
            this.gradingService = gradingService;
            this.timerService = timerService;
            this.validationService = validationService;
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
            await this.attemptRepository.SaveAsync(attempt);
            this.timerService.StartTimer(attempt.Id);
        }

        /// <summary>
        /// Asynchronously submits a test attempt by grading the answers and calculating the final score.
        /// </summary>
        /// <param name="attemptId">The ID of the test attempt to be submitted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SubmitTestAsync(int attemptId)
        {
            var answers = await this.answerRepository.FindByAttemptAsync(attemptId);
            var attempt = new TestAttempt { Id = attemptId, Answers = answers };

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
            await this.attemptRepository.UpdateAsync(attempt);
        }

        /// <summary>
        /// Asynchronously retrieves the next available test for a given category.
        /// </summary>
        /// <param name="category">The category of the test to retrieve.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Test?> GetNextAvailableTestAsync(string category)
        {
            var tests = await this.testRepository.FindTestsByCategoryAsync(category);

            if (tests.Count == 0)
            {
                return null;
            }

            return tests[0];
        }
    }
}