namespace RestaurantReservation.API.Models;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;

    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
}