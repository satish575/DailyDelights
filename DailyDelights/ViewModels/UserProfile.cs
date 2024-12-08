using System;

namespace DailyDelights.ViewModels;

public class UserProfile
{
    public Guid Id { get; set; }
    public string Gender { get; set; }
    public DateTime DOB { get; set; }
    public string UserName { get; set; }
    public string NormalizedUserName { get; set; }
    public string Email { get; set; }
    public string NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; }
    public string ImagePath { get; set; }  // Placeholder for user image
}

