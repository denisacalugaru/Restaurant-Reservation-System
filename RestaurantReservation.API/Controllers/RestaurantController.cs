using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Data;
using RestaurantReservation.API.Models;
using Microsoft.AspNetCore.Authorization;
namespace RestaurantReservation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RestaurantController : ControllerBase
{
    private readonly AppDbContext _context;

    public RestaurantController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("~/api/restaurants")]
    public async Task<ActionResult<IEnumerable<object>>> GetRestaurants()
    {
        var restaurants = await _context.Restaurants
            .Select(r => new {
                r.Id,
                r.Name,
                r.City,
                r.Description,
                r.Image,
                MenuItems = r.MenuItems.Select(m => new { m.Id, m.Name, m.Price, m.Description })
            })
            .ToListAsync();
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetRestaurant(int id)
    {
        var restaurant = await _context.Restaurants
            .Select(r => new {
                r.Id,
                r.Name,
                r.City,
                r.Description,
                r.Image,
                MenuItems = r.MenuItems.Select(m => new { m.Id, m.Name, m.Price, m.Description })
            })
            .FirstOrDefaultAsync(r => r.Id == id);

        if (restaurant == null)
            return NotFound(new { message = "Restaurantul nu a fost găsit." });

        return Ok(restaurant);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateRestaurant([FromBody] Restaurant restaurant)
    {
        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id }, restaurant);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] Restaurant restaurant)
    {
        if (id != restaurant.Id) return BadRequest();

        _context.Entry(restaurant).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Restaurants.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null) return NotFound();

        _context.Restaurants.Remove(restaurant);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}