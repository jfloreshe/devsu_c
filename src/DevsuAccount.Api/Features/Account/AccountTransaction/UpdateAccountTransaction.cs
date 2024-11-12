using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account.AccountTransaction;

public static class UpdateAccountTransactionErrors
{
    public const string Title = "Actualizar movimiento";
    
    public static Error AccountTransactionNotFound => new(
        Title,
        "No se encontro el movimiento");
}
public class UpdateAccountTransactionResult
{
    public Guid MovimientoId { get; set; }
}
public class UpdateAccountTransactionRequest : IRequest<Result<UpdateAccountTransactionResult>>
{
    public required Guid MovimientoId { get; set; }
    public required string Tipo { get; set; }
    public required decimal Movimiento { get; set; }
}

public class UpdateAccountTransactionRequestHandler : IRequestHandler<UpdateAccountTransactionRequest ,Result<UpdateAccountTransactionResult>>
{
    private readonly IAccountRepository _accountRepository;

    public UpdateAccountTransactionRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<UpdateAccountTransactionResult>> Handle(UpdateAccountTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccount(request.MovimientoId, cancellationToken);
        if(account is null)
        {
            return Result<UpdateAccountTransactionResult>.Failure(UpdateAccountTransactionErrors.AccountTransactionNotFound);
        }
        
        var transaction = account.Transactions
            .First(t => t.TransactionId == request.MovimientoId);
        
        var accTransactionTypeResult = AccountTransactionTypeFactory.Create(request.Tipo);
        if (accTransactionTypeResult.IsFailure)
        {
            return Result<UpdateAccountTransactionResult>.Failure(accTransactionTypeResult.Error);
        }

        var endRequest = transaction.Type == accTransactionTypeResult.Value;
        

        if (transaction.TransactionValue == request.Movimiento && endRequest)
        {
            return Result<UpdateAccountTransactionResult>.Success(new UpdateAccountTransactionResult
            {
                MovimientoId = transaction.TransactionId
            });
        } 
        
        var transactionUpdatedResult = account.UpdateTransaction(request.MovimientoId, request.Tipo, request.Movimiento);
        if (transactionUpdatedResult.IsFailure)
        {
            return Result<UpdateAccountTransactionResult>.Failure(transactionUpdatedResult.Error);
        }
        
        await _accountRepository.SaveEntities(cancellationToken);

        return Result<UpdateAccountTransactionResult>.Success(new UpdateAccountTransactionResult
        {
            MovimientoId = transactionUpdatedResult.Value.TransactionId,
        });
    }
}