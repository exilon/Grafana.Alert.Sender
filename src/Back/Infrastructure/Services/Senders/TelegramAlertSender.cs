using Application.Abstractions;
using Application.Options;
using Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Infrastructure.Services.Senders
{
    public class TelegramAlertSender : IAlertSender
    {
        private readonly IAlertGenerator _alertGenerator;
        private readonly TelegramOptions _telegramOptions;
        private TelegramBotClient _botClient;
        private readonly ILogger<TelegramAlertSender> _logger;
        public TelegramAlertSender(IAlertGenerator alertGenerator, IOptions<TelegramOptions> telegramOptions, ILogger<TelegramAlertSender> logger)
        {
            _alertGenerator = alertGenerator;
            _telegramOptions = telegramOptions.Value;
            _logger = logger;
        }
        public async Task Send(string contactName, Alert alert, CancellationToken cancellationToken)
        {
            try
            {
                if (!_telegramOptions.Contacts.TryGetValue(contactName, out var contact)) throw new Exception($"Contact '{contactName}' not found!");
                _botClient = new TelegramBotClient(contact.BotToken);
                var alertmsg = _alertGenerator.GenerateAlert(alert);
                _logger.LogInformation($"Alert html generated");
                try
                {
                    _logger.LogInformation("Getting panel image...");
                    using var image = await _alertGenerator.GenerateImage(alert);
                    _logger.LogInformation($"Alert image generated");
                    var inputMedia = InputFile.FromStream(image);
                    await _botClient.SendPhotoAsync(chatId: contact.ChatId, photo: inputMedia, caption: alertmsg, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Alert image generation failed! {ex.Message}");
                    await _botClient.SendTextMessageAsync(contact.ChatId, alertmsg, parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                }
                _logger.LogInformation($"Alert sent to {contact.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending alert: {ex.Message}");
                throw new Exception($"Error sending alert!");
            }
            
        }
    }
}