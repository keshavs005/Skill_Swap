using System.Net.Http.Json;
using SkillSwap.Web.Models;

namespace SkillSwap.Web.Services;

public sealed class SkillSwapApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<SkillCard>> GetSkillsAsync(
        string? search = null,
        CancellationToken ct = default)
    {
        var requestUri = "api/skills" +
            (string.IsNullOrWhiteSpace(search)
                ? ""
                : $"?search={Uri.EscapeDataString(search)}");

        try
        {
            return await httpClient.GetFromJsonAsync<List<SkillCard>>(requestUri, ct) ?? [];
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // The browser/request was canceled. Treat it as an empty result instead
            // of allowing a TaskCanceledException to escape to the MVC action.
            return [];
        }
    }

    public async Task<SkillDetailsViewModel?> GetSkillAsync(
        int id,
        CancellationToken ct = default)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<SkillDetailsViewModel>($"api/skills/{id}", ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            return null;
        }
    }
}
