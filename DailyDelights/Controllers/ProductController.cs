using System;
using System.Net;
using DailyDelights.DatabaseContext;
using DailyDelights.Models;
using DailyDelights.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DailyDelights.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    public ProductController(ApplicationDbContext applicationDbContext,UserManager<ApplicationUser> userManager){
        _dbContext=applicationDbContext;
        _userManager=userManager;
    }
    public async Task<IActionResult> GetProducts(String Category="All",int page=1,String SortByPrice=""){

        int productPerPage=6;
        int limit=productPerPage*page;
        int skip=(page-1) * productPerPage;
        List<Product> products=new List<Product>();
       if(Category=="Suggestion"){
      if(User.Identity.IsAuthenticated){
          var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
        // Get all user orders grouped by UserId, including average order and product values
        var userOrders = await _dbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .GroupBy(o => o.UserId)
            .Select(group => new 
            {
                UserId = group.Key, // Group orders by UserId
                Gender = group.FirstOrDefault().User.Gender, // Fetch gender of the user (assuming you have a User reference in Orders)
                AverageOrderValue = group.Average(o => o.TotalAmount),
                AverageProductValue = group.SelectMany(o => o.OrderItems)
                                            .Average(oi => oi.Product.Price)
            })
            .ToListAsync();

        // Map the orders data to Orders model
        List<Orders> userOrder = new List<Orders>();
        foreach(var userorder in userOrders){
            userOrder.Add(new Orders(userorder.UserId, 
                                     userorder.Gender == "male" ? 0 : 1, 
                                     userorder.AverageOrderValue, 
                                     userorder.AverageProductValue));
        }

        // Get the cluster for the authenticated user
        List<Orders> cluster = KMeansCluster.GetCluster(userOrder, user.Id);

        // Get all the products ordered by users in the same cluster
        var similarUsersProductIds = new HashSet<Guid>(); // To store unique product ids ordered by similar users

        foreach (var order in cluster)
        {
            // Get the orders for each user in the cluster
            var userOrdersInCluster = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == order.OriginalId)
                .ToListAsync();

            // Add products from these orders to the list
            foreach (var userOrderInCluster in userOrdersInCluster)
            {
                foreach (var orderItem in userOrderInCluster.OrderItems)
                {
                    similarUsersProductIds.Add(orderItem.ProductId); // Assuming ProductId is the unique identifier for products
                }
            }
        }

        // Get the list of products that were ordered by similar users
        var suggestedProducts = await _dbContext.Products
            .Where(p => similarUsersProductIds.Contains(p.Id)) // Filter products based on the ordered product ids
            .Skip(skip)
            .Take(limit)
            .ToListAsync();

        // Now, suggestedProducts contains the products ordered by similar users
        products = suggestedProducts; // Assign to the products list to be displayed in your view
    }
    else{
        return RedirectToAction("Login", "Account");
    }
        }
        else if(Category=="All"){
            if(SortByPrice==""){
              products = _dbContext.Products.Skip(skip).Take(limit).ToList();
            }else{
                if(SortByPrice=="desc"){
                    products = _dbContext.Products.OrderByDescending(p=>p.Price).Skip(skip).Take(limit).ToList();
                }else if(SortByPrice=="asc"){
                     products = _dbContext.Products.OrderByDescending(p=>p.Price).Skip(skip).Take(limit).ToList();
                }
            }
            
        }else{
            if(SortByPrice==""){
              products= _dbContext.Products.Where(c=>c.Category.CategoryName==Category).Skip(skip).Take(limit).ToList();
            }else{
                if(SortByPrice=="desc"){
                    products= _dbContext.Products.OrderByDescending(p=>p.Price).Where(c=>c.Category.CategoryName==Category).Skip(skip).Take(limit).ToList();
                }else if(SortByPrice=="asc"){
                    products= _dbContext.Products.Where(c=>c.Category.CategoryName==Category).OrderBy(p=>p.Price).Skip(skip).Take(limit).ToList();
                }
            }

             
        }


        var Categories = _dbContext.Categories.ToList();
       
        var ProductCategoryViewModel=new ProductCategoryViewModel(){
            Categories=Categories,
            Products=products
        };

        ProductCategoryViewModel.Category=Category;
        ProductCategoryViewModel.Page=page;
        ProductCategoryViewModel.SortByPrice=SortByPrice;
        return View(ProductCategoryViewModel);
    }


//     public async Task<IActionResult> GetProducts(string SearchQuery = "", string Category = "All", int page = 1, string SortByPrice = "")
// {
//     int productsPerPage = 6;
//     int limit = productsPerPage * page;
//     int skip = (page - 1) * productsPerPage;
//     List<Product> products = new List<Product>();

//     // Search functionality
//     if (!string.IsNullOrEmpty(SearchQuery))
//     {
//         SearchQuery = SearchQuery.ToLower(); // Make the search case-insensitive
//     }

//     if (Category == "Suggestion")
//     {
//         if (User.Identity.IsAuthenticated)
//         {
//             var user = await _userManager.FindByNameAsync(User.Identity.Name);
        
//             // Fetch user orders with average values
//             var userOrders = await _dbContext.Orders
//                 .Include(o => o.OrderItems)
//                 .ThenInclude(oi => oi.Product)
//                 .GroupBy(o => o.UserId)
//                 .Select(group => new 
//                 {
//                     UserId = group.Key,
//                     Gender = group.FirstOrDefault().User.Gender,
//                     AverageOrderValue = group.Average(o => o.TotalAmount),
//                     AverageProductValue = group.SelectMany(o => o.OrderItems)
//                                                .Average(oi => oi.Product.Price)
//                 })
//                 .ToListAsync();

//             // Map to Orders model for clustering
//             var userOrderList = userOrders.Select(uo => new Orders(uo.UserId, 
//                                                                   uo.Gender == "male" ? 0 : 1, 
//                                                                   uo.AverageOrderValue, 
//                                                                   uo.AverageProductValue)).ToList();

//             // Get the cluster for the authenticated user
//             var cluster = KMeansCluster.GetCluster(userOrderList, user.Id);

//             // Get products ordered by similar users in the cluster
//             var similarUsersProductIds = new HashSet<Guid>();
//             foreach (var order in cluster)
//             {
//                 var userOrdersInCluster = await _dbContext.Orders
//                     .Include(o => o.OrderItems)
//                     .ThenInclude(oi => oi.Product)
//                     .Where(o => o.UserId == order.OriginalId)
//                     .ToListAsync();

//                 foreach (var userOrderInCluster in userOrdersInCluster)
//                 {
//                     foreach (var orderItem in userOrderInCluster.OrderItems)
//                     {
//                         similarUsersProductIds.Add(orderItem.ProductId);
//                     }
//                 }
//             }

//             // Fetch recommended products
//             products = await _dbContext.Products
//                 .Where(p => similarUsersProductIds.Contains(p.Id) && 
//                            (string.IsNullOrEmpty(SearchQuery) || p.Title.ToLower().Contains(SearchQuery) || p.Description.ToLower().Contains(SearchQuery)))
//                 .Skip(skip)
//                 .Take(limit)
//                 .ToListAsync();
//         }
//         else
//         {
//             return RedirectToAction("Login", "Account");
//         }
//     }
//     else if (Category == "All")
//     {
//         if (SortByPrice == "")
//         {
//             products = await _dbContext.Products
//                 .Where(p => string.IsNullOrEmpty(SearchQuery) || p.Title.ToLower().Contains(SearchQuery) || p.Description.ToLower().Contains(SearchQuery))
//                 .Skip(skip)
//                 .Take(limit)
//                 .ToListAsync();
//         }
//         else if (SortByPrice == "desc")
//         {
//             products = await _dbContext.Products
//                 .Where(p => string.IsNullOrEmpty(SearchQuery) || p.Title.ToLower().Contains(SearchQuery) || p.Description.ToLower().Contains(SearchQuery))
//                 .OrderByDescending(p => p.Price)
//                 .Skip(skip)
//                 .Take(limit)
//                 .ToListAsync();
//         }
//         else if (SortByPrice == "asc")
//         {
//             products = await _dbContext.Products
//                 .Where(p => string.IsNullOrEmpty(SearchQuery) || p.Title.ToLower().Contains(SearchQuery) || p.Description.ToLower().Contains(SearchQuery))
//                 .OrderBy(p => p.Price)
//                 .Skip(skip)
//                 .Take(limit)
//                 .ToListAsync();
//         }
//     }
//     else
//     {
//         if (SortByPrice == "")
//         {
//             products = await _dbContext.Products
//                 .Where(c => c.Category.CategoryName == Category &&
//                            (string.IsNullOrEmpty(SearchQuery) || c.Title.ToLower().Contains(SearchQuery) || c.Description.ToLower().Contains(SearchQuery)))
//                 .Skip(skip)
//                 .Take(limit)
//                 .ToListAsync();
//         }
//         else if (SortByPrice == "desc")
//         {
//             products = await _dbContext.Products
//                 .Where(c => c.Category.CategoryName == Category &&
//                            (string.IsNullOrEmpty(SearchQuery) || c.Title.ToLower().Contains(SearchQuery) || c.Description.ToLower().Contains(SearchQuery)))
//                 .OrderByDescending(p => p.Price)
//                 .Skip(skip)
//                 .Take(limit)
//                 .ToListAsync();
//         }
//         else if (SortByPrice == "asc")
//         {
//             products = await _dbContext.Products
//                 .Where(c => c.Category.CategoryName == Category &&
//                            (string.IsNullOrEmpty(SearchQuery) || c.Title.ToLower().Contains(SearchQuery) || c.Description.ToLower().Contains(SearchQuery)))
//                 .OrderBy(p => p.Price)
//                 .Skip(skip)
//                 .Take(limit)
//                 .ToListAsync();
//         }
//     }

//     // Fetch categories asynchronously to improve page load performance
//     var categories = await _dbContext.Categories.ToListAsync();
    
//     var ProductCategoryViewModel = new ProductCategoryViewModel()
//     {
//         Categories = categories,
//         Products = products,
//         Category = Category,
//         Page = page,
//         SortByPrice = SortByPrice
//     };

//     return View(ProductCategoryViewModel);
// }

    public IActionResult Create(){
        return View();
    }

    public IActionResult Details(Guid id){
        var product = _dbContext.Products
                            .Include(p => p.Category)
                            .FirstOrDefault(p => p.Id == id);
       if(product==null){
         return Redirect("Error/404");
       }
       return View(product);
    }
    
}
