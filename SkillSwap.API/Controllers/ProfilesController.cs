using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.API.Controllers;

[ApiController, Route("api/profiles")]
public sealed class ProfilesController(SkillSwapDbContext db) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct) => (await db.UserProfiles.AsNoTracking().Include(x => x.SkillListings).SingleOrDefaultAsync(x => x.Id == id, ct)) is { } user ? Ok(user) : NotFound();
}
