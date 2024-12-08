using System;
using Stripe;

namespace DailyDelights.ViewModels;

public class DashboardViewModel
{
    public List<LastFiveDaysSales> lastFiveDaysSales{get; set;}
    public List<MaxSoldProduct> maxSoldProducts {get; set;}
}


public class LastFiveDaysSales{
   public DateTime Date {get; set;}
   public int TotalAmount{get; set;} 
}

public class MaxSoldProduct{
     public string? ProductName  {get; set;}
     public int  TotalQuantity  {get; set;}
}