using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public enum UnitType
    {
        g,
        kg,
        ml,
        L,
        tsp,
        tbsp,
        cup,
        pc,
        oz,
        lb
    }
    public enum RecipeStatus
    {
        Draft,
        Public,
        Private,
        Unlisted
    }
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public RecipeStatus Status { get; set; }
        public virtual StandardUser? User { get; set; }

        [MaxLength(7)]
        public virtual ICollection<Image>? Images { get; set; } = new List<Image>();

        [Required]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 5)]
        public string? Instructions { get; set; }

        public string? Tips { get; set; }
        [Display(Name="Ingredients")]
        public virtual ICollection<RecipeIngredient>? RecipeIngredients { get; set; } = new List<RecipeIngredient>();

        [Display(Name = "Temperature (Fahrenheit)")]
        [Range(-500, 800, ErrorMessage = "Temperature should be between -500\u00B0F ~ 800\u00B0F ")]
        public double? Temperature { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Servings must be at least 1")]
        public int Servings { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Preparation time must be greater than zero")]
        [Display(Name = "Preparation time")]
        public int PrepTime { get; set; }

        [Display(Name = "Cook time")]
        [Range(1, int.MaxValue, ErrorMessage = "Cooking time must be greater than zero")]
        public int? CookTime { get; set; }

        public virtual RecipeNutrition? Nutrition { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? CreationDate { get; set; }

        [NotMapped]
        public string FormattedPrepTime
        {
            get
            {

                TimeSpan minutes = TimeSpan.FromMinutes(this.PrepTime);

                string totalTime = minutes.Days > 0 ? $"{minutes.Days}d " : "";
                totalTime += minutes.Hours > 0 ? $"{minutes.Hours}h " : "";
                totalTime += $"{minutes.Minutes}m";

                return totalTime;
            }
        }

        [NotMapped]
        public string FormattedCookTime
        {
            get
            {
                if (this.CookTime != null)
                {
                    TimeSpan minutes = TimeSpan.FromMinutes((double)this.CookTime);

                    string totalTime = minutes.Days > 0 ? $"{minutes.Days}d " : "";
                    totalTime += minutes.Hours > 0 ? $"{minutes.Hours}h " : "";
                    totalTime += $"{minutes.Minutes}m";

                    return totalTime;
                } else
                {
                    return "";
                }
               
            }
        }


    }
    public class Ingredient
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        [Display(Name="Ingredient Type")]
        [Required]
        public int IngredientTypeID { get; set; }
        public virtual IngredientType? Type { get; set; }
        public string? Details { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public string? NewName { get; set; }
        public string? NewDetails { get; set; }
        public bool Approved { get; set; } // made it default to TRUE 
    }
    public class IngredientType
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }
        public virtual ICollection<Ingredient>? Ingredients { get; set; }
    }
    public class RecipeIngredient
    {
        public int ID { get; set; }
        [Required]
        public int RecipeId { get; set; }
        public virtual Recipe? Recipe { get; set; }
        [Required]
        public int IngredientId { get; set; }
        public virtual Ingredient? Ingredient { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public double Quantity { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(20)")] 
        public UnitType Unit { get; set; }
    }

    public class RecipeNutrition
    {
        public int RecipeNutritionID { get; set; }

        public int RecipeID { get; set; }

        public virtual Recipe? Recipe { get; set; }
        public int? Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Carbs { get; set; }
        public decimal? Fat { get; set; }
    }
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Name { get; set; }
        public string? Details { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public string? NewName { get; set; }
        public string? NewDetails { get; set; }
        public bool Approved { get; set; } // made it default to TRUE 
    }
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? Path { get; set; }  

        [Required]
        public string? Name { get; set; } 

        [Required]
        [StringLength(2000, MinimumLength = 5)]
        public string? Description { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 5)]
        [Display(Name = "Alternative Text")]
        public string? AltText { get; set; }

        [Required]
        public DateTime UploadedDate { get; set; } 

        [Required]
        [Display(Name = "Related Recipe")]
        public int? RecipeID { get; set; }

        public virtual Recipe? Recipe { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
