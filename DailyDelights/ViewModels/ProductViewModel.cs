using System;
using System.ComponentModel.DataAnnotations;
using DailyDelights.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DailyDelights.ViewModels;

public class ProductViewModel
{
    
    public ProductDTO? ProductDTO {get; set;}
    public List<SelectListItem>? Categories {get; set;}
}
