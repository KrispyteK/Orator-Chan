using System;
using System.Collections.Generic;
using System.Text;

namespace OratorChan {
    [Serializable]
    public class GuildData {
        public Dictionary<ulong, ChannelData> Channels { get; private set; } = new Dictionary<ulong, ChannelData>();
    }
    
    [Serializable]
    public class ChannelData {
        public ulong lastMessageLearned;
        public ulong oldestMessageLearned;
        public bool hasOldestMessage;
        public bool hasNewestMessage;
    }
}
