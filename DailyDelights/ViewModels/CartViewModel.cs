using System;

namespace DailyDelights.ViewModels;

public class CartViewModel
{
    public string productId {get; set;}
    public string productTitle {get; set;}
    public string productImage {get; set;}
    public int   productPrice {get; set;}
   
    public int  noOfItem {get; set;}
}
