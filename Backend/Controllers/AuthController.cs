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

        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok("Register");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok("Logout");
        }
    }
}
