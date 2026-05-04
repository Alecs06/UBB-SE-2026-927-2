namespace Tests_and_Interviews.Data
{
    using Microsoft.EntityFrameworkCore;
    using Tests_and_Interviews.Helpers;
    using Tests_and_Interviews.Models;
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

        public DbSet<Company> Companies { get; set; }
        public DbSet<JobPosting> Jobs { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Collaborator> Collaborators { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<InterviewSession> InterviewSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<JobSkill>()
                .HasKey(js => new { js.SkillId, js.JobId });

            modelBuilder.Entity<Collaborator>()
                .HasKey(c => new { c.EventId, c.CompanyId });

            modelBuilder.Entity<Applicant>()
                .HasOne(a => a.RecommendedFromCompany)
                .WithMany()
                .HasForeignKey(a => a.RecommendedFromCompanyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TestId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TestAttempt>()
                .HasOne(ta => ta.User)
                .WithMany()
                .HasForeignKey(ta => ta.ExternalUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<InterviewSession>()
                .HasOne(i => i.User)
                .WithMany()
                .HasForeignKey(i => i.ExternalUserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Slot>()
                .HasOne(s => s.Candidate)
                .WithMany()
                .HasForeignKey(s => s.CandidateId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Recruiter>()
                .HasOne(r => r.Company)
                .WithMany()
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.TestAttempt)
                .WithMany(ta => ta.Answers)
                .HasForeignKey(a => a.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Collaborator>()
                .HasKey(c => new { c.EventId, c.CompanyId });

            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Collaborators)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Collaborator>()
                .HasOne(c => c.Company)
                .WithMany(co => co.Collaborators)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
