using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantReservation.API.Models;
using RestaurantReservation.API.Services;
using RestaurantReservation.API.DTOs;
using System.Security.Claims;

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
    public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
    {
        var reservations = await _reservationService.GetReservationsAsync();
        return Ok(reservations);
    }

    [HttpGet("my-reservations")]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetMyReservations([FromQuery] string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            userId = userIdFromToken;
        }

        var reservations = await _reservationService.GetReservationsAsync();

        if (!string.IsNullOrEmpty(userId))
        {
            reservations = reservations.Where(r => r.UserId == userId).ToList();
        }

        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetReservation(int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);

        if (reservation == null)
            return NotFound(new { message = "Rezervarea nu a fost găsită." });

        return Ok(reservation);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
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

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
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

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var reservation = await _reservationService.GetReservationByIdAsync(id);
        if (reservation == null) return NotFound();

        await _reservationService.DeleteReservationAsync(id);
        return NoContent();
    }
}