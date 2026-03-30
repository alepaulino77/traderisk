using TradeRiskApi.Domain.Entities;
using TradeRiskApi.Domain.Enums;
using TradeRiskApi.Domain.Interfaces;

namespace TradeRiskApi.Domain.Rules;

public sealed class LowRiskRule : ITradeClassificationRule
{
    public int Order => 1;
    public string Category => RiskCategory.LowRisk;

    public bool IsMatch(Trade trade) => trade.Value < 1_000_000m;
}
