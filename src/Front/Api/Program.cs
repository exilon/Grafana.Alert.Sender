using Application.Abstractions;
using Application.Services;
using Domain.Abstractions;
using Infrastructure.Services.Senders;
using System.Reflection;
using Application.Options;
using Infrastructure.Services.AlertGenerators;

string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
builder.Configuration.AddYamlFile("appsettings.yml", optional: false)
    .AddYamlFile($"appsettings.{env.EnvironmentName}.yml", optional: true);

builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection("Telegram"));

builder.Services.Configure<GrafanaOptions>(builder.Configuration.GetSection("Grafana"));

builder.Services.AddTransient<IAlertSender, TelegramAlertSender>();

builder.Services.AddTransient<IAlertGenerator, HtmlAlertGenerator>();

builder.Services.AddTransient<IAlertService, AlertService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
