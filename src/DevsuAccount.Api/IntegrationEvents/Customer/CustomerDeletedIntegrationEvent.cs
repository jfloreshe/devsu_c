using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.IntegrationEvents.Customer;

public class CustomerDeletedIntegrationEvent : INotification
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
}

public class CustomerDeletedIntegrationEventHandler : INotificationHandler<CustomerDeletedIntegrationEvent>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAccountRepository _accountRepository;

    public CustomerDeletedIntegrationEventHandler(ICustomerRepository customerRepository, IAccountRepository accountRepository)
    {
        _customerRepository = customerRepository;
        _accountRepository = accountRepository;
    }

    public async Task Handle(CustomerDeletedIntegrationEvent request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindCustomer(request.CustomerId, cancellationToken);
        if (customer is not null)
        {
            _customerRepository.DeleteCustomer(customer);
        }

        var accounts = await _accountRepository.GetAccounts(request.CustomerId, cancellationToken);
        foreach (var account in accounts)
        {
            _accountRepository.DeleteAccount(account);
        }
        
        
    }
}

