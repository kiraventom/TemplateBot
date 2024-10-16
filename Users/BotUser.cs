using System.Text.Json;
using System.Text.Json.Serialization;

namespace TemplateBot.Users;

public class BotUser
{
   [JsonInclude]
   public long SenderId { get; private set; }

   [JsonInclude]
   public BotState BotState { get; private set; }

   public event Action<BotUser, string> PropertyChanged;

   [JsonConstructor]
   public BotUser()
   {
   }

   public BotUser(long senderId)
   {
      SenderId = senderId;
      BotState = BotState.Idle;
   }

   public void SetState(BotState newState)
   {
      if (BotState != newState)
      {
         BotState = newState;
         PropertyChanged?.Invoke(this, nameof(BotState));
      }
   }
}
