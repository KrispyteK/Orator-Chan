﻿using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OratorChan {
    class Program {

        public Configuration config;
        private DiscordSocketClient _client;       

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync () {
            config = RetrieveConfig();

            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            await _client.LoginAsync(TokenType.Bot,config.Token);
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message) {

            if (message.Content.StartsWith(config.Prefix)) {

                if (message.Content == "!ping") {
                    await message.Channel.SendMessageAsync("Pong!");
                }
            }
        }

        private Configuration RetrieveConfig () {
            using (StreamReader file = File.OpenText(@$"{Environment.CurrentDirectory}\Resources\config.json"))
            using (JsonTextReader reader = new JsonTextReader(file)) {
                return JToken.ReadFrom(reader).ToObject<Configuration>();
            }
        }
    }
}
