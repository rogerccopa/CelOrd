using CelOrdApp.Data;
using CelOrdApp.DTOs;
using CelOrdApp.Models;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CelOrdApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[ApiController]
[Route("Admin/[controller]")]
public class UsersController : Controller
{
	private readonly ClientDbContext _clientDbCtx;
	public UsersController(ClientDbContext clientDbContext)
	{
		_clientDbCtx = clientDbContext;
	}
	public IActionResult Index()
	{
		List<User> users = _clientDbCtx.Users.ToList();

		return View(users);
	}

	[HttpPost("SaveUser")]
	public IActionResult SaveUser([FromBody] UserDto userDto)
	{
		if (!ModelState.IsValid)
		{
			Result<MessageObj> result = Result<MessageObj>.Failure("Datos de usuario no válidos.");
			return BadRequest(result);
		}

		var passwordHasher = new PasswordHasher<User>();

		if (userDto.Id == 0)
		{
			var newUser = new User
			{
				FullName = userDto.FullName,
				Username = userDto.Username,
				Password = userDto.Password,
				Areas = userDto.Areas
			};

			newUser.Password = passwordHasher.HashPassword(newUser, userDto.Password);

			_clientDbCtx.Users.Add(newUser);
			_clientDbCtx.SaveChanges();

			userDto.Id = newUser.Id;
			userDto.Password = string.Empty;
		}
		else
		{
			var existingUser = _clientDbCtx.Users.Find(userDto.Id);

			if (existingUser == null)
			{
				Result<MessageObj> result = Result<MessageObj>.Failure("Usuario no encontrado.");
				return NotFound(result);
			}

			existingUser.FullName = userDto.FullName;
			existingUser.Username = userDto.Username;
			existingUser.Areas = userDto.Areas;
			
			if (!string.IsNullOrEmpty(userDto.Password))
			{
				string hashedPassword = passwordHasher.HashPassword(existingUser, userDto.Password);
				if (hashedPassword != existingUser.Password)
				{
					existingUser.Password = hashedPassword;
				}

				userDto.Password = string.Empty;
			}
		}

		return Ok(Result<UserDto>.Success(userDto));
	}
}
