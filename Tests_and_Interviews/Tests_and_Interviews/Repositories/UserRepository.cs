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
    /// UserRepository class provides methods to perform CRUD operations on the Users table in the database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// Constructor for UserRepository that initializes the connection string from the environment variable.
        /// This allows for flexible configuration of the database connection without hardcoding sensitive
        /// information in the codebase.
        /// </summary>
        public UserRepository()
        {
            this.connectionString = Env.CONNECTION_STRING;
        }

        /// <summary>
        /// Gets a user by their unique identifier asynchronously. This method executes a SQL query to retrieve the user's details
        /// </summary>
        /// <param name="id">The id of the user to be found.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            string query = "SELECT id, name, email, cv_xml FROM Users WHERE id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return this.MapUser(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all users from the database asynchronously.
        /// This method executes a SQL query to retrieve all user records and maps them to a list of User objects.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();
            string query = "SELECT id, name, email, cv_xml FROM Users";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(this.MapUser(reader));
                    }
                }
            }

            return users;
        }

        /// <summary>
        /// Asynchronously adds a new user to the database.
        /// This method executes an INSERT SQL query to add a new user record and retrieves the generated ID for the newly inserted user.
        /// </summary>
        /// <param name="user">The user object containing the details of the user to be added.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task AddAsync(User user)
        {
            string query = @"
                INSERT INTO Users (name, email, cv_xml) 
                OUTPUT INSERTED.id 
                VALUES (@name, @email, @cv_xml)";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", user.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cv_xml", user.CvXml ?? (object)DBNull.Value);

                await connection.OpenAsync();
                user.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        /// <summary>
        /// Asynchronously updates an existing user's details in the database.
        /// This method executes an UPDATE SQL query to modify the user's name and email based on their unique identifier (ID).
        /// </summary>
        /// <param name="user">The user object containing the details of the upated user</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UpdateAsync(User user)
        {
            string query = @"
                UPDATE Users 
                SET name = @name, email = @email, cv_xml = @cv_xml
                WHERE id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@name", user.Name ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cv_xml", user.CvXml ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Asynchronously deletes a user from the database based on their unique identifier (ID).
        /// </summary>
        /// <param name="id">The id of the user to be deleted</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task DeleteAsync(int id)
        {
            string query = "DELETE FROM Users WHERE id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Maps a SqlDataReader to a User object.
        /// This method reads the values from the SqlDataReader and constructs a User object with the corresponding properties.
        /// </summary> 
        /// <param name="reader">The SqlDataReader containing the user data retrieved from the database.</param>
        /// <returns>A User object populated with the data from the SqlDataReader.</returns>
        private User MapUser(SqlDataReader reader)
        {
            return new User(
                reader.GetInt32(reader.GetOrdinal("id")),
                reader.IsDBNull(reader.GetOrdinal("name")) ? string.Empty : reader.GetString(reader.GetOrdinal("name")),
                reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                reader.IsDBNull(reader.GetOrdinal("cv_xml")) ? null : reader.GetString(reader.GetOrdinal("cv_xml"))
            );
        }
    }
}