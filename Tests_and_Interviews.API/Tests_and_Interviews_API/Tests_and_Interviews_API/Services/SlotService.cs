namespace Tests_and_Interviews_API.Services
{
    using Tests_and_Interviews_API.Models;
    using Tests_and_Interviews_API.Repositories;
    using Tests_and_Interviews_API.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing slot entities, including retrieval, creation, update, and deletion of slots
    /// associated with recruiters.
    /// </summary>
    /// <remarks>SlotService acts as the main entry point for slot-related business logic and data access.</remarks>
    public class SlotService: ISlotService
    {
        private readonly ISlotRepository _repository;

        /// <summary>
        /// Initializes a new instance of the SlotService class using the specified slot repository.
        /// </summary>
        /// <param name="repository">The repository used to access and manage slot data. Cannot be null.</param>
        public SlotService(ISlotRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Asynchronously retrieves the list of available slots for a specified recruiter on a given date.
        /// </summary>
        /// <param name="recruiterId">The unique identifier of the recruiter whose slots are to be retrieved.</param>
        /// <param name="date">The date for which to retrieve available slots.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of slots available for
        /// the specified recruiter on the given date. The list is empty if no slots are available.</returns>
        public async Task<List<Slot>> GetSlotsAsync(int recruiterId, DateTime date)
        {
            return await this._repository.GetSlotsAsync(recruiterId, date);
        }

        /// <summary>
        /// Asynchronously retrieves all available slots associated with the specified recruiter.
        /// </summary>
        /// <param name="recruiterId">The unique identifier of the recruiter whose slots are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of slots for the
        /// specified recruiter. The list is empty if no slots are found.</returns>
        public async Task<List<Slot>> GetAllSlotsAsync(int recruiterId)
        {
            return await this._repository.GetAllSlotsAsync(recruiterId);
        }

        /// <summary>
        /// Asynchronously retrieves a slot with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the slot to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the slot with the specified
        /// identifier.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a slot with the specified identifier does not exist.</exception>
        public async Task<Slot> GetSlotByIdAsync(int id)
        {
            Slot? slot = await this._repository.GetByIdAsync(id);

            if (slot == null)
            {
                throw new KeyNotFoundException("Slot not found.");
            }

            return slot;
        }

        /// <summary>
        /// Asynchronously adds a new slot to the data store.
        /// </summary>
        /// <param name="slot">The slot to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added slot.</returns>
        public async Task<Slot> AddSlotAsync(Slot slot)
        {
            await this._repository.AddAsync(slot);

            return slot;
        }

        /// <summary>
        /// Updates the slot with the specified identifier using the provided slot data asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the slot to update.</param>
        /// <param name="slot">The slot data to use for the update. The <see cref="Slot.Id"/> property is ignored and replaced with the
        /// value of <paramref name="id"/>.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated <see cref="Slot"/>
        /// instance.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a slot with the specified <paramref name="id"/> does not exist.</exception>
        public async Task<Slot> UpdateSlotAsync(int id, Slot slot)
        {
            Slot? initialSlot = await this._repository.GetByIdAsync(slot.Id);

            if (initialSlot == null)
            {
                throw new KeyNotFoundException("Slot to update not found.");
            }

            slot.Id = id;

            return slot;
        }

        /// <summary>
        /// Asynchronously deletes the slot with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the slot to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the slot was
        /// successfully deleted.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a slot with the specified <paramref name="id"/> does not exist.</exception>
        public async Task<bool> DeleteSlotAsync(int id)
        {
            Slot? initialSlot = await this._repository.GetByIdAsync(id);

            if (initialSlot == null)
            {
                throw new KeyNotFoundException("Slot to delete not found.");
            }

            await this._repository.DeleteAsync(id);

            return true;
        }
    }
}
