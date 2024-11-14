using Devsu.Shared.Primitives;
using DevsuCustomer.Api.IntegrationEvents;
using DevsuCustomer.Api.Models;
using DevsuCustomer.Api.Models.DomainEvents;
using MediatR;

namespace DevsuCustomer.Api.Features;

public static class PatchCustomerErrors
{
    public const string Title = "Actualizar cliente";
    public static readonly Error CustomerNotFound = new(Title, "No se encontró al cliente");
}

public class PatchCustomerResult
{
    public Guid? CustomerId { get; set; }
}

public class PatchCustomerRequest : IRequest<Result<PatchCustomerResult>>
{
    public Guid ClienteId { get; set; }
    public string? Contrasena { get; set; }
    public string? Identificacion { get; set; }
    public string? Nombre { get; set; }
    public string? Genero { get; set; }
    public int? Edad { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool? Estado { get; set; }
}

public class PatchCustomerRequestHandler : IRequestHandler<PatchCustomerRequest, Result<PatchCustomerResult>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBusIntegrationEvent _busEvent;
    public PatchCustomerRequestHandler(ICustomerRepository customerRepository, IBusIntegrationEvent busEvent)
    {
        _customerRepository = customerRepository;
        _busEvent = busEvent;
    }

    public async Task<Result<PatchCustomerResult>> Handle(PatchCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.FindCustomer(request.ClienteId, cancellationToken);
        if (customer is null)
        {
            return Result<PatchCustomerResult>.Failure(PatchCustomerErrors.CustomerNotFound);
        }

        bool publlishIntegrationEvent = false;
        if (request.Contrasena != null)
        {
            customer.Password = request.Contrasena;
        }
        
        if (request.Identificacion != null)
        {
            customer.PersonalIdentifier = request.Identificacion;
        }
        
        if (request.Nombre != null)
        {
            customer.Name = request.Nombre;
            publlishIntegrationEvent = true;
        }
        
        if (request.Genero != null)
        {
            customer.Gender = request.Genero;
        }
        
        if (request.Edad != null)
        {
            customer.Age = request.Edad.Value;
        }
        
        if (request.Direccion != null)
        {
            customer.Address = request.Direccion;
        }
        
        if (request.Telefono != null)
        {
            customer.Phone = request.Telefono;
        }
        
        if (request.Estado != null)
        {
            customer.State = request.Estado.Value;
            publlishIntegrationEvent = true;
        }
        
        _customerRepository.UpdateCustomer(customer);
        
        await _customerRepository.SaveEntities(cancellationToken);

        if (publlishIntegrationEvent)
        {
            //we should use outbox pattern here, but we are using an optimistic strategy
            await _busEvent.PublishCustomerIntegrationEvent(new CustomerUpdatedDomainEvent
            {
                CustomerId = customer.CustomerId,
                Name = customer.Name,
                State = customer.State
            }, cancellationToken);
        }
        
        return Result<PatchCustomerResult>.Success(new PatchCustomerResult
        {
            CustomerId = customer.CustomerId
        }); 
    }
}