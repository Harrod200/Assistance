using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000403 RID: 1027
public static class EnumerableExtensions
{
	// Token: 0x0600150D RID: 5389 RVA: 0x00066934 File Offset: 0x00064B34
	public static T SelectRandomWeightedItem<T>(this IEnumerable<T> weightedList, Func<T, float> Selector, float totalWeight = -1f, float min = 1E-37f)
	{
		if (totalWeight < 0f)
		{
			totalWeight = weightedList.Sum<T>((T x) => Mathf.Max(min, Selector(x)));
		}
		if (totalWeight > 0f)
		{
			float num = TIUtilities.RandomFloatValue() * totalWeight;
			float num2 = 0f;
			Func<T, <>f__AnonymousType0<T, float>> <>9__1;
			var func;
			if ((func = <>9__1) == null)
			{
				func = (<>9__1 = (T weightedItem) => new
				{
					Value = weightedItem,
					Weight = Mathf.Max(min, Selector(weightedItem))
				});
			}
			foreach (var anon in weightedList.Select(func))
			{
				num2 += anon.Weight;
				if (num2 >= num)
				{
					return anon.Value;
				}
			}
		}
		return weightedList.LastOrDefault<T>();
	}

	// Token: 0x0600150E RID: 5390 RVA: 0x00066A04 File Offset: 0x00064C04
	public static IEnumerable<T> SelectRandomWeightedItems<T>(this IEnumerable<T> enumberable, Func<T, float> Selector, int count, bool doNotReplace = true)
	{
		if (count == 0)
		{
			return Enumerable.Empty<T>();
		}
		IEnumerable<T> enumerable = Enumerable.Empty<T>();
		List<ValueTuple<T, float>> list = new List<ValueTuple<T, float>>();
		float num = 0f;
		foreach (T t in enumberable)
		{
			list.Add(new ValueTuple<T, float>(t, num = Selector(t) + num));
		}
		if (list.Count == 0)
		{
			return Enumerable.Empty<T>();
		}
		HashSet<int> hashSet = null;
		if (doNotReplace)
		{
			hashSet = new HashSet<int>();
			count = Mathf.Min(count, list.Count);
		}
		int i = 0;
		while (i < count)
		{
			float num2 = TIUtilities.RandomFloatValue();
			float num3 = num2 * num;
			int num4 = 0;
			int num5 = list.Count - 1;
			int num6 = ((float)num5 * num2).Round();
			int num7 = 1;
			for (;;)
			{
				float num8 = 0f;
				if (num6 > 0)
				{
					num8 = list[num6 - 1].Item2;
				}
				float item = list[num6].Item2;
				if (num3 >= num8 && num3 <= item)
				{
					break;
				}
				if (num3 < num8)
				{
					num5 = num6;
				}
				else
				{
					num4 = num6;
				}
				int num9 = num6;
				num6 = num4 + ((float)(num5 - num4) / 2f).Round();
				if (num6 == num9)
				{
					if (num6 < num5)
					{
						num6++;
					}
					else
					{
						num6--;
					}
				}
				num7++;
			}
			if (!doNotReplace)
			{
				goto IL_015C;
			}
			if (!hashSet.Contains(num6))
			{
				hashSet.Add(num6);
				goto IL_015C;
			}
			i--;
			IL_0170:
			i++;
			continue;
			IL_015C:
			enumerable = enumerable.Append(list[num6].Item1);
			goto IL_0170;
		}
		return enumerable;
	}

	// Token: 0x0600150F RID: 5391 RVA: 0x00066BA0 File Offset: 0x00064DA0
	public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
	{
		if (en != null && en.Any<T>())
		{
			return en.Select<T, Tuple<T, R>>((T t) => new Tuple<T, R>(t, evaluate(t))).Aggregate<Tuple<T, R>>(delegate(Tuple<T, R> max, Tuple<T, R> next)
			{
				R item = next.Item2;
				if (item.CompareTo(max.Item2) <= 0)
				{
					return max;
				}
				return next;
			}).Item1;
		}
		return default(T);
	}

	// Token: 0x06001510 RID: 5392 RVA: 0x00066C0C File Offset: 0x00064E0C
	public static T MinBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
	{
		if (en != null && en.Any<T>())
		{
			return en.Select<T, Tuple<T, R>>((T t) => new Tuple<T, R>(t, evaluate(t))).Aggregate<Tuple<T, R>>(delegate(Tuple<T, R> max, Tuple<T, R> next)
			{
				R item = next.Item2;
				if (item.CompareTo(max.Item2) >= 0)
				{
					return max;
				}
				return next;
			}).Item1;
		}
		return default(T);
	}

	// Token: 0x06001511 RID: 5393 RVA: 0x00066C78 File Offset: 0x00064E78
	public static T SelectRandomItem<T>(this IList<T> collection)
	{
		if (collection.Count == 0)
		{
			return default(T);
		}
		return collection[TIUtilities.RandomRange(0, collection.Count)];
	}

	// Token: 0x06001512 RID: 5394 RVA: 0x00066CAC File Offset: 0x00064EAC
	public static T SelectRandomItem<T>(this T[] collection)
	{
		if (collection.Length == 0)
		{
			return default(T);
		}
		return collection[TIUtilities.RandomRange(0, collection.Length)];
	}

	// Token: 0x06001513 RID: 5395 RVA: 0x00066CD8 File Offset: 0x00064ED8
	public static T SelectRandomItem<T>(this IEnumerable<T> collection)
	{
		int num = collection.Count<T>();
		if (num == 0)
		{
			return default(T);
		}
		return collection.ElementAt<T>(TIUtilities.RandomRange(0, num));
	}

	// Token: 0x06001514 RID: 5396 RVA: 0x00066D06 File Offset: 0x00064F06
	public static IEnumerable<T> SelectRandomItems<T>(this IEnumerable<T> collection, int count)
	{
		return collection.OrderBy<T, float>((T x) => TIUtilities.RandomFloatValue()).Take<T>(count);
	}

	// Token: 0x06001515 RID: 5397 RVA: 0x00066D33 File Offset: 0x00064F33
	public static bool None<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
	{
		return !collection.Any<T>(predicate);
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x00066D3F File Offset: 0x00064F3F
	public static bool NotAll<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
	{
		return !collection.All<T>(predicate);
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x00066D4B File Offset: 0x00064F4B
	public static bool OnlySome<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
	{
		return collection.Any<T>(predicate) && !collection.All<T>(predicate);
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x00066D62 File Offset: 0x00064F62
	public static bool AddUnique<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
			return true;
		}
		return false;
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x00066D78 File Offset: 0x00064F78
	public static void AddRangeUnique<T>(this List<T> list, List<T> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (!list.Contains(items[i]))
			{
				list.Add(items[i]);
			}
		}
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x00066DB4 File Offset: 0x00064FB4
	public static IEnumerable<ValueTuple<T, U>> ToCollection<T, U>(this Dictionary<T, IEnumerable<U>> dictionary)
	{
		return dictionary.Keys.Select<T, IEnumerable<ValueTuple<T, U>>>((T x) => dictionary[x].Select<U, ValueTuple<T, U>>((U y) => new ValueTuple<T, U>(x, y))).SelectMany<IEnumerable<ValueTuple<T, U>>, ValueTuple<T, U>>(([TupleElementNames(new string[] { "x", "y" })] IEnumerable<ValueTuple<T, U>> x) => x);
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x00066E0E File Offset: 0x0006500E
	public static IEnumerable<ValueTuple<T, U>> ToCollection<T, U>(this Dictionary<T, List<U>> dictionary)
	{
		return dictionary.ToEnumerableDictionary<T, U>().ToCollection<T, U>();
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x00066E1B File Offset: 0x0006501B
	public static IEnumerable<ValueTuple<T, U>> ToCollection<T, U>(this Dictionary<T, HashSet<U>> dictionary)
	{
		return dictionary.ToEnumerableDictionary<T, U>().ToCollection<T, U>();
	}

	// Token: 0x0600151D RID: 5405 RVA: 0x00066E28 File Offset: 0x00065028
	public static List<T> Sort<T, U>(this List<T> list, Func<T, U> Evaluate) where U : IComparable
	{
		list.Sort(delegate(T a, T b)
		{
			U u = Evaluate(a);
			return u.CompareTo(Evaluate(b));
		});
		return list;
	}

	// Token: 0x0600151E RID: 5406 RVA: 0x00066E55 File Offset: 0x00065055
	public static List<T> Sorted<T, U>(this IEnumerable<T> elements, Func<T, U> Evaluate) where U : IComparable
	{
		List<T> list = new List<T>(elements);
		list.Sort<T, U>(Evaluate);
		return list;
	}

	// Token: 0x0600151F RID: 5407 RVA: 0x00066E65 File Offset: 0x00065065
	public static IEnumerable<T> Take_Random<T>(this IEnumerable<T> elements, int count)
	{
		return elements.OrderBy<T, float>((T x) => TIUtilities.RandomFloatValue()).Take<T>(count);
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x00066E94 File Offset: 0x00065094
	public static IEnumerable<T> BottomPercentage<T, U>(this IEnumerable<T> elements, Func<T, U> Evaluate, float percentage) where U : IComparable
	{
		List<T> list = elements.Sorted<T, U>(Evaluate);
		int num = ((float)elements.Count<T>() * percentage).RoundUp();
		return list.GetRange(0, num);
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x00066EC0 File Offset: 0x000650C0
	public static IEnumerable<T> TopPercentage<T, U>(this IEnumerable<T> elements, Func<T, U> Evaluate, float percentage) where U : IComparable
	{
		List<T> list = elements.Sorted<T, U>(Evaluate);
		int num = ((float)elements.Count<T>() * percentage).RoundUp();
		int num2 = elements.Count<T>() - num;
		return list.GetRange(num2, num);
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x00066EF4 File Offset: 0x000650F4
	public static float Product(this float[] elements, float emptyValue = 0f)
	{
		if (elements.Length == 0)
		{
			return emptyValue;
		}
		float num = elements[0];
		for (int i = 1; i < elements.Count<float>(); i++)
		{
			num *= elements[i];
		}
		return num;
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x00066F24 File Offset: 0x00065124
	public static float Product(this List<float> elements, float emptyValue = 0f)
	{
		if (elements.Count == 0)
		{
			return emptyValue;
		}
		float num = elements[0];
		for (int i = 1; i < elements.Count<float>(); i++)
		{
			num *= elements[i];
		}
		return num;
	}
}
