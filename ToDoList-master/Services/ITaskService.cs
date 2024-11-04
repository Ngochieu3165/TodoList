using BusinessObjects;

namespace Services
{
    public interface ITaskService
    {
        Team GetTeamById(int teamId);
        IEnumerable<ToDo> GetToDosByTeam(int teamId);
        void UpdateToDoForTeam(int teamId, ToDo todo);
        void DeleteToDo(int todoId);
        ToDo GetToDoById(int teamId, int todoId);
        IEnumerable<ToDo> GetToDoByTitleAsync(string title, int teamId);
        bool IsTaskCompleted(int todoId);
        void AddToDoForTeam(int teamId, ToDo newToDo);
        void UpdateTaskCompletionStatus(int teamId, int todoId, bool v);
    }
}