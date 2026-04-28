using Moq;
using Tests_and_Interviews.Dtos;
using Tests_and_Interviews.Models;
using Tests_and_Interviews.Models.Enums;
using Tests_and_Interviews.Repositories;
using Tests_and_Interviews.Services;


namespace TestsAndInterviews.Tests.Services
{
	public class SlotServiceTests
	{
		[Fact]
		public async Task LoadRecruiterVisibleSlots_ForExistentRecruiter_ReturnsListWithOccupiedAndFreeSlots()
		{
			var recruiterId = 1;
			var date = new DateTime(2026, 04, 21);

			var mockRepository = new Mock<ISlotRepository>();
			mockRepository.Setup(repository => repository.GetSlotsAsync(recruiterId, date))
				.ReturnsAsync(new List<Slot>
				{
					new Slot
					{
						Id = 1,
						RecruiterId = 1,
						CandidateId = 1,
						StartTime = new DateTime(2026, 04, 21, 8, 0, 0),
						EndTime = new DateTime(2026, 04, 21, 9, 0, 0),
						Duration = 60,
						Status = SlotStatus.Occupied,
					}
				});

			var service = new SlotService(mockRepository.Object);

			var recruiterSlots = await service.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

			mockRepository.Verify(repository => repository.GetSlotsAsync(recruiterId, date), Times.Once);

			var resultOccupiedSlot = recruiterSlots[0];
			var resultFreeSlot = recruiterSlots.Last();

			Assert.Equal(recruiterId, resultOccupiedSlot.RecruiterId);
			Assert.Equal(new DateTime(2026, 04, 21, 8, 0, 0), resultOccupiedSlot.StartTime);
			Assert.Equal(new DateTime(2026, 04, 21, 9, 0, 0), resultOccupiedSlot.EndTime);
			Assert.Equal(60, resultOccupiedSlot.Duration);
			Assert.Equal(SlotStatus.Occupied, resultOccupiedSlot.Status);

			Assert.Equal(recruiterId, resultFreeSlot.RecruiterId);
			Assert.Equal(new DateTime(2026, 04, 21, 17, 30, 0), resultFreeSlot.StartTime);
			Assert.Equal(new DateTime(2026, 04, 21, 18, 0, 0), resultFreeSlot.EndTime);
			Assert.Equal(30, resultFreeSlot.Duration);
			Assert.Equal(SlotStatus.Free, resultFreeSlot.Status);
		}

		[Fact]
		public async Task LoadRecruiterVisibleSlots_ForRecruiterWithNoAllocatedSlots_ReturnsListWithFreeSlotsAllDay()
		{
			var recruiterId = 1;
			var date = new DateTime(2026, 04, 21);

			var mockRepository = new Mock<ISlotRepository>();
			mockRepository.Setup(repository => repository.GetSlotsAsync(recruiterId, date))
				.ReturnsAsync(new List<Slot>());

			var service = new SlotService(mockRepository.Object);

			var recruiterSlots = await service.LoadRecruiterVisibleSlotsAsync(recruiterId, date);

			mockRepository.Verify(repository => repository.GetSlotsAsync(recruiterId, date), Times.Once);

			var resultOccupiedSlot = recruiterSlots[0];
			var resultFreeSlot = recruiterSlots.Last();

			foreach (var slot in recruiterSlots)
			{
				Assert.Equal(recruiterId, slot.RecruiterId);
				Assert.Equal(30, slot.Duration);
				Assert.Equal(SlotStatus.Free, slot.Status);
			}

			Assert.Equal(20, recruiterSlots.Count);
		}

		[Fact]
		public async Task CreateNewSlot_FromValidBaseSlot_CallsCreateMethodWithCorrectArguments()
		{
			var mockBaseSlot = new SlotDto
			{
				Id = 0,
				StartTime = new DateTime(2026, 04, 21, 10, 0, 0),
			};
			var duration = 60;

			var mockRepository = new Mock<ISlotRepository>();

			var service = new SlotService(mockRepository.Object);

			await service.CreateRecruiterSlotAsync(mockBaseSlot, duration);

			mockRepository.Verify(repository => repository.AddAsync(
				It.Is<Slot>( slot =>
					slot.Id == mockBaseSlot.Id && 
					slot.Status == SlotStatus.Free &&
					DateTime.Equals(slot.StartTime, mockBaseSlot.StartTime))
				), Times.Once);
		}

		[Fact]
		public async Task CreateNewSlot_OverlappingSlot_ThrowsException()
		{
			var mockBaseSlot = new SlotDto
			{
				Id = 0,
				StartTime = new DateTime(2026, 04, 21, 10, 30, 0),
			};
			var duration = 60;

			var mockRepository = new Mock<ISlotRepository>();
			mockRepository.Setup(repository => repository.AddAsync(It.IsAny<Slot>()))
				.ThrowsAsync(new Exception("Slot overlaps with an existing appointment!"));

			var service = new SlotService(mockRepository.Object);

			await Assert.ThrowsAsync<Exception>(async () => await service.CreateRecruiterSlotAsync(mockBaseSlot, duration));
		}

		[Fact]
		public async Task DeleteRecruiterSlot_CallsDeleteMethodWithCorrectArguments()
		{
			var mockRepository = new Mock<ISlotRepository>();

			var service = new SlotService(mockRepository.Object);

			var slotToDeleteId = 1;

			await service.DeleteRecruiterSlotAsync(slotToDeleteId);

			mockRepository.Verify(repository => repository.DeleteAsync(
				It.Is<int>(id => id == slotToDeleteId)), Times.Once);
		}

		[Theory]
		[InlineData(5, 0)]
		[InlineData(21, 0)]
		public async Task UpdateRecruiterSlot_InvalidNewStartTime_ThrowsException(int newStartTimeHours, int newStartTimeMinutes)
		{
			var initialSlot = new SlotDto
			{
				Id = 0,
				RecruiterId = 0,
				StartTime = new DateTime(2026, 04, 21, 10, 30, 0),
			};
			var newStartTime = new DateTime(2026, 04, 21, newStartTimeHours, newStartTimeMinutes, 0);
			var duration = 30;

			var mockRepository = new Mock<ISlotRepository>();
			var service = new SlotService(mockRepository.Object);
			await Assert.ThrowsAsync<Exception>(async () => await service.UpdateRecruiterSlotAsync(initialSlot, newStartTime, duration));
		}

		[Fact]
		public async Task UpdateRecruiterSlot_ValidNewStartTimeAndDuration_CallsUpdateMethodWithCorrectArguments()
		{
			var initialSlot = new SlotDto
			{
				Id = 0,
				RecruiterId = 0,
				StartTime = new DateTime(2026, 04, 21, 10, 30, 0),
			};
			var newStartTime = new DateTime(2026, 04, 21, 12, 0, 0);
			var duration = 30;

			var mockRepository = new Mock<ISlotRepository>();
			var service = new SlotService(mockRepository.Object);

			await service.UpdateRecruiterSlotAsync(initialSlot, newStartTime, duration);

			mockRepository.Verify(repository => repository.UpdateAsync(
				It.Is<Slot>(slot =>
					slot.Id == initialSlot.Id &&
					slot.RecruiterId == initialSlot.RecruiterId &&
					slot.StartTime == newStartTime &&
					slot.EndTime == newStartTime.AddMinutes(duration))
				), Times.Once);
		}
	}
}
