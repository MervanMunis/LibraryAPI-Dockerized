using LibraryAPI.Models.Entities;
using LibraryAPI.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Auth.Controller
{
    public class RoleInitializer
	{
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetValues(typeof(Title)))
            {
                if (await roleManager.FindByNameAsync(role.ToString()) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }
            }

            // Add the "Member" role
            if (await roleManager.FindByNameAsync("Member") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Member"));
            }

            // Example admin user
            string adminEmail = "admin@admin.com";
            string password = "Admin1234!";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Title.HeadOfLibrary.ToString());
                }
            }
        }
    }
}

