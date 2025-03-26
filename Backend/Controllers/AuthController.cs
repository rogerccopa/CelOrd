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
            // CONTINUE HERE... support json post
            
            return Ok("Login");
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
