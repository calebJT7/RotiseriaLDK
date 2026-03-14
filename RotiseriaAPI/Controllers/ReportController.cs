using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;
using RotiseriaAPI.DTOs;

namespace RotiseriaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ReportsController(AppDbContext context) => _context = context;
    [Authorize(Roles = "Admin")] // ¡SOLO EL ADMIN PUEDE ENTRAR!
    [HttpGet("daily")]
    public async Task<ActionResult<DailyReport>> GetDailyReport()
    {
        var today = DateTime.Today;

        // 1. Pedidos del día
        var orders = await _context.Orders
            .Where(o => o.Date >= today)
            .ToListAsync();

        // 2. Gastos del día
        var totalExpenses = await _context.Expenses
            .Where(e => e.Date >= today)
            .SumAsync(e => e.Amount);

        // 3. Deudas generadas HOY que siguen sin pagar
        // Esto es lo que "perdiste" de cobrar hoy
        var unpaidDebtsToday = await _context.Debts
            .Where(d => d.Date >= today && !d.IsPaid)
            .SumAsync(d => d.Amount);

        // 4. Armar el resumen inteligente
        var report = new DailyReport
        {
            Date = today,
            OrdersCount = orders.Count,
            TotalSales = orders.Sum(o => o.Total),

            // Dinero REAL que entró (Efectivo + Transferencia)
            TotalCash = orders.Where(o => o.PaymentMethod == "Efectivo").Sum(o => o.Total),
            TotalTransfer = orders.Where(o => o.PaymentMethod == "Transferencia").Sum(o => o.Total),

            // Lo que se anotó en el cuaderno y no está en la caja
            TotalDebt = unpaidDebtsToday,

            TotalExpenses = totalExpenses
        };

        return Ok(report);
    }
}