using RestaurantReservation.API.Models;

namespace RestaurantReservation.API.Repositories;

public interface IRestaurantRepository
{
    Task<IEnumerable<Restaurant>> GetAllAsync();
    Task<Restaurant> GetByIdAsync(Guid id);
    Task AddAsync(Restaurant restaurant);
    Task UpdateAsync(Restaurant restaurant);
    Task DeleteAsync(Guid id);
}