using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelOrdApp.Areas.Cook.Controllers
{
	public class HomeController : Controller
	{
		[Area("Cook")]
		[Authorize(Roles = "Cook")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
