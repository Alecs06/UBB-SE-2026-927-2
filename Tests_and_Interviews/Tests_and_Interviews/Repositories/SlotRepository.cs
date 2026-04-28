namespace Tests_and_Interviews.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Microsoft.Data.SqlClient;
	using Tests_and_Interviews.Helpers;
	using Tests_and_Interviews.Models;
	using Tests_and_Interviews.Models.Enums;

	/// <summary>
	/// Provides methods for managing Slot entities in the database, including retrieval, creation, update, and deletion
	/// operations for recruiter slots.
	/// </summary>
	/// <remarks>Implements both synchronous and asynchronous operations for slot management. Ensures slot time
	/// validation to prevent overlapping appointments.</remarks>
	public class SlotRepository : ISlotRepository
	{
		private readonly string connectionString;

		/// <summary>
		/// Initializes a new instance of the <see cref="SlotRepository"/> class.
		/// </summary>
		public SlotRepository()
		{
			this.connectionString = Env.CONNECTION_STRING;
		}

		/// <summary>
		/// Asynchronously retrieves all slots for the specified recruiter on the given date.
		/// </summary>
		/// <param name="recruiterId">The unique identifier of the recruiter.</param>
		/// <param name="date">The date for which to retrieve slots.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a list of slots for the recruiter on
		/// the specified date.</returns>
		public async Task<List<Slot>> GetSlotsAsync(int recruiterId, DateTime date)
		{
			var slots = new List<Slot>();
			string query = @"
                SELECT id, recruiter_id, start_time, end_time, duration, status, interview_type
                FROM Slots 
                WHERE recruiter_id = @recruiter_id AND CAST(start_time AS DATE) = CAST(@date AS DATE)
                ORDER BY start_time";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@recruiter_id", recruiterId);
				command.Parameters.AddWithValue("@date", date);

				await connection.OpenAsync();
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						slots.Add(this.MapSlot(reader));
					}
				}
			}

			return slots;
		}

		/// <summary>
		/// Asynchronously retrieves all slots associated with the specified recruiter, ordered by start time.
		/// </summary>
		/// <param name="recruiterId">The unique identifier of the recruiter.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a list of slots for the recruiter.</returns>
		public async Task<List<Slot>> GetAllSlotsAsync(int recruiterId)
		{
			var slots = new List<Slot>();
			string query = @"
                SELECT id, recruiter_id, start_time, end_time, duration, status, interview_type
                FROM Slots 
                WHERE recruiter_id = @recruiter_id
                ORDER BY start_time";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@recruiter_id", recruiterId);

				await connection.OpenAsync();
				using (var reader = await command.ExecuteReaderAsync())
				{
					while (await reader.ReadAsync())
					{
						slots.Add(this.MapSlot(reader));
					}
				}
			}

			return slots;
		}

		/// <summary>
		/// Asynchronously retrieves a slot by its unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier of the slot.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the slot if found; otherwise, null.</returns>
		public async Task<Slot?> GetByIdAsync(int id)
		{
			string query = "SELECT id, recruiter_id, start_time, end_time FROM Slots WHERE id = @id";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@id", id);

				await connection.OpenAsync();
				using (var reader = await command.ExecuteReaderAsync())
				{
					if (await reader.ReadAsync())
					{
						return this.MapSlot(reader);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Asynchronously adds the specified slot.
		/// </summary>
		/// <param name="slot">The slot to add.</param>
		/// <returns>A task that represents the asynchronous operation.</returns>
		/// <exception cref="Exception">When slot is overlapping with an existing slot.</exception>
		public async Task AddAsync(Slot slot)
		{
			using (var connection = new SqlConnection(this.connectionString))
			{
				await connection.OpenAsync();

				string checkOverlapQuery = @"
                    SELECT COUNT(1) 
                    FROM Slots 
                    WHERE recruiter_id = @recruiter_id 
                      AND CAST(start_time AS DATE) = CAST(@start_time AS DATE)
                      AND @start_time < end_time 
                      AND @end_time > start_time";

				using (var checkCommand = new SqlCommand(checkOverlapQuery, connection))
				{
					checkCommand.Parameters.AddWithValue("@recruiter_id", slot.RecruiterId);
					checkCommand.Parameters.AddWithValue("@start_time", slot.StartTime);
					checkCommand.Parameters.AddWithValue("@end_time", slot.EndTime);

					int overlapCount = (int)await checkCommand.ExecuteScalarAsync();
					if (overlapCount > 0)
					{
						throw new Exception("Slot overlaps with an existing appointment!");
					}
				}

				string insertQuery = @"
                   INSERT INTO Slots (recruiter_id, start_time, end_time, status, duration, interview_type) 
                   OUTPUT INSERTED.id 
                   VALUES (@recruiter_id, @start_time, @end_time, @status, @duration, @interview_type)";

				using (var insertCommand = new SqlCommand(insertQuery, connection))
				{
					insertCommand.Parameters.AddWithValue("@recruiter_id", slot.RecruiterId);
					insertCommand.Parameters.AddWithValue("@start_time", slot.StartTime);
					insertCommand.Parameters.AddWithValue("@end_time", slot.EndTime);
					insertCommand.Parameters.AddWithValue("@status", slot.Status);
					insertCommand.Parameters.AddWithValue("@duration", slot.Duration);
					insertCommand.Parameters.AddWithValue("@interview_type", slot.InterviewType);

					slot.Id = (int)await insertCommand.ExecuteScalarAsync();
				}
			}
		}

		/// <summary>
		/// Asynchronously updates the specified slot.
		/// </summary>
		/// <param name="slot">The slot to update.</param>
		/// <returns>A task that represents the asynchronous update operation.</returns>
		/// <exception cref="Exception">When slot not found or is overlapping with another slot.</exception>
		public async Task UpdateAsync(Slot slot)
		{
			using (var connection = new SqlConnection(this.connectionString))
			{
				await connection.OpenAsync();

				string checkOverlapQuery = @"
                    SELECT COUNT(1) 
                    FROM Slots 
                    WHERE recruiter_id = @recruiter_id 
					  AND id <> @id
                      AND @start_time < end_time 
                      AND @end_time > start_time";

				using (var checkCommand = new SqlCommand(checkOverlapQuery, connection))
				{
					checkCommand.Parameters.AddWithValue("@id", slot.Id);
					checkCommand.Parameters.AddWithValue("@recruiter_id", slot.RecruiterId);
					checkCommand.Parameters.AddWithValue("@start_time", slot.StartTime);
					checkCommand.Parameters.AddWithValue("@end_time", slot.EndTime);

					int overlapCount = (int)await checkCommand.ExecuteScalarAsync();
					if (overlapCount > 0)
					{
						throw new Exception("Slot overlaps with an existing appointment!");
					}
				}

				string query = @"
					UPDATE Slots 
					SET start_time = @start_time, end_time = @end_time 
					WHERE id = @id";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@id", slot.Id);
					command.Parameters.AddWithValue("@start_time", slot.StartTime);
					command.Parameters.AddWithValue("@end_time", slot.EndTime);

					int rowsAffected = await command.ExecuteNonQueryAsync();

					if (rowsAffected == 0)
					{
						throw new Exception("Slot not found");
					}
				}
			}
		}

		/// <summary>
		/// Asynchronously deletes the slot with the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the slot to delete.</param>
		/// <returns>A task that represents the asynchronous delete operation.</returns>
		public async Task DeleteAsync(int id)
		{
			string query = "DELETE FROM Slots WHERE id = @id";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@id", id);

				await connection.OpenAsync();
				await command.ExecuteNonQueryAsync();
			}
		}

		/// <summary>
		/// Retrieves available slots for a specified recruiter on a given date.
		/// </summary>
		/// <param name="recruiterId">The unique identifier of the recruiter.</param>
		/// <param name="date">The date for which to retrieve slots.</param>
		/// <returns>A list of available slots for the recruiter on the specified date.</returns>
		public List<Slot> GetSlots(int recruiterId, DateTime date)
		{
			var slots = new List<Slot>();
			string query = @"
                SELECT id, recruiter_id, start_time, end_time 
                FROM Slots 
                WHERE recruiter_id = @recruiter_id AND CAST(start_time AS DATE) = CAST(@date AS DATE)
                ORDER BY start_time";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@recruiter_id", recruiterId);
				command.Parameters.AddWithValue("@date", date);

				connection.Open();
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						slots.Add(this.MapSlot(reader));
					}
				}
			}

			return slots;
		}

		/// <summary>
		/// Retrieves all slots associated with the specified recruiter.
		/// </summary>
		/// <param name="recruiterId">The unique identifier of the recruiter.</param>
		/// <returns>A list of slots belonging to the recruiter.</returns>
		public List<Slot> GetAllSlots(int recruiterId)
		{
			var slots = new List<Slot>();
			string query = @"
                SELECT id, recruiter_id, start_time, end_time, duration, status, interview_type
                FROM Slots 
                WHERE recruiter_id = @recruiter_id
                ORDER BY start_time";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@recruiter_id", recruiterId);

				connection.Open();
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						slots.Add(this.MapSlot(reader));
					}
				}
			}

			return slots;
		}

		/// <summary>
		/// Retrieves a slot with the specified identifier.
		/// </summary>
		/// <param name="id">The unique identifier of the slot.</param>
		/// <returns>The slot with the specified identifier, or null if not found.</returns>
		public Slot? GetById(int id)
		{
			string query = "SELECT id, recruiter_id, start_time, end_time FROM Slots WHERE id = @id";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@id", id);

				connection.Open();
				using (var reader = command.ExecuteReader())
				{
					if (reader.Read())
					{
						return this.MapSlot(reader);
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Adds the specified slot to the collection.
		/// </summary>
		/// <param name="slot">The slot to add.</param>
		/// <exception cref="Exception">When slot is overlapping with an existing slot.</exception>
		public void Add(Slot slot)
		{
			using (var connection = new SqlConnection(this.connectionString))
			{
				connection.Open();

				string checkOverlapQuery = @"
                    SELECT COUNT(1) 
                    FROM Slots 
                    WHERE recruiter_id = @recruiter_id 
                      AND CAST(start_time AS DATE) = CAST(@start_time AS DATE)
                      AND @start_time < end_time 
                      AND @end_time > start_time";

				using (var checkCommand = new SqlCommand(checkOverlapQuery, connection))
				{
					checkCommand.Parameters.AddWithValue("@recruiter_id", slot.RecruiterId);
					checkCommand.Parameters.AddWithValue("@start_time", slot.StartTime);
					checkCommand.Parameters.AddWithValue("@end_time", slot.EndTime);

					int overlapCount = (int)checkCommand.ExecuteScalar();
					if (overlapCount > 0)
					{
						throw new Exception("Slot overlaps with an existing appointment!");
					}
				}

				string insertQuery = @"
                    INSERT INTO Slots (recruiter_id, start_time, end_time, status, duration, interview_type) 
                    OUTPUT INSERTED.id 
                    VALUES (@recruiter_id, @start_time, @end_time, @status, @duration, @interview_type)";

				using (var insertCommand = new SqlCommand(insertQuery, connection))
				{
					insertCommand.Parameters.AddWithValue("@recruiter_id", slot.RecruiterId);
					insertCommand.Parameters.AddWithValue("@start_time", slot.StartTime);
					insertCommand.Parameters.AddWithValue("@end_time", slot.EndTime);
					insertCommand.Parameters.AddWithValue("@status", slot.Status);
					insertCommand.Parameters.AddWithValue("@duration", slot.Duration);
					insertCommand.Parameters.AddWithValue("@interview_type", slot.InterviewType);

					slot.Id = (int)insertCommand.ExecuteScalar();
				}
			}
		}

		/// <summary>
		/// Updates the specified slot with new values.
		/// </summary>
		/// <param name="slot">The slot to update.</param>
		/// <exception cref="Exception">When slot is not found.</exception>
		public void Update(Slot slot)
		{
			string query = @"
                UPDATE Slots 
                SET start_time = @start_time, end_time = @end_time, recruiter_id = @recruiter_id, duration = @duration, status = @status, interview_type = @interview_type
                WHERE id = @id";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@id", slot.Id);
				command.Parameters.AddWithValue("@start_time", slot.StartTime);
				command.Parameters.AddWithValue("@end_time", slot.EndTime);
				command.Parameters.AddWithValue("@recruiter_id", slot.RecruiterId);
				command.Parameters.AddWithValue("@duration", slot.Duration);
				command.Parameters.AddWithValue("@status", slot.Status);
				command.Parameters.AddWithValue("@interview_type", slot.InterviewType);

				connection.Open();
				int rowsAffected = command.ExecuteNonQuery();

				if (rowsAffected == 0)
				{
					throw new Exception("Slot not found");
				}
			}
		}

		/// <summary>
		/// Deletes the slot with the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the slot to delete.</param>
		public void Delete(int id)
		{
			string query = "DELETE FROM Slots WHERE id = @id";

			using (var connection = new SqlConnection(this.connectionString))
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@id", id);

				connection.Open();
				command.ExecuteNonQuery();
			}
		}

		private Slot MapSlot(SqlDataReader reader)
		{
			return new Slot
			{
				Id = reader.GetInt32(reader.GetOrdinal("id")),
				RecruiterId = reader.GetInt32(reader.GetOrdinal("recruiter_id")),
				StartTime = reader.GetDateTime(reader.GetOrdinal("start_time")),
				EndTime = reader.GetDateTime(reader.GetOrdinal("end_time")),
				Duration = reader.GetInt32(reader.GetOrdinal("duration")),
				Status = (SlotStatus)reader.GetInt32(reader.GetOrdinal("status")),
				InterviewType = reader.GetString(reader.GetOrdinal("interview_type")),
			};
		}
	}
}