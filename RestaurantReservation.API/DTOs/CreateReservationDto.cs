namespace RestaurantReservation.API.DTOs;

public class CreateReservationDto
{
    public DateTime ReservationDate { get; set; }
    public int NumberOfPersons { get; set; }
    public string AdditionalNotes { get; set; } = string.Empty;
    public int TableId { get; set; }
    public string UserId { get; set; } = string.Empty;
}