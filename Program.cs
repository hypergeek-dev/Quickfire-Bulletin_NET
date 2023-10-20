using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quickfire_Bulletin.Areas.Identity.Data;
using Quickfire_Bulletin.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Data.SqlClient;
using edu.stanford.nlp.pipeline;


namespace Quickfire_Bulletin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs.txt", restrictedToMinimumLevel: LogEventLevel.Verbose)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<MyAppSettings>(builder.Configuration.GetSection("MyAppSettings"));

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("MemberOrAdmin", policy => policy.RequireRole("Member", "Admin"));
            });

            var connectionString = builder.Configuration.GetConnectionString("ContextConnection")
                                   ?? throw new InvalidOperationException("Connection string 'ContextConnection' not found.");

            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<Quickfire_BulletinUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<Context>();

            builder.Services.AddTransient<NewsService>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();  // Added RazorPages to services

            var host = builder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Loaded Connection String: {ConnectionString}", connectionString);

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    logger.LogInformation("Successfully connected to SQL database.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while connecting to SQL database.");
            }

            var jarRoot = @"\Models\stanford-corenlp-4.5.5-models.jar";
            var props = new java.util.Properties();
            props.setProperty("annotators", "tokenize, ssplit");
            var pipeline = new StanfordCoreNLP(props);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var newsService = services.GetRequiredService<NewsService>();
                var userManager = services.GetRequiredService<UserManager<Quickfire_BulletinUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                try
                {
                    await newsService.SeedDatabaseAsync();
                    await CreateAdminUserAndRole(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            if (!host.Environment.IsDevelopment())
            {
                host.UseExceptionHandler("/Home/Error");
                host.UseHsts();
            }

            host.UseHttpsRedirection();
            host.UseStaticFiles();
            host.UseRouting();
            host.UseAuthentication();
            host.UseAuthorization();

            host.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            host.Run();
        }

        private static async Task CreateAdminUserAndRole(UserManager<Quickfire_BulletinUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string adminEmail = "admin@example.com";
            string adminPassword = "AdminPassword!1";

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                var adminUser = new Quickfire_BulletinUser
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
