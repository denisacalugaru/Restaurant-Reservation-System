using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Data;
using RestaurantReservation.API.Models;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantReservation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TablesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TablesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetTables()
    {
        var tables = await _context.Tables
            .Select(t => new { t.Id, t.TableNumber, t.Capacity, t.IsAvailable, t.RestaurantId })
            .ToListAsync();
        return Ok(tables);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetTable(int id)
    {
        var table = await _context.Tables
            .Select(t => new { t.Id, t.TableNumber, t.Capacity, t.IsAvailable, t.RestaurantId })
            .FirstOrDefaultAsync(t => t.Id == id);

        if (table == null) return NotFound(new { message = "Masa nu a fost găsită." });
        return Ok(table);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateTable([FromBody] Table table)
    {
        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTable), new { id = table.Id }, table);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTable(int id, [FromBody] Table table)
    {
        if (id != table.Id) return BadRequest();

        _context.Entry(table).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tables.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTable(int id)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null) return NotFound();

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}