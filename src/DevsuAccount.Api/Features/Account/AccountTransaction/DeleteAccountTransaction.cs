using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account.AccountTransaction;

public static class DeleteAccountTransactionErrors
{
    public const string Title = "Eliminar movimiento";
    
    public static Error AccountTransactionNotFound => new(
        Title,
        "No se encontro el movimiento");
}
public class DeleteAccountTransactionResult
{
    public Guid MovimientoId { get; set; }
}
public class DeleteAccountTransactionRequest : IRequest<Result<DeleteAccountTransactionResult>>
{
    public Guid AccountTransactionId { get; set; }
}

public class DeleteAccountTransactionRequestHandler : IRequestHandler<DeleteAccountTransactionRequest ,Result<DeleteAccountTransactionResult>>
{
    private readonly IAccountRepository _accountRepository;

    public DeleteAccountTransactionRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<DeleteAccountTransactionResult>> Handle(DeleteAccountTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccount(request.AccountTransactionId, cancellationToken);
        if(account is null)
        {
            return Result<DeleteAccountTransactionResult>.Failure(DeleteAccountErrors.AccountNotFound);
        }
        
        var deleteAccountTransactionResult = account.DeleteTransaction(request.AccountTransactionId);
        if (deleteAccountTransactionResult.IsFailure)
        {
            return Result<DeleteAccountTransactionResult>.Failure(deleteAccountTransactionResult.Error);
        }
        
        _accountRepository.DeleteAccountTransaction(deleteAccountTransactionResult.Value);
        await _accountRepository.SaveEntities(cancellationToken);

        return Result<DeleteAccountTransactionResult>.Success(new DeleteAccountTransactionResult
        {
            MovimientoId = request.AccountTransactionId
        });
    }
}