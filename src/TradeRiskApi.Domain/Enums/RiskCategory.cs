namespace TradeRiskApi.Domain.Enums;

public static class RiskCategory
{
    public const string LowRisk = "LOWRISK";
    public const string MediumRisk = "MEDIUMRISK";
    public const string HighRisk = "HIGHRISK";

    public static readonly IReadOnlyCollection<string> All =
    [
        LowRisk,
        MediumRisk,
        HighRisk
    ];
}
