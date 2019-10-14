using System;
using System.Collections.Generic;

namespace OratorChan.Bot.Communication
{

	[Serializable]
	public struct WordSet
	{

		public string Word;
		public Dictionary<string, int> Words { get; set; }

		public WordSet(string word)
		{
			Word = word;
			Words = new Dictionary<string, int>();
		}

		public void AddWord(string word)
		{
			if (Words.ContainsKey(word))
			{
				Words[word]++;
			}
			else
			{
				Words[word] = word == string.Empty ? 0 : 1;
			}
		}
	}
}
