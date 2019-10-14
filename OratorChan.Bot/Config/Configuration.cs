using System;
using System.Collections.Generic;
using System.Text;

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
