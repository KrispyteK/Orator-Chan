using System;

namespace OratorChan.Bot.Data
{

	[Serializable]
	public class ChannelData
	{

		public ulong lastMessageLearned;
		public ulong oldestMessageLearned;
		public bool hasOldestMessage;
		public bool hasNewestMessage;

		public bool learn;
		public bool reply;

	}
}
