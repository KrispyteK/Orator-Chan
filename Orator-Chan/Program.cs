using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OratorChan {
    class Program {
        private Client _client;
        private CommandHandler _commandHandler;
        private OratorCommandService _commandService;
        private CommunicationHandler _communicationHandler;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync () {
            _client = new Client();

            _client.Config = RetrieveConfig();
            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            _commandService = new OratorCommandService();
            _commandHandler = new CommandHandler(_client, _commandService);

            await _commandHandler.InstallCommandsAsync();

            _communicationHandler = new CommunicationHandler(_client);

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

        private Configuration RetrieveConfig () {
            using (StreamReader file = File.OpenText(@$"{Environment.CurrentDirectory}\Resources\config.json"))
            using (JsonTextReader reader = new JsonTextReader(file)) {
                return JToken.ReadFrom(reader).ToObject<Configuration>();
            }
        }
    }
}
