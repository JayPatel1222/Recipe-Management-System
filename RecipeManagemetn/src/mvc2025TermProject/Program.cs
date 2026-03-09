/*
 * NAME:    Team10 - Jay Patel & Vianey Gandara
 * CLASS:   Software Developer Sr. B 
 * DATE:    11/30/2025
 * PURPOSE: Project Term SSW-MVC - The Recipe Town Website
 */


using EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;

namespace mvc2025TermProject
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("RecipesConnection2")
                ?? throw new InvalidOperationException("Connection string 'RecipesConnection' not found.");

            var emailConfig = builder.Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddScoped<IEmailSender, EmailSeneder>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options
                .UseLazyLoadingProxies(useLazyLoadingProxies: true)
                .UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services
                .AddDefaultIdentity<CustomUser>(options => {
                    options.SignIn.RequireConfirmedAccount = true;

                    // added
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365 * 150);
                    options.Lockout.MaxFailedAccessAttempts = 3;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Retrieving services * Add roles * 
            using (var scope = app.Services.CreateScope())
            {

                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<CustomUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await ContextSeed.SeedRolesAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, $"An error ocurred during Seeding the DB.");
                }

            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); // added
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Recipes}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
