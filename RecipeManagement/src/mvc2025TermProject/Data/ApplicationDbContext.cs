using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Models;

namespace mvc2025TermProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<CustomUser>
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<IngredientType> IngredientTypes { get; set; }
        public DbSet<RecipeNutrition> RecipeNutritions { get; set; }
        public DbSet<Image>? Images { get; set; }
        public DbSet<CustomUser>? StandardUserAccounts { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<CustomUser>()
                .HasDiscriminator<string>("AccountType")
                .HasValue<CustomUser>("Unassigned")
                .HasValue<StandardUser>("Standard");
        }
    }
}
