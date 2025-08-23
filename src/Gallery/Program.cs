using Gallery.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Gallery;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Build services.
        Configuration configuration = Configuration.ImportFromEnvironmentVariables();
        builder.Services.AddSingleton(configuration);

        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContextFactory<GalleryContext>(options =>
        {
            options.UseNpgsql(configuration.DatabaseConnectionString);
        });

        builder.Services
            .AddIdentityCore<User>()
            .AddRoles<Role>()
            .AddSignInManager()
            .AddEntityFrameworkStores<GalleryContext>();

        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

        // Build application
        WebApplication app = builder.Build();

        await app.InitialiseDatabaseAsync();

        Directory.CreateDirectory(configuration.ImageDirectory);
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(configuration.ImageDirectory), RequestPath = "/images"
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        await app.RunAsync();
    }
}
