using System;
using DailyDelights.Models;

namespace DailyDelights.ViewModels;

public class OrderViewModel
{
   
    public List<CartViewModel>  products {get; set;}
    public int totalAmount {get; set;}
    public DateTime orderDate{get; set;}

    
}
