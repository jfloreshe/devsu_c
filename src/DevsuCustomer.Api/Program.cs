using DevsuCustomer.Api.Apis;
using DevsuCustomer.Api.Infrastructure.Persistence;
using DevsuCustomer.Api.Models;
using DevsuCustomer.Api.Models.Primitives;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddDbContext<CustomerDbContext>(options => options.UseInMemoryDatabase("DevsuCustomers"));

var app = builder.Build();
app.MapCustomerApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
