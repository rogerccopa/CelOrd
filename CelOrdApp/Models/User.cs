using static Domain.EntityTypes;

namespace CelOrdApp.Models;

public class User
{
	public int Id { get; set; }
	public string FullName { get; set; } = string.Empty;
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public UserType UserType { get; set; } = UserType.Unknown;
	public DateTime CreatedAt { get; set; }
	public virtual List<UserClaim> Claims { get; set; } = [];
}
