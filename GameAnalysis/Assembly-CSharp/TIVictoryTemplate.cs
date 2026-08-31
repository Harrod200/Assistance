using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200035D RID: 861
public class TIVictoryTemplate : TIDataTemplate
{
	// Token: 0x06000F1F RID: 3871 RVA: 0x0004B528 File Offset: 0x00049728
	public bool AllVictoryConditionsMet(TIFactionState faction)
	{
		return this.victoryConditions.Where<TIVictoryTemplate.VictoryCondition>((TIVictoryTemplate.VictoryCondition x) => x.conditionType > TIVictoryTemplate.VictoryConditionType.none).All<TIVictoryTemplate.VictoryCondition>((TIVictoryTemplate.VictoryCondition x) => this.SingleVictoryConditionMet(faction, x));
	}

	// Token: 0x06000F20 RID: 3872 RVA: 0x0004B584 File Offset: 0x00049784
	public string SingleVictoryConditionDescriptionWithScore(TIFactionState faction, TIVictoryTemplate.VictoryCondition condition, out List<TISpaceAssetState> failingAssets)
	{
		failingAssets = new List<TISpaceAssetState>();
		if (this.defeatAllHabsCondition.Contains(condition.conditionType))
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TISpaceBodyState tispaceBodyState in this.GetMajorPlanetRegions(condition))
			{
				List<TINaturalSpaceObjectState> list;
				List<TIHabState> list2;
				List<TIFactionState> list3;
				bool flag = this.FreePlanetRegion(faction, condition.conditionType, tispaceBodyState, (int)condition.value, true, out list, out list2, out list3);
				failingAssets.AddRange(list2);
				bool flag2 = tispaceBodyState.objectType == SpaceObjectType.Planet;
				string text = new StringBuilder("TIVictoryCondition.").Append(condition.conditionType.ToString()).Append(flag2 ? "_PlanetarySystem" : "_Region").ToString();
				object[] array = new object[3];
				array[0] = condition.value.ToString("N0");
				int num = 1;
				object obj;
				if (!flag2)
				{
					obj = TIUtilities.ConstructTextList(list.Select<TINaturalSpaceObjectState, string>((TINaturalSpaceObjectState x) => x.displayName).ToList<string>(), true, false);
				}
				else
				{
					obj = tispaceBodyState.displayName;
				}
				array[num] = obj;
				array[2] = TIUtilities.ConstructTextList(list3.Select<TIFactionState, string>((TIFactionState x) => x.displayName).ToList<string>(), true, false);
				string text2 = Loc.T(text, array);
				stringBuilder.AppendLine(text2);
				if (flag)
				{
					stringBuilder.AppendLine(TIUtilities.GreenLine(Loc.T("TIVictoryCondition.Complete")));
				}
				else
				{
					foreach (TIHabState tihabState in list2)
					{
						stringBuilder.AppendLine(TIUtilities.RedLine(TIUtilities.GetLocationString(tihabState, true, false)));
					}
				}
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}
		if (this.defeatAllBasesCondition.Contains(condition.conditionType))
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (TISpaceBodyState tispaceBodyState2 in this.GetMajorPlanetRegions(condition))
			{
				List<TINaturalSpaceObjectState> list4;
				List<TIHabState> list5;
				List<TIFactionState> list6;
				bool flag3 = this.FreePlanetRegion(faction, condition.conditionType, tispaceBodyState2, (int)condition.value, true, out list4, out list5, out list6);
				failingAssets.AddRange(list5);
				bool flag4 = tispaceBodyState2.objectType == SpaceObjectType.Planet;
				string text3 = new StringBuilder("TIVictoryCondition.").Append(condition.conditionType.ToString()).Append(flag4 ? "_PlanetarySystem" : "_Region").ToString();
				object[] array2 = new object[3];
				array2[0] = condition.value.ToString("N0");
				int num2 = 1;
				object obj2;
				if (!flag4)
				{
					obj2 = TIUtilities.ConstructTextList(list4.Select<TINaturalSpaceObjectState, string>((TINaturalSpaceObjectState x) => x.displayName).ToList<string>(), true, false);
				}
				else
				{
					obj2 = tispaceBodyState2.displayName;
				}
				array2[num2] = obj2;
				array2[2] = TIUtilities.ConstructTextList(list6.Select<TIFactionState, string>((TIFactionState x) => x.displayName).ToList<string>(), true, false);
				string text4 = Loc.T(text3, array2);
				stringBuilder2.AppendLine(text4);
				if (flag3)
				{
					stringBuilder2.AppendLine(TIUtilities.GreenLine(Loc.T("TIVictoryCondition.Complete")));
				}
				else
				{
					foreach (TIHabState tihabState2 in list5)
					{
						stringBuilder2.AppendLine(TIUtilities.RedLine(TIUtilities.GetLocationString(tihabState2, true, false)));
					}
				}
				stringBuilder2.AppendLine();
			}
			return stringBuilder2.ToString();
		}
		if (this.defeatAllFleetsCondition.Contains(condition.conditionType))
		{
			StringBuilder stringBuilder3 = new StringBuilder();
			TISpaceBodyState tispaceBodyState3 = null;
			List<TINaturalSpaceObjectState> list7;
			List<TISpaceFleetState> list8;
			List<TIFactionState> list9;
			bool flag5 = this.FreeFleetRegion(faction, condition.conditionType, tispaceBodyState3, condition.value, true, out list7, out list8, out list9);
			failingAssets.AddRange(list8);
			bool flag6 = tispaceBodyState3 != null && tispaceBodyState3.objectType == SpaceObjectType.Planet;
			string text5 = new StringBuilder("TIVictoryCondition.").Append(condition.conditionType.ToString()).Append((tispaceBodyState3 == null) ? "_All" : (flag6 ? "_PlanetarySystem" : "_Region")).ToString();
			object[] array3 = new object[3];
			array3[0] = condition.value.ToString("N0");
			int num3 = 1;
			object obj3;
			if (!(tispaceBodyState3 == null))
			{
				if (!flag6)
				{
					obj3 = TIUtilities.ConstructTextList(list7.Select<TINaturalSpaceObjectState, string>((TINaturalSpaceObjectState x) => x.displayName).ToList<string>(), true, false);
				}
				else
				{
					obj3 = ((tispaceBodyState3 != null) ? tispaceBodyState3.displayName : null);
				}
			}
			else
			{
				obj3 = "";
			}
			array3[num3] = obj3;
			array3[2] = TIUtilities.ConstructTextList(list9.Select<TIFactionState, string>((TIFactionState x) => x.displayName).ToList<string>(), true, false);
			string text6 = Loc.T(text5, array3);
			stringBuilder3.AppendLine(text6);
			if (flag5)
			{
				stringBuilder3.AppendLine(TIUtilities.GreenLine(Loc.T("TIVictoryCondition.Complete")));
			}
			else
			{
				foreach (TISpaceFleetState tispaceFleetState in list8)
				{
					stringBuilder3.Append(TIUtilities.RedLine(tispaceFleetState.GetDisplayName(GameControl.control.activePlayer))).Append(": ").Append(tispaceFleetState.GetLocationDescription(GameControl.control.activePlayer, true, true))
						.Append(TIGlobalConfig.globalConfig.spaceCombatScoreInlineSpritePath)
						.Append(tispaceFleetState.SpaceCombatValue().ToString("N0"));
					if (list8.Last<TISpaceFleetState>() != tispaceFleetState)
					{
						stringBuilder3.AppendLine();
					}
				}
			}
			stringBuilder3.AppendLine();
			return stringBuilder3.ToString();
		}
		if (this.spaceAssetConstructionCondition.Contains(condition.conditionType))
		{
			StringBuilder stringBuilder4 = new StringBuilder();
			this.SingleVictoryConditionMet(faction, condition);
			string text7 = new StringBuilder("TIVictoryCondition.").Append(condition.conditionType.ToString()).ToString();
			stringBuilder4.AppendLine(Loc.T(text7, new object[] { condition.value.ToString("N0") }));
			if (condition.conditionType == TIVictoryTemplate.VictoryConditionType.BasePresence_MajorPlanets)
			{
				foreach (TISpaceBodyState tispaceBodyState4 in this.GetMajorBuildablePlanetRegions())
				{
					stringBuilder4.AppendLine(Loc.T("TIVictoryCondition.Colon", new object[]
					{
						tispaceBodyState4.displayName,
						this.MySpaceAssetRegion(faction, condition.conditionType, tispaceBodyState4, (int)condition.value) ? TIUtilities.GreenLine(Loc.T("TIVictoryCondition.Complete")) : TIUtilities.RedLine(Loc.T("TIVictoryCondition.Incomplete"))
					}));
				}
				stringBuilder4.AppendLine();
			}
			return stringBuilder4.ToString();
		}
		float num4 = this.VictoryConditionNumerator(faction, condition);
		float num5 = Mathf.Max(this.VictoryConditionDenominator(condition), 1f);
		float num6;
		if (num5 <= 0f)
		{
			num6 = 1f;
		}
		else
		{
			num6 = num4 / num5;
		}
		TIVictoryTemplate.VictoryConditionType conditionType = condition.conditionType;
		string text8;
		string text9;
		if (conditionType - TIVictoryTemplate.VictoryConditionType.MinGlobalDemocracyWeightedAverage <= 3)
		{
			text8 = Loc.T(new StringBuilder("TIVictoryCondition.description.").Append(condition.conditionType.ToString()).ToString(), new object[] { (condition.value * 10f).ToString("N2") });
			text9 = Loc.T("TIVictoryCondition.Status", new object[]
			{
				num4.ToString("N2"),
				num5.ToString("N2"),
				num6.ToPercent("P0")
			});
		}
		else
		{
			text8 = Loc.T(new StringBuilder("TIVictoryCondition.description.").Append(condition.conditionType.ToString()).ToString(), new object[] { condition.value.ToPercent("P0") });
			text9 = Loc.T("TIVictoryCondition.Status", new object[]
			{
				num4.ToString("N0"),
				num5.ToString("N0"),
				num6.ToPercent("P0")
			});
		}
		text9 = (this.SingleVictoryConditionMet(faction, condition) ? TIUtilities.GreenLine(text9) : TIUtilities.RedLine(text9));
		return new StringBuilder(text8).AppendLine().AppendLine(text9).ToString();
	}

	// Token: 0x06000F21 RID: 3873 RVA: 0x0004BEC8 File Offset: 0x0004A0C8
	public float VictoryConditionNumerator(TIFactionState faction, TIVictoryTemplate.VictoryCondition condition)
	{
		switch (condition.conditionType)
		{
		case TIVictoryTemplate.VictoryConditionType.GlobalControlPointProportion:
			return (float)faction.controlPoints.Sum<TIControlPoint>((TIControlPoint x) => x.nation.numControlPoints_unclamped);
		case TIVictoryTemplate.VictoryConditionType.GlobalRegionProportion:
			return (float)faction.majorityControlNations.Select<TINationState, List<TIRegionState>>((TINationState x) => x.regions).Count<List<TIRegionState>>();
		case TIVictoryTemplate.VictoryConditionType.GlobalEcoProportion:
			return (float)faction.majorityControlNations.Select<TINationState, IEnumerable<TIRegionState>>((TINationState nation) => nation.regions.Where<TIRegionState>((TIRegionState region) => region.coreEconomicRegion)).Count<IEnumerable<TIRegionState>>();
		case TIVictoryTemplate.VictoryConditionType.GlobalPopulationProportion:
			return faction.majorityControlNations.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions).Sum<TIRegionState>((TIRegionState x) => x.populationInMillions);
		case TIVictoryTemplate.VictoryConditionType.GlobalPopularityProportion:
			return TIGlobalValuesState.GlobalValues.GetGlobalPublicOpinionProportions()[faction.ideology.ideology] * 100f;
		case TIVictoryTemplate.VictoryConditionType.GlobalGDPProportion:
			return faction.majorityControlNations.Sum<TINationState>((TINationState x) => (float)x.GDP);
		case TIVictoryTemplate.VictoryConditionType.GlobalMissionControlCapacity:
			return (float)(faction.majorityControlNations.Sum<TINationState>((TINationState nation) => nation.missionControl) + faction.habs.Sum<TIHabState>((TIHabState x) => (from x in x.ActiveModules()
				where x.moduleTemplate.missionControl > 0
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.missionControl)));
		case TIVictoryTemplate.VictoryConditionType.AlienNationMaxRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMinRegionProportion:
			return (float)GameStateManager.AlienNation().regions.Count;
		case TIVictoryTemplate.VictoryConditionType.AlienNationMaxPopulationProportion:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMinPopulationProportion:
			return GameStateManager.AlienNation().regions.Sum<TIRegionState>((TIRegionState x) => x.populationInMillions);
		case TIVictoryTemplate.VictoryConditionType.ProAlienMaxRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.ProAlienMinRegionProportion:
			return (float)(from x in GameStateManager.AllFactions()
				where x.proAlien
				select x).SelectMany<TIFactionState, TINationState>((TIFactionState x) => x.majorityControlNations).SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions).Count<TIRegionState>();
		case TIVictoryTemplate.VictoryConditionType.MinProAlienFleetPower:
		case TIVictoryTemplate.VictoryConditionType.MaxProAlienFleetPower:
			return (from x in GameStateManager.AllFactions()
				where x.proAlien
				select x).SelectMany<TIFactionState, TISpaceFleetState>((TIFactionState x) => x.fleets).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
		case TIVictoryTemplate.VictoryConditionType.MaxOtherFactionsFleetPower:
			return (from x in GameStateManager.AllFactions()
				where !x.permanentAlly(faction)
				select x).SelectMany<TIFactionState, TISpaceFleetState>((TIFactionState x) => x.fleets).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
		case TIVictoryTemplate.VictoryConditionType.MinGlobalDemocracyWeightedAverage:
		case TIVictoryTemplate.VictoryConditionType.MaxGlobalDemocracyWeightedAverage:
			return GameStateManager.AllExtantHumanNations().Sum<TINationState>((TINationState nation) => nation.democracy * nation.population_Millions) / GameStateManager.AllRegions().Sum<TIRegionState>((TIRegionState x) => x.populationInMillions);
		case TIVictoryTemplate.VictoryConditionType.MinGlobalInequalityWeightedAverage:
		case TIVictoryTemplate.VictoryConditionType.MaxGlobalInequalityWeightedAverage:
			return GameStateManager.AllExtantHumanNations().Sum<TINationState>((TINationState nation) => nation.inequality * nation.population_Millions) / GameStateManager.AllRegions().Sum<TIRegionState>((TIRegionState x) => x.populationInMillions);
		default:
			Log.Error("Bad conditon passed to Victory Condition Numerator", Array.Empty<object>());
			return 0f;
		}
	}

	// Token: 0x06000F22 RID: 3874 RVA: 0x0004C334 File Offset: 0x0004A534
	public float VictoryConditionDenominator(TIVictoryTemplate.VictoryCondition condition)
	{
		switch (condition.conditionType)
		{
		case TIVictoryTemplate.VictoryConditionType.GlobalControlPointProportion:
			return (float)GameStateManager.AllActiveControlPoints().Sum<TIControlPoint>((TIControlPoint x) => x.nation.numControlPoints_unclamped);
		case TIVictoryTemplate.VictoryConditionType.GlobalRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMaxRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMinRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.ProAlienMaxRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.ProAlienMinRegionProportion:
			return (float)GameStateManager.IterateByClass<TIRegionState>(false).Count<TIRegionState>();
		case TIVictoryTemplate.VictoryConditionType.GlobalEcoProportion:
			return (float)(from region in GameStateManager.IterateByClass<TIRegionState>(false)
				where region.coreEconomicRegion
				select region).Count<TIRegionState>();
		case TIVictoryTemplate.VictoryConditionType.GlobalPopulationProportion:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMaxPopulationProportion:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMinPopulationProportion:
			return GameStateManager.IterateByClass<TIRegionState>(false).Sum<TIRegionState>((TIRegionState x) => x.populationInMillions);
		case TIVictoryTemplate.VictoryConditionType.GlobalPopularityProportion:
			return 100f;
		case TIVictoryTemplate.VictoryConditionType.GlobalGDPProportion:
			return (float)TIGlobalValuesState.globalGDP;
		case TIVictoryTemplate.VictoryConditionType.GlobalMissionControlCapacity:
			return (float)(GameStateManager.AllRegions().Sum<TIRegionState>((TIRegionState region) => region.missionControl) + GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TIHabState>((TIFactionState x) => x.habs).Sum<TIHabState>((TIHabState x) => (from x in x.ActiveModules()
				where x.moduleTemplate.missionControl > 0
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.missionControl)));
		case TIVictoryTemplate.VictoryConditionType.MinProAlienFleetPower:
		case TIVictoryTemplate.VictoryConditionType.MaxProAlienFleetPower:
		case TIVictoryTemplate.VictoryConditionType.MaxOtherFactionsFleetPower:
			return GameStateManager.AllFactions().SelectMany<TIFactionState, TISpaceFleetState>((TIFactionState x) => x.fleets).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
		case TIVictoryTemplate.VictoryConditionType.MinGlobalDemocracyWeightedAverage:
		case TIVictoryTemplate.VictoryConditionType.MaxGlobalDemocracyWeightedAverage:
			return 10f;
		case TIVictoryTemplate.VictoryConditionType.MinGlobalInequalityWeightedAverage:
		case TIVictoryTemplate.VictoryConditionType.MaxGlobalInequalityWeightedAverage:
			return 9f;
		default:
			Log.Error("Bad condition " + condition.conditionType.ToString() + " passed to Victory Condition Denominator", Array.Empty<object>());
			return 1f;
		}
	}

	// Token: 0x06000F23 RID: 3875 RVA: 0x0004C54C File Offset: 0x0004A74C
	public List<TISpaceBodyState> GetMajorBuildablePlanetRegions()
	{
		return new List<TISpaceBodyState>
		{
			GameStateManager.Mercury(),
			GameStateManager.Earth(),
			GameStateManager.Mars(),
			GameStateManager.Ceres(),
			GameStateManager.Jupiter(),
			GameStateManager.Saturn()
		};
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x0004C5A0 File Offset: 0x0004A7A0
	public List<TISpaceBodyState> GetMajorPlanetRegions(TIVictoryTemplate.VictoryCondition condition)
	{
		List<TISpaceBodyState> list = new List<TISpaceBodyState>
		{
			GameStateManager.Mercury(),
			GameStateManager.Venus(),
			GameStateManager.Earth(),
			GameStateManager.Mars(),
			GameStateManager.Ceres(),
			GameStateManager.Jupiter(),
			GameStateManager.Saturn(),
			GameStateManager.Uranus(),
			GameStateManager.Neptune()
		};
		if (this.defeatAlienHomeworldCondition.Contains(condition.conditionType))
		{
			list.AddUnique(GameStateManager.AlienFaction().primaryHab.barycenter.ref_spaceBody);
		}
		return list;
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x0004C648 File Offset: 0x0004A848
	protected bool SingleVictoryConditionMet(TIFactionState faction, TIVictoryTemplate.VictoryCondition condition)
	{
		switch (condition.conditionType)
		{
		case TIVictoryTemplate.VictoryConditionType.none:
			return true;
		case TIVictoryTemplate.VictoryConditionType.GlobalControlPointProportion:
		case TIVictoryTemplate.VictoryConditionType.GlobalRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.GlobalEcoProportion:
		case TIVictoryTemplate.VictoryConditionType.GlobalPopulationProportion:
		case TIVictoryTemplate.VictoryConditionType.GlobalPopularityProportion:
		case TIVictoryTemplate.VictoryConditionType.GlobalGDPProportion:
		case TIVictoryTemplate.VictoryConditionType.GlobalMissionControlCapacity:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMinRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMinPopulationProportion:
		case TIVictoryTemplate.VictoryConditionType.ProAlienMinRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.MinProAlienFleetPower:
		case TIVictoryTemplate.VictoryConditionType.MinGlobalDemocracyWeightedAverage:
		case TIVictoryTemplate.VictoryConditionType.MinGlobalInequalityWeightedAverage:
		{
			float num = this.VictoryConditionDenominator(condition);
			return num <= 0f || this.VictoryConditionNumerator(faction, condition) / num >= condition.value;
		}
		case TIVictoryTemplate.VictoryConditionType.AlienNationMaxRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.AlienNationMaxPopulationProportion:
		case TIVictoryTemplate.VictoryConditionType.ProAlienMaxRegionProportion:
		case TIVictoryTemplate.VictoryConditionType.MaxProAlienFleetPower:
		case TIVictoryTemplate.VictoryConditionType.MaxOtherFactionsFleetPower:
		case TIVictoryTemplate.VictoryConditionType.MaxGlobalDemocracyWeightedAverage:
		case TIVictoryTemplate.VictoryConditionType.MaxGlobalInequalityWeightedAverage:
		{
			float num2 = this.VictoryConditionDenominator(condition);
			return num2 <= 0f || this.VictoryConditionNumerator(faction, condition) / num2 <= condition.value;
		}
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliens:
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliensAndAllies:
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatEveryone:
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAntiAlienFactions:
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatNonVeryProAlienFactions:
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatExtremists:
			foreach (TISpaceBodyState tispaceBodyState in this.GetMajorPlanetRegions(condition))
			{
				List<TINaturalSpaceObjectState> list;
				List<TIHabState> list2;
				List<TIFactionState> list3;
				if (!this.FreePlanetRegion(faction, condition.conditionType, tispaceBodyState, (int)condition.value, false, out list, out list2, out list3))
				{
					return false;
				}
			}
			return true;
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliens:
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliensAndAllies:
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatEveryone:
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAntiAlienFactions:
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatNonVeryProAlienFactions:
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatExtremists:
		{
			List<TINaturalSpaceObjectState> list;
			List<TIFactionState> list3;
			List<TISpaceFleetState> list4;
			return this.FreeFleetRegion(faction, condition.conditionType, null, condition.value, false, out list, out list4, out list3);
		}
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliens:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliensAndAllies:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatEveryone:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAntiAlienFactions:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatNonVeryProAlienFactions:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatExtremists:
			foreach (TISpaceBodyState tispaceBodyState2 in this.GetMajorPlanetRegions(condition))
			{
				List<TINaturalSpaceObjectState> list;
				List<TIHabState> list2;
				List<TIFactionState> list3;
				if (!this.FreePlanetRegion(faction, condition.conditionType, tispaceBodyState2, (int)condition.value, false, out list, out list2, out list3))
				{
					return false;
				}
			}
			return true;
		case TIVictoryTemplate.VictoryConditionType.BasePresence_MajorPlanets:
			foreach (TISpaceBodyState tispaceBodyState3 in this.GetMajorBuildablePlanetRegions())
			{
				if (!this.MySpaceAssetRegion(faction, condition.conditionType, tispaceBodyState3, (int)condition.value))
				{
					return false;
				}
			}
			return true;
		default:
			Log.Error("Bad condition " + condition.conditionType.ToString() + " passed to Single Victory Condition Met", Array.Empty<object>());
			return false;
		}
		bool flag;
		return flag;
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x0004C8D0 File Offset: 0x0004AAD0
	private bool MySpaceAssetRegion(TIFactionState faction, TIVictoryTemplate.VictoryConditionType condition, TISpaceBodyState keySpaceBody, int tier)
	{
		List<TISpaceBodyState> list = new List<TISpaceBodyState>();
		if (keySpaceBody != null)
		{
			list.Add(keySpaceBody);
			list.AddRange(keySpaceBody.naturalSatellites);
		}
		Func<TIHabModuleState, bool> <>9__1;
		Func<TIHabState, bool> <>9__0;
		foreach (TISpaceBodyState tispaceBodyState in list)
		{
			if (condition == TIVictoryTemplate.VictoryConditionType.BasePresence_MajorPlanets)
			{
				IEnumerable<TIHabState> surfaceBases = tispaceBodyState.surfaceBases;
				Func<TIHabState, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = delegate(TIHabState x)
					{
						if (x.faction == faction)
						{
							IEnumerable<TIHabModuleState> enumerable = x.AllModuleStates();
							Func<TIHabModuleState, bool> func2;
							if ((func2 = <>9__1) == null)
							{
								func2 = (<>9__1 = (TIHabModuleState x) => x.active && x.tier >= tier);
							}
							return enumerable.All<TIHabModuleState>(func2);
						}
						return false;
					});
				}
				if (surfaceBases.Any<TIHabState>(func))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x0004C98C File Offset: 0x0004AB8C
	private bool FreePlanetRegion(TIFactionState faction, TIVictoryTemplate.VictoryConditionType condition, TISpaceBodyState keySpaceBody, int tier, bool collectList, out List<TINaturalSpaceObjectState> bodiesToSurvey, out List<TIHabState> failingHabs, out List<TIFactionState> factions)
	{
		factions = new List<TIFactionState>();
		failingHabs = new List<TIHabState>();
		bodiesToSurvey = new List<TINaturalSpaceObjectState>();
		switch (condition)
		{
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliens:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliens:
			factions.Add(GameStateManager.AlienFaction());
			goto IL_0178;
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliensAndAllies:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliensAndAllies:
			factions.Add(GameStateManager.AlienFaction());
			factions.Add(GameStateManager.AlienProxy());
			factions.AddUnique(GameStateManager.AlienAppeaser());
			goto IL_0178;
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatEveryone:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatEveryone:
			factions.AddRange(from x in GameStateManager.AllFactions()
				where x != faction
				select x);
			goto IL_0178;
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAntiAlienFactions:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAntiAlienFactions:
			factions.AddRange(from x in GameStateManager.AllHumanFactions()
				where x.veryAntiAlien
				select x);
			goto IL_0178;
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatNonVeryProAlienFactions:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatNonVeryProAlienFactions:
			factions.AddRange(from x in GameStateManager.AllHumanFactions()
				where !x.veryProAlien
				select x);
			goto IL_0178;
		case TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatExtremists:
		case TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatExtremists:
			factions.AddRange(from x in GameStateManager.AllFactions()
				where x.extremist
				select x);
			goto IL_0178;
		}
		return true;
		IL_0178:
		if (keySpaceBody != null)
		{
			bodiesToSurvey.Add(keySpaceBody);
			bodiesToSurvey.AddRange(keySpaceBody.naturalSatellites);
			if (keySpaceBody.objectType == SpaceObjectType.DwarfPlanet)
			{
				if (GameStateManager.FullAsteroidBelt(true).Contains(keySpaceBody))
				{
					bodiesToSurvey.AddRange((from x in GameStateManager.FullAsteroidBelt(true)
						where x.habSites.Length > 1
						select x).Except<TINaturalSpaceObjectState>(bodiesToSurvey));
				}
				else if (GameStateManager.KuiperBeltObjects(true).Contains(keySpaceBody))
				{
					bodiesToSurvey.AddRange((from x in GameStateManager.KuiperBeltObjects(true)
						where x.habSites.Length > 1
						select x).Except<TINaturalSpaceObjectState>(bodiesToSurvey));
				}
				else if (GameStateManager.Centaurs(true).Contains(keySpaceBody))
				{
					bodiesToSurvey.AddRange((from x in GameStateManager.Centaurs(true)
						where x.habSites.Length > 1
						select x).Except<TINaturalSpaceObjectState>(bodiesToSurvey));
				}
			}
			using (List<TINaturalSpaceObjectState>.Enumerator enumerator = bodiesToSurvey.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TINaturalSpaceObjectState tinaturalSpaceObjectState = enumerator.Current;
					List<TIHabState> list = new List<TIHabState>(tinaturalSpaceObjectState.habs);
					if (this.defeatAllBasesCondition.Contains(condition))
					{
						list = list.Where<TIHabState>((TIHabState x) => x.IsBase).ToList<TIHabState>();
					}
					foreach (TIHabState tihabState in list)
					{
						if (factions.Contains(tihabState.faction) && tihabState.tier >= tier && tihabState.anyCoreCompleted && (tihabState.faction.IsActiveHumanFaction || tihabState != tihabState.faction.primaryHab))
						{
							if (!collectList)
							{
								return false;
							}
							failingHabs.Add(tihabState);
						}
					}
				}
				goto IL_0474;
			}
		}
		List<TIHabState> list2 = new List<TIHabState>(GameStateManager.AllFactions().SelectMany<TIFactionState, TIHabState>((TIFactionState x) => x.habs));
		if (this.defeatAllBasesCondition.Contains(condition))
		{
			list2 = list2.Where<TIHabState>((TIHabState x) => x.IsBase).ToList<TIHabState>();
		}
		foreach (TIHabState tihabState2 in list2)
		{
			if (factions.Contains(tihabState2.faction) && tihabState2.tier >= tier && (tihabState2.faction.IsActiveHumanFaction || tihabState2 != tihabState2.faction.primaryHab))
			{
				if (!collectList)
				{
					return false;
				}
				failingHabs.Add(tihabState2);
			}
		}
		IL_0474:
		return !collectList || failingHabs.Count == 0;
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x0004CE4C File Offset: 0x0004B04C
	private bool FreeFleetRegion(TIFactionState faction, TIVictoryTemplate.VictoryConditionType condition, TISpaceBodyState keySpaceBody, float combatScore, bool collectList, out List<TINaturalSpaceObjectState> bodiesToSurvey, out List<TISpaceFleetState> failingFleets, out List<TIFactionState> factions)
	{
		factions = new List<TIFactionState>();
		failingFleets = new List<TISpaceFleetState>();
		bodiesToSurvey = new List<TINaturalSpaceObjectState>();
		switch (condition)
		{
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliens:
			factions.Add(GameStateManager.AlienFaction());
			break;
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliensAndAllies:
			factions.Add(GameStateManager.AlienFaction());
			factions.Add(GameStateManager.AlienProxy());
			factions.AddUnique(GameStateManager.AlienAppeaser());
			break;
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatEveryone:
			factions.AddRange(from x in GameStateManager.AllFactions()
				where x != faction
				select x);
			break;
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAntiAlienFactions:
			factions.AddRange(from x in GameStateManager.AllHumanFactions()
				where x.veryAntiAlien
				select x);
			break;
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatNonVeryProAlienFactions:
			factions.AddRange(from x in GameStateManager.AllHumanFactions()
				where !x.veryProAlien
				select x);
			break;
		case TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatExtremists:
			factions.AddRange(from x in GameStateManager.AllFactions()
				where x.extremist
				select x);
			break;
		default:
			return true;
		}
		if (keySpaceBody != null)
		{
			bodiesToSurvey.Add(keySpaceBody);
			bodiesToSurvey.AddRange(keySpaceBody.naturalSatellites);
			if (keySpaceBody.objectType == SpaceObjectType.DwarfPlanet)
			{
				if (GameStateManager.FullAsteroidBelt(true).Contains(keySpaceBody))
				{
					bodiesToSurvey.AddRange((from x in GameStateManager.FullAsteroidBelt(true)
						where x.habSites.Length > 1
						select x).Except<TINaturalSpaceObjectState>(bodiesToSurvey));
				}
				else if (GameStateManager.KuiperBeltObjects(true).Contains(keySpaceBody))
				{
					bodiesToSurvey.AddRange((from x in GameStateManager.KuiperBeltObjects(true)
						where x.habSites.Length > 2
						select x).Except<TINaturalSpaceObjectState>(bodiesToSurvey));
				}
				else if (GameStateManager.Centaurs(true).Contains(keySpaceBody))
				{
					bodiesToSurvey.AddRange((from x in GameStateManager.Centaurs(true)
						where x.habSites.Length > 1
						select x).Except<TINaturalSpaceObjectState>(bodiesToSurvey));
				}
			}
			using (List<TINaturalSpaceObjectState>.Enumerator enumerator = bodiesToSurvey.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TINaturalSpaceObjectState body = enumerator.Current;
					IEnumerable<TISpaceFleetState> enumerable = factions.SelectMany<TIFactionState, TISpaceFleetState>((TIFactionState x) => x.fleets);
					Func<TISpaceFleetState, bool> func;
					Func<TISpaceFleetState, bool> <>9__8;
					if ((func = <>9__8) == null)
					{
						func = (<>9__8 = (TISpaceFleetState x) => x.InSphereOfInfluence(body));
					}
					foreach (TISpaceFleetState tispaceFleetState in enumerable.Where<TISpaceFleetState>(func))
					{
						if (tispaceFleetState.SpaceCombatValue() >= combatScore && !tispaceFleetState.fleetIsLost)
						{
							if (!collectList)
							{
								return false;
							}
							failingFleets.AddUnique(tispaceFleetState);
						}
					}
				}
				goto IL_03A8;
			}
		}
		foreach (TISpaceFleetState tispaceFleetState2 in factions.SelectMany<TIFactionState, TISpaceFleetState>((TIFactionState x) => x.fleets))
		{
			if (tispaceFleetState2.SpaceCombatValue() >= combatScore && !tispaceFleetState2.fleetIsLost)
			{
				if (!collectList)
				{
					return false;
				}
				failingFleets.Add(tispaceFleetState2);
			}
		}
		IL_03A8:
		return !collectList || failingFleets.Count == 0;
	}

	// Token: 0x06000F29 RID: 3881 RVA: 0x0004D240 File Offset: 0x0004B440
	public List<TISpaceAssetState> GetConditionBlockingSpaceAssets(TIFactionState faction)
	{
		if (faction.unlockedVictoryObjective)
		{
			TIVictoryTemplate victoryTemplate = faction.victoryTemplate;
			List<TIVictoryTemplate.VictoryCondition> list = victoryTemplate.victoryConditions.Where<TIVictoryTemplate.VictoryCondition>((TIVictoryTemplate.VictoryCondition x) => x.conditionType > TIVictoryTemplate.VictoryConditionType.none).ToList<TIVictoryTemplate.VictoryCondition>();
			if (list.Any<TIVictoryTemplate.VictoryCondition>())
			{
				StringBuilder stringBuilder = new StringBuilder(Loc.T((list.Count == 1) ? "UI.Objectives.VictoryConditions1" : "UI.Objectives.VictoryConditionsMult")).AppendLine().AppendLine();
				List<TISpaceAssetState> list2 = new List<TISpaceAssetState>();
				foreach (TIVictoryTemplate.VictoryCondition victoryCondition in list)
				{
					List<TISpaceAssetState> list3;
					stringBuilder.AppendLine(victoryTemplate.SingleVictoryConditionDescriptionWithScore(faction, victoryCondition, out list3));
					list2.AddRange(list3);
				}
				return list2;
			}
		}
		return new List<TISpaceAssetState>();
	}

	// Token: 0x04000F3B RID: 3899
	public List<TIVictoryTemplate.VictoryCondition> victoryConditions;

	// Token: 0x04000F3C RID: 3900
	public TIVictoryTemplate.VictoryEffectType victoryEffect;

	// Token: 0x04000F3D RID: 3901
	private readonly List<TIVictoryTemplate.VictoryConditionType> defeatAllHabsCondition = new List<TIVictoryTemplate.VictoryConditionType>
	{
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliens,
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliensAndAllies,
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatEveryone,
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAntiAlienFactions,
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatNonVeryProAlienFactions,
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatExtremists
	};

	// Token: 0x04000F3E RID: 3902
	private readonly List<TIVictoryTemplate.VictoryConditionType> defeatAllBasesCondition = new List<TIVictoryTemplate.VictoryConditionType>
	{
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliens,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliensAndAllies,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatEveryone,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAntiAlienFactions,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatNonVeryProAlienFactions,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatExtremists
	};

	// Token: 0x04000F3F RID: 3903
	private readonly List<TIVictoryTemplate.VictoryConditionType> defeatAllFleetsCondition = new List<TIVictoryTemplate.VictoryConditionType>
	{
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliens,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliensAndAllies,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatEveryone,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAntiAlienFactions,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatNonVeryProAlienFactions,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatExtremists
	};

	// Token: 0x04000F40 RID: 3904
	private readonly List<TIVictoryTemplate.VictoryConditionType> defeatAlienHomeworldCondition = new List<TIVictoryTemplate.VictoryConditionType>
	{
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliens,
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatAliensAndAllies,
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatEveryone,
		TIVictoryTemplate.VictoryConditionType.FreePlanets_DefeatExtremists,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliens,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatAliensAndAllies,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatEveryone,
		TIVictoryTemplate.VictoryConditionType.FreeFleets_DefeatExtremists,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliens,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatAliensAndAllies,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatEveryone,
		TIVictoryTemplate.VictoryConditionType.FreeBases_DefeatExtremists
	};

	// Token: 0x04000F41 RID: 3905
	private readonly List<TIVictoryTemplate.VictoryConditionType> spaceAssetConstructionCondition = new List<TIVictoryTemplate.VictoryConditionType> { TIVictoryTemplate.VictoryConditionType.BasePresence_MajorPlanets };

	// Token: 0x02000BA2 RID: 2978
	public enum VictoryConditionType
	{
		// Token: 0x04004B13 RID: 19219
		none,
		// Token: 0x04004B14 RID: 19220
		GlobalControlPointProportion,
		// Token: 0x04004B15 RID: 19221
		GlobalRegionProportion,
		// Token: 0x04004B16 RID: 19222
		GlobalEcoProportion,
		// Token: 0x04004B17 RID: 19223
		GlobalPopulationProportion,
		// Token: 0x04004B18 RID: 19224
		GlobalPopularityProportion,
		// Token: 0x04004B19 RID: 19225
		GlobalGDPProportion,
		// Token: 0x04004B1A RID: 19226
		GlobalMissionControlCapacity,
		// Token: 0x04004B1B RID: 19227
		AlienNationMaxRegionProportion,
		// Token: 0x04004B1C RID: 19228
		AlienNationMinRegionProportion,
		// Token: 0x04004B1D RID: 19229
		AlienNationMaxPopulationProportion,
		// Token: 0x04004B1E RID: 19230
		AlienNationMinPopulationProportion,
		// Token: 0x04004B1F RID: 19231
		ProAlienMaxRegionProportion,
		// Token: 0x04004B20 RID: 19232
		ProAlienMinRegionProportion,
		// Token: 0x04004B21 RID: 19233
		MinProAlienFleetPower,
		// Token: 0x04004B22 RID: 19234
		MaxProAlienFleetPower,
		// Token: 0x04004B23 RID: 19235
		MaxOtherFactionsFleetPower,
		// Token: 0x04004B24 RID: 19236
		MinGlobalDemocracyWeightedAverage,
		// Token: 0x04004B25 RID: 19237
		MaxGlobalDemocracyWeightedAverage,
		// Token: 0x04004B26 RID: 19238
		MinGlobalInequalityWeightedAverage,
		// Token: 0x04004B27 RID: 19239
		MaxGlobalInequalityWeightedAverage,
		// Token: 0x04004B28 RID: 19240
		FreePlanets_DefeatAliens,
		// Token: 0x04004B29 RID: 19241
		FreePlanets_DefeatAliensAndAllies,
		// Token: 0x04004B2A RID: 19242
		FreePlanets_DefeatEveryone,
		// Token: 0x04004B2B RID: 19243
		FreePlanets_DefeatAntiAlienFactions,
		// Token: 0x04004B2C RID: 19244
		FreePlanets_DefeatNonVeryProAlienFactions,
		// Token: 0x04004B2D RID: 19245
		FreePlanets_DefeatExtremists,
		// Token: 0x04004B2E RID: 19246
		FreeFleets_DefeatAliens,
		// Token: 0x04004B2F RID: 19247
		FreeFleets_DefeatAliensAndAllies,
		// Token: 0x04004B30 RID: 19248
		FreeFleets_DefeatEveryone,
		// Token: 0x04004B31 RID: 19249
		FreeFleets_DefeatAntiAlienFactions,
		// Token: 0x04004B32 RID: 19250
		FreeFleets_DefeatNonVeryProAlienFactions,
		// Token: 0x04004B33 RID: 19251
		FreeFleets_DefeatExtremists,
		// Token: 0x04004B34 RID: 19252
		FreeBases_DefeatAliens,
		// Token: 0x04004B35 RID: 19253
		FreeBases_DefeatAliensAndAllies,
		// Token: 0x04004B36 RID: 19254
		FreeBases_DefeatEveryone,
		// Token: 0x04004B37 RID: 19255
		FreeBases_DefeatAntiAlienFactions,
		// Token: 0x04004B38 RID: 19256
		FreeBases_DefeatNonVeryProAlienFactions,
		// Token: 0x04004B39 RID: 19257
		FreeBases_DefeatExtremists,
		// Token: 0x04004B3A RID: 19258
		BasePresence_MajorPlanets
	}

	// Token: 0x02000BA3 RID: 2979
	public struct VictoryCondition
	{
		// Token: 0x04004B3B RID: 19259
		public TIVictoryTemplate.VictoryConditionType conditionType;

		// Token: 0x04004B3C RID: 19260
		public float value;
	}

	// Token: 0x02000BA4 RID: 2980
	public enum VictoryEffectType
	{
		// Token: 0x04004B3E RID: 19262
		none,
		// Token: 0x04004B3F RID: 19263
		EndGame,
		// Token: 0x04004B40 RID: 19264
		HumanNationsToWinningFaction,
		// Token: 0x04004B41 RID: 19265
		HumanNationsToAlienNation
	}
}
