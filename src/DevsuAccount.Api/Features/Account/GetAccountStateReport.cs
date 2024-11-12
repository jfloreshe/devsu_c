using Devsu.Shared.Primitives;
using DevsuAccount.Api.Features.Account;
using DevsuAccount.Api.Models;
using MediatR;

namespace DevsuAccount.Api.Features.Account;

public class GetAccountStateReportDetailsResult
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
public class GetAccountStateReportResult
{
    public ICollection<GetAccountStateReportDetailsResult> Data { get; set; } =
        new List<GetAccountStateReportDetailsResult>();
    public int Pagina { get; set; }
    public int TamanoBatch { get; set; }
    public int NumeroTotalRegistros { get; set; }
}

public class GetAccountStateReportRequest : IRequest<GetAccountStateReportResult>
{
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public required Guid CustomerId { get; set; }
    public int? Page { get; set; }
    public int? Size { get; set; }
}

public class GetAccountStateReportRequestHandler : IRequestHandler<GetAccountStateReportRequest ,GetAccountStateReportResult>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountStateReportRequestHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountStateReportResult> Handle(GetAccountStateReportRequest request, 
        CancellationToken cancellationToken = default)
    {
        var initialDate = request.FechaInicio ?? DateTime.Now.AddDays(-30);
        var finalDate = request.FechaFin?.AddDays(1).AddSeconds(-1) ?? DateTime.Now.AddDays(1).AddSeconds(-1);
        var page = request.Page ?? 0;
        var size = request.Size ?? 10;
     
        var report = await _accountRepository.GetAccountState(request.CustomerId, initialDate, finalDate, page, size, cancellationToken);
        
        return report;
    }
}