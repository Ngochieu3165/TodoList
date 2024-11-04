using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ToDoListContext _context;
        private User _currentUser; // This could be set when the user logs in

        public UserService(User currentUser)
        {
            _currentUser = currentUser; // Set the current user
        }
        public UserService(ToDoListContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Khởi tạo context để truy cập dữ liệu
        }
        // UserService.cs
        public User GetCurrentUser()
        {
            return User.CurrentUser;  // Trả về người dùng hiện tại
        }


        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetUserTeamsAsync(int userId)
        {
            return await _userRepository.GetUserWithTeamsAsync(userId);
        }
        public User GetUser(int userId)
        {
            return _userRepository.GetUser(userId);
        }

        public IEnumerable<Team> GetTeamsByUserId(int userId)
        {
            var user = _context.Users
         .Include(u => u.Teams)
         .FirstOrDefault(u => u.UserId == userId);

            return user?.Teams ?? Enumerable.Empty<Team>();
        }
        

    }
}
