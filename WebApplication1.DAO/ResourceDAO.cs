using System.Data;
using WebApplication1.DAO.Models;
using Microsoft.Data.SqlClient;
using WebApplication1.DAO.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication1.DAO
{
    public class ResourceDAO : IResourceDAO
    {
        private readonly string _connectionString;
        private readonly ILogger<ResourceDAO> _logger;

        public ResourceDAO(IConfiguration configuration, ILogger<ResourceDAO> logger)
        {
            _connectionString = configuration.GetConnectionString("DB_Connection_string");
            _logger = logger;
        }

        public async Task<List<ResourceRequest>> GetAllEmployeesAsync()
        {
            var employees = new List<ResourceRequest>();

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("sp_GetAllEmployees", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(new ResourceRequest
                    {
                        EmpId = reader["EmpId"].ToString(),
                        Name = reader["Name"].ToString(),
                        Designation = reader["Designation"].ToString(),
                        Reporting_To = reader["Reporting_To"].ToString(),
                        Billable = Convert.ToBoolean(reader["Billable"]),
                        Skills = reader["Skills"].ToString(),
                        Project_Allocation = reader["Project_Allocation"].ToString(),
                        Location = reader["Location"].ToString(),
                        Email = reader["Email"].ToString(),
                        CTE_DOJ = DateOnly.FromDateTime((DateTime)reader["CTE_DOJ"]),
                        Remarks = reader["Remarks"].ToString()
                    });
                }

                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllEmployeesAsync");
                return new List<ResourceRequest>();
            }
        }

        public async Task<ResourceRequest> GetEmployeeByIdAsync(string empId)
        {
            try
            {
                ResourceRequest employee = null;

                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("sp_GetEmployeeById", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@EmpId", empId);

                await conn.OpenAsync();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    employee = new ResourceRequest
                    {
                        EmpId = reader["EmpId"].ToString(),
                        Name = reader["Name"].ToString(),
                        Designation = reader["Designation"].ToString(),
                        Reporting_To = reader["Reporting_To"].ToString(),
                        Billable = Convert.ToBoolean(reader["Billable"]),
                        Skills = reader["Skills"].ToString(),
                        Project_Allocation = reader["Project_Allocation"].ToString(),
                        Location = reader["Location"].ToString(),
                        Email = reader["Email"].ToString(),
                        CTE_DOJ = DateOnly.FromDateTime((DateTime)reader["CTE_DOJ"]),
                        Remarks = reader["Remarks"].ToString()
                    };
                }

                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetEmployeeByIdAsync for EmpId: {EmpId}", empId);
                return null;
            }
        }

        public async Task<ResourceResponse> AddEmployeeAsync(ResourceRequest employee)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("sp_AddEmployee", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SetParameters(cmd, employee);

                await conn.OpenAsync();
                int rows = await cmd.ExecuteNonQueryAsync();

                return new ResourceResponse
                {
                    Success = rows > 0,
                    Message = rows > 0 ? "Employee added successfully." : "Add failed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddEmployeeAsync for EmpId: {EmpId}", employee.EmpId);
                return new ResourceResponse { Success = false, Message = "Internal server error." };
            }
        }

        public async Task<ResourceResponse> UpdateEmployeeAsync(ResourceRequest employee)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("sp_UpdateEmployee", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SetParameters(cmd, employee);

                await conn.OpenAsync();
                int rows = await cmd.ExecuteNonQueryAsync();

                return new ResourceResponse
                {
                    Success = rows > 0,
                    Message = rows > 0 ? "Employee updated successfully." : "Update failed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UpdateEmployeeAsync for EmpId: {EmpId}", employee.EmpId);
                return new ResourceResponse { Success = false, Message = "Internal server error." };
            }
        }

        public async Task<ResourceResponse> DeleteEmployeeAsync(string empId)
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand("sp_DeleteEmployee", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@EmpId", empId);

                await conn.OpenAsync();
                int rows = await cmd.ExecuteNonQueryAsync();

                return new ResourceResponse
                {
                    Success = rows > 0,
                    Message = rows > 0 ? "Employee deleted successfully." : "Delete failed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in DeleteEmployeeAsync for EmpId: {EmpId}", empId);
                return new ResourceResponse { Success = false, Message = "Internal server error." };
            }
        }

        private void SetParameters(SqlCommand cmd, ResourceRequest employee)
        {
            
            cmd.Parameters.AddWithValue("@EmpId", employee.EmpId);
            cmd.Parameters.AddWithValue("@Name", employee.Name);
            cmd.Parameters.AddWithValue("@Designation", employee.Designation);
            cmd.Parameters.AddWithValue("@Reporting_To", employee.Reporting_To);
            cmd.Parameters.AddWithValue("@Billable", employee.Billable);
            cmd.Parameters.AddWithValue("@Skills", employee.Skills);
            cmd.Parameters.AddWithValue("@Project_Allocation", employee.Project_Allocation ?? (object)DBNull.Value); 
            cmd.Parameters.AddWithValue("@Location", employee.Location);
            cmd.Parameters.AddWithValue("@Email", employee.Email);
            cmd.Parameters.AddWithValue("@CTE_DOJ", employee.CTE_DOJ);
            cmd.Parameters.AddWithValue("@Remarks", employee.Remarks ?? (object)DBNull.Value);
        }
    }
}
