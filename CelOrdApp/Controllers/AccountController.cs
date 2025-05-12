using CelOrdApp.Data.Repository;
using CelOrdApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Domain.EntityTypes;

namespace CelOrdApp.Controllers;

public class AccountController(
	ILogger<HomeController> logger,
	IRepository repository,
	IDataProtectionProvider dpProvider) : Controller
{
	private readonly ILogger<HomeController> _logger = logger;
	private readonly IRepository _repo = repository;
	private readonly IDataProtectionProvider _dpProvider = dpProvider;

	[HttpGet]
	[Route("/Signup")]
	public IActionResult Signup()
	{
		return View();
	}

	[HttpPost]
	[Route("/Signup")]
	public IActionResult Signup(LoginViewModel signupRequest)
	{
		if (!ModelState.IsValid)
			return RedirectToAction("Signup");

		signupRequest.Subdomain = signupRequest.CompanyName.GenerateSlug();

		var result = _repo.SetupNewAccount(signupRequest);

		if (result.IsFailure)
		{
			ViewBag.CompanyName = signupRequest.CompanyName;
			ViewBag.Username = signupRequest.Username;
			ViewBag.ErrorMsg = result.Error;

			return View();
		}

		//var redirectUrl = $"{Request.Scheme}://{result.Value.Subdomain}.{Request.Host}/Home/login";

		string userMessage= "Se registr&oacute; exitosamente. <br/>Ingrese usando su Usuario (email) y Contraseña.";

		return RedirectToAction("Login", new { signupRequest.CompanyName, SuccessMsg = userMessage });
	}

	[HttpGet]
	[Route("/Login")]
	public IActionResult Login()
	{
		var loginModel = new LoginViewModel();

		if (!string.IsNullOrWhiteSpace(Request.Query["CompanyName"]))
		{
			loginModel.CompanyName = Request.Query["CompanyName"].ToString();
		}

		if (!string.IsNullOrEmpty(Request.Query["SuccessMsg"]))
		{
			ViewBag.SuccessMsg = Request.Query["SuccessMsg"].ToString();
		}

		return View(loginModel);
	}

	[HttpPost]
	[Route("/Login")]
	public IActionResult Login(LoginViewModel loginViewModel)
	{
		loginViewModel.Subdomain = loginViewModel.CompanyName.GenerateSlug();

		Company? company = _repo.GetCompany(loginViewModel.Subdomain);

		if (company == null)
		{
			ModelState.AddModelError("", $"{loginViewModel.Subdomain} no existe.");
			return View(loginViewModel);
		}

		if (!ModelState.IsValid)
		{
			return View(loginViewModel);
		}

		var siteUser = _repo.GetUser(company, loginViewModel.Username, loginViewModel.Password);

		if (siteUser.Areas.Count() == 0)   // invalid user credentials
		{
			ModelState.AddModelError("", "Usuario o contraseña incorrecto");
			return View(loginViewModel);
		}

		// create claims principal
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, siteUser.Id.ToString()),
			new(ClaimTypes.Role, string.Join(',', siteUser.Areas)),
			new("dbName", company.DbName)
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

		return RedirectToAction("Index", "Dashboard");
	}

	[HttpGet("/Logout")]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		return RedirectToAction("Index", "Home");
	}
}
