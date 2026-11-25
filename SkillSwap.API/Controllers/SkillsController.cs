using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.DTOs;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.API.Controllers;

[ApiController, Route("api/skills")]
public sealed class SkillsController(SkillSwapDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SkillListItem>>> Get(string? search, int? categoryId, CancellationToken ct)
        => Ok(await db.SkillListings.AsNoTracking().Include(x => x.Teacher).Include(x => x.Category).Where(x => x.IsActive && (categoryId == null || x.CategoryId == categoryId) && (string.IsNullOrWhiteSpace(search) || x.Title.Contains(search) || x.Description.Contains(search))).Select(x => new SkillListItem(x.Id, x.Title, x.Description, x.Teacher.DisplayName, x.Category.Name)).ToListAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SkillDetails>> Get(int id, CancellationToken ct)
    {
        var item = await db.SkillListings.AsNoTracking().Include(x => x.Teacher).Include(x => x.Category).Where(x => x.Id == id).Select(x => new SkillDetails(x.Id, x.Title, x.Description, x.TeacherId, x.Teacher.DisplayName, x.Category.Name)).FirstOrDefaultAsync(ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateSkillRequest request, CancellationToken ct)
    {
        var listing = new SkillSwap.Domain.Entities.SkillListing { TeacherId = request.TeacherId, CategoryId = request.CategoryId, Title = request.Title, Description = request.Description };
        db.SkillListings.Add(listing); await db.SaveChangesAsync(ct); return CreatedAtAction(nameof(Get), new { id = listing.Id }, listing.Id);
    }
}
