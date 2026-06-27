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
        // 1. Configuración inicial
        order.Date = DateTime.Now;
        order.Status = "Pendiente"; // Unificado con la pantalla de la Cocina
        decimal totalProductos = 0;

        // 2. Procesar ítems: Validamos Stock y calculamos Precios
        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                // Validación de Stock
                if (product.Stock < item.Quantity)
                {
                    return BadRequest($"No hay stock suficiente de {product.Name}. Disponible: {product.Stock}");
                }

                // Restamos del stock
                product.Stock -= item.Quantity;

                item.ProductName = product.Name;
                item.UnitPrice = product.Price;
                totalProductos += (item.UnitPrice * item.Quantity);
            }
        }

        // 3. Calculamos el Total Final
        order.Total = totalProductos + order.DeliveryCost;

        // 4. Lógica de "Fiado": Si es Cuenta Corriente, actualizamos saldo del cliente
        if (order.PaymentMethod == "Cuenta Corriente" && order.CustomerId.HasValue)
        {
            var customer = await _context.Customers.FindAsync(order.CustomerId.Value);
            if (customer != null)
            {
                // Actualizamos la deuda. Si tu sistema suma deuda con saldos negativos:
                customer.Balance -= order.Total;
            }
        }

        // 5. Guardar en Base de Datos
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // 6. Intento de Impresión automática
        try
        {
            _printService.PrintOrder(order);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error de impresión (ignorado para no frenar la venta): " + ex.Message);
        }

        return Ok(order);
    }

    // GET: api/Order 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.Date)
            .ToListAsync();
    }

    // GET: api/Order/today 
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

    // PATCH: api/Order/dispatch/5
    [HttpPatch("dispatch/{id}")]
    public async Task<IActionResult> DispatchOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = "Despachado";
        order.DispatchedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok();
    }

    // PATCH: api/Order/dismiss30m/5
    [HttpPatch("dismiss30m/{id}")]
    public async Task<IActionResult> Dismiss30MinAlert(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Alert30Dismissed = true;
        await _context.SaveChangesAsync();
        return Ok();
    }

    // PATCH: api/Order/cancel/5
    [HttpPatch("cancel/{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = "Cancelado";
        await _context.SaveChangesAsync();
        return Ok();
    }
}