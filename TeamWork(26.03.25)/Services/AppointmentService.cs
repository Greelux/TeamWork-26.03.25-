using HealthMeet.Data;
using HealthMeet.DTOs;
using HealthMeet.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealthMeet.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool, string)> CreateAppointmentAsync(AppointmentDto dto)
        {
            bool conflict = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                a.ScheduledAt == dto.ScheduledAt &&
                a.Status == "Created");

            if (conflict)
                return (false, "Лікар уже має прийом на цей час.");

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                ScheduledAt = dto.ScheduledAt,
                Status = "Created"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return (true, "Прийом створено.");
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null || appointment.Status != "Created")
                return false;

            appointment.Status = "Canceled";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null || appointment.Status != "Created")
                return false;

            appointment.Status = "Completed";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
