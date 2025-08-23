using Gallery.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gallery;

public static class DatabaseInitialiser
{
    public static async Task<IApplicationBuilder> InitialiseDatabaseAsync(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        {
            // TODO: Make this use migrations if production
            IDbContextFactory<GalleryContext> contextFactory =
                scope.ServiceProvider.GetRequiredService<IDbContextFactory<GalleryContext>>();
            await using GalleryContext context = await contextFactory.CreateDbContextAsync();
            await context.Database.EnsureCreatedAsync();
        }

        {
            IDbContextFactory<GalleryContext> contextFactory =
                scope.ServiceProvider.GetRequiredService<IDbContextFactory<GalleryContext>>();
            await using GalleryContext context = await contextFactory.CreateDbContextAsync();

            using UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            using RoleManager<Role> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            // Add roles.
            string adminRole = "Admin";
            string[] roles = new[] { adminRole };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Role { Name = role });
                }
            }

            // Add admin user.
            string adminUsername = "admin";

            User? userExist = await userManager.FindByNameAsync(adminUsername);
            if (userExist == null)
            {
                User adminUser = new() { UserName = adminUsername };

                await userManager.CreateAsync(adminUser, "Password1@");
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

        return app;
    }
}
