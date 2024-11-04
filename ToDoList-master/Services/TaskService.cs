using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        public TaskService()
        {
            _taskRepository = new TaskRepository();
        }
        public void AddToDoForTeam(int teamId, ToDo newToDo)
        {
            var team = _taskRepository.GetTeamById(teamId);
            if (team == null)
            {
                throw new Exception("Team not found");
            }
            _taskRepository.AddToDo(teamId, newToDo);

        }
        public void UpdateToDoForTeam(int teamId, ToDo todo)
        {
            if (todo == null) throw new ArgumentNullException(nameof(todo));
            var existingTodo = _taskRepository.GetToDoById(teamId, todo.Id);
            if (existingTodo == null)
            {
                throw new Exception("ToDo not found");
            }
            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.DueDate = todo.DueDate;
            existingTodo.IsCompleted = todo.IsCompleted;
            _taskRepository.UpdateToDo(existingTodo);
        }
        public void DeleteToDo(int todoId) => _taskRepository.DeleteToDo(todoId);

        public Team GetTeamById(int teamId) => _taskRepository.GetTeamById(teamId);

        public ToDo GetToDoById(int teamId, int todoId) => _taskRepository.GetToDoById(teamId, todoId);

        public IEnumerable<ToDo> GetToDoByTitleAsync(string title, int teamId) => _taskRepository.GetToDoByTitleAsync(title, teamId);

        public IEnumerable<ToDo> GetToDosByTeam(int teamId) => _taskRepository.GetToDosByTeam(teamId);

        public bool IsTaskCompleted(int todoId) => _taskRepository.IsTaskCompleted(todoId);
        public void UpdateTaskCompletionStatus(int teamId, int todoId, bool isCompleted)
        {
            var existingTodo = _taskRepository.GetToDoById(teamId, todoId);
            if (existingTodo == null)
            {
                throw new Exception("ToDo not found");
            }

            // Chỉ cập nhật trạng thái IsCompleted
            existingTodo.IsCompleted = isCompleted;

            // Lưu thay đổi
            _taskRepository.UpdateToDo(existingTodo);
        }
    }
}
