﻿using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OratorChan {
    class Program {

        public JObject localconfig;
        private DiscordSocketClient _client;
        

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync () {
            localconfig = RetrieveConfig();


            _client = new DiscordSocketClient();

            _client.Log += Log;

            // Remember to keep token private or to read it from an 
            // external source! In this case, we are reading the token 
            // from an environment variable. If you do not know how to set-up
            // environment variables, you may find more information on the 
            // Internet or by using other methods such as reading from 
            // a configuration.
            await _client.LoginAsync(TokenType.Bot,
                localconfig.GetValue("DiscordToken").ToString());
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private JObject RetrieveConfig () {
            using (StreamReader file = File.OpenText(@$"{Environment.CurrentDirectory}\Config\localconfig.json"))
            using (JsonTextReader reader = new JsonTextReader(file)) {
                return (JObject)JToken.ReadFrom(reader);
            }
        }
    }
}
