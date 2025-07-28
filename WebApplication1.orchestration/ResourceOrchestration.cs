using Microsoft.Extensions.Logging;
using WebApplication1.DAO.Interface;
using WebApplication1.DAO.Models;
using WebApplication1.orchestration.Orches_Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.orchestration
{
    public class ResourceOrchestration : IResourceOrchestration
    {
        private readonly IResourceDAO _employeeDao;
        private readonly ILogger<ResourceOrchestration> _logger;

        public ResourceOrchestration(IResourceDAO employeeDao, ILogger<ResourceOrchestration> logger)
        {
            _employeeDao = employeeDao;
            _logger = logger;
        }

        public async Task<List<ResourceRequest>> GetAllEmployeesAsync()
        {
            try
            {
                return await _employeeDao.GetAllEmployeesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all employees.");
                return new List<ResourceRequest>();
            }
        }

        public async Task<ResourceRequest> GetEmployeeByIdAsync(string empId)
        {

            try
            {
                return await _employeeDao.GetEmployeeByIdAsync(empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while fetching employee with ID: {empId}");
                return new ResourceRequest();
                
            }
        }

        public async Task<ResourceResponse> AddEmployeeAsync(ResourceRequest employee)
        {
            try
            {
                return await _employeeDao.AddEmployeeAsync(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding employee.");
                return new ResourceResponse();
                
            }
        }

        public async Task<ResourceResponse> UpdateEmployeeAsync(ResourceRequest employee)
        {
            

            try
            {
                return await _employeeDao.UpdateEmployeeAsync(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating employee with ID: {employee.EmpId}");
                return new ResourceResponse();
                
            }
        }

        public async Task<ResourceResponse> DeleteEmployeeAsync(string empId)
        {
          
            try
            {
                return await _employeeDao.DeleteEmployeeAsync(empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting employee with ID: {empId}");
                return new ResourceResponse();
                
            }
        }
    }
}
