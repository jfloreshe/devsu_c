using DevsuCustomer.Api.Models;
using DevsuCustomer.Api.Models.Primitives;
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
    public string Contrasena { get; set; }
    public string Identificacion { get; set; }
    public string Nombre { get; set; }
    public string Genero { get; set; }
    public int Edad { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
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

        Customer newCustomer = new (
            request.Identificacion,
            request.Nombre,
            request.Genero,
            request.Edad,
            request.Direccion,
            request.Telefono,
            request.Contrasena,
            true)
        {
            CustomerId = Guid.NewGuid()
        };

        _customerRepository.AddCustomer(newCustomer);

        await _customerRepository.SaveEntities(cancellationToken);
        
        return Result<CreateCustomerResult>.Success(new CreateCustomerResult
        {
            ClienteId = newCustomer.CustomerId,
            Nombre = newCustomer.Name
        });
    }
}