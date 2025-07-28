using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.DAO.Models
{
    public class ResourceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public static ResourceResponse SuccessResponse(string message) =>
        new ResourceResponse { Success = true, Message = message };

        public static ResourceResponse FailureResponse(string message) =>
            new ResourceResponse { Success = false, Message = message };
    }
}
