using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account.AccountTransaction;

public static class GetAccountTransactionErrors
{
    public const string Title = "Buscar movimiento";
    
    public static readonly Error AccountTransactionNotFound = new(
        Title,
        "No se encontró el movimiento");
    
}
public class GetAccountTransactionResult
{
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; }
    public string NumeroCuenta { get; set; }
    public string Tipo { get; set; }
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public decimal Movimiento { get; set; }
    public decimal SaldoDisponible { get; set; }
}
public class GetAccountTransactionRequest : IRequest<Result<GetAccountTransactionResult>>
{
    public Guid AccountTransactionId { get; set; }
}

public class GetAccountTransactionRequestHandler : IRequestHandler<GetAccountTransactionRequest ,Result<GetAccountTransactionResult>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;

    public GetAccountTransactionRequestHandler(IAccountRepository accountRepository, ICustomerRepository customerRepository)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Result<GetAccountTransactionResult>> Handle(GetAccountTransactionRequest transactionRequest, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccount(transactionRequest.AccountTransactionId, cancellationToken);
        
        if (account is null)
        {
            return Result<GetAccountTransactionResult>.Failure(GetAccountTransactionErrors.AccountTransactionNotFound);
        }

        var customerAccount = await _customerRepository.FindCustomer(account.CustomerId, cancellationToken);
        var accountTransaction = account.Transactions.First();
        
        return Result<GetAccountTransactionResult>.Success(new GetAccountTransactionResult
        {
            Fecha = accountTransaction.DateCreation,
            Cliente = customerAccount?.Name ?? string.Empty,
            NumeroCuenta = account.AccountNumber,
            Tipo = account.AccountType.Value,
            SaldoInicial = account.OpeningBalance,
            Estado = account.State,
            Movimiento = accountTransaction.TransactionValue,
            SaldoDisponible = accountTransaction.Balance
        });
    }
}