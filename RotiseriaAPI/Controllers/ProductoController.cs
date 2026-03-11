using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase // Si el archivo es ProductoController, la clase se llama así
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
        }
        // GET: api/Producto/search/mil
        [HttpGet("search/{term}")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string term)
        {
            return await _context.Products
                .Where(p => p.IsActive && p.Name.ToLower().Contains(term.ToLower()))
                .Take(10) // Solo los primeros 10 para que sea instantáneo
                .ToListAsync();
        }
    }
}