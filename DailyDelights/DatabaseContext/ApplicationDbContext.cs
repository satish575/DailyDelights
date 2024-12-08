using System;
using DailyDelights.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DailyDelights.DatabaseContext;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){

        }

        public DbSet<Category> Categories {get; set;}
        public DbSet<Product> Products {get; set;}

        public DbSet<Order> Orders {get; set;}
        public DbSet<OrderItem> OrderItems {get; set;}
        public DbSet<ProductReview> ProductReviews {get;set;}
}
