using Dapper;
using HealthMeet.DTOs;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

public class AppointmentRepository
{
    private readonly string _connectionString;

    public AppointmentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<(bool, string)> CreateAppointmentAsync(AppointmentDto dto)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var conflict = @"SELECT COUNT(*) FROM Appointments 
                             WHERE DoctorId = @DoctorId AND ScheduledAt = @ScheduledAt AND Status = 'Created'";

            int exists = await connection.ExecuteScalarAsync<int>(conflict, dto);
            if (exists > 0)
                return (false, "Конфлікт: лікар уже зайнятий у цей час.");

            var insert = @"INSERT INTO Appointments (PatientId, DoctorId, ScheduledAt, Status)
                           VALUES (@PatientId, @DoctorId, @ScheduledAt, 'Created')";

            await connection.ExecuteAsync(insert, dto);
            return (true, "Прийом створено.");
        }
    }

    public async Task<bool> CancelAppointmentAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "UPDATE Appointments SET Status = 'Canceled' WHERE Id = @Id AND Status = 'Created'";
            return await connection.ExecuteAsync(query, new { Id = id }) > 0;
        }
    }

    public async Task<bool> CompleteAppointmentAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            var query = "UPDATE Appointments SET Status = 'Completed' WHERE Id = @Id AND Status = 'Created'";
            return await connection.ExecuteAsync(query, new { Id = id }) > 0;
        }
    }

    public async Task<IEnumerable<AppointmentHistoryDto>> GetAppointmentHistoryAsync()
    {
        const string sql = @"
            SELECT 
                a.Id AS AppointmentId,
                p.Username AS PatientUsername,
                d.Username AS DoctorUsername,
                s.Name AS SpecialtyName,
                a.ScheduledAt,
                a.Status
            FROM Appointments a
            JOIN Users p ON a.PatientId = p.Id
            JOIN Users d ON a.DoctorId = d.Id
            JOIN Specialties s ON d.SpecialtyId = s.Id
            ORDER BY a.ScheduledAt DESC";

        using (var connection = new SqlConnection(_connectionString))
        {
            return await connection.QueryAsync<AppointmentHistoryDto>(sql);
        }
    }
}
