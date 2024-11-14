using System.Text.Json;
using DevsuAccount.Api.IntegrationEvents.Customer;
using MassTransit;
using MediatR;

namespace DevsuAccount.Api.Infrastructure.Integration.RabbitMq;

public sealed class CustomerCreatedIntegrationEventConsumer(ILogger<CustomerCreatedIntegrationEventConsumer> logger, IMediator mediator) : IConsumer<BusMessage>
{
    public async Task Consume(ConsumeContext<BusMessage> context)
    {
        logger.LogInformation("{Consumer} Received: \n {MessageData} \n at {CreationTime}",
            nameof(CustomerCreatedIntegrationEvent), context.Message.DataJson, context.Message.CreationDate);

        var integrationEvent = JsonSerializer.Deserialize<CustomerCreatedIntegrationEvent>(context.Message.DataJson);
        if (integrationEvent is null || integrationEvent.CustomerId == Guid.Empty)
        {
            return;
        }

        await mediator.Publish(integrationEvent);
    }
}

public sealed class CustomerUpdatedIntegrationEventConsumer(ILogger<CustomerUpdatedIntegrationEventConsumer> logger, IMediator mediator) : IConsumer<BusMessage>
{
    public async Task Consume(ConsumeContext<BusMessage> context)
    {
        logger.LogInformation("{Consumer} Received: \n {MessageData} \n at {CreationTime}",
            nameof(CustomerUpdatedIntegrationEvent), context.Message.DataJson, context.Message.CreationDate);
        
        var integrationEvent = JsonSerializer.Deserialize<CustomerUpdatedIntegrationEvent>(context.Message.DataJson);
        if (integrationEvent is null)
        {
            return;
        }
        
        await mediator.Publish(integrationEvent);
    }
}

public sealed class CustomerDeletedIntegrationEventConsumer(ILogger<CustomerDeletedIntegrationEventConsumer> logger, IMediator mediator) : IConsumer<BusMessage>
{
    public async Task Consume(ConsumeContext<BusMessage> context)
    {
        logger.LogInformation("{Consumer} Received: \n {MessageData} \n at {CreationTime}",
            nameof(CustomerDeletedIntegrationEvent), context.Message.DataJson, context.Message.CreationDate);
        
        var integrationEvent = JsonSerializer.Deserialize<CustomerDeletedIntegrationEvent>(context.Message.DataJson);
        if (integrationEvent is null)
        {
            return;
        }
        
        await mediator.Publish(integrationEvent);
    }
}

public record BusMessage(DateTime CreationDate, string DataJson);
