using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace DailyDelights.Validators
{
    public class ProductImageValidator : ValidationAttribute
    {
        // Define the allowed extensions
        private readonly string[] AllowedExtensions = { ".jpg", ".png", ".jpeg", ".gif", ".webp" };
        // Max file size in bytes (5MB)
        private const int MaxFileSize = 5 * 1024 * 1024;
        // Min file size in bytes (1 byte)
        private const int MinFileSize = 1;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Ensure the file is provided
            IFormFile image = value as IFormFile;
            if (image == null)
            {
                return ValidationResult.Success;
            }

            // Validate the file size (between 1 byte and 5MB)
            if (image.Length > MaxFileSize || image.Length < MinFileSize)
            {
                return new ValidationResult("Image size should be between 1 byte and 5MB.");
            }

            // Validate the file extension
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return new ValidationResult("Only .jpg, .png, .jpeg, .gif extensions are allowed.");
            }

            // If everything is valid, return success
            return ValidationResult.Success;
        }
    }
}
