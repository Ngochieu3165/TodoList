using BusinessObjects;

namespace Repositories
{
    public interface IToDoRepository
    {
        Team GetTeamById(int teamId);
        IEnumerable<ToDo> GetToDosByTeam(int teamId);
        void AddToDo(ToDo todo);
        void UpdateToDo(ToDo todo);
        void DeleteToDo(int todoId);
        ToDo GetToDoById(int teamId, int todoId);
        Task<IEnumerable<ToDo>> GetToDoByTitleAsync(string title, int teamId);
        Task<bool> IsTaskCompleted(int todoId);
        IEnumerable<ToDo> GetDeletedTodos(int teamId);
        void RestoreToDo(int todoId, int teamId);
        void PermanentlyDeleteToDo(int id);
        ToDo GetToDoDeletedById(int teamId, int todoId);
    }
}
