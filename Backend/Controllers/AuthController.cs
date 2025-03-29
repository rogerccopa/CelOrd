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
