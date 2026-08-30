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
        // TRASPASO DE TURNO NOCTURNO: Restamos 5 horas para que la madrugada (hasta la 01:00 AM) 
        // siga contando dentro del día operativo anterior (la noche del fin de semana).
        var today = DateTime.Now.AddHours(-5).Date;

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
        // 1. Descargamos los pedidos del último año a la memoria (ignorando los fiados sin pagar)
        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Date >= DateTime.Now.AddYears(-1) && o.PaymentMethod != "Cuenta Corriente")
            .ToListAsync();

        // 2. Agrupamos por mes en la memoria (¡Esto no falla nunca!)
        var history = orders
            .GroupBy(o => new { o.Date.Year, o.Date.Month })
            .Select(g => new
            {
                Label = $"{g.Key.Month}/{g.Key.Year}",
                Total = g.Sum(o => o.Total - o.DeliveryCost), // Ventas netas sin delivery
                OrderCount = g.Count()
            })
            .OrderByDescending(x => x.Label) // El mes más reciente arriba
            .ToList();

        return Ok(history);
    }
}