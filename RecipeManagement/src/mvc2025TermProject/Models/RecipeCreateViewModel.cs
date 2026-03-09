using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class RecipeCreateViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Name { get; set; }

        public RecipeStatus Status { get; set; }

        [Display(Name="Category")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 5)]
        public string? Instructions { get; set; }

        public string? Tips { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Preparation time must be greater than zero")]
        [Display(Name = "Preparation time (minutes)")]
        public int PrepTime { get; set; }

        [Display(Name = "Cook time (minutes)")]
        [Range(1, int.MaxValue, ErrorMessage = "Cooking time must be greater than zero")]
        public int? CookTime { get; set; }

        [Display(Name = "Temperature (Fahrenheit)")]
        [Range(-500, 800, ErrorMessage = "Temperature should be between -500\u00B0F ~ 800\u00B0F ")]
        public double? Temperature { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Servings must be at least 1")]
        public int Servings { get; set; }

        [Required(ErrorMessage ="Add at least one ingredient")]
        [MinLength(1)]
        public ICollection<RecipeIngredientInfo>? SelectedIngredients { get; set; }

        public RecipeNutrition? Nutrition { get; set; }

        [MaxLength(7)]
        public ICollection<Image>? Images { get; set; } = new List<Image>();
    }

    public class RecipeIngredientInfo
    {
     
        public int IngredientID { get; set; }
        public int IngredientTypeID { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public double Quantity { get; set; }
        public UnitType Unit { get; set; }

    }
}
