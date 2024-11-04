using System.Collections.Generic;

namespace BusinessObjects
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Để lưu hash mật khẩu
        public bool isAdmin { get; set; }
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<ToDo> ToDos { get; set; } = new List<ToDo>();

        // Optional: Add properties for full name, date of birth, and phone if needed
        public string FullName { get; set; }
        public DateTime? DayOfBirth { get; set; } // Nullable to allow for no input
        public string Phone { get; set; }
        public int Role { get; set; } // Assuming role is represented as an int (0 for user, 1 for admin)
        public static User CurrentUser { get; set; }
    }
}
