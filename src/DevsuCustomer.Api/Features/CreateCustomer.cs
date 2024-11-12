using Devsu.Shared.Primitives;
using DevsuCustomer.Api.Models;
using MediatR;

namespace DevsuCustomer.Api.Features;

public static class CreateCustomerErrors
{
    public const string Title = "Crear cliente";
    
    public static Error ExistingCustomer(string personalIdentifier) => new(
        Title,
        $"Ya existe un cliente con identificacion {personalIdentifier}");
}
public class CreateCustomerResult
{
    public Guid ClienteId { get; set; }
    public string Nombre { get; set; }
}
public class CreateCustomerRequest : IRequest<Result<CreateCustomerResult>>
{
    public required string Contrasena { get; set; }
    public required string Identificacion { get; set; }
    public required string Nombre { get; set; }
    public string Genero { get; set; }
    public int Edad { get; set; }
    public required string Direccion { get; set; }
    public required string Telefono { get; set; }
    public required bool Estado { get; set; }
}

public class CreateCustomerRequestHandler : IRequestHandler<CreateCustomerRequest, Result<CreateCustomerResult>>
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerRequestHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CreateCustomerResult>> Handle(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.FindCustomer(request.Identificacion, cancellationToken);
        
        if(customer is not null)
        {
            return Result<CreateCustomerResult>.Failure(CreateCustomerErrors.ExistingCustomer(customer.PersonalIdentifier));
        }

        var newCustomerResult = Customer.Create(
            personIdentifier: request.Identificacion,
            name: request.Nombre,
            gender: request.Genero,
            age: request.Edad,
            address: request.Direccion,
            phone: request.Telefono,
            password: request.Contrasena,
            state: request.Estado);
        
        if (newCustomerResult.IsFailure)
        {
            return Result<CreateCustomerResult>.Failure(newCustomerResult.Error);            
        }

        var newCustomer = newCustomerResult.Value;

        _customerRepository.AddCustomer(newCustomer);

        await _customerRepository.SaveEntities(cancellationToken);
        
        return Result<CreateCustomerResult>.Success(new CreateCustomerResult
        {
            ClienteId = newCustomer.CustomerId,
            Nombre = newCustomer.Name
        });
    }
}