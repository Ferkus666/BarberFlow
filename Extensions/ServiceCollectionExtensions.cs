using BarberFlow.Application.Mappings;
using BarberFlow.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace BarberFlow.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<RegistroViewModelValidator>();
        return services;
    }
}
