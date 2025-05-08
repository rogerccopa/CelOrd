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
[Route("Admin/[controller]")]
[ApiController]
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
				UserType = userDto.UserType
			};

			newUser.Password = passwordHasher.HashPassword(newUser, userDto.Password);

			_clientDbCtx.Users.Add(newUser);
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
			existingUser.UserType = userDto.UserType;

			if (!string.IsNullOrEmpty(userDto.Password))
			{
				string hashedPassword = passwordHasher.HashPassword(existingUser, userDto.Password);
				if (hashedPassword != existingUser.Password)
				{
					existingUser.Password = hashedPassword;
				}
			}
		}
		_clientDbCtx.SaveChanges();

		return Ok(Result<UserDto>.Success(userDto));
	}
}
