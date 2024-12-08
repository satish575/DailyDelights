using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Identity;
using DailyDelights.Validators;
namespace DailyDelights.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    [Required]
    public String? Gender {get; set;}
    [Required]
    [DateOfBirthValidator(ErrorMessage = "You must be at least 18 years old.")]
    public DateTime DOB {get; set;}


    public string? ImageUrl {get; set;}
}
