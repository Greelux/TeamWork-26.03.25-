using System.Linq;
using TeamWork_26._03._25_.DTOs;
using TeamWork_26._03._25_.Models;
using TeamWork_26._03._25_.Services;

namespace TeamWork_26._03._25_.Controllers
{
    public class AuthController
    {
        private readonly UserService _userService = new UserService();

        public string Register(RegisterDto dto)
        {
            var existingUser = _userService.Login(dto.Username, dto.Password);
            if (existingUser != null)
                return "Username вже зайнятий";

            var newUser = new User
            {
                Username = dto.Username,
                Password = dto.Password,
                Role = dto.Role
            };

            _userService.RegisterUser(newUser);
            return "Реєстрація успішна!";
        }

        public string Login(LoginDto dto)
        {
            var user = _userService.Login(dto.Username, dto.Password);
            return user == null ? "Помилка входу" : $"Привіт, {user.Username} ({user.Role})";
        }

        public User GetUserByUsername(string username)
        {
            var all = _userService.GetUsersByRole("Patient")
                        .Concat(_userService.GetUsersByRole("Doctor"));
            return all.FirstOrDefault(u => u.Username == username);
        }
    }
}
