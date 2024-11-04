using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserService
    {
        Task<User> GetUserTeamsAsync(int userId);
        User GetUser(int userId);
        IEnumerable<Team> GetTeamsByUserId(int userId);
        User GetCurrentUser();
      
    }
}
