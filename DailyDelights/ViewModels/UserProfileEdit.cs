using System;
using System.ComponentModel.DataAnnotations;
using DailyDelights.Validators;
namespace DailyDelights.ViewModels;

public class UserProfileEdit
{
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [DateOfBirthValidator]
        
        public DateTime DOB { get; set; }

        [Display(Name = "Profile Image")]
        public string? ImagePath { get; set; }

        [Required(ErrorMessage ="image is required")]
        public IFormFile image {get;set;}
}
