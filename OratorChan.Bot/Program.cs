using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OratorChan.Bot.Commands;
using OratorChan.Bot.Communication;
using OratorChan.Bot.Config;
using OratorChan.Bot.Data;

namespace OratorChan.Bot {
    class Program {
        private Client _client;
		private static JsonSerializer _serializer = new JsonSerializer();


		public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync () {
            _client = new Client();

            _client.Config = GetResource<Configuration>(null);
            _client.GuildData = GetResource<GuildData>(new GuildData());
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

		public static T GetResource<T>(string name, object defaultValue)
		{
			string path = Path.Combine(Environment.CurrentDirectory, "Resources", string.Format("{0}.json", name));

			if (!File.Exists(path))
			{
				return (T) defaultValue;
			}

			try
			{
				using (StreamReader file = File.OpenText(path))
				{
					return (T) _serializer.Deserialize(file, typeof(T));
				}
			} catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return (T) defaultValue;
			}

		}

		public static T GetResource<T>(object defaultValue)
		{
			return GetResource<T>(typeof(T).Name, defaultValue);
		}

		public static void SaveResource(string name, object resource)
		{
			using (StreamWriter file = File.CreateText(Path.Combine(Environment.CurrentDirectory, "Resources", string.Format("{0}.json", name))))
			{
				_serializer.Serialize(file, resource);
			}
		}

		public static void SaveResource(object resource)
		{
			SaveResource(resource.GetType().Name, resource);
		}
	}
}
