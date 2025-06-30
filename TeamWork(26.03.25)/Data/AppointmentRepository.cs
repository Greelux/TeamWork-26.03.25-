using Dapper;
using HealthMeet.DTOs;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AppointmentRepository
{
    private readonly string _connectionString;

    public AppointmentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<AppointmentHistoryDto>> GetAppointmentHistoryAsync(SqlConnection connection)
    {
        const string query = @"
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
            ORDER BY a.ScheduledAt DESC;";

        SqlConnection sqlConnection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<AppointmentHistoryDto>(query);
    }
}
