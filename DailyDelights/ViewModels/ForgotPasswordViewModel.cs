using System;
using System.ComponentModel.DataAnnotations;

namespace DailyDelights.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
