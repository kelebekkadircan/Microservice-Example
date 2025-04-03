using Microsoft.EntityFrameworkCore;
using ORDER.API.Models.Entities;

namespace ORDER.API.Models
{
    public class OrderAPIDbContext : DbContext
    {
        public OrderAPIDbContext(DbContextOptions options) : base(options)
        {

        }

      

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

    }
}
