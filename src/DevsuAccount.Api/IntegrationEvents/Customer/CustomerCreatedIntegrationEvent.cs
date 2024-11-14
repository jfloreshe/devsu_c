using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.IntegrationEvents.Customer;

public class CustomerCreatedIntegrationEvent : INotification
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
}

public class CustomerCreatedIntegrationEventHandler : INotificationHandler<CustomerCreatedIntegrationEvent>
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerCreatedIntegrationEventHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task Handle(CustomerCreatedIntegrationEvent request, CancellationToken cancellationToken)
    {
        await _customerRepository.InsertCustomer(new AccountCustomer
        {
            CustomerId = request.CustomerId,
            Name = request.Name
        }, cancellationToken);
    }
}

