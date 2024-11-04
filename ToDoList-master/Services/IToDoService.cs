using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IToDoService
    {
        IEnumerable<ToDo> GetToDosForTeam(int teamId);
        void AddToDoForTeam(int teamId, ToDo todo);
        void UpdateToDoForTeam(int teamId, ToDo todo);
        void DeleteToDoForTeam(int teamId, int todoId);
        ToDo GetToDoDetails(int teamId, int todoId);
        Task<IEnumerable<ToDo>> GetToDoByTitleAsync(string title, int teamId);
        Task<bool> IsTaskCompleted(int todoId);
        void UpdateTaskCompletionStatus(int teamId, int todoId, bool isCompleted);
        void RestoreToDo(int todoId, int teamId);
        IEnumerable<ToDo> GetToDosByTeamId(int teamId);
        IEnumerable<ToDo> GetDeletedTodos(int teamId); 
        void PermanentlyDeleteTodo(int teamId, int todoId);
    }
}
