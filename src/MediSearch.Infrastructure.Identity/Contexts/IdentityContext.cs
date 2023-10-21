using MediSearch.Core.Domain.Entities;
using MediSearch.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Identity.Contexts
{
	public class IdentityContext : IdentityDbContext<ApplicationUser>
	{
		public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//FLUENT API
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<ApplicationUser>(entity =>
			{
				entity.ToTable(name: "Users");
			});

			modelBuilder.Entity<IdentityRole>(entity =>
			{
				entity.ToTable(name: "Roles");
			});

			modelBuilder.Entity<IdentityUserRole<string>>(entity =>
			{
				entity.ToTable(name: "UserRoles");
			});

			modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
			{
				entity.ToTable(name: "UserLogins");
			});

		}

        public void TruncateTables()
        {
            var users = Set<ApplicationUser>();
            var roles = Set<IdentityRole>();
            var userRoles = Set<IdentityUserRole<string>>();
            var logins = Set<IdentityUserLogin<string>>();

            users.RemoveRange(users);
            roles.RemoveRange(roles);
            userRoles.RemoveRange(userRoles);
            logins.RemoveRange(logins);

			SaveChanges();
        }
    }
}
