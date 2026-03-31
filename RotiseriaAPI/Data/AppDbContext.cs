using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        // Agregá estas dos líneas debajo de la de Products:
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<EmployeeConsumption> EmployeeConsumptions { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customer { get; set; }
    }
}