namespace RestaurantReservation.API.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>(); // o categorie contine mai multe restaurante din oras 
}