using DevsuAccount.Api.Infrastructure.Integration;
using DevsuAccount.Api.Infrastructure.Persistence;
using DevsuAccount.Api.Models;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevsuAccount.Api.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
        // builder.Services.AddDbContext<AccountDbContext>(o =>
        // {
        //     o.EnableSensitiveDataLogging();
        //     o.UseInMemoryDatabase("DevsuAccounts");
        // });
        builder.Services.AddDbContext<AccountDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DevsuAccountDb")));
        builder.Services.AddMassTransit(iBusConfig =>
        {
            iBusConfig.SetKebabCaseEndpointNameFormatter();
            iBusConfig.AddConsumer<CurrentTimeConsumer>();
            iBusConfig.UsingInMemory((iBusContext, configInMemory) => configInMemory.ConfigureEndpoints(iBusContext));
        });
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddHostedService<MessagePublisher>();
    }

    public static void AddApiMiddlewareException(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                ctx.Response.ContentType = "application/json";
                var title = "Internal Server Error";
                var details = "No se pudo procesar su request";
                
                var ctxFeature = ctx.Features.Get<IExceptionHandlerFeature>();
                
                if (ctxFeature is not null)
                {
                    if (ctxFeature.Error is BadHttpRequestException badRequestError)
                    {
                        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                        title = badRequestError.Message;
                        details = badRequestError.InnerException?.Message ?? details;
                    }
                    
                    Console.WriteLine($"Error: {ctxFeature.Error}");

                    await ctx.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = ctx.Response.StatusCode,
                        Title = title,
                        Detail = details
                    });
                }
            });
        });
    }
}