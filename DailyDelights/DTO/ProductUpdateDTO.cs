using System;
using System.ComponentModel.DataAnnotations;
using DailyDelights.Validators;
namespace DailyDelights.DTO;

public class ProductUpdateDTO
{
 public Guid Id {get; set;}
     [Required(ErrorMessage ="Product Title is required")]
    public string? Title {get; set;}
    [Required(ErrorMessage ="Product description is required")]
    public string? Description {get; set;}
    [Required(ErrorMessage ="Product Price is required")]
    public int Price {get; set;}
    
   [ProductImageValidator]
    public IFormFile? Image {get; set;}

    public string? ImageUrl {get;set;}
    [Required(ErrorMessage ="Product stock is required")]
    public int Stock {get; set;}
    [Required(ErrorMessage ="Product category is required")]
    public Guid CategoryId {get; set;}
}
