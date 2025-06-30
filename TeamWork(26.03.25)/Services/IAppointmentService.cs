using HealthMeet.DTOs;
using System.Threading.Tasks;

namespace HealthMeet.Services
{
    public interface IAppointmentService
    {
        Task<(bool Success, string Message)> CreateAppointmentAsync(AppointmentDto dto);
        Task<bool> CancelAppointmentAsync(int id);
        Task<bool> CompleteAppointmentAsync(int id);
    }
}
