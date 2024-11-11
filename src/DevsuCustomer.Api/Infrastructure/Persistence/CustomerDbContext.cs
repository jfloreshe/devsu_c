using DevsuCustomer.Api.Infrastructure.Persistence.EntityTypeConfigurations;
using DevsuCustomer.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevsuCustomer.Api.Infrastructure.Persistence;

public class CustomerDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    
    public CustomerDbContext(){}
    public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PersonEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
    }
}