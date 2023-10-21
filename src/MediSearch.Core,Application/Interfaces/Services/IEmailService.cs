using MediSearch.Core.Domain.Settings;
using MediSearch.Core.Application.Dtos.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Interfaces.Services
{
	public interface IEmailService
	{
		public MailSettings _mailSettings { get; }
		Task SendAsync(EmailRequest request);
	}
}
