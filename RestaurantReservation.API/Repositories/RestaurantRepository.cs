using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Data;
using RestaurantReservation.API.Models;

namespace RestaurantReservation.API.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly AppDbContext _context;
    public RestaurantRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Restaurant>> GetAllAsync() => await _context.Restaurants.ToListAsync();
    public async Task<Restaurant> GetByIdAsync(Guid id) => await _context.Restaurants.FindAsync(id);
    public async Task AddAsync(Restaurant restaurant) { await _context.Restaurants.AddAsync(restaurant); await _context.SaveChangesAsync(); }
    public async Task UpdateAsync(Restaurant restaurant) { _context.Restaurants.Update(restaurant); await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(Guid id)
    {
        var r = await GetByIdAsync(id);
        if (r != null) { _context.Restaurants.Remove(r); await _context.SaveChangesAsync(); }
    }
}