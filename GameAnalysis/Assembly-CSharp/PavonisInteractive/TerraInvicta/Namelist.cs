using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006FC RID: 1788
	public class Namelist<TKey> : INamelist<TKey>, INamelist where TKey : INamelistKey<TKey>
	{
		// Token: 0x06002A74 RID: 10868 RVA: 0x000E6751 File Offset: 0x000E4951
		public Namelist(string filename, INamelistParser<TKey> parser, List<string> moddedNameLists = null)
		{
			this.filename = filename;
			this.parser = parser;
			this.names = new Dictionary<TKey, List<NamelistEntry>>();
			this.weights = new Dictionary<TKey, int>();
			this.moddedNameListPaths = new List<string>(moddedNameLists);
			this.LoadFile();
		}

		// Token: 0x06002A75 RID: 10869 RVA: 0x000E6790 File Offset: 0x000E4990
		public string GetName(TKey key)
		{
			TKey tkey = key.Any();
			int num = 0;
			List<NamelistEntry> list;
			if (this.names.TryGetValue(tkey, out list))
			{
				num += this.weights[tkey];
			}
			List<NamelistEntry> list2;
			if (this.names.TryGetValue(key, out list2))
			{
				num += this.weights[key];
			}
			if (num > 0)
			{
				int num2 = TIUtilities.RandomRange(0, num - 1);
				if (list2 != null)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						NamelistEntry namelistEntry = list2[i];
						if (namelistEntry.weight > num2)
						{
							return namelistEntry.name;
						}
						num2 -= namelistEntry.weight;
					}
				}
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						NamelistEntry namelistEntry2 = list[j];
						if (namelistEntry2.weight > num2)
						{
							return namelistEntry2.name;
						}
						num2 -= namelistEntry2.weight;
					}
					return list[list.Count - 1].name;
				}
			}
			return string.Empty;
		}

		// Token: 0x06002A76 RID: 10870 RVA: 0x000E6898 File Offset: 0x000E4A98
		private void LoadFile()
		{
			if (Error.Is(!File.Exists(this.filename), "Could not load namelist, file not found: " + this.filename, Array.Empty<object>()))
			{
				return;
			}
			string[] array = File.ReadAllLines(this.filename);
			foreach (string text in this.moddedNameListPaths)
			{
				array = array.Concat<string>(File.ReadAllLines(text)).ToArray<string>();
			}
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[] { ',' });
				TKey tkey = this.parser.ParseKey(array3);
				NamelistEntry namelistEntry = this.parser.ParseEntry(array3);
				if (namelistEntry.weight != 0)
				{
					if (!this.weights.ContainsKey(tkey))
					{
						this.weights[tkey] = 0;
						this.names[tkey] = new List<NamelistEntry>(100);
					}
					this.names[tkey].Add(namelistEntry);
					Dictionary<TKey, int> dictionary = this.weights;
					TKey tkey2 = tkey;
					dictionary[tkey2] += namelistEntry.weight;
				}
			}
		}

		// Token: 0x0400209D RID: 8349
		private readonly Dictionary<TKey, List<NamelistEntry>> names;

		// Token: 0x0400209E RID: 8350
		private readonly Dictionary<TKey, int> weights;

		// Token: 0x0400209F RID: 8351
		private readonly INamelistParser<TKey> parser;

		// Token: 0x040020A0 RID: 8352
		private readonly string filename;

		// Token: 0x040020A1 RID: 8353
		private readonly List<string> moddedNameListPaths;
	}
}
