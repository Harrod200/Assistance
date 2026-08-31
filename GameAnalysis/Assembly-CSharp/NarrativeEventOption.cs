using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020002AA RID: 682
public struct NarrativeEventOption
{
	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000968 RID: 2408 RVA: 0x0002F261 File Offset: 0x0002D461
	public List<string> UseAIModifiers
	{
		get
		{
			if (this.useAIModifiers == null)
			{
				this.useAIModifiers = new List<string>();
			}
			return this.useAIModifiers;
		}
	}

	// Token: 0x06000969 RID: 2409 RVA: 0x0002F27C File Offset: 0x0002D47C
	public float outcomeChance(int idx, TIFactionState actingFaction, TIGameState target, TIGameState secondary)
	{
		return this.outcomes[idx].GetModifiedWeight(actingFaction, target, secondary) / this.outcomes.Sum<NarrativeEventOutcome>((NarrativeEventOutcome x) => x.GetModifiedWeight(actingFaction, target, secondary));
	}

	// Token: 0x0600096A RID: 2410 RVA: 0x0002F2E4 File Offset: 0x0002D4E4
	public List<NarrativeEventOutcome> possibleOutcomes(TIFactionState actingFaction, TIGameState target, TIGameState secondary)
	{
		return this.outcomes.Where<NarrativeEventOutcome>((NarrativeEventOutcome x) => x.GetModifiedWeight(actingFaction, target, secondary) > 0f).ToList<NarrativeEventOutcome>();
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0002F328 File Offset: 0x0002D528
	public string outcomeDescription(string eventDataName, int optionIdx, int outcomeIdx)
	{
		return Loc.T(new StringBuilder("TINarrativeEventTemplate.").Append(eventDataName).Append(".option").Append(optionIdx)
			.Append(".outcome")
			.Append(outcomeIdx)
			.ToString());
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x0002F364 File Offset: 0x0002D564
	public bool ValidOption(TIFactionState faction, TIGameState targetState, TIGameState secondary)
	{
		if (!faction.defeated && NarrativeEventOption.PassesActorCondition(faction, this.actingFactionCondition) && NarrativeEventOption.PassesTargetCondition(targetState, this.targetCondition))
		{
			List<NarrativeEventOutcome> list = this.possibleOutcomes(faction, targetState, secondary);
			if (list.Count > 0)
			{
				foreach (NarrativeEventOutcome narrativeEventOutcome in list)
				{
					if (!narrativeEventOutcome.GetCosts(targetState).CanAfford(faction, 1f, null, float.PositiveInfinity))
					{
						return false;
					}
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600096D RID: 2413 RVA: 0x0002F408 File Offset: 0x0002D608
	public bool ValidOption(TINationState nation, TIGameState targetState, TIGameState secondary)
	{
		if (!NarrativeEventOption.PassesActorCondition(null, this.actingFactionCondition) || !NarrativeEventOption.PassesTargetCondition(targetState, this.targetCondition))
		{
			return false;
		}
		List<NarrativeEventOutcome> list = this.possibleOutcomes(null, targetState, secondary);
		IEnumerable<TIResourcesCost> enumerable = list.Select<NarrativeEventOutcome, TIResourcesCost>((NarrativeEventOutcome x) => x.GetCosts(targetState));
		if (!list.All<NarrativeEventOutcome>((NarrativeEventOutcome x) => string.IsNullOrEmpty(x.projectGrantedTemplateName)))
		{
			return false;
		}
		if (enumerable != null)
		{
			return enumerable.All<TIResourcesCost>((TIResourcesCost x) => x.resourceCosts.Count == 0);
		}
		return true;
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x0002F4BC File Offset: 0x0002D6BC
	public static bool PassesActorCondition(TIFactionState actingFaction, TICondition actingFactionCondition)
	{
		if (actingFactionCondition == null)
		{
			return true;
		}
		ConditionTargetType conditionTargetType = actingFactionCondition.ConditionTarget();
		if (conditionTargetType == ConditionTargetType.global)
		{
			return actingFactionCondition.PassesCondition(null);
		}
		if (conditionTargetType == ConditionTargetType.faction)
		{
			return actingFaction != null && !actingFaction.defeated && actingFactionCondition.PassesCondition(actingFaction);
		}
		Log.Error("Bad actor condition type in narrative event option passed to " + actingFactionCondition.ToString(), Array.Empty<object>());
		return false;
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x0002F51C File Offset: 0x0002D71C
	public static bool PassesTargetCondition(TIGameState target, TICondition targetCondition)
	{
		if (targetCondition == null)
		{
			return true;
		}
		switch (targetCondition.ConditionTarget())
		{
		case ConditionTargetType.global:
			return targetCondition.PassesCondition(null);
		case ConditionTargetType.faction:
			return target.ref_faction != null && !target.ref_faction.defeated && targetCondition.PassesCondition(target.ref_faction);
		case ConditionTargetType.nation:
			return target.ref_nation != null && targetCondition.PassesCondition(target.ref_nation);
		case ConditionTargetType.region:
			return target.ref_region != null && targetCondition.PassesCondition(target.ref_region);
		case ConditionTargetType.councilor:
			return target.ref_councilor != null && targetCondition.PassesCondition(target.ref_councilor);
		case ConditionTargetType.hab:
			return target.ref_hab != null && targetCondition.PassesCondition(target.ref_hab);
		case ConditionTargetType.habSite:
			return target.ref_naturalSpaceObject != null && targetCondition.PassesCondition(target.ref_naturalSpaceObject);
		case ConditionTargetType.naturalSpaceObject:
			return target.ref_naturalSpaceObject != null && targetCondition.PassesCondition(target.ref_naturalSpaceObject);
		case ConditionTargetType.spaceBody:
			return target.ref_spaceBody != null && targetCondition.PassesCondition(target.ref_spaceBody);
		case ConditionTargetType.fleet:
			return target.ref_fleet != null && targetCondition.PassesCondition(target.ref_fleet);
		case ConditionTargetType.ship:
			return target.ref_ship != null && targetCondition.PassesCondition(target.ref_ship);
		case ConditionTargetType.officer:
			return target.ref_officer != null && targetCondition.PassesCondition(target.ref_officer);
		case ConditionTargetType.army:
			return target.ref_army != null && targetCondition.PassesCondition(target.ref_army);
		case ConditionTargetType.war:
			return target.ref_war != null && targetCondition.PassesCondition(target.ref_war);
		default:
			return false;
		}
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0002F70C File Offset: 0x0002D90C
	public StringBuilder OptionDetail(TIFactionState faction, TIGameState targetState, TIGameState secondaryState, string eventDataName, int idx, Dictionary<TIGameState, TIGameState> allTargetsandSeconds, TINarrativeEventTemplate narrativeEvent)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string text = Loc.T(new StringBuilder("TINarrativeEventTemplate.").Append(eventDataName).Append(".optionDetail").Append(idx.ToString())
			.ToString());
		if (!string.IsNullOrEmpty(text))
		{
			stringBuilder.AppendLine(text).AppendLine();
		}
		bool flag = false;
		if (this.actingFactionCondition != null)
		{
			TIFactionCondition_eIdeology tifactionCondition_eIdeology = this.actingFactionCondition as TIFactionCondition_eIdeology;
			if (tifactionCondition_eIdeology != null && !tifactionCondition_eIdeology.PassesCondition(faction))
			{
				flag = true;
			}
			stringBuilder.AppendLine(Loc.T("NarrativeEventOption.conditional", new object[] { this.actingFactionCondition.GetDescription() }));
		}
		if (flag)
		{
			stringBuilder.Append(Loc.T("NarrativeEventOption.hideOptionInfoFromFaction"));
			return stringBuilder;
		}
		if (this.targetCondition != null)
		{
			stringBuilder.AppendLine(Loc.T("NarrativeEventOption.conditional", new object[] { this.targetCondition.GetDescription() }));
		}
		int num = 1;
		List<NarrativeEventOutcome> list = this.possibleOutcomes(faction, targetState, secondaryState);
		foreach (NarrativeEventOutcome narrativeEventOutcome in list)
		{
			if (list.Count > 1)
			{
				int num2 = this.outcomes.IndexOf(narrativeEventOutcome);
				stringBuilder.AppendLine(Loc.T("NarrativeEventOption.outcome", new object[]
				{
					num.ToString("N0"),
					this.outcomeDescription(eventDataName, idx, num2),
					this.outcomeChance(num2, faction, targetState, secondaryState).ToPercent("P0")
				}));
			}
			TIResourcesCost costs = narrativeEventOutcome.GetCosts(targetState);
			if (costs.anyDebit)
			{
				stringBuilder.AppendLine(Loc.T("NarrativeEventOption.costs", new object[] { costs.GetString("Relevant", false, false, false, 7, true, false, null, false, FactionResource.None) }));
			}
			if (costs.anyCredit)
			{
				stringBuilder.AppendLine(Loc.T("NarrativeEventOption.gains", new object[] { costs.GetString("Relevant", false, false, false, 7, false, true, null, false, FactionResource.None) }));
			}
			TIOrgTemplate orgGranted = narrativeEventOutcome.orgGranted;
			if (orgGranted != null)
			{
				stringBuilder.AppendLine(Loc.T("NarrativeEventOption.org", new object[] { orgGranted.randomized ? Loc.T("TIOrgTemplate.displayName.noNameYetWithArticle") : orgGranted.displayNameWithArticle }));
			}
			TIProjectTemplate projectGranted = narrativeEventOutcome.projectGranted;
			if (projectGranted != null)
			{
				stringBuilder.AppendLine(Loc.T("NarrativeEventOption.project", new object[] { projectGranted.displayName }));
			}
			foreach (TINarrativeEventTemplate tinarrativeEventTemplate in narrativeEventOutcome.eventsToAdd)
			{
				stringBuilder.AppendLine(Loc.T("NarrativeEventOption.unlocksEvent", new object[] { tinarrativeEventTemplate.displayName }));
			}
			foreach (TINarrativeEventTemplate tinarrativeEventTemplate2 in narrativeEventOutcome.eventsToRemove)
			{
				stringBuilder.AppendLine(Loc.T("NarrativeEventOption.removesEvent", new object[] { tinarrativeEventTemplate2.displayName }));
			}
			List<TIEffectTemplate> effectTemplates = narrativeEventOutcome.effectTemplates;
			effectTemplates.AddRange(narrativeEventOutcome.delayedEffectTemplates);
			if (effectTemplates.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("NarrativeEventOption.effects"));
				foreach (TIEffectTemplate tieffectTemplate in effectTemplates)
				{
					if (allTargetsandSeconds != null && allTargetsandSeconds.Count > 1 && narrativeEvent.hitAllQualifyingTargets && narrativeEvent.firstTargetNotificationOnly)
					{
						if (allTargetsandSeconds.Count > 45)
						{
							bool flag2 = false;
							List<TIGameState> list2 = allTargetsandSeconds.Keys.ToList<TIGameState>();
							if (allTargetsandSeconds.Values.ToList<TIGameState>().All<TIGameState>((TIGameState x) => x == null))
							{
								NarrativeEventTargetType targetType = narrativeEvent.targetType;
								if (targetType != NarrativeEventTargetType.nation)
								{
									if (targetType == NarrativeEventTargetType.region)
									{
										if (list2.Intersect<TIGameState>(GameStateManager.AllRegions()).Count<TIGameState>() == list2.Count<TIGameState>())
										{
											stringBuilder.AppendLine(tieffectTemplate.allDescription(2));
											flag2 = true;
										}
									}
								}
								else if (list2.Intersect<TIGameState>(GameStateManager.AllExtantHumanNations()).Count<TIGameState>() == list2.Count<TIGameState>())
								{
									stringBuilder.AppendLine(tieffectTemplate.allDescription(0));
									flag2 = true;
								}
								else if (list2.Intersect<TIGameState>(GameStateManager.AllExtantNations()).Count<TIGameState>() == list2.Count<TIGameState>())
								{
									stringBuilder.AppendLine(tieffectTemplate.allDescription(1));
									flag2 = true;
								}
							}
							if (!flag2)
							{
								List<string> list3 = new List<string>();
								foreach (KeyValuePair<TIGameState, TIGameState> keyValuePair in allTargetsandSeconds)
								{
									list3.Add(tieffectTemplate.description(keyValuePair.Key, keyValuePair.Value).Remove(0, 1));
								}
								stringBuilder.AppendLine(TIUtilities.ConstructTextList(list3, false, false));
								continue;
							}
							continue;
						}
						else
						{
							using (Dictionary<TIGameState, TIGameState>.Enumerator enumerator4 = allTargetsandSeconds.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									KeyValuePair<TIGameState, TIGameState> keyValuePair2 = enumerator4.Current;
									stringBuilder.AppendLine(tieffectTemplate.description(keyValuePair2.Key, keyValuePair2.Value));
								}
								continue;
							}
						}
					}
					stringBuilder.AppendLine(tieffectTemplate.description(targetState, secondaryState));
				}
			}
			if (list.Count > 1 && num <= list.Count)
			{
				stringBuilder.AppendLine();
			}
			num++;
		}
		return stringBuilder;
	}

	// Token: 0x040007E1 RID: 2017
	public TICondition actingFactionCondition;

	// Token: 0x040007E2 RID: 2018
	public TICondition targetCondition;

	// Token: 0x040007E3 RID: 2019
	public List<NarrativeEventOutcome> outcomes;

	// Token: 0x040007E4 RID: 2020
	public float baseAIPreference;

	// Token: 0x040007E5 RID: 2021
	[SerializeField]
	private List<string> useAIModifiers;
}
