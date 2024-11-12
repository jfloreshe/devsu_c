using Azure.Core;
using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account.AccountTransaction;

public static class PatchAccountTransactionErrors
{
    public const string Title = "Actualizar movimiento";
    
    public static Error AccountTransactionNotFound => new(
        Title,
        "No se encontro el movimiento");
}
public class PatchAccountTransactionResult
{
    public Guid MovimientoId { get; set; }
}
public class PatchAccountTransactionRequest : IRequest<Result<PatchAccountTransactionResult>>
{
    public required Guid MovimientoId { get; set; }
    public string? Tipo { get; set; }
    public decimal? Movimiento { get; set; }
}

public class PatchAccountTransactionRequestHandler : IRequestHandler<PatchAccountTransactionRequest ,Result<PatchAccountTransactionResult>>
{
    private readonly IAccountRepository _accountRepository;

    public PatchAccountTransactionRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<PatchAccountTransactionResult>> Handle(PatchAccountTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccount(request.MovimientoId, cancellationToken);
        if(account is null)
        {
            return Result<PatchAccountTransactionResult>.Failure(UpdateAccountTransactionErrors.AccountTransactionNotFound);
        }

        bool endRequest;
        var transaction = account.Transactions
            .First(t => t.TransactionId == request.MovimientoId);
        
        if (request.Tipo is not null)
        {
            var accTransactionTypeResult = AccountTransactionTypeFactory.Create(request.Tipo);
            if (accTransactionTypeResult.IsFailure)
            {
                return Result<PatchAccountTransactionResult>.Failure(accTransactionTypeResult.Error);
            }

            endRequest = transaction.Type == accTransactionTypeResult.Value;
        }
        else
        {
            request.Tipo = transaction.Type.Value;
            endRequest = true;
        }

        if (request.Movimiento is not null && transaction.TransactionValue == request.Movimiento && endRequest)
        {
            return Result<PatchAccountTransactionResult>.Success(new PatchAccountTransactionResult
            {
                MovimientoId = transaction.TransactionId
            });
        }

        if (request.Movimiento is null && endRequest)
        {
            return Result<PatchAccountTransactionResult>.Success(new PatchAccountTransactionResult
            {
                MovimientoId = transaction.TransactionId
            });
        }

        request.Movimiento ??= transaction.TransactionValue;

        var transactionUpdatedResult = account.UpdateTransaction(request.MovimientoId, request.Tipo!, request.Movimiento!.Value);
        if (transactionUpdatedResult.IsFailure)
        {
            return Result<PatchAccountTransactionResult>.Failure(transactionUpdatedResult.Error);
        }
        
        await _accountRepository.SaveEntities(cancellationToken);

        return Result<PatchAccountTransactionResult>.Success(new PatchAccountTransactionResult
        {
            MovimientoId = transactionUpdatedResult.Value.TransactionId
        });
    }
}