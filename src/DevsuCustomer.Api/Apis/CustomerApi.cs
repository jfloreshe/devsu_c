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
        api.MapPut("/", UpdateCustomer);
        api.MapDelete("/{clienteId:guid}", DeleteCustomer);
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

    public static async Task<Results<Ok<GetCustomerResult>, NotFound<ProblemDetails>>> GetCustomer([FromRoute] Guid clienteId, IMediator mediator)
    {
        var result = await mediator.Send(new GetCustomerRequest { CustomerId = clienteId });
        if (result.IsFailure)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }

        return TypedResults.Ok(result.Value);
    }

    public static async Task<Results<NoContent, NotFound<ProblemDetails>>> UpdateCustomer([FromBody] UpdateCustomerRequest request, IMediator mediator)
    {
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }

        return TypedResults.NoContent();
    }
    
    public static async Task<Results<NoContent, NotFound<ProblemDetails>>> DeleteCustomer([FromRoute] Guid clienteId, IMediator mediator)
    {
        //This makes a hard delete (be careful using it)
        //In case you need soft delete use PatchCustomer or UpdateCustomer changing the state should be enough
        
        var result = await mediator.Send(new DeleteCustomerRequest { CustomerId = clienteId });
        if (result.IsFailure)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }

        return TypedResults.NoContent();
    }
}