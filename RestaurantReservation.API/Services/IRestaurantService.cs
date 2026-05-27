using RestaurantReservation.API.Models;

namespace RestaurantReservation.API.Services;

public interface IRestaurantService
{
    Task<IEnumerable<Restaurant>> GetRestaurantsAsync();
    Task<Restaurant> GetRestaurantByIdAsync(Guid id);
    Task CreateRestaurantAsync(Restaurant restaurant);
    Task UpdateRestaurantAsync(Restaurant restaurant);
    Task DeleteRestaurantAsync(Guid id);
}