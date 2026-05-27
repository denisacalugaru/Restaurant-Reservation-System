using Microsoft.AspNetCore.Identity;

namespace RestaurantReservation.API.Models;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}