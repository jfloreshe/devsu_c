using System.Text.Json;
using Devsu.Shared.BusEvent;
using DevsuCustomer.Api.IntegrationEvents;
using DevsuCustomer.Api.Models.DomainEvents;
using MassTransit;

namespace DevsuCustomer.Api.Infrastructure.Integration.RabbitMq;

public class IntegrationEventProducer : IBusIntegrationEvent
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<IntegrationEventProducer> _logger;
    public IntegrationEventProducer(IPublishEndpoint publishEndpoint, ILogger<IntegrationEventProducer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishCustomerIntegrationEvent(CustomerDomainEvent newEvent, CancellationToken cancellationToken = default)
    {
        var (routingKey, dataSerialized) = newEvent switch
        {
            CustomerCreatedDomainEvent e => (RabbitMqConstants.ConsumerCustomerCreatedRoutingKey, JsonSerializer.Serialize(e)),
            CustomerUpdatedDomainEvent e => (RabbitMqConstants.ConsumerCustomerUpdatedRoutingKey, JsonSerializer.Serialize(e)),
            CustomerDeletedDomainEvent e => (RabbitMqConstants.ConsumerCustomerDeletedRoutingKey, JsonSerializer.Serialize(e)),
            _ => throw new Exception("The event is not supported to be sent as a customer integration event")
        };
        
        var message = new BusIntegrationEventMessage(
            DateTime.Now,
            dataSerialized);
            
        _logger.LogInformation("publishing integration event: {@routingKey}", routingKey);
        await _publishEndpoint.Publish(message, context =>
        {
            context.SetRoutingKey(routingKey);
        }, cancellationToken);
    }
}