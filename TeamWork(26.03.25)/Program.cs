using System;
using System.Threading.Tasks;
using HealthMeet.DTOs;

class Program
{
    static async Task Main()
    {
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