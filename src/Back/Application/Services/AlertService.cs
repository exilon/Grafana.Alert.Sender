using Application.Abstractions;
using Application.Options;
using Domain.Abstractions;
using Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertSender _alertSender;
        private readonly GrafanaOptions _options;
        private readonly ILogger<AlertService> _logger;
        public AlertService(IAlertSender alertSender, IOptions<GrafanaOptions> options, ILogger<AlertService> logger)
        {
            _alertSender = alertSender;
            _options = options.Value;
            _logger = logger;
        }

        async Task IAlertService.Handle(string contactName, GrafanaAlert grafanaAlert, CancellationToken cancellationToken)
        {
            foreach (var alert in grafanaAlert?.alerts)
            {
                var contact = contactName;
                if (alert.labels?.GetValueOrDefault("alertname") == "DataSourceError" ||
                    alert.labels?.GetValueOrDefault("alertname") == "DatasourceNoData")
                {
                    if (_options.RejectNoData)
                        continue;

                    if (!string.IsNullOrEmpty(_options.RedirectNoDataToContact))
                        contact = _options.RedirectNoDataToContact;
                }

                await _alertSender.Send(contact, alert, cancellationToken);
            }
        }
    }
}