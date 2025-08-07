using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resource.DAO.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.DAO;
using System.Data;
using Resource.DAO.Interface;

namespace Resource.DAO
{
    public class AuthDAO : IAuthDAO
    {
        private readonly string _connectionString;
        private readonly ILogger<ResourceDAO> _logger;
        public AuthDAO(IConfiguration configuration, ILogger<ResourceDAO> logger)
        {
            _connectionString = configuration.GetConnectionString("DB_Connection_string");
            _logger = logger;
        }

        public async Task<int?> CreateUserAsync(SignupDTO signup)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CreateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@userName", signup.UserName);
            command.Parameters.AddWithValue("@email", signup.Email);
            command.Parameters.AddWithValue("@hashPassword", signup.Password);

            var UserId = new SqlParameter("@userId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(UserId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (int)UserId.Value;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto login)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_Login", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserNameOrEmail", login.UserNameOrEmail);
            
            var hashPassword = new SqlParameter("@HashPassword", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(hashPassword);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return new LoginResponseDto { 
                HashPassword = hashPassword.Value != DBNull.Value ? (string)hashPassword.Value : null
            };
        }
    }
}
