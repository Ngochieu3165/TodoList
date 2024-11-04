using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
namespace Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ToDoListContext _context;

        public TeamRepository(ToDoListContext context)
        {
            _context = context;
        }
     
        public User GetUserById(int userId)
        {
           return _context.Users.FirstOrDefault(t => t.UserId == userId);
        }
        public Team GetTeamById(int teamId)
        {
           return _context.Teams.Include(t => t.Members).FirstOrDefault(t => t.TeamId == teamId);
        }
        public IEnumerable<Team> GetAll()
        {
            return _context.Teams.Include(t => t.Members).ToList();
        }

        public IEnumerable<User> GetUsersOutTeam(int teamId)
        {
            var usersInTeam = _context.Teams
                .Where(t => t.TeamId == teamId)
                .SelectMany(t => t.Members) 
                .ToList();
            var team = _context.Teams.FirstOrDefault(t => t.TeamId == teamId);
            var allUsers = _context.Users.ToList();
            var usersOutTeam = allUsers.Where(u => !usersInTeam.Any(member => member.UserId == u.UserId) && u.UserId != team.AdminUserId);
            return usersOutTeam;
        }

        public void CreateTeam(Team team)
         {
            _context.Teams.Add(team);
            _context.SaveChanges();
         }
        public void UpdateTeam(Team team)
        {
            _context.Teams.Update(team);
            _context.SaveChanges();
        }
        public void DeleteTeam(int teamId)
        {
            var team = _context.Teams.FirstOrDefault(t => t.TeamId == teamId);
            if (team != null)
            {
                team.DeletedAt = DateTime.Now;
                _context.Teams.Update(team);
                _context.SaveChanges();
            }
        }
        public async Task UpdateTeamStatusAsync(int teamId, TeamStatus newStatus)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.TeamId == teamId);
            if (team != null)
            {
                team.Status = newStatus;
                _context.Teams.Update(team);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> IsAdminUser(int userId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.AdminUserId == userId);
            if (team == null)
            {
                throw new Exception("Team not found");
            }
            return team.AdminUser.isAdmin;
        }
       
        public async Task<IEnumerable<User>> GetMembersByNameAsync(int teamId, string name)
        {
            var team = await _context.Teams.Include(t => t.Members).FirstOrDefaultAsync(t => t.TeamId == teamId);
            return team.Members.Where(u => u.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
        public async Task<IEnumerable<Team>> GetTeamByNameAsync(string name, int adminUserId)
        {
            return await _context.Teams
                .Where(t => t.Name.Contains(name) && t.DeletedAt == null && t.AdminUserId == adminUserId)
                .ToListAsync();
        }
    }
}

