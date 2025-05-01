using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CelOrdApp.Areas.Cashier.Controllers
{
	public class HomeController : Controller
	{
		[Area("Cashier")]
		[Authorize(Roles = "Cashier")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
