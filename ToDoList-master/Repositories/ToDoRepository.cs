using BusinessObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly ToDoListContext _context;

        public ToDoRepository(ToDoListContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<ToDo> GetDeletedTodos(int teamId)
        {
            return _context.ToDos
                .Where(t => t.IsDeleted && t.DeletedAt >= DateTime.Now.AddDays(-7)
                              && t.TeamId == teamId)
                .ToList();
        }

        public ToDo GetToDoById(int id)
        {
            return _context.ToDos.Find(id);
        }

        public void RestoreToDo(int todoId, int teamId)
        {
            var todo = GetToDoDeletedById(teamId, todoId);

            if (todo != null)
            {
                todo.IsDeleted = false;
                todo.DeletedAt = null;
                _context.SaveChanges();
            }
        }

        public void PermanentlyDeleteToDo(int todoId)
        {
            var todo = GetToDoById(todoId);
            if (todo != null)
            {
                _context.ToDos.Remove(todo);
                _context.SaveChanges();
            }
        }

        public Team GetTeamById(int teamId)
        {
            return _context.Teams
                 .Include(t => t.ToDos) 
                 .FirstOrDefault(t => t.TeamId == teamId);
        }

        public IEnumerable<ToDo> GetToDosByTeam(int teamId)
        {
            return _context.ToDos
                .Where(t => t.TeamId == teamId && t.DeletedAt == null)
                .ToList();
        }

        public void AddToDo(ToDo todo)
        {
            _context.ToDos.Add(todo);
            _context.SaveChanges();
        }

        public void UpdateToDo(ToDo todo)
        {
            _context.ToDos.Update(todo);
            _context.SaveChanges();
        }

        public void DeleteToDo(int todoId)
        {
            var todo = _context.ToDos.FirstOrDefault(t => t.Id == todoId);
            if (todo != null)
            {
                todo.DeletedAt = DateTime.Now;
                todo.IsDeleted = true;
                _context.ToDos.Update(todo);
                _context.SaveChanges();
            }
        }
        public ToDo GetToDoDeletedById(int teamId, int todoId)
        {
            return _context.ToDos
                .FirstOrDefault(t => t.TeamId == teamId && t.Id == todoId && t.DeletedAt != null);
        }

        public ToDo GetToDoById(int teamId, int todoId)
        {
            return _context.ToDos
                .FirstOrDefault(t => t.TeamId == teamId && t.Id == todoId && t.DeletedAt == null);
        }

        public async Task<IEnumerable<ToDo>> GetToDoByTitleAsync(string title, int teamId)
        {
            return await _context.ToDos
                .Where(t => t.Title.Contains(title) && t.TeamId == teamId && t.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<bool> IsTaskCompleted(int todoId)
        {
            var todo = await _context.ToDos
                .FirstOrDefaultAsync(t => t.Id == todoId && t.DeletedAt == null);
            if (todo == null)
            {
                throw new Exception("Task not found");
            }
            return todo.IsCompleted;
        }
    }
}
