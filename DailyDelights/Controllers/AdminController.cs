using DailyDelights.DatabaseContext;
using DailyDelights.Models;
using DailyDelights.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DailyDelights.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _dbContext;


        public AdminController(ApplicationDbContext applicationDbContext){
            this._dbContext=applicationDbContext;
           
        }

        public IActionResult Index(){
            // To get last five days sales

            List<LastFiveDaysSales> lastFiveDaysSales = _dbContext.Orders
        .Where(o => o.OrderDate >= DateTime.Now.AddDays(-5)) // Filter for last 5 days
        .GroupBy(o => o.OrderDate.Date) // Group by the date part
        .Select(g => new LastFiveDaysSales()
        {

             Date = g.Key.Date,
             TotalAmount = g.Sum(o => o.TotalAmount) // Sum up the TotalAmount for each day
        })
        .OrderByDescending(s => s.Date) // Order by date (descending)
        .ToList();

        List<MaxSoldProduct> maxSoldProducts = _dbContext.OrderItems
    .Include(oi => oi.Product) // Include Product to access ProductName
    .GroupBy(oi => new { oi.ProductId, oi.Product.Title }) // Group by ProductId and ProductName
    .Select(g => new MaxSoldProduct()
    {
        ProductName = g.Key.Title,
        TotalQuantity = g.Sum(oi => oi.Quantity)
    })
    .OrderByDescending(ps => ps.TotalQuantity)
    .Take(6) // Order by total quantity in descending order
    .ToList();


        return View(new DashboardViewModel(){lastFiveDaysSales=lastFiveDaysSales,maxSoldProducts=maxSoldProducts});
        }
    #region Product
        [HttpGet]
        public IActionResult Products(string searchQuery=""){

           
            if(!String.IsNullOrEmpty(searchQuery)){
                    var SearchedProducts = _dbContext.Products
            .Include(p => p.Category)
            .Where(p => p.Title.ToLower().Contains(searchQuery.ToLower()))
            .ToList();
        return View(SearchedProducts);
            }
             var Products=_dbContext.Products.Include(p=>p.Category).ToList();
            // Products.Add(new Product(){Id=new Guid(),CategoryId=null,Title="Shirt",Description="Shirts",ImageUrl="https://images.pexels.com/photos/297933/pexels-photo-297933.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",Price=34,Stock=5});
            // Products.Add(new Product(){Id=new Guid(),CategoryId=null,Title="Pants",Description="Shirts",ImageUrl="https://images.pexels.com/photos/297933/pexels-photo-297933.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",Price=34,Stock=5});
            // Products.Add(new Product(){Id=new Guid(),CategoryId=null,Title="Goggles",Description="Shirts",ImageUrl="https://images.pexels.com/photos/297933/pexels-photo-297933.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",Price=34,Stock=5});
            // Products.Add(new Product(){Id=new Guid(),CategoryId=null,Title="Shoes",Description="Shirts",ImageUrl="https://images.pexels.com/photos/297933/pexels-photo-297933.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1",Price=34,Stock=5});

            return View(Products);
        }

        [HttpGet]
        public IActionResult AddProduct(){

            var ProductViewModel=new ProductViewModel(){
                ProductDTO=new DTO.ProductDTO(),
                Categories=_dbContext.Categories.Select(c=>new SelectListItem(){
                    Value=c.Id.ToString(),
                    Text=c.CategoryName
                }).ToList()
            };
            
            //TempData["Categories"]=ProductViewModel.Categories;
            return View(ProductViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductViewModel productViewModel){
            if(!ModelState.IsValid){
               var categoriesList=_dbContext.Categories.Select(c=>new SelectListItem(){
                Text=c.CategoryName,
                Value=c.Id.ToString()
            }).ToList();
            productViewModel.Categories=categoriesList;
            return View(productViewModel);
            }
          
          try{
            var UploadDir= Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images/productImages");
            if(!Directory.Exists(UploadDir)){
                Directory.CreateDirectory(UploadDir);
            }
            var UniqueName=Guid.NewGuid()+"_"+Path.GetFileName(productViewModel.ProductDTO.Image.FileName);

            var filePath=Path.Combine(UploadDir,UniqueName);
            var fileStrem=new FileStream(filePath,FileMode.Create);
            await productViewModel.ProductDTO.Image.CopyToAsync(fileStrem);

            Product product=new Product(){
                CategoryId=productViewModel.ProductDTO.CategoryId,
                Description=productViewModel.ProductDTO.Description,
                ImageUrl=UniqueName,
                Price=productViewModel.ProductDTO.Price,
                Stock=productViewModel.ProductDTO.Stock,
                Title=productViewModel.ProductDTO.Title,
            };

            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
           
            ViewBag.Success="Producted Added successfully";

            //productViewModel.Categories=TempData["Categories"] as List<SelectListItem>;
            var categories=_dbContext.Categories.Select(c=>new SelectListItem(){
                Text=c.CategoryName,
                Value=c.Id.ToString()
            }).ToList();
            productViewModel.Categories=categories;
            return View(productViewModel);
          }catch(Exception e){
            return RedirectToAction("ErrorPage",new{message=e.Message});
          }
        }

        public IActionResult DeleteProduct([FromRoute]Guid id){

            try{
             var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);

    // Check if the product exists
    if (product == null)
    {
        return RedirectToAction("ErrorPage",new {message="Product not found"});
    }

    // Remove the product from the database
    _dbContext.Products.Remove(product);

    // Save the changes to the database
    _dbContext.SaveChanges();

    // Return a success response
    return RedirectToAction("Products");
            }catch(Exception e){
                return RedirectToAction("ErrorPage",new {message="Internal server Error"});
            }
        }

        public IActionResult EditProduct(Guid id){
            try{
               var product = _dbContext.Products.Find(id);
                var ProductViewModel=new ProductUpdateViewModel(){
                ProductDTO=new DTO.ProductUpdateDTO(){ImageUrl=product.ImageUrl,
                CategoryId=(Guid)product.CategoryId,
                Description=product.Description,
                Id=product.Id,
                Stock=product.Stock,
                Price=product.Price,
                Title=product.Title},
                Categories=_dbContext.Categories.Select(c=>new SelectListItem(){
                    Value=c.Id.ToString(),
                    Text=c.CategoryName
                }).ToList()

                
            };
            return View(ProductViewModel);
            }catch(Exception e){
                return RedirectToAction("ErrorPage",new{message=e.Message});
            }   
        }
        
        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductUpdateViewModel productViewModel){
            try{
            if(!ModelState.IsValid){
               var categoriesList=_dbContext.Categories.Select(c=>new SelectListItem(){
                Text=c.CategoryName,
                Value=c.Id.ToString()
            }).ToList();
            productViewModel.Categories=categoriesList;
            return View(productViewModel);
            }
        var UniqueName="";
      if(productViewModel.ProductDTO.Image is not null){
            var UploadDir= Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images/productImages");
            if(!Directory.Exists(UploadDir)){
                Directory.CreateDirectory(UploadDir);
            }
             UniqueName=Guid.NewGuid()+"_"+Path.GetFileName(productViewModel.ProductDTO.Image.FileName);

            var filePath=Path.Combine(UploadDir,UniqueName);
            var fileStrem=new FileStream(filePath,FileMode.Create);
            await productViewModel.ProductDTO.Image.CopyToAsync(fileStrem);
      }
           

            var product=_dbContext.Products.Find(productViewModel.ProductDTO.Id);

            product.CategoryId=productViewModel.ProductDTO.CategoryId;
            product.Description=productViewModel.ProductDTO.Description;
            product.Title=productViewModel.ProductDTO.Title;
            product.ImageUrl= String.IsNullOrEmpty(UniqueName)?productViewModel.ProductDTO.ImageUrl : UniqueName;
            product.Price=productViewModel.ProductDTO.Price;
            product.Stock=productViewModel.ProductDTO.Stock;
            

            _dbContext.Products.Update(product);
            _dbContext.SaveChanges();

            ViewBag.Success="Producted updated successfully";

            //productViewModel.Categories=TempData["Categories"] as List<SelectListItem>;
            var categories=_dbContext.Categories.Select(c=>new SelectListItem(){
                Text=c.CategoryName,
                Value=c.Id.ToString()
            }).ToList();
            productViewModel.Categories=categories;
            return View(productViewModel);
            }catch(Exception e){
                return RedirectToAction("ErrorPage",new {message="Internal Server Error"});
            }
        }
   #endregion
    #region Category
        public IActionResult Category(){
           List<Category> categories= _dbContext.Categories.ToList();
           
           return View(categories);
        }

        [HttpGet]
        public IActionResult AddCategory(){
            return View();
        }
        [HttpPost]
        public IActionResult AddCategory(Category category){
            if(!ModelState.IsValid){
                return View();
            }
            try{
               bool categoryExists = _dbContext.Categories
        .Any(c => c.CategoryName == category.CategoryName);

    if (categoryExists)
    {
        ModelState.AddModelError(string.Empty, "A category with this name already exists.");
        return View();
    }
             _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();
            ViewBag.Success="Category Added successfully";
            }catch(Exception e){
                ModelState.AddModelError(string.Empty,e.Message);
            }
            
            return View();
        }

        public IActionResult DeleteCategory(Guid id)
{
    var category = _dbContext.Categories.FirstOrDefault(c => c.Id == id);
    if (category == null)
    {
        //var errorModel = new ErrorViewModelPage { message = "Category not found" };
        return RedirectToAction("ErrorPage", new {message="Not found"});
    }

    _dbContext.Categories.Remove(category);
    _dbContext.SaveChanges();
    return RedirectToAction("Category");
}
        
        [HttpGet]
        public IActionResult EditCategory(Guid id){
            try{
                Category c=_dbContext.Categories.FirstOrDefault(cat=>cat.Id==id);
                if(c==null){
                    return RedirectToAction("ErrorPage",new{ message="category not found"});
                }
                return View(c);
            }catch(Exception e){
                return RedirectToAction("ErrorPage",new {message=e.Message});
            }
            return View();
        } 
        [HttpPost]
        public IActionResult EditCategory(Category category){
            if(!ModelState.IsValid){
                return View();
            }
            try{
                bool categoryExists = _dbContext.Categories
            .Any(c => c.CategoryName == category.CategoryName);

            if (categoryExists)
    {
        ModelState.AddModelError(string.Empty, "A category with this name already exists.");
        return View(category);
    }   
       Category c= _dbContext.Categories.FirstOrDefault(cat=>cat.Id==category.Id);
       c.CategoryName=category.CategoryName;
       _dbContext.SaveChanges();
     ViewBag.Success="Category Edited successfully successfully";
     return View(category);
            }catch(Exception e){
                return RedirectToAction("ErrorPage",new {message=e.Message});
            }
        } 
#endregion

     #region Orders
public IActionResult Orders(){
    var orders=_dbContext.Orders.OrderByDescending(o=>o.OrderDate).ToList();
    return View(orders);
}
public IActionResult OrderDetail(Guid orderid){
    var order = _dbContext.Orders.Where(o=>o.Id==orderid)
        .Include(o => o.OrderItems) // Include OrderItems
        .ThenInclude(o=>o.Product)
        .Include(o => o.User)       // Include User (assuming navigation property exists)
        .ToList();

    if (order == null)
    {
        return RedirectToAction("ErrorPage", new { message = "Order not found" });
    }
    
    return View(order);
}

public IActionResult OrderMarkDeliver(Guid orderid){
    try{
    var order=_dbContext.Orders.FirstOrDefault(o=>o.Id==orderid);
    if(order==null){
        return RedirectToAction("ErrorPage",new {message="not found"});
    }
    order.IsDelivered=true;
    _dbContext.SaveChanges();
    return RedirectToAction("Orders");
    }catch(Exception e){
        return RedirectToAction("ErrorPage",new {message=e.Message});
    }
}   
#endregion

    #region User
         public IActionResult Users(string searchQuery=""){
            if(!String.IsNullOrEmpty(searchQuery)){
               var searchedusers= _dbContext.Users
                 .Where(u=>u.UserName.ToLower().Contains(searchQuery.ToLower()))
            .ToList();
            return View(searchedusers);
            }
            var users=_dbContext.Users.ToList();
            return View(users);
         }
    #endregion
    public IActionResult ErrorPage(string message)
{
    ViewBag.Error = message;
    return View();
}










        



    }
}
