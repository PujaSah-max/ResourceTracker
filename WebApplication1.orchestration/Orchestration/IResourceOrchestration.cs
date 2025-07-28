using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.DAO.Models;

namespace WebApplication1.orchestration.Orches_Interface
{
    public interface IResourceOrchestration
    {
        Task<List<ResourceRequest>> GetAllEmployeesAsync();

        Task<ResourceRequest> GetEmployeeByIdAsync(string empId);

        Task<ResourceResponse> AddEmployeeAsync(ResourceRequest employee);

        Task<ResourceResponse> UpdateEmployeeAsync(ResourceRequest employee);

        Task<ResourceResponse> DeleteEmployeeAsync(string empId);
    }
}
