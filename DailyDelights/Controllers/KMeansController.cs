using DailyDelights.DatabaseContext;
using DailyDelights.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyDelights.Controllers
{
    public class KMeansController : Controller
    {

        private readonly ApplicationDbContext _context;
        public KMeansController(ApplicationDbContext applicationDbContext){
            this._context=applicationDbContext;
        }
        // GET: KMeansController
        [AllowAnonymous]
        public ActionResult Index()
        {
            var orders=  _context.Orders.GroupBy(order=>order.UserId).Select(group=> new{
                UserId=group.Key,
                AverageOrderAmount=group.Average(order=>order.TotalAmount)
            }).ToList();
            
            return View();
        }

    }
}
