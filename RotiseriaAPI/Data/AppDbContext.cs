using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<EmployeeConsumption> EmployeeConsumptions { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // --- NUEVO: La tabla de Mesas ---
        public DbSet<Table> Tables { get; set; }

        // MAGIA DE ANALISTA: Hacemos que la base de datos nazca con las 10 mesas creadas automáticamente
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var mesasIniciales = new Table[10];
            for (int i = 1; i <= 10; i++)
            {
                mesasIniciales[i - 1] = new Table { Id = i, Number = i, Status = "Libre" };
            }

            modelBuilder.Entity<Table>().HasData(mesasIniciales);
        }
    }
}