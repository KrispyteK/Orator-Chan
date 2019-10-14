using System;

namespace OratorChan.Bot.Config
{

	[Serializable]
	public class Configuration
	{
		public string Token { get; set; }
		public string Prefix { get; set; }
		public ulong[] Admins { get; set; }
    }
}
