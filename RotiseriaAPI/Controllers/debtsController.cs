using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DebtsController : ControllerBase
{
    private readonly AppDbContext _context;
    public DebtsController(AppDbContext context) => _context = context;

    // Ver todos los fiados (pendientes y pagados)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Debt>>> GetDebts() => await _context.Debts.ToListAsync();

    // Anotar un fiado nuevo
    [HttpPost]
    public async Task<ActionResult<Debt>> PostDebt(Debt debt)
    {
        _context.Debts.Add(debt);
        await _context.SaveChangesAsync();
        return Ok(debt);
    }

    // Marcar como pagado
    [HttpPut("{id}/pay")]
    public async Task<IActionResult> PayDebt(int id)
    {
        var debt = await _context.Debts.FindAsync(id);
        if (debt == null) return NotFound();

        debt.IsPaid = true;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}