using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyDelights.Models;

public class ProductReview
{
 public Guid Id {get; set;}
 
 public Guid ProductId { get; set; }
[ForeignKey("ProductId")]
 public Product Product {get; set;}

 public Guid UserId {get; set;}

[ForeignKey("UserId")]
 public ApplicationUser User {get; set;}
 public double Rating { get; set; }
}
