using DailyDelights.DatabaseContext;
using DailyDelights.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace DailyDelights.Controllers
{
    [Authorize(Roles ="Customer")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context){
            _context=context;
        }
        // GET: PaymentController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Success(Guid orderId){
        
           Order order= _context.Orders.Find(orderId);
           order.IsPain=true;
           _context.SaveChanges();
           ViewData["orderId"]=orderId;
           return View();
        }


        public ActionResult DownloadReceipt(Guid orderId){
               var order = _context.Orders
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Product)
                                .Include(oi=>oi.User)
                                .FirstOrDefault(o => o.Id == orderId);
                if(!order.IsPain){
                    return Redirect("Error/404");
                }

                QuestPDF.Settings.License = LicenseType.Community;

                    var pdfBytes = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1, Unit.Inch);
            page.Header().Text("Payment Receipt").FontSize(20).Bold();
           // page.Header().Text("Ordered By"+order.User.UserName).FontSize(20).Bold();
            page.Content().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // Add header row
                table.Cell().Text("Product ID").Bold();
                table.Cell().Text("Product Name").Bold();
                table.Cell().Text("Price").Bold();

                // Add rows
                 foreach (var product in order.OrderItems)
                {
                    table.Cell().Text(product.Product.Id);
                    table.Cell().Text(product.Product.Title);
                    table.Cell().Text($"${product.Product.Price}");
                }

                table.Cell().Text("Ordered By").Bold();
                table.Cell().Text("Total Amount").Bold();
                table.Cell().Text("order Date").Bold();

                  table.Cell().Text(order.User.UserName).Bold();
                table.Cell().Text(order.TotalAmount).Bold();
                table.Cell().Text(order.OrderDate).Bold();
            });

        
            page.Footer().AlignRight().Text($"Generated on {DateTime.Now:yyyy-MM-dd}");
        });
    }).GeneratePdf();

    return File(pdfBytes, "application/pdf", "Report.pdf");

        }
    }
}
