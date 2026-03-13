using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;
using RotiseriaAPI.Models;
using RotiseriaAPI.Services;

namespace RotiseriaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PrintService _printService;
    public OrdersController(AppDbContext context, PrintService printService)
    {
        _context = context;
        _printService = printService;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(Order order)
    {
        // 1. Fecha y Hora automática del servidor
        order.Date = DateTime.Now;
        order.Status = "Recibido"; // Estado inicial

        decimal totalProductos = 0;

        // 2. Procesamos cada ítem para buscar el precio real
        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                item.ProductName = product.Name; // Guardamos el nombre actual
                item.UnitPrice = product.Price;  // Usamos el precio de la base de datos
                totalProductos += (item.UnitPrice * item.Quantity);
            }
        }

        // 3. Calculamos el Total Final
        order.Total = totalProductos + order.DeliveryCost;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        // 4. Imprimir comanda automáticamente
        try
        {
            _printService.PrintOrder(order);
        }
        catch (Exception ex)
        {
            // Si no hay impresora conectada, que no se trabe el programa
            Console.WriteLine("Error de impresion: " + ex.Message);
        }

        return Ok(order);
    }

    // GET: api/Orders/today (Para ver solo los pedidos de HOY)
    [HttpGet("today")]
    public async Task<ActionResult<IEnumerable<Order>>> GetTodayOrders()
    {
        var today = DateTime.Today;
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.Date >= today)
            .OrderByDescending(o => o.Date)
            .ToListAsync();
    }
}