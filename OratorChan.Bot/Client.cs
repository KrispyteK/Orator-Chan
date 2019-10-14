using Discord.WebSocket;
using OratorChan.Bot.Commands;
using OratorChan.Bot.Communication;
using OratorChan.Bot.Config;
using OratorChan.Bot.Data;

namespace OratorChan.Bot
{

    public class Client : DiscordSocketClient
	{

        public Configuration Config { get; set; }
        public GuildData GuildData { get; set; }
        public CommandHandler CommandHandler { get; set; }
        public OratorCommandService CommandService { get; set; }
        public CommunicationHandler CommunicationHandler { get; set; }

    }

}
