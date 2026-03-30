using TradeRiskApi.Domain.Entities;
using TradeRiskApi.Domain.Enums;
using TradeRiskApi.Domain.Interfaces;

namespace TradeRiskApi.Domain.Rules;

public sealed class HighRiskRule : ITradeClassificationRule
{
    public int Order => 3;
    public string Category => RiskCategory.HighRisk;

    public bool IsMatch(Trade trade) =>
        trade.Value >= 1_000_000m &&
        string.Equals(trade.ClientSector, "Private", StringComparison.OrdinalIgnoreCase);
}
