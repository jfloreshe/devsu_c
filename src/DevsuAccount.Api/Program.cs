using DevsuAccount.Api.Apis;
using DevsuAccount.Api.Extensions;
using DevsuAccount.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddApplicationServices();

var app = builder.Build();
app.AddApiMiddlewareException();
app.MapAccountApi();
app.MapAccountTransactionApi();
app.MapAccountReports();

if (builder.Configuration.GetValue<bool>("Development"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    
    var pendingMigrations = context.Database.GetPendingMigrations();
        
    if (pendingMigrations.Any())
    {
        context.Database.Migrate();
    }
}

app.UseHttpsRedirection();
app.Run();