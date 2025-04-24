using Backend.Data.Repository;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
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

		try
		{
			Result<CelOrdenAccount> result = _repo.SetupNewAccount(signUp.Company, signUp.Username, signUp.Password);

			if (result.IsFailure)
			{
				return BadRequest(result);
			}

			//var redirectUrl = $"{Request.Scheme}://{result.Value.Subdomain}.{Request.Host}/login";
			//return Redirect(redirectUrl);

			return Ok(result);
		}
		catch (Exception ex)
		{
			string module = ControllerContext.ActionDescriptor.ControllerName;
			string error = ex.Message + (ex.InnerException != null ? $" {ex.InnerException.Message}" : "");
			_repo.LogError(module, error);

			Result<CelOrdenAccount> result = Result<CelOrdenAccount>.Failure($"Error en servidor: {error}");
			return BadRequest(result);
		}
	}

	[HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok("Logout");
    }
}

public record LoginDto(string Company, string Subdomain, string Username, string Password);
