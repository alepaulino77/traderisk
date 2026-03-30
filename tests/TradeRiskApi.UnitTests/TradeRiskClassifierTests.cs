using FluentAssertions;
using TradeRiskApi.Domain.Entities;
using TradeRiskApi.Domain.Enums;
using TradeRiskApi.Domain.Interfaces;
using TradeRiskApi.Domain.Rules;
using TradeRiskApi.Domain.Services;
using Xunit;

namespace TradeRiskApi.UnitTests;

public sealed class TradeRiskClassifierTests
{
    private static ITradeRiskClassifier CreateClassifier() =>
        new TradeRiskClassifier(new ITradeClassificationRule[]
        {
            new LowRiskRule(),
            new MediumRiskRule(),
            new HighRiskRule()
        });

    [Fact]
    public void Should_Classify_As_LowRisk_When_Value_Is_Below_One_Million()
    {
        var classifier = CreateClassifier();
        var trade = new Trade(999_999.99m, "Private", "CLI001");

        var result = classifier.Classify(trade);

        result.Should().Be(RiskCategory.LowRisk);
    }

    [Fact]
    public void Should_Classify_As_MediumRisk_When_Value_Is_At_Least_One_Million_And_Sector_Is_Public()
    {
        var classifier = CreateClassifier();
        var trade = new Trade(1_000_000m, "Public", "CLI002");

        var result = classifier.Classify(trade);

        result.Should().Be(RiskCategory.MediumRisk);
    }

    [Fact]
    public void Should_Classify_As_HighRisk_When_Value_Is_At_Least_One_Million_And_Sector_Is_Private()
    {
        var classifier = CreateClassifier();
        var trade = new Trade(2_000_000m, "Private", "CLI003");

        var result = classifier.Classify(trade);

        result.Should().Be(RiskCategory.HighRisk);
    }
}
