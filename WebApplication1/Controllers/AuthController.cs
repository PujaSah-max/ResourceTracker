using Microsoft.AspNetCore.Mvc;
using Resource.DAO.Models.Auth;
using Resource.Orchestration.Orchestration;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Resource.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthOrchestration _authOrchestration;
        public AuthController(IAuthOrchestration authOrchestration)
        {
            _authOrchestration = authOrchestration;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> CreateUserAsync([FromBody] SignupDTO signup)
        {
            try
            {
                var userId = await _authOrchestration.CreateUserAsync(signup);
                if (userId == null)
                {
                    return StatusCode(500, "New User creation failed!");
                }
                return Ok(new { Message = "User Created Successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] LoginDto login)
        {
            try
            {
                var token = await _authOrchestration.LoginAsync(login);
                if (token == null)
                {
                    return Unauthorized();
                }
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
