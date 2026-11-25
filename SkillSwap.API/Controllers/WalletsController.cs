using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.API.Controllers;

[ApiController, Route("api/wallets")]
public sealed class WalletsController(SkillSwapDbContext db) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> Get(int userId, CancellationToken ct) => (await db.Wallets.AsNoTracking().SingleOrDefaultAsync(x => x.UserProfileId == userId, ct)) is { } wallet ? Ok(wallet) : NotFound();
    [HttpGet("{userId:int}/transactions")]
    public async Task<IActionResult> Transactions(int userId, CancellationToken ct) => Ok(await db.CreditTransactions.AsNoTracking().Where(x => x.FromWallet!.UserProfileId == userId || x.ToWallet!.UserProfileId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync(ct));
}
