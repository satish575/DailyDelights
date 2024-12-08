using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DailyDelights.Models;

public class OrderItem
{
    public Guid Id{get; set;}
    public Guid OrderId { get; set; } // Foreign key to Order
    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    public Guid ProductId { get; set; } // Foreign key to Product
    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    public int Quantity { get; set; } // Quantity of the product in this order

    
}

