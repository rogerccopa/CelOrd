using Backend.Data.Repository;
using Backend.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SiteController(
        ILogger<AuthController> logger,
        IRepository repository,
        IDataProtectionProvider dpProvider) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IRepository _repo = repository;
        private readonly IDataProtectionProvider _dpProvider = dpProvider;

        [HttpPost("contactus")]
        public IActionResult ContactUs([FromBody] JsonElement body)
        {
            var result = Models.Result<ErrorObj>.Success(new(0, "Hemos recibido su mensaje."));

			// use gmail smtp server to send out email
			var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("rogerccopa@gmail.com", "upbs oiab fhwe xlwc"),
                Timeout = 30_000
			};

            string fromEmail = body.GetProperty("email").GetString() ?? "rogerccopa@yahoo.com";
            string fromMessage = body.GetProperty("message").GetString() ?? "No message";

			using (var message = new MailMessage(fromEmail, "rogerccopa@gmail.com"))
            {
				message.Subject = "CelOrden - Contact Us";
				message.Body = $"FROM: {fromEmail} <br/>MESSAGE: {fromMessage}";
				message.IsBodyHtml = true;
				message.Priority = MailPriority.High;
				try
				{
					smtpClient.Send(message);
				}
				catch (Exception ex)
				{
					result = Models.Result<ErrorObj>.Failure($"Error en servidor: {ex.Message}");
					return BadRequest(result);
				}
			}

			return Ok(result);
		}
	}
}
