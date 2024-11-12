using DevsuAccount.Api.Features;
using DevsuAccount.Api.Features.Account;
using DevsuAccount.Api.Features.Account.AccountTransaction;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DevsuAccount.Api.Apis;

public static class AccountApi
{
    public static class Routes
    {
        public const string AccountReportsGroupName = "api/reportes";
        public const string AccountGroupName = "api/cuentas";
        public const string AccountTransactionGroupName = "api/movimientos";
    }

    public static void MapAccountReports(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Routes.AccountReportsGroupName);
        api.MapGet("/", GetReportAccountState);
    }
    public static void MapAccountApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Routes.AccountGroupName);
        api.MapPost("/", CreateAccount);
        api.MapGet("/{numeroCuenta}", GetAccount);
        api.MapPut("/", UpdateAccount);
        api.MapPatch("/", PatchAccount);
        api.MapDelete("/{numerCuenta}", DeleteAccount);
    }
    
    public static void MapAccountTransactionApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Routes.AccountTransactionGroupName);
        api.MapPost("/", CreateAccountTransaction);
        api.MapGet("/{movimientoId:guid}", GetAccountTransaction);
        api.MapPut("/", UpdateAccountTransaction);
        api.MapPatch("/", PatchAccountTransaction);
        api.MapDelete("/{movimientoId:guid}", DeleteAccountTransaction);
    }
    
    public static async Task<Results<
        Created<CreateAccountResult>,
        UnprocessableEntity<ProblemDetails>,
        Conflict<ProblemDetails>>>
    CreateAccount([FromBody] CreateAccountRequest request, IMediator mediator, HttpContext ctx)
    {
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            if (result.Error.Code == StatusCodes.Status422UnprocessableEntity)
            {
                return TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
            }
            
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status409Conflict
            });
        }
        var location = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{Routes.AccountGroupName}/{result.Value.NumeroCuenta}";
        return TypedResults.Created(location, result.Value);
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
        UnprocessableEntity<ProblemDetails>,
        NotFound<ProblemDetails>>>
    UpdateAccount([FromBody] UpdateAccountRequest request, IMediator mediator)
    {
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            if (result.Error.Code == StatusCodes.Status422UnprocessableEntity)
            {
                return TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
            }
            
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }
        
        return TypedResults.NoContent();
    }
    
    public static async Task<Results<
        NoContent,
        UnprocessableEntity<ProblemDetails>,
        NotFound<ProblemDetails>>>
    PatchAccount([FromBody] PatchAccountRequest request, IMediator mediator)
    {
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            if (result.Error.Code == StatusCodes.Status422UnprocessableEntity)
            {
                return TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
            }
            
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }
        
        return TypedResults.NoContent();
    }
    
    public static async Task<Results<
        NoContent,
        NotFound<ProblemDetails>>>
    DeleteAccount([FromRoute] string numerCuenta, IMediator mediator)
    {
        //This makes a hard delete (be careful using it)
        //In case you need soft delete use PatchAccount or UpdateAccount changing the state should be enough
        var response = await mediator.Send(new DeleteAccountRequest { AccountNumber = numerCuenta });
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
    
    public static async Task<Results<
        Created<CreateAccountTransactionResult>,
        UnprocessableEntity<ProblemDetails>,
        Conflict<ProblemDetails>>>
    CreateAccountTransaction([FromBody] CreateAccountTransactionRequest request, IMediator mediator, HttpContext ctx)
    {
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            if (result.Error.Code == StatusCodes.Status422UnprocessableEntity)
            {
                return TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
            }
            
            return TypedResults.Conflict(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status409Conflict
            });
        }
        var location = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{Routes.AccountGroupName}/{result.Value.MovimientoId}";
        return TypedResults.Created(location, result.Value);
    }
    
    public static async Task<Results<
        Ok<GetAccountTransactionResult>,
        NotFound<ProblemDetails>>> 
    GetAccountTransaction([FromRoute] Guid movimientoId, IMediator mediator)
    {
        var response = await mediator.Send(new GetAccountTransactionRequest{AccountTransactionId = movimientoId});
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
        UnprocessableEntity<ProblemDetails>,
        Conflict<ProblemDetails>,
        NotFound<ProblemDetails>>>
    UpdateAccountTransaction([FromBody] UpdateAccountTransactionRequest request, IMediator mediator)
    {
        //This update will modify also all the movements created after the main movement
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            if (result.Error.Code == StatusCodes.Status422UnprocessableEntity)
            {
                return TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
            }
            
            if (result.Error.Code == StatusCodes.Status409Conflict)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status409Conflict
                });
            }
            
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }
        
        return TypedResults.NoContent();
    }
    
    public static async Task<Results<
        NoContent,
        UnprocessableEntity<ProblemDetails>,
        Conflict<ProblemDetails>,
        NotFound<ProblemDetails>>>
    PatchAccountTransaction([FromBody] PatchAccountTransactionRequest request, IMediator mediator)
    {
        //This update will modify also all the movements created after the main movement
        var result = await mediator.Send(request);
        if (result.IsFailure)
        {
            if (result.Error.Code == StatusCodes.Status422UnprocessableEntity)
            {
                return TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
            }
            
            if (result.Error.Code == StatusCodes.Status409Conflict)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status409Conflict
                });
            }
            
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }
        
        return TypedResults.NoContent();
    }
    
    public static async Task<Results<
        NoContent,
        UnprocessableEntity<ProblemDetails>,
        Conflict<ProblemDetails>,
        NotFound<ProblemDetails>>>
    DeleteAccountTransaction([FromRoute] Guid movimientoId, IMediator mediator)
    {
        //This update will modify also all the movements created after the main movement
        var result = await mediator.Send(new DeleteAccountTransactionRequest { AccountTransactionId = movimientoId });
        if (result.IsFailure)
        {
            if (result.Error.Code == StatusCodes.Status422UnprocessableEntity)
            {
                return TypedResults.UnprocessableEntity(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status422UnprocessableEntity
                });
            }
            
            if (result.Error.Code == StatusCodes.Status409Conflict)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = result.Error.Title,
                    Detail = result.Error.Description,
                    Status = StatusCodes.Status409Conflict
                });
            }
            
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = result.Error.Title,
                Detail = result.Error.Description,
                Status = StatusCodes.Status404NotFound
            });
        }
        
        return TypedResults.NoContent();
    }
    
    public static async Task<
        Ok<GetAccountStateReportResult>>
    GetReportAccountState([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin, [FromQuery] Guid clienteId, [FromQuery] int? pagina, [FromQuery] int? tamanoPagina,IMediator mediator)
    {
        
        var result = await mediator.Send(new GetAccountStateReportRequest
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            CustomerId =clienteId,
            Page = pagina,
            Size = tamanoPagina
        });
                
        return TypedResults.Ok(result);
    }
}