using HealthMeet.DTOs;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TeamWork_26._03._25_.Controllers;
using TeamWork_26._03._25_.DTOs;
using TeamWork_26._03._25_.Models;

class Program
{
    static User currentUser = null;
    static async Task Main()
    {
        var auth = new AuthController();

        while (currentUser == null)
        {
            Console.WriteLine("1. Реєстрація");
            Console.WriteLine("2. Вхід");
            Console.Write("Виберіть опцію: ");
            var input = Console.ReadLine();

            if (input == "1")
            {
                Console.Write("Ім'я користувача: ");
                var username = Console.ReadLine();

                Console.Write("Пароль: ");
                var password = Console.ReadLine();

                Console.Write("Роль (Patient / Doctor): ");
                var roleStr = Console.ReadLine();

                Enum.TryParse<UserRole>(roleStr, true, out var role);

                var result = auth.Register(new RegisterDto
                {
                    Username = username,
                    Password = password,
                    Role = role
                });

                Console.WriteLine(result);
            }
            else if (input == "2")
            {
                Console.Write("Ім'я користувача: ");
                var username = Console.ReadLine();

                Console.Write("Пароль: ");
                var password = Console.ReadLine();

                var result = auth.Login(new LoginDto { Username = username, Password = password });

                if (result.StartsWith("Привіт"))
                {
                    currentUser = auth.GetUserByUsername(username); // Отримаємо об'єкт User
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine(result);
                }
            }
        }
        var connectionString = "Server=DESKTOP-3OLQEIJ;Database=HealthMeet;Trusted_Connection=True;";
        var repo = new AppointmentRepository(connectionString);

        while (true)
        {
            Console.WriteLine("=== HealthMeet Appointments ===");
            Console.WriteLine("1. Створити прийом");
            Console.WriteLine("2. Скасувати прийом");
            Console.WriteLine("3. Завершити прийом");
            Console.WriteLine("4. Історія прийомів (JOIN)");
            Console.WriteLine("0. Вийти");
            Console.Write("Оберіть опцію: ");
            Console.WriteLine("30.06.2025");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": await Create(repo); break;
                case "2": await Cancel(repo); break;
                case "3": await Complete(repo); break;
                case "4": await ShowHistory(repo); break;
                case "0": return;
            }

            Console.WriteLine();
        }
    }
    public class JwtService
    {
        private readonly string _secretKey = "super_secret_key_1234567890";

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)); ////Створюється симетричний ключ
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); ///Формируем Ширфовку для токена, по гайду с ютуба)

            var token = new JwtSecurityToken(
                issuer: "HealthMeet", ///Указываем издателя токена
                audience: "HealthMeetUsers", ///Указываем приём токена
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
    static async Task Create(AppointmentRepository repo)
    {
        Console.Write("PatientId: ");
        int pid = int.Parse(Console.ReadLine());
        Console.Write("DoctorId: ");
        int did = int.Parse(Console.ReadLine());
        Console.Write("Дата (yyyy-MM-dd HH:mm): ");
        DateTime date = DateTime.Parse(Console.ReadLine());

        var dto = new AppointmentDto { PatientId = pid, DoctorId = did, ScheduledAt = date };
        var (ok, msg) = await repo.CreateAppointmentAsync(dto);
        Console.WriteLine(msg);
    }

    static async Task Cancel(AppointmentRepository repo)
    {
        Console.Write("AppointmentId: ");
        int id = int.Parse(Console.ReadLine());
        Console.WriteLine(await repo.CancelAppointmentAsync(id) ? "Скасовано" : "Не вдалося.");
    }

    static async Task Complete(AppointmentRepository repo)
    {
        Console.Write("AppointmentId: ");
        int id = int.Parse(Console.ReadLine());
        Console.WriteLine(await repo.CompleteAppointmentAsync(id) ? "Завершено" : "Не вдалося.");
    }

    static async Task ShowHistory(AppointmentRepository repo)
    {
        var history = await repo.GetAppointmentHistoryAsync();
        foreach (var h in history)
        {
            Console.WriteLine($"ID: {h.AppointmentId}");
            Console.WriteLine($"Пацієнт: {h.PatientUsername}");
            Console.WriteLine($"Лікар: {h.DoctorUsername}");
            Console.WriteLine($"Спеціальність: {h.SpecialtyName}");
            Console.WriteLine($"Час: {h.ScheduledAt}");
            Console.WriteLine($"Статус: {h.Status}");
            Console.WriteLine(new string('-', 30));
        }
    }
}