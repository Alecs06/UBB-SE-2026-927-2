// <copyright file="InterviewSessionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services.Interfaces;

    /// <summary>
    /// Handles all business logic related to interview sessions.
    /// </summary>
    public class InterviewSessionService : IInterviewSessionService
    {
        private readonly IInterviewSessionRepository sessionRepo;
        private readonly IQuestionRepository questionRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterviewSessionService"/> class.
        /// </summary>
        /// <param name="sessionRepository">The interview session repository.</param>
        /// <param name="questionRepository">The question repository.</param>
        public InterviewSessionService(IInterviewSessionRepository sessionRepository, IQuestionRepository questionRepository)
        {
            this.sessionRepo = sessionRepository;
            this.questionRepo = questionRepository;
        }

        /// <summary>
        /// Loads the session and its questions, and marks the session as started.
        /// </summary>
        /// <param name="sessionId">The ID of the interview session.</param>
        /// <returns>A tuple containing the loaded interview session and its questions.</returns>
        public async Task<(InterviewSession? Session, List<Question> Questions)> StartSessionAsync(int sessionId)
        {
            var session = await this.sessionRepo.GetInterviewSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.DateStart = DateTime.UtcNow;
                await this.sessionRepo.UpdateInterviewSessionAsync(session);
            }

            var questions = session != null
                ? await this.questionRepo.GetInterviewQuestionsByPositionAsync(session.PositionId)
                : new List<Question>();

            return (session, questions);
        }

        /// <summary>
        /// Saves the recording path and marks the session as in progress.
        /// </summary>
        /// <param name="session">The current interview session.</param>
        /// <param name="recordingFilePath">The file path of the recorded video.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SubmitRecordingAsync(InterviewSession session, string recordingFilePath)
        {
            session.Video = recordingFilePath;
            session.Status = InterviewStatus.InProgress.ToString();
            await this.sessionRepo.UpdateInterviewSessionAsync(session);
        }

        /// <summary>
        /// Saves the score and marks the session as completed.
        /// </summary>
        /// <param name="sessionId">The ID of the interview session.</param>
        /// <param name="score">The score given by the interviewer.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SubmitScoreAsync(int sessionId, float score)
        {
            var session = await this.sessionRepo.GetInterviewSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.Score = (decimal)score;
                session.Status = InterviewStatus.Completed.ToString();
                await this.sessionRepo.UpdateInterviewSessionAsync(session);
            }
        }

        /// <summary>
        /// Loads an interview session by its ID.
        /// </summary>
        /// <param name="sessionId">The ID of the interview session.</param>
        /// <returns>The interview session corresponding to the specified ID.</returns>
        public async Task<InterviewSession> GetSessionAsync(int sessionId)
        {
            return await this.sessionRepo.GetInterviewSessionByIdAsync(sessionId);
        }
    }
}