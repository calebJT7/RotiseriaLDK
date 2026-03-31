using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;

namespace RotiseriaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult> GetDashboardData()
    {
        var today = DateTime.Today;

        // Ventas netas (comida + bebida)
        var salesToday = await _context.Orders
            .Where(o => o.Date >= today && o.PaymentMethod != "Cuenta Corriente")
            .SumAsync(o => o.Total);

        // NUEVO: Total de recaudación por Delivery (la suma de todos los "Costo de Envío")
        var deliveryRevenue = await _context.Orders
            .Where(o => o.Date >= today && o.OrderType == "Delivery")
            .SumAsync(o => o.DeliveryCost);

        // NUEVO: Cantidad de viajes (cuántos pedidos fueron Delivery)
        var deliveryTrips = await _context.Orders
            .CountAsync(o => o.Date >= today && o.OrderType == "Delivery");

        var totalDebt = await _context.Customers
            .Where(c => c.Balance < 0)
            .SumAsync(c => c.Balance);

        var ordersCount = await _context.Orders
            .CountAsync(o => o.Date >= today);

        var lowStockBebidas = await _context.Products
            .Where(p => p.Category == "Bebida" && p.Stock < 5)
            .Select(p => new { p.Name, p.Stock })
            .ToListAsync();

        return Ok(new
        {
            SalesToday = salesToday,
            DeliveryRevenue = deliveryRevenue, // Plata de los envíos
            DeliveryTrips = deliveryTrips,     // Cantidad de viajes
            TotalDebt = Math.Abs(totalDebt),
            OrdersCount = ordersCount,
            LowStock = lowStockBebidas
        });
    }
    [HttpGet("monthly-history")]
    public async Task<ActionResult> GetMonthlyHistory()
    {
        // Agrupamos los pedidos por Año y Mes
        var history = await _context.Orders
            .Where(o => o.Date >= DateTime.Now.AddYears(-1)) // Solo el último año
            .GroupBy(o => new { o.Date.Year, o.Date.Month })
            .Select(g => new
            {
                Label = $"{g.Key.Month}/{g.Key.Year}", // Ejemplo: "3/2026"
                Total = g.Sum(o => o.Total),
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.Label)
            .ToListAsync();

        return Ok(history);
    }
}