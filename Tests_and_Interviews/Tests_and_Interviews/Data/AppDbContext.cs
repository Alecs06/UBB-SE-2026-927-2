namespace Tests_and_Interviews.Data
{
    using Microsoft.EntityFrameworkCore;
    using Tests_and_Interviews.Helpers;
    using Tests_and_Interviews.Models.Core;

    /// <summary>
    /// DbContext Object.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        public AppDbContext()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets tests.
        /// </summary>
        public DbSet<Test> Tests { get; set; }

        /// <summary>
        /// Gets or sets test attempts.
        /// </summary>
        public DbSet<TestAttempt> TestAttempts { get; set; }

        /// <summary>
        /// Gets or sets questions.
        /// </summary>
        public DbSet<Question> Questions { get; set; }

        /// <summary>
        /// Gets or sets answers.
        /// </summary>
        public DbSet<Answer> Answers { get; set; }

        /// <summary>
        /// Gets or sets users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets leaderboard entries.
        /// </summary>
        public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }

        /// <summary>
        /// Configures the database connection and provider for the context.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Env.CONNECTION_STRING);
        }
    }
}
