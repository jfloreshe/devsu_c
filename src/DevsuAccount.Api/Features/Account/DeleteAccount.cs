using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account;

public static class DeleteAccountErrors
{
    public const string Title = "Eliminar cuenta";
    
    public static readonly Error AccountNotFound = new(
        Title,
        "No se encontró la cuenta");
    
}
public class DeleteAccountResult
{
    public string NumeroCuenta { get; set; }
}
public class DeleteAccountRequest : IRequest<Result<DeleteAccountResult>>
{
    public string AccountNumber { get; set; }
}

public class DeleteAccountRequestHandler : IRequestHandler<DeleteAccountRequest ,Result<DeleteAccountResult>>
{
    private readonly IAccountRepository _accountRepository;

    public DeleteAccountRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<DeleteAccountResult>> Handle(DeleteAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccountAsync(request.AccountNumber, cancellationToken);
        if (account is null)
        {
            return Result<DeleteAccountResult>.Failure(DeleteAccountErrors.AccountNotFound);
        }

        _accountRepository.DeleteCustomer(account);
        await _accountRepository.SaveEntities(cancellationToken);
        
        return Result<DeleteAccountResult>.Success(new DeleteAccountResult
        {
            NumeroCuenta = account.AccountNumber
        });
    }
}