using System;
using System.Collections.Generic;
using System.Text;

namespace OratorChan {
    [Serializable]
    public class Configuration {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string[] LearningChannels { get; set; }
        public string[] ReplyChannels { get; set; }
    }
} 
