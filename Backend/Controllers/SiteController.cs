using Backend.Data;
using Backend.Data.Repository;
using Backend.Models;
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
        AppParams appParams) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;
        private readonly IRepository _repo = repository;
        private readonly AppParams _appParams = appParams;

        [HttpPost("contactus")]
        public IActionResult ContactUs([FromBody] JsonElement body)
        {
            var result = Models.Result<MessageObj>.Success(new(0, "Hemos recibido su mensaje."));

			// use gmail smtp server
			var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_appParams.SmtpAccount, _appParams.SmtpPassword),
                Timeout = 30_000
			};

            string fromEmail = body.GetProperty("email").GetString() ?? _appParams.SmtpAccount;
            string fromMessage = body.GetProperty("message").GetString() ?? "No message";

			using (var message = new MailMessage(fromEmail, _appParams.SmtpAccount))
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
					string module = ControllerContext.ActionDescriptor.ControllerName;
					string error = ex.Message + (ex.InnerException != null ? $" {ex.InnerException.Message}" : "");
					_repo.LogError(module, error);

					result = Models.Result<MessageObj>.Failure($"Error en servidor: {ex.Message}");
					
					return StatusCode(StatusCodes.Status500InternalServerError, result);
				}
			}

			return Ok(result);
		}
	}
}
