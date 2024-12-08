using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DailyDelights.Models;
using Microsoft.AspNetCore.Authorization;
using DailyDelights.DatabaseContext;

namespace DailyDelights.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public HomeController(ILogger<HomeController> logger,ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext=dbContext;
    }
   
   [AllowAnonymous]
    public IActionResult Index()
    {
       
        return View();
       
    }
    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About(){
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
