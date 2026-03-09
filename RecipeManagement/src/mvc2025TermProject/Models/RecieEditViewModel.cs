using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class RecipeEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? Name { get; set; }
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 5)]
        public string? Instructions { get; set; }

        public string? Tips { get; set; }
        [Range(1, 1020, ErrorMessage = "Preparation time must be between 1-1020 minutes")]
        [Display(Name = "Preparation time (Minutes)")]
        public int PrepTime { get; set; }

        [Display(Name = "Cook time (Minutes)")]
        [Range(1, 1020, ErrorMessage = "Cook time must be between 1-1020 minutes")]
        public int? CookTime { get; set; }

        [Display(Name = "Temperature (Degrees Fahrenheit)")]
        [Range(100, 500, ErrorMessage = "Temperature should be between 100\u00B0F ~ 500\u00B0F ")]
        public double? Temperature { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Servings must be at least 1")]
        public int Servings { get; set; }
        public ICollection<RecipeIngredientInfo>? SelectedIngredients { get; set; } = new List<RecipeIngredientInfo>();

    }
}