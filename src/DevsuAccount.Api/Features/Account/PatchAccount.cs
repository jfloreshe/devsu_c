using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account;

public static class PatchAccountErrors
{
    public const string Title = "Actualizar cuenta";
    
    public static readonly Error AccountNotFound = new(
        Title,
        "No se encontró la cuenta");
    
}
public class PAtchAccountResult
{
    public string numeroCuenta { get; set; }
}
public class PatchAccountRequest : IRequest<Result<PAtchAccountResult>>
{
    public required string NumeroCuenta { get; set; }
    public string? Tipo { get; set; }
    public decimal? SaldoInicial { get; set; }
    public bool? Estado { get; set; }
    public Guid? ClienteId { get; set; }
}

public class PatchAccountRequestHandler : IRequestHandler<PatchAccountRequest ,Result<PAtchAccountResult>>
{
    private readonly IAccountRepository _accountRepository;

    public PatchAccountRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<PAtchAccountResult>> Handle(PatchAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccountAsync(request.NumeroCuenta, cancellationToken);
        if (account is null)
        {
            return Result<PAtchAccountResult>.Failure(PatchAccountErrors.AccountNotFound);
        }

        if (request.Tipo != null)
        {
            account.AccountType = request.Tipo;
        }
        
        if (request.SaldoInicial != null)
        {
            account.OpeningBalance = request.SaldoInicial.Value;
        }
        
        if (request.Estado != null)
        {
            account.State = request.Estado.Value;
        }
        
        if (request.ClienteId != null)
        {
            account.CustomerId = request.ClienteId.Value;
        }
        
        _accountRepository.UpdateAccount(account);
        await _accountRepository.SaveEntities(cancellationToken);
        
        return Result<PAtchAccountResult>.Success(new PAtchAccountResult
        {
            numeroCuenta = account.AccountNumber
        });
    }
}