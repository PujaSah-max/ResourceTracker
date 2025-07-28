using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.DAO.Models
{
    public class ResourceRequest
    {
        public string? EmpId { get; set; } 
        public string Name { get; set; } 
        public string Designation { get; set; } 
        public string Reporting_To { get; set; } 
        public bool Billable { get; set; } 
        public string Skills { get; set; } 
        public string Project_Allocation { get; set; } 
        public string Location { get; set; } 
        

        public string Email { get; set; }
        public DateOnly CTE_DOJ { get; set; } 
        public string Remarks { get; set; } 
    }
}
