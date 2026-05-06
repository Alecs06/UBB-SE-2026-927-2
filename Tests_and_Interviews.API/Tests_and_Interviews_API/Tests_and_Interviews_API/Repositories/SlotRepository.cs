namespace Tests_and_Interviews_API.Repositories
{
    using Tests_and_Interviews_API.Models;
    using Tests_and_Interviews_API.Models.Enums;

    /// <summary>
    /// MOCK REPOSITORY FOR TESTING CONTROLLER TODO
    /// </summary>
    public class SlotRepository: ISlotRepository
    {
        private readonly List<Slot> _slots = new();

        public SlotRepository()
        {
            // Seed with realistic sample slots
            _slots.Add(new Slot
            {
                Id = 1,
                RecruiterId = 10,
                CandidateId = null,
                StartTime = DateTime.Today.AddHours(9),
                EndTime = DateTime.Today.AddHours(10),
                Duration = 60,
                Status = SlotStatus.Free,
                InterviewType = "Technical"
            });

            _slots.Add(new Slot
            {
                Id = 2,
                RecruiterId = 10,
                CandidateId = 5,
                StartTime = DateTime.Today.AddHours(10),
                EndTime = DateTime.Today.AddHours(11),
                Duration = 60,
                Status = SlotStatus.Occupied,
                InterviewType = "HR"
            });

            _slots.Add(new Slot
            {
                Id = 3,
                RecruiterId = 20,
                CandidateId = null,
                StartTime = DateTime.Today.AddDays(1).AddHours(14),
                EndTime = DateTime.Today.AddDays(1).AddHours(15),
                Duration = 60,
                Status = SlotStatus.Free,
                InterviewType = "Technical"
            });
        }

        // ---------------------------------------------------------
        // ASYNC METHODS
        // ---------------------------------------------------------

        public Task<List<Slot>> GetSlotsAsync(int recruiterId, DateTime date)
        {
            var result = _slots
                .Where(s => s.RecruiterId == recruiterId &&
                            s.StartTime.Date == date.Date &&
                            s.Status == SlotStatus.Free)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<List<Slot>> GetAllSlotsAsync(int recruiterId)
        {
            var result = _slots
                .Where(s => s.RecruiterId == recruiterId)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<Slot?> GetByIdAsync(int id)
        {
            var slot = _slots.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(slot);
        }

        public Task AddAsync(Slot slot)
        {
            slot.Id = _slots.Count == 0 ? 1 : _slots.Max(s => s.Id) + 1;
            _slots.Add(slot);

            return Task.CompletedTask;
        }

        public Task UpdateAsync(Slot slot)
        {
            var existing = _slots.FirstOrDefault(s => s.Id == slot.Id);
            if (existing == null)
                return Task.CompletedTask; // no exception

            _slots.Remove(existing);
            _slots.Add(slot);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var existing = _slots.FirstOrDefault(s => s.Id == id);
            if (existing != null)
                _slots.Remove(existing);

            return Task.CompletedTask;
        }

        // ---------------------------------------------------------
        // SYNC METHODS
        // ---------------------------------------------------------

        public List<Slot> GetSlots(int recruiterId, DateTime date)
        {
            return _slots
                .Where(s => s.RecruiterId == recruiterId &&
                            s.StartTime.Date == date.Date &&
                            s.Status == SlotStatus.Free)
                .ToList();
        }

        public List<Slot> GetAllSlots(int recruiterId)
        {
            return _slots
                .Where(s => s.RecruiterId == recruiterId)
                .ToList();
        }

        public Slot? GetById(int id)
        {
            return _slots.FirstOrDefault(s => s.Id == id);
        }

        public void Add(Slot slot)
        {
            slot.Id = _slots.Count == 0 ? 1 : _slots.Max(s => s.Id) + 1;
            _slots.Add(slot);
        }

        public void Update(Slot slot)
        {
            var existing = _slots.FirstOrDefault(s => s.Id == slot.Id);
            if (existing == null)
                return;

            _slots.Remove(existing);
            _slots.Add(slot);
        }

        public void Delete(int id)
        {
            var existing = _slots.FirstOrDefault(s => s.Id == id);
            if (existing != null)
                _slots.Remove(existing);
        }
    }
}
