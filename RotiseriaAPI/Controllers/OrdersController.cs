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
        order.Status = "Recibido";
        decimal totalProductos = 0;

        // 2. Procesamos ítems: Validamos Stock y calculamos Precios
        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                // Validación de Stock (Crítico para bebidas)
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
        if (order.PaymentMethod == "Cuenta Corriente")
        {
            var customer = await _context.Customers.FindAsync(order.CustomerId);
            if (customer != null)
            {
                // El saldo baja (se hace más negativo) porque nos debe más
                customer.Balance -= order.Total;
                _context.Entry(customer).State = EntityState.Modified;
            }
        }

        // 5. Guardamos en Base de Datos
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

    // GET: api/Orders (Historial completo para el Día 23)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.Date)
            .ToListAsync();
    }

    // GET: api/Orders/today (Para ver solo lo de hoy)
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