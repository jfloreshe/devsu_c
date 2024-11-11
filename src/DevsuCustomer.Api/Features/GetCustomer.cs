using DevsuCustomer.Api.Models;
using DevsuCustomer.Api.Models.Primitives;
using MediatR;

namespace DevsuCustomer.Api.Features;

public static class GetCustomerErrors
{
    public const string Title = "Buscar cliente";
    
    public static readonly Error CustomerNotFound = new(
        Title,
        "No se encontró al cliente");
}
public class GetCustomerResult
{
    public Guid ClienteId { get; set; }
    public string Contrasena { get; set; }
    public string Identificacion { get; set; }
    public string Nombre { get; set; }
    public string Genero { get; set; }
    public int Edad { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public bool Estado { get; set; }
}

public class GetCustomerRequest : IRequest<Result<GetCustomerResult>>
{
    public Guid CustomerId { get; set; }
}

public class GetCustomerRequestHandler : IRequestHandler<GetCustomerRequest, Result<GetCustomerResult>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerRequestHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<GetCustomerResult>> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindCustomer(request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result<GetCustomerResult>.Failure(GetCustomerErrors.CustomerNotFound);
        }

        return Result<GetCustomerResult>.Success(new GetCustomerResult
        {
            ClienteId = customer.CustomerId,
            Contrasena = customer.Password,
            Identificacion = customer.PersonalIdentifier,
            Nombre = customer.Name,
            Genero = customer.Gender,
            Edad = customer.Age,
            Direccion = customer.Address,
            Telefono = customer.Phone,
            Estado = customer.State
        });
    }
}