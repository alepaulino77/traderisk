using Microsoft.AspNetCore.Mvc;
using TradeRiskApi.Application.DTOs;
using TradeRiskApi.Application.Interfaces;

namespace TradeRiskApi.Web.Controllers;

[ApiController]
[Route("api/trades/risk")]
public sealed class TradeRiskController : ControllerBase
{
    private readonly ITradeRiskService _tradeRiskService;
    private readonly ILogger<TradeRiskController> _logger;

    public TradeRiskController(ITradeRiskService tradeRiskService, ILogger<TradeRiskController> logger)
    {
        _tradeRiskService = tradeRiskService;
        _logger = logger;
    }

    [HttpPost("classify")]
    [ProducesResponseType(typeof(ClassifyTradesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Classify(
        [FromBody] List<TradeRequest> trades,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Classifying {Count} trades.", trades.Count);

        var response = await _tradeRiskService.ClassifyAsync(trades, cancellationToken);
        return Ok(response);
    }

    [HttpPost("classify-with-summary")]
    [ProducesResponseType(typeof(ClassifyTradesWithSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClassifyWithSummary(
        [FromBody] List<TradeRequest> trades,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Classifying {Count} trades with summary.", trades.Count);

        var response = await _tradeRiskService.ClassifyWithSummaryAsync(trades, cancellationToken);
        return Ok(response);
    }
}
