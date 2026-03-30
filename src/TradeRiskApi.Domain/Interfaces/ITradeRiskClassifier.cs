using TradeRiskApi.Domain.Entities;

namespace TradeRiskApi.Domain.Interfaces;

public interface ITradeRiskClassifier
{
    string Classify(Trade trade);
}
