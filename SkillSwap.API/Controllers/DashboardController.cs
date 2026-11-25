using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.API.Controllers;

[ApiController, Route("api/dashboard")]
public sealed class DashboardController(SkillSwapDbContext db) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> Get(int userId, CancellationToken ct) => Ok(new { wallet = await db.Wallets.AsNoTracking().SingleOrDefaultAsync(x => x.UserProfileId == userId, ct), bookings = await db.Bookings.AsNoTracking().Include(x => x.SkillListing).ThenInclude(x => x.Teacher).Where(x => x.LearnerId == userId || x.SkillListing.TeacherId == userId).OrderBy(x => x.StartTime).ToListAsync(ct), listings = await db.SkillListings.AsNoTracking().Where(x => x.TeacherId == userId).ToListAsync(ct) });
}
