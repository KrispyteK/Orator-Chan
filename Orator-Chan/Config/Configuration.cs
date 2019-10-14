using System;
using System.Collections.Generic;
using System.Text;

namespace OratorChan {
    [Serializable]
    public class Configuration {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public ulong[] LearningChannels { get; set; }
        public ulong[] ReplyChannels { get; set; }
        public ulong[] Admins { get; set; }
        public ulong BaseChannel { get; set; }
        public ulong GuildID { get; set; }
    }
} 
