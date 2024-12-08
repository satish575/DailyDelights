using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyDelights.Models;

public class Product
{

    public Guid Id {get; set;}
    [Required(ErrorMessage ="Product Title is required")]
    public string? Title {get; set;}
    [Required(ErrorMessage ="Product Description is required")]
    public string? Description {get; set;}
    [Required(ErrorMessage ="Product Price is required")]
    public int Price {get; set;}
    [Required(ErrorMessage ="Product Image url is required")]
    public string? ImageUrl {get; set;}
    
    public int Stock {get; set;} =0;
    public Guid? CategoryId {get; set;} =null;

    [ForeignKey("CategoryId")]
    public Category? Category {get; set;} // navigation property

    public DateTime CreatedAt{get; set;} = DateTime.Now;
    
    public double Ratting {get; set;}=0.0;
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}
