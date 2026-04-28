// <copyright file="TestRepository.cs" company="PlaceholderCompany">
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
    /// TestRepository class provides methods to perform CRUD operations on the Tests and Questions tables in the database.
    /// </summary>
    public class TestRepository : ITestRepository
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRepository"/> class.
        /// </summary>
        public TestRepository()
        {
            this.connectionString = Env.CONNECTION_STRING;
        }

        /// <summary>
        /// Asynchronously finds a test by its ID, including its associated questions.
        /// </summary>
        /// <param name="id">The ID of the test to find.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Test?> FindByIdAsync(int id)
        {
            Test? test = null;

            string query = @"
                SELECT 
                    t.id AS t_id, t.title, t.category, t.created_at,
                    q.id AS q_id, q.position_id, q.test_id, q.question_text, 
                    q.question_type_string, q.question_score, q.question_answer, q.options_json
                FROM Tests t
                LEFT JOIN Questions q ON t.id = q.test_id
                WHERE t.id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (test == null)
                        {
                            test = new Test
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("t_id")),
                                Title = reader.IsDBNull(reader.GetOrdinal("title")) ? null : reader.GetString(reader.GetOrdinal("title")),
                                Category = reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                Questions = [],
                            };
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("q_id")))
                        {
                            test.Questions.Add(new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("q_id")),
                                PositionId = reader.IsDBNull(reader.GetOrdinal("position_id")) ? null : reader.GetInt32(reader.GetOrdinal("position_id")),
                                TestId = reader.IsDBNull(reader.GetOrdinal("test_id")) ? null : reader.GetInt32(reader.GetOrdinal("test_id")),
                                QuestionText = reader.IsDBNull(reader.GetOrdinal("question_text")) ? null : reader.GetString(reader.GetOrdinal("question_text")),
                                QuestionTypeString = reader.IsDBNull(reader.GetOrdinal("question_type_string")) ? null : reader.GetString(reader.GetOrdinal("question_type_string")),
                                QuestionScore = reader.GetFloat(reader.GetOrdinal("question_score")),
                                QuestionAnswer = reader.IsDBNull(reader.GetOrdinal("question_answer")) ? null : reader.GetString(reader.GetOrdinal("question_answer")),
                                OptionsJson = reader.IsDBNull(reader.GetOrdinal("options_json")) ? null : reader.GetString(reader.GetOrdinal("options_json")),
                            });
                        }
                    }
                }
            }

            return test;
        }

        /// <summary>
        /// Asynchronously finds tests by their category, including their associated questions.
        /// </summary>
        /// <param name="category">The category of the tests to find.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<Test>> FindTestsByCategoryAsync(string category)
        {
            var testDictionary = new Dictionary<int, Test>();

            string query = @"
                SELECT 
                    t.id AS t_id, t.title, t.category, t.created_at,
                    q.id AS q_id, q.position_id, q.test_id, q.question_text, 
                    q.question_type_string, q.question_score, q.question_answer, q.options_json
                FROM Tests t
                LEFT JOIN Questions q ON t.id = q.test_id
                WHERE t.category = @category";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@category", category);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int testId = reader.GetInt32(reader.GetOrdinal("t_id"));

                        if (!testDictionary.TryGetValue(testId, out var test))
                        {
                            test = new Test
                            {
                                Id = testId,
                                Title = reader.IsDBNull(reader.GetOrdinal("title")) ? null : reader.GetString(reader.GetOrdinal("title")),
                                Category = reader.IsDBNull(reader.GetOrdinal("category")) ? null : reader.GetString(reader.GetOrdinal("category")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                Questions = [],
                            };
                            testDictionary.Add(testId, test);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("q_id")))
                        {
                            test.Questions.Add(new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("q_id")),
                                PositionId = reader.IsDBNull(reader.GetOrdinal("position_id")) ? null : reader.GetInt32(reader.GetOrdinal("position_id")),
                                TestId = reader.IsDBNull(reader.GetOrdinal("test_id")) ? null : reader.GetInt32(reader.GetOrdinal("test_id")),
                                QuestionText = reader.IsDBNull(reader.GetOrdinal("question_text")) ? null : reader.GetString(reader.GetOrdinal("question_text")),
                                QuestionTypeString = reader.IsDBNull(reader.GetOrdinal("question_type_string")) ? null : reader.GetString(reader.GetOrdinal("question_type_string")),
                                QuestionScore = reader.GetFloat(reader.GetOrdinal("question_score")),
                                QuestionAnswer = reader.IsDBNull(reader.GetOrdinal("question_answer")) ? null : reader.GetString(reader.GetOrdinal("question_answer")),
                                OptionsJson = reader.IsDBNull(reader.GetOrdinal("options_json")) ? null : reader.GetString(reader.GetOrdinal("options_json")),
                            });
                        }
                    }
                }
            }

            return [.. testDictionary.Values];
        }
    }
}