﻿namespace Backend.Models;

public class CelOrdenAccount
{
	public int Id { get; set; }
	public string CompanyName { get; set; } = string.Empty;
	public string Subdomain { get; set; } = string.Empty;
	public string DbName { get; set; } = string.Empty;
}
