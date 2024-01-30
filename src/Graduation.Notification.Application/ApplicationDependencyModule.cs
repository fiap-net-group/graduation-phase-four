using FluentValidation;
using Graduation.Notification.Application.Email.Send;
using Graduation.Notification.Application.Email.Send.Boundaries;
using Graduation.Notification.Application.Request.CreateRequest;
using Graduation.Notification.Application.Request.CreateRequest.Boundaries;
using Graduation.Notification.Application.Request.GetRequest;
using Graduation.Notification.Application.Request.GetRequest.Boundaries;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Graduation.Notification.Application;

public static class ApplicationDependencyModule
{
    public static IServiceCollection AddApiApplicationConfiguration(this IServiceCollection services)
    {
        return services.AddApiUseCases()
                       .AddValidators()
                       .AddMappers();
    }

    public static IServiceCollection AddWorkerApplicationConfiguration(this IServiceCollection services)
    {
        return services.AddWorkerUseCases()
                       .AddValidators()
                       .AddMappers();
    }

    private static IServiceCollection AddApiUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICreateRequestUseCase, CreateRequestInteractor>();
        services.AddScoped<IGetRequestUseCase, GetRequestInteractor>();
        services.AddScoped<IUpdateRequestStatusUseCase, UpdateRequestStatusInteractor>();

        return services;
    }

    private static IServiceCollection AddWorkerUseCases(this IServiceCollection services)
    {
        services.AddScoped<ISendEmailUseCase, SendEmailInteractor>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<GetRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateRequestStatusValidator>();

        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        SendEmailMapper.Add();

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        return services;
    }

}
