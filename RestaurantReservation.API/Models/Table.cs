namespace RestaurantReservation.API.Models;

public class Table
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; } = true;

    public int RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>(); //one to many o masa poate avea mai multe rezervari
}