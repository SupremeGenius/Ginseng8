using Ginseng.Mvc.Classes;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ginseng.Mvc.Services
{
	/// <summary>
	/// help from https://docs.microsoft.com/en-us/aspnet/core/security/authentication/accconfirm?view=aspnetcore-2.2&tabs=visual-studio
	/// </summary>
	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration _config;

		public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, IConfiguration config)
		{
			Options = optionsAccessor.Value;
			_config = config;
		}

		public AuthMessageSenderOptions Options { get; }

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			await Execute(Options.SendGridKey, email, subject, htmlMessage);
		}

		public async Task<Response> Execute(string apiKey, string email, string subject, string htmlMessage)
		{
			var client = new SendGridClient(apiKey);
			var msg = new SendGridMessage()
			{
				From = new EmailAddress("Joe@contoso.com", "Joe Smith"),
				Subject = subject,
				PlainTextContent = htmlMessage,
				HtmlContent = htmlMessage
			};
			msg.AddTo(new EmailAddress(email));

			// Disable click tracking.
			// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
			msg.SetClickTracking(false, false);

			return await client.SendEmailAsync(msg);
		}
	}
}
