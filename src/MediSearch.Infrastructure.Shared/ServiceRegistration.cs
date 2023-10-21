using MediSearch.Core.Domain.Settings;
using MediSearch.Core.Application.Interfaces.Services;
using MediSearch.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Shared
{
	public static class ServiceRegistration
	{
		public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
			services.AddTransient<IEmailService, EmailService>();
		}
	}
}
