using FluentValidation;
using FluentValidation.AspNetCore;
using TradeRiskApi.Application.DTOs;
using TradeRiskApi.Application.Interfaces;
using TradeRiskApi.Application.Services;
using TradeRiskApi.Application.Validation;
using TradeRiskApi.Domain.Interfaces;
using TradeRiskApi.Domain.Rules;
using TradeRiskApi.Domain.Services;
using TradeRiskApi.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IValidator<TradeRequest>, TradeRequestValidator>();
builder.Services.AddScoped<IValidator<IReadOnlyCollection<TradeRequest>>, TradeRequestListValidator>();

builder.Services.AddScoped<ITradeClassificationRule, LowRiskRule>();
builder.Services.AddScoped<ITradeClassificationRule, MediumRiskRule>();
builder.Services.AddScoped<ITradeClassificationRule, HighRiskRule>();
builder.Services.AddScoped<ITradeRiskClassifier, TradeRiskClassifier>();
builder.Services.AddScoped<ITradeRiskService, TradeRiskService>();

builder.Services.AddLogging();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program;
