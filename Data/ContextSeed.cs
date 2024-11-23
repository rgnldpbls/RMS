using Microsoft.AspNetCore.Identity;
using ResearchManagementSystem.Models;
using ResearchManagementSystem.Enum;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ResearchManagementSystem.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Enum.Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enum.Roles.Faculty.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enum.Roles.Student.ToString()));
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "superadmin@gmail.com",
                Email = "superadmin@gmail.com",
                FirstName = "Ash",
                MiddleName = "RMO",
                LastName = "RMIS",
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user != null)
            {
                // Delete existing user
                await userManager.DeleteAsync(user);
            }

            // Recreate the user
            await userManager.CreateAsync(defaultUser, "123Pa$$word.");
            await userManager.AddToRoleAsync(defaultUser, Enum.Roles.Faculty.ToString());
            await userManager.AddToRoleAsync(defaultUser, Enum.Roles.Student.ToString());
            await userManager.AddToRoleAsync(defaultUser, Enum.Roles.SuperAdmin.ToString());
        }
    }
}