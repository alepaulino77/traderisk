using FluentValidation;
using TradeRiskApi.Application.DTOs;

namespace TradeRiskApi.Application.Validation;

public sealed class TradeRequestValidator : AbstractValidator<TradeRequest>
{
    private static readonly string[] ValidSectors = ["Public", "Private"];

    public TradeRequestValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0m)
            .WithMessage("Trade value must be greater than or equal to zero.");

        RuleFor(x => x.ClientSector)
            .NotEmpty()
            .Must(sector => ValidSectors.Contains(sector, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Client sector must be 'Public' or 'Private'.");
    }
}
