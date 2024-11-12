using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features;

public static class CreateAccountErrors
{
    public const string Title = "Crear cuenta";
    
    public static Error ExistingAccount(string accountNumber) => new(
        Title,
        $"Ya existe una cuenta con numero {accountNumber}");

}
public class CreateAccountResult
{
    public string NumeroCuenta { get; set; }
}
public class CreateAccountRequest : IRequest<Result<CreateAccountResult>>
{
    public string NumeroCuenta { get; set; }
    public string Tipo { get; set; }
    public decimal SaldoInicial { get; set; }
    public bool Estado { get; set; }
    public Guid ClienteId { get; set; }
}

public class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest ,Result<CreateAccountResult>>
{
    private readonly IAccountRepository _accountRepository;

    public CreateAccountRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<CreateAccountResult>> Handle(CreateAccountRequest request, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.FindAccountAsync(request.NumeroCuenta, cancellationToken);
        
        if (account is not null)
        {
            return Result<CreateAccountResult>.Failure(CreateAccountErrors.ExistingAccount(account.AccountNumber));
        }
        
        var newAccount = new Account(
            accountNumber: request.NumeroCuenta,
            type: request.Tipo,
            openingBalance: request.SaldoInicial,
            state: request.Estado,
            customerId: request.ClienteId
        );
        
        _accountRepository.AddAccount(newAccount);
        await _accountRepository.SaveEntities(cancellationToken);
        
        var result = new CreateAccountResult
        {
            NumeroCuenta = newAccount.AccountNumber,
        };

        return Result<CreateAccountResult>.Success(result);
    }
}