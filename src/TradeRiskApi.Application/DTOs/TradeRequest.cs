namespace TradeRiskApi.Application.DTOs;

public sealed record TradeRequest(decimal Value, string ClientSector, string? ClientId);
