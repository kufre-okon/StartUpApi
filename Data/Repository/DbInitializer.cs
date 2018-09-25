using Data.Models;
using Data.Models.Enums;
using General;
using General.Utilities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public DbInitializer(ApplicationContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private void SeedPermissions()
        {
            if (!_context.Permission.Any())
            {
                var pList = new List<Permission>() {
                    new Permission(){Name = "Pages", DisplayName = "Pages", },
                    new Permission(){Name = "Pages.Role", DisplayName = "Roles", },
                    new Permission(){Name = "Pages.Users", DisplayName = "Users",  },
                    new Permission(){Name = "Pages.OrganizationUnits", DisplayName = "Organization Units",},
                    new Permission(){Name = "Pages.Settings", DisplayName = "Settings",},
                    new Permission(){Name = "Pages.AuditLogs", DisplayName = "Audit Logs",},
                    new Permission(){Name = "Pages.Roles.Create", DisplayName = "Create",},
                    new Permission(){Name = "Pages.Roles.Edit", DisplayName = "Edit", },
                    new Permission(){Name = "Pages.Roles.Delete", DisplayName = "Delete",   },
                    new Permission(){Name = "Pages.Users.Create", DisplayName = "Create", },
                    new Permission(){Name = "Pages.Users.Edit", DisplayName = "Edit", },
                    new Permission(){Name = "Pages.Users.Delete", DisplayName = "Delete", },
                    new Permission(){Name = "Pages.Users.ChangePermission", DisplayName = "Change Permission", },
                    new Permission(){Name = "Pages.Users.ResetPassword", DisplayName = "Reset Password", },
                    new Permission(){Name = "Pages.OrganizationUnits.ManageOrganizationTree", DisplayName = "Manage Organization Tree", },
                    new Permission(){Name = "Pages.OrganizationUnits.ManageMembers", DisplayName = "Manage Members", },
                };

                int id = 1;
                pList.ForEach(e =>
                {
                    e.ID = id;
                    var split = (e.Name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)).ToList();
                    if (split.Count > 1)
                    {
                        split.RemoveAt(split.Count - 1);
                        var parentName = string.Join(".", split);
                        var parent = pList.FirstOrDefault(p => p.Name == parentName);
                        if (parent != null)
                            e.ParentId = parent.ID;
                    }
                    id++;
                });
                _context.Permission.AddRange(pList);
            }
        }
        private void SeedAdminPermissions()
        {
            var adminRole = _context.Roles.FirstOrDefault(r => r.Name.ToLower() == "administrator");
            if (adminRole != null)
            {
                // lets clear any previous admin rolePermissions
                var oldPerms = _context.ApplicationRolePermission.Where(e => e.roleId == adminRole.Id).ToList();
                if (oldPerms.Count > 0)
                    _context.ApplicationRolePermission.RemoveRange(oldPerms);
                _context.SaveChanges();
                // lets create new ones
                var permsIds = _context.Permission.Select(e => e.ID).ToList();
                var adminPerms = permsIds.Select(e => new ApplicationRolePermission() { permissionId = e, roleId = adminRole.Id });

                _context.ApplicationRolePermission.AddRange(adminPerms);
            }
        }

        private async Task SeedRoles()
        {
            var roles = new List<ApplicationRole>() {
                    new ApplicationRole(){ Name = "Administrator",  IsStatic=true, DateCreated=DateTime.UtcNow, Description="Administrator role" },
                    new ApplicationRole(){Name = "User", Description= "Default user role", Default=true, IsStatic=true, DateCreated=DateTime.UtcNow },
            };
            foreach (var r in roles)
            {
                if (!await _roleManager.RoleExistsAsync(r.Name))
                    await _roleManager.CreateAsync(r);
            }
        }

        private async Task SeedSuperUser()
        {            
            var user = new ApplicationUser()
            {
                UserName = Constants.SUPER_ADMIN_USERNAME.ToLower(),
                EmailConfirmed = true,
                IsActive = true,
                IsTemporaryPassword = false,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                Id = "01",
                FirstName = "Kufre",
                Surname = "Okon",
                Email = "startup123@gmail.com",
                DateCreated = DateTime.UtcNow,
                PhoneNumber = "07000000000",
                SecurityStamp = Guid.NewGuid().ToString("D"),
            };
            if (await _userManager.FindByNameAsync(user.UserName) == null)
            {
                var result = await _userManager.CreateAsync(user, "Superadmin_123");
                if (!result.Succeeded)
                    throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
            }
            var role = await _roleManager.FindByNameAsync("Administrator");
            var iis = _context.ApplicationUserRole.FirstOrDefault(e => e.UserId == user.Id && e.RoleId == role.Id);

            if (iis == null)
                _context.ApplicationUserRole.Add(new ApplicationUserRole() { RoleId = role.Id, UserId = user.Id });
        }

        private async Task SeedSampleUsers()
        {
            for (var i = 1; i <= 15; i++)
            {
                var u_i = DateTime.UtcNow.Millisecond;
                var user = new ApplicationUser()
                {
                    UserName = "User_" + i,
                    EmailConfirmed = true,
                    IsActive = true,
                    IsTemporaryPassword = false,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                    FirstName = "First Name " + u_i,
                    Surname = "Last Name " + u_i,
                    Email = "defaultemail" + u_i + "@gmail.com",
                    DateCreated = DateTime.UtcNow,
                    PhoneNumber = "07000000000" + u_i,
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                };
                if (await _userManager.FindByNameAsync(user.UserName) == null)
                {
                    var result = await _userManager.CreateAsync(user, "Default_" + i);
                    if (!result.Succeeded)
                        throw new Exception(string.Join("\n", result.Errors.Select(e => e.Description)));
                }
                var role = await _roleManager.FindByNameAsync("User");
                if (role != null)
                {
                    var iis = _context.ApplicationUserRole.FirstOrDefault(e => e.UserId == user.Id && e.RoleId == role.Id);

                    if (iis == null)
                        _context.ApplicationUserRole.Add(new ApplicationUserRole() { RoleId = role.Id, UserId = user.Id });
                }
            }
        }

        public async Task Seed()
        {
            try
            {
                _context.Database.EnsureCreated();

                SeedPermissions();
                await SeedRoles();
                await SeedSuperUser();
                await SeedSampleUsers(); // this is only for test purpose
                SeedAdminPermissions();

                SeedClients();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                Console.WriteLine(error);
                Debug.WriteLine(error);
            }
        }

        private void SeedClients()
        {
            List<ApplicationClient> ClientsList = new List<ApplicationClient>
            {
                new ApplicationClient
                {
                    Id = "ngStartupApp",
                    Secret= Helper.GetHash("<startup></startup>"),
                    Name="AngularJS front-end Application",
                    ApplicationType =  ApplicationTypes.JavaScript,
                    Active = true,
                    RefreshTokenLifeTime = 7200
                },
                new ApplicationClient
                {
                    Id = "mobileStartUpApp",
                    Secret= Helper.GetHash("<startup-mobile></startup-mobile>"),
                    Name="Mobile Application",
                    ApplicationType =  ApplicationTypes.NativeConfidential,
                    Active = true,
                    RefreshTokenLifeTime = 7200
                }
            };
            if (!_context.ApplicationClient.Any())
                _context.ApplicationClient.AddRange(ClientsList);
        }
    }
}
