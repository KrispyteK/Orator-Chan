using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OratorChan.Bot.Data;

namespace OratorChan.Bot.Communication
{
	public class Learning
	{
		public Dictionary<string, WordSet> WordData;
		private bool _dirtyData;
		private Timer _timer; 

		public Learning()
		{
			WordData = Program.GetResource<Dictionary<string, WordSet>>("Data", new Dictionary<string, WordSet>());
			_timer = new Timer(new TimerCallback(SaveData), this, 1000, 10000);
		}

		private static void SaveData(object learning)
		{

			var state = learning as Learning;
			if (!state._dirtyData)
			{
				return;
			}

			Program.SaveResource("Data", state.WordData);

			state._dirtyData = false;
		}

		public WordSet GetWord(string word)
		{
			word = RemovePunctuationFromWord(word);
			if (!WordData.ContainsKey(word))
			{
				WordData[word] = new WordSet(word);
			}

			return WordData[word];
		}

		public void SetDirty()
		{
			_dirtyData = true;
		}

		public async Task LearnFromString(string message)
		{
			var words = message.Trim().Split(new char[] { ' ', '\t', '\n' });

			for (int i = 0; i < words.Length; i++)
			{
				var dirtyWord = words[i].Trim();
				var word = RemovePunctuationFromWord(dirtyWord);
				var nextWord = i < words.Length - 1 ? words[i + 1] : string.Empty;

				WordSet data;

				if (WordData.ContainsKey(word))
				{
					data = WordData[word];
				}
				else
				{
					data = WordData[word] = new WordSet(word);
				}

				if (!dirtyWord.Equals(word, StringComparison.OrdinalIgnoreCase))
				{
					data.Punctuate = true;
				}

				if (string.IsNullOrWhiteSpace(nextWord))
				{
					data.SentanceEnd = true;
				} else
				{
					data.AddWord(nextWord);
				}
			}
		}

		private string RemovePunctuationFromWord(string message)
		{
			return new string(message.Where(c => !char.IsPunctuation(c)).ToArray()).Trim().ToLower();
		}

	}
}
