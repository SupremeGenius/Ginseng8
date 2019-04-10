using Ginseng.Mvc.Classes;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Ginseng.Mvc.Services
{
	/// <summary>
	/// help from https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-2.2&tabs=visual-studio
	/// </summary>
	public class EmailSender : IEmailSender
	{
		private readonly EmailService _emailService;

		public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, IConfiguration config)
		{
			Options = optionsAccessor.Value;
			_emailService = new EmailService(config);
		}

		public AuthMessageSenderOptions Options { get; }

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			await _emailService.SendAsync(email, subject, htmlMessage, htmlMessage);
		}
	}
}