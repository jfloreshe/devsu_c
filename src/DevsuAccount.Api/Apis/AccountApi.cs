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
    }
    
    public static void MapAccountTransactionApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup(Routes.AccountTransactionGroupName);
    }
}