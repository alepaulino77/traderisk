using FluentValidation;
using TradeRiskApi.Application.DTOs;

namespace TradeRiskApi.Application.Validation;

public sealed class TradeRequestListValidator : AbstractValidator<IReadOnlyCollection<TradeRequest>>
{
    public TradeRequestListValidator(IValidator<TradeRequest> tradeValidator)
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Trade list is required.");

        RuleFor(x => x.Count)
            .GreaterThan(0)
            .WithMessage("At least one trade must be informed.")
            .LessThanOrEqualTo(100_000)
            .WithMessage("The maximum supported batch size is 100,000 trades.");

        RuleForEach(x => x).SetValidator(tradeValidator);
    }
}
