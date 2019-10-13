using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace OratorChan {
    public class Client : DiscordSocketClient {
        public Configuration Config { get; set; }
    }
}
