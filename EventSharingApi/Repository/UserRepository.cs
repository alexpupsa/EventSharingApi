using EventSharingApi.Context;
using EventSharingApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSharingApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Register(string username)
        {
            var userExists = await _dbContext.Users.AnyAsync(x => x.Username == username);
            if (userExists)
            {
                return false;
            }
            await _dbContext.AddAsync(new User
            {
                Username = username
            });
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
