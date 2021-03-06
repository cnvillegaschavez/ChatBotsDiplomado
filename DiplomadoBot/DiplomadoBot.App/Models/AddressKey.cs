﻿using Microsoft.Bot.Builder.Dialogs;

namespace DiplomadoBot.App.Models
{
    public class AddressKey : IAddress
    {
        public string BotId { get; set; }
        public string ChannelId { get; set; }
        public string ConversationId { get; set; }
        public string ServiceUrl { get; set; }
        public string UserId { get; set; }
    }

}