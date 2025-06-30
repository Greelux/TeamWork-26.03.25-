using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamWork_26._03._25_.DTOs;
using TeamWork_26._03._25_.Models;

namespace TeamWork_26._03._25_.Controllers
{
    public class AuthController
    {
        private static List<User> users = new List<User>();

        public string Register(RegisterDto dto)
        {
            if (users.Any(u => u.Email == dto.Email))
                return "Email вже зайнятий:(";

            var user = new User
            {
                Id = users.Count + 1,
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                Role = dto.Role
            };

            users.Add(user);
            return "Реєстрація успішна!";
        }

        public string Login(LoginDto dto)
        {
            var user = users.FirstOrDefault(u => u.Email == dto.Email && u.PasswordHash == dto.Password);
            return user == null ? "Помилка входу" : $"Привіт, {user.FullName}";
        }
    }
}
