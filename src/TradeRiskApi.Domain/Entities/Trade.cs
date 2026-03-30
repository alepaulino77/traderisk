namespace TradeRiskApi.Domain.Entities;

public sealed class Trade
{
    public Trade(decimal value, string clientSector, string? clientId = null)
    {
        Value = value;
        ClientSector = clientSector;
        ClientId = clientId;
    }

    public decimal Value { get; }
    public string ClientSector { get; }
    public string? ClientId { get; }
}
