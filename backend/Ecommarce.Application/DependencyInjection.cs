namespace Ecommarce.Application;

using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // Services
        services.AddScoped<Services.Interfaces.IProductService, Services.Implementations.ProductService>();
        services.AddScoped<Services.Interfaces.ICategoryService, Services.Implementations.CategoryService>();
        services.AddScoped<Services.Interfaces.ICartService, Services.Implementations.CartService>();
        services.AddScoped<Services.Interfaces.IAuthService, Services.Implementations.AuthService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
