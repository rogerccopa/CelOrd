using static Domain.EntityTypes;

namespace CelOrdApp.DTOs;

public class UserDto
{
    public int Id { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public UserType UserType { get; set; }
}
