using TradeRiskApi.Application.DTOs;

namespace TradeRiskApi.Application.Interfaces;

public interface ITradeRiskService
{
    Task<ClassifyTradesResponse> ClassifyAsync(IReadOnlyCollection<TradeRequest> trades, CancellationToken cancellationToken = default);
    Task<ClassifyTradesWithSummaryResponse> ClassifyWithSummaryAsync(IReadOnlyCollection<TradeRequest> trades, CancellationToken cancellationToken = default);
}
