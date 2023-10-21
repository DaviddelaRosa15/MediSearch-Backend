using MediSearch.Core.Application.Interfaces.Repositories;
using MediSearch.Infrastructure.Persistence.Contexts;
using MediSearch.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Persistence
{
	public static class ServiceRegistration
	{
		public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			var connection = configuration.GetConnectionString("PostgreSQL");
            var password = configuration["PASSWORD"];
            var host = configuration["HOST"];
            connection = connection.Replace("#", password);
			connection = connection.Replace("ServerHost", host);

            #region Vaciar tablas
            /*var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseNpgsql(connection, m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName));
            var context = new ApplicationContext(optionsBuilder.Options);
			context.TruncateTables();*/
			#endregion

			#region Contexts
			if (configuration.GetValue<bool>("UseInMemoryDatabase"))
			{
				services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("MediSearchDb"));
			}
			else
			{
				services.AddDbContext<ApplicationContext>(options =>
				{
					options.EnableSensitiveDataLogging();
					options.UseNpgsql(connection,
					m => m.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName));
				});
			}
			#endregion

			#region Repositories
			services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddTransient<ICommentRepository, CommentRepository>();
			services.AddTransient<ICompanyRepository, CompanyRepository>();
			services.AddTransient<ICompanyTypeRepository, CompanyTypeRepository>();
			services.AddTransient<ICompanyUserRepository, CompanyUserRepository>();
			services.AddTransient<IFavoriteCompanyRepository, FavoriteCompanyRepository>();
			services.AddTransient<IFavoriteProductRepository, FavoriteProductRepository>();
			services.AddTransient<IHallRepository, HallRepository>();
			services.AddTransient<IHallUserRepository, HallUserRepository>();
			services.AddTransient<IMessageRepository, MessageRepository>();
			services.AddTransient<IMessageTypeRepository, MessageTypeRepository>();
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IReplyRepository, ReplyRepository>();
			#endregion
		}
	}
}
