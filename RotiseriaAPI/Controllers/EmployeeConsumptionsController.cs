using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RotiseriaAPI.Data;
using RotiseriaAPI.Models;

namespace RotiseriaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeConsumptionsController : ControllerBase
{
    private readonly AppDbContext _context;
    public EmployeeConsumptionsController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeConsumption>>> GetConsumptions() => await _context.EmployeeConsumptions.ToListAsync();

    [HttpPost]
    public async Task<ActionResult<EmployeeConsumption>> PostConsumption(EmployeeConsumption consumption)
    {
        _context.EmployeeConsumptions.Add(consumption);
        await _context.SaveChangesAsync();
        return Ok(consumption);
    }
}