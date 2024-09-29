using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using System.Text.RegularExpressions;

namespace TemplateBot;

public class TelegramController
{
   private readonly ILogger _logger;
   private readonly TelegramBotClient _client;

   private bool _started;

   public TelegramController(ILogger logger, string token)
   {
      _logger = logger;
      _client = new TelegramBotClient(token);
   }

   public void StartReceiving()
   {
      if (_started)
      {
         _logger.Error("Tried to start {className} twice", nameof(TelegramController));
         return;
      }

      _started = true;

      var receiverOptions = new ReceiverOptions()
      {
         AllowedUpdates = new[] {UpdateType.Message, UpdateType.ChannelPost}
      };

      _client.StartReceiving(OnUpdate, OnError, receiverOptions);
      _logger.Information("Started listening");
   }

   private async Task OnUpdate(ITelegramBotClient client, Update update, CancellationToken ct)
   {
      _logger.Information("Received update: {updateType}", update.Type);

      if (update.Message is Message message)
         await HandleMessageAsync(client, message);
   }

   private async Task HandleMessageAsync(ITelegramBotClient client, Message message)
   {
      User sender = message.From;
      if (message.Text is not null)
      {
         _logger.Information("Received '{text}' ([{messageId}]) from '{first} {last}' (@{tag} [{id}])", message.Text, message.MessageId, sender.FirstName, sender.LastName, sender.Username, sender.Id);
      }

      const string response = "Test";

      var replyParameters = new ReplyParameters
      {
         MessageId = message.MessageId
      };

      await client.SendTextMessageAsync(sender.Id, response, null, ParseMode.None, null, null, false, false, null, replyParameters);

      _logger.Information("Responded to [{messageId}] with '{text}'", message.MessageId, response);
   }

   private Task OnError(ITelegramBotClient client, Exception exception, CancellationToken cts)
   {
      return Task.CompletedTask;
   }
}
