// <copyright file="InterviewSessionService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Tests_and_Interviews.Api;
    using Tests_and_Interviews.Dtos;
    using Tests_and_Interviews.Mappers;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories.Interfaces;
    using Tests_and_Interviews.Services.Interfaces;
    using Windows.Storage;
    using Windows.Storage.Streams;

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
        /// Uploads a video recording file for the specified interview session and updates the session's video and
        /// status information.
        /// </summary>
        /// <remarks>After a successful upload, the session's video and status are updated to reflect the
        /// new recording. The method ensures the server response indicates success before updating the session.</remarks>
        /// <param name="session">The interview session to which the recording will be attached. Must not be null.</param>
        /// <param name="recordingFilePath">The full file system path to the video recording file to upload. Must refer to an existing file.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SubmitRecordingAsync(InterviewSession session, string recordingFilePath)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(recordingFilePath);
            using IRandomAccessStreamWithContentType randomAccessStream = await file.OpenReadAsync();
            using Stream stream = randomAccessStream.AsStreamForRead();

            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StreamContent(stream), "file", file.Name);

            HttpResponseMessage response = await ApiClient.Http.PostAsync(
                $"interviewsessions/{session.Id}/video",
                content);

            response.EnsureSuccessStatusCode();

            InterviewSessionDto? updatedSession = await response.Content.ReadFromJsonAsync<InterviewSessionDto>();

            // TODO next lines currently save changes to db but that's the job of the api (used next lines for testing)
            // remove next lines when api really saves to db and the rest of the app is connected to the api
            session.Video = updatedSession?.ToEntity().Video;
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