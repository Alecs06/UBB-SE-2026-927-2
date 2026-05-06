namespace Tests_and_Interviews_API.Repositories
{
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Repositories.Interfaces;

    /// <summary>
    /// MOCK REPOSITORY FOR TESTING CONTROLLER TODO
    /// </summary>
    public class InterviewSessionRepository: IInterviewSessionRepository
    {
        private readonly List<InterviewSession> _sessions = new();

        public InterviewSessionRepository()
        {
            // Seed with some fake data for testing
            _sessions.Add(new InterviewSession
            {
                Id = 1,
                PositionId = 10,
                ExternalUserId = 5,
                InterviewerId = 3,
                DateStart = DateTime.Now.AddDays(1),
                Status = "Scheduled",
                Score = null,
                Video = null
            });

            _sessions.Add(new InterviewSession
            {
                Id = 2,
                PositionId = 11,
                ExternalUserId = null,
                InterviewerId = 4,
                DateStart = DateTime.Now.AddDays(2),
                Status = "Completed",
                Score = 8.5m,
                Video = "Videos/test.mp4"
            });
        }

        public Task<InterviewSession?> GetInterviewSessionByIdAsync(int id)
        {
            return Task.FromResult(_sessions.FirstOrDefault(s => s.Id == id));
        }

        public InterviewSession? GetInterviewSessionById(int id)
        {
            return _sessions.FirstOrDefault(s => s.Id == id);
        }

        public Task<List<InterviewSession>> GetScheduledSessionsAsync()
        {
            var result = _sessions
                .Where(s => s.Status == "Scheduled")
                .ToList();

            return Task.FromResult(result);
        }

        public Task<List<InterviewSession>> GetSessionsByStatusAsync(string status)
        {
            var result = _sessions
                .Where(s => s.Status == status)
                .ToList();

            return Task.FromResult(result);
        }

        public void Add(InterviewSession session)
        {
            // Simulate auto-increment ID
            session.Id = _sessions.Count == 0 ? 1 : _sessions.Max(s => s.Id) + 1;
            _sessions.Add(session);
        }

        public Task UpdateInterviewSessionAsync(InterviewSession updated)
        {
            var existing = _sessions.FirstOrDefault(s => s.Id == updated.Id);
            if (existing == null)
                return Task.CompletedTask; // no exception, service handles null

            // Replace the object
            _sessions.Remove(existing);
            _sessions.Add(updated);

            return Task.CompletedTask;
        }

        public void Delete(InterviewSession session)
        {
            _sessions.Remove(session);
        }
    }
}
