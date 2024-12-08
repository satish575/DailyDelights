using System.Security.Claims;
using DailyDelights.DatabaseContext;
using DailyDelights.Models;
using DailyDelights.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace DailyDelights.Controllers
{
    [Authorize(Roles ="Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public OrderController(ApplicationDbContext applicationDbContext,IConfiguration configuration){
            _context=applicationDbContext;
            _configuration=configuration;
        }
        // GET: OrderController
        [HttpPost]
      
       public IActionResult PlaceOrder([FromBody] OrderViewModel orderViewModel){
        var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid userid;
        if(Guid.TryParse(userId,out userid)){
            var Order=new Order(){
            UserId=userid,
            OrderDate=DateTime.Now,
            TotalAmount=orderViewModel.totalAmount,
        };

        _context.Orders.Add(Order);
        _context.SaveChanges();

        foreach(var item in orderViewModel.products){
        var OrderItem=new OrderItem(){
            OrderId= Order.Id,
            ProductId=Guid.Parse(item.productId),
            Quantity=item.noOfItem,    

        };
        var product=_context.Products.Find(Guid.Parse(item.productId));
        product.Stock=product.Stock-item.noOfItem;
        
        _context.OrderItems.Add(OrderItem);
        
       }
       _context.SaveChanges();
         return Json(new { success = true, orderId = Order.Id });
        

        }

       return RedirectToAction("Index","Home");

       
        
       }

        [HttpGet]
       public IActionResult PaymentPage([FromQuery]Guid orderId){
        var publishableKey = _configuration["Stripe:PublishableKey"];

    // Pass the publishable key to the view using ViewData
    ViewData["PublishableKey"] = publishableKey;
        Order order=_context.Orders.Find(orderId);
    return View(order);
        
       }

        [HttpGet]
        [Authorize(Roles ="Customer")]
        public IActionResult GetOrders(){
            string userid=User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId;

            if(Guid.TryParse(userid,out userId)){
                var OrderList= _context.Orders.Where(order=>order.UserId==userId);
                return View(OrderList);
            }
           
            return View();
        }

        public IActionResult OrderDetail(Guid orderid){
            var orderDetail=_context.Orders.Where(order=> order.Id==orderid).Include(o=>o.OrderItems).ThenInclude(oi=>oi.Product).ToList();

            if(orderDetail.Count<1){
                return Redirect("Error/404");
            }
            return View(orderDetail);
        }

        [HttpPost]
public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest paymentRequest)
{
    try
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(paymentRequest.amount * 100), // Convert to cents
            Currency = "usd",
            PaymentMethod = paymentRequest.paymentMethodId,
            ConfirmationMethod = "manual",
            Confirm = true,
            ReturnUrl = Url.Action("Success", "Payment", null, Request.Scheme),
        };

        var service = new PaymentIntentService();
        var paymentIntent = service.Create(options);

        return Ok(new { success = true, clientSecret = paymentIntent.ClientSecret, orderId = paymentRequest.orderId });
    }
    catch (Exception ex)
    {
        return BadRequest(new { success = false, message = ex.Message });
    }
}


  

// DTO for payment data
public class PaymentRequest
{
    public string? paymentMethodId
 { get; set; }
    public Guid orderId { get; set; }
    public decimal amount { get; set; }
}

    }
}
