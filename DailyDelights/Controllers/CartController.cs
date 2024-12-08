using Microsoft.AspNetCore.Mvc;

namespace DailyDelights.Controllers
{
    public class CartController : Controller
    {
        // GET: CartController
        public ActionResult Index()
        {
            return View();
        }

    }
}
