using Devsu.Shared.Primitives;
using DevsuCustomer.Api.Models;
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

    public DeleteCustomerRequestHander(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<DeleteCustomerResult>> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindCustomer(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<DeleteCustomerResult>.Failure(DeleteCustomerErrors.CustomerNotFound);
        }
        
        customer.State = false;
        _customerRepository.DeleteCustomer(customer);
        await _customerRepository.SaveEntities(cancellationToken);

        return Result<DeleteCustomerResult>.Success(new DeleteCustomerResult
        {
            CustomerId = customer.CustomerId
        });
    }
}