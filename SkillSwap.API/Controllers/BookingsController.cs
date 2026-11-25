using Microsoft.AspNetCore.Mvc;
using SkillSwap.Application.DTOs;
using SkillSwap.Application.Interfaces;

namespace SkillSwap.API.Controllers;

[ApiController, Route("api/bookings")]
public sealed class BookingsController(IBookingService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BookingDto>> Post(CreateBookingRequest request, CancellationToken ct) => Ok(await service.CreateAsync(request, ct));
    [HttpPost("{id:int}/accept")] public Task<BookingDto> Accept(int id, [FromQuery] int teacherId, CancellationToken ct) => service.AcceptAsync(id, teacherId, ct);
    [HttpPost("{id:int}/reject")] public Task<BookingDto> Reject(int id, [FromQuery] int teacherId, CancellationToken ct) => service.RejectAsync(id, teacherId, ct);
    [HttpPost("{id:int}/cancel")] public Task<BookingDto> Cancel(int id, [FromQuery] int userId, CancellationToken ct) => service.CancelAsync(id, userId, ct);
    [HttpPost("{id:int}/mark-delivered")] public Task<BookingDto> Delivered(int id, [FromQuery] int teacherId, CancellationToken ct) => service.MarkDeliveredAsync(id, teacherId, ct);
    [HttpPost("{id:int}/confirm-completion")] public Task<BookingDto> Complete(int id, [FromQuery] int learnerId, CancellationToken ct) => service.ConfirmCompletionAsync(id, learnerId, ct);
    [HttpPost("/reviews")] public async Task<IActionResult> Review(ReviewRequest request, CancellationToken ct) { await service.AddReviewAsync(request, ct); return NoContent(); }
}
