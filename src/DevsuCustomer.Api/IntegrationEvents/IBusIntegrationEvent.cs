﻿using DevsuCustomer.Api.Models.DomainEvents;

namespace DevsuCustomer.Api.IntegrationEvents;

public interface IBusIntegrationEvent
{
    Task PublishCustomerIntegrationEvent(CustomerDomainEvent newEvent,
        CancellationToken cancellationToken = default);
}