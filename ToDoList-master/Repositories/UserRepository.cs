using BusinessObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ToDoListContext _context;

        public UserRepository(ToDoListContext context)
        {
            _context = context;
        }

        public async Task<bool> GetUserByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> LoginUserAsync(string email, string hashPassword)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hashPassword);
        }


        public async Task<User> AddUserAsync(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public User GetUser(int userId)
        {
            return _context.Users
                .Include(u => u.Teams)
                .FirstOrDefault(u => u.UserId == userId);
        }
        public Task<User> GetUserWithTeamsAsync(int userId)
        {
            throw new NotImplementedException();
        }

     
        public List<Team> GetTeamsForUser(int userId)
        {
            return _context.Users
                .Where(user => user.UserId == userId)
                .SelectMany(user => user.Teams)
                .ToList();
        }
    }
}