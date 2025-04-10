namespace Backend.Models;

public class AppError
{
	public int Id { get; set; }
	public string Module { get; set; } = string.Empty;
	public string Error { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
}
