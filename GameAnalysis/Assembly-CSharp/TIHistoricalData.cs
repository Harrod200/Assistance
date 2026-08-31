using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using FullSerializer;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200043F RID: 1087
public class TIHistoricalData : TIGameState
{
	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06001684 RID: 5764 RVA: 0x00072EFC File Offset: 0x000710FC
	public static TIHistoricalData Singleton
	{
		get
		{
			if (TIHistoricalData.singleton == null)
			{
				TIHistoricalData.singleton = GameStateManager.IterateByClass<TIHistoricalData>(false).FirstOrDefault<TIHistoricalData>();
				if (TIHistoricalData.singleton == null)
				{
					TIHistoricalData.singleton = GameStateManager.CreateNewGameState<TIHistoricalData>();
				}
				TIHistoricalData.singleton.displayName = "Graph";
			}
			return TIHistoricalData.singleton;
		}
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x00072F51 File Offset: 0x00071151
	public static void ClearStaticData()
	{
		TIHistoricalData.singleton = null;
	}

	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06001686 RID: 5766 RVA: 0x00072F59 File Offset: 0x00071159
	public static IEnumerable<TIGameState> States
	{
		get
		{
			return TIHistoricalData.Singleton.Data.Keys;
		}
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x00072F6A File Offset: 0x0007116A
	public static IEnumerable<string> GetAttributes(TIGameState state)
	{
		return TIHistoricalData.Singleton.Data[state].Keys;
	}

	// Token: 0x06001688 RID: 5768 RVA: 0x00072F84 File Offset: 0x00071184
	public static ValueTuple<TIDateTime, TIDateTime> GetDateRange(TIGameState state, string attribute)
	{
		Dictionary<string, List<KeyValuePair<TIDateTime, float>>> dictionary;
		List<KeyValuePair<TIDateTime, float>> list;
		if (!TIHistoricalData.Singleton.Data.TryGetValue(state, out dictionary) || !dictionary.TryGetValue(attribute, out list) || list.Count == 0)
		{
			return new ValueTuple<TIDateTime, TIDateTime>(new TIDateTime(), new TIDateTime());
		}
		TIDateTime key = list.First<KeyValuePair<TIDateTime, float>>().Key;
		TIDateTime key2 = list.Last<KeyValuePair<TIDateTime, float>>().Key;
		return new ValueTuple<TIDateTime, TIDateTime>(key, key2);
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x00072FEC File Offset: 0x000711EC
	public static ValueTuple<float, float> GetValueRange(TIGameState state, string attribute)
	{
		if (state != TIHistoricalData.valueRangeCachedState || attribute != TIHistoricalData.valueRangeCachedAttribute || TIHistoricalData.cachedValueRangeIsTight)
		{
			List<float> list = (from x in TIHistoricalData.Singleton.Data[state][attribute]
				select x.Value into x
				orderby x
				select x).ToList<float>();
			float num = list.First<float>();
			float num2 = list.Last<float>();
			TIHistoricalData.cachedValueRange = new ValueTuple<float, float>(num, num2);
			TIHistoricalData.valueRangeCachedState = state;
			TIHistoricalData.valueRangeCachedAttribute = attribute;
			TIHistoricalData.cachedValueRangeIsTight = false;
		}
		return TIHistoricalData.cachedValueRange;
	}

	// Token: 0x0600168A RID: 5770 RVA: 0x000730B0 File Offset: 0x000712B0
	public static ValueTuple<float, float> GetValueRange_Tight(TIGameState state, string attribute)
	{
		if (state != TIHistoricalData.valueRangeCachedState || attribute != TIHistoricalData.valueRangeCachedAttribute || !TIHistoricalData.cachedValueRangeIsTight)
		{
			List<float> list = (from x in TIHistoricalData.Singleton.Data[state][attribute]
				select x.Value into x
				orderby x
				select x).ToList<float>();
			float num = list.First<float>();
			float num2 = list.Last<float>();
			List<float> list2 = (from x in TIHistoricalData.Singleton.Data[state][attribute].Take_Random<KeyValuePair<TIDateTime, float>>(100)
				select x.Value).ToList<float>();
			float mean = list2.Average();
			float num3 = Mathf.Sqrt(list2.Sum<float>((float x) => Mathf.Pow(x - mean, 2f)) / (float)(list2.Count - 1));
			TIHistoricalData.cachedValueRange = new ValueTuple<float, float>(Mathf.Max(num, mean - num3 * 3f), Mathf.Min(num2, mean + num3 * 3f));
			TIHistoricalData.valueRangeCachedState = state;
			TIHistoricalData.valueRangeCachedAttribute = attribute;
			TIHistoricalData.cachedValueRangeIsTight = true;
		}
		return TIHistoricalData.cachedValueRange;
	}

	// Token: 0x0600168B RID: 5771 RVA: 0x00073218 File Offset: 0x00071418
	public static float GetHighestValue(TIGameState state, string attribute)
	{
		Dictionary<string, List<KeyValuePair<TIDateTime, float>>> dictionary;
		List<KeyValuePair<TIDateTime, float>> list;
		if (!TIHistoricalData.Singleton.Data.TryGetValue(state, out dictionary) || !dictionary.TryGetValue(attribute, out list))
		{
			return 0f;
		}
		return list.Max<KeyValuePair<TIDateTime, float>>((KeyValuePair<TIDateTime, float> x) => x.Value);
	}

	// Token: 0x0600168C RID: 5772 RVA: 0x00073270 File Offset: 0x00071470
	public static TIDateTime GetLerpDate(TIGameState state, string attribute, float lerp)
	{
		ValueTuple<TIDateTime, TIDateTime> dateRange = TIHistoricalData.GetDateRange(state, attribute);
		TIDateTime item = dateRange.Item1;
		float num = (float)(dateRange.Item2 - item).TotalDays;
		TIDateTime tidateTime = new TIDateTime(item);
		tidateTime.AddDays(num * lerp);
		return tidateTime;
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x000732B0 File Offset: 0x000714B0
	public static void Record(TIGameState state, string attribute, Func<float> GetValue, float resolutionInDays, bool sum, bool isDebugData)
	{
		if (!GameControl.loadcycle100 || (!TIHistoricalData.RecordDebugData && isDebugData))
		{
			return;
		}
		if (!TIHistoricalData.Singleton.Data.ContainsKey(state))
		{
			TIHistoricalData.Singleton.Data[state] = new Dictionary<string, List<KeyValuePair<TIDateTime, float>>>();
		}
		if (!TIHistoricalData.Singleton.Data[state].ContainsKey(attribute))
		{
			TIHistoricalData.Singleton.Data[state][attribute] = new List<KeyValuePair<TIDateTime, float>>();
		}
		List<KeyValuePair<TIDateTime, float>> list = TIHistoricalData.Singleton.Data[state][attribute];
		if (list.Count > 0)
		{
			KeyValuePair<TIDateTime, float> keyValuePair = list.Last<KeyValuePair<TIDateTime, float>>();
			if ((TITimeState.Now() - keyValuePair.Key).TotalDays < (double)resolutionInDays)
			{
				if (sum)
				{
					list[list.Count - 1] = new KeyValuePair<TIDateTime, float>(keyValuePair.Key, keyValuePair.Value + GetValue());
					TIHistoricalData.valueRangeCachedState = null;
				}
				return;
			}
		}
		list.Add(new KeyValuePair<TIDateTime, float>(TITimeState.Now(), GetValue()));
		TIHistoricalData.valueRangeCachedState = null;
	}

	// Token: 0x0600168E RID: 5774 RVA: 0x000733C0 File Offset: 0x000715C0
	public static void Record(TIGameState state, string attribute, float value, float resolutionInDays = 0f, bool isDebugData = true)
	{
		TIHistoricalData.Record(state, attribute, () => value, resolutionInDays, false, isDebugData);
	}

	// Token: 0x0600168F RID: 5775 RVA: 0x000733F4 File Offset: 0x000715F4
	public static void Record_Sum(TIGameState state, string attribute, float value, float resolutionInDays, bool isDebugData = true)
	{
		TIHistoricalData.Record(state, attribute, () => value, resolutionInDays, true, isDebugData);
	}

	// Token: 0x06001690 RID: 5776 RVA: 0x00073428 File Offset: 0x00071628
	public static void Clear(TIGameState state, string attribute)
	{
		Dictionary<string, List<KeyValuePair<TIDateTime, float>>> dictionary;
		List<KeyValuePair<TIDateTime, float>> list;
		if (TIHistoricalData.Singleton.Data.TryGetValue(state, out dictionary) && dictionary.TryGetValue(attribute, out list))
		{
			list.Clear();
		}
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x0007345C File Offset: 0x0007165C
	public static float Sample(TIGameState state, string attribute, TIDateTime date)
	{
		Dictionary<string, List<KeyValuePair<TIDateTime, float>>> dictionary;
		List<KeyValuePair<TIDateTime, float>> list;
		if (!TIHistoricalData.Singleton.Data.TryGetValue(state, out dictionary) || !dictionary.TryGetValue(attribute, out list))
		{
			return 0f;
		}
		if (list.Count == 0)
		{
			return 0f;
		}
		if (list.Count == 1)
		{
			return list.First<KeyValuePair<TIDateTime, float>>().Value;
		}
		int num = list.BinarySearch(new KeyValuePair<TIDateTime, float>(date, 0f), new TIHistoricalData.DatumComparer());
		if (num > 0)
		{
			return list[num].Value;
		}
		int num2 = ~num - 1;
		int num3 = num2 + 1;
		if (num2 < 0)
		{
			return list[0].Value;
		}
		if (num2 == list.Count - 1)
		{
			return list[num2].Value;
		}
		TIDateTime key = list[num2].Key;
		TIDateTime key2 = list[num3].Key;
		float num4 = (float)(key - key2).Duration().TotalDays;
		return list[num2].Value * (float)(1.0 - (key - date).Duration().TotalDays / (double)num4) + list[num3].Value * (float)(1.0 - (key2 - date).Duration().TotalDays / (double)num4);
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x000735D5 File Offset: 0x000717D5
	public static float Sample(TIGameState state, string attribute, float lerp)
	{
		return TIHistoricalData.Sample(state, attribute, TIHistoricalData.GetLerpDate(state, attribute, lerp));
	}

	// Token: 0x06001693 RID: 5779 RVA: 0x000735E8 File Offset: 0x000717E8
	public static float GuessNextReading(TIGameState state, string attribute, float windowSize_days, float windowSetback_days, int sampleCount, out bool windowWasTruncated, TIHistoricalData.EstimateType estimateType = TIHistoricalData.EstimateType.Standard)
	{
		ValueTuple<TIDateTime, TIDateTime> dateRange = TIHistoricalData.GetDateRange(state, attribute);
		TIDateTime dataStartDate = dateRange.Item1;
		TIDateTime dataEndDate = dateRange.Item2;
		windowWasTruncated = (dataEndDate - dataStartDate).TotalDays < (double)windowSize_days;
		float sampleLength_days = windowSize_days / (float)sampleCount;
		IEnumerable<ValueTuple<TIDateTime, float>> enumerable = from x in Enumerable.Range(0, sampleCount).Reverse<int>().Select<int, ValueTuple<TIDateTime, float>>(delegate(int index)
			{
				TIDateTime tidateTime = TITimeState.Now();
				tidateTime.AddDays(-(sampleLength_days * (float)index + windowSetback_days));
				float num = Mathf.Pow(1f - (float)index / ((float)sampleCount - 1f), 1.5f);
				return new ValueTuple<TIDateTime, float>(tidateTime, num);
			})
			where x.Item1 >= dataStartDate && x.Item1 <= dataEndDate
			select x;
		if (!enumerable.Any<ValueTuple<TIDateTime, float>>())
		{
			return 0f;
		}
		return enumerable.Sum<ValueTuple<TIDateTime, float>>(([TupleElementNames(new string[] { "Date", "Weight" })] ValueTuple<TIDateTime, float> pair) => TIHistoricalData.Sample(state, attribute, pair.Item1) * pair.Item2) / enumerable.Sum<ValueTuple<TIDateTime, float>>(([TupleElementNames(new string[] { "Date", "Weight" })] ValueTuple<TIDateTime, float> x) => x.Item2);
	}

	// Token: 0x06001694 RID: 5780 RVA: 0x000736EC File Offset: 0x000718EC
	public static void ExportFactionCSV(HashSet<string> allowedAttributes = null)
	{
		string text = Path.Combine(CreateSaveFileScrollList.GetSaveFolderPath(), "HistoricalData.csv");
		StreamWriter streamWriter = new StreamWriter(text);
		string text2 = "faction_name,attribute,sample,value";
		streamWriter.WriteLine(text2);
		foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
		{
			foreach (string text3 in TIHistoricalData.GetAttributes(tifactionState))
			{
				if (allowedAttributes == null || allowedAttributes.Contains(text3))
				{
					int num = 0;
					for (float num2 = 0f; num2 <= 1f; num2 += 0.1f)
					{
						string text4 = string.Format("{0},{1},{2},{3}", new object[]
						{
							tifactionState.displayName,
							text3,
							num++,
							TIHistoricalData.Sample(tifactionState, text3, num2)
						});
						streamWriter.WriteLine(text4);
					}
				}
			}
		}
		streamWriter.Close();
		Log.Debug("Created historical data save named " + text, Array.Empty<object>());
	}

	// Token: 0x040014DA RID: 5338
	[fsProperty]
	private Dictionary<TIGameState, Dictionary<string, List<KeyValuePair<TIDateTime, float>>>> Data = new Dictionary<TIGameState, Dictionary<string, List<KeyValuePair<TIDateTime, float>>>>();

	// Token: 0x040014DB RID: 5339
	private static TIHistoricalData singleton;

	// Token: 0x040014DC RID: 5340
	public static bool RecordDebugData;

	// Token: 0x040014DD RID: 5341
	private static ValueTuple<float, float> cachedValueRange;

	// Token: 0x040014DE RID: 5342
	private static TIGameState valueRangeCachedState;

	// Token: 0x040014DF RID: 5343
	private static string valueRangeCachedAttribute;

	// Token: 0x040014E0 RID: 5344
	private static bool cachedValueRangeIsTight;

	// Token: 0x02000C32 RID: 3122
	private class DatumComparer : IComparer<KeyValuePair<TIDateTime, float>>
	{
		// Token: 0x06006BF2 RID: 27634 RVA: 0x00305CA4 File Offset: 0x00303EA4
		public int Compare(KeyValuePair<TIDateTime, float> x, KeyValuePair<TIDateTime, float> y)
		{
			return x.Key.CompareTo(y.Key);
		}
	}

	// Token: 0x02000C33 RID: 3123
	public enum EstimateType
	{
		// Token: 0x04004D9A RID: 19866
		Standard,
		// Token: 0x04004D9B RID: 19867
		Low,
		// Token: 0x04004D9C RID: 19868
		High
	}
}
