using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.IntegrationEvents.Customer;

public class CustomerUpdatedIntegrationEvent : INotification
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
}

public class CustomerUpdatedIntegrationEventHandler : INotificationHandler<CustomerUpdatedIntegrationEvent>
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerUpdatedIntegrationEventHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task Handle(CustomerUpdatedIntegrationEvent request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindCustomer(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            await _customerRepository.InsertCustomer(new AccountCustomer
            {
                CustomerId = request.CustomerId,
                Name = request.Name
            }, cancellationToken);
            return;
        }
        await _customerRepository.UpdateCustomer(customer, cancellationToken);
    }
}

