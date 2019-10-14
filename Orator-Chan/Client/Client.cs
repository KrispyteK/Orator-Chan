using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace OratorChan {
    public class Client : DiscordSocketClient {
        public Configuration Config { get; set; }
        public GuildData GuildData { get; set; }
        public CommandHandler CommandHandler { get; set; }
        public OratorCommandService CommandService { get; set; }
        public CommunicationHandler CommunicationHandler { get; set; }
    }
}
