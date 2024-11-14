using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.IntegrationEvents.Customer;

public class CustomerUpdatedIntegrationEvent : INotification
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public bool State { get; set; }
}

public class CustomerUpdatedIntegrationEventHandler : INotificationHandler<CustomerUpdatedIntegrationEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerUpdatedIntegrationEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CustomerUpdatedIntegrationEvent request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.CustomerRepository.FindCustomer(request.CustomerId, cancellationToken);
        var accounts = !request.State 
            ? await _unitOfWork.AccountRepository.GetActiveAccounts(request.CustomerId, cancellationToken) 
            : [];

        var dbTransaction = new IUnitOfWork.DbTransaction<int>(async () =>
        {
            if (customer is null)
            {
                await _unitOfWork.CustomerRepository.InsertCustomer(new AccountCustomer
                {
                    CustomerId = request.CustomerId,
                    Name = request.Name,
                    State = request.State
                }, cancellationToken);
            }
            else
            {
                customer.Name = request.Name;
                customer.State = request.State;
                await _unitOfWork.CustomerRepository.UpdateCustomer(customer, cancellationToken);
            }
            accounts.ForEach(a =>
            {
                a.State = false;
                _unitOfWork.AccountRepository.UpdateAccount(a);
            });
            return await _unitOfWork.SaveChanges();
        });

        await _unitOfWork.ExecuteTransaction(dbTransaction);
    }
}

