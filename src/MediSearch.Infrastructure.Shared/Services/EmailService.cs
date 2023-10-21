using MailKit.Net.Smtp;
using MailKit.Security;
using MediSearch.Core.Domain.Settings;
using MediSearch.Core.Application.Dtos.Email;
using MediSearch.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Shared.Services
{
	public class EmailService : IEmailService
	{
		public MailSettings _mailSettings { get; }

		public EmailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}

		public async Task SendAsync(EmailRequest request)
		{
			try
			{
				// create message
				var email = new MimeMessage();
				email.Sender = MailboxAddress.Parse(request.From ?? _mailSettings.EmailFrom);
				email.To.Add(MailboxAddress.Parse(request.To));
				email.Subject = request.Subject;
				var builder = new BodyBuilder();
				builder.HtmlBody = request.Body;
				email.Body = builder.ToMessageBody();
				using var smtp = new SmtpClient();
				smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
				smtp.Connect(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);
				smtp.Authenticate(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
				await smtp.SendAsync(email);
				smtp.Disconnect(true);

			}
			catch (Exception ex)
			{

			}
		}

	}
}
