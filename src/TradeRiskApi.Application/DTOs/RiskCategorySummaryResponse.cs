namespace TradeRiskApi.Application.DTOs;

public sealed record RiskCategorySummaryResponse(int Count, decimal TotalValue, string? TopClient);
