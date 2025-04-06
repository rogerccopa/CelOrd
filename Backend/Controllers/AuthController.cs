using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login()
        {
            return Ok("Hello from login Action");
        }

        [HttpPost("signup")]
        public IActionResult SignUp([FromBody] SignupRequest signUp)
        {
            // CONTINUE HERE... test this block of code
            // Check if the request is null or if any of the fields are empty
            if (signUp == null || 
                string.IsNullOrEmpty(signUp.Company) || 
                string.IsNullOrEmpty(signUp.Email) || 
                string.IsNullOrEmpty(signUp.Password))
            {
                return BadRequest("Todos los cmapos son necesarios");
            }

            // Create new company database

            return Ok($"Hello from Signup; {signUp.Company}, {signUp.Email}, {signUp.Password}");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Logout");
        }
    }

    public record SignupRequest(string Company, string Email, string Password);
}
