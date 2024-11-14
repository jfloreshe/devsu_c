using Devsu.Shared.BusEvent;
using DevsuCustomer.Api.Infrastructure.Integration.RabbitMq;
using DevsuCustomer.Api.Infrastructure.Persistence;
using DevsuCustomer.Api.IntegrationEvents;
using DevsuCustomer.Api.Models;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DevsuCustomer.Api.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
        // builder.Services.AddDbContext<CustomerDbContext>(o => o.UseInMemoryDatabase("DevsuCustomers"));
        builder.Services.AddDbContext<CustomerDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DevsuCustomerDb")));
        
        //MassTransit RMQ
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("MsgBroker"));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);
        
        builder.Services.AddMassTransit(iBusConfig =>
        {
            iBusConfig.SetKebabCaseEndpointNameFormatter();
            iBusConfig.UsingRabbitMq((context, cfg) =>
            {
                var settings = context.GetRequiredService<RabbitMqSettings>();
                cfg.Host(settings.Host, settings.Port, settings.VirtualHost, h =>
                {
                    h.Username(settings.Username);
                    h.Password(settings.Password);
                });

                cfg.Message<BusIntegrationEventMessage>(x => x.SetEntityName(RabbitMqConstants.CustomerExchange));
                cfg.Publish<BusIntegrationEventMessage>(x => x.ExchangeType = RabbitMqConstants.ExchangeTypeDirect);
            });
        });

        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IBusIntegrationEvent, IntegrationEventProducer>();
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