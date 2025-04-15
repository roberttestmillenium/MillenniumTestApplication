using MillenniumTestApplication.Application.Queries.GetAllowedActions;
using MillenniumTestApplication.Domain.Interfaces;
using MillenniumTestApplication.Domain.Services;
using MillenniumTestApplication.Infrastructure.Mock;
using MediatR;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetAllowedActionsQuery).Assembly);
});

builder.Services.AddSingleton<ICardReader, CardService>();

var rulesPath = Path.Combine(builder.Environment.ContentRootPath, "Config", "card_rules.json");
var actionResolver = new ActionResolver(rulesPath);
builder.Services.AddSingleton<IActionResolver>(actionResolver);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
    app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();