using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TeamWork_26._03._25_.Models;

namespace TeamWork_26._03._25_.Services
{
    public class JwtService
    {
        private readonly string _secretKey = "super_secret_key_1234567890";

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Name, user.FullName)
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
}
