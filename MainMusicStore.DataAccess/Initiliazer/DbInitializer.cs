using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using MainMusicStore.DataAccess.Data;
using MainMusicStore.Models.DbModels;
using MainMusicStore.Utility;
using Microsoft.EntityFrameworkCore;

namespace MainMusicStore.DataAccess.Initiliazer
{
    public class DbInitializer : IDbInitiliazer
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer( UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, MainMusicStoreDbContext mainMusicStoreDbContext)
        {
            
            _roleManager = roleManager;
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
            _userManager = userManager;
        }

        public void Initiliaze()
        {
            try
            {
                if (_mainMusicStoreDbContext.Database.GetPendingMigrations().Any())
                {
                    _mainMusicStoreDbContext.Database.Migrate();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            if (_mainMusicStoreDbContext.Roles.Any(r => r.Name == ProjectConstant.RoleAdmin)) return;

            _roleManager.CreateAsync(new IdentityRole(ProjectConstant.RoleAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(ProjectConstant.RoleEmployee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(ProjectConstant.RoleUserComp)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(ProjectConstant.RoleUserIndi)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "emrullah04",
                Email = "emrullah04@outlook.com",
                EmailConfirmed = true,
                Name = "Emrullah IŞIK"
            }, "Admin123.").GetAwaiter().GetResult();

            ApplicationUser user = _mainMusicStoreDbContext.ApplicationUsers.FirstOrDefault(u => u.Email == "emrullah04@outlook.com");

            _userManager.AddToRoleAsync(user, ProjectConstant.RoleAdmin).GetAwaiter().GetResult();
        }
    }
}
