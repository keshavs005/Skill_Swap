using Microsoft.AspNetCore.Mvc;
using SkillSwap.Web.Services;

namespace SkillSwap.Web.Controllers;

public sealed class SkillsController(SkillSwapApiClient apiClient) : Controller
{
    public async Task<IActionResult> Index(string? search, CancellationToken ct) => View(await apiClient.GetSkillsAsync(search, ct));
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var skill = await apiClient.GetSkillAsync(id, ct);
        return skill is null ? NotFound() : View(skill);
    }
}
