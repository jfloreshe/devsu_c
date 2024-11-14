using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.IntegrationEvents.Customer;

public class CustomerDeletedIntegrationEvent : INotification
{
    public Guid CustomerId { get; set; }
}

public class CustomerDeletedIntegrationEventHandler : INotificationHandler<CustomerDeletedIntegrationEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerDeletedIntegrationEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CustomerDeletedIntegrationEvent request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.FindCustomer(request.CustomerId, cancellationToken);
        var accounts = await _unitOfWork.AccountRepository.GetActiveAccounts(request.CustomerId, cancellationToken);
        var dbTransaction = new IUnitOfWork.DbTransaction<int>(async () =>
        {
            if (customer is not null)
            {
                _unitOfWork.CustomerRepository.DeleteCustomer(customer);
            }

            foreach (var account in accounts)
            {
                _unitOfWork.AccountRepository.DeleteAccount(account);
            }

            return await _unitOfWork.SaveChanges();
        });
        
        await _unitOfWork.ExecuteTransaction(dbTransaction);
    }
}

