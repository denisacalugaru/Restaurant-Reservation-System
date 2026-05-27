using RestaurantReservation.API.Models;
using RestaurantReservation.API.Repositories;
using Microsoft.Extensions.Logging;

namespace RestaurantReservation.API.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly ILogger<ReservationService> _logger;

    public ReservationService(IReservationRepository repository, ILogger<ReservationService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Reservation>> GetReservationsAsync()
    {
        _logger.LogInformation("Preluare listă rezervări.");
        return await _repository.GetAllAsync();
    }

    public async Task<Reservation> GetReservationByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task CreateReservationAsync(Reservation reservation)
    {
        _logger.LogInformation($"Adăugare rezervare nouă pentru masa: {reservation.TableId}");
        await _repository.AddAsync(reservation);
    }

    public async Task UpdateReservationAsync(Reservation reservation) => await _repository.UpdateAsync(reservation);

    public async Task DeleteReservationAsync(int id)
    {
        _logger.LogWarning($"Ștergere rezervare cu ID: {id}");
        await _repository.DeleteAsync(id);
    }
}