using DevsuCustomer.Api.Apis;
using DevsuCustomer.Api.Extensions;
using DevsuCustomer.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddApplicationServices();

var app = builder.Build();
app.AddApiMiddlewareException();
app.MapCustomerApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    
    var pendingMigrations = context.Database.GetPendingMigrations();
        
    if (pendingMigrations.Any())
    {
        context.Database.Migrate();
    }
}

app.UseHttpsRedirection();
app.Run();
