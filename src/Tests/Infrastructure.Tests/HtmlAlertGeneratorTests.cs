using Application.Options;
using Domain.DTOs;
using Infrastructure.Services.AlertGenerators;
using Microsoft.Extensions.Options;
using Xunit;

namespace Infrastructure.Tests;

public class HtmlAlertGeneratorTests
{
    [Fact]
    public void GenerateAlert_ShouldIncludeThemeInGrafanaLinks()
    {
        var generator = new HtmlAlertGenerator(Options.Create(new GrafanaOptions
        {
            Url = "http://grafana:3000",
            Theme = "dark"
        }));

        var alert = new Alert
        {
            status = "firing",
            labels = new Dictionary<string, string>
            {
                ["alertname"] = "High CPU",
                ["severity"] = "critical",
                ["host"] = "server-1",
                ["location"] = "eu-west-1"
            },
            annotations = new Dictionary<string, string>
            {
                ["message"] = "CPU usage is high",
                ["summary"] = "Check the node"
            },
            startsAt = "2024-01-01T00:00:00Z",
            dashboardURL = "http://grafana:3000/d/test-dashboard",
            panelURL = "http://grafana:3000/d/test-dashboard?orgId=1&viewPanel=2",
            silenceURL = "http://grafana:3000/alerting/silence/new"
        };

        var html = generator.GenerateAlert(alert);

        Assert.Contains("theme=dark", html);
    }
}
