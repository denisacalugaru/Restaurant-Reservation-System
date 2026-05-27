namespace RestaurantReservation.API.Models;

public class Reservation
{
    public int Id { get; set; }
    public DateTime ReservationDate { get; set; }
    public int NumberOfPersons { get; set; }
    public string AdditionalNotes { get; set; } = string.Empty;

    //o rezervare specifica se face la o singura masa
    public int TableId { get; set; } //foreign key
    public Table? Table { get; set; }

    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
}