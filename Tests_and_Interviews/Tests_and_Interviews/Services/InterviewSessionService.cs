using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tests_and_Interviews.Models.Core;
using Tests_and_Interviews.Models.Enums;
using Tests_and_Interviews.Repositories.Interfaces;
using Tests_and_Interviews.Services.Interfaces;

namespace Tests_and_Interviews.Services
{
    public class InterviewSessionService : IInterviewSessionService
    {
        private readonly IInterviewSessionRepository sessionRepo;
        private readonly IQuestionRepository questionRepo;

        public InterviewSessionService(IInterviewSessionRepository sessionRepository, IQuestionRepository questionRepository)
        {
            this.sessionRepo = sessionRepository;
            this.questionRepo = questionRepository;
        }

        public async Task<(InterviewSession Session, List<Question> Questions)> StartSessionAsync(int sessionId)
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

        public async Task SubmitRecordingAsync(InterviewSession session, string recordingFilePath)
        {
            session.Video = recordingFilePath;
            session.Status = InterviewStatus.InProgress.ToString();
            await this.sessionRepo.UpdateInterviewSessionAsync(session);
        }

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

        public async Task<InterviewSession> GetSessionAsync(int sessionId)
        {
            return await this.sessionRepo.GetInterviewSessionByIdAsync(sessionId);
        }
    }
}
