using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class CustomUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Names must be 2-50 characters")]
        public string? FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last Names must be 2-50 characters")]
        public string? LastName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Middle Names must be 2-50 characters")]
        public string? MiddleName { get; set; }

        [Required]
        [Display(Name = "Street Address")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Street Address must be 2-200 characters")]
        public string? StreetAddress { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Municipality must be 2-100 characters")]
        public string? Municipality { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; } 


    }
    public class StandardUser : CustomUser
    {
        public virtual ICollection<Recipe>? Recipes { get; set; }
    }
}
