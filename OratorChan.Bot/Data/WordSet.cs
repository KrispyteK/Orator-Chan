using System;
using System.Collections.Generic;

namespace OratorChan.Bot.Data
{

	[Serializable]
	public class WordSet
	{

		public string Word;
		public Dictionary<string, int> Words { get; set; }

		public bool SentanceEnd;
		public bool Punctuate;

		public WordSet(string word)
		{
			Word = word.ToLower();
			Words = new Dictionary<string, int>();
		}

		public void AddWord(string word)
		{
			word = word.ToLower();

			if (Word.Equals(word, StringComparison.Ordinal))
			{
				return;
			}

			if (Words.ContainsKey(word))
			{
				Words[word]++;
			}
			else
			{
				Words[word] = 1;
			}
		}
	}
}
