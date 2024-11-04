using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ToDoDAO
    {

        public static Team GetTeamById(int teamId)
        {
            var team = new Team();
            try
            {
                using var context = new ToDoListContext();
                team = context.Teams
                    .Include(t => t.ToDos)
                    .FirstOrDefault(t => t.TeamId == teamId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return team;

        }

        public static IEnumerable<ToDo> GetToDosByTeam(int teamId)
        {
            var listToDo = new List<ToDo>();
            try
            {
                using var context = new ToDoListContext();
                listToDo = context.ToDos
                    .Where(t => t.TeamId == teamId && t.DeletedAt == null)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return listToDo;
        }

        public static void AddToDoForTeam(int teamId, ToDo todo)
        {
            try
            {
                using var context = new ToDoListContext();
                todo.TeamId = teamId; // Set the TeamId property of the todo object
                context.ToDos.Add(todo);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void UpdateToDo(ToDo todo)
        {
            try
            {
                using var context = new ToDoListContext();
                context.ToDos.Update(todo);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void DeleteToDo(int todoId)
        {
            try
            {
                using var context = new ToDoListContext();
                var todo = context.ToDos.FirstOrDefault(t => t.Id == todoId);
                if (todo != null)
                {
                    todo.DeletedAt = DateTime.Now;
                    todo.IsDeleted = true;
                    context.ToDos.Update(todo);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public static ToDo GetToDoById(int teamId, int todoId)
        {
            var todo = new ToDo();
            try
            {
                using var context = new ToDoListContext();
                todo = context.ToDos
                    .FirstOrDefault(t => t.TeamId == teamId && t.Id == todoId && t.DeletedAt == null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return todo;
        }

        public static IEnumerable<ToDo> GetToDoByTitleAsync(string title, int teamId)
        {
            var listToDo = new List<ToDo>();
            try
            {
                using var context = new ToDoListContext();
                listToDo = context.ToDos
                    .Where(t => t.Title.Contains(title) && t.TeamId == teamId && t.DeletedAt == null)
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return listToDo;
        }

        public static bool IsTaskCompleted(int todoId)
        {
            var todo = new ToDo();
            try
            {
                using var context = new ToDoListContext();
                todo = context.ToDos
                    .FirstOrDefault(t => t.Id == todoId && t.DeletedAt == null);
                if (todo == null)
                {
                    throw new Exception("Task not found");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return todo.IsCompleted;
        }
    }
}
