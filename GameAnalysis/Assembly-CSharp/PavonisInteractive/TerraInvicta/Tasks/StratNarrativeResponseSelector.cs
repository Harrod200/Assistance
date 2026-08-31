using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000952 RID: 2386
	public class StratNarrativeResponseSelector : INarrativeResponseSelectionStrategy
	{
		// Token: 0x06005AE9 RID: 23273 RVA: 0x002BBBC0 File Offset: 0x002B9DC0
		public int SelectOption(TIFactionState faction, TIGameState target, TIGameState secondary, TINarrativeEventTemplate eventTemplate)
		{
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < eventTemplate.numOptions; i++)
			{
				NarrativeEventOption narrativeEventOption = eventTemplate.eventOptions[i];
				if (narrativeEventOption.ValidOption(faction, target, secondary))
				{
					float num = narrativeEventOption.baseAIPreference;
					foreach (string text in narrativeEventOption.UseAIModifiers)
					{
						num *= faction.aiValues.AIValueFromString(text);
					}
					List<NarrativeEventOutcome> list = narrativeEventOption.possibleOutcomes(faction, target, secondary);
					if (list.Count > 1)
					{
						if (list.Any<NarrativeEventOutcome>((NarrativeEventOutcome x) => x.AIFavored))
						{
							float num2 = 0f;
							float num3 = 0f;
							foreach (NarrativeEventOutcome narrativeEventOutcome in list)
							{
								float modifiedWeight = narrativeEventOutcome.GetModifiedWeight(faction, target, secondary);
								num3 += modifiedWeight;
								if (narrativeEventOutcome.AIFavored)
								{
									num2 += modifiedWeight;
								}
							}
							num *= num2 / num3;
						}
					}
					dictionary.Add(i, num);
				}
			}
			if (dictionary.Count == 0)
			{
				dictionary.Add(0, 1f);
				Log.Error("No valid options found for eventTemplate " + eventTemplate.dataName, Array.Empty<object>());
			}
			else if (dictionary.Values.All<float>((float x) => x <= 0f))
			{
				dictionary[0] = 1f;
				if (dictionary.Count > 1)
				{
					Log.Warn(string.Concat(new string[]
					{
						"No valid option weights found for multiple options with eventTemplate ",
						eventTemplate.dataName,
						" ",
						faction.displayName,
						" ",
						target.displayName,
						" ",
						(secondary != null) ? secondary.displayName : null
					}), Array.Empty<object>());
				}
			}
			return dictionary.SelectRandomWeightedItem<KeyValuePair<int, float>>((KeyValuePair<int, float> o) => o.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x06005AEA RID: 23274 RVA: 0x002BBE2C File Offset: 0x002BA02C
		public int SelectOption(TINationState nation, TIGameState target, TIGameState secondary, TINarrativeEventTemplate eventTemplate)
		{
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < eventTemplate.numOptions; i++)
			{
				NarrativeEventOption narrativeEventOption = eventTemplate.eventOptions[i];
				if (narrativeEventOption.ValidOption(nation, target, secondary))
				{
					float baseAIPreference = narrativeEventOption.baseAIPreference;
					dictionary.Add(i, baseAIPreference);
				}
			}
			if (dictionary.Count == 0)
			{
				dictionary.Add(0, 1f);
				Log.Warn("No valid options found for eventTemplate " + eventTemplate.dataName, Array.Empty<object>());
			}
			else if (dictionary.Values.All<float>((float x) => x <= 0f))
			{
				dictionary[0] = 1f;
				Log.Warn("No valid option weights found for eventTemplate " + eventTemplate.dataName, Array.Empty<object>());
			}
			return dictionary.SelectRandomWeightedItem<KeyValuePair<int, float>>((KeyValuePair<int, float> o) => o.Value, -1f, 1E-37f).Key;
		}
	}
}
