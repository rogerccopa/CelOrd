using CelOrdApp.Models;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Domain.EntityTypes;

namespace CelOrdApp.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Menu()
		{
			return View();
		}

		public IActionResult Users()
		{
			List<User> accounts = [
				new User{FullName="Roger Ccopa", Username="roger", Password="roger", UserType=UserType.Attendant, CreatedAt=DateTime.Now},
				new User{FullName="Enrique Ccopa", Username="kike", Password="kike", UserType=UserType.Cook, CreatedAt=DateTime.Now},
				new User{FullName="Sonia Quispe Espinoza", Username="sonia", Password="sonia", UserType=UserType.Cashier, CreatedAt=DateTime.Now},
			];

			return View(accounts);
		}
	}
}
