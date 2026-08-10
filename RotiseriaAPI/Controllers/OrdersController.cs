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
        order.Date = DateTime.Now;
        decimal totalProductos = 0;

        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                // Control de stock inteligente (solo para bebidas)
                if (product.Category != null && product.Category.ToLower().Contains("bebida"))
                {
                    if (product.Stock < item.Quantity)
                        return BadRequest($"No hay stock suficiente de {product.Name}. Disponible: {product.Stock}");

                    product.Stock -= item.Quantity;
                }

                item.ProductName = product.Name;
                item.UnitPrice = product.Price;
                totalProductos += (item.UnitPrice * item.Quantity);
            }
        }

        // =========================================================
        // LÓGICA DE SALÓN (MESAS)
        // =========================================================
        if (order.OrderType == "Salon" && order.TableId.HasValue)
        {
            var table = await _context.Tables.FindAsync(order.TableId.Value);
            if (table == null) return BadRequest("La mesa no existe.");

            if (table.CurrentOrderId.HasValue)
            {
                // A) ADICIÓN
                var existingOrder = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == table.CurrentOrderId);
                if (existingOrder != null)
                {
                    existingOrder.Items.AddRange(order.Items);
                    existingOrder.Total += totalProductos;
                    existingOrder.Status = "Pendiente";
                    existingOrder.Date = DateTime.Now;

                    await _context.SaveChangesAsync();

                    var ticketAdicion = new Order
                    {
                        Id = existingOrder.Id,
                        ClientName = $"Mesa {table.Number}", // Solucionado: Ahora sí sale en el ticket
                        OrderType = $"ADICIÓN - MESA {table.Number}",
                        Date = DateTime.Now,
                        Items = order.Items
                    };
                    ImprimirTicket(ticketAdicion);
                    return Ok(existingOrder);
                }
            }
            else
            {
                // B) ABRIR MESA NUEVA
                order.Total = totalProductos;
                order.Status = "Pendiente";
                order.ClientName = $"MESA {table.Number}";
                order.OrderType = $"SALÓN - MESA {table.Number}";

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Forzamos la actualización de la mesa a Ocupada
                table.CurrentOrderId = order.Id;
                table.Status = "Ocupada";
                _context.Tables.Update(table);
                await _context.SaveChangesAsync();

                ImprimirTicket(order);
                return Ok(order);
            }
        }

        // =========================================================
        // LÓGICA NORMAL (DELIVERY / MOSTRADOR)
        // =========================================================
        order.Total = totalProductos + order.DeliveryCost;
        order.Status = "Pendiente";

        if (order.PaymentMethod == "Cuenta Corriente" && order.CustomerId.HasValue)
        {
            var customer = await _context.Customers.FindAsync(order.CustomerId.Value);
            if (customer != null) customer.Balance -= order.Total;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        ImprimirTicket(order);
        return Ok(order);
    }

    private void ImprimirTicket(Order order)
    {
        try { _printService.PrintOrder(order); }
        catch { /* Error ignorado en consola */ }
    }

    // --- NUEVO BOTÓN REIMPRIMIR ---
    [HttpPost("reprint/{id}")]
    public async Task<IActionResult> ReprintOrder(int id)
    {
        var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();

        ImprimirTicket(order);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders() => await _context.Orders.Include(o => o.Items).OrderByDescending(o => o.Date).ToListAsync();

    [HttpGet("today")]
    public async Task<ActionResult<IEnumerable<Order>>> GetTodayOrders() => await _context.Orders.Include(o => o.Items).Where(o => o.Date >= DateTime.Today).OrderByDescending(o => o.Date).ToListAsync();

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

    [HttpPatch("dismiss30m/{id}")]
    public async Task<IActionResult> Dismiss30MinAlert(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();
        order.Alert30Dismissed = true;
        await _context.SaveChangesAsync();
        return Ok();
    }

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