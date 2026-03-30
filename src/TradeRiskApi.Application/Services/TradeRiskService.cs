using System.Diagnostics;
using FluentValidation;
using TradeRiskApi.Application.DTOs;
using TradeRiskApi.Application.Interfaces;
using TradeRiskApi.Domain.Entities;
using TradeRiskApi.Domain.Enums;
using TradeRiskApi.Domain.Interfaces;

namespace TradeRiskApi.Application.Services;

public sealed class TradeRiskService : ITradeRiskService
{
    private readonly ITradeRiskClassifier _classifier;
    private readonly IValidator<IReadOnlyCollection<TradeRequest>> _validator;

    public TradeRiskService(
        ITradeRiskClassifier classifier,
        IValidator<IReadOnlyCollection<TradeRequest>> validator)
    {
        _classifier = classifier;
        _validator = validator;
    }

    public async Task<ClassifyTradesResponse> ClassifyAsync(
        IReadOnlyCollection<TradeRequest> trades,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(trades, cancellationToken);

        var categories = new List<string>(trades.Count);

        foreach (var trade in trades)
        {
            var category = _classifier.Classify(ToEntity(trade));
            categories.Add(category);
        }

        return new ClassifyTradesResponse(categories);
    }

    public async Task<ClassifyTradesWithSummaryResponse> ClassifyWithSummaryAsync(
        IReadOnlyCollection<TradeRequest> trades,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(trades, cancellationToken);

        var stopwatch = Stopwatch.StartNew();

        var categories = new List<string>(trades.Count);
        var summaryAccumulators = InitializeSummaryAccumulators();

        foreach (var trade in trades)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entity = ToEntity(trade);
            var category = _classifier.Classify(entity);
            categories.Add(category);

            var accumulator = summaryAccumulators[category];
            accumulator.Count++;
            accumulator.TotalValue += entity.Value;

            if (!string.IsNullOrWhiteSpace(entity.ClientId))
            {
                if (!accumulator.ClientExposure.TryAdd(entity.ClientId!, entity.Value))
                {
                    accumulator.ClientExposure[entity.ClientId!] += entity.Value;
                }
            }
        }

        var summary = summaryAccumulators.ToDictionary(
            kvp => kvp.Key,
            kvp => new RiskCategorySummaryResponse(
                kvp.Value.Count,
                kvp.Value.TotalValue,
                kvp.Value.ClientExposure.Count == 0
                    ? null
                    : kvp.Value.ClientExposure.MaxBy(c => c.Value).Key));

        stopwatch.Stop();

        return new ClassifyTradesWithSummaryResponse(categories, summary, stopwatch.ElapsedMilliseconds);
    }

    private async Task ValidateAsync(IReadOnlyCollection<TradeRequest> trades, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(trades, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }

    private static Trade ToEntity(TradeRequest request) =>
        new(request.Value, request.ClientSector, request.ClientId);

    private static Dictionary<string, SummaryAccumulator> InitializeSummaryAccumulators() =>
        RiskCategory.All.ToDictionary(c => c, _ => new SummaryAccumulator());

    private sealed class SummaryAccumulator
    {
        public int Count { get; set; }
        public decimal TotalValue { get; set; }
        public Dictionary<string, decimal> ClientExposure { get; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
