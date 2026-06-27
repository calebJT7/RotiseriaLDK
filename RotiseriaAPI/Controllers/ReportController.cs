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

        // Traemos todos los pedidos de hoy de una sola vez y súper rápido con AsNoTracking
        var ordersToday = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Date >= today)
            .ToListAsync();

        // Ventas netas (Comida + Bebida + Envío, excluyendo fiados)
        // Ventas netas: Sumamos los Totales pero restándole el costo del envío a cada uno 
        // y excluyendo las cuentas corrientes (fiados)
        var salesToday = ordersToday
            .Where(o => o.PaymentMethod != "Cuenta Corriente")
            .Sum(o => o.Total - o.DeliveryCost); // ¡ACÁ RESTAMOS EL ENVÍO!

        // Recaudación exclusiva de los cadetes
        var deliveryRevenue = ordersToday
            .Where(o => o.OrderType == "Delivery")
            .Sum(o => o.DeliveryCost);

        // Cantidad de viajes
        var deliveryTrips = ordersToday
            .Count(o => o.OrderType == "Delivery");

        var totalDebt = await _context.Customers
            .AsNoTracking()
            .Where(c => c.Balance < 0)
            .SumAsync(c => c.Balance);

        var lowStockBebidas = await _context.Products
            .AsNoTracking()
            .Where(p => p.Category == "Bebida" && p.Stock < 5)
            .Select(p => new { p.Name, p.Stock })
            .ToListAsync();

        return Ok(new
        {
            SalesToday = salesToday,
            DeliveryRevenue = deliveryRevenue,
            DeliveryTrips = deliveryTrips,
            TotalDebt = Math.Abs(totalDebt),
            OrdersCount = ordersToday.Count,
            LowStock = lowStockBebidas
        });
    }

    [HttpGet("monthly-history")]
    public async Task<ActionResult> GetMonthlyHistory()
    {
        var history = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Date >= DateTime.Now.AddYears(-1))
            .GroupBy(o => new { o.Date.Year, o.Date.Month })
            .Select(g => new
            {
                Label = $"{g.Key.Month}/{g.Key.Year}",
                Total = g.Sum(o => o.Total),
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.Label)
            .ToListAsync();

        return Ok(history);
    }
}