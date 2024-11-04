using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ITaskRepository
    {
        Team GetTeamById(int teamId);
        IEnumerable<ToDo> GetToDosByTeam(int teamId);
        void AddToDo(int teamID, ToDo todo);
        void UpdateToDo(ToDo todo);
        void DeleteToDo(int todoId);
        ToDo GetToDoById(int teamId, int todoId);
        IEnumerable<ToDo> GetToDoByTitleAsync(string title, int teamId);
        bool IsTaskCompleted(int todoId);
    }
}
