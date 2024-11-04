using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;

        public ToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository ?? throw new ArgumentNullException(nameof(toDoRepository));
        }

        public IEnumerable<ToDo> GetToDosForTeam(int teamId)
        {
            var team = _toDoRepository.GetTeamById(teamId);
            if (team == null)
            {
                throw new Exception("Team not found");
            }
            return _toDoRepository.GetToDosByTeam(teamId);
        }

        public IEnumerable<ToDo> GetDeletedTodos(int teamId)
        {
            return _toDoRepository.GetDeletedTodos(teamId);
        }

        public void RestoreToDo(int todoId, int teamId)
        {
            var todo = _toDoRepository.GetToDoDeletedById(teamId, todoId);
            if (todo == null)
            {
                throw new Exception("ToDo not found");
            }
            _toDoRepository.RestoreToDo(todoId, teamId);
        }

        public void PermanentlyDeleteTodo(int teamId, int todoId)
        {
            var todo = _toDoRepository.GetToDoById(teamId, todoId);
            if (todo == null)
            {
                throw new Exception("ToDo not found");
            }
            _toDoRepository.PermanentlyDeleteToDo(todoId);
        }

        public void AddToDoForTeam(int teamId, ToDo todo)
        {
            if (todo == null) throw new ArgumentNullException(nameof(todo));
            var team = _toDoRepository.GetTeamById(teamId);
            if (team == null)
            {
                throw new Exception("Team not found");
            }
            todo.TeamId = teamId;
            todo.Team = team;
            _toDoRepository.AddToDo(todo);
        }

        public void UpdateToDoForTeam(int teamId, ToDo todo)
        {
            if (todo == null) throw new ArgumentNullException(nameof(todo));
            var existingTodo = _toDoRepository.GetToDoById(teamId, todo.Id);
            if (existingTodo == null)
            {
                throw new Exception("ToDo not found");
            }
            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.DueDate = todo.DueDate;
            existingTodo.IsCompleted = todo.IsCompleted;
            _toDoRepository.UpdateToDo(existingTodo);
        }

        public void DeleteToDoForTeam(int teamId, int todoId)
        {
            var todo = _toDoRepository.GetToDoById(teamId, todoId);
            if (todo == null)
            {
                throw new Exception("ToDo not found");
            }
            _toDoRepository.DeleteToDo(todoId);
        }

        public ToDo GetToDoDetails(int teamId, int todoId)
        {
            var todo = _toDoRepository.GetToDoById(teamId, todoId);
            if (todo == null)
            {
                throw new Exception("ToDo not found");
            }
            return todo;
        }

        public async Task<IEnumerable<ToDo>> GetToDoByTitleAsync(string title, int teamId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            return await _toDoRepository.GetToDoByTitleAsync(title, teamId);
        }

        public async Task<bool> IsTaskCompleted(int todoId)
        {
            return await _toDoRepository.IsTaskCompleted(todoId);
        }

        public IEnumerable<ToDo> GetToDosByTeamId(int teamId)
        {
            return _toDoRepository.GetToDosByTeam(teamId);
        }
        public void UpdateTaskCompletionStatus(int teamId, int todoId, bool isCompleted)
        {
            var existingTodo = _toDoRepository.GetToDoById(teamId, todoId);
            if (existingTodo == null)
            {
                throw new Exception("ToDo not found");
            }

            // Chỉ cập nhật trạng thái IsCompleted
            existingTodo.IsCompleted = isCompleted;

            // Lưu thay đổi
            _toDoRepository.UpdateToDo(existingTodo);
        }

    }
}
