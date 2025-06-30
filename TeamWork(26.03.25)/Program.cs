using HealthMeet.Data;
using HealthMeet.DTOs;
using HealthMeet.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = "Server=localhost;Database=HealthMeet;Trusted_Connection=True;";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        using var context = new ApplicationDbContext(options);
        var service = new AppointmentService(context);

        while (true)
        {
            Console.WriteLine("=== HealthMeet Appointments ===");
            Console.WriteLine("1. Створити прийом");
            Console.WriteLine("2. Скасувати прийом");
            Console.WriteLine("3. Завершити прийом");
            Console.WriteLine("4. Історія прийомів (JOIN Dapper)");
            Console.WriteLine("0. Вийти");
            Console.Write("Оберіть опцію: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": await CreateAppointment(service); break;
                case "2": await CancelAppointment(service); break;
                case "3": await CompleteAppointment(service); break;
                case "4": await ShowAppointmentHistory(connectionString); break;
                case "0": return;
            }

            Console.WriteLine();
        }
    }

    static async Task CreateAppointment(IAppointmentService service)
    {
        Console.Write("PatientId: ");
        int patientId = int.Parse(Console.ReadLine());

        Console.Write("DoctorId: ");
        int doctorId = int.Parse(Console.ReadLine());

        Console.Write("Дата і час (yyyy-MM-dd HH:mm): ");
        DateTime time = DateTime.Parse(Console.ReadLine());

        var dto = new AppointmentDto { PatientId = patientId, DoctorId = doctorId, ScheduledAt = time };
        var (ok, msg) = await service.CreateAppointmentAsync(dto);
        Console.WriteLine(msg);
    }

    static async Task CancelAppointment(IAppointmentService service)
    {
        Console.Write("AppointmentId: ");
        int id = int.Parse(Console.ReadLine());
        var ok = await service.CancelAppointmentAsync(id);
        Console.WriteLine(ok ? "Скасовано" : "Не знайдено або неможливо скасувати.");
    }

    static async Task CompleteAppointment(IAppointmentService service)
    {
        Console.Write("AppointmentId: ");
        int id = int.Parse(Console.ReadLine());
        var ok = await service.CompleteAppointmentAsync(id);
        Console.WriteLine(ok ? "Завершено" : "Не знайдено або вже завершено.");
    }

    static async Task ShowAppointmentHistory(string connectionString)
    {
        var repo = new AppointmentRepository(connectionString);
        var history = await repo.GetAppointmentHistoryAsync(sqlConnection);

        foreach (var a in history)
        {
            Console.WriteLine($"ID: {a.AppointmentId}");
            Console.WriteLine($"Пацієнт: {a.PatientUsername}");
            Console.WriteLine($"Лікар: {a.DoctorUsername}");
            Console.WriteLine($"Спеціальність: {a.SpecialtyName}");
            Console.WriteLine($"Час: {a.ScheduledAt}");
            Console.WriteLine($"Статус: {a.Status}");
            Console.WriteLine(new string('-', 30));
        }
    }
}
