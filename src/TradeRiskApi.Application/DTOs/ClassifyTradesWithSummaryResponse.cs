namespace TradeRiskApi.Application.DTOs;

public sealed record ClassifyTradesWithSummaryResponse(
    IReadOnlyList<string> Categories,
    Dictionary<string, RiskCategorySummaryResponse> Summary,
    long ProcessingTimeMs);
