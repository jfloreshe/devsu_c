using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features;

public static class UpdateAccountErrors
{
    public const string Title = "Actualizar cuenta";
    
    public static readonly Error AccountNotFound = new(
        Title,
        "No se encontró la cuenta");
    
}
public class UpdateAccountResult
{
    public string numeroCuenta { get; set; }
}
public class UpdateAccountRequest : IRequest<Result<UpdateAccountResult>>
{
    public string NumeroCuenta { get; set; }
    public string Tipo { get; set; }
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public Guid ClienteId { get; set; }
}

public class UpdateAccountRequestHandler : IRequestHandler<UpdateAccountRequest ,Result<UpdateAccountResult>>
{
    private readonly IAccountRepository _accountRepository;

    public UpdateAccountRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<UpdateAccountResult>> Handle(UpdateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccountAsync(request.NumeroCuenta, cancellationToken);
        if (account is null)
        {
            return Result<UpdateAccountResult>.Failure(UpdateAccountErrors.AccountNotFound);
        }

        account.Type = request.Tipo;
        account.OpeningBalance = request.SaldoInicial;
        account.CustomerId = request.ClienteId;
        account.State = request.Estado;
        
        _accountRepository.UpdateAccount(account);
        await _accountRepository.SaveEntities(cancellationToken);
        
        return Result<UpdateAccountResult>.Success(new UpdateAccountResult
        {
            numeroCuenta = account.AccountNumber
        });
    }
}