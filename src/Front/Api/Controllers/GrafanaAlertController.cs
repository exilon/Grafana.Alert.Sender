using Domain.Abstractions;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Polling;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GrafanaAlertController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<GrafanaAlertController> _logger;

        public GrafanaAlertController(IAlertService alertService, ILogger<GrafanaAlertController> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        [HttpPost("Send/{contactName}")]
        //[Route("Send")]
        public async Task<IActionResult> Send(string contactName, [FromBody] GrafanaAlert grafanaAlert)
        {
            var result = _alertService.Handle(contactName, grafanaAlert);
            return Ok();
        }
    }
}