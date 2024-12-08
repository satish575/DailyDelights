using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DailyDelights.Models;


[Index(nameof(CategoryName),IsUnique =true)]
public class Category
{
    public Guid Id {get; set;}
    [Required(ErrorMessage ="Category Name is required.")]
     public string? CategoryName {get; set;}

}
