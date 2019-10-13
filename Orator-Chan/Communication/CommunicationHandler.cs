using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OratorChan {
    public class CommunicationHandler {
        public Dictionary<string, WordSet> wordData;

        private readonly Client _client;
        private readonly Random _random;
        private bool _hasData;
        private int _saveInterval = 5;
        private int _messagesSinceSave = 0;

        public CommunicationHandler(Client client) {
            wordData = new Dictionary<string, WordSet>();
            _random = new Random();

            _client = client;
            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task SaveData() {
            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(@$"{Environment.CurrentDirectory}\Resources\data.json")) {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, wordData);
            }
        }

        private async Task GetData() {
            using (StreamReader file = File.OpenText(@$"{Environment.CurrentDirectory}\Resources\data.json"))
            using (JsonTextReader reader = new JsonTextReader(file)) {
                wordData = JToken.ReadFrom(reader).ToObject<Dictionary<string, WordSet>>();
            }

            _hasData = true;
        }

        private async Task HandleMessageAsync(SocketMessage messageParam) {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            if (message.Author.Id == _client.CurrentUser.Id) return;

            if (!_hasData) {
                await GetData();
            }

            if (_client.Config.LearningChannels.Contains(message.Channel.Id.ToString())) {

                // Create a number to track where the prefix ends and the command begins
                int argPos = 0;

                // Determine if the message is a command based on the prefix and make sure no bots trigger commands
                if (message.HasCharPrefix(_client.Config.Prefix[0], ref argPos))
                    return;

                await LearnFromString(message.Content);
            }

            if (_client.Config.ReplyChannels.Contains(message.Channel.Id.ToString())) {
                var constructedMessage = ConstructMessage(message.Content.Split(' '));

                await message.Channel.SendMessageAsync(constructedMessage);
            }

            _messagesSinceSave++;

            if (_messagesSinceSave == _saveInterval) {
                await SaveData();

                _saveInterval = 0;
            }
        }

        public string RemovePunctuation (string str) {
            return str.Replace(",", "").Replace(".", "").Replace("?", "").Replace("!", "");
        }

        public string ConstructMessage(string[] referenceWords = null) {
            var word = string.Empty;

            if (referenceWords != null) {
                var reference = referenceWords[_random.Next(0, referenceWords.Length - 1)];

                var find = wordData.Keys.ToList().Find(x => RemovePunctuation(x).ToLower() == RemovePunctuation(reference).ToLower());

                word = find ?? reference;
            }
            else {
                word = wordData.ElementAt(_random.Next(0, wordData.Keys.Count - 1)).Key;
            }

            var message = word;
            int repetition = 0;
            string latestSubString = string.Empty;

            while ((word != string.Empty && !word.EndsWith('.') && !word.EndsWith('?') && !word.EndsWith('!')) && message.Length < 1900) {
                var wordSet = wordData[word];
                var max = wordSet.Words.Values.Max();
                var choice = _random.Next(0, Math.Max(max - repetition, 0));

                var keys = wordSet.Words.Keys.OrderByDescending(x => _random.NextDouble() > 0.5d);

                // Choose next word based on frequency
                foreach (var key in keys) {
                    var value = wordSet.Words[key];
                    var checkWord = key;

                    if (value >= choice) {
                        if (checkWord == word) {
                            // If a word is repeated too much there's a bigger chance there's going to be a different word next.
                            if (repetition > 0 && _random.NextDouble() > 1 / repetition) {
                                continue;
                            }

                            repetition++;
                        }
                        // Attempt to reduce repeated sections.
                        else if (message.Contains(checkWord) && !message.EndsWith(checkWord)) {
                            var index = message.LastIndexOf(checkWord);
                            var subString = message.Substring(index, message.Length - index);

                            if (latestSubString.EndsWith(subString)) {
                                latestSubString = string.Empty;
                                choice = _random.Next(0, Math.Max(max - repetition, 0));
                                continue;
                            }
                            
                        }
                        else {
                            repetition = 0;
                            //latestSubString = string.Empty;
                        }

                        word = checkWord;
                        break;
                    }
                }

                message += $" {word}";
                latestSubString += $" {word}";
                latestSubString = latestSubString.Trim();
            }

            if (message.Length > 3) {
                message = message[0].ToString().ToUpper() + message.Substring(1, message.Length - 2);
            }

            if (!message.EndsWith(".")) message += ".";

            return message;
        }

        public async Task LearnFromString(string message) {
            var words = message.Trim().Split(new char[] { ' ', '\t', '\n' });

            for (int i = 0; i < words.Length; i++) {
                var word = words[i].Trim();
                var nextWord = i < words.Length - 1 ? words[i + 1] : string.Empty;

                if (wordData.ContainsKey(word)) {
                    wordData[word].AddWord(nextWord);
                }
                else {
                    var newWordSet = new WordSet(word);
                    newWordSet.AddWord(nextWord);

                    wordData[word] = newWordSet;
                }
            }
        }
    }
}
