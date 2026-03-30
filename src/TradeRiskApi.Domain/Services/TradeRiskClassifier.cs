using TradeRiskApi.Domain.Entities;
using TradeRiskApi.Domain.Interfaces;

namespace TradeRiskApi.Domain.Services;

public sealed class TradeRiskClassifier : ITradeRiskClassifier
{
    private readonly IReadOnlyList<ITradeClassificationRule> _rules;

    public TradeRiskClassifier(IEnumerable<ITradeClassificationRule> rules)
    {
        _rules = rules.OrderBy(r => r.Order).ToList();
    }

    public string Classify(Trade trade)
    {
        var rule = _rules.FirstOrDefault(r => r.IsMatch(trade));

        if (rule is null)
        {
            throw new InvalidOperationException("No classification rule matched the provided trade.");
        }

        return rule.Category;
    }
}
