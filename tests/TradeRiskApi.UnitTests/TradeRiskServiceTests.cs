using FluentAssertions;
using FluentValidation;
using TradeRiskApi.Application.DTOs;
using TradeRiskApi.Application.Services;
using TradeRiskApi.Application.Validation;
using TradeRiskApi.Domain.Interfaces;
using TradeRiskApi.Domain.Rules;
using TradeRiskApi.Domain.Services;
using Xunit;

namespace TradeRiskApi.UnitTests;

public sealed class TradeRiskServiceTests
{
    private static TradeRiskService CreateService()
    {
        var classifier = new TradeRiskClassifier(new ITradeClassificationRule[]
        {
            new LowRiskRule(),
            new MediumRiskRule(),
            new HighRiskRule()
        });

        IValidator<TradeRequest> tradeValidator = new TradeRequestValidator();
        IValidator<IReadOnlyCollection<TradeRequest>> listValidator = new TradeRequestListValidator(tradeValidator);

        return new TradeRiskService(classifier, listValidator);
    }

    [Fact]
    public async Task Should_Return_Categories_In_Same_Order_As_Request()
    {
        var service = CreateService();
        var trades = new List<TradeRequest>
        {
            new(2_000_000m, "Private", "CLI003"),
            new(400_000m, "Public", "CLI001"),
            new(3_000_000m, "Public", "CLI002")
        };

        var result = await service.ClassifyAsync(trades);

        result.Categories.Should().Equal("HIGHRISK", "LOWRISK", "MEDIUMRISK");
    }

    [Fact]
    public async Task Should_Return_Summary_With_Count_Total_And_TopClient()
    {
        var service = CreateService();
        var trades = new List<TradeRequest>
        {
            new(2_000_000m, "Private", "CLI003"),
            new(500_000m, "Public", "CLI001"),
            new(400_000m, "Private", "CLI001"),
            new(3_000_000m, "Public", "CLI002"),
            new(1_500_000m, "Public", "CLI002")
        };

        var result = await service.ClassifyWithSummaryAsync(trades);

        result.Summary["LOWRISK"].Count.Should().Be(2);
        result.Summary["LOWRISK"].TotalValue.Should().Be(900_000m);
        result.Summary["LOWRISK"].TopClient.Should().Be("CLI001");

        result.Summary["MEDIUMRISK"].Count.Should().Be(2);
        result.Summary["MEDIUMRISK"].TotalValue.Should().Be(4_500_000m);
        result.Summary["MEDIUMRISK"].TopClient.Should().Be("CLI002");

        result.Summary["HIGHRISK"].Count.Should().Be(1);
        result.Summary["HIGHRISK"].TotalValue.Should().Be(2_000_000m);
        result.Summary["HIGHRISK"].TopClient.Should().Be("CLI003");
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Sector_Is_Invalid()
    {
        var service = CreateService();
        var trades = new List<TradeRequest>
        {
            new(100m, "Unknown", "CLI001")
        };

        var act = async () => await service.ClassifyAsync(trades);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
