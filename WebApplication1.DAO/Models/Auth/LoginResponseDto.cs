using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource.DAO.Models.Auth
{
    public class LoginResponseDto
    {
        public string? HashPassword { get; set; }
        public string? Role { get; set; }
    }
}
