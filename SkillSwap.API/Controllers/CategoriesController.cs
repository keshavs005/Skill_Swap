using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.DTOs;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.API.Controllers;

[ApiController, Route("api/categories")]
public sealed class CategoriesController(SkillSwapDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> Get(CancellationToken ct) => Ok(await db.SkillCategories.AsNoTracking().Select(x => new CategoryDto(x.Id, x.Name, x.Description)).ToListAsync(ct));
}
