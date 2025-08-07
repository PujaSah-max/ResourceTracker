using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Resource.DAO.Interface;
using Resource.DAO.Models.Auth;
using Resource.Orchestration.Orchestration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Resource.Orchestration
{
    public class AuthOrchestration : IAuthOrchestration
    {
        private readonly IConfiguration _configuration;
        IAuthDAO _authDAO;
        public AuthOrchestration(IAuthDAO authDAO, IConfiguration configuration)
        {
            _authDAO = authDAO;
            _configuration = configuration;
        }

        public async Task<int?> CreateUserAsync(SignupDTO signup)
        {
            signup.Password = HashPassword(signup.Password);
            return await _authDAO.CreateUserAsync(signup);
        }

        public async Task<string?> LoginAsync(LoginDto login)
        {
            try
            {
                var loginResult = await _authDAO.LoginAsync(login);
                var myHashedPassword = HashPassword(login.Password);
                Console.WriteLine(myHashedPassword);
                Console.WriteLine(loginResult.HashPassword);
                if(myHashedPassword != loginResult.HashPassword)
                {
                    return null;
                }
                var token = GetJwtToken();
                return token;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string GetJwtToken()
        {
            var claims = new List<Claim>
            {
                new Claim("Name", "Puja")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
