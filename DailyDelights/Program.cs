using DailyDelights.DatabaseContext;
using DailyDelights.Models;
using DailyDelights.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IEmailSender,EmailSender>();
builder.Services.AddDbContext<ApplicationDbContext>(options=>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

Stripe.StripeConfiguration.ApiKey=builder.Configuration["Stripe:SecretKey"];

// appliction user and application role configuration
builder.Services.AddIdentity<ApplicationUser,ApplicationRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// default password constraints modification
builder.Services.Configure<IdentityOptions>(option=>{
    option.Password.RequiredLength=5;
    option.Password.RequireDigit=false;
    option.Password.RequireUppercase=false;
    option.Password.RequireLowercase=false;
    option.Password.RequiredUniqueChars=0;
    option.Password.RequireNonAlphanumeric=false;
    option.SignIn.RequireConfirmedEmail = true;
}
        
        
        
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
     app.UseHsts();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
