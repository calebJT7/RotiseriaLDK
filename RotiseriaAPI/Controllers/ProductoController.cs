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

        // NUEVO - GET: api/Producto/activos (Para el cliente: solo ve lo que hay)
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

        // PATCH: api/Producto/toggle/5 (Prender/Apagar producto)
        [HttpPatch("toggle/{id}")]
        public async Task<IActionResult> ToggleProductStatus(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Invertimos el estado
            product.IsActive = !product.IsActive;

            await _context.SaveChangesAsync();
            return Ok(new { name = product.Name, isActive = product.IsActive });
        }
    }
}