using System.Security.Claims;
using DailyDelights.DatabaseContext;
using DailyDelights.Models;
using DailyDelights.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles ="Customer")]
public class ReviewController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public ReviewController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IActionResult ReviewOrder(Guid orderid)
    {
        try{
        var orderDetail = _dbContext.Orders
            .Where(order => order.Id == orderid)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToList();
        if(orderDetail.Count < 1){
            return Redirect("Error/404");
        }

        var productReviewViewModels = orderDetail
            .SelectMany(order => order.OrderItems)
            .Select(orderItem => new ProductReviewViewModel
            {
                product = orderItem.Product,
                ratting = 0
            })
            .ToList();
            return View(productReviewViewModels);
        }catch(Exception e)
        {
            return Redirect("Error/500");
        }
        
    }

    [HttpPost]
    public IActionResult SubmitRatings(List<Guid> ProductIds, List<double> Ratings)
    {
        if (ProductIds.Count != Ratings.Count)
        {
            return BadRequest("Mismatch in products and ratings.");
        }

        if (Ratings.Any(r => r < 1 || r > 5))
        {
            return BadRequest("Invalid rating value.");
        }
     var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
      Guid userid;
      Guid.TryParse(userId,out userid);
        var productReviews = ProductIds.Select((id, index) => new ProductReview
        {
            ProductId = id,
            Rating = Ratings[index],
            UserId=userid

        }).ToList();

        _dbContext.ProductReviews.AddRange(productReviews);

        

        _dbContext.SaveChanges();


         var productsToUpdate = ProductIds.Distinct().ToList();

    foreach (var productId in productsToUpdate)
    {
        // Get all ratings for the product
        var productRatings = _dbContext.ProductReviews
                                        .Where(r => r.ProductId == productId)
                                        .Select(r => r.Rating)
                                        .ToList();

        // Calculate the average rating
        var averageRating = productRatings.Average();

        // Find the product and update its average rating
        var product = _dbContext.Products.FirstOrDefault(p => p.Id == productId);
        if (product != null)
        {
            product.Ratting = averageRating; // Assuming there's an `AverageRating` column in your `Products` table
        }
    }

    // Save changes to the Products table
       _dbContext.SaveChanges();

        return RedirectToAction("ThankYou");
    }

    public IActionResult ThankYou(){
        return View();
    }
}
