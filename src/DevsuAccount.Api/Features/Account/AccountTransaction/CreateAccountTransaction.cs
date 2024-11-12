using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account.AccountTransaction;

public static class CreateAccountTransactionErrors
{
    public const string Title = "Realizar movimiento";
    
    public static Error NotEnoughBalance(string TransactionNumber) => new(
        Title,
        "Saldo no disponible");
    
    public static Error InactiveAccount => new(
        Title,
        "La cuenta esta inactiva");
    public static Error AccountNotFound => new(
        Title,
        "No existe la cuenta donde desea realizar el movimiento");
}
public class CreateAccountTransactionResult
{
    public Guid MovimientoId { get; set; }
}
public class CreateAccountTransactionRequest : IRequest<Result<CreateAccountTransactionResult>>
{
    public string NumeroCuenta { get; set; }
    public string Tipo { get; set; }
    public decimal Movimiento { get; set; }
}

public class CreateAccountTransactionRequestHandler : IRequestHandler<CreateAccountTransactionRequest ,Result<CreateAccountTransactionResult>>
{
    private readonly IAccountRepository _accountRepository;

    public CreateAccountTransactionRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<CreateAccountTransactionResult>> Handle(CreateAccountTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccountAsync(request.NumeroCuenta, cancellationToken);
        switch (account)
        {
            case null:
                return Result<CreateAccountTransactionResult>.Failure(CreateAccountTransactionErrors.AccountNotFound);
            case { State: false }:
                return Result<CreateAccountTransactionResult>.Failure(CreateAccountTransactionErrors.InactiveAccount);
        }
        
        var newTransaction = account.AddNewTransaction(request.Tipo, request.Movimiento);
        if (newTransaction.IsFailure)
        {
            return Result<CreateAccountTransactionResult>.Failure(newTransaction.Error);
        }

        _accountRepository.AddTransaction(newTransaction.Value);
        await _accountRepository.SaveEntities(cancellationToken);
        
        var result = new CreateAccountTransactionResult
        {
            MovimientoId = newTransaction.Value.TransactionId,
        };

        return Result<CreateAccountTransactionResult>.Success(result);
    }
}