﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using StartUpApi.Models;
using StartUpApi.Models.Enums;

namespace StartUpApi.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20180725072249_userPermission")]
    partial class userPermission
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("StartUpApi.Models.ApplicationClient", b =>
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

                    b.ToTable("ApplicationClients");
                });

            modelBuilder.Entity("StartUpApi.Models.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("Default");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<bool>("IsStatic");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("ApplicationRole");
                });

            modelBuilder.Entity("StartUpApi.Models.ApplicationRolePermission", b =>
                {
                    b.Property<int>("permissionId");

                    b.Property<string>("roleId");

                    b.HasKey("permissionId", "roleId");

                    b.HasIndex("roleId");

                    b.ToTable("ApplicationRolePermission");
                });

            modelBuilder.Entity("StartUpApi.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsTemporaryPassword");

                    b.Property<DateTime?>("LastLoginDate");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Surname");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("ApplicationUser");
                });

            modelBuilder.Entity("StartUpApi.Models.ApplicationUserRole", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnName("UserId");

                    b.Property<string>("RoleId")
                        .HasColumnName("RoleId");

                    b.Property<string>("Discriminator");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("ApplicationUserRole");

                    b.HasDiscriminator().HasValue("ApplicationUserRole");
                });

            modelBuilder.Entity("StartUpApi.Models.Permission", b =>
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

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("StartUpApi.Models.RefreshToken", b =>
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

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("StartUpApi.Models.UserPermission", b =>
                {
                    b.Property<int>("permissionId")
                        .HasColumnName("permissionId");

                    b.Property<string>("userId")
                        .HasColumnName("userId");

                    b.HasKey("permissionId", "userId");

                    b.HasIndex("userId");

                    b.ToTable("UserPermission");
                });

            modelBuilder.Entity("StartUpApi.Models.ApplicationRolePermission", b =>
                {
                    b.HasOne("StartUpApi.Models.Permission", "PERMISSION")
                        .WithMany("APPLICATIONROLEPERMISSIONS")
                        .HasForeignKey("permissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StartUpApi.Models.ApplicationRole", "ROLE")
                        .WithMany("ROLEPERMISSIONS")
                        .HasForeignKey("roleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StartUpApi.Models.ApplicationUserRole", b =>
                {
                    b.HasOne("StartUpApi.Models.ApplicationRole", "ROLE")
                        .WithMany("APPLICATIONUSERROLES")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StartUpApi.Models.ApplicationUser", "USER")
                        .WithMany("USERROLES")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StartUpApi.Models.Permission", b =>
                {
                    b.HasOne("StartUpApi.Models.Permission", "PARENTPERMISSION")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("StartUpApi.Models.UserPermission", b =>
                {
                    b.HasOne("StartUpApi.Models.Permission", "PERMISSION")
                        .WithMany()
                        .HasForeignKey("permissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StartUpApi.Models.ApplicationUser", "USER")
                        .WithMany()
                        .HasForeignKey("userId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
