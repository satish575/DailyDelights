using System;
using System.ComponentModel.DataAnnotations;

namespace DailyDelights.Validators;

public class DateOfBirthValidator : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return new ValidationResult("Date of birth is required.");
        }

        if (value is DateTime date)
        {
            int age = DateTime.Now.Year - date.Year;
            
          
           
            if (age < 18)
            {
                return new ValidationResult("You must be at least 18 years old.");
            }
        }
        else
        {
            return new ValidationResult("Invalid date format.");
        }

        return ValidationResult.Success;
    }
}

