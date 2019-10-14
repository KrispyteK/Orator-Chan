using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OratorChan.Bot.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OratorChan.Bot.Communication
{

	public class CommunicationHandler
	{

		private readonly Client _client;
		private readonly Random _random = new Random();
		private readonly Learning _learning = new Learning();

		public CommunicationHandler(Client client)
		{
			_client = client;
			_client.MessageReceived += HandleMessageAsync;

			_client.Ready += LearnNewMessages;
		}

		private void SetLatestMessage(IMessage message)
		{
			if (!_client.GuildData.Channels.ContainsKey(message.Channel.Id))
			{
				_client.GuildData.Channels[message.Channel.Id] = new ChannelData
				{
					lastMessageLearned = message.Id,
					oldestMessageLearned = message.Id
				};
			}
			else
			{
				_client.GuildData.Channels[message.Channel.Id].lastMessageLearned = message.Id;
			}
		}

		private async Task HandleMessageAsync(SocketMessage messageParam)
		{
			// Don't process the command if it was a system message
			if (!(messageParam is SocketUserMessage message)) return;

			if (message.Author.Id == _client.CurrentUser.Id) return;

			if (!_client.GuildData.Channels.ContainsKey(message.Channel.Id))
			{
				return;
			}

			ChannelData data = _client.GuildData.Channels[message.Channel.Id];

			if (data.learn)
			{

				// Create a number to track where the prefix ends and the command begins
				int argPos = 0;

				// Determine if the message is a command based on the prefix and make sure no bots trigger commands
				if (message.HasCharPrefix(_client.Config.Prefix[0], ref argPos))
					return;


				await _learning.LearnFromString(message.Content);

				SetLatestMessage(message);
			}

			if (data.reply)
			{
				var constructedMessage = ConstructMessage(message.Content.Split(' '));

				if (constructedMessage.Trim().Length == 0)
				{
					return;
				}

				await message.Channel.SendMessageAsync(constructedMessage);
			}
		}

		public string ConstructMessage(string[] referenceWords = null)
		{
			WordSet currentWord = _learning.GetWord(referenceWords[_random.Next(0, referenceWords.Length - 1)]);

			string message = "";
			bool lastPunctuation = false;
			int length = 0;

			List<string> usedReferences = new List<string>();

			do
			{

				usedReferences.Add(currentWord.Word);

				if (string.IsNullOrEmpty(message))
				{
					message = string.Format("{0}{1}", char.ToUpper(currentWord.Word[0]), currentWord.Word.Substring(1));
				} else
				{
					var word = currentWord.Word;
					if (lastPunctuation)
					{
						word = string.Format("{0}{1}", char.ToUpper(currentWord.Word[0]), currentWord.Word.Substring(1));
						lastPunctuation = false;
					}
					message = string.Format("{0} {1}", message, word);
				}
				length++;

				if (currentWord.SentanceEnd && _random.NextDouble() > 0.80d)
				{
					string[] wordEnds = { ".", "!", "?" };
					message = string.Format("{0}{1}", message, wordEnds[_random.Next(0, wordEnds.Length - 1)]);
					break;
				} else if (currentWord.Punctuate && _random.NextDouble() > 0.80d && length > 3)
				{
					string[] punctuation = { ".", "!", "?", ":", ";", "'s", "..."};
					message = string.Format("{0}{1}", message, punctuation[_random.Next(0, punctuation.Length - 1)]);
					lastPunctuation = true;
				}

				var words = currentWord.Words.Keys.ToList();
				if (words.Count() > 0)
				{
					var wordToPick = words[_random.Next(0, words.Count())];

					currentWord = _learning.GetWord(wordToPick);

					if (lastPunctuation)
					{

						if (currentWord.Words.Count() == 0)
						{
							break;
						}
						else
						{

							if (referenceWords.Length != usedReferences.Count())
							{
								int attempts = 0;
								do
								{
									currentWord = _learning.GetWord(referenceWords[_random.Next(0, referenceWords.Length - 1)]);
									attempts++;
								} while (attempts < 10 && usedReferences.Contains(currentWord.Word));

								if (attempts >= 10 )
								{
									currentWord = null;
								}
							}
						}
					}
				}
			} while (currentWord != null && currentWord.Words.Count() > 0);


			return message;
		}

		public async Task LearnChannelHistory(ITextChannel channel)
		{
			var lastMessages = await channel.GetMessagesAsync(1, CacheMode.AllowDownload, RequestOptions.Default).FlattenAsync();
			var lastMessage = lastMessages.First();
			int messagesLearned = 0;

			if (!_client.GuildData.Channels.ContainsKey(channel.Id))
			{
				_client.GuildData.Channels[channel.Id] = new ChannelData
				{
					lastMessageLearned = lastMessage.Id,
					oldestMessageLearned = lastMessage.Id,
					learn = true
				};
				_client.GuildData.Save();
			} else
			{
				_client.GuildData.Channels[channel.Id].learn = true;
			}

			var channelData = _client.GuildData.Channels[channel.Id];

			await Task.Run(() =>
			{
				while (!channelData.hasOldestMessage)
				{
					var messages = channel.GetMessagesAsync(channelData.oldestMessageLearned, Direction.Before).FlattenAsync().Result;

					if (!messages.Any())
					{
						channelData.hasOldestMessage = true;

						channel.SendMessageAsync($"Succesfully learned full channel history!");

						break;
					}

					foreach (var msg in messages)
					{
						if (msg.Author.Id == _client.CurrentUser.Id) continue;

						Task.WaitAll(_learning.LearnFromString(msg.Content));

						messagesLearned++;
					}

					channelData.oldestMessageLearned = messages.Last().Id;
					_learning.SetDirty();
					Console.WriteLine($"Learned from {messagesLearned} messages.");
				}
			});
		}

		public async Task LearnFromChannel(ITextChannel channel)
		{
			var lastMessages = await channel.GetMessagesAsync(1, CacheMode.AllowDownload, RequestOptions.Default).FlattenAsync();
			var lastMessage = lastMessages.First();

			if (!_client.GuildData.Channels.ContainsKey(channel.Id))
			{
				_client.GuildData.Channels[channel.Id] = new ChannelData
				{
					lastMessageLearned = lastMessage.Id,
					oldestMessageLearned = lastMessage.Id
				};
			}

			var channelData = _client.GuildData.Channels[channel.Id];

			var newerMessages = channel.GetMessagesAsync(channelData.lastMessageLearned, Direction.After).FlattenAsync().Result;

			await Task.Run(() =>
			{
				while (newerMessages.Any())
				{
					Console.WriteLine($"Learning from {newerMessages.Count()} messages.");

					foreach (var msg in newerMessages)
					{
						if (msg.Author.Id == _client.CurrentUser.Id) continue;

						Task.Run(() => _learning.LearnFromString(msg.Content));
						_learning.SetDirty();
					}

					channelData.lastMessageLearned = newerMessages.Last().Id;

					newerMessages = channel.GetMessagesAsync(channelData.lastMessageLearned, Direction.After).FlattenAsync().Result;
				}
			});
		}

        public async Task LearnNewMessages() {
			if (_client.GuildData.BaseChannel < 1)
			{
				return;
			}

            var guild = _client.Guilds.FirstOrDefault(x => x.Id == _client.GuildData.Guild);
            var baseChannel = guild.GetTextChannel(_client.GuildData.BaseChannel) as ITextChannel;

            foreach (var kv in _client.GuildData.Channels) {
                var channel = guild.GetTextChannel(kv.Key) as ITextChannel;

                await baseChannel.SendMessageAsync($"Learning new messages from {channel.Name}");

                await LearnFromChannel(channel);
            }
        }
    }
}
