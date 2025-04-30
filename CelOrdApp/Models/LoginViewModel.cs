using System.ComponentModel.DataAnnotations;

namespace CelOrdApp.Models;

public class LoginViewModel
{
	[Required(ErrorMessage = "Ingrese nombre del negocio")]
	public string CompanyName { get; set; } = string.Empty;

	public string Subdomain { get; set; } = string.Empty;

	[Required(ErrorMessage = "Ingrese usuario/email")]
	[EmailAddress(ErrorMessage = "Email incorrecto")]
	public string Username { get; set; } = string.Empty;

	[Required(ErrorMessage = "Ingrese contraseña")]
	public string Password { get; set; } = string.Empty;
}
