namespace Backend.Models;

public class Company
{
	public int Id { get; set; }
	public string CompanyName { get; set; } = string.Empty;
	public string Subdomain { get; set; } = string.Empty;   // Slug e.g. sabor-del-norte.celuorden.com
	public string DbName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
}
