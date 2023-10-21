using MediSearch.Core.Application;
using MediSearch.Infrastructure.Identity;
using MediSearch.Infrastructure.Persistence;
using MediSearch.Infrastructure.Shared;
using MediSearch.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace MediSearch.WebApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		public IWebHostEnvironment Environment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificDomain",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5173")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });
            services.AddApplicationLayer(Configuration);
			services.AddPersistenceInfrastructure(Configuration);
			services.AddIdentityInfrastructure(Configuration);
			services.AddSharedInfrastructure(Configuration);
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			services.AddControllers(options =>
			{
				options.Filters.Add(new ProducesAttribute("application/json"));
			}).ConfigureApiBehaviorOptions(options =>
			{
				options.SuppressInferBindingSourcesForParameters = true;
				options.SuppressMapClientErrors = true;
			})
			.AddJsonOptions(x =>
			{
				x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
				x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

			services.AddHealthChecks();
			services.AddSwaggerExtension();
			services.AddApiVersioningExtension();
			services.AddDistributedMemoryCache();
			services.AddSession(options =>
            {
                options.Cookie.Name = "MiSesion";
                options.Cookie.HttpOnly = true; 
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });
			
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
            app.UseCors("AllowSpecificDomain");
            if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();

			app.UseRouting();
			
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseSwaggerExtension();
			app.UseHealthChecks("/health");
			app.UseSession();
			app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
