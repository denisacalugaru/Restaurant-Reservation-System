using Microsoft.EntityFrameworkCore;
using RestaurantReservation.API.Data;
using RestaurantReservation.API.Models;

namespace RestaurantReservation.API.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;
    public ReservationRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Reservation>> GetAllAsync() => await _context.Reservations.ToListAsync();
    public async Task<Reservation> GetByIdAsync(int id) => await _context.Reservations.FindAsync(id);
    public async Task AddAsync(Reservation reservation) { await _context.Reservations.AddAsync(reservation); await _context.SaveChangesAsync(); }
    public async Task UpdateAsync(Reservation reservation) { _context.Reservations.Update(reservation); await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(int id)
    {
        var r = await GetByIdAsync(id);
        if (r != null) { _context.Reservations.Remove(r); await _context.SaveChangesAsync(); }
    }
}