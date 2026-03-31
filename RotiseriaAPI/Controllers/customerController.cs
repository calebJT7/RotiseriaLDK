using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Controllers;

[Route("api/[controller]")] // Esto hace que la ruta sea /api/Customer
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomerController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        // Esto va a la base de datos y trae la lista
        return await _context.Customers.ToListAsync();
    }
    [HttpPost]
    public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCustomer(int id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();
        _context.Entry(customer).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPost("{id}/pay")]
    public async Task<ActionResult> PayDebt(int id, [FromBody] decimal amount)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        // Sumamos el monto al saldo (que es negativo)
        customer.Balance += amount;

        _context.Entry(customer).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(customer);
    }
}