using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelOrdApp.Areas.Attendant.Controllers
{
	public class HomeController : Controller
	{
		[Area("Attendant")]
		[Authorize(Roles = "Attendant")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
