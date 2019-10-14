using System;
using System.Collections.Generic;

namespace OratorChan.Bot.Data
{

	[Serializable]
	public class GuildData
	{

		public Dictionary<ulong, ChannelData> Channels { get; private set; } = new Dictionary<ulong, ChannelData>();

	}
}
