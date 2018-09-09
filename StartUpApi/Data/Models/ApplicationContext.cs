using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StartUpApi.Data.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> opts) : base(opts)
        {
        }

        public DbSet<ApplicationClient> ApplicationClient { get; set; }
        public DbSet<ApplicationRolePermission> ApplicationRolePermission { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<UserPermission> UserPermission { get; set; }
        public DbSet<OrganizationUnit> OrganizationUnit { get; set; }
        public DbSet<UserOrganization> UserOrganization { get; set; }
        public DbSet<AuditTrail> AuditTrail { get; set; }
        public DbSet<UserSession> UserSession { get; set; }
 public DbSet<DbVersion> DbVersion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasKey(s => s.Id);
            builder.Entity<ApplicationRole>().HasKey(s => s.Id);
            builder.Entity<UserOrganization>().HasKey(s => new { s.OrganizationId, s.UserId });

            builder.Ignore<IdentityRole>();
            builder.Ignore<IdentityRoleClaim<string>>();
            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityUser>();
            builder.Ignore<IdentityUserRole<string>>();
            builder.Ignore<IdentityUserToken<string>>();

            builder.Entity<ApplicationUserRole>().HasKey(s => new { s.RoleId, s.UserId });

            ApplicationRoleMapping.Map(builder);
            ApplicationUserMapping.Map(builder);
            ApplicationRolePermissionMapping.Map(builder);
            ApplicationUserPermissionMapping.Map(builder);
            //builder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //builder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //builder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

    #region mappings

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

    public class ApplicationUserPermissionMapping
    {
        public static void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<UserPermission>();
            builder.ToTable("UserPermission")
                .HasKey(x => new
                {
                    x.permissionId,
                    x.userId
                });
            builder.Property(x => x.permissionId).HasColumnName("permissionId");
            builder.Property(x => x.userId).HasColumnName("userId");
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

    #endregion

    #region others

    #endregion
}
