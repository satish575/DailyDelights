using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DailyDelights.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } // Foreign key to the User (Assuming you're using Identity or custom user model)
    [ForeignKey("UserId")]
    public ApplicationUser User {get; set;}
    public DateTime OrderDate { get; set; }
    public int TotalAmount { get; set; } // Total amount for the order

    public Boolean IsPain {get; set;} =false;

    public Boolean IsDelivered {get; set;} =false;
    // Navigation property
    public virtual ICollection<OrderItem> OrderItems { get; set; }
}

