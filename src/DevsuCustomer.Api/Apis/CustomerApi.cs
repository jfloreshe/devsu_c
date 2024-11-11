using Microsoft.AspNetCore.Http.HttpResults;

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

        api.MapGet("/", GetCustomer);
    }

    public static async Task<Ok> GetCustomer()
    {
        return TypedResults.Ok();
    }
}