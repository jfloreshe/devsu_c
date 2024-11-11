using DevsuCustomer.Api.Infrastructure.Persistence;
using DevsuCustomer.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevsuCustomer.Api.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
        builder.Services.AddDbContext<CustomerDbContext>(o => o.UseInMemoryDatabase("DevsuCustomers"));
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    }
}