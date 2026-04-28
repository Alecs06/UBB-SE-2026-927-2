// <copyright file="AnswerRepository.cs" company="PlaceholderCompany">
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
    using Tests_and_Interviews.Repositories.Interfaces;

    /// <summary>
    /// AnswerRepository class provides methods to perform CRUD operations on the Answers table in the database.
    /// </summary>
    public class AnswerRepository : IAnswerRepository
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerRepository"/> class.
        /// </summary>
        public AnswerRepository()
        {
            this.connectionString = Env.CONNECTION_STRING;
        }

        /// <summary>
        /// Asynchronously saves an answer to the database. It inserts a new record into the Answers table with the provided answer details.
        /// </summary>
        /// <param name="answer">The <see cref="Answer"/> object containing the details of the answer to be saved.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SaveAsync(Answer answer)
        {
            string query = @"
                INSERT INTO Answers (attempt_id, question_id, value)
                VALUES (@attempt_id, @question_id, @value);";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@attempt_id", answer.AttemptId);
                command.Parameters.AddWithValue("@question_id", answer.QuestionId);
                command.Parameters.AddWithValue("@value", answer.Value ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of answers associated with a specific attempt ID from the database.
        /// It performs a JOIN operation between the Answers and Questions tables to fetch the relevant answer
        /// details along with the corresponding question information.
        /// </summary>
        /// <param name="attemptId">The ID of the attempt for which to retrieve the answers.</param>
        /// <returns>A <see cref="Task{List{Answer}}"/> representing the asynchronous operation, containing a list of <see cref="Answer"/> objects.</returns>
        public async Task<List<Answer>> FindByAttemptAsync(int attemptId)
        {
            var answers = new List<Answer>();

            string query = @"
                SELECT 
                    a.id AS answer_id, a.attempt_id, a.question_id, a.value,
                    q.id AS q_id, q.position_id, q.test_id, q.question_text, 
                    q.question_type_string, q.question_score, q.question_answer, q.options_json
                FROM Answers a
                INNER JOIN Questions q ON a.question_id = q.id
                WHERE a.attempt_id = @attempt_id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@attempt_id", attemptId);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var answer = new Answer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("answer_id")),
                            AttemptId = reader.GetInt32(reader.GetOrdinal("attempt_id")),
                            QuestionId = reader.GetInt32(reader.GetOrdinal("question_id")),
                            Value = reader.IsDBNull(reader.GetOrdinal("value")) ? null : reader.GetString(reader.GetOrdinal("value")),

                            Question = new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("q_id")),
                                PositionId = reader.IsDBNull(reader.GetOrdinal("position_id")) ? null : reader.GetInt32(reader.GetOrdinal("position_id")),
                                TestId = reader.IsDBNull(reader.GetOrdinal("test_id")) ? null : reader.GetInt32(reader.GetOrdinal("test_id")),
                                QuestionText = reader.GetString(reader.GetOrdinal("question_text")),
                                QuestionTypeString = reader.GetString(reader.GetOrdinal("question_type_string")),
                                QuestionScore = reader.GetFloat(reader.GetOrdinal("question_score")),
                                QuestionAnswer = reader.IsDBNull(reader.GetOrdinal("question_answer")) ? null : reader.GetString(reader.GetOrdinal("question_answer")),
                                OptionsJson = reader.IsDBNull(reader.GetOrdinal("options_json")) ? null : reader.GetString(reader.GetOrdinal("options_json")),
                            },
                        };
                        answers.Add(answer);
                    }
                }
            }

            return answers;
        }
    }
}