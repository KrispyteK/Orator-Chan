using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OratorChan.Bot.Commands;
using OratorChan.Bot.Communication;
using OratorChan.Bot.Config;
using OratorChan.Bot.Data;

namespace OratorChan.Bot {
    class Program {
        private Client _client;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync () {
            _client = new Client();

            _client.Config = GetResource<Configuration>("config");
            _client.GuildData = GetResource<GuildData>("config");
            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            _client.CommandService = new OratorCommandService();
            _client.CommandHandler = new CommandHandler(_client, _client.CommandService);

            await _client.CommandHandler.InstallCommandsAsync();

            _client.CommunicationHandler = new CommunicationHandler(_client);

            await _client.LoginAsync(TokenType.Bot, _client.Config.Token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message) {
            //if (message.Content.StartsWith(config.Prefix)) {

            //    if (message.Content == "!ping") {
            //        await message.Channel.SendMessageAsync("Pong!");
            //    }
            //}
        }

		private T GetResource<T>(string resource)
		{

			string fileContent = File.ReadAllText(@$"{Environment.CurrentDirectory}\Resources\{resource}.json");
			return JsonConvert.DeserializeObject<T>(fileContent);
		}
    }
}
