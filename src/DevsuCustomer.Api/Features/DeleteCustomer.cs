using Devsu.Shared.Primitives;
using DevsuCustomer.Api.IntegrationEvents;
using DevsuCustomer.Api.Models;
using DevsuCustomer.Api.Models.DomainEvents;
using MediatR;

namespace DevsuCustomer.Api.Features;

public static class DeleteCustomerErrors
{
    public const string Title = "ELiminar cliente";
    public static readonly Error CustomerNotFound = new(Title, "No se encontró al cliente"); 
}

public class DeleteCustomerResult
{
    public Guid? CustomerId { get; set; }
}

public class DeleteCustomerRequest : IRequest<Result<DeleteCustomerResult>>
{
    public Guid CustomerId { get; set; }
}

public class DeleteCustomerRequestHander : IRequestHandler<DeleteCustomerRequest, Result<DeleteCustomerResult>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBusIntegrationEvent _busEvent;

    public DeleteCustomerRequestHander(ICustomerRepository customerRepository, IBusIntegrationEvent busEvent)
    {
        _customerRepository = customerRepository;
        _busEvent = busEvent;
    }

    public async Task<Result<DeleteCustomerResult>> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindCustomer(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<DeleteCustomerResult>.Failure(DeleteCustomerErrors.CustomerNotFound);
        }
        
        _customerRepository.DeleteCustomer(customer);
        
        await _customerRepository.SaveEntities(cancellationToken);
        
        //we should use outbox pattern here, but we are using an optimistic strategy
        await _busEvent.PublishCustomerIntegrationEvent(new CustomerDeletedDomainEvent
        {
            CustomerId = customer.CustomerId
        }, cancellationToken);

        return Result<DeleteCustomerResult>.Success(new DeleteCustomerResult
        {
            CustomerId = customer.CustomerId
        });
    }
}