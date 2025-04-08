using Backend.Data.Repository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
		ILogger<AuthController> logger,
		IRepository repository,
		IDataProtectionProvider dpProvider ) : ControllerBase
{
	private readonly ILogger<AuthController> _logger = logger;
	private readonly IRepository _repo = repository;
	private readonly IDataProtectionProvider _dpProvider = dpProvider;

	[HttpPost("login")]
    public IActionResult Login()
    {
        return Ok("Hello from login Action");
    }

    [HttpPost("signup")]
    public IActionResult SignUp([FromBody] LoginDto signUp)
    {
        // Check if the request is null or if any of the fields are empty
        if (signUp == null || 
            string.IsNullOrEmpty(signUp.Company) || 
            string.IsNullOrEmpty(signUp.Username) || 
            string.IsNullOrEmpty(signUp.Password))
        {
            return BadRequest("Todos los campos son necesarios");
        }

        var result = _repo.SetupNewAccount(signUp.Company, signUp.Username, signUp.Password);
        
        if (result.IsFailure)
		{
			return BadRequest(result.Error);
		}

		//var redirectUrl = $"{Request.Scheme}://{result.Value.Subdomain}.{Request.Host}/login";
		//return Redirect(redirectUrl);

		return Ok(result.Value);
	}

	[HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok("Logout");
    }
}

public record LoginDto(string Company, string Subdomain, string Username, string Password);
