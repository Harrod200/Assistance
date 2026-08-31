using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000404 RID: 1028
public static class DictionaryExtensions
{
	// Token: 0x06001524 RID: 5412 RVA: 0x00066F5F File Offset: 0x0006515F
	public static string ToDetailedString<T, V>(this Dictionary<T, V> dict)
	{
		return string.Join("; ", dict.Select<KeyValuePair<T, V>, string>((KeyValuePair<T, V> x) => string.Format("{0}: {1}", x.Key, x.Value)));
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x00066F90 File Offset: 0x00065190
	public static Dictionary<T, IEnumerable<U>> ToEnumerableDictionary<T, U>(this Dictionary<T, List<U>> dictionary)
	{
		return dictionary.Keys.ToDictionary<T, T, IEnumerable<U>>((T x) => x, (T x) => dictionary[x]);
	}

	// Token: 0x06001526 RID: 5414 RVA: 0x00066FE8 File Offset: 0x000651E8
	public static Dictionary<T, IEnumerable<U>> ToEnumerableDictionary<T, U>(this Dictionary<T, HashSet<U>> dictionary)
	{
		return dictionary.Keys.ToDictionary<T, T, IEnumerable<U>>((T x) => x, (T x) => dictionary[x]);
	}

	// Token: 0x06001527 RID: 5415 RVA: 0x00067040 File Offset: 0x00065240
	public static Dictionary<U, T> Inverted<T, U>(this Dictionary<T, IEnumerable<U>> dictionary)
	{
		Dictionary<U, T> dictionary2 = new Dictionary<U, T>();
		foreach (T t in dictionary.Keys)
		{
			foreach (U u in dictionary[t])
			{
				dictionary2.Add(u, t);
			}
		}
		return dictionary2;
	}

	// Token: 0x06001528 RID: 5416 RVA: 0x000670D4 File Offset: 0x000652D4
	public static Dictionary<U, T> Inverted<T, U>(this Dictionary<T, List<U>> dictionary)
	{
		return dictionary.ToEnumerableDictionary<T, U>().Inverted<T, U>();
	}

	// Token: 0x06001529 RID: 5417 RVA: 0x000670E4 File Offset: 0x000652E4
	public static Dictionary<K, V> CorrectEnumKeyedDictionary<K, V>(this Dictionary<K, V> dictionary, V defaultValue = default(V)) where K : Enum
	{
		foreach (object obj in Enum.GetValues(typeof(K)))
		{
			K k = (K)((object)obj);
			if (!dictionary.ContainsKey(k))
			{
				dictionary.Add(k, defaultValue);
			}
		}
		return dictionary;
	}
}
