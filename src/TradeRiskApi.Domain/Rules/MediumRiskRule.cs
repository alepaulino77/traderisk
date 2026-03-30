using TradeRiskApi.Domain.Entities;
using TradeRiskApi.Domain.Enums;
using TradeRiskApi.Domain.Interfaces;

namespace TradeRiskApi.Domain.Rules;

public sealed class MediumRiskRule : ITradeClassificationRule
{
    public int Order => 2;
    public string Category => RiskCategory.MediumRisk;

    public bool IsMatch(Trade trade) =>
        trade.Value >= 1_000_000m &&
        string.Equals(trade.ClientSector, "Public", StringComparison.OrdinalIgnoreCase);
}
