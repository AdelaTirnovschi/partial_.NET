using System.Reflection;

namespace RoomHub.Api.Common.Cqrs;

public static class HandlerRegistrationExtensions
{
    public static IServiceCollection AddHandlers(
        this IServiceCollection services,
        Assembly assembly)
    {
        var handlerType = typeof(ICommandHandler<,>);

        var handlers = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract)
            .SelectMany(type => type.GetInterfaces()
                .Where(i => i.IsGenericType &&
                            i.GetGenericTypeDefinition() == handlerType)
                .Select(i => new
                {
                    Interface = i,
                    Implementation = type
                }));

        foreach (var handler in handlers)
        {
            services.AddScoped(
                handler.Interface,
                handler.Implementation);
        }

        return services;
    }
}