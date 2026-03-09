using Microsoft.AspNetCore.Identity;
using mvc2025TermProject.Models;

namespace mvc2025TermProject.Data
{
    public enum Roles
    {
        Administrator,
        Contributor
    }
    public static class ContextSeed
    {
        public async static Task SeedRolesAsync(UserManager<CustomUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Contributor.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Administrator.ToString()));

            return;
        }
    }
}
