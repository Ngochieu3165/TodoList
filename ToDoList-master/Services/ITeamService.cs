using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITeamService
    {
        IEnumerable<User> GetUsersOutTeam(int teamId);
        User GetUserById(int teamId);
        Team GetTeamById(int teamId);
        IEnumerable<Team> GetAllTeams();
        void CreateTeam(Team team, int adminUserId);
        void UpdateTeam(Team team);
        void DeleteTeam(int teamId);
        void AddMemberInTeam(int teamId, User user);
        void RemoveMemberFromTeam(int teamId, int userId);
        Task UpdateTeamStatusAsync(int teamId, TeamStatus newStatus);
        Task<bool> IsAdminUserAsync(int userId);
        Task<IEnumerable<User>> GetUserByNameAsync(int teamId, string name);
        Task<IEnumerable<Team>> GetTeamByNameAsync(string name, int adminUserId);
    }
}
