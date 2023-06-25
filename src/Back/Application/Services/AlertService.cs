using Application.Abstractions;
using Domain.Abstractions;
using Domain.DTOs;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertSender _alertSender;
        private readonly ILogger<AlertService> _logger;
        public AlertService(IAlertSender alertSender, ILogger<AlertService> logger)
        {
            _alertSender = alertSender;
            _logger = logger;
        }

        async Task IAlertService.Handle(string contactName, GrafanaAlert grafanaAlert, CancellationToken cancellationToken)
        {
            foreach (var alert in grafanaAlert?.alerts)
            {
                await _alertSender.Send(contactName, alert, cancellationToken);
            }
        }
    }
}