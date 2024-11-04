using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TaskRepository : ITaskRepository
    {
        public void AddToDo(int teamID, ToDo todo) => ToDoDAO.AddToDoForTeam(teamID, todo);

        public void DeleteToDo(int todoId) => ToDoDAO.DeleteToDo(todoId);

        public Team GetTeamById(int teamId) => ToDoDAO.GetTeamById(teamId);

        public ToDo GetToDoById(int teamId, int todoId) => ToDoDAO.GetToDoById(teamId, todoId);

        public IEnumerable<ToDo> GetToDoByTitleAsync(string title, int teamId) => ToDoDAO.GetToDoByTitleAsync(title, teamId);
        
        public IEnumerable<ToDo> GetToDosByTeam(int teamId) => ToDoDAO.GetToDosByTeam(teamId);

        public bool IsTaskCompleted(int todoId) => ToDoDAO.IsTaskCompleted(todoId);

        public void UpdateToDo(ToDo todo) => ToDoDAO.UpdateToDo(todo);

    }
}
