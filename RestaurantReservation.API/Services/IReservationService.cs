using RestaurantReservation.API.Models;

namespace RestaurantReservation.API.Services;

public interface IReservationService
{
    Task<IEnumerable<Reservation>> GetReservationsAsync();
    Task<Reservation> GetReservationByIdAsync(int id);
    Task CreateReservationAsync(Reservation reservation);
    Task UpdateReservationAsync(Reservation reservation);
    Task DeleteReservationAsync(int id);
}