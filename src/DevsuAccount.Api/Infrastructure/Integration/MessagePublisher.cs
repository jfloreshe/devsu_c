using System.Text.Json;
using DevsuAccount.Api.Infrastructure.Integration.RabbitMq;
using DevsuAccount.Api.Models;
using MassTransit;

namespace DevsuAccount.Api.Infrastructure.Integration;

internal class MessagePublisher(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            Random random = new Random();
            var i = random.Next(1000) % 3;
            string routingKey = i switch
            {
                0 => RabbitMqConstants.ConsumerCustomerCreatedRoutingKey,
                1 => RabbitMqConstants.ConsumerCustomerUpdatedRoutingKey,
                _ => RabbitMqConstants.ConsumerCustomerDeletedRoutingKey
            };

            var message = new BusMessage(
                DateTime.Now,
                JsonSerializer.Serialize(new AccountCustomer
                {
                    CustomerId = Guid.NewGuid(),
                    Name = "John Doe"
                }));

            Console.WriteLine($"publishing message: {routingKey}");
            await publishEndpoint.Publish(message, context =>
            {
                context.SetRoutingKey(routingKey);
            }, cancellationToken);

            await Task.Delay(2000, cancellationToken);
        }
    }
}