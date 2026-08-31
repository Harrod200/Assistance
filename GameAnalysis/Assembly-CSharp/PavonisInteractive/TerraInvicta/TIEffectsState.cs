using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.Tasks;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200077C RID: 1916
	public class TIEffectsState : TIGameState
	{
		// Token: 0x06003B78 RID: 15224 RVA: 0x0015EA70 File Offset: 0x0015CC70
		public override bool Initialize()
		{
			this.factionEffectsNames = new Dictionary<TIFactionState, Dictionary<Context, List<string>>>();
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				this.factionEffectsNames.Add(tifactionState, new Dictionary<Context, List<string>>());
			}
			this.factionEffects = new Dictionary<TIFactionState, Dictionary<Context, List<TIEffectTemplate>>>();
			foreach (TIFactionState tifactionState2 in GameStateManager.AllFactions())
			{
				this.factionEffects.Add(tifactionState2, new Dictionary<Context, List<TIEffectTemplate>>());
			}
			return base.Initialize();
		}

		// Token: 0x06003B79 RID: 15225 RVA: 0x0015EAEC File Offset: 0x0015CCEC
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this.factionEffects == null)
			{
				this.factionEffects = new Dictionary<TIFactionState, Dictionary<Context, List<TIEffectTemplate>>>();
				foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
				{
					this.factionEffects.Add(tifactionState, new Dictionary<Context, List<TIEffectTemplate>>());
				}
			}
			if (this.factionEffectExpirations == null)
			{
				this.factionEffectExpirations = GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, Dictionary<string, TIDateTime>>((TIFactionState x) => x, (TIFactionState x) => new Dictionary<string, TIDateTime>());
			}
			if (!this.gameStateSubjectCreated)
			{
				using (IEnumerator<TIEffectTemplate> enumerator = TemplateManager.IterateByClass<TIEffectTemplate>(true).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIEffectTemplate tieffectTemplate = enumerator.Current;
						foreach (TIFactionState tifactionState2 in tieffectTemplate.InitialFactions)
						{
							this.AddEffectToFaction(tifactionState2, tieffectTemplate);
						}
					}
					goto IL_023B;
				}
			}
			foreach (TIFactionState tifactionState3 in this.factionEffectsNames.Keys)
			{
				foreach (Context context in this.factionEffectsNames[tifactionState3].Keys)
				{
					if (!this.factionEffects[tifactionState3].Keys.Contains(context))
					{
						this.factionEffects[tifactionState3].Add(context, new List<TIEffectTemplate>());
					}
					foreach (string text in this.factionEffectsNames[tifactionState3][context])
					{
						TIEffectTemplate tieffectTemplate2 = TemplateManager.Find<TIEffectTemplate>(text, false);
						if (tieffectTemplate2 != null)
						{
							this.factionEffects[tifactionState3][context].Add(tieffectTemplate2);
						}
						else
						{
							Log.Error("Bad effectName " + text + " stored in factionEffectsNames", Array.Empty<object>());
						}
					}
				}
			}
			IL_023B:
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x06003B7A RID: 15226 RVA: 0x0015EDB8 File Offset: 0x0015CFB8
		private void AddEffectToFaction(TIFactionState faction, TIEffectTemplate effectTemplate)
		{
			foreach (Context context in effectTemplate.GetContexts())
			{
				if (!this.factionEffects[faction].Keys.Contains(context))
				{
					this.factionEffectsNames[faction].Add(context, new List<string>());
					this.factionEffects[faction].Add(context, new List<TIEffectTemplate>());
				}
				if (effectTemplate.stackable || !this.factionEffects[faction][context].Contains(effectTemplate))
				{
					this.factionEffectsNames[faction][context].Add(effectTemplate.dataName);
					this.factionEffects[faction][context].Add(effectTemplate);
					this.factionEffects[faction][context].OrderBy<TIEffectTemplate, int>((TIEffectTemplate x) => (int)x.operation).ToList<TIEffectTemplate>();
					if (effectTemplate.effectDuration == EffectDuration.temporary)
					{
						if (effectTemplate.duration_months <= 0f)
						{
							effectTemplate.duration_months = 1f;
						}
						if (effectTemplate.stackable && this.factionEffectExpirations[faction].ContainsKey(effectTemplate.dataName))
						{
							this.factionEffectExpirations[faction][effectTemplate.dataName].AddHours((double)(effectTemplate.duration_months * 30.436874f * 24f));
						}
						else
						{
							TIDateTime tidateTime = TITimeState.Now();
							tidateTime.AddHours((double)(effectTemplate.duration_months * 30.436874f * 24f));
							if (this.factionEffectExpirations[faction].ContainsKey(effectTemplate.dataName))
							{
								this.factionEffectExpirations[faction][effectTemplate.dataName] = tidateTime;
							}
							else
							{
								this.factionEffectExpirations[faction].Add(effectTemplate.dataName, tidateTime);
							}
						}
					}
				}
				this.ProcessContextUpdate(context, faction);
			}
		}

		// Token: 0x06003B7B RID: 15227 RVA: 0x0015EFDC File Offset: 0x0015D1DC
		public void GlobalCheckForRemoveEffects()
		{
			Dictionary<TIFactionState, List<string>> dictionary = new Dictionary<TIFactionState, List<string>>();
			foreach (TIFactionState tifactionState in this.factionEffectExpirations.Keys)
			{
				dictionary[tifactionState] = new List<string>();
				foreach (string text in this.factionEffectExpirations[tifactionState].Keys)
				{
					if (TITimeState.Now() >= this.factionEffectExpirations[tifactionState][text])
					{
						dictionary[tifactionState].Add(text);
					}
				}
			}
			foreach (TIFactionState tifactionState2 in dictionary.Keys)
			{
				foreach (string text2 in dictionary[tifactionState2])
				{
					TIEffectTemplate tieffectTemplate = TemplateManager.Find<TIEffectTemplate>(text2, false);
					this.RemoveEffect(tieffectTemplate, tifactionState2);
					this.factionEffectExpirations[tifactionState2].Remove(text2);
				}
			}
		}

		// Token: 0x06003B7C RID: 15228 RVA: 0x0015F15C File Offset: 0x0015D35C
		private void RemoveEffect(TIEffectTemplate effectTemplate, TIGameState source)
		{
			TIFactionState tifactionState = source as TIFactionState;
			if (tifactionState != null)
			{
				foreach (TIFactionState tifactionState2 in TIEffectsState.GetAffectedFactions(effectTemplate, tifactionState))
				{
					foreach (Context context in effectTemplate.GetContexts())
					{
						if (this.factionEffectsNames[tifactionState2].ContainsKey(context))
						{
							this.factionEffectsNames[tifactionState2][context].Remove(effectTemplate.dataName);
							this.factionEffects[tifactionState2][context].Remove(effectTemplate);
							this.ProcessContextUpdate(context, tifactionState2);
						}
					}
				}
			}
		}

		// Token: 0x06003B7D RID: 15229 RVA: 0x0015F258 File Offset: 0x0015D458
		private void ProcessContextUpdate(Context testContext, TIFactionState faction)
		{
			if (testContext <= Context.Environment_BestSustainabilityValue)
			{
				if (testContext == Context.DetectAlienSpaceAssetsRange)
				{
					faction.fullSpaceVisibility = faction.FullSystemVisibility;
					return;
				}
				if (testContext != Context.Environment_BestSustainabilityValue)
				{
					return;
				}
			}
			else
			{
				switch (testContext)
				{
				case Context.LaserDefenseType:
				case Context.LaserDefenseFreq:
				{
					using (IEnumerator<TISpaceDefensesFacilityState> enumerator = GameStateManager.IterateByClass<TISpaceDefensesFacilityState>(false).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TISpaceDefensesFacilityState tispaceDefensesFacilityState = enumerator.Current;
							if (tispaceDefensesFacilityState.Extant())
							{
								tispaceDefensesFacilityState.SetLaserDefenseWeaponTemplate();
							}
						}
						return;
					}
					break;
				}
				case Context.MCFreeSpaceMineNetwork:
				case Context.HabMissionControlReduction:
				case Context.ShipMissionControlReduction:
				case Context.MissionControlDisruption_PCT:
					faction.SetMissionControlUsageDataDirty();
					faction.SetResourceIncomeDataDirty(FactionResource.MissionControl);
					return;
				case Context.AlienHateFromMCUsage:
					return;
				default:
					switch (testContext)
					{
					case Context.HabResearchProduction:
						faction.SetResourceIncomeDataDirty(FactionResource.Research);
						return;
					case Context.HabNuclearFreighters:
					case Context.BombardmentHabDefenseBonus:
					case Context.CanAmassAntimatter:
					case Context.CanAmassExotics:
					case Context.ResourceMarketSales:
					case Context.ShipConstructionTime:
					case Context.ShipOfficerPromotion:
						return;
					case Context.SpaceMiningBonus:
					case Context.MiningWaterBonus:
					case Context.MiningVolatilesBonus:
					case Context.MiningMetalsBonus:
					case Context.MiningNoblesBonus:
					case Context.MiningFissilesBonus:
						break;
					case Context.Ship_MaxSurvivableCombatAcceleration_Bonus:
					case Context.Ship_MaxSurvivableCruiseAcceleration_Bonus:
						goto IL_0116;
					default:
						if (testContext != Context.MiningOutputPerformanceRecord)
						{
							return;
						}
						break;
					}
					faction.habs.ForEach(delegate(TIHabState x)
					{
						x.UpdateCurrentAnnualNetResourceIncomes(false);
					});
					return;
				}
				IL_0116:
				using (List<TISpaceShipState>.Enumerator enumerator2 = faction.ships.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TISpaceShipState tispaceShipState = enumerator2.Current;
						tispaceShipState.UpdatePropulsionValues(false);
					}
					return;
				}
			}
			TINationState.BestCurrentSustainabilityValue(true);
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				tinationState.PossiblePriorityValidationChange(false);
			}
		}

		// Token: 0x06003B7E RID: 15230 RVA: 0x0015F40C File Offset: 0x0015D60C
		public static List<TIFactionState> GetAffectedFactions(TIEffectTemplate effectTemplate, TIFactionState sourceFaction)
		{
			List<TIFactionState> list = new List<TIFactionState>();
			if (effectTemplate.effectTarget == EffectTargetType.SourceFaction)
			{
				list.Add(sourceFaction);
				return list;
			}
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			if (effectTemplate.effectTarget == EffectTargetType.AlienFaction)
			{
				if (tifactionState != null)
				{
					list.Add(tifactionState);
				}
				return list;
			}
			foreach (TIFactionState tifactionState2 in GameStateManager.Effects().factionEffects.Keys)
			{
				if (((effectTemplate.effectTarget != EffectTargetType.AllHumanFactions && effectTemplate.effectTarget != EffectTargetType.AllHumanFactionsExceptSource) || !(tifactionState2 == tifactionState)) && ((effectTemplate.effectTarget != EffectTargetType.AllFactionsExceptSource && effectTemplate.effectTarget != EffectTargetType.AllHumanFactionsExceptSource) || !(tifactionState2 == sourceFaction)))
				{
					list.Add(tifactionState2);
				}
			}
			return list;
		}

		// Token: 0x06003B7F RID: 15231 RVA: 0x0015F4D8 File Offset: 0x0015D6D8
		public static void AddEffect(TIEffectTemplate effectTemplate, TIFactionState sourceFaction, TIGameState inputEffectTarget = null, TIGameState inputEffectSecondaryTarget = null, string triggeringTemplateDataName = "")
		{
			if (effectTemplate.effectDuration != EffectDuration.instant)
			{
				TIEffectsState tieffectsState = GameStateManager.FindGameState<TIEffectsState>();
				using (List<TIFactionState>.Enumerator enumerator = TIEffectsState.GetAffectedFactions(effectTemplate, sourceFaction).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIFactionState tifactionState = enumerator.Current;
						tieffectsState.AddEffectToFaction(tifactionState, effectTemplate);
					}
					return;
				}
			}
			TIEffectsState.ProcessInstantEffect(sourceFaction, effectTemplate.effectTarget, effectTemplate.effectSecondaryTarget, effectTemplate.instantEffect, effectTemplate.value, effectTemplate.instantRnd, effectTemplate.strValue, inputEffectTarget, inputEffectSecondaryTarget, triggeringTemplateDataName);
		}

		// Token: 0x06003B80 RID: 15232 RVA: 0x0015F56C File Offset: 0x0015D76C
		public static bool CheckForEffectInContext(Context context, TIGameState gameState, TIEffectTemplate effectTemplate)
		{
			if (context == Context.None || gameState == null)
			{
				return false;
			}
			TIFactionState ref_faction = gameState.ref_faction;
			List<TIEffectTemplate> list;
			return !(ref_faction == null) && GameStateManager.Effects().factionEffects[ref_faction].TryGetValue(context, out list) && list.Any<TIEffectTemplate>((TIEffectTemplate x) => x != null && x.referenceName == effectTemplate.referenceName);
		}

		// Token: 0x06003B81 RID: 15233 RVA: 0x0015F5D4 File Offset: 0x0015D7D4
		public static bool CheckForEffectInAnyContext(TIGameState gameState, TIEffectTemplate effectTemplate)
		{
			if (gameState == null)
			{
				return false;
			}
			if (gameState.ref_faction == null)
			{
				return false;
			}
			using (List<Context>.Enumerator enumerator = effectTemplate.GetContexts().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (TIEffectsState.CheckForEffectInContext(enumerator.Current, gameState, effectTemplate))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003B82 RID: 15234 RVA: 0x0015F64C File Offset: 0x0015D84C
		public static bool CheckForAnyEffectInContext(Context context, TIGameState gameState)
		{
			if (context == Context.None || gameState == null)
			{
				return false;
			}
			TIFactionState ref_faction = gameState.ref_faction;
			if (ref_faction == null)
			{
				return false;
			}
			TIEffectsState tieffectsState = GameStateManager.Effects();
			return tieffectsState.factionEffects[ref_faction].ContainsKey(context) && tieffectsState.factionEffects[ref_faction][context].Count > 0;
		}

		// Token: 0x06003B83 RID: 15235 RVA: 0x0015F6B0 File Offset: 0x0015D8B0
		public static List<TIEffectTemplate> GetFactionEffectsForContext(Context context, TIFactionState faction)
		{
			TIEffectsState tieffectsState = GameStateManager.Effects();
			if (tieffectsState.factionEffects[faction].ContainsKey(context))
			{
				return tieffectsState.factionEffects[faction][context].ToList<TIEffectTemplate>();
			}
			return new List<TIEffectTemplate>();
		}

		// Token: 0x06003B84 RID: 15236 RVA: 0x0015F6F4 File Offset: 0x0015D8F4
		public static float SumEffectsModifiers(Context context, TIGameState sourceState, float baseValue, string strFilter = null)
		{
			TIFactionState tifactionState = ((sourceState != null) ? sourceState.ref_faction : null);
			if (tifactionState == null)
			{
				return 0f;
			}
			float num = baseValue;
			TIEffectsState tieffectsState = GameStateManager.Effects();
			if (tieffectsState == null)
			{
				return 0f;
			}
			if (tieffectsState.factionEffects[tifactionState].ContainsKey(context))
			{
				foreach (TIEffectTemplate tieffectTemplate in tieffectsState.factionEffects[tifactionState][context])
				{
					if (string.IsNullOrEmpty(tieffectTemplate.strValue) || tieffectTemplate.strValue == strFilter)
					{
						switch (tieffectTemplate.operation)
						{
						case StatModSetOperation.SetToFixedValue:
							num = tieffectTemplate.value;
							break;
						case StatModSetOperation.IncreaseToValue:
							if (num < tieffectTemplate.value)
							{
								num = tieffectTemplate.value;
							}
							break;
						case StatModSetOperation.DecreaseToValue:
							if (num > tieffectTemplate.value)
							{
								num = tieffectTemplate.value;
							}
							break;
						case StatModSetOperation.Additive:
							num += tieffectTemplate.value;
							break;
						case StatModSetOperation.Multiplicative:
							num *= tieffectTemplate.value;
							break;
						}
					}
				}
			}
			return num - baseValue;
		}

		// Token: 0x06003B85 RID: 15237 RVA: 0x0015F834 File Offset: 0x0015DA34
		public static List<TIGameState> InstantEffectTargetToGameStates(TIGameState sourceState, EffectTargetType effectTarget, TIGameState inputState = null)
		{
			List<TIGameState> list = new List<TIGameState>();
			switch (effectTarget)
			{
			case EffectTargetType.SourceFaction:
				if (sourceState != null && sourceState.ref_faction != null)
				{
					list.Add(sourceState.ref_faction);
				}
				break;
			case EffectTargetType.AlienFaction:
				list.Add(GameStateManager.AlienFaction());
				break;
			case EffectTargetType.AllFactions:
				list.AddRange(GameStateManager.AllFactions());
				break;
			case EffectTargetType.AllHumanFactions:
				list.AddRange(from faction in GameStateManager.AllFactions()
					where !faction.IsAlienFaction
					select faction);
				break;
			case EffectTargetType.AllFactionsExceptSource:
				list.AddRange(from faction in GameStateManager.AllFactions()
					where faction != sourceState.ref_faction
					select faction);
				break;
			case EffectTargetType.AllHumanFactionsExceptSource:
				list.AddRange(from faction in GameStateManager.AllFactions()
					where !faction.IsAlienFaction && faction != sourceState.ref_faction
					select faction);
				break;
			case EffectTargetType.GlobalEffectSet:
				list.Add(TIGlobalValuesState.GlobalValues);
				break;
			case EffectTargetType.AllNations:
				list.AddRange(GameStateManager.AllNations().ToList<TINationState>());
				break;
			case EffectTargetType.AllExtantNations:
				list.AddRange(GameStateManager.AllExtantNations().ToList<TINationState>());
				break;
			case EffectTargetType.AllExtantHumanNations:
				list.AddRange(GameStateManager.AllExtantHumanNations().ToList<TINationState>());
				break;
			case EffectTargetType.AllSourceFactionNations:
				if (sourceState != null && sourceState.ref_faction != null)
				{
					list.AddRange((from nation in GameStateManager.AllExtantNations()
						where nation.ref_faction == sourceState.ref_faction
						select nation).ToList<TINationState>());
				}
				break;
			case EffectTargetType.AllSourceFactionHabs:
				if (sourceState != null && sourceState.ref_faction != null)
				{
					list.AddRange(sourceState.ref_faction.habs.ToList<TIHabState>());
				}
				break;
			case EffectTargetType.AllRegions:
				list.AddRange(GameStateManager.AllRegions().ToList<TIRegionState>());
				break;
			case EffectTargetType.InputState:
				list.Add(inputState);
				break;
			}
			return list;
		}

		// Token: 0x06003B86 RID: 15238 RVA: 0x0015FA58 File Offset: 0x0015DC58
		public static List<TIGameState> GetEffectSecondaryStateCandidates(TIGameState primaryState, EffectSecondaryStateType targetType, TIGameState secondaryInputState = null, TINarrativeEventTemplate narrativeEvent = null)
		{
			List<TIGameState> list = new List<TIGameState>();
			switch (targetType)
			{
			case EffectSecondaryStateType.none:
				return null;
			case EffectSecondaryStateType.InputState:
				list.Add(secondaryInputState);
				return list;
			case EffectSecondaryStateType.Nation_FederationMember:
				list.AddRange(primaryState.ref_nation.federation.members);
				return list;
			case EffectSecondaryStateType.Nation_FederationLeader:
				list.Add(primaryState.ref_nation.federation.leadNation);
				return list;
			case EffectSecondaryStateType.Nation_Ally:
				list.AddRange(primaryState.ref_nation.allies);
				return list;
			case EffectSecondaryStateType.Nation_NonAlly:
				list.AddRange(GameStateManager.AllExtantNations().Except<TINationState>(primaryState.ref_nation.allies));
				return list;
			case EffectSecondaryStateType.Nation_LargerAlly:
				list.AddRange(primaryState.ref_nation.allies.Where<TINationState>((TINationState x) => x.numControlPoints > primaryState.ref_nation.numControlPoints));
				return list;
			case EffectSecondaryStateType.Nation_Ally_HigherMiltech:
				list.AddRange(primaryState.ref_nation.allies.Where<TINationState>((TINationState x) => x.militaryTechLevel > primaryState.ref_nation.militaryTechLevel));
				return list;
			case EffectSecondaryStateType.Nation_AllyOfCouncilorHomeNation:
				list.AddRange(primaryState.ref_councilor.homeNation.allies);
				return list;
			case EffectSecondaryStateType.Nation_NormalRelations:
				list.AddRange(GameStateManager.AllExtantNations().Except<TINationState>(primaryState.ref_nation.allies).Except<TINationState>(primaryState.ref_nation.rivals)
					.Except<TINationState>(primaryState.ref_nation.wars));
				return list;
			case EffectSecondaryStateType.Nation_AllyOrRival:
				list.AddRange(from x in primaryState.ref_nation.allies.Union<TINationState>(primaryState.ref_nation.rivals)
					where x.extant
					select x);
				return list;
			case EffectSecondaryStateType.Nation_Rival:
				list.AddRange(primaryState.ref_nation.rivals.Where<TINationState>((TINationState x) => x.extant));
				return list;
			case EffectSecondaryStateType.Nation_Rival_NMF:
				list.AddRange(primaryState.ref_nation.rivals.Where<TINationState>((TINationState x) => x.extant && (x.executiveFaction == null || x.executiveFaction != primaryState.ref_nation.executiveFaction)));
				return list;
			case EffectSecondaryStateType.Nation_NonRival:
				list.AddRange(GameStateManager.AllExtantNations().Except<TINationState>(primaryState.ref_nation.rivals).Except<TINationState>(primaryState.ref_nation.wars));
				return list;
			case EffectSecondaryStateType.Nation_SmallerRival_NMF:
				list.AddRange(primaryState.ref_nation.rivals.Where<TINationState>((TINationState x) => x.extant && x.numControlPoints < primaryState.ref_nation.numControlPoints && (x.executiveFaction == null || x.executiveFaction != primaryState.ref_nation.executiveFaction)));
				return list;
			case EffectSecondaryStateType.Nation_EqualRival_NMF:
				list.AddRange(primaryState.ref_nation.rivals.Where<TINationState>((TINationState x) => x.extant && x.numControlPoints == primaryState.ref_nation.numControlPoints && (x.executiveFaction == null || x.executiveFaction != primaryState.ref_nation.executiveFaction)));
				return list;
			case EffectSecondaryStateType.Nation_LargerRival_NMF:
				list.AddRange(primaryState.ref_nation.rivals.Where<TINationState>((TINationState x) => x.extant && x.numControlPoints > primaryState.ref_nation.numControlPoints && (x.executiveFaction == null || x.executiveFaction != primaryState.ref_nation.executiveFaction)));
				return list;
			case EffectSecondaryStateType.Nation_Rival_Neighbor:
				list.AddRange(primaryState.ref_nation.rivals.Intersect<TINationState>(primaryState.ref_nation.AdjacentNations(true)));
				return list;
			case EffectSecondaryStateType.Nation_Rival_Neighbor_NMF:
			{
				list.AddRange(primaryState.ref_nation.rivals.Where<TINationState>((TINationState x) => x.executiveFaction == null || x.executiveFaction != primaryState.ref_nation.executiveFaction).Intersect<TINationState>(primaryState.ref_nation.AdjacentNations(true)));
				TIFactionState executiveFaction = primaryState.ref_nation.executiveFaction;
				if (executiveFaction != null && executiveFaction.IsAlienProxy)
				{
					list.Remove(GameStateManager.AlienNation());
					return list;
				}
				return list;
			}
			case EffectSecondaryStateType.Nation_Rival_Accessible:
				list.AddRange(primaryState.ref_nation.rivals.Where<TINationState>((TINationState x) => primaryState.ref_nation.AccessibleWarEnemy(x, true)));
				return list;
			case EffectSecondaryStateType.Nation_RivalOfCouncilorHomeNation:
				list.AddRange(primaryState.ref_councilor.homeNation.rivals.Where<TINationState>((TINationState x) => x.extant));
				return list;
			case EffectSecondaryStateType.Nation_WarEnemy:
				list.AddRange(primaryState.ref_nation.wars.Where<TINationState>((TINationState x) => x.extant));
				return list;
			case EffectSecondaryStateType.Nation_WarEnemy_Accessible:
				list.AddRange(primaryState.ref_nation.wars.Where<TINationState>((TINationState x) => primaryState.ref_nation.AccessibleWarEnemy(x, false)));
				return list;
			case EffectSecondaryStateType.Nation_OffensiveWarEnemy_Accessible:
				list.AddRange(from x in primaryState.ref_nation.offensiveWarStates.SelectMany<TIWarState, TINationState>((TIWarState x) => x.defendingAlliance)
					where primaryState.ref_nation.AccessibleWarEnemy(x, false)
					select x);
				return list;
			case EffectSecondaryStateType.Nation_OffensiveWarEnemy_AtrocityVictim:
			{
				Func<TIRegionState, bool> <>9__47;
				list.AddRange(primaryState.ref_nation.offensiveWarStates.SelectMany<TIWarState, TINationState>((TIWarState x) => x.defendingAlliance).Where<TINationState>(delegate(TINationState x)
				{
					if (!primaryState.ref_nation.armies.Any<TIArmyState>((TIArmyState y) => y.currentNation == x))
					{
						IEnumerable<TIRegionState> regions = x.regions;
						Func<TIRegionState, bool> func;
						if ((func = <>9__47) == null)
						{
							func = (<>9__47 = (TIRegionState z) => z.leadOccupier == primaryState.ref_nation);
						}
						return regions.Any<TIRegionState>(func);
					}
					return true;
				}));
				return list;
			}
			case EffectSecondaryStateType.Nation_Neighbor:
				list.AddRange(primaryState.ref_nation.AdjacentNations(false));
				return list;
			case EffectSecondaryStateType.Nation_OpenControlPoint:
				list.AddRange(from x in GameStateManager.AllExtantHumanNations()
					where x.NumNativeControlPoints > 0
					select x);
				return list;
			case EffectSecondaryStateType.Nation_OpenControlPoint_NonExec:
				list.AddRange(from x in GameStateManager.AllExtantHumanNations()
					where x.NumNativeControlPoints > 1
					select x);
				return list;
			case EffectSecondaryStateType.Nation_NMF:
				list.AddRange(from x in GameStateManager.AllExtantHumanNations()
					where x.executiveFaction == null || x.executiveFaction != primaryState.ref_faction
					select x);
				return list;
			case EffectSecondaryStateType.Nation_MyFaction:
				list.AddRange(from x in GameStateManager.AllExtantHumanNations()
					where x.executiveFaction == primaryState.ref_faction
					select x);
				return list;
			case EffectSecondaryStateType.Nation_AlienNation:
				if (GameStateManager.AlienNation().extant)
				{
					list.Add(GameStateManager.AlienNation());
					return list;
				}
				return list;
			case EffectSecondaryStateType.Region_CouncilorHomeRegion:
				list.Add(primaryState.ref_councilor.homeRegion);
				return list;
			case EffectSecondaryStateType.Region_Neighbor:
				list.AddRange(primaryState.ref_region.AdjacentRegions(true));
				return list;
			case EffectSecondaryStateType.Region_Neighbor_Rival:
				list.AddRange(from x in primaryState.ref_region.AdjacentRegions(true)
					where primaryState.ref_nation.rivals.Contains(x.ref_nation)
					select x);
				return list;
			case EffectSecondaryStateType.Region_Neighbor_WarEnemy:
				list.AddRange(from x in primaryState.ref_region.AdjacentRegions(true)
					where primaryState.ref_nation.wars.Contains(x.ref_nation)
					select x);
				return list;
			case EffectSecondaryStateType.Region_Claimed:
				list.AddRange(primaryState.ref_nation.claims.Where<TIRegionState>((TIRegionState x) => x.nation != primaryState.ref_nation));
				return list;
			case EffectSecondaryStateType.Region_Claimed_Rival:
				list.AddRange(primaryState.ref_nation.claims.Where<TIRegionState>((TIRegionState x) => x.nation != primaryState.ref_nation && x.nation.IsRivalWith(primaryState.ref_nation)));
				return list;
			case EffectSecondaryStateType.Region_Claimed_Neighbor:
				list.AddRange(primaryState.ref_nation.claims.Where<TIRegionState>((TIRegionState x) => x.nation != primaryState.ref_nation).Intersect<TIRegionState>(primaryState.ref_region.AdjacentRegions(true)));
				return list;
			case EffectSecondaryStateType.Region_ArmyHome:
				list.Add(primaryState.ref_army.homeRegion);
				return list;
			case EffectSecondaryStateType.Faction_AlienFaction:
				list.Add(GameStateManager.AlienFaction());
				return list;
			case EffectSecondaryStateType.Faction_AlienProxy:
				list.Add(GameStateManager.AlienProxy());
				return list;
			case EffectSecondaryStateType.Faction_AlienAppeaser:
				list.Add(GameStateManager.AlienAppeaser());
				return list;
			case EffectSecondaryStateType.Faction_NonExecFaction:
				list.AddRange(from x in GameStateManager.AllHumanFactions()
					where primaryState.ref_nation.executiveFaction != x
					select x);
				return list;
			case EffectSecondaryStateType.Faction_AnyHuman:
				list.AddRange(GameStateManager.AllHumanFactions());
				return list;
			case EffectSecondaryStateType.Faction_MostPopularInNation:
				list.Add(TIFactionIdeologyTemplate.GetFactionByIdeologyTemplate(primaryState.ref_nation.GetMostPopularIdeology(false)));
				return list;
			case EffectSecondaryStateType.Faction_MostPopularOnEarth:
				return list;
			case EffectSecondaryStateType.Faction_NMF_CloseIdeology:
				list.AddRange(from x in GameStateManager.AllHumanFactions()
					where x.ref_faction != primaryState.ref_faction && TINationState.GetIdeologicalDistance(primaryState.ref_faction.ideology, x.ref_faction.ideology) < 2f
					select x);
				return list;
			case EffectSecondaryStateType.Councilor_MyFaction:
				list.AddRange(primaryState.ref_councilor.ref_faction.councilors);
				return list;
			case EffectSecondaryStateType.Councilor_MyFaction_OpposedWealth:
				list.AddRange(primaryState.ref_councilor.ref_faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.GetTraitGrouping(1) != primaryState.ref_councilor.GetTraitGrouping(1)));
				return list;
			case EffectSecondaryStateType.Councilor_MyFaction_OpposedScience:
				list.AddRange(primaryState.ref_councilor.ref_faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.GetTraitGrouping(2) != primaryState.ref_councilor.GetTraitGrouping(2)));
				return list;
			case EffectSecondaryStateType.Councilor_MyFaction_OpposedGovStatus:
				list.AddRange(primaryState.ref_councilor.ref_faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.GetTraitGrouping(4) != primaryState.ref_councilor.GetTraitGrouping(4)));
				return list;
			case EffectSecondaryStateType.Councilor_MyFaction_OpposedPersonality:
				list.AddRange(primaryState.ref_councilor.ref_faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.GetTraitGrouping(5) != primaryState.ref_councilor.GetTraitGrouping(5)));
				return list;
			case EffectSecondaryStateType.Councilor_MyFaction_OpposedLearner:
				list.AddRange(primaryState.ref_councilor.ref_faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.GetTraitGrouping(10) != primaryState.ref_councilor.GetTraitGrouping(10)));
				return list;
			case EffectSecondaryStateType.Councilor_SharedHomeRegion:
				list.AddRange(from x in GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors)
					where x.homeRegion == primaryState.ref_councilor.homeRegion
					select x);
				return list;
			case EffectSecondaryStateType.Councilor_MyFactionOrCloseIdeology:
				list.AddRange(from x in GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors)
					where x.faction == primaryState.ref_faction || TINationState.GetIdeologicalDistance(primaryState.ref_faction.ideology, x.ref_faction.ideology) < 2f
					select x);
				return list;
			case EffectSecondaryStateType.Councilor_NMF_CloseIdeology:
				list.AddRange(from x in GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors)
					where x.faction != primaryState.ref_faction && TINationState.GetIdeologicalDistance(primaryState.ref_faction.ideology, x.ref_faction.ideology) < 2f
					select x);
				return list;
			case EffectSecondaryStateType.Councilor_MyFaction_AllyHomeNation:
				list.AddRange(primaryState.ref_councilor.ref_faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.homeNation.allies.Contains(primaryState.ref_councilor.homeNation)));
				return list;
			case EffectSecondaryStateType.Councilor_MyFaction_RivalHomeNation:
				list.AddRange(primaryState.ref_councilor.ref_faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.homeNation.enemies.Contains(primaryState.ref_councilor.homeNation) && x.homeNation.CanEndRivalry(primaryState.ref_councilor.homeNation)));
				return list;
			case EffectSecondaryStateType.Orbit_ThisSpaceObject:
				list.AddRange(primaryState.ref_naturalSpaceObject.orbits);
				return list;
			case EffectSecondaryStateType.Hab_Any:
				list.AddRange(GameStateManager.IterateByClass<TIHabState>(false));
				return list;
			case EffectSecondaryStateType.Hab_AnyHuman:
				list.AddRange(from x in GameStateManager.IterateByClass<TIHabState>(false)
					where !x.IsAlien()
					select x);
				return list;
			case EffectSecondaryStateType.Hab_InOrbit:
				list.AddRange(primaryState.ref_naturalSpaceObject.stationsInOrbit);
				return list;
			case EffectSecondaryStateType.Hab_CouncilorOnBoard:
				if (primaryState.ref_councilor.ref_hab != null)
				{
					list.Add(primaryState.ref_councilor.ref_hab);
					return list;
				}
				return list;
			case EffectSecondaryStateType.Hab_NMF_CloseIdeology:
				list.AddRange((from x in GameStateManager.AllHumanFactions()
					where x.ref_faction != primaryState.ref_faction && TINationState.GetIdeologicalDistance(primaryState.ref_faction.ideology, x.ref_faction.ideology) < 2f
					select x).SelectMany<TIFactionState, TIHabState>((TIFactionState y) => y.ref_faction.habs));
				return list;
			case EffectSecondaryStateType.SpaceBody_HabSiteParent:
				list.Add(primaryState.ref_habSite.parentBody);
				return list;
			case EffectSecondaryStateType.SpaceBody_FleetInOrbit:
				list.Add(primaryState.ref_fleet.ref_orbit.ref_spaceBody);
				return list;
			case EffectSecondaryStateType.Ship_MyFaction_AnotherFleet:
				list.AddRange(primaryState.ref_faction.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.ref_fleet != primaryState.ref_fleet).SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ref_fleet.ships));
				return list;
			case EffectSecondaryStateType.PriorEvent_Actor:
			{
				if (!TIGlobalValuesState.GlobalValues.priorNarrativeEventData.ContainsKey(narrativeEvent.dataName))
				{
					return list;
				}
				using (List<PriorNarrativeEventData>.Enumerator enumerator = TIGlobalValuesState.GlobalValues.priorNarrativeEventData[narrativeEvent.dataName].GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PriorNarrativeEventData priorNarrativeEventData = enumerator.Current;
						if (priorNarrativeEventData.actorState != null && !priorNarrativeEventData.actorState.deleted)
						{
							list.Add(priorNarrativeEventData.actorState);
						}
					}
					return list;
				}
				break;
			}
			case EffectSecondaryStateType.PriorEvent_Target:
				break;
			case EffectSecondaryStateType.PriorEvent_SecondaryTarget:
				goto IL_0DC1;
			default:
				return list;
			}
			if (!TIGlobalValuesState.GlobalValues.priorNarrativeEventData.ContainsKey(narrativeEvent.dataName))
			{
				return list;
			}
			using (List<PriorNarrativeEventData>.Enumerator enumerator = TIGlobalValuesState.GlobalValues.priorNarrativeEventData[narrativeEvent.dataName].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PriorNarrativeEventData priorNarrativeEventData2 = enumerator.Current;
					if (priorNarrativeEventData2.selectedTarget != null && !priorNarrativeEventData2.selectedTarget.deleted)
					{
						list.Add(priorNarrativeEventData2.selectedTarget);
					}
				}
				return list;
			}
			IL_0DC1:
			if (TIGlobalValuesState.GlobalValues.priorNarrativeEventData.ContainsKey(narrativeEvent.dataName))
			{
				foreach (PriorNarrativeEventData priorNarrativeEventData3 in TIGlobalValuesState.GlobalValues.priorNarrativeEventData[narrativeEvent.dataName])
				{
					if (priorNarrativeEventData3.secondaryTarget != null && !priorNarrativeEventData3.secondaryTarget.deleted)
					{
						list.Add(priorNarrativeEventData3.secondaryTarget);
					}
				}
			}
			return list;
		}

		// Token: 0x06003B87 RID: 15239 RVA: 0x001608D0 File Offset: 0x0015EAD0
		public static TIGameState GetSecondaryStateForEffect(EffectSecondaryStateType secondaryStateType, TIGameState primaryState, TIGameState secondaryInputState = null)
		{
			return TIEffectsState.GetEffectSecondaryStateCandidates(primaryState, secondaryStateType, secondaryInputState, null).SelectRandomItem<TIGameState>();
		}

		// Token: 0x06003B88 RID: 15240 RVA: 0x001608E0 File Offset: 0x0015EAE0
		public static float MinScaledTenPointStatEffect(float value)
		{
			return -2f * Mathf.Abs(value);
		}

		// Token: 0x06003B89 RID: 15241 RVA: 0x001608EE File Offset: 0x0015EAEE
		public static float MaxScaledTenPointStatEffect(float value)
		{
			return 2f * Mathf.Abs(value);
		}

		// Token: 0x06003B8A RID: 15242 RVA: 0x001608FC File Offset: 0x0015EAFC
		private static float RandomizedInstantEffectValue(float value, float randomizer)
		{
			float num = value * randomizer;
			return value - num + TIUtilities.RandomRange(0f, 2f * num);
		}

		// Token: 0x06003B8B RID: 15243 RVA: 0x00160924 File Offset: 0x0015EB24
		public static void ProcessInstantEffect(TIFactionState sourceFaction, EffectTargetType effectTargetType, EffectSecondaryStateType secondaryStateType, InstantEffect instantEffect, float value, float randomizer, string strValue, TIGameState inputState = null, TIGameState secondaryinputState = null, string triggeringTemplateDataName = "")
		{
			TIEffectsState.<>c__DisplayClass24_0 CS$<>8__locals1 = new TIEffectsState.<>c__DisplayClass24_0();
			CS$<>8__locals1.triggeringTemplateDataName = triggeringTemplateDataName;
			CS$<>8__locals1.sourceFaction = sourceFaction;
			List<TIGameState> list = TIEffectsState.InstantEffectTargetToGameStates(CS$<>8__locals1.sourceFaction, effectTargetType, inputState);
			bool flag = secondaryStateType > EffectSecondaryStateType.none;
			List<TIGameState> effectSecondaryStateCandidates = TIEffectsState.GetEffectSecondaryStateCandidates(inputState, secondaryStateType, secondaryinputState, null);
			CS$<>8__locals1.techProgress = null;
			CS$<>8__locals1.factionTechPlacements = null;
			switch (instantEffect)
			{
			case InstantEffect.None:
			case InstantEffect.DummyInstantEffect:
			case InstantEffect.LoseArmyControlPoint:
				return;
			case InstantEffect.Propaganda:
			{
				using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState = enumerator.Current;
						if (secondaryStateType == EffectSecondaryStateType.none)
						{
							if (CS$<>8__locals1.sourceFaction != null)
							{
								tigameState.ref_nation.PropagandaOnPop(CS$<>8__locals1.sourceFaction.ideology, TIEffectsState.RandomizedInstantEffectValue(value, randomizer), false);
							}
						}
						else
						{
							TINationState ref_nation = tigameState.ref_nation;
							TIFactionState ref_faction = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState, secondaryinputState).ref_faction;
							ref_nation.PropagandaOnPop(((ref_faction != null) ? ref_faction.ideology : null) ?? GameStateManager.UndecidedIdeology(), TIEffectsState.RandomizedInstantEffectValue(value, randomizer), false);
						}
					}
					return;
				}
				break;
			}
			case InstantEffect.Propaganda_Faction:
				break;
			case InstantEffect.Propaganda_PerOwnedCP:
				goto IL_047C;
			case InstantEffect.Propaganda_AllFactionsWithCP:
				goto IL_051E;
			case InstantEffect.Propaganda_Region:
				goto IL_05B1;
			case InstantEffect.SpaceScan:
				goto IL_064E;
			case InstantEffect.DamageRegions:
				goto IL_06EC;
			case InstantEffect.DamageRegionBoost:
				goto IL_073D;
			case InstantEffect.DamageRegions_Nuclear:
				goto IL_079E;
			case InstantEffect.NationProsperity:
				goto IL_081C;
			case InstantEffect.NationRecession:
				goto IL_086F;
			case InstantEffect.FreePriorityWelfare:
				goto IL_0906;
			case InstantEffect.FreePriorityEnvironment:
				goto IL_0959;
			case InstantEffect.FreePriorityKnowledge:
				goto IL_09AC;
			case InstantEffect.FreePriorityMilitary:
				goto IL_09FF;
			case InstantEffect.FreePriorityFunding:
				goto IL_0A52;
			case InstantEffect.FreePriorityBoost:
				goto IL_0AA5;
			case InstantEffect.RerollMissingProjects:
				goto IL_0B77;
			case InstantEffect.XenoformingChange:
			{
				using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState2 = enumerator.Current;
						tigameState2.ref_region.xenoforming.ChangeXenoformingLevel(TIEffectsState.RandomizedInstantEffectValue(value, randomizer));
					}
					return;
				}
				goto IL_0BFB;
			}
			case InstantEffect.Exposure:
				goto IL_0BFB;
			case InstantEffect.Exposure_SingleCouncilor:
				goto IL_0CE4;
			case InstantEffect.NationDemocracyChange:
				goto IL_0D93;
			case InstantEffect.NationDemocracyChange_PopScaled:
				goto IL_0E03;
			case InstantEffect.NationMiltechChange:
				goto IL_1A5D;
			case InstantEffect.NationMiltechChange_ReduceExcess:
				goto IL_1AC9;
			case InstantEffect.NationCohesionChange:
				goto IL_0EBB;
			case InstantEffect.NationCohesionChange_ToExtreme:
				goto IL_0FE7;
			case InstantEffect.NationCohesionChange_PopScaled:
				goto IL_0F2D;
			case InstantEffect.NationCohesionChange_ToExtreme_PopScaled:
				goto IL_10D0;
			case InstantEffect.NationEducationChange:
				goto IL_170C;
			case InstantEffect.NationEducationChange_PopScaled:
				goto IL_177A;
			case InstantEffect.NationUnrestChange:
				goto IL_1222;
			case InstantEffect.NationUnrestChange_FactionCredit:
				goto IL_136B;
			case InstantEffect.NationUnrestChange_PopScaled:
				goto IL_129C;
			case InstantEffect.NationUnrestChange_FactionCredit_PopScaled:
				goto IL_145F;
			case InstantEffect.NationGDPPctChange:
				goto IL_1832;
			case InstantEffect.RegionGDPPctChange:
				goto IL_18A2;
			case InstantEffect.RegionGDPPctChange_StrValue:
				goto IL_1920;
			case InstantEffect.MapRegionGDPPctChange_StrValue:
				if (TIUtilities.GetTemplateValue<TIMapRegionTemplate>(strValue) != null)
				{
					TIRegionState tiregionState = GameStateManager.MapRegionLookup(strValue);
					tiregionState.nation.GDPPctChange(TIEffectsState.RandomizedInstantEffectValue(value, randomizer) * tiregionState.NationalGDPProportion(), TINationState.GDPChangeReason.GDPReason_EventEffect);
					return;
				}
				return;
			case InstantEffect.CPVariableNationGDPPctChange:
			{
				float num = Mathf.Max(0.001f, Mathf.Pow((float)(inputState.ref_nation.numControlPoints_unclamped - 1), 1.1f));
				if (num > 0f)
				{
					using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIGameState tigameState3 = enumerator.Current;
							tigameState3.ref_nation.GDPPctChange(TIEffectsState.RandomizedInstantEffectValue(value, randomizer) * num, TINationState.GDPChangeReason.GDPReason_EventEffect);
						}
						return;
					}
					goto IL_1A05;
				}
				return;
			}
			case InstantEffect.AllFactionNationsGDPPctChange:
				goto IL_1A05;
			case InstantEffect.NationInequalityChange:
				goto IL_15E0;
			case InstantEffect.NationInequalityChange_PopScaled:
				goto IL_164E;
			case InstantEffect.NationMaxMiltechChange:
				goto IL_1B91;
			case InstantEffect.NationSetCanBuildSpaceDefenses:
				goto IL_1BD1;
			case InstantEffect.NationSetCanBuildSTOFighters:
				goto IL_1C0F;
			case InstantEffect.NationSetCanDecontaminateRegion:
			{
				using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState4 = enumerator.Current;
						if (secondaryStateType == EffectSecondaryStateType.none)
						{
							tigameState4.ref_nation.ActivateCanDecontaminateRegion();
						}
					}
					return;
				}
				goto IL_1CAD;
			}
			case InstantEffect.NationNukesChange:
				goto IL_1CAD;
			case InstantEffect.NationAnnualSpaceFundingChange:
				goto IL_1D0D;
			case InstantEffect.NationPopGrowthModifierChange:
				goto IL_1D6B;
			case InstantEffect.RegionRevealAlienActivities:
				goto IL_1E94;
			case InstantEffect.GlobalCO2Change_ppm:
				GameStateManager.GlobalValues().AddCO2_ppm(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), GHGSources.Effect);
				return;
			case InstantEffect.GlobalCH4Change_ppm:
				goto IL_1DD8;
			case InstantEffect.GlobalN2OChange_ppm:
				GameStateManager.GlobalValues().AddN2O_ppm(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), GHGSources.Effect);
				return;
			case InstantEffect.GlobalStratosphericAerosolsChange_ppm:
				GameStateManager.GlobalValues().AddStratosphericAerosols_ppm(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), false);
				return;
			case InstantEffect.GlobalSeaLevelChange_cm:
				GameStateManager.GlobalValues().AddToSeaLevel_cm(TIEffectsState.RandomizedInstantEffectValue(value, randomizer));
				return;
			case InstantEffect.GlobalLooseNukesChange:
				GameStateManager.GlobalValues().ChangeLooseNukesValue((int)value);
				return;
			case InstantEffect.GlobalSeasonalOceansToPermanent:
			{
				using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState5 = enumerator.Current;
						TIRegionState ref_region = tigameState5.ref_region;
						if (ref_region.oceanType == WorldOceanType.Seasonal)
						{
							ref_region.ChangeOceanType(WorldOceanType.Yes);
						}
					}
					return;
				}
				goto IL_1E94;
			}
			case InstantEffect.Atrocity:
				goto IL_1F90;
			case InstantEffect.GainOpenControlPoint:
			{
				if (!(CS$<>8__locals1.sourceFaction != null))
				{
					return;
				}
				TIControlPoint ticontrolPoint = secondaryinputState.ref_nation.FirstNativeControlPoint();
				if (ticontrolPoint != null)
				{
					secondaryinputState.ref_nation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Event, CS$<>8__locals1.sourceFaction);
					return;
				}
				return;
			}
			case InstantEffect.GainAnyControlPoint:
			{
				if (!(CS$<>8__locals1.sourceFaction != null))
				{
					return;
				}
				TIControlPoint ticontrolPoint2 = secondaryinputState.ref_nation.RandomOtherFactionControlPoint(CS$<>8__locals1.sourceFaction, false, true);
				if (ticontrolPoint2 == null && secondaryinputState.ref_nation.NumNativeControlPoints > 1)
				{
					ticontrolPoint2 = secondaryinputState.ref_nation.FirstNativeControlPoint();
				}
				if (ticontrolPoint2 != null)
				{
					secondaryinputState.ref_nation.ChangeControlPointOwner(ticontrolPoint2.positionInNation, ControlPointChangeCause.Event, CS$<>8__locals1.sourceFaction);
					return;
				}
				return;
			}
			case InstantEffect.GainAnyControlPoint_Plus:
			{
				if (!(CS$<>8__locals1.sourceFaction != null))
				{
					return;
				}
				TIControlPoint ticontrolPoint3 = secondaryinputState.ref_nation.RandomOtherFactionControlPoint(CS$<>8__locals1.sourceFaction, true, true);
				if (ticontrolPoint3 == null && secondaryinputState.ref_nation.NumNativeControlPoints > 1)
				{
					ticontrolPoint3 = secondaryinputState.ref_nation.FirstNativeControlPoint();
				}
				if (!(ticontrolPoint3 != null))
				{
					return;
				}
				List<TIGameState> controlPointOwnersByPoint = secondaryinputState.ref_nation.controlPointOwnersByPoint;
				TIFactionState faction3 = ticontrolPoint3.faction;
				secondaryinputState.ref_nation.ChangeControlPointOwner(ticontrolPoint3.positionInNation, ControlPointChangeCause.Event, CS$<>8__locals1.sourceFaction);
				ticontrolPoint3.ResolveDefendControlPointEffect((int)(value * 30.436874f));
				List<TIGameState> controlPointOwnersByPoint2 = secondaryinputState.ref_nation.controlPointOwnersByPoint;
				if (faction3 != null)
				{
					TINotificationQueueState.LogMyControlPointPurged(faction3, CS$<>8__locals1.sourceFaction, ticontrolPoint3, controlPointOwnersByPoint2, controlPointOwnersByPoint);
					return;
				}
				return;
			}
			case InstantEffect.ReassignControlPointOfTypeByPopularity:
			{
				if (!(CS$<>8__locals1.sourceFaction != null))
				{
					return;
				}
				ControlPointType controlPointType = strValue.ToEnum(ControlPointType.none);
				if (secondaryStateType == EffectSecondaryStateType.none)
				{
					inputState.ref_nation.GrantControlPointOfTypeByPopularity(controlPointType, CS$<>8__locals1.sourceFaction, value);
					return;
				}
				secondaryinputState.ref_nation.GrantControlPointOfTypeByPopularity(controlPointType, CS$<>8__locals1.sourceFaction, value);
				return;
			}
			case InstantEffect.RedistributeControlPointsByPopularity_Individual:
				if (!(CS$<>8__locals1.sourceFaction != null))
				{
					return;
				}
				if (secondaryStateType == EffectSecondaryStateType.none)
				{
					inputState.ref_nation.DistributeControlPointsByPopularity_Individual(CS$<>8__locals1.sourceFaction, value);
					return;
				}
				secondaryinputState.ref_nation.DistributeControlPointsByPopularity_Individual(CS$<>8__locals1.sourceFaction, value);
				return;
			case InstantEffect.GainControlPointOfType:
			{
				ControlPointType controlPointType2 = (ControlPointType)value;
				using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState6 = enumerator.Current;
						TIControlPoint controlPointOfType = tigameState6.ref_nation.GetControlPointOfType(controlPointType2);
						if (controlPointOfType != null)
						{
							tigameState6.ref_nation.ChangeControlPointOwner(controlPointOfType.positionInNation, ControlPointChangeCause.Event, inputState.ref_faction);
						}
					}
					return;
				}
				goto IL_2352;
			}
			case InstantEffect.CrackdownArmyControlPoint:
				goto IL_2352;
			case InstantEffect.DefendAllOwnedControlPoints:
				goto IL_23E3;
			case InstantEffect.GainMoneyIncome:
				goto IL_245F;
			case InstantEffect.GainInfluenceIncome:
			{
				TIFactionState sourceFaction2 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction2 == null)
				{
					return;
				}
				sourceFaction2.ChangeBaseResourceIncome(FactionResource.Influence, value * 12f);
				return;
			}
			case InstantEffect.GainOpsIncome:
			{
				TIFactionState sourceFaction3 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction3 == null)
				{
					return;
				}
				sourceFaction3.ChangeBaseResourceIncome(FactionResource.Operations, value * 12f);
				return;
			}
			case InstantEffect.GainBoostIncome:
			{
				TIFactionState sourceFaction4 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction4 == null)
				{
					return;
				}
				sourceFaction4.ChangeBaseResourceIncome(FactionResource.Boost, value * 12f);
				return;
			}
			case InstantEffect.GainResearchIncome:
			{
				TIFactionState sourceFaction5 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction5 == null)
				{
					return;
				}
				sourceFaction5.ChangeBaseResourceIncome(FactionResource.Research, value * 12f);
				return;
			}
			case InstantEffect.GainMissionControl:
			{
				TIFactionState sourceFaction6 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction6 == null)
				{
					return;
				}
				sourceFaction6.ChangeBaseResourceIncome(FactionResource.MissionControl, value);
				return;
			}
			case InstantEffect.GainWaterIncome:
			{
				TIFactionState sourceFaction7 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction7 == null)
				{
					return;
				}
				sourceFaction7.ChangeBaseResourceIncome(FactionResource.Water, value * 12f);
				return;
			}
			case InstantEffect.GainVolatilesIncome:
			{
				TIFactionState sourceFaction8 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction8 == null)
				{
					return;
				}
				sourceFaction8.ChangeBaseResourceIncome(FactionResource.Volatiles, value * 12f);
				return;
			}
			case InstantEffect.GainMetalsIncome:
			{
				TIFactionState sourceFaction9 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction9 == null)
				{
					return;
				}
				sourceFaction9.ChangeBaseResourceIncome(FactionResource.Metals, value * 12f);
				return;
			}
			case InstantEffect.GainNoblesIncome:
			{
				TIFactionState sourceFaction10 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction10 == null)
				{
					return;
				}
				sourceFaction10.ChangeBaseResourceIncome(FactionResource.NobleMetals, value * 12f);
				return;
			}
			case InstantEffect.GainFissilesIncome:
			{
				TIFactionState sourceFaction11 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction11 == null)
				{
					return;
				}
				sourceFaction11.ChangeBaseResourceIncome(FactionResource.Fissiles, value * 12f);
				return;
			}
			case InstantEffect.GainAntimatterIncome:
			{
				TIFactionState sourceFaction12 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction12 == null)
				{
					return;
				}
				sourceFaction12.ChangeBaseResourceIncome(FactionResource.Antimatter, value * 12f);
				return;
			}
			case InstantEffect.GainExoticsIncome:
			{
				TIFactionState sourceFaction13 = CS$<>8__locals1.sourceFaction;
				if (sourceFaction13 == null)
				{
					return;
				}
				sourceFaction13.ChangeBaseResourceIncome(FactionResource.Exotics, value * 12f);
				return;
			}
			case InstantEffect.UpgradeRelations:
			{
				using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState7 = enumerator.Current;
						if (secondaryStateType != EffectSecondaryStateType.none && CS$<>8__locals1.sourceFaction != null)
						{
							tigameState7.ref_nation.UpgradeRelations(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_nation);
						}
					}
					return;
				}
				goto IL_260A;
			}
			case InstantEffect.DowngradeRelations:
				goto IL_260A;
			case InstantEffect.DeclareLimitedWar:
				goto IL_2663;
			case InstantEffect.DeclareFullWar:
				goto IL_26BC;
			case InstantEffect.JoinPrimaryFederation:
				goto IL_2715;
			case InstantEffect.JoinSecondaryFederation:
				goto IL_27CA;
			case InstantEffect.LoseUndefendedControlPoint:
			{
				if (!(CS$<>8__locals1.sourceFaction != null))
				{
					return;
				}
				TIControlPoint ticontrolPoint4 = inputState.ref_nation.HighestFactionControlPoint(CS$<>8__locals1.sourceFaction, false);
				if (ticontrolPoint4 != null && secondaryStateType != EffectSecondaryStateType.none)
				{
					List<TIGameState> controlPointOwnersByPoint3 = inputState.ref_nation.controlPointOwnersByPoint;
					inputState.ref_nation.ChangeControlPointOwner(ticontrolPoint4.positionInNation, ControlPointChangeCause.Event, secondaryinputState.ref_faction);
					List<TIGameState> controlPointOwnersByPoint4 = inputState.ref_nation.controlPointOwnersByPoint;
					TINotificationQueueState.LogLoyaltySwitch(secondaryinputState.ref_faction, CS$<>8__locals1.sourceFaction, ticontrolPoint4, controlPointOwnersByPoint4, controlPointOwnersByPoint3, null);
					return;
				}
				return;
			}
			case InstantEffect.LoseUndefendedControlPoint_Plus:
			{
				if (!(CS$<>8__locals1.sourceFaction != null))
				{
					return;
				}
				TIControlPoint ticontrolPoint5 = inputState.ref_nation.HighestFactionControlPoint(CS$<>8__locals1.sourceFaction, false);
				if (ticontrolPoint5 != null && secondaryStateType != EffectSecondaryStateType.none)
				{
					List<TIGameState> controlPointOwnersByPoint5 = inputState.ref_nation.controlPointOwnersByPoint;
					inputState.ref_nation.ChangeControlPointOwner(ticontrolPoint5.positionInNation, ControlPointChangeCause.Event, secondaryinputState.ref_faction);
					List<TIGameState> controlPointOwnersByPoint6 = inputState.ref_nation.controlPointOwnersByPoint;
					ticontrolPoint5.ResolveDefendControlPointEffect((int)(value * 30.436874f));
					TINotificationQueueState.LogLoyaltySwitch(secondaryinputState.ref_faction, CS$<>8__locals1.sourceFaction, ticontrolPoint5, controlPointOwnersByPoint6, controlPointOwnersByPoint5, null);
					return;
				}
				return;
			}
			case InstantEffect.DamageNationalSpaceAssets:
				goto IL_287F;
			case InstantEffect.RegionPopulationPctChange:
				goto IL_2985;
			case InstantEffect.RegionPopulationPctChange_WealthMitigation:
				goto IL_29EE;
			case InstantEffect.RegionTransferPopulationPctToSecondary:
				goto IL_2AAD;
			case InstantEffect.RegionAbductionsChange:
				goto IL_2B16;
			case InstantEffect.NationAbductionsChange:
				goto IL_2B55;
			case InstantEffect.RegionNuclearDetonationsChange:
				goto IL_2BC2;
			case InstantEffect.GainFactionSpaceOrg:
				goto IL_2C02;
			case InstantEffect.PrimaryOccupiesSecondary:
			{
				using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState8 = enumerator.Current;
						if (secondaryStateType != EffectSecondaryStateType.none)
						{
							secondaryinputState.ref_region.IncreaseOccupationValue(tigameState8.ref_nation, value, null);
						}
					}
					return;
				}
				goto IL_2C83;
			}
			case InstantEffect.SecondaryOccupiesPrimary:
				goto IL_2C83;
			case InstantEffect.PrimaryRegimeChangesSecondary:
				goto IL_2CCB;
			case InstantEffect.SecondaryRegimeChangesPrimary:
				goto IL_2D2A;
			case InstantEffect.PrimaryAbsorbsSecondary:
				goto IL_2D89;
			case InstantEffect.SecondaryAbsorbsPrimary:
				goto IL_2DD4;
			case InstantEffect.SecondaryAnnexesPrimary:
				goto IL_2E1F;
			case InstantEffect.RandomSecession:
				goto IL_2E9D;
			case InstantEffect.NationBreaksUp:
				goto IL_2F51;
			case InstantEffect.Coup:
				goto IL_2FE7;
			case InstantEffect.GlobalIPProduction:
				goto IL_5104;
			case InstantEffect.RandomFactionNationGainsRandomClaim:
				goto IL_51A3;
			case InstantEffect.NationLosesClaim:
				goto IL_3049;
			case InstantEffect.NationMoveCapitalToSecondaryOwnedRegion:
				goto IL_30A7;
			case InstantEffect.DestroyRandomModules:
				goto IL_3116;
			case InstantEffect.DestroyRandomModules_Marines:
				goto IL_3194;
			case InstantEffect.DestroyRandomModules_Power:
				goto IL_3212;
			case InstantEffect.DestroyRandomNumberOfModules:
				goto IL_3290;
			case InstantEffect.DestroyHabSector:
				goto IL_331F;
			case InstantEffect.DestroyHab:
				goto IL_33DD;
			case InstantEffect.HabDefectsToSecondary:
				goto IL_342C;
			case InstantEffect.DestroyShip:
				goto IL_3477;
			case InstantEffect.ShipDefectsToSecondary:
				goto IL_34BB;
			case InstantEffect.DamageShipParts_Marines:
				goto IL_3545;
			case InstantEffect.DamageShipParts_SpecifiedUtilityModule:
				goto IL_35F7;
			case InstantEffect.ShipNuclearTorpedoMagazineChange:
				goto IL_36B4;
			case InstantEffect.ShipFreeOfficerCreation:
				goto IL_3785;
			case InstantEffect.ShipFreeOfficerPromotion:
				goto IL_37C6;
			case InstantEffect.SpawnSpaceFleet_StrValue:
				goto IL_3832;
			case InstantEffect.OfficerPromoted:
			{
				using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState9 = enumerator.Current;
						int num2 = 0;
						while ((float)num2 < value)
						{
							tigameState9.ref_officer.Promote();
							num2++;
						}
					}
					return;
				}
				goto IL_38B6;
			}
			case InstantEffect.FreeDaysSpaceResourceIncome:
				goto IL_38B6;
			case InstantEffect.LoseDaysSpaceResourceIncome:
				goto IL_392A;
			case InstantEffect.ModifyHabMiningResourceIncomes:
				goto IL_399E;
			case InstantEffect.FreeMonthsHabMiningResourceIncome:
				goto IL_3A0F;
			case InstantEffect.OrbitDestroyedAssetsChange:
				goto IL_3AF1;
			case InstantEffect.LEODestroyedAssetsChange:
				goto IL_3B44;
			case InstantEffect.SpawnMegafaunaArmies:
			{
				using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState10 = enumerator.Current;
						int num3 = 0;
						while ((float)num3 < value)
						{
							tigameState10.ref_xenoforming.SpawnMegafaunaArmy();
							num3++;
						}
					}
					return;
				}
				goto IL_3C45;
			}
			case InstantEffect.SpawnMegafaunaArmyDamaged:
				goto IL_3C45;
			case InstantEffect.AlienSpaceResourceSharing:
				goto IL_3CD8;
			case InstantEffect.CouncilorGainsXP:
				goto IL_3D8F;
			case InstantEffect.CouncilorGainsTrait:
				goto IL_3DCF;
			case InstantEffect.FactionAllCouncilorsModifyAttribute:
				goto IL_3E7B;
			case InstantEffect.FactionAllCouncilorsGainTrait:
				goto IL_3E0E;
			case InstantEffect.FactionAllEligibleCouncilorsGainXP:
				goto IL_402D;
			case InstantEffect.GainExoticsFromSpaceIndustry:
				goto IL_40E2;
			case InstantEffect.FactionAllEligibleCouncilorsGainTrait:
				goto IL_3F40;
			case InstantEffect.CouncilorLosesTrait:
				goto IL_41D2;
			case InstantEffect.CouncilorGainsTraitGroup:
				goto IL_4234;
			case InstantEffect.CouncilorLosesTraitGroup:
				goto IL_42AA;
			case InstantEffect.CouncilorDetained:
				goto IL_42FA;
			case InstantEffect.CouncilorKilled:
				goto IL_4357;
			case InstantEffect.CouncilorKilled_NoProtection:
				goto IL_43AD;
			case InstantEffect.CouncilorKilled_NoProtection_Nonviolent:
				goto IL_43EC;
			case InstantEffect.CouncilorInHiding:
				goto IL_442B;
			case InstantEffect.CouncilorModifyAttribute:
				goto IL_4462;
			case InstantEffect.CouncilorHomeNationPropaganda:
				goto IL_44AF;
			case InstantEffect.CouncilorHomeNationsImproveRelations:
				goto IL_4507;
			case InstantEffect.CouncilorLosesOrgs:
				goto IL_455C;
			case InstantEffect.CouncilorInitializeUnique_StrValue:
				goto IL_465F;
			case InstantEffect.FactionInvestigationsChange:
				if (CS$<>8__locals1.sourceFaction != null)
				{
					CS$<>8__locals1.sourceFaction.alienInvestigations += (int)value;
					return;
				}
				return;
			case InstantEffect.ArmyStrengthChange:
			{
				using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState11 = enumerator.Current;
						tigameState11.ref_army.TakeDamage(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), tigameState11.ref_faction, null, true);
					}
					return;
				}
				goto IL_4740;
			}
			case InstantEffect.DecreaseRegionOccupations:
				goto IL_4740;
			case InstantEffect.DamageAllRegionArmies:
				goto IL_4880;
			case InstantEffect.DamageAllRegionArmies_Enemy:
				goto IL_4907;
			case InstantEffect.DamageAllNationArmies:
				goto IL_4999;
			case InstantEffect.EndofOil:
				goto IL_4A20;
			case InstantEffect.GlobalCrackdown:
				goto IL_47AB;
			case InstantEffect.RegionCreateCoreEco_InputState:
			{
				using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState12 = enumerator.Current;
						tigameState12.ref_region.coreEconomicRegion = Convert.ToBoolean(value);
						GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(tigameState12.ref_region), null, new object[] { tigameState12.ref_region });
					}
					return;
				}
				goto IL_4AEA;
			}
			case InstantEffect.RegionCreateCoreEco_StrValue:
				goto IL_4BDB;
			case InstantEffect.RegionCreateResource_InputState:
				goto IL_4AEA;
			case InstantEffect.RegionCreateResource_StrValue:
				if (TIUtilities.GetTemplateValue<TIRegionTemplate>(strValue) != null)
				{
					TIRegionState tiregionState2 = GameStateManager.RegionLookup()[strValue];
					tiregionState2.resourceRegion = Convert.ToBoolean(value);
					GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(tiregionState2), null, new object[] { tiregionState2 });
					return;
				}
				return;
			case InstantEffect.RegionCreateOilResource_InputState:
				goto IL_4B59;
			case InstantEffect.RegionCreateOilResource_StrValue:
			{
				if (TIUtilities.GetTemplateValue<TIRegionTemplate>(strValue) == null)
				{
					return;
				}
				TIRegionState tiregionState3 = GameStateManager.RegionLookup()[strValue];
				if (tiregionState3.template.oilResource)
				{
					tiregionState3.resourceRegion = Convert.ToBoolean(value);
					GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(tiregionState3), null, new object[] { tiregionState3 });
					return;
				}
				return;
			}
			case InstantEffect.RegionAccumulateCoreEconomyTriggers:
			{
				using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState13 = enumerator.Current;
						tigameState13.ref_region.accumulatedCoreEconomyRegionTriggers += (int)value;
					}
					return;
				}
				goto IL_4D05;
			}
			case InstantEffect.RegionAccumulateCoreOilTriggers:
				goto IL_4D05;
			case InstantEffect.RegionAccumulateCoreMiningTriggers:
				goto IL_4D4C;
			case InstantEffect.RegionAccumulateDecontaminateTriggers:
				goto IL_4DDA;
			case InstantEffect.RegionAccumulateDecolonizeTriggers:
				goto IL_4D93;
			case InstantEffect.EnergyCrisis:
				goto IL_4E21;
			case InstantEffect.TriggerNarrativeEvent_StrValue:
				goto IL_4F63;
			case InstantEffect.UpdateAlienThreatMeter:
				CS$<>8__locals1.sourceFaction.UpdateEstimatedAlienHate(0f, true);
				return;
			case InstantEffect.UpdateAlienThreatMeter_Accurate:
				CS$<>8__locals1.sourceFaction.FixAssessedAlienHateToActualValue();
				return;
			case InstantEffect.SetAlienHate:
				GameStateManager.AlienFaction().SetFactionHate(CS$<>8__locals1.sourceFaction, value, true, "");
				return;
			case InstantEffect.GainAlienHate:
				GameStateManager.AlienFaction().GainFactionHate(CS$<>8__locals1.sourceFaction, value, true, "Effect", true);
				return;
			case InstantEffect.LoseAlienHate:
				GameStateManager.AlienFaction().GainFactionHate(CS$<>8__locals1.sourceFaction, -value, true, "Effect", true);
				return;
			case InstantEffect.RemoveEffectFromFaction:
			{
				TIEffectTemplate tieffectTemplate = TemplateManager.Find<TIEffectTemplate>(strValue, false);
				using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState14 = enumerator.Current;
						if (value == 0f)
						{
							GameStateManager.Effects().RemoveEffect(tieffectTemplate, tigameState14.ref_faction);
						}
						int num4 = 0;
						while ((float)num4 < value)
						{
							GameStateManager.Effects().RemoveEffect(tieffectTemplate, tigameState14.ref_faction);
							num4++;
						}
					}
					return;
				}
				goto IL_50C1;
			}
			case InstantEffect.CompleteCampaignMilestone:
				goto IL_50C1;
			case InstantEffect.BSBE_1stPlaceInTheSpaceRace:
			{
				CS$<>8__locals1.<ProcessInstantEffect>g__GetTriggeringTechProgressAndFactionPlacements|0();
				for (int i = 0; i < CS$<>8__locals1.factionTechPlacements.Count; i++)
				{
					if (i >= 1)
					{
						return;
					}
					TIFactionState tifactionState = CS$<>8__locals1.factionTechPlacements[i];
					IEnumerable<TIHabSiteState> enumerable = GameStateManager.Luna().habSites.Where<TIHabSiteState>((TIHabSiteState x) => !x.hasPlannedOrOperatingBase);
					if (enumerable.Any<TIHabSiteState>())
					{
						TIHabSiteState tihabSiteState = enumerable.MaxBy<TIHabSiteState, float>(delegate(TIHabSiteState x)
						{
							float num49 = (float)TIResourcesCost.basicSpaceResources.Count<FactionResource>((FactionResource y) => x.GetDailyProduction(y) > 0.01f);
							float num50 = TIResourcesCost.basicSpaceResources.Sum<FactionResource>((FactionResource y) => x.GetDailyProduction(y));
							float num51 = num49 * Mathf.Pow(num50, 0.5f);
							if (x.GetDailyProduction(FactionResource.Water) > 0.01f)
							{
								num51 *= 2f;
							}
							if (x.GetDailyProduction(FactionResource.Volatiles) > 0.01f)
							{
								num51 *= 2f;
							}
							return num51;
						});
						tihabSiteState.MarkPendingHab();
						new FoundOutpostOperation().ExecuteOperation(tifactionState, tihabSiteState);
						tihabSiteState.hab.coreModule.CompleteConstruction(false);
						TIHabModuleTemplate tihabModuleTemplate = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Mining)
							where !x.alienModule && !x.automated
							select x).MinBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
						TIHabModuleTemplate tihabModuleTemplate2 = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Power)
							where !x.alienModule && !x.automated && x.IsSolarPower
							select x).MinBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
						tihabSiteState.hab.MineSlot.SetCompletedModule(tihabModuleTemplate.dataName, false);
						tihabSiteState.hab.AvailableSlots().First<TIHabModuleState>().SetCompletedModule(tihabModuleTemplate2.dataName, false);
						tihabSiteState.hab.ResetPower();
						int num5 = tihabSiteState.hab.GetNetCurrentMonthlyIncome(tifactionState, FactionResource.MissionControl, true, false).Round();
						tifactionState.ChangeBaseResourceIncome(FactionResource.MissionControl, (float)(-(float)num5 + 1));
					}
				}
				return;
			}
			case InstantEffect.BSBE_2ndPlaceInTheSpaceRace:
			{
				CS$<>8__locals1.<ProcessInstantEffect>g__GetTriggeringTechProgressAndFactionPlacements|0();
				for (int j = 0; j < CS$<>8__locals1.factionTechPlacements.Count; j++)
				{
					if (j >= 2)
					{
						return;
					}
					TIFactionState faction4 = CS$<>8__locals1.factionTechPlacements[j];
					IEnumerable<TIOrbitState> enumerable2 = GameStateManager.Earth().orbits.Where<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(1, faction4));
					if (!enumerable2.Any<TIOrbitState>())
					{
						break;
					}
					TIOrbitState tiorbitState = enumerable2.MinBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km);
					tiorbitState.MarkPendingHab();
					new FoundOrbitalOperation().ExecuteOperation(faction4, tiorbitState);
					TIHabState tihabState = tiorbitState.stationsInOrbit.MaxBy<TIHabState, DateTime>((TIHabState x) => x.coreModule.completionDate);
					tihabState.coreModule.CompleteConstruction(false);
					TIHabModuleTemplate tihabModuleTemplate3 = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Power)
						where !x.alienModule && !x.automated && x.IsSolarPower
						select x).MinBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
					int powerPerModule = tihabModuleTemplate3.ProspectivePower(tihabState, faction4);
					for (int k = 0; k < 3; k++)
					{
						tihabState.AvailableSlots().First<TIHabModuleState>().SetCompletedModule(tihabModuleTemplate3.dataName, false);
					}
					TIHabModuleTemplate tihabModuleTemplate4 = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Research)
						where !x.alienModule && !x.automated && x.tier == 1 && (double)(-(double)x.power) <= (double)powerPerModule * 1.501
						where x.incomeAntimatter_month == 0f && !x.SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter)
						select x).SelectRandomItem<TIHabModuleTemplate>();
					TIHabModuleTemplate tihabModuleTemplate5 = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Influence)
						where !x.alienModule && !x.automated && x.tier == 1 && (double)(-(double)x.power) <= (double)powerPerModule * 1.501
						where x.incomeAntimatter_month == 0f && !x.SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter)
						select x).MaxBy<TIHabModuleTemplate, float>((TIHabModuleTemplate x) => x.incomeInfluence_month);
					tihabState.AvailableSlots().First<TIHabModuleState>().SetCompletedModule(tihabModuleTemplate4.dataName, false);
					tihabState.AvailableSlots().First<TIHabModuleState>().SetCompletedModule(tihabModuleTemplate5.dataName, false);
					tihabState.ResetPower();
					int num6 = tihabState.GetNetCurrentMonthlyIncome(faction4, FactionResource.MissionControl, true, false).Round();
					faction4.ChangeBaseResourceIncome(FactionResource.MissionControl, (float)(-(float)num6));
				}
				return;
			}
			case InstantEffect.BSBE_3rdPlaceInTheSpaceRace:
			{
				CS$<>8__locals1.<ProcessInstantEffect>g__GetTriggeringTechProgressAndFactionPlacements|0();
				int num7 = 0;
				while (num7 < CS$<>8__locals1.factionTechPlacements.Count && num7 < 3)
				{
					TIFactionState faction = CS$<>8__locals1.factionTechPlacements[num7];
					IEnumerable<TINationState> enumerable3 = faction.nationsWithInterest(false);
					if (enumerable3.Any<TINationState>())
					{
						IEnumerable<TINationState> enumerable4 = enumerable3.Where<TINationState>((TINationState x) => x.executiveFaction == faction);
						if (enumerable4.Any<TINationState>())
						{
							enumerable3 = enumerable4;
						}
						int highestControlPoints = enumerable3.Max<TINationState>((TINationState x) => x.controlPoints.Count);
						IEnumerable<TINationState> enumerable5 = enumerable3.Where<TINationState>((TINationState x) => x.controlPoints.Count >= highestControlPoints);
						if (enumerable5.Count<TINationState>() >= 2)
						{
							enumerable3 = enumerable5;
						}
						else
						{
							enumerable5 = enumerable3.Where<TINationState>((TINationState x) => x.controlPoints.Count >= highestControlPoints - 1);
							if (enumerable5.Any<TINationState>())
							{
								enumerable3 = enumerable5;
							}
						}
						TINationState tinationState = enumerable3.MinBy<TINationState, float>((TINationState x) => x.BestBoostLatitude);
						float num8 = 100f;
						if (!tinationState.spaceFlightProgram)
						{
							num8 -= tinationState.GetRequiredInvestmentPointsForPriority(PriorityType.Civilian_InitiateSpaceflightProgram);
							tinationState.GrantSpaceFlightProgram();
						}
						TIEffectsState.ProcessInstantEffect(faction, EffectTargetType.InputState, EffectSecondaryStateType.none, InstantEffect.FreePriorityBoost, num8, randomizer, "", tinationState, secondaryinputState, CS$<>8__locals1.triggeringTemplateDataName);
						tinationState.ModifyGDP(100000000000.0, TINationState.GDPChangeReason.GDPReason_EventEffect);
					}
					float yearlyIncome = faction.GetYearlyIncome(FactionResource.Boost, false, false, true);
					if (yearlyIncome < 0f)
					{
						faction.ChangeBaseResourceIncome(FactionResource.Boost, -yearlyIncome);
					}
					num7++;
				}
				return;
			}
			default:
				return;
			}
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState15 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						if (CS$<>8__locals1.sourceFaction != null)
						{
							tigameState15.ref_nation.PropagandaOnPop(CS$<>8__locals1.sourceFaction.ideology, TIEffectsState.RandomizedInstantEffectValue(value, randomizer), false);
						}
					}
					else if (CS$<>8__locals1.sourceFaction != null)
					{
						TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState15, secondaryinputState).ref_nation.PropagandaOnPop(CS$<>8__locals1.sourceFaction.ideology, TIEffectsState.RandomizedInstantEffectValue(value, randomizer), false);
					}
				}
				return;
			}
			IL_047C:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState16 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						if (CS$<>8__locals1.sourceFaction != null)
						{
							tigameState16.ref_nation.PropagandaOnPop_PerOwnedCP(CS$<>8__locals1.sourceFaction.ideology, TIEffectsState.RandomizedInstantEffectValue(value, randomizer), 0, false);
						}
					}
					else
					{
						TINationState ref_nation2 = tigameState16.ref_nation;
						TIFactionState ref_faction2 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState16, secondaryinputState).ref_faction;
						ref_nation2.PropagandaOnPop_PerOwnedCP(((ref_faction2 != null) ? ref_faction2.ideology : null) ?? GameStateManager.UndecidedIdeology(), TIEffectsState.RandomizedInstantEffectValue(value, randomizer), 0, false);
					}
				}
				return;
			}
			IL_051E:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState17 = enumerator.Current;
					if (tigameState17.ref_nation != null)
					{
						foreach (TIFactionState tifactionState2 in tigameState17.ref_nation.FactionsWithControlPoint)
						{
							tigameState17.ref_nation.PropagandaOnPop(tifactionState2.ideology, TIEffectsState.RandomizedInstantEffectValue(value, randomizer), false);
						}
					}
				}
				return;
			}
			IL_05B1:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState18 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						if (CS$<>8__locals1.sourceFaction != null)
						{
							tigameState18.ref_region.PropagandaOnPop(CS$<>8__locals1.sourceFaction.ideology, TIEffectsState.RandomizedInstantEffectValue(value, randomizer));
						}
					}
					else
					{
						TIRegionState ref_region2 = tigameState18.ref_region;
						TIFactionState ref_faction3 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState18, secondaryinputState).ref_faction;
						ref_region2.PropagandaOnPop(((ref_faction3 != null) ? ref_faction3.ideology : null) ?? GameStateManager.UndecidedIdeology(), TIEffectsState.RandomizedInstantEffectValue(value, randomizer));
					}
				}
				return;
			}
			IL_064E:
			List<TISpaceAssetState> list2 = new List<TISpaceAssetState>();
			list2.AddRange(GameStateManager.AlienFaction().fleets);
			list2.AddRange(GameStateManager.AlienFaction().habs);
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState19 = enumerator.Current;
					TIFactionState tifactionState3 = (TIFactionState)tigameState19;
					foreach (TISpaceAssetState tispaceAssetState in list2)
					{
						tifactionState3.SetIntel(tispaceAssetState, tispaceAssetState.BaselineIntelOnAlienAsset(tifactionState3), null, false);
					}
				}
				return;
			}
			IL_06EC:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState20 = enumerator.Current;
					tigameState20.ref_region.ApplyDamageToRegion(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), CS$<>8__locals1.sourceFaction, null, true, false, false, false);
				}
				return;
			}
			IL_073D:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState21 = enumerator.Current;
					TIRegionState ref_region3 = tigameState21.ref_region;
					float num9 = TIUtilities.RandomRange(value / 3f, value);
					ref_region3.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -num9 * ref_region3.boostPerMonth_dekatons, false, false);
				}
				return;
			}
			IL_079E:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState22 = enumerator.Current;
					GameControl.eventManager.TriggerEvent(new NuclearStrike(tigameState22.ref_nation, tigameState22.ref_region), null, new object[] { tigameState22.ref_region });
					tigameState22.ref_region.ApplyDamageToRegion(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), null, null, true, true, false, true);
				}
				return;
			}
			IL_081C:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState23 = enumerator.Current;
					for (int l = 0; l < (int)value; l++)
					{
						tigameState23.ref_nation.OnEconomyPriorityComplete();
					}
				}
				return;
			}
			IL_086F:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState24 = enumerator.Current;
					TINationState ref_nation3 = tigameState24.ref_nation;
					for (int m = 0; m < (int)value; m++)
					{
						tigameState24.ref_nation.ModifyGDP((double)(-8f * ref_nation3.population_Millions * 1000000f), TINationState.GDPChangeReason.GDPReason_EventEffect);
						tigameState24.ref_nation.AddToInequality(ref_nation3.economyPriorityInequalityChange, TINationState.InequalityChangeReason.InqReason_EventEffects);
						TIGlobalValuesState.GlobalValues.ModifyMarketValuesForRecession(ref_nation3.numControlPoints);
					}
				}
				return;
			}
			IL_0906:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState25 = enumerator.Current;
					for (int n = 0; n < (int)value; n++)
					{
						tigameState25.ref_nation.OnWelfarePriorityComplete();
					}
				}
				return;
			}
			IL_0959:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState26 = enumerator.Current;
					for (int num10 = 0; num10 < (int)value; num10++)
					{
						tigameState26.ref_nation.OnEnvironmentPriorityComplete();
					}
				}
				return;
			}
			IL_09AC:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState27 = enumerator.Current;
					for (int num11 = 0; num11 < (int)value; num11++)
					{
						tigameState27.ref_nation.OnKnowledgePriorityComplete();
					}
				}
				return;
			}
			IL_09FF:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState28 = enumerator.Current;
					for (int num12 = 0; num12 < (int)value; num12++)
					{
						tigameState28.ref_nation.OnMilitaryPriorityComplete();
					}
				}
				return;
			}
			IL_0A52:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState29 = enumerator.Current;
					for (int num13 = 0; num13 < (int)value; num13++)
					{
						tigameState29.ref_nation.OnFundingPriorityComplete();
					}
				}
				return;
			}
			IL_0AA5:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState30 = enumerator.Current;
					if (tigameState30.ref_nation.spaceFlightProgram)
					{
						for (int num14 = 0; num14 < (int)value; num14++)
						{
							tigameState30.ref_nation.OnBoostPriorityComplete();
						}
					}
					else
					{
						float requiredInvestmentPointsForPriority = tigameState30.ref_nation.GetRequiredInvestmentPointsForPriority(PriorityType.LaunchFacilities);
						float num15 = value * requiredInvestmentPointsForPriority;
						float num16 = tigameState30.ref_nation.DeltaToInvestmentThreshhold(PriorityType.Civilian_InitiateSpaceflightProgram);
						float num17 = num15 - num16;
						tigameState30.ref_nation.ModifyAccumulatedInvestment(PriorityType.Civilian_InitiateSpaceflightProgram, (float)((int)value), false, true);
						if (num17 > requiredInvestmentPointsForPriority)
						{
							int num18 = (int)(num17 / requiredInvestmentPointsForPriority);
							for (int num19 = 0; num19 < num18; num19++)
							{
								tigameState30.ref_nation.OnBoostPriorityComplete();
							}
						}
					}
				}
				return;
			}
			IL_0B77:
			if (CS$<>8__locals1.sourceFaction != null)
			{
				TIEffectsState.<ProcessInstantEffect>g__GrantMissedProjectToFaction|24_3(CS$<>8__locals1.sourceFaction);
				return;
			}
			TIFactionState[] array = GameStateManager.AllHumanFactions();
			for (int num20 = 0; num20 < array.Length; num20++)
			{
				TIEffectsState.<ProcessInstantEffect>g__GrantMissedProjectToFaction|24_3(array[num20]);
			}
			return;
			IL_0BFB:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState31 = enumerator.Current;
					foreach (TIFactionState tifactionState4 in GameStateManager.AllFactions())
					{
						if (tifactionState4 != tigameState31.ref_faction)
						{
							tifactionState4.GainIntel(tigameState31, value, null, false);
							TIFactionState ref_faction4 = tigameState31.ref_faction;
							foreach (TICouncilorState ticouncilorState in (((ref_faction4 != null) ? ref_faction4.councilors : null) ?? null))
							{
								if (TIUtilities.RandomFloatValue() < value)
								{
									tifactionState4.GainIntel(ticouncilorState, value, null, false);
								}
								if (value >= TemplateManager.global.intelToSeeCouncilorBasicData)
								{
									ticouncilorState.AddToParanoia(tifactionState4);
								}
							}
						}
					}
				}
				return;
			}
			IL_0CE4:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState32 = enumerator.Current;
					foreach (TIFactionState tifactionState5 in GameStateManager.AllFactions())
					{
						if (tifactionState5 != tigameState32.ref_councilor.faction)
						{
							tifactionState5.GainIntel(tigameState32, value, null, false);
							if (TIUtilities.RandomFloatValue() < value)
							{
								tifactionState5.GainIntel(tigameState32.ref_councilor, value, null, false);
							}
							if (value >= TemplateManager.global.intelToSeeCouncilorBasicData)
							{
								tigameState32.ref_councilor.AddToParanoia(tifactionState5);
							}
						}
					}
				}
				return;
			}
			IL_0D93:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState33 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState33.ref_nation.AddToDemocracy(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.DemocracyChangeReason.DemReason_EventEffect);
					}
					else
					{
						TIGameState secondaryStateForEffect = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState33, secondaryinputState);
						if (secondaryStateForEffect != null)
						{
							secondaryStateForEffect.ref_nation.AddToDemocracy(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.DemocracyChangeReason.DemReason_EventEffect);
						}
					}
				}
				return;
			}
			IL_0E03:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState34 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState34.ref_nation.AddToDemocracy(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tigameState34.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.DemocracyChangeReason.DemReason_EventEffect);
					}
					else
					{
						TIGameState secondaryStateForEffect2 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState34, secondaryinputState);
						if (secondaryStateForEffect2 != null)
						{
							secondaryStateForEffect2.ref_nation.AddToDemocracy(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * secondaryStateForEffect2.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.DemocracyChangeReason.DemReason_EventEffect);
						}
					}
				}
				return;
			}
			IL_0EBB:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState35 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState35.ref_nation.AddToCohesion(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.CohesionChangeReason.CohesionReason_Effect);
					}
					else
					{
						TIGameState secondaryStateForEffect3 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState35, secondaryinputState);
						if (secondaryStateForEffect3 != null)
						{
							secondaryStateForEffect3.ref_nation.AddToCohesion(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.CohesionChangeReason.CohesionReason_Effect);
						}
					}
				}
				return;
			}
			IL_0F2D:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState36 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState36.ref_nation.AddToCohesion(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tigameState36.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.CohesionChangeReason.CohesionReason_Effect);
					}
					else
					{
						TIGameState secondaryStateForEffect4 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState36, secondaryinputState);
						if (secondaryStateForEffect4 != null)
						{
							secondaryStateForEffect4.ref_nation.AddToCohesion(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * secondaryStateForEffect4.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.CohesionChangeReason.CohesionReason_Effect);
						}
					}
				}
				return;
			}
			IL_0FE7:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState37 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						if (tigameState37.ref_nation.cohesion <= 5f)
						{
							tigameState37.ref_nation.AddToCohesion(-1f * TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.CohesionChangeReason.CohesionReason_Effect);
						}
						else
						{
							tigameState37.ref_nation.AddToCohesion(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.CohesionChangeReason.CohesionReason_Effect);
						}
					}
					else
					{
						TIGameState secondaryStateForEffect5 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState37, secondaryinputState);
						TINationState tinationState2 = ((secondaryStateForEffect5 != null) ? secondaryStateForEffect5.ref_nation : null);
						if (tinationState2 != null)
						{
							if (tinationState2.cohesion <= 5f)
							{
								tinationState2.AddToCohesion(-1f * TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.CohesionChangeReason.CohesionReason_Effect);
							}
							else
							{
								tinationState2.AddToCohesion(1f * TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.CohesionChangeReason.CohesionReason_Effect);
							}
						}
					}
				}
				return;
			}
			IL_10D0:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState38 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						TINationState ref_nation4 = tigameState38.ref_nation;
						if (ref_nation4.cohesion <= 5f)
						{
							ref_nation4.AddToCohesion(Mathf.Clamp(-1f * TIEffectsState.RandomizedInstantEffectValue(value * ref_nation4.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.CohesionChangeReason.CohesionReason_Effect);
						}
						else
						{
							ref_nation4.AddToCohesion(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * ref_nation4.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.CohesionChangeReason.CohesionReason_Effect);
						}
					}
					else
					{
						TIGameState secondaryStateForEffect6 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState38, secondaryinputState);
						TINationState tinationState3 = ((secondaryStateForEffect6 != null) ? secondaryStateForEffect6.ref_nation : null);
						if (tinationState3 != null)
						{
							if (tinationState3.cohesion <= 5f)
							{
								tinationState3.AddToCohesion(Mathf.Clamp(-1f * TIEffectsState.RandomizedInstantEffectValue(value * tinationState3.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.CohesionChangeReason.CohesionReason_Effect);
							}
							else
							{
								tinationState3.AddToCohesion(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tinationState3.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.CohesionChangeReason.CohesionReason_Effect);
							}
						}
					}
				}
				return;
			}
			IL_1222:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState39 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState39.ref_nation.AddToUnrest(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.UnrestChangeReason.UnrestReason_EventEffect, 10f);
					}
					else
					{
						TIGameState secondaryStateForEffect7 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState39, secondaryinputState);
						if (secondaryStateForEffect7 != null)
						{
							secondaryStateForEffect7.ref_nation.AddToUnrest(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.UnrestChangeReason.UnrestReason_EventEffect, 10f);
						}
					}
				}
				return;
			}
			IL_129C:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState40 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState40.ref_nation.AddToUnrest(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tigameState40.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.UnrestChangeReason.UnrestReason_EventEffect, 10f);
					}
					else
					{
						TIGameState secondaryStateForEffect8 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState40, secondaryinputState);
						TINationState tinationState4 = ((secondaryStateForEffect8 != null) ? secondaryStateForEffect8.ref_nation : null);
						if (tinationState4 != null)
						{
							tinationState4.AddToUnrest(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tigameState40.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.UnrestChangeReason.UnrestReason_EventEffect, 10f);
						}
					}
				}
				return;
			}
			IL_136B:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState41 = enumerator.Current;
					if (value > 0f)
					{
						if (secondaryStateType == EffectSecondaryStateType.none)
						{
							tigameState41.ref_nation.IncreaseUnrest(CS$<>8__locals1.sourceFaction, TIEffectsState.RandomizedInstantEffectValue(value, randomizer), true, TINationState.UnrestChangeReason.UnrestReason_EventEffect);
						}
						else
						{
							TIGameState secondaryStateForEffect9 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState41, secondaryinputState);
							if (secondaryStateForEffect9 != null)
							{
								secondaryStateForEffect9.ref_nation.IncreaseUnrest(CS$<>8__locals1.sourceFaction, TIEffectsState.RandomizedInstantEffectValue(value, randomizer), true, TINationState.UnrestChangeReason.UnrestReason_EventEffect);
							}
						}
					}
					else if (value < 0f)
					{
						if (secondaryStateType == EffectSecondaryStateType.none)
						{
							tigameState41.ref_nation.StabilizeNation(CS$<>8__locals1.sourceFaction, Mathf.Abs(TIEffectsState.RandomizedInstantEffectValue(value, randomizer)), TINationState.UnrestChangeReason.UnrestReason_EventEffect);
						}
						else
						{
							TIGameState secondaryStateForEffect10 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState41, secondaryinputState);
							if (secondaryStateForEffect10 != null)
							{
								secondaryStateForEffect10.ref_nation.StabilizeNation(CS$<>8__locals1.sourceFaction, Mathf.Abs(TIEffectsState.RandomizedInstantEffectValue(value, randomizer)), TINationState.UnrestChangeReason.UnrestReason_EventEffect);
							}
						}
					}
				}
				return;
			}
			IL_145F:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState42 = enumerator.Current;
					if (value > 0f)
					{
						if (secondaryStateType == EffectSecondaryStateType.none)
						{
							tigameState42.ref_nation.IncreaseUnrest(CS$<>8__locals1.sourceFaction, Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tigameState42.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), true, TINationState.UnrestChangeReason.UnrestReason_EventEffect);
						}
						else
						{
							TIGameState secondaryStateForEffect11 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState42, secondaryinputState);
							TINationState tinationState5 = ((secondaryStateForEffect11 != null) ? secondaryStateForEffect11.ref_nation : null);
							if (tinationState5 != null)
							{
								tinationState5.IncreaseUnrest(CS$<>8__locals1.sourceFaction, Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tinationState5.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), true, TINationState.UnrestChangeReason.UnrestReason_EventEffect);
							}
						}
					}
					else if (value < 0f)
					{
						if (secondaryStateType == EffectSecondaryStateType.none)
						{
							tigameState42.ref_nation.StabilizeNation(CS$<>8__locals1.sourceFaction, Mathf.Clamp(Mathf.Abs(TIEffectsState.RandomizedInstantEffectValue(value * tigameState42.ref_nation.priorityEffectPopScaling, randomizer)), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.UnrestChangeReason.UnrestReason_EventEffect);
						}
						else
						{
							TINationState ref_nation5 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState42, secondaryinputState).ref_nation;
							if (ref_nation5 != null)
							{
								ref_nation5.StabilizeNation(CS$<>8__locals1.sourceFaction, Mathf.Clamp(Mathf.Abs(TIEffectsState.RandomizedInstantEffectValue(value * ref_nation5.priorityEffectPopScaling, randomizer)), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.UnrestChangeReason.UnrestReason_EventEffect);
							}
						}
					}
				}
				return;
			}
			IL_15E0:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState43 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState43.ref_nation.AddToInequality(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.InequalityChangeReason.InqReason_EventEffects);
					}
					else
					{
						TIGameState secondaryStateForEffect12 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState43, secondaryinputState);
						if (secondaryStateForEffect12 != null)
						{
							secondaryStateForEffect12.ref_nation.AddToInequality(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.InequalityChangeReason.InqReason_EventEffects);
						}
					}
				}
				return;
			}
			IL_164E:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState44 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState44.ref_nation.AddToInequality(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tigameState44.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.InequalityChangeReason.InqReason_EventEffects);
					}
					else
					{
						TIGameState secondaryStateForEffect13 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState44, secondaryinputState);
						TINationState tinationState6 = ((secondaryStateForEffect13 != null) ? secondaryStateForEffect13.ref_nation : null);
						if (tinationState6 != null)
						{
							tinationState6.AddToInequality(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tinationState6.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.InequalityChangeReason.InqReason_EventEffects);
						}
					}
				}
				return;
			}
			IL_170C:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState45 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState45.ref_nation.AddToEducation(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.EducationChangeReason.EducationReason_EventEffect);
					}
					else
					{
						TIGameState secondaryStateForEffect14 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState45, secondaryinputState);
						if (secondaryStateForEffect14 != null)
						{
							secondaryStateForEffect14.ref_nation.AddToEducation(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.EducationChangeReason.EducationReason_EventEffect);
						}
					}
				}
				return;
			}
			IL_177A:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState46 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState46.ref_nation.AddToEducation(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tigameState46.ref_nation.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.EducationChangeReason.EducationReason_EventEffect);
					}
					else
					{
						TIGameState secondaryStateForEffect15 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState46, secondaryinputState);
						TINationState tinationState7 = ((secondaryStateForEffect15 != null) ? secondaryStateForEffect15.ref_nation : null);
						if (tinationState7 != null)
						{
							tinationState7.AddToEducation(Mathf.Clamp(TIEffectsState.RandomizedInstantEffectValue(value * tinationState7.priorityEffectPopScaling, randomizer), TIEffectsState.MinScaledTenPointStatEffect(value), TIEffectsState.MaxScaledTenPointStatEffect(value)), TINationState.EducationChangeReason.EducationReason_EventEffect);
						}
					}
				}
				return;
			}
			IL_1832:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState47 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState47.ref_nation.GDPPctChange(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.GDPChangeReason.GDPReason_EventEffect);
					}
					else
					{
						TIGameState secondaryStateForEffect16 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState47, secondaryinputState);
						if (secondaryStateForEffect16 != null)
						{
							secondaryStateForEffect16.ref_nation.GDPPctChange(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.GDPChangeReason.GDPReason_EventEffect);
						}
					}
				}
				return;
			}
			IL_18A2:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState48 = enumerator.Current;
					TIRegionState tiregionState4;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tiregionState4 = tigameState48.ref_region;
					}
					else
					{
						TIGameState secondaryStateForEffect17 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState48, secondaryinputState);
						tiregionState4 = ((secondaryStateForEffect17 != null) ? secondaryStateForEffect17.ref_region : null);
					}
					if (tiregionState4 != null)
					{
						tiregionState4.ref_nation.GDPPctChange(TIEffectsState.RandomizedInstantEffectValue(value, randomizer) * tiregionState4.NationalGDPProportion(), TINationState.GDPChangeReason.GDPReason_EventEffect);
					}
				}
				return;
			}
			IL_1920:
			if (TIUtilities.GetTemplateValue<TIRegionTemplate>(strValue) != null)
			{
				TIRegionState tiregionState5 = GameStateManager.RegionLookup()[strValue];
				tiregionState5.nation.GDPPctChange(TIEffectsState.RandomizedInstantEffectValue(value, randomizer) * tiregionState5.NationalGDPProportion(), TINationState.GDPChangeReason.GDPReason_EventEffect);
				return;
			}
			return;
			IL_1A05:
			if (!(CS$<>8__locals1.sourceFaction != null))
			{
				return;
			}
			using (List<TINationState>.Enumerator enumerator5 = CS$<>8__locals1.sourceFaction.executiveNations.GetEnumerator())
			{
				while (enumerator5.MoveNext())
				{
					TINationState tinationState8 = enumerator5.Current;
					tinationState8.GDPPctChange(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), TINationState.GDPChangeReason.GDPReason_EventEffect);
				}
				return;
			}
			IL_1A5D:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState49 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState49.ref_nation.AddToMilitaryTechLevel(TIEffectsState.RandomizedInstantEffectValue(value, randomizer));
					}
					else
					{
						TIGameState secondaryStateForEffect18 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState49, secondaryinputState);
						if (secondaryStateForEffect18 != null)
						{
							secondaryStateForEffect18.ref_nation.AddToMilitaryTechLevel(TIEffectsState.RandomizedInstantEffectValue(value, randomizer));
						}
					}
				}
				return;
			}
			IL_1AC9:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState50 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						float num21 = tigameState50.ref_nation.militaryTechLevel - TIUtilities.GetFloatValue(strValue);
						if (num21 > 0f)
						{
							float num22 = num21 * TIEffectsState.RandomizedInstantEffectValue(value, randomizer);
							tigameState50.ref_nation.AddToMilitaryTechLevel(-num22);
						}
					}
					else
					{
						TIGameState secondaryStateForEffect19 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState50, secondaryinputState);
						if (secondaryStateForEffect19 != null)
						{
							float num23 = secondaryStateForEffect19.ref_nation.militaryTechLevel - TIUtilities.GetFloatValue(strValue);
							if (num23 > 0f)
							{
								float num24 = num23 * TIEffectsState.RandomizedInstantEffectValue(value, randomizer);
								secondaryStateForEffect19.ref_nation.AddToMilitaryTechLevel(-num24);
							}
						}
					}
				}
				return;
			}
			IL_1B91:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState51 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState51.ref_nation.AddToMaxMilitaryTechLevel(value);
					}
				}
				return;
			}
			IL_1BD1:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState52 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState52.ref_nation.ActivateBuildSpaceDefenses();
					}
				}
				return;
			}
			IL_1C0F:
			foreach (TIGameState tigameState53 in list)
			{
				if (secondaryStateType == EffectSecondaryStateType.none)
				{
					tigameState53.ref_nation.ActivateBuildSTOSquadron();
				}
			}
			array = GameStateManager.AllHumanFactions();
			for (int num20 = 0; num20 < array.Length; num20++)
			{
				array[num20].CacheSTOFighterMass();
			}
			return;
			IL_1CAD:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState54 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState54.ref_nation.ChangeNumNuclearWeapons((int)value);
					}
					else
					{
						TIGameState secondaryStateForEffect20 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState54, secondaryinputState);
						if (secondaryStateForEffect20 != null)
						{
							secondaryStateForEffect20.ref_nation.ChangeNumNuclearWeapons((int)value);
						}
					}
				}
				return;
			}
			IL_1D0D:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState55 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState55.ref_nation.ChangeAnnualSpaceFundingValue(value);
					}
					else
					{
						TIGameState secondaryStateForEffect21 = TIEffectsState.GetSecondaryStateForEffect(secondaryStateType, tigameState55, secondaryinputState);
						if (secondaryStateForEffect21 != null)
						{
							secondaryStateForEffect21.ref_nation.ChangeAnnualSpaceFundingValue(value);
						}
					}
				}
				return;
			}
			IL_1D6B:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState56 = enumerator.Current;
					foreach (TIRegionState tiregionState6 in tigameState56.ref_nation.regions)
					{
						tiregionState6.ChangeAnnualPopulationGrowthModifier(value);
					}
				}
				return;
			}
			IL_1DD8:
			GameStateManager.GlobalValues().AddCH4_ppm(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), GHGSources.Effect);
			return;
			IL_1E94:
			if (!(CS$<>8__locals1.sourceFaction != null))
			{
				return;
			}
			TIRegionState ref_region4 = inputState.ref_region;
			CS$<>8__locals1.sourceFaction.GainIntel(ref_region4, 1f, null, false);
			using (List<TICouncilorState>.Enumerator enumerator4 = GameStateManager.AlienFaction().councilors.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					TICouncilorState ticouncilorState2 = enumerator4.Current;
					if (ticouncilorState2.OnEarth && (ticouncilorState2.ref_region == ref_region4 || ticouncilorState2.ref_region.IsAdjacent(ref_region4, false)))
					{
						TIMissionState timissionState = ticouncilorState2.activeMission;
						if (timissionState == null)
						{
							timissionState = ticouncilorState2.completedMission;
						}
						if (CS$<>8__locals1.sourceFaction.CanDetectAlienMission(timissionState.missionTemplate))
						{
							ref_region4.alienActivity.ActivitySightedByFaction(CS$<>8__locals1.sourceFaction, timissionState.missionTemplate, timissionState.target.ref_councilor, timissionState.target.ref_faction, timissionState);
						}
					}
				}
				return;
			}
			IL_1F90:
			TIFactionState sourceFaction14 = CS$<>8__locals1.sourceFaction;
			if (sourceFaction14 == null)
			{
				return;
			}
			sourceFaction14.CommitAtrocity((int)value, TIFactionState.AtrocityCause.EventEffect, false, 0.333f);
			return;
			IL_2352:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState57 = enumerator.Current;
					if (tigameState57.ref_army.homeNation.controlPoints[tigameState57.ref_army.controlPointIdx].faction != null)
					{
						tigameState57.ref_army.homeNation.controlPoints[tigameState57.ref_army.controlPointIdx].ResolveCrackdownEffect((int)value, null, false, false, 0f);
					}
				}
				return;
			}
			IL_23E3:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState58 = enumerator.Current;
					foreach (TIControlPoint ticontrolPoint6 in tigameState58.ref_nation.controlPoints)
					{
						if (ticontrolPoint6.owned)
						{
							ticontrolPoint6.ResolveDefendControlPointEffect((int)(value * 30.436874f));
						}
					}
				}
				return;
			}
			IL_245F:
			TIFactionState sourceFaction15 = CS$<>8__locals1.sourceFaction;
			if (sourceFaction15 == null)
			{
				return;
			}
			sourceFaction15.ChangeBaseResourceIncome(FactionResource.Money, value * 12f);
			return;
			IL_260A:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState59 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none && CS$<>8__locals1.sourceFaction != null)
					{
						tigameState59.ref_nation.DowngradeRelations(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_nation);
					}
				}
				return;
			}
			IL_2663:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState60 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none && CS$<>8__locals1.sourceFaction != null)
					{
						tigameState60.ref_nation.DeclareLimitedWar(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_nation);
					}
				}
				return;
			}
			IL_26BC:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState61 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none && CS$<>8__locals1.sourceFaction != null)
					{
						tigameState61.ref_nation.DeclareFullWar(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_nation);
					}
				}
				return;
			}
			IL_2715:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState62 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none && CS$<>8__locals1.sourceFaction != null)
					{
						if (secondaryinputState.ref_nation.inFederation)
						{
							secondaryinputState.ref_nation.federation.RemoveNation(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_nation, false);
						}
						if (!tigameState62.ref_nation.inFederation)
						{
							tigameState62.ref_nation.FormFederation(secondaryinputState.ref_nation);
						}
						else
						{
							tigameState62.ref_nation.federation.AddNation(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_nation, false);
						}
					}
				}
				return;
			}
			IL_27CA:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState63 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none && CS$<>8__locals1.sourceFaction != null)
					{
						if (tigameState63.ref_nation.inFederation)
						{
							tigameState63.ref_nation.federation.RemoveNation(CS$<>8__locals1.sourceFaction, tigameState63.ref_nation, false);
						}
						if (!secondaryinputState.ref_nation.inFederation)
						{
							secondaryinputState.ref_nation.FormFederation(tigameState63.ref_nation);
						}
						else
						{
							secondaryinputState.ref_nation.federation.AddNation(CS$<>8__locals1.sourceFaction, tigameState63.ref_nation, false);
						}
					}
				}
				return;
			}
			IL_287F:
			if (secondaryStateType == EffectSecondaryStateType.none)
			{
				using (List<TIRegionState>.Enumerator enumerator6 = inputState.ref_nation.regions.GetEnumerator())
				{
					while (enumerator6.MoveNext())
					{
						TIRegionState tiregionState7 = enumerator6.Current;
						tiregionState7.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -tiregionState7.boostPerYear_dekatons * value, false, false);
						tiregionState7.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, (float)(-(float)tiregionState7.missionControl) * value, false, false);
						if (TIUtilities.RandomFloatValue() < value)
						{
							tiregionState7.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, false);
						}
					}
					return;
				}
			}
			using (List<TIRegionState>.Enumerator enumerator6 = secondaryinputState.ref_nation.regions.GetEnumerator())
			{
				while (enumerator6.MoveNext())
				{
					TIRegionState tiregionState8 = enumerator6.Current;
					tiregionState8.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -tiregionState8.boostPerYear_dekatons * value, false, false);
					tiregionState8.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, (float)(-(float)tiregionState8.missionControl) * value, false, false);
					if (TIUtilities.RandomFloatValue() < value)
					{
						tiregionState8.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, false);
					}
				}
				return;
			}
			IL_2985:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState64 = enumerator.Current;
					float num25 = tigameState64.ref_region.populationInMillions * (1f + TIEffectsState.RandomizedInstantEffectValue(value, randomizer));
					tigameState64.ref_region.ChangePopulation_Millions(num25 - tigameState64.ref_region.populationInMillions, true);
				}
				return;
			}
			IL_29EE:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState65 = enumerator.Current;
					float num26 = TIEffectsState.RandomizedInstantEffectValue(value, randomizer);
					if (num26 < 0f)
					{
						float num27 = Mathf.Max(1f, 3500f * tigameState65.ref_nation.inequality);
						float num28 = (float)tigameState65.ref_region.regionalPerCapitaGDP / num27;
						num26 = Mathf.Clamp(num26 / num28, value, 0f);
					}
					float num29 = tigameState65.ref_region.populationInMillions * (1f + num26);
					tigameState65.ref_region.ChangePopulation_Millions(num29 - tigameState65.ref_region.populationInMillions, true);
				}
				return;
			}
			IL_2AAD:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState66 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						float num30 = tigameState66.ref_region.populationInMillions * TIEffectsState.RandomizedInstantEffectValue(value, randomizer);
						tigameState66.ref_region.ChangePopulation_Millions(-num30, true);
						secondaryinputState.ref_region.ChangePopulation_Millions(num30, true);
					}
				}
				return;
			}
			IL_2B16:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState67 = enumerator.Current;
					tigameState67.ref_region.ConductAbductions(GameStateManager.AlienFaction(), (int)value);
				}
				return;
			}
			IL_2B55:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState68 = enumerator.Current;
					foreach (TIRegionState tiregionState9 in tigameState68.ref_nation.regions)
					{
						tiregionState9.ConductAbductions(GameStateManager.AlienFaction(), (int)value);
					}
				}
				return;
			}
			IL_2BC2:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState69 = enumerator.Current;
					tigameState69.ref_region.ChangeNuclearDetonations((int)value);
				}
				return;
			}
			IL_2C02:
			if (CS$<>8__locals1.sourceFaction != null)
			{
				TIOrgTemplate tiorgTemplate = TemplateManager.Find<TIOrgTemplate>(CS$<>8__locals1.sourceFaction.template.spaceOrg, false);
				CS$<>8__locals1.sourceFaction.CreateOrTransferOrgToFactionPool(tiorgTemplate, true);
				return;
			}
			return;
			IL_2C83:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState70 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						tigameState70.ref_region.IncreaseOccupationValue(secondaryinputState.ref_nation, value, null);
					}
				}
				return;
			}
			IL_2CCB:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState71 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						secondaryinputState.ref_nation.RegimeChange(tigameState71.ref_nation, new List<TINationState> { tigameState71.ref_nation }, tigameState71.ref_faction, false);
					}
				}
				return;
			}
			IL_2D2A:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState72 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						tigameState72.ref_nation.RegimeChange(secondaryinputState.ref_nation, new List<TINationState> { secondaryinputState.ref_nation }, secondaryinputState.ref_faction, false);
					}
				}
				return;
			}
			IL_2D89:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState73 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						tigameState73.ref_nation.AbsorbNation(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_nation);
					}
				}
				return;
			}
			IL_2DD4:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState74 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						secondaryinputState.ref_nation.AbsorbNation(CS$<>8__locals1.sourceFaction, tigameState74.ref_nation);
					}
				}
				return;
			}
			IL_2E1F:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState75 = enumerator.Current;
					if (secondaryinputState.ref_nation == GameStateManager.AlienNation())
					{
						if (GameStateManager.AlienNation().extant)
						{
							GameStateManager.AlienNation().AnnexNation(GameStateManager.AlienFaction(), tigameState75.ref_nation, false);
						}
						else
						{
							GameStateManager.AlienNation().AnnexNation(GameStateManager.AlienFaction(), tigameState75.ref_nation, true);
						}
					}
				}
				return;
			}
			IL_2E9D:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState76 = enumerator.Current;
					if (CS$<>8__locals1.sourceFaction != null)
					{
						if (secondaryStateType == EffectSecondaryStateType.none)
						{
							tigameState76.ref_nation.Secession(CS$<>8__locals1.sourceFaction, tigameState76.ref_region.SecessionCandidates().SelectRandomItem<TINationState>(), new List<TIRegionState> { tigameState76.ref_region }, null);
						}
						else
						{
							secondaryinputState.ref_nation.Secession(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_region.SecessionCandidates().SelectRandomItem<TINationState>(), new List<TIRegionState> { secondaryinputState.ref_region }, null);
						}
					}
				}
				return;
			}
			IL_2F51:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState77 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState77.ref_nation.SunderNation(tigameState77.ref_nation.HighestUnrestContributor(), tigameState77.ref_nation, tigameState77.ref_nation.regions, 1f, ControlPointChangeCause.Independence);
					}
					else
					{
						secondaryinputState.ref_nation.SunderNation(secondaryinputState.ref_nation.HighestUnrestContributor(), tigameState77.ref_nation, secondaryinputState.ref_nation.regions, 1f, ControlPointChangeCause.Independence);
					}
				}
				return;
			}
			IL_2FE7:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState78 = enumerator.Current;
					if (secondaryStateType == EffectSecondaryStateType.none)
					{
						tigameState78.ref_nation.Coup(null, 0);
					}
					else
					{
						tigameState78.ref_nation.Coup(secondaryinputState.ref_faction.councilors.SelectRandomItem<TICouncilorState>(), (int)value);
					}
				}
				return;
			}
			IL_3049:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState79 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						TIRegionState ref_region5 = secondaryinputState.ref_region;
						if (tigameState79.ref_nation.claims.Contains(ref_region5))
						{
							tigameState79.ref_nation.RemoveClaim(ref_region5);
						}
					}
				}
				return;
			}
			IL_30A7:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState80 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						TIRegionState ref_region6 = secondaryinputState.ref_region;
						if (tigameState80.ref_nation.regions.Contains(ref_region6))
						{
							tigameState80.ref_nation.SetCapital(ref_region6);
							if (ref_region6.colonyRegion)
							{
								ref_region6.colonyRegion = false;
							}
						}
					}
				}
				return;
			}
			IL_3116:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState81 = enumerator.Current;
					int num31 = 0;
					while ((float)num31 < value)
					{
						TIHabModuleState tihabModuleState = tigameState81.ref_hab.SelectModuleToDestroy();
						if (tihabModuleState != null)
						{
							tigameState81.ref_hab.DestroyModule(CS$<>8__locals1.sourceFaction, tihabModuleState, false, false, true, 0f, false, false);
						}
						num31++;
					}
				}
				return;
			}
			IL_3194:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState82 = enumerator.Current;
					int num32 = 0;
					while ((float)num32 < value)
					{
						TIHabModuleState tihabModuleState2 = tigameState82.ref_hab.SelectModuleToDestroy_Marines();
						if (tihabModuleState2 != null)
						{
							tigameState82.ref_hab.DestroyModule(CS$<>8__locals1.sourceFaction, tihabModuleState2, false, false, true, 0f, false, false);
						}
						num32++;
					}
				}
				return;
			}
			IL_3212:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState83 = enumerator.Current;
					int num33 = 0;
					while ((float)num33 < value)
					{
						TIHabModuleState tihabModuleState3 = tigameState83.ref_hab.SelectModuleToDestroy_Power();
						if (tihabModuleState3 != null)
						{
							tigameState83.ref_hab.DestroyModule(CS$<>8__locals1.sourceFaction, tihabModuleState3, false, false, true, 0f, false, false);
						}
						num33++;
					}
				}
				return;
			}
			IL_3290:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState84 = enumerator.Current;
					int num34 = 0;
					while ((float)num34 < TIUtilities.RandomRange(0f, value))
					{
						TIHabState ref_hab = tigameState84.ref_hab;
						TIHabModuleState tihabModuleState4 = ((ref_hab != null) ? ref_hab.SelectModuleToDestroy() : null);
						if (tihabModuleState4 != null)
						{
							tigameState84.ref_hab.DestroyModule(CS$<>8__locals1.sourceFaction, tihabModuleState4, false, false, true, 0f, false, false);
						}
						num34++;
					}
				}
				return;
			}
			IL_331F:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState85 = enumerator.Current;
					foreach (TIHabModuleState tihabModuleState5 in tigameState85.ref_hab.activeSectors.SelectRandomItem<TISectorState>().OkayModules())
					{
						if (!tihabModuleState5.moduleTemplate.coreModule && !tihabModuleState5.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.AlienWormhole))
						{
							tigameState85.ref_hab.DestroyModule(CS$<>8__locals1.sourceFaction, tihabModuleState5, false, false, true, 0f, false, false);
						}
					}
				}
				return;
			}
			IL_33DD:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState86 = enumerator.Current;
					tigameState86.ref_hab.DestroyHab(CS$<>8__locals1.sourceFaction, 0f, false, null, 0f);
				}
				return;
			}
			IL_342C:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState87 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						tigameState87.ref_hab.CaptureHab(secondaryinputState.ref_faction, 2, false, true, null, null);
					}
				}
				return;
			}
			IL_3477:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState88 = enumerator.Current;
					tigameState88.ref_ship.DestroyShip(false, CS$<>8__locals1.sourceFaction);
				}
				return;
			}
			IL_34BB:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState89 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						TISpaceShipState ref_ship = tigameState89.ref_ship;
						TIFactionState faction2 = ref_ship.faction;
						TIFactionState ref_faction5 = secondaryinputState.ref_faction;
						TISpaceFleetState.CreateAtRunTime(ref_faction5, new List<TISpaceShipState> { ref_ship }, ref_ship.fleet, ref_ship.fleet, null, false, false, null);
						TINotificationQueueState.LogOurShipChangedSides(ref_ship, faction2, ref_faction5);
						TINotificationQueueState.LogEnemyShipChangedSidesToUs(ref_ship, faction2, ref_faction5);
					}
				}
				return;
			}
			IL_3545:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState90 = enumerator.Current;
					foreach (ModuleDataEntry moduleDataEntry in tigameState90.ref_ship.utilityModules)
					{
						TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
						if (ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Assault))
						{
							float num35;
							tigameState90.ref_ship.ApplyDamageToPart(moduleDataEntry, value, out num35);
						}
					}
					tigameState90.ref_ship.PostCombat(true);
				}
				return;
			}
			IL_35F7:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState91 = enumerator.Current;
					SpecialModuleRule specialModuleRule = strValue.ToEnum(SpecialModuleRule.None);
					foreach (ModuleDataEntry moduleDataEntry2 in tigameState91.ref_ship.utilityModules)
					{
						TIUtilityModuleTemplate ref_utilityModule2 = moduleDataEntry2.moduleTemplate.ref_utilityModule;
						if (ref_utilityModule2 != null && ref_utilityModule2.specialModuleRules.Contains(specialModuleRule))
						{
							float num36;
							tigameState91.ref_ship.ApplyDamageToPart(moduleDataEntry2, value, out num36);
						}
					}
					tigameState91.ref_ship.PostCombat(true);
				}
				return;
			}
			IL_36B4:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState target2 = enumerator.Current;
					List<ModuleDataEntry> list3 = target2.ref_ship.NuclearWeaponModuleData();
					if (list3.Count > 0)
					{
						if (value < 0f)
						{
							ModuleDataEntry moduleDataEntry3 = list3.Where<ModuleDataEntry>((ModuleDataEntry x) => target2.ref_ship.WeaponHasAmmo(x)).SelectRandomItem<ModuleDataEntry>();
							target2.ref_ship.ChangeAmmoValue(moduleDataEntry3, (int)value);
						}
						else
						{
							ModuleDataEntry moduleDataEntry4 = list3.Where<ModuleDataEntry>((ModuleDataEntry x) => !target2.ref_ship.WeaponHasAmmo(x)).SelectRandomItem<ModuleDataEntry>();
							target2.ref_ship.ChangeAmmoValue(moduleDataEntry4, (int)value);
						}
					}
				}
				return;
			}
			IL_3785:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState92 = enumerator.Current;
					tigameState92.ref_ship.FreeOfficerCreationEvent((int)value);
				}
				return;
			}
			IL_37C6:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState93 = enumerator.Current;
					(from x in tigameState93.ref_ship.GetOfficers()
						where x.maxRank < 3
						select x).SelectRandomItem<TIOfficerState>().Promote();
				}
				return;
			}
			IL_3832:
			TISpaceFleetTemplate templateValue = TIUtilities.GetTemplateValue<TISpaceFleetTemplate>(strValue);
			if (templateValue != null)
			{
				TISpaceFleetState tispaceFleetState = new TISpaceFleetState();
				tispaceFleetState.Initialize();
				tispaceFleetState.InitWithTemplate(templateValue);
				tispaceFleetState.PostGameStateCreateInit_OnCreationOnly_1();
				tispaceFleetState.PostGlobalGameStateCreateInit_2();
				return;
			}
			return;
			IL_38B6:
			if (!(CS$<>8__locals1.sourceFaction != null))
			{
				return;
			}
			using (HashSet<FactionResource>.Enumerator enumerator10 = TIResourcesCost.basicSpaceResources.GetEnumerator())
			{
				while (enumerator10.MoveNext())
				{
					FactionResource factionResource = enumerator10.Current;
					CS$<>8__locals1.sourceFaction.AddToCurrentResource(Mathf.Max(CS$<>8__locals1.sourceFaction.GetDailyIncome(factionResource, false, false) * value, 0f), factionResource, false, "Effect.FreeDaysSpaceResourceIncome");
				}
				return;
			}
			IL_392A:
			if (!(CS$<>8__locals1.sourceFaction != null))
			{
				return;
			}
			using (HashSet<FactionResource>.Enumerator enumerator10 = TIResourcesCost.basicSpaceResources.GetEnumerator())
			{
				while (enumerator10.MoveNext())
				{
					FactionResource factionResource2 = enumerator10.Current;
					CS$<>8__locals1.sourceFaction.SubtractFromCurrentResource(Mathf.Max(CS$<>8__locals1.sourceFaction.GetDailyIncome(factionResource2, false, false) * value, 0f), factionResource2, false, "Effect.LoseDaysSpaceResourceIncome");
				}
				return;
			}
			IL_399E:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState94 = enumerator.Current;
					if (tigameState94.ref_habSite != null)
					{
						tigameState94.ref_habSite.ModifySiteMiningData(TIEffectsState.RandomizedInstantEffectValue(value, randomizer));
						TIHabState hab = tigameState94.ref_habSite.hab;
						if (hab != null)
						{
							hab.UpdateCurrentAnnualNetResourceIncomes(false);
						}
					}
				}
				return;
			}
			IL_3A0F:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState95 = enumerator.Current;
					TIHabState ref_hab2 = tigameState95.ref_hab;
					if (ref_hab2 != null && ref_hab2.HasMineFunctional)
					{
						TIResourcesCost tiresourcesCost = new TIResourcesCost();
						foreach (FactionResource factionResource3 in TIResourcesCost.basicSpaceResources)
						{
							tiresourcesCost.AddCost(factionResource3, tigameState95.ref_hab.mine.moduleTemplate.GetMiningIncome_Month(tigameState95.ref_hab.coreFaction, tigameState95.ref_hab.habSite, factionResource3), true);
						}
						tiresourcesCost.MultiplyCost(value);
						tiresourcesCost.RefundCost(tigameState95.ref_hab.coreFaction, null);
					}
				}
				return;
			}
			IL_3AF1:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState96 = enumerator.Current;
					if (tigameState96.ref_orbit != null)
					{
						tigameState96.ref_orbit.DestroyedAssetsChange((int)value);
					}
				}
				return;
			}
			IL_3B44:
			IEnumerable<TIOrbitState> enumerable6 = GameStateManager.Earth().orbits.Where<TIOrbitState>((TIOrbitState x) => x.isEarthLEO);
			if (value > 0f)
			{
				for (int num37 = (int)value; num37 > 0; num37--)
				{
					enumerable6.SelectRandomItem<TIOrbitState>().DestroyedAssetsChange(1);
				}
				return;
			}
			for (int num38 = (int)value; num38 < 0; num38++)
			{
				IEnumerable<TIOrbitState> enumerable7 = enumerable6.Where<TIOrbitState>((TIOrbitState x) => x.destroyedAssets > 0);
				if (enumerable7.Count<TIOrbitState>() <= 0)
				{
					return;
				}
				enumerable7.SelectRandomItem<TIOrbitState>().DestroyedAssetsChange(-1);
			}
			return;
			IL_3C45:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState97 = enumerator.Current;
					tigameState97.ref_xenoforming.SpawnMegafaunaArmy();
					foreach (TIArmyState tiarmyState in tigameState97.ref_region.MegafaunaArmiesPresent().ToList<TIArmyState>())
					{
						tiarmyState.TakeDamage(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), tigameState97.ref_faction, null, false);
					}
				}
				return;
			}
			IL_3CD8:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState98 = enumerator.Current;
					foreach (FactionResource factionResource4 in TIResourcesCost.basicSpaceResources)
					{
						float num39 = value / AIEvaluators.GetAIRelativeValuation(factionResource4);
						float dailyIncome = GameStateManager.AlienFaction().GetDailyIncome(factionResource4, false, false);
						if (num39 > dailyIncome * 0.35f)
						{
							num39 = dailyIncome * 0.35f;
						}
						if (num39 > 0f)
						{
							GameStateManager.AlienFaction().AddDailyResourceTransfer(tigameState98.ref_faction, factionResource4, num39, null, false);
						}
					}
				}
				return;
			}
			IL_3D8F:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState99 = enumerator.Current;
					tigameState99.ref_councilor.ChangeXP((int)value);
				}
				return;
			}
			IL_3DCF:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState100 = enumerator.Current;
					tigameState100.ref_councilor.AddTrait(strValue);
				}
				return;
			}
			IL_3E0E:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState101 = enumerator.Current;
					foreach (TICouncilorState ticouncilorState3 in tigameState101.ref_faction.councilors)
					{
						ticouncilorState3.AddTrait(strValue);
					}
				}
				return;
			}
			IL_3E7B:
			List<TIGameState> list4 = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates);
			CouncilorAttribute councilorAttribute = strValue.ToEnum(CouncilorAttribute.None);
			using (List<TIGameState>.Enumerator enumerator = list4.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState102 = enumerator.Current;
					foreach (TICouncilorState ticouncilorState4 in tigameState102.ref_faction.councilors)
					{
						ticouncilorState4.ModifyAttribute(councilorAttribute, (int)value);
					}
					foreach (TICouncilorState ticouncilorState5 in tigameState102.ref_faction.availableCouncilors)
					{
						ticouncilorState5.ModifyAttribute(councilorAttribute, (int)value);
					}
				}
				return;
			}
			IL_3F40:
			TITraitTemplate titraitTemplate = TemplateManager.Find<TITraitTemplate>(strValue, false);
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState103 = enumerator.Current;
					foreach (TICouncilorState ticouncilorState6 in tigameState103.ref_faction.councilors)
					{
						if (titraitTemplate.CouncilorCanHave(ticouncilorState6, tigameState103.ref_faction, false))
						{
							ticouncilorState6.AddTrait(titraitTemplate, false);
						}
					}
					foreach (TICouncilorState ticouncilorState7 in tigameState103.ref_faction.availableCouncilors)
					{
						if (titraitTemplate.CouncilorCanHave(ticouncilorState7, tigameState103.ref_faction, false))
						{
							ticouncilorState7.AddTrait(titraitTemplate, false);
						}
					}
				}
				return;
			}
			IL_402D:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState104 = enumerator.Current;
					foreach (TICouncilorState ticouncilorState8 in tigameState104.ref_faction.councilors)
					{
						ticouncilorState8.ChangeXP((int)value);
					}
					foreach (TICouncilorState ticouncilorState9 in tigameState104.ref_faction.availableCouncilors)
					{
						ticouncilorState9.ChangeXP((int)value);
					}
				}
				return;
			}
			IL_40E2:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState105 = enumerator.Current;
					if (tigameState105.ref_faction.UnlockedExotics)
					{
						float num40 = 0f;
						foreach (TIHabModuleState tihabModuleState6 in tigameState105.ref_faction.activeHabModules)
						{
							if (tihabModuleState6.moduleTemplate.constructionModule && tihabModuleState6.moduleTemplate.GetTechBonusByCategory(TechCategory.Materials) > 0f)
							{
								num40 += tihabModuleState6.moduleTemplate.GetTechBonusByCategory(TechCategory.Materials);
							}
						}
						num40 *= value;
						tigameState105.ref_faction.AddToCurrentResource(num40, FactionResource.Exotics, false, "Effect.GainExoticsFromSpaceIndustry");
					}
					else
					{
						tigameState105.ref_faction.AddAvailableProject("Project_Exotics");
					}
				}
				return;
			}
			IL_41D2:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState106 = enumerator.Current;
					TITraitTemplate templateValue2 = TIUtilities.GetTemplateValue<TITraitTemplate>(strValue);
					if (tigameState106.ref_councilor.traits.Contains(templateValue2))
					{
						tigameState106.ref_councilor.RemoveTrait(templateValue2);
					}
				}
				return;
			}
			IL_4234:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState target = enumerator.Current;
					TITraitTemplate titraitTemplate2 = (from x in TICouncilorState.GetAllTraitsOfGrouping((int)value)
						where !target.ref_councilor.traits.Contains(x)
						select x).SelectRandomItem<TITraitTemplate>();
					target.ref_councilor.AddTrait(titraitTemplate2, false);
				}
				return;
			}
			IL_42AA:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState107 = enumerator.Current;
					TITraitTemplate traitGrouping = tigameState107.ref_councilor.GetTraitGrouping((int)value);
					tigameState107.ref_councilor.RemoveTrait(traitGrouping);
				}
				return;
			}
			IL_42FA:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState108 = enumerator.Current;
					if (tigameState108.ref_faction != null)
					{
						tigameState108.ref_councilor.DetainCouncilor(tigameState108.ref_faction, value, randomizer, false);
					}
				}
				return;
			}
			IL_4357:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState109 = enumerator.Current;
					if (tigameState109.ref_councilor.GetProtectors().Count<TICouncilorState>() == 0)
					{
						tigameState109.ref_councilor.KillCouncilor(true, null);
					}
				}
				return;
			}
			IL_43AD:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState110 = enumerator.Current;
					tigameState110.ref_councilor.KillCouncilor(true, null);
				}
				return;
			}
			IL_43EC:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState111 = enumerator.Current;
					tigameState111.ref_councilor.KillCouncilor(false, null);
				}
				return;
			}
			IL_442B:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState112 = enumerator.Current;
					TIMissionEffect_GoToGround.ApplyEffect_Static(tigameState112.ref_councilor);
				}
				return;
			}
			IL_4462:
			List<TIGameState> list5 = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates);
			CouncilorAttribute councilorAttribute2 = strValue.ToEnum(CouncilorAttribute.None);
			using (List<TIGameState>.Enumerator enumerator = list5.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState113 = enumerator.Current;
					tigameState113.ref_councilor.ModifyAttribute(councilorAttribute2, (int)value);
				}
				return;
			}
			IL_44AF:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState114 = enumerator.Current;
					tigameState114.ref_councilor.homeNation.PropagandaOnPop(CS$<>8__locals1.sourceFaction.ideology, TIEffectsState.RandomizedInstantEffectValue(value, randomizer), false);
				}
				return;
			}
			IL_4507:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState115 = enumerator.Current;
					if (secondaryStateType != EffectSecondaryStateType.none)
					{
						tigameState115.ref_councilor.homeNation.UpgradeRelations(CS$<>8__locals1.sourceFaction, secondaryinputState.ref_councilor.homeNation);
					}
				}
				return;
			}
			IL_455C:
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState116 = enumerator.Current;
					int num41 = 0;
					while ((float)num41 < value)
					{
						if (TIUtilities.RandomFloatValue() < randomizer)
						{
							List<TIOrgState> loseableOrgs = tigameState116.ref_councilor.GetLoseableOrgs();
							if (loseableOrgs.Count > 0)
							{
								if (secondaryStateType == EffectSecondaryStateType.none)
								{
									TIOrgState tiorgState = loseableOrgs.SelectRandomWeightedItem<TIOrgState>((TIOrgState x) => (float)(4 - x.tier), -1f, 1E-37f);
									tigameState116.ref_councilor.faction.LoseOrg(tiorgState);
								}
								else
								{
									TIOrgState tiorgState2 = loseableOrgs.SelectRandomWeightedItem<TIOrgState>((TIOrgState x) => (float)(4 - x.tier), -1f, 1E-37f);
									secondaryinputState.ref_councilor.faction.LoseOrg(tiorgState2);
								}
							}
						}
						num41++;
					}
				}
				return;
			}
			IL_465F:
			if (CS$<>8__locals1.sourceFaction != null)
			{
				TICouncilorState ticouncilorState10 = GameStateManager.CreateNewGameState<TICouncilorState>();
				ticouncilorState10.InitWithTemplate(TemplateManager.Find<TICouncilorTemplate>(strValue, false));
				ticouncilorState10.NewCharacterGeneration(null, null, CS$<>8__locals1.sourceFaction, false, false);
				CS$<>8__locals1.sourceFaction.availableCouncilors.Insert(0, ticouncilorState10);
				CS$<>8__locals1.sourceFaction.newAvailableCouncilors.Add(ticouncilorState10);
				ticouncilorState10.everBeenAvailable = true;
				return;
			}
			return;
			IL_4740:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState117 = enumerator.Current;
					foreach (TIRegionState tiregionState10 in tigameState117.ref_nation.regions)
					{
						tiregionState10.LiberateMyRegion();
					}
				}
				return;
			}
			IL_47AB:
			if (!flag)
			{
				using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIGameState tigameState118 = enumerator.Current;
						foreach (TIControlPoint ticontrolPoint7 in tigameState118.ref_faction.controlPoints)
						{
							if (TIUtilities.RandomFloatValue() < value)
							{
								ticontrolPoint7.ResolveCrackdownEffect(12, null, false, false, 0f);
							}
						}
					}
					return;
				}
			}
			using (List<TIControlPoint>.Enumerator enumerator7 = secondaryinputState.ref_faction.controlPoints.GetEnumerator())
			{
				while (enumerator7.MoveNext())
				{
					TIControlPoint ticontrolPoint8 = enumerator7.Current;
					if (TIUtilities.RandomFloatValue() < value)
					{
						ticontrolPoint8.ResolveCrackdownEffect(12, null, false, true, 0f);
					}
				}
				return;
			}
			IL_4880:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState119 = enumerator.Current;
					foreach (TIArmyState tiarmyState2 in tigameState119.ref_region.armies.ToList<TIArmyState>())
					{
						tiarmyState2.TakeDamage(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), tigameState119.ref_faction, null, true);
					}
				}
				return;
			}
			IL_4907:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState120 = enumerator.Current;
					foreach (TIArmyState tiarmyState3 in tigameState120.ref_region.FilteredArmiesPresent(false, false, true, false, false).ToList<TIArmyState>())
					{
						tiarmyState3.TakeDamage(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), tigameState120.ref_faction, tigameState120.ref_nation, true);
					}
				}
				return;
			}
			IL_4999:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState121 = enumerator.Current;
					foreach (TIArmyState tiarmyState4 in tigameState121.ref_nation.armies.ToList<TIArmyState>())
					{
						tiarmyState4.TakeDamage(TIEffectsState.RandomizedInstantEffectValue(value, randomizer), tigameState121.ref_faction, null, true);
					}
				}
				return;
			}
			IL_4A20:
			TIGlobalValuesState.GlobalValues.endOfOil = true;
			foreach (TIRegionState tiregionState11 in GameStateManager.AllRegions())
			{
				if (tiregionState11.oilRegion)
				{
					tiregionState11.oilRegion = false;
					GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(tiregionState11), null, new object[] { tiregionState11 });
				}
			}
			return;
			IL_4AEA:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState122 = enumerator.Current;
					tigameState122.ref_region.resourceRegion = Convert.ToBoolean(value);
					GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(tigameState122.ref_region), null, new object[] { tigameState122.ref_region });
				}
				return;
			}
			IL_4B59:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState123 = enumerator.Current;
					if (tigameState123.ref_region.template.oilResource)
					{
						tigameState123.ref_region.resourceRegion = Convert.ToBoolean(value);
						GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(tigameState123.ref_region), null, new object[] { tigameState123.ref_region });
					}
				}
				return;
			}
			IL_4BDB:
			if (TIUtilities.GetTemplateValue<TIRegionTemplate>(strValue) != null)
			{
				TIRegionState tiregionState12 = GameStateManager.RegionLookup()[strValue];
				tiregionState12.coreEconomicRegion = Convert.ToBoolean(value);
				GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(tiregionState12), null, new object[] { tiregionState12 });
				return;
			}
			return;
			IL_4D05:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState124 = enumerator.Current;
					tigameState124.ref_region.accumulatedCoreOilRegionTriggers += (int)value;
				}
				return;
			}
			IL_4D4C:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState125 = enumerator.Current;
					tigameState125.ref_region.accumulatedCoreMiningRegionTriggers += (int)value;
				}
				return;
			}
			IL_4D93:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState126 = enumerator.Current;
					tigameState126.ref_region.accumulatedDecolonizeTriggers += (int)value;
				}
				return;
			}
			IL_4DDA:
			using (List<TIGameState>.Enumerator enumerator = ((secondaryStateType == EffectSecondaryStateType.none) ? list : effectSecondaryStateCandidates).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState127 = enumerator.Current;
					tigameState127.ref_region.accumulatedDecontaminateTriggers += (int)value;
				}
				return;
			}
			IL_4E21:
			if (!(CS$<>8__locals1.sourceFaction != null))
			{
				return;
			}
			float globalEnergyCrisisBaseGDPLoss = TemplateManager.global.globalEnergyCrisisBaseGDPLoss;
			float globalEnergyCrisisBaseInequalityGain = TemplateManager.global.globalEnergyCrisisBaseInequalityGain;
			float globalEnergyCrisisOilRegionGDPGain = TemplateManager.global.globalEnergyCrisisOilRegionGDPGain;
			float num42 = Mathf.Max((10f - TIEffectsState.SumEffectsModifiers(Context.GlobalFissionTechLevel, CS$<>8__locals1.sourceFaction, 0f, null) / 2.5f) / 10f, 0.5f);
			float num43 = Mathf.Max((10f - TIEffectsState.SumEffectsModifiers(Context.GlobalFusionTechLevel, CS$<>8__locals1.sourceFaction, 0f, null) / 2.5f) / 10f, 0.5f);
			float num44 = num42 * num43;
			IEnumerable<TINationState> enumerable8 = GameStateManager.AllExtantHumanNations();
			Func<TINationState, bool> func;
			if ((func = CS$<>8__locals1.<>9__17) == null)
			{
				func = (CS$<>8__locals1.<>9__17 = (TINationState x) => x.executiveFaction == CS$<>8__locals1.sourceFaction);
			}
			using (IEnumerator<TINationState> enumerator12 = enumerable8.Where<TINationState>(func).GetEnumerator())
			{
				while (enumerator12.MoveNext())
				{
					TINationState tinationState9 = enumerator12.Current;
					float num45 = (globalEnergyCrisisBaseGDPLoss + globalEnergyCrisisOilRegionGDPGain * (float)tinationState9.oilRegions) * num44 * TIEffectsState.RandomizedInstantEffectValue(value, randomizer);
					tinationState9.GDPPctChange(num45, TINationState.GDPChangeReason.GDPReason_EventEffect);
					float num46 = globalEnergyCrisisBaseInequalityGain * num44 * TIEffectsState.RandomizedInstantEffectValue(value, randomizer);
					tinationState9.AddToInequality(num46, TINationState.InequalityChangeReason.InqReason_EventEffects);
				}
				return;
			}
			IL_4F63:
			TINarrativeEventTemplate tinarrativeEventTemplate = TIUtilities.GetTemplateValue<TINarrativeEventTemplate>(strValue);
			if (tinarrativeEventTemplate == null)
			{
				tinarrativeEventTemplate = TemplateManager.Find<TINarrativeEventTemplate>("event_" + strValue, false);
			}
			if (tinarrativeEventTemplate != null)
			{
				GameStateManager.GlobalValues().TriggerNarrativeEvent(tinarrativeEventTemplate, (value == 1f && CS$<>8__locals1.sourceFaction != null) ? CS$<>8__locals1.sourceFaction : null, false);
				return;
			}
			return;
			IL_50C1:
			CampaignMilestone campaignMilestone = strValue.ToEnum(CampaignMilestone.None);
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState128 = enumerator.Current;
					(tigameState128 as TIFactionState).CompleteMilestone(campaignMilestone);
				}
				return;
			}
			IL_5104:
			using (IEnumerator<TINationState> enumerator12 = GameStateManager.AllExtantHumanNations().GetEnumerator())
			{
				while (enumerator12.MoveNext())
				{
					TINationState tinationState10 = enumerator12.Current;
					for (int num47 = 0; num47 < Enums.PriorityTypes.Length; num47++)
					{
						PriorityType priorityType = Enums.PriorityTypes[num47];
						tinationState10.ModifyAccumulatedInvestment(priorityType, tinationState10.ControlPointWeightsTotalToPriorityIP(priorityType) * value, false, false);
					}
					tinationState10.ProcessPrioritySpending();
				}
				return;
			}
			IL_51A3:
			int num48 = 0;
			while ((float)num48 < value)
			{
				ValueTuple<TINationState, TIRegionState> valueTuple = TIEffectsState.<ProcessInstantEffect>g__NewClaim|24_5(CS$<>8__locals1.sourceFaction, false);
				if (valueTuple.Item1 == null || valueTuple.Item2 == null)
				{
					foreach (TIFactionState tifactionState6 in GameStateManager.AllHumanFactions().ToList<TIFactionState>().Shuffle<TIFactionState>())
					{
						valueTuple = TIEffectsState.<ProcessInstantEffect>g__NewClaim|24_5(tifactionState6, false);
						if (valueTuple.Item1 != null && valueTuple.Item2 != null)
						{
							break;
						}
					}
					if (valueTuple.Item1 == null || valueTuple.Item2 == null)
					{
						valueTuple = TIEffectsState.<ProcessInstantEffect>g__NewClaim|24_5(null, false);
						if (valueTuple.Item1 == null || valueTuple.Item2 == null)
						{
							valueTuple = TIEffectsState.<ProcessInstantEffect>g__NewClaim|24_5(null, true);
						}
					}
				}
				if (valueTuple.Item1 != null && valueTuple.Item2 != null)
				{
					valueTuple.Item1.SetClaim(valueTuple.Item2, false, false);
					TINotificationQueueState.LogNationsGainClaims(new Dictionary<TINationState, List<TIRegionState>> { 
					{
						valueTuple.Item1,
						new List<TIRegionState> { valueTuple.Item2 }
					} }, null);
				}
				num48++;
			}
		}

		// Token: 0x06003B8D RID: 15245 RVA: 0x001673EC File Offset: 0x001655EC
		[CompilerGenerated]
		internal static void <ProcessInstantEffect>g__GrantMissedProjectToFaction|24_3(TIFactionState faction)
		{
			if (faction.missedProjects.Count > 0)
			{
				TIProjectTemplate key = faction.missedProjects.ConvertAll<TIProjectTemplate>((string x) => TemplateManager.Find<TIProjectTemplate>(x, false)).ToDictionary<TIProjectTemplate, TIProjectTemplate, float>((TIProjectTemplate x) => x, (TIProjectTemplate x) => x.factionAvailableChance / x.researchCost).SelectRandomWeightedItem<KeyValuePair<TIProjectTemplate, float>>((KeyValuePair<TIProjectTemplate, float> x) => x.Value, -1f, 1E-37f)
					.Key;
				faction.AddAvailableProject(key, null);
				TINotificationQueueState.LogProjectTriggered(faction, key, true);
			}
		}

		// Token: 0x06003B8E RID: 15246 RVA: 0x001674BF File Offset: 0x001656BF
		[CompilerGenerated]
		internal static ValueTuple<TINationState, TIRegionState> <ProcessInstantEffect>g__NewClaim|24_5(TIFactionState faction, bool nonExtant)
		{
			if (faction == null || faction.executiveNations.Count > 0)
			{
				return TIEffectsState.<ProcessInstantEffect>g__GetClaimedRegion|24_21(faction);
			}
			return new ValueTuple<TINationState, TIRegionState>(null, null);
		}

		// Token: 0x06003B8F RID: 15247 RVA: 0x001674E8 File Offset: 0x001656E8
		[CompilerGenerated]
		internal static IEnumerable<TINationState> <ProcessInstantEffect>g__GetNationsToTest|24_18(int iteration, TIFactionState faction, bool nonExtant)
		{
			if (faction == null)
			{
				if (nonExtant)
				{
					return from x in GameStateManager.AllHumanNations()
						where x.executiveFaction == null
						select x;
				}
				return from x in GameStateManager.AllExtantHumanNations()
					where x.executiveFaction == null
					select x;
			}
			else
			{
				if (iteration == 3 || iteration == 6)
				{
					return faction.executiveNations.Where<TINationState>((TINationState x) => x.claims.Count < GameStateManager.AllRegions().Length && x.coastalRegions > 0);
				}
				return faction.executiveNations.Where<TINationState>((TINationState x) => x.claims.Count < GameStateManager.AllRegions().Length);
			}
		}

		// Token: 0x06003B90 RID: 15248 RVA: 0x001675B4 File Offset: 0x001657B4
		[CompilerGenerated]
		internal static IEnumerable<TIRegionState> <ProcessInstantEffect>g__GetRegionGroup|24_19(int iteration, TINationState nation)
		{
			if (iteration - 1 > 1)
			{
				return from x in GameStateManager.AllRegions()
					where !nation.claims.Contains(x)
					select x;
			}
			if (nation.AdjacentNations(false).Count > 0)
			{
				Func<TIRegionState, bool> <>9__29;
				return nation.AdjacentNations(false).SelectMany<TINationState, TIRegionState>(delegate(TINationState x)
				{
					IEnumerable<TIRegionState> regions = x.regions;
					Func<TIRegionState, bool> func;
					if ((func = <>9__29) == null)
					{
						func = (<>9__29 = (TIRegionState y) => !nation.claims.Contains(y));
					}
					return regions.Where<TIRegionState>(func);
				});
			}
			return from x in GameStateManager.AllRegions()
				where !nation.claims.Contains(x)
				select x;
		}

		// Token: 0x06003B91 RID: 15249 RVA: 0x00167634 File Offset: 0x00165834
		[CompilerGenerated]
		internal static bool <ProcessInstantEffect>g__RegionPassesCondition|24_20(int iteration, TIRegionState region, TINationState nation)
		{
			switch (iteration)
			{
			case 1:
				return region.AdjacentNations(true, false).Contains(nation);
			case 2:
				return true;
			case 3:
			case 4:
				return region.isCoastal && region.mapRegionTemplate.supraRegion == nation.capital.mapRegionTemplate.supraRegion;
			case 5:
				return region.mapRegionTemplate.supraRegion == nation.capital.mapRegionTemplate.supraRegion;
			case 6:
				return region.isCoastal;
			default:
				return true;
			}
		}

		// Token: 0x06003B92 RID: 15250 RVA: 0x001676C4 File Offset: 0x001658C4
		[CompilerGenerated]
		internal static ValueTuple<TINationState, TIRegionState> <ProcessInstantEffect>g__GetClaimedRegion|24_21(TIFactionState faction)
		{
			TINationState nation = null;
			TIRegionState tiregionState = null;
			bool flag = false;
			int k;
			Func<TIRegionState, bool> <>9__31;
			int i;
			for (k = 1; k <= 7; k = i + 1)
			{
				List<TINationState> list = TIEffectsState.<ProcessInstantEffect>g__GetNationsToTest|24_18(k, faction, false).ToList<TINationState>();
				while (!flag && list.Count > 0)
				{
					nation = list.SelectRandomWeightedItem<TINationState>((TINationState x) => x.population_Millions, -1f, 1E-37f);
					list.Remove(nation);
					IEnumerable<TIRegionState> enumerable = TIEffectsState.<ProcessInstantEffect>g__GetRegionGroup|24_19(k, nation);
					IEnumerable<TIRegionState> enumerable2 = enumerable;
					Func<TIRegionState, bool> func;
					if ((func = <>9__31) == null)
					{
						func = (<>9__31 = (TIRegionState x) => TIEffectsState.<ProcessInstantEffect>g__RegionPassesCondition|24_20(k, x, nation));
					}
					enumerable = enumerable2.Where<TIRegionState>(func);
					if (enumerable.Any<TIRegionState>())
					{
						tiregionState = enumerable.SelectRandomItem<TIRegionState>();
						flag = true;
						break;
					}
				}
				i = k;
			}
			return new ValueTuple<TINationState, TIRegionState>(nation, tiregionState);
		}

		// Token: 0x040025CE RID: 9678
		[SerializeField]
		private Dictionary<TIFactionState, Dictionary<Context, List<string>>> factionEffectsNames;

		// Token: 0x040025CF RID: 9679
		[SerializeField]
		private Dictionary<TIFactionState, Dictionary<string, TIDateTime>> factionEffectExpirations;

		// Token: 0x040025D0 RID: 9680
		private Dictionary<TIFactionState, Dictionary<Context, List<TIEffectTemplate>>> factionEffects;

		// Token: 0x040025D1 RID: 9681
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x040025D2 RID: 9682
		public const float maxMultiplierForScaledEffects = 2f;
	}
}
