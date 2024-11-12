using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account;

public static class GetAccountErrors
{
    public const string Title = "Buscar cuenta";
    
    public static readonly Error AccountNotFound = new(
        Title,
        "No se encontró la cuenta");
    
}
public class GetAccountResult
{
    public string NumeroCuenta { get; set; }
    public string Tipo { get; set; }
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public string Cliente { get; set; }
}
public class GetAccountRequest : IRequest<Result<GetAccountResult>>
{
    public string AccountNumber { get; set; }
}

public class GetAccountRequestHandler : IRequestHandler<GetAccountRequest ,Result<GetAccountResult>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<GetAccountResult>> Handle(GetAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccountAsync(request.AccountNumber, cancellationToken);
        if (account is null)
        {
            return Result<GetAccountResult>.Failure(GetAccountErrors.AccountNotFound);
        }

        var customer = await _accountRepository.FindCustomerAsync(account.CustomerId, cancellationToken);
        
        
        return Result<GetAccountResult>.Success(new GetAccountResult
        {
            NumeroCuenta = account.AccountNumber,
            Tipo = account.AccountType.Value,
            SaldoInicial = account.OpeningBalance,
            Estado = account.State,
            Cliente = customer?.Name ?? string.Empty
        });
    }
}