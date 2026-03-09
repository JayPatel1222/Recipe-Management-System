using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class ImageAddViewModel
    {

        [Required]
        [StringLength(2000, MinimumLength = 5)]
        public string? Description { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 5)]
        [Display(Name = "Alternative Text")]
        public string? AltText { get; set; }

        [Required]
        [Display(Name = "Related Recipe")]
        public int? RecipeID { get; set; }
    }
}
