using Backend.Data.Repository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
	[ApiController]
	[Route("api/{controller}")]
	public class SiteController(
		ILogger<AuthController> logger,
		IRepository repository,
		IDataProtectionProvider dpProvider) : ControllerBase
	{
		private readonly ILogger<AuthController> _logger = logger;
		private readonly IRepository _repo = repository;
		private readonly IDataProtectionProvider _dpProvider = dpProvider;

		[HttpPost("contactus")]
		public IActionResult ContactUs()
		{
			// CONTINUE HERE...

			return View();
		}
	}
}
