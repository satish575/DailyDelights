using System;
using System.ComponentModel.DataAnnotations;

namespace DailyDelights.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage ="Username is required")]
    public string? Username {get; set;}
    [Required(ErrorMessage ="Password is rquired")]
    public string? Password {get; set;}

    public string? ReturnUrl {get; set;}
}
