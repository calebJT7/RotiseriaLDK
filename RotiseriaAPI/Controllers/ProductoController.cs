using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Producto (Para el administrador: ve todos)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Producto/activos (Para el cliente: solo ve lo que hay)
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<Product>>> GetActiveProducts()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        // POST: api/Producto (Cargar producto nuevo)
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }

        // GET: api/Producto/search/mil (Búsqueda instantánea)
        [HttpGet("search/{term}")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string term)
        {
            return await _context.Products
                .Where(p => p.IsActive && p.Name.ToLower().Contains(term.ToLower()))
                .Take(10)
                .ToListAsync();
        }

        // PATCH: api/Producto/toggle/5 (Prender/Apagar producto - Método viejo, lo dejamos por si acaso)
        [HttpPatch("toggle/{id}")]
        public async Task<IActionResult> ToggleProductStatus(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsActive = !product.IsActive;
            await _context.SaveChangesAsync();
            return Ok(new { name = product.Name, isActive = product.IsActive });
        }

        // --- ¡ACÁ ESTÁN LAS PUERTAS NUEVAS QUE FALTABAN! ---

        // PUT: api/Producto/5 (Sirve para Editar el precio/nombre y para el Switch de Activo/Inactivo)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("El ID no coincide.");
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content (Es el código de éxito estándar para PUT)
        }

        // DELETE: api/Producto/5 (Eliminar producto)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content (Es el código de éxito estándar para DELETE)
        }

        // Función auxiliar de seguridad
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}