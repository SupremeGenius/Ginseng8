using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Ginseng.Mvc.Services
{
	/// <summary>
	/// help from https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-2.2&tabs=visual-studio
	/// and https://medium.com/@kevinrodrguez/enabling-email-verification-in-asp-net-core-identity-ui-2-1-b87f028a97e0
	/// </summary>
	public class IdentityEmailSender : IEmailSender
	{
		private readonly EmailService _emailService;

		public IdentityEmailSender(IConfiguration config)
		{
			_emailService = new EmailService(config);
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			// not ever getting called
			await _emailService.SendAsync(email, subject, htmlMessage, htmlMessage);
		}
	}
}