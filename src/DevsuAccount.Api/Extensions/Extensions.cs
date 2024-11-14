using Devsu.Shared.BusEvent;
using DevsuAccount.Api.Infrastructure.Integration.RabbitMq;
using DevsuAccount.Api.Infrastructure.Persistence;
using DevsuAccount.Api.Models;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DevsuAccount.Api.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
        builder.Services.AddDbContext<AccountDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DevsuAccountDb")));
        
        //MassTransit RMQ
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("MsgBroker"));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);
        
        builder.Services.AddMassTransit(iBusConfig =>
        {
            iBusConfig.SetKebabCaseEndpointNameFormatter();
            iBusConfig.AddConsumer<CustomerCreatedIntegrationEventConsumer>();
            iBusConfig.AddConsumer<CustomerUpdatedIntegrationEventConsumer>();
            iBusConfig.AddConsumer<CustomerDeletedIntegrationEventConsumer>();
            iBusConfig.UsingRabbitMq((context, cfg) =>
            {
                var settings = context.GetRequiredService<RabbitMqSettings>();
                cfg.Host(settings.Host, settings.Port , settings.VirtualHost, h =>
                {
                    h.Username(settings.Username);
                    h.Password(settings.Password);
                });
                
                cfg.Message<BusIntegrationEventMessage>(x => x.SetEntityName(RabbitMqConstants.CustomerExchange));
                cfg.Publish<BusIntegrationEventMessage>(x => x.ExchangeType = RabbitMqConstants.ExchangeTypeDirect);
                
                cfg.ReceiveEndpoint(RabbitMqConstants.ConsumerCustomerCreatedEndPoint, e =>
                {
                    e.Bind(RabbitMqConstants.CustomerExchange, x =>
                    {
                        x.RoutingKey = RabbitMqConstants.ConsumerCustomerCreatedRoutingKey;
                        x.ExchangeType = RabbitMqConstants.ExchangeTypeDirect;
                    });
                    e.ConfigureConsumer<CustomerCreatedIntegrationEventConsumer>(context);
                });
                
                cfg.ReceiveEndpoint(RabbitMqConstants.ConsumerCustomerUpdatedEndPoint, e =>
                {
                    e.Bind(RabbitMqConstants.CustomerExchange, x =>
                    {
                        x.RoutingKey = RabbitMqConstants.ConsumerCustomerUpdatedRoutingKey;
                        x.ExchangeType = RabbitMqConstants.ExchangeTypeDirect;
                    });
                    e.ConfigureConsumer<CustomerUpdatedIntegrationEventConsumer>(context);
                });
                
                cfg.ReceiveEndpoint(RabbitMqConstants.ConsumerCustomerDeletedEndPoint, e =>
                {
                    e.Bind(RabbitMqConstants.CustomerExchange, x =>
                    {
                        x.RoutingKey = RabbitMqConstants.ConsumerCustomerDeletedRoutingKey;
                        x.ExchangeType = RabbitMqConstants.ExchangeTypeDirect;
                    });
                    e.ConfigureConsumer<CustomerDeletedIntegrationEventConsumer>(context);
                });
            });
        });
        
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
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