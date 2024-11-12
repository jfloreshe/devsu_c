using Devsu.Shared.Primitives;
using DevsuCustomer.Api.Infrastructure.Persistence;
using DevsuCustomer.Api.Models;
using MediatR;

namespace DevsuCustomer.Api.Features;

public static class UpdateCustomerErrors
{
    public const string Title = "Actualizar cliente";
    
    public static readonly Error CustomerNotFound = new(
        Title,
        "No se encontró al cliente");
}

public class UpdateCustomerResult
{
    public Guid? CustomerId { get; set; }
}

public class UpdateCustomerRequest : IRequest<Result<UpdateCustomerResult>>
{
    
    public required Guid ClienteId { get; set; }
    public required string Contrasena { get; set; }
    public required string Identificacion { get; set; }
    public required string Nombre { get; set; }
    public required string Genero { get; set; }
    public required int Edad { get; set; }
    public required string Direccion { get; set; }
    public required string Telefono { get; set; }
    public required bool Estado { get; set; }
}

public class UpdateCustomerRequestHandler : IRequestHandler<UpdateCustomerRequest, Result<UpdateCustomerResult>>
{
    private readonly ICustomerRepository _customerRepository;

    public UpdateCustomerRequestHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<UpdateCustomerResult>> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindCustomer(request.ClienteId, cancellationToken);
        if (customer is null)
        {
            return Result<UpdateCustomerResult>.Failure(UpdateCustomerErrors.CustomerNotFound);
        }
        
        customer.PersonalIdentifier = request.Identificacion;
        customer.Name = request.Nombre;
        customer.Gender = request.Genero;
        customer.Age = request.Edad;
        customer.Address = request.Direccion;
        customer.Phone = request.Telefono;
        customer.Password = request.Contrasena;
        customer.State = request.Estado;
        
        _customerRepository.UpdateCustomer(customer);
        await _customerRepository.SaveEntities(cancellationToken);

        return Result<UpdateCustomerResult>.Success(new UpdateCustomerResult
        {
            CustomerId = customer.CustomerId
        });
    }
}