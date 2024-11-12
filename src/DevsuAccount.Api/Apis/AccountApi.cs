using DevsuAccount.Api.Features;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DevsuAccount.Api.Apis;

public static class AccountApi
{
    public static class Routes
    {
        public const string AccountGroupName = "api/cuentas";
        public const string AccountTransactionGroupName = "api/movimientos";
    }

    public static void MapAccountApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Routes.AccountGroupName);
        api.MapPost("/", CreateAccount);
        api.MapGet("/{numeroCuenta}", GetAccount);
        api.MapPut("/", UpdateAccount);
    }
    
    public static void MapAccountTransactionApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Routes.AccountTransactionGroupName);
    }
    
    public static async Task<Results<
        Created<CreateAccountResult>,
        Conflict<ProblemDetails>>>
        CreateAccount([FromBody] CreateAccountRequest request, IMediator mediator, HttpContext ctx)
    {
        var response = await mediator.Send(request);
        if (response.IsFailure)
        {
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = response.Error.Title,
                Detail = response.Error.Description,
                Status = StatusCodes.Status409Conflict
            });
        }
        var location = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{Routes.AccountGroupName}/{response.Value.NumeroCuenta}";
        return TypedResults.Created(location, response.Value);
    }
    
    public static async Task<Results<
        Ok<GetAccountResult>,
        NotFound<ProblemDetails>>>
        GetAccount([FromRoute] string numeroCuenta, IMediator mediator)
    {
        var response = await mediator.Send(new GetAccountRequest{AccountNumber = numeroCuenta});
        if (response.IsFailure)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = response.Error.Title,
                Detail = response.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }
        return TypedResults.Ok(response.Value);
    }
    
    public static async Task<Results<
        NoContent,
        NotFound<ProblemDetails>>>
        UpdateAccount([FromBody] UpdateAccountRequest request, IMediator mediator)
    {
        var response = await mediator.Send(request);
        if (response.IsFailure)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = response.Error.Title,
                Detail = response.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }
        
        return TypedResults.NoContent();
    }
}