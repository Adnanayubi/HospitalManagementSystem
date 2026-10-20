using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagementSystem.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ============================================
            // CREATE ROLES
            // ============================================

            string[] roles =
            {
                "Admin",
                "Doctor",
                "Receptionist"
            };
           // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult =
                        await roleManager.CreateAsync(
                            new IdentityRole(role));

                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(
                            "; ",
                            roleResult.Errors.Select(e => e.Description));

                        throw new Exception(
                            "Role creation failed: " + errors);
                    }
                }
            }

            // ============================================
            // ADMIN ACCOUNT
            // ============================================

            string adminEmail = "admin@hospital.com";
            string adminPassword = "Admin@123";

            var admin =
                await userManager.FindByEmailAsync(adminEmail);

            // ============================================
            // CREATE ADMIN IF NOT EXISTS
            // ============================================

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Hospital Administrator",
                    RoleDescription = "System Administrator",
                    EmailConfirmed = true
                };

                var result =
                    await userManager.CreateAsync(
                        admin,
                        adminPassword);

                if (!result.Succeeded)
                {
                    var errors =
                        string.Join(
                            "; ",
                            result.Errors.Select(
                                e => e.Description));

                    throw new Exception(
                        "Admin creation failed: " + errors);
                }
            }
            else
            {
                // ========================================
                // RESET ADMIN PASSWORD
                // ========================================

                var token =
                    await userManager.GeneratePasswordResetTokenAsync(
                        admin);

                var passwordResult =
                    await userManager.ResetPasswordAsync(
                        admin,
                        token,
                        adminPassword);

                if (!passwordResult.Succeeded)
                {
                    var errors =
                        string.Join(
                            "; ",
                            passwordResult.Errors.Select(
                                e => e.Description));

                    throw new Exception(
                        "Admin password reset failed: "
                        + errors);
                }

                // Make sure email is confirmed
                if (!admin.EmailConfirmed)
                {
                    admin.EmailConfirmed = true;

                    await userManager.UpdateAsync(admin);
                }
            }

            // ============================================
            // ASSIGN ADMIN ROLE
            // ============================================

            if (!await userManager.IsInRoleAsync(
                    admin,
                    "Admin"))
            {
                var roleResult =
                    await userManager.AddToRoleAsync(
                        admin,
                        "Admin");

                if (!roleResult.Succeeded)
                {
                    var errors =
                        string.Join(
                            "; ",
                            roleResult.Errors.Select(
                                e => e.Description));

                    throw new Exception(
                        "Admin role assignment failed: "
                        + errors);
                }
            }
        }
    }
}