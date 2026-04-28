// <copyright file="InterviewSessionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Tests_and_Interviews.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Data.SqlClient;
    using Tests_and_Interviews.Helpers;
    using Tests_and_Interviews.Models.Core;
    using Tests_and_Interviews.Models.Enums;
    using Tests_and_Interviews.Repositories.Interfaces;

    /// <summary>
    /// Repository for managing interview sessions.
    /// </summary>
    public class InterviewSessionRepository : IInterviewSessionRepository
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterviewSessionRepository"/> class using the connection string defined in.
        /// the environment settings.
        /// </summary>
        public InterviewSessionRepository()
        {
            this.connectionString = Env.CONNECTION_STRING;
        }

        /// <inheritdoc/>
        public async Task<InterviewSession> GetInterviewSessionByIdAsync(int id)
        {
            string query = @"
                SELECT id, position_id, external_user_id, interviewer_id, date_start, video, status, score 
                FROM InterviewSessions 
                WHERE id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return this.MapInterviewSession(reader);
                    }
                }
            }

            throw new KeyNotFoundException($"InterviewSession with ID {id} was not found.");
        }

        /// <inheritdoc/>
        public InterviewSession GetInterviewSessionById(int id)
        {
            string query = @"
                SELECT id, position_id, external_user_id, interviewer_id, date_start, video, status, score 
                FROM InterviewSessions 
                WHERE id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return this.MapInterviewSession(reader);
                    }
                }
            }

            throw new KeyNotFoundException($"InterviewSession with ID {id} was not found.");
        }

        /// <inheritdoc/>
        public async Task UpdateInterviewSessionAsync(InterviewSession updated)
        {
            string query = @"
                UPDATE InterviewSessions 
                SET interviewer_id = @interviewer_id, 
                    position_id = @position_id, 
                    external_user_id = @external_user_id, 
                    status = @status, 
                    date_start = @date_start, 
                    video = @video, 
                    score = @score 
                WHERE id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", updated.Id);
                command.Parameters.AddWithValue("@interviewer_id", updated.InterviewerId);
                command.Parameters.AddWithValue("@position_id", updated.PositionId);
                command.Parameters.AddWithValue("@external_user_id", (object?)updated.ExternalUserId ?? DBNull.Value);
                command.Parameters.AddWithValue("@status", updated.Status ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@date_start", updated.DateStart);
                command.Parameters.AddWithValue("@video", updated.Video ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@score", updated.Score);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <inheritdoc/>
        public void Add(InterviewSession session)
        {
            string query = @"
                INSERT INTO InterviewSessions (position_id, external_user_id, interviewer_id, date_start, video, status, score)
                VALUES (@position_id, @external_user_id, @interviewer_id, @date_start, @video, @status, @score)";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@position_id", session.PositionId);
                int? externalUserId = session.ExternalUserId;
                command.Parameters.AddWithValue("@external_user_id", (object?)externalUserId ?? DBNull.Value);
                command.Parameters.AddWithValue("@interviewer_id", session.InterviewerId);
                command.Parameters.AddWithValue("@date_start", session.DateStart);
                command.Parameters.AddWithValue("@video", session.Video ?? (object?)DBNull.Value);
                command.Parameters.AddWithValue("@status", session.Status ?? (object?)DBNull.Value);
                command.Parameters.AddWithValue("@score", session.Score);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <inheritdoc/>
        public void Delete(InterviewSession session)
        {
            string query = @"
                DELETE FROM InterviewSessions
                WHERE id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", session.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <inheritdoc/>
        public async Task<List<InterviewSession>> GetScheduledSessionsAsync()
        {
            var sessions = new List<InterviewSession>();
            string query = @"
                SELECT id, position_id, external_user_id, interviewer_id, date_start, video, status, score 
                FROM InterviewSessions 
                WHERE status = @status";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@status", InterviewStatus.Scheduled.ToString());

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        sessions.Add(this.MapInterviewSession(reader));
                    }
                }
            }

            return sessions;
        }

        /// <inheritdoc/>
        public async Task<List<InterviewSession>> GetSessionsByStatusAsync(string status)
        {
            var sessions = new List<InterviewSession>();
            string query = @"
                SELECT id, position_id, external_user_id, interviewer_id, date_start, video, status, score 
                FROM InterviewSessions 
                WHERE status = @status
                ORDER BY date_start DESC";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@status", status);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        sessions.Add(this.MapInterviewSession(reader));
                    }
                }
            }

            return sessions;
        }

        /// <summary>
        /// Maps the current row of a SqlDataReader to a new InterviewSession instance.
        /// </summary>
        /// <remarks>This method expects the reader to include columns for all InterviewSession
        /// properties. Nullable fields are set to null if the corresponding database value is DBNull.</remarks>
        /// <param name="reader">The SqlDataReader positioned at the row to map. Must not be null and must contain the expected columns.</param>
        /// <returns>An InterviewSession object populated with values from the current row of the reader.</returns>
        private InterviewSession MapInterviewSession(SqlDataReader reader)
        {
            return new InterviewSession
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                PositionId = reader.GetInt32(reader.GetOrdinal("position_id")),
                ExternalUserId = reader.IsDBNull(reader.GetOrdinal("external_user_id")) ? null : reader.GetInt32(reader.GetOrdinal("external_user_id")),
                InterviewerId = reader.GetInt32(reader.GetOrdinal("interviewer_id")),
                DateStart = reader.GetDateTime(reader.GetOrdinal("date_start")),
                Video = reader.IsDBNull(reader.GetOrdinal("video")) ? null : reader.GetString(reader.GetOrdinal("video")),
                Status = reader.IsDBNull(reader.GetOrdinal("status")) ? null : reader.GetString(reader.GetOrdinal("status")),
                Score = reader.IsDBNull(reader.GetOrdinal("score")) ? 0 : reader.GetDecimal(reader.GetOrdinal("score")),
            };
        }
    }
}