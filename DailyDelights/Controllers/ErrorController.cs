using Microsoft.AspNetCore.Mvc;

namespace DailyDelights.Controllers
{
    public class ErrorController : Controller
    {
        // GET: ErrorController
       [Route("Error/{statusCode}")]
    public IActionResult Index(int statusCode)
    {
        if (statusCode == 404)
        {
            return View("404");
        }

        // You can handle other status codes (e.g., 500, 403) here
        return View("Error");
    }
    }
}
