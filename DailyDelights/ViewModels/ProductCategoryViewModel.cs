using System;
using DailyDelights.Models;

namespace DailyDelights.ViewModels;

public class ProductCategoryViewModel
{
 public List<Category> Categories { get; set; }
    public List<Product> Products { get; set; }
    public string Category { get; set; }
    public int Page { get; set; }
    public string SortByPrice { get; set; }
    public string SearchQuery { get; set; } // New field

}
