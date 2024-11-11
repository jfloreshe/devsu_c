﻿using System.Text.Json;
using DevsuCustomer.Api.Infrastructure.Persistence;
using DevsuCustomer.Api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevsuCustomer.Api.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
        // builder.Services.AddDbContext<CustomerDbContext>(o => o.UseInMemoryDatabase("DevsuCustomers"));
        builder.Services.AddDbContext<CustomerDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DevsuCustomerDb")));
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
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