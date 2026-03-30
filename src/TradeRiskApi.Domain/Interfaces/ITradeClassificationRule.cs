using TradeRiskApi.Domain.Entities;

namespace TradeRiskApi.Domain.Interfaces;

public interface ITradeClassificationRule
{
    int Order { get; }
    bool IsMatch(Trade trade);
    string Category { get; }
}
