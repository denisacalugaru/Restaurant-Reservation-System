using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Data;
using RestaurantReservation.API.Models;
using Microsoft.AspNetCore.Authorization;
namespace RestaurantReservation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetCategories()
    {
        var categories = await _context.Categories
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetCategory(int id)
    {
        var category = await _context.Categories
            .Select(c => new { c.Id, c.Name })
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound(new { message = "Categoria nu a fost găsită." });
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateCategory([FromBody] Category category)
    {
        if (string.IsNullOrEmpty(category.Name)) return BadRequest(new { message = "Date invalide." });

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
    {
        if (id != category.Id) return BadRequest(new { message = "ID-urile nu coincid." });

        _context.Entry(category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Categories.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}