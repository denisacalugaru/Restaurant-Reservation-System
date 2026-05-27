using RestaurantReservation.API.Models;
using RestaurantReservation.API.Repositories;
using Microsoft.Extensions.Logging; // Necesar pentru Logging

namespace RestaurantReservation.API.Services;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _repository;
    private readonly ILogger<RestaurantService> _logger; // 

    public RestaurantService(IRestaurantRepository repository, ILogger<RestaurantService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Restaurant>> GetRestaurantsAsync()
    {
        _logger.LogInformation("Preluare listă restaurante.");
        return await _repository.GetAllAsync();
    }

    public async Task<Restaurant> GetRestaurantByIdAsync(Guid id) => await _repository.GetByIdAsync(id);

    public async Task CreateRestaurantAsync(Restaurant restaurant)
    {
        _logger.LogInformation($"Adăugare restaurant nou: {restaurant.Name}");
        await _repository.AddAsync(restaurant);
    }

    public async Task UpdateRestaurantAsync(Restaurant restaurant) => await _repository.UpdateAsync(restaurant);

    public async Task DeleteRestaurantAsync(Guid id)
    {
        _logger.LogWarning($"Ștergere restaurant cu ID: {id}");
        await _repository.DeleteAsync(id);
    }
}