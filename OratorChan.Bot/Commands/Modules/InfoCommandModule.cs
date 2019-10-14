using Discord;
using Discord.Commands;
using OratorChan.Bot.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OratorChan.Bot.Commands.Modules
{

	public class InfoCommandModule : ModuleBase<SocketCommandContext>
	{

		[Command("say")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
			=> ReplyAsync(echo);

		[Command("learn")]
		[Summary("Learns from messages in the current channel")]
		public async Task LearnFromChannelAsync()
		{
			var client = Context.Client as Client;

			if (client.Config.Admins.Contains(Context.User.Id))
			{
				var channel = Context.Channel;

				await ReplyAsync($"Learning from {channel.Name}. This might take a while...");

				await Task.Run(() => client.CommunicationHandler.LearnChannelHistory(channel as ITextChannel));
			}
		}

		[Command("reply")]
		[Summary("Replys to messages in the current channel")]
		public async Task ReplyToChannelAsync()
		{
			var client = Context.Client as Client;

			if (client.Config.Admins.Contains(Context.User.Id))
			{
				var channel = Context.Channel as ITextChannel;

				await Task.Run(() => {
					if (!client.GuildData.Channels.ContainsKey(channel.Id))
					{
						client.GuildData.Channels[channel.Id] = new ChannelData
						{
							reply = true
						};
					} else
					{
						client.GuildData.Channels[channel.Id].reply = true;
					}
					client.GuildData.Save();
				});
			}
		}
	}
}
