using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F8 RID: 2040
	public static class ListExtensions
	{
		// Token: 0x06004A0D RID: 18957 RVA: 0x001F13A8 File Offset: 0x001EF5A8
		public static T GetElement<T>(this List<T> list, Func<T, bool> cmp) where T : class
		{
			foreach (T t in list)
			{
				if (cmp(t))
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x06004A0E RID: 18958 RVA: 0x001F1408 File Offset: 0x001EF608
		public static List<T> Shuffle<T>(this List<T> collection)
		{
			int count = collection.Count;
			for (int i = 0; i < count - 1; i++)
			{
				int num = TIUtilities.RandomRange(i, count);
				T t = collection[i];
				collection[i] = collection[num];
				collection[num] = t;
			}
			return collection;
		}

		// Token: 0x06004A0F RID: 18959 RVA: 0x001F1454 File Offset: 0x001EF654
		public static List<T> AddSizeItemsToDefault<T>(this List<T> list, int newSize, T defaultValue = default(T))
		{
			if (list.Count < newSize)
			{
				for (int i = list.Count - 1; i < newSize; i++)
				{
					if (i > 0)
					{
						list.Add(list[i - 1]);
					}
					else if (i == 0)
					{
						list.Add(defaultValue);
					}
				}
			}
			return list;
		}
	}
}
