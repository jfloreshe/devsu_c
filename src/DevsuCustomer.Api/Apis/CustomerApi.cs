using DevsuCustomer.Api.Features;
using DevsuCustomer.Api.Models;
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
        api.MapGet("/", GetCustomer);
    }
    public static async Task<Results<Created<CreateCustomerResult>,Conflict<ProblemDetails>>> CreateCustomer([FromBody] CreateCustomerRequest request, HttpContext ctx)
    {
        var newCustomer = new Customer(request.Identificacion, request.Nombre, request.Genero, request.Edad,
            request.Direccion, request.Telefono, request.Contrasena, true);
        newCustomer.CustomerId = Guid.NewGuid();
        
        var location = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{Routes.GroupName}/{newCustomer.CustomerId}";
        return TypedResults.Created(location, new CreateCustomerResult
        {
            ClienteId = newCustomer.CustomerId,
            Nombre = newCustomer.Name
        });
    }

    public static async Task<Ok> GetCustomer()
    {
        return TypedResults.Ok();
    }

}