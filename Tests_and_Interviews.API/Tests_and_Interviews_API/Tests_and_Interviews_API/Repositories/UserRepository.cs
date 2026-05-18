namespace Tests_and_Interviews_API.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Tests_and_Interviews_API.Data;
    using Tests_and_Interviews_API.Models.Core;
    using Tests_and_Interviews_API.Repositories.Interfaces;

    /// <summary>
    /// UserRepository class provides methods to perform CRUD operations on the Users table in the database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext appDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        public UserRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <inheritdoc />
        public async Task<User?> GetByIdAsync(int id)
        {
            return await this.appDbContext.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <inheritdoc />
        public async Task<List<User>> GetAllAsync()
        {
            return await this.appDbContext.Users
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddAsync(User user)
        {
            this.appDbContext.Users.Add(user);
            await this.appDbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(User user)
        {
            var existing = await this.appDbContext.Users.FindAsync(user.Id);
            if (existing == null) return;

            existing.Name = user.Name;
            existing.Email = user.Email;
            existing.CvXml = user.CvXml;

            await this.appDbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int id)
        {
            var user = await this.appDbContext.Users.FindAsync(id);
            if (user != null)
            {
                this.appDbContext.Users.Remove(user);
                await this.appDbContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await this.appDbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}