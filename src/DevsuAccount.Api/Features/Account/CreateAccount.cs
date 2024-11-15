﻿using Devsu.Shared.Primitives;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account;

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
    public required string NumeroCuenta { get; set; }
    public required string Tipo { get; set; }
    public required decimal SaldoInicial { get; set; }
    public required bool Estado { get; set; }
    public required Guid ClienteId { get; set; }
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
        var account = await _accountRepository.FindAccount(request.NumeroCuenta, cancellationToken);
        
        if (account is not null)
        {
            return Result<CreateAccountResult>.Failure(CreateAccountErrors.ExistingAccount(account.AccountNumber));
        }
        
        var newAccountResult = Models.Account.Create(
            accountNumber: request.NumeroCuenta,
            accountTypeRaw: request.Tipo,
            openingBalance: request.SaldoInicial,
            state: request.Estado,
            customerId: request.ClienteId
        );

        if (newAccountResult.IsFailure)
        {
            return Result<CreateAccountResult>.Failure(newAccountResult.Error);
        }
        
        _accountRepository.AddAccount(newAccountResult.Value);
        await _accountRepository.SaveEntities(cancellationToken);
        
        var result = new CreateAccountResult
        {
            NumeroCuenta = newAccountResult.Value.AccountNumber,
        };

        return Result<CreateAccountResult>.Success(result);
    }
}