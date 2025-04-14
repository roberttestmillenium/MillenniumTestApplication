using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MillenniumTestApplication.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CardService>();
builder.Services.AddSingleton<ActionResolver>(provider => new ActionResolver("Config/card_rules1.json"));

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

app.Run();
