using DevsuCustomer.Api.Features;
using DevsuCustomer.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DevsuCustomer.Api.Apis;

public static class CustomerApi
{
    public static class Routes
    {
        public const string GroupName = "api/clientes";
    }

    public static void MapCustomerApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Routes.GroupName);

        api.MapPost("/", CreateCustomer);
        api.MapGet("/{clienteId:guid}", GetCustomer);
    }
    public static async Task<Results<Created<CreateCustomerResult>,Conflict<ProblemDetails>>> CreateCustomer([FromBody] CreateCustomerRequest request, HttpContext ctx, IMediator mediator)
    {
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status409Conflict
            });
        }
        var location = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{Routes.GroupName}/{result.Value.ClienteId}";
        return TypedResults.Created(location, result.Value);
    }

    public static async Task<Results<Ok<GetCustomerResult>, NotFound<ProblemDetails>>> GetCustomer([FromRoute] Guid clienteId, ICustomerRepository customerRepository)
    {
        var customer = await customerRepository.FindCustomer(clienteId);
        if (customer is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Buscar cliente",
                Detail = "No se encontró al cliente",
                Status = StatusCodes.Status404NotFound
            });
        }
        
        return TypedResults.Ok(new GetCustomerResult
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