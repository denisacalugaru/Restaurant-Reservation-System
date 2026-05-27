using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.API.Models;
using RestaurantReservation.API.Services;
using RestaurantReservation.API.DTOs;

namespace RestaurantReservation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")] //403
    public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
    {
        var reservations = await _reservationService.GetReservationsAsync();
        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetReservation(int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);

        if (reservation == null)
            return NotFound(new { message = "Rezervarea nu a fost găsită." }); //404

        return Ok(reservation); //200
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); //400
        }

        var reservation = new Reservation
        {
            ReservationDate = dto.ReservationDate,
            NumberOfPersons = dto.NumberOfPersons,
            AdditionalNotes = dto.AdditionalNotes,
            TableId = dto.TableId,
            UserId = dto.UserId
        };

        await _reservationService.CreateReservationAsync(reservation);

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation); //201
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation reservation)
    {
        if (id != reservation.Id) return BadRequest(new { message = "ID-urile nu coincid." });

        try
        {
            await _reservationService.UpdateReservationAsync(reservation);
        }
        catch
        {
            var existing = await _reservationService.GetReservationByIdAsync(id);
            if (existing == null) return NotFound();
            throw;
        }

        return NoContent(); //204
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null) return NotFound();

        await _reservationService.DeleteReservationAsync(id);
        return NoContent();
    }
}