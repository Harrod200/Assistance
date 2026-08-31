using System;
using System.Collections.Generic;
using System.Text;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200071B RID: 1819
	public static class Throw
	{
		// Token: 0x06002BED RID: 11245 RVA: 0x000F0821 File Offset: 0x000EEA21
		public static void CollectionItemMissing<T>(TIGameState target, T source, ICollection<T> collection, string collectionName) where T : TIGameState
		{
			Throw.builder.Clear();
			Throw.builder.AppendLine(string.Format("{0} does not contain {1} in {2}", target, source, collectionName));
			Throw.BuildCollectionStrings<T>(collection);
			throw new CollectionItemMissing(Throw.builder.ToString());
		}

		// Token: 0x06002BEE RID: 11246 RVA: 0x000F0860 File Offset: 0x000EEA60
		public static void Null<T>(string name)
		{
			throw new ArgumentNullException(name, typeof(T).Name);
		}

		// Token: 0x06002BEF RID: 11247 RVA: 0x000F0878 File Offset: 0x000EEA78
		private static void BuildCollectionStrings<T>(ICollection<T> collection) where T : TIGameState
		{
			int num = 0;
			foreach (T t in collection)
			{
				Throw.builder.AppendLine(string.Format("{0}: {1}", num++, t));
			}
		}

		// Token: 0x04002170 RID: 8560
		private static readonly StringBuilder builder = new StringBuilder();
	}
}
