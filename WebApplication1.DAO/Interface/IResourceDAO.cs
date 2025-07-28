using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.DAO.Models;


namespace WebApplication1.DAO.Interface
{
    public interface IResourceDAO
    {
        Task<List<ResourceRequest>> GetAllEmployeesAsync();

        Task<ResourceRequest> GetEmployeeByIdAsync(string empId);

        // Updated to return ResponseModel
        Task<ResourceResponse> AddEmployeeAsync(ResourceRequest employee);

        Task<ResourceResponse> UpdateEmployeeAsync(ResourceRequest employee);

        Task<ResourceResponse> DeleteEmployeeAsync(string empId);

        


    }
}
