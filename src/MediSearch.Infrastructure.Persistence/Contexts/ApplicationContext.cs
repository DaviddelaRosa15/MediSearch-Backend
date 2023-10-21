using MediSearch.Core.Domain.Common;
using MediSearch.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
        public DbSet<FavoriteCompany> FavoriteCompanies { get; set; }
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<HallUser> HallUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageType> MessagTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Reply> Replies { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Id = Guid.NewGuid().ToString().Substring(5, 8);
                        entry.Entity.Created = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "DefaultMediSearchUser";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "DefaultMediSearchUser";
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //FLUENT API
            modelBuilder.UseSerialColumns();

            #region Tables

            modelBuilder.Entity<Comment>()
                .ToTable("Comments");
            
            modelBuilder.Entity<Company>()
                .ToTable("Companies");

            modelBuilder.Entity<CompanyType>()
                .ToTable("Company_Types");

            modelBuilder.Entity<CompanyUser>()
                .ToTable("Company_Users");
            
            modelBuilder.Entity<FavoriteCompany>()
                .ToTable("Favorite_Companies");
            
            modelBuilder.Entity<FavoriteProduct>()
                .ToTable("Favorite_Products");

            modelBuilder.Entity<Hall>()
                .ToTable("Halls");

            modelBuilder.Entity<HallUser>()
                .ToTable("Hall_Users");

            modelBuilder.Entity<Message>()
                .ToTable("Messages");

            modelBuilder.Entity<MessageType>()
                .ToTable("Message_Types");

            modelBuilder.Entity<Product>()
                .ToTable("Products");
            
            modelBuilder.Entity<Reply>()
                .ToTable("Replies");
            #endregion

            #region Primary keys
            modelBuilder.Entity<Comment>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<Company>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<CompanyType>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<CompanyUser>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<FavoriteCompany>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<FavoriteProduct>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Hall>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<HallUser>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Message>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<MessageType>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Product>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<Reply>()
                .HasKey(x => x.Id);
            #endregion

            #region Relationships

            modelBuilder.Entity<Company>()
                .HasOne<CompanyType>(x => x.CompanyType)
                .WithMany(x => x.Companies)
                .HasForeignKey(x => x.CompanyTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CompanyUser>()
                .HasOne<Company>(x => x.Company)
                .WithMany(x => x.CompanyUsers)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HallUser>()
                .HasOne<Hall>(x => x.Hall)
                .WithMany(x => x.HallUsers)
                .HasForeignKey(x => x.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne<Hall>(x => x.Hall)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne<MessageType>(x => x.MessageType)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.MessageTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasOne<Company>(x => x.Company)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne<Product>(x => x.Product)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reply>()
                .HasOne<Comment>(x => x.Comment)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<FavoriteCompany>()
                .HasOne<Company>(x => x.Company)
                .WithMany(x => x.FavoriteCompanies)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteProduct>()
                .HasOne<Product>(x => x.Product)
                .WithMany(x => x.FavoriteProducts)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Property configurations
            #endregion
        }

        public void TruncateTables()
        {
            Comments.RemoveRange(Comments);
            Companies.RemoveRange(Companies);
            CompanyTypes.RemoveRange(CompanyTypes);
            CompanyUsers.RemoveRange(CompanyUsers);
            FavoriteCompanies.RemoveRange(FavoriteCompanies);
            FavoriteProducts.RemoveRange(FavoriteProducts);
            Halls.RemoveRange(Halls);
            HallUsers.RemoveRange(HallUsers);
            Messages.RemoveRange(Messages);
            MessagTypes.RemoveRange(MessagTypes);
            Products.RemoveRange(Products);
            Replies.RemoveRange(Replies);
            
            SaveChanges();
        }
    }
}
