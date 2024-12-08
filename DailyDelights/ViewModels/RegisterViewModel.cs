using System;
using System.ComponentModel.DataAnnotations;
using DailyDelights.Validators;
namespace DailyDelights.ViewModels;

public class RegisterViewModel
{

    [Required(ErrorMessage = "Username is required")]
    public string? Username {get; set;}
    [Required(ErrorMessage ="Password is required")]
    public string? Password {get; set;}
    [Required(ErrorMessage ="Email is required")]
    [EmailAddress(ErrorMessage ="Enter valid email address")]
    public string? Email {get; set;}

    [Required(ErrorMessage ="Confirm Password is required")]
   [Compare("Password",ErrorMessage ="Password and confirm password should match")]
    public string? ConfirmPassword {get; set;}

    
    public string? Role {get; set;} = "Customer";

    [Required(ErrorMessage ="Gender is required")]
    public string? Gender {get; set;}

    [Required(ErrorMessage ="Date of birth is required")]
    [DateOfBirthValidator]
    public DateTime DOB {get; set;}
}
