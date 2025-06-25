using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using TeamWork_26._03._25_.Models;


namespace TeamWork_26._03._25_.Services
{
    public class UserService
    {
        private readonly string _connectionString = "Server=DESKTOP-H1DK40Q;Database=HealthMeet;Trusted_Connection=True;TrustServerCertificate=True;";

        public void RegisterUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                connection.Execute(sql, user);
            }
        }

        public User Login(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                return connection.QueryFirstOrDefault<User>(sql, new { Username = username, Password = password });
            }
        }

        public List<User> GetUsersByRole(string role)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Users WHERE Role = @Role";
                return connection.Query<User>(sql, new { Role = role }).ToList();
            }
        }

    }
}
