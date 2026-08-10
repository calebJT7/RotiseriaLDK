using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TablesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Trae todas las mesas (Para dibujar el mapa en verde y rojo)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Table>>> GetTables()
        {
            return await _context.Tables.OrderBy(t => t.Number).ToListAsync();
        }

        // 2. Trae la cuenta actual de una mesa (Para ver qué están comiendo)
        [HttpGet("{id}/order")]
        public async Task<ActionResult<Order>> GetTableOrder(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null || table.CurrentOrderId == null) return NotFound("Mesa libre");

            // Buscamos el pedido y le INCLUIMOS la lista de productos
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == table.CurrentOrderId);

            if (order == null) return NotFound();

            return order;
        }

        // 3. Botón "Cobrar y Liberar Mesa"
        [HttpPost("{id}/close")]
        public async Task<IActionResult> CloseTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null || table.CurrentOrderId == null) return BadRequest("La mesa ya está libre");

            var order = await _context.Orders.FindAsync(table.CurrentOrderId);
            if (order != null)
            {
                order.Status = "Despachado"; // El pedido se da por terminado
                order.IsPaid = true;         // La plata entra a la caja
            }

            // Liberamos la mesa para el próximo cliente
            table.Status = "Libre";
            table.CurrentOrderId = null;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}