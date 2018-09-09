using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using StartUpApi.Data.Models;
using StartUpApi.Data.Models.Enums;

namespace StartUpApi.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StartUpApi.Data.Models.ApplicationClient", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("AllowedOrigin")
                        .HasMaxLength(150);

                    b.Property<int>("ApplicationType");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<int>("RefreshTokenLifeTime");

                    b.Property<string>("Secret")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ApplicationClient");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("Default");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<bool>("IsStatic");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedName");

                    b.HasKey("Id");

                    b.ToTable("ApplicationRole");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.ApplicationRolePermission", b =>
                {
                    b.Property<int>("permissionId");

                    b.Property<string>("roleId");

                    b.HasKey("permissionId", "roleId");

                    b.HasIndex("roleId");

                    b.ToTable("ApplicationRolePermission");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsTemporaryPassword");

                    b.Property<DateTime?>("LastLoginDate");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("ProfilePictureUrl");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Surname");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("ApplicationUser");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.ApplicationUserRole", b =>
                {
                    b.Property<string>("RoleId");

                    b.Property<string>("UserId");

                    b.Property<string>("Discriminator");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("ApplicationUserRole");

                    b.HasDiscriminator().HasValue("ApplicationUserRole");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.AuditTrail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Error")
                        .HasMaxLength(600);

                    b.Property<string>("Operation")
                        .HasMaxLength(50);

                    b.Property<DateTime>("OperationDate");

                    b.Property<string>("OperationDescription")
                        .HasMaxLength(200);

                    b.Property<string>("UserId");

                    b.HasKey("ID");

                    b.HasIndex("UserId");

                    b.ToTable("AuditTrail");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.DbVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("VersionNumber");

                    b.HasKey("Id");

                    b.ToTable("DbVersion");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.OrganizationUnit", b =>
                {
                    b.Property<int>("OrganizationId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Level");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70);

                    b.Property<int?>("ParentId");

                    b.HasKey("OrganizationId");

                    b.HasIndex("ParentId");

                    b.ToTable("OrganizationUnit");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.Permission", b =>
                {
                    b.Property<int>("ID");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(200);

                    b.Property<int?>("Level");

                    b.Property<string>("Name")
                        .HasMaxLength(150);

                    b.Property<int?>("ParentId");

                    b.HasKey("ID");

                    b.HasIndex("ParentId");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.RefreshToken", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<DateTime>("ExpiresUtc");

                    b.Property<DateTime>("IssuedUtc");

                    b.Property<string>("LoginSource")
                        .HasMaxLength(128);

                    b.Property<string>("ProtectedTicket")
                        .IsRequired();

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.UserOrganization", b =>
                {
                    b.Property<int>("OrganizationId");

                    b.Property<string>("UserId");

                    b.HasKey("OrganizationId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserOrganization");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.UserPermission", b =>
                {
                    b.Property<int>("permissionId")
                        .HasColumnName("permissionId");

                    b.Property<string>("userId")
                        .HasColumnName("userId");

                    b.HasKey("permissionId", "userId");

                    b.HasIndex("userId");

                    b.ToTable("UserPermission");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.UserSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("appMode");

                    b.Property<DateTime?>("endTime");

                    b.Property<DateTime>("startDate");

                    b.Property<string>("userId");

                    b.HasKey("Id");

                    b.HasIndex("userId");

                    b.ToTable("UserSession");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.ApplicationRolePermission", b =>
                {
                    b.HasOne("StartUpApi.Data.Models.Permission", "PERMISSION")
                        .WithMany("APPLICATIONROLEPERMISSIONS")
                        .HasForeignKey("permissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StartUpApi.Data.Models.ApplicationRole", "ROLE")
                        .WithMany("ROLEPERMISSIONS")
                        .HasForeignKey("roleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StartUpApi.Data.Models.ApplicationUserRole", b =>
                {
                    b.HasOne("StartUpApi.Data.Models.ApplicationRole", "ROLE")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StartUpApi.Data.Models.ApplicationUser", "USER")
                        .WithMany("USERROLES")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StartUpApi.Data.Models.AuditTrail", b =>
                {
                    b.HasOne("StartUpApi.Data.Models.ApplicationUser", "USER")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.OrganizationUnit", b =>
                {
                    b.HasOne("StartUpApi.Data.Models.OrganizationUnit", "PARENT")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.Permission", b =>
                {
                    b.HasOne("StartUpApi.Data.Models.Permission", "PARENTPERMISSION")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("StartUpApi.Data.Models.UserOrganization", b =>
                {
                    b.HasOne("StartUpApi.Data.Models.OrganizationUnit", "ORGANIZATIONUNIT")
                        .WithMany("USERORGANIZATION")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StartUpApi.Data.Models.ApplicationUser", "USER")
                        .WithMany("USERORGANIZATION")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StartUpApi.Data.Models.UserPermission", b =>
                {
                    b.HasOne("StartUpApi.Data.Models.Permission", "PERMISSION")
                        .WithMany()
                        .HasForeignKey("permissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StartUpApi.Data.Models.ApplicationUser", "USER")
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StartUpApi.Data.Models.UserSession", b =>
                {
                    b.HasOne("StartUpApi.Data.Models.ApplicationUser", "USER")
                        .WithMany()
                        .HasForeignKey("userId");
                });
        }
    }
}
