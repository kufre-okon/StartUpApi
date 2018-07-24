using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StartUpApi.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> opts) : base(opts)
        {
        }

        public DbSet<ApplicationClient> ApplicationClients { get; set; }
        public DbSet<ApplicationRolePermission> ApplicationRolePermissions { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().HasKey(s => s.Id);
            builder.Entity<ApplicationRole>().HasKey(s => s.Id);


            builder.Ignore<IdentityRole>();
            builder.Ignore<IdentityRoleClaim<string>>();
            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityUser>();
            builder.Ignore<IdentityUserRole<string>>();
            builder.Ignore<IdentityUserToken<string>>();

            ApplicationRoleMapping.Map(builder);
            ApplicationUserMapping.Map(builder);
            ApplicationRolePermissionMapping.Map(builder);
            ApplicationUserRoleMapping.Map(builder);
           
            //builder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //builder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //builder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    public class ApplicationRoleMapping
    {
        public static void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ApplicationRole>();
            builder.ToTable("ApplicationRole");
        }
    }

    public class ApplicationUserMapping
    {
        public static void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ApplicationUser>();
            builder.ToTable("ApplicationUser");
        }
    }

    public class ApplicationUserRoleMapping
    {
        public static void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ApplicationUserRole>();
            builder.ToTable("ApplicationUserRole")
                .HasKey(x => new
                {
                    x.UserId,
                    x.RoleId
                });
            builder.Property(x => x.UserId).HasColumnName("UserId");
            builder.Property(x => x.RoleId).HasColumnName("RoleId");
        }
    }

    public class ApplicationRolePermissionMapping
    {
        public static void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<ApplicationRolePermission>();
            builder.ToTable("ApplicationRolePermission")
                .HasKey(x => new
                {
                    x.permissionId,
                    x.roleId
                });

        }
    }
}
