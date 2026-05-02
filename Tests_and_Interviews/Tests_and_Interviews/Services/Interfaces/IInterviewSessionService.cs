using System.Collections.Generic;
using System.Threading.Tasks;
using Tests_and_Interviews.Models.Core;

namespace Tests_and_Interviews.Services.Interfaces
{
    public interface IInterviewSessionService
    {
        /// <summary>
        /// Loads the session and its questions, and marks the session as started.
        /// </summary>
        /// <param name="sessionId">The ID of the interview session.</param>
        /// <returns>The loaded interview session and its questions.</returns>
        Task<(InterviewSession Session, List<Question> Questions)> StartSessionAsync(int sessionId);

        /// <summary>
        /// Saves the recording path and marks the session as in progress.
        /// </summary>
        /// <param name="session">The current interview session.</param>
        /// <param name="recordingFilePath">The file path of the recorded video.</param>
        Task SubmitRecordingAsync(InterviewSession session, string recordingFilePath);

        /// <summary>
        /// Saves the score and marks the session as completed.
        /// </summary>
        /// <param name="sessionId">The ID of the interview session.</param>
        /// <param name="score">The score given by the interviewer.</param>
        Task SubmitScoreAsync(int sessionId, float score);

        /// <summary>
        /// Loads an interview session by its ID.
        /// </summary>
        /// <param name="sessionId">The ID of the interview session.</param>
        /// <returns>The interview session.</returns>
        Task<InterviewSession> GetSessionAsync(int sessionId);
    }
}

