using BusinessObjects;

namespace Repositories
{
    public interface ITeamRepository
    {
        User GetUserById(int userId);
        Team GetTeamById(int teamId);
        IEnumerable<User> GetUsersOutTeam(int teamId);
        IEnumerable<Team> GetAll();
        void CreateTeam(Team team);
        void UpdateTeam(Team team);
        void DeleteTeam(int teamId);
        Task<bool> IsAdminUser(int userId);
        Task UpdateTeamStatusAsync(int teamId, TeamStatus newStatus);
        Task<IEnumerable<User>> GetMembersByNameAsync(int teamId, string name);
        Task<IEnumerable<Team>> GetTeamByNameAsync(string name, int userIdadminUserId);
    }
}
