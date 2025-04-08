using static Backend.Models.EntityTypes;

namespace Backend.Models;

public class User
{
	public int Id { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public UserType UserType { get; set; } = UserType.Unknown;
	public DateTime CreatedAt { get; set; }
	public List<UserClaim> Claims { get; set; } = [];
}
