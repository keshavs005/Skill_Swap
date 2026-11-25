using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkillSwap.Application.Interfaces;
using SkillSwap.Application.Services;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSkillSwapInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SkillSwapDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("SkillSwapConnection")));
        services.AddScoped<ISkillSwapRepository, EfSkillSwapRepository>();
        services.AddScoped<IBookingService, BookingService>();
        return services;
    }
}
