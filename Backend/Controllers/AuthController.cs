using Backend.Data.Repository;
using Backend.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using static Backend.Models.EntityTypes;

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
    public IActionResult Login([FromBody] LoginDto loginRequest)
    {
		var company = _repo.GetCompany(loginRequest.Subdomain);

		if (company == null) {
			var result = Result<MessageObj>.Failure($"{loginRequest.Company} no existe.");
			return BadRequest(result);
		}

		// CONTINUE HERE...

		/////////// OLD CODE ///////////
		var siteUser = _repo.GetUser(company, loginRequest.Username, loginRequest.Password);

		if (siteUser.UserType == UserType.Unknown)   // invalid user credentials
		{
			var result = Result<MessageObj>.Failure("Usuario o contraseña incorrecto.");
			return BadRequest(result);
		}

		string controller = "Login";
		switch (siteUser.UserType)
		{
			case UserType.Admin:
				controller = "Admin";
				break;
			case UserType.Cashier:
				// todo
				break;
			case UserType.Cook:
				// todo
				break;
			case UserType.Attendant:
				// todo
				break;
			default:
				ViewBag.Username = loginViewModel.Username;
				ViewBag.ErrorMsg = $"Tipo de usuario {siteUser.UserType} no es valido.";
				return View("Login");
		}

		// create claims principal
		var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, siteUser.Id.ToString()),
				new Claim(ClaimTypes.Role, siteUser.UserType.ToString())
			};
		var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
		var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

		HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			claimsPrincipal,
			new AuthenticationProperties
			{
				IsPersistent = true,    // keep the user logged in across browser sessions
				ExpiresUtc = DateTime.UtcNow.AddDays(7)
			}
		);

		return RedirectToAction("Index", controller);
		/////////// OLD CODE ///////////


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
            return BadRequest(Result<MessageObj>.Failure("Todos los campos son necesarios"));
        }

		try
		{
			Result<CelOrdenAccount> result = _repo.SetupNewAccount(signUp.Company, signUp.Username, signUp.Password);

			if (result.IsFailure)
			{
				return BadRequest(Result<MessageObj>.Failure(result.Error));
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

			var result = Result<MessageObj>.Failure($"Error en servidor: {error}");
			return BadRequest(result);
		}
	}

	[HttpPost("logout")]
    public IActionResult Logout()
    {
		HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		return RedirectToAction("Index", "Home");
	}
}

public record LoginDto(string Company, string Subdomain, string Username, string Password);
