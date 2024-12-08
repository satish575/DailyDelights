using System;
using DailyDelights.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DailyDelights.ViewModels;

public class ProductUpdateViewModel
{
     public ProductUpdateDTO? ProductDTO {get; set;}
    public List<SelectListItem>? Categories {get; set;}
}
