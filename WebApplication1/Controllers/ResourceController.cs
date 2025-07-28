using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Resource.Services;
using WebApplication1.DAO.Models;
using WebApplication1.orchestration.Orches_Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceOrchestration _orchestration;
        private readonly ILogger<ResourceController> _logger;
        private readonly IConfiguration _configuration; // ✅ Injected properly

        public ResourceController(
            IResourceOrchestration orchestration,
            ILogger<ResourceController> logger,
            IConfiguration configuration) // ✅ Constructor injection
        {
            _orchestration = orchestration;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet(APIUri.GetAllEmployee)]
        public async Task<ActionResult<List<ResourceRequest>>> GetAll()
        {
            try
            {
                _logger.LogInformation("GetAllEmployee=");
                var result = await _orchestration.GetAllEmployeesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetAllEmployee error: " + ex.Message, ex);
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet(APIUri.GetEmployeeByID)]
        public async Task<ActionResult<ResourceRequest>> GetById(string empId)
        {
            try
            {
                _logger.LogInformation("GetEmployeeByID: {EmpId}", empId);
                var employee = await _orchestration.GetEmployeeByIdAsync(empId);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching employee with ID: {EmpId}", empId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost(APIUri.InsertEmployee)]
        public async Task<ActionResult<ResourceResponse>> Add([FromBody] ResourceRequest employee)
        {
            try
            {
                _logger.LogInformation("InsertEmployee: {@Employee}", employee);
                var result = await _orchestration.AddEmployeeAsync(employee);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding employee: {@Employee}", employee);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut(APIUri.UpdateEmployee)]
        public async Task<ActionResult<ResourceResponse>> Update([FromBody] ResourceRequest employee)
        {
            try
            {
                _logger.LogInformation("UpdateEmployee: {@Employee}", employee);
                var result = await _orchestration.UpdateEmployeeAsync(employee);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating employee: {@Employee}", employee);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete(APIUri.DeleteEmployee)]
        public async Task<ActionResult<ResourceResponse>> Delete(string empId)
        {
            _logger.LogInformation("Delete request for EmpId: {EmpId}", empId);

            try
            {
                var result = await _orchestration.DeleteEmployeeAsync(empId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting EmpId: {EmpId}", empId);
                return StatusCode(500, new ResourceResponse { Success = false, Message = "Internal server error" });
            }
        }

        [HttpPost(APIUri.UploadExcel)]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            _logger.LogInformation("UploadExcel called.");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
                return BadRequest("No file uploaded.");
            }

            try
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook(file.OpenReadStream());
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed()?.RowsUsed();

                if (rows == null)
                {
                    return BadRequest("The uploaded Excel file is empty or invalid.");
                }

                var connectionString = _configuration.GetConnectionString("DB_Connection_string");

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    foreach (var row in rows.Skip(1))
                    {
                        var empId = row.Cell(1).GetString();
                        var name = row.Cell(2).GetString();
                        var designation = row.Cell(3).GetString();
                        var reportingTo = row.Cell(4).GetString();
                        var billable = row.Cell(5).GetValue<string>().Trim().ToUpper() == "YES";
                        var skills = row.Cell(6).GetString();
                        var projectAllocation = row.Cell(7).GetString();
                        var location = row.Cell(8).GetString();
                        var email = row.Cell(9).GetString();
                        var cteDoj = row.Cell(10).GetDateTime();
                        var remarks = row.Cell(11).GetString();

                        var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Employees WHERE EmpId = @EmpId", connection, transaction);
                        checkCmd.Parameters.AddWithValue("@EmpId", empId);
                        int count = (int)await checkCmd.ExecuteScalarAsync();

                        if (count > 0)
                        {
                            _logger.LogWarning("Skipping duplicate EmpId: {EmpId}", empId);
                            continue;
                        }

                        var insertEmployee = new SqlCommand(@"
                            INSERT INTO Employees (
                                EmpId, Name, Designation, Reporting_To, Billable,
                                Location, Email, CTE_DOJ
                            ) VALUES (
                                @EmpId, @Name, @Designation, @Reporting_To, @Billable,
                                @Location, @Email, @CTE_DOJ)", connection, transaction);

                        insertEmployee.Parameters.AddWithValue("@EmpId", empId);
                        insertEmployee.Parameters.AddWithValue("@Name", name);
                        insertEmployee.Parameters.AddWithValue("@Designation", designation);
                        insertEmployee.Parameters.AddWithValue("@Reporting_To", reportingTo);
                        insertEmployee.Parameters.AddWithValue("@Billable", billable);
                        insertEmployee.Parameters.AddWithValue("@Location", location);
                        insertEmployee.Parameters.AddWithValue("@Email", email);
                        insertEmployee.Parameters.AddWithValue("@CTE_DOJ", cteDoj);

                        await insertEmployee.ExecuteNonQueryAsync();

                        var insertDetails = new SqlCommand(@"
                            INSERT INTO EmployeeDetails (
                                EmpId, Skills, Project_Allocation, Remarks
                            ) VALUES (
                                @EmpId, @Skills, @Project_Allocation, @Remarks)", connection, transaction);

                        insertDetails.Parameters.AddWithValue("@EmpId", empId);
                        insertDetails.Parameters.AddWithValue("@Skills", skills);
                        insertDetails.Parameters.AddWithValue("@Project_Allocation", projectAllocation);
                        insertDetails.Parameters.AddWithValue("@Remarks", string.IsNullOrWhiteSpace(remarks) ? DBNull.Value : remarks);

                        await insertDetails.ExecuteNonQueryAsync();
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("Excel data uploaded successfully.");
                    //return Ok("Excel data imported successfully.");
                    return Ok(new { message = "Excel data imported successfully." });
                }
                catch (Exception innerEx)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(innerEx, "Transaction rolled back due to error.");
                    return StatusCode(500, "Failed to import data from Excel. Transaction rolled back.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during Excel upload.");
                return StatusCode(500, $"An error occurred while importing Excel data: {ex.Message}");
            }
        }
    }
}
