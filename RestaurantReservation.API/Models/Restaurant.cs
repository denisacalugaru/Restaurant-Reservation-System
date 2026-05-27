namespace RestaurantReservation.API.Models;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;

    public ICollection<Table> Tables { get; set; } = new List<Table>();
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>(); //one to many un restaurant are mai multe preparate
    public ICollection<Category> Categories { get; set; } = new List<Category>(); //many to many un restaurant poate fi din mai multe categorii ex pizzerie fast-food, etc
}