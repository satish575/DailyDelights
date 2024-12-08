using DailyDelights.DatabaseContext;
using DailyDelights.Models;
using DailyDelights.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyDelights.Controllers
{
    public class ClusterController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public ClusterController(ApplicationDbContext dbContext,UserManager<ApplicationUser> userManager){
            _dbContext=dbContext;
            _userManager=userManager;
        }
        // GET: ClusterController
        public async  Task<ActionResult> Index()
        {
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
        List<DailyDelights.ViewModels.Orders> cluster = KMeansCluster.GetCluster(userOrder, user.Id);

            return View(cluster);
            }
    else{
        return RedirectToAction("Login", "Account");
    }
        }

    }
}
