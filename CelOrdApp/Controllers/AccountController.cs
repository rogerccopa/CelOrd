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

		var redirectUrl = $"{Request.Scheme}://{result.Value.Subdomain}.{Request.Host}/Home/login";
		return Redirect(redirectUrl);
	}

	[HttpGet]
	[Route("/Login")]
	public IActionResult Login()
	{
		string subdomain = Request.Host.Host.Split('.')[0].ToLower();
		var loginModel = new LoginViewModel();

		if (((string[])["localhost", "www", ""]).Contains(subdomain))
		{
			// ignore it
		}
		else
		{
			loginModel.Subdomain = subdomain;
		}

		return View(loginModel);
	}

	[HttpPost]
	public IActionResult Login(LoginViewModel loginViewModel)
	{
		if (string.IsNullOrEmpty(loginViewModel.Subdomain))
		{
			loginViewModel.Subdomain = loginViewModel.CompanyName.GenerateSlug();
		}

		var company = _repo.GetCompany(loginViewModel.Subdomain);

		if (string.IsNullOrEmpty(company.CompanyName))
		{
			ModelState.AddModelError("", $"[{loginViewModel.Subdomain}] no existe.");
			return View(loginViewModel);
		}

		if (!ModelState.IsValid)
		{
			return View(loginViewModel);
		}

		var siteUser = _repo.GetUser(company, loginViewModel.Username, loginViewModel.Password);

		if (siteUser.UserType == UserType.Unknown)   // invalid user credentials
		{
			ModelState.AddModelError("", "Usuario o contrase�a incorrecto");
			return View(loginViewModel);
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
		new(ClaimTypes.NameIdentifier, siteUser.Id.ToString()),
		new(ClaimTypes.Role, siteUser.UserType.ToString())
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
	}

	[HttpGet("/Logout")]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

		return RedirectToAction("Index", "Home");
	}
}
