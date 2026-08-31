using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020002A9 RID: 681
public struct NarrativeEventOutcome
{
	// Token: 0x0600095D RID: 2397 RVA: 0x0002EAE5 File Offset: 0x0002CCE5
	public bool ReportOutcome(TIGameState targetState)
	{
		return true;
	}

	// Token: 0x0600095E RID: 2398 RVA: 0x0002EAE8 File Offset: 0x0002CCE8
	public float GetModifiedWeight(TIFactionState actingFaction, TIGameState target, TIGameState secondary)
	{
		if (this.weight <= 0f)
		{
			return 0f;
		}
		float num = this.weight;
		if (actingFaction != null && this.facWtMod != null)
		{
			foreach (NarrativeEventWeightModifier narrativeEventWeightModifier in this.facWtMod)
			{
				if (narrativeEventWeightModifier.condition != null && NarrativeEventOption.PassesActorCondition(actingFaction, narrativeEventWeightModifier.condition))
				{
					num += narrativeEventWeightModifier.value;
				}
			}
		}
		if (this.tarWtMod != null)
		{
			foreach (NarrativeEventWeightModifier narrativeEventWeightModifier2 in this.tarWtMod)
			{
				if (narrativeEventWeightModifier2.condition != null && NarrativeEventOption.PassesTargetCondition(target, narrativeEventWeightModifier2.condition))
				{
					num += narrativeEventWeightModifier2.value;
				}
			}
		}
		if (this.secWtMod != null && secondary != null)
		{
			foreach (NarrativeEventWeightModifier narrativeEventWeightModifier3 in this.secWtMod)
			{
				if (narrativeEventWeightModifier3.condition != null && NarrativeEventOption.PassesTargetCondition(secondary, narrativeEventWeightModifier3.condition))
				{
					num += narrativeEventWeightModifier3.value;
				}
			}
		}
		return num;
	}

	// Token: 0x0600095F RID: 2399 RVA: 0x0002EC50 File Offset: 0x0002CE50
	public TIResourcesCost GetCosts(TIGameState targetState)
	{
		return this.costBuilder.ToResourcesCost(this.GetCostMultiplier(targetState));
	}

	// Token: 0x06000960 RID: 2400 RVA: 0x0002EC64 File Offset: 0x0002CE64
	public TIResourcesCost GetRawCosts()
	{
		return this.costBuilder.ToResourcesCost(1f);
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000961 RID: 2401 RVA: 0x0002EC76 File Offset: 0x0002CE76
	public TIProjectTemplate projectGranted
	{
		get
		{
			if (!string.IsNullOrEmpty(this.projectGrantedTemplateName))
			{
				TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(this.projectGrantedTemplateName, false);
				if (tiprojectTemplate == null)
				{
					Log.Error("Bad projectTemplateName" + this.projectGrantedTemplateName + "in NarrativeEventOutcome", Array.Empty<object>());
				}
				return tiprojectTemplate;
			}
			return null;
		}
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000962 RID: 2402 RVA: 0x0002ECB5 File Offset: 0x0002CEB5
	public TIOrgTemplate orgGranted
	{
		get
		{
			if (!string.IsNullOrEmpty(this.orgGrantedTemplateName))
			{
				TIOrgTemplate tiorgTemplate = TemplateManager.Find<TIOrgTemplate>(this.orgGrantedTemplateName, false);
				if (tiorgTemplate == null)
				{
					Log.Error("Bad orgTemplateName " + this.orgGrantedTemplateName + " in NarrativeEventOutcome", Array.Empty<object>());
				}
				return tiorgTemplate;
			}
			return null;
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000963 RID: 2403 RVA: 0x0002ECF4 File Offset: 0x0002CEF4
	public List<TINarrativeEventTemplate> eventsToAdd
	{
		get
		{
			List<TINarrativeEventTemplate> list = new List<TINarrativeEventTemplate>();
			if (this.addNarrativeEvents != null)
			{
				foreach (string text in this.addNarrativeEvents)
				{
					if (!string.IsNullOrEmpty(text))
					{
						TINarrativeEventTemplate tinarrativeEventTemplate = TemplateManager.Find<TINarrativeEventTemplate>(text, false);
						if (!list.Contains(tinarrativeEventTemplate))
						{
							list.Add(tinarrativeEventTemplate);
						}
					}
				}
			}
			return list;
		}
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000964 RID: 2404 RVA: 0x0002ED70 File Offset: 0x0002CF70
	public List<TINarrativeEventTemplate> eventsToRemove
	{
		get
		{
			List<TINarrativeEventTemplate> list = new List<TINarrativeEventTemplate>();
			if (this.removeNarrativeEvents != null)
			{
				foreach (string text in this.removeNarrativeEvents)
				{
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(TemplateManager.Find<TINarrativeEventTemplate>(text, false));
					}
				}
			}
			return list;
		}
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000965 RID: 2405 RVA: 0x0002EDE0 File Offset: 0x0002CFE0
	public List<TIEffectTemplate> effectTemplates
	{
		get
		{
			List<TIEffectTemplate> list = new List<TIEffectTemplate>();
			if (this.effectTemplateNames != null)
			{
				foreach (string text in this.effectTemplateNames)
				{
					if (!string.IsNullOrEmpty(text))
					{
						TIEffectTemplate tieffectTemplate = TemplateManager.Find<TIEffectTemplate>(text, false);
						if (tieffectTemplate == null)
						{
							Log.Error("Bad effectTemplateName " + text + " in NarrativeEventOption", Array.Empty<object>());
						}
						list.Add(tieffectTemplate);
					}
				}
			}
			return list;
		}
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000966 RID: 2406 RVA: 0x0002EE70 File Offset: 0x0002D070
	public List<TIEffectTemplate> delayedEffectTemplates
	{
		get
		{
			List<TIEffectTemplate> list = new List<TIEffectTemplate>();
			if (this.delayedEffectTemplateNames != null)
			{
				foreach (string text in this.delayedEffectTemplateNames)
				{
					if (!string.IsNullOrEmpty(text))
					{
						TIEffectTemplate tieffectTemplate = TemplateManager.Find<TIEffectTemplate>(text, false);
						if (tieffectTemplate == null)
						{
							Log.Error("Bad effectTemplateName " + text + " in NarrativeEventOption", Array.Empty<object>());
						}
						list.Add(tieffectTemplate);
					}
				}
			}
			return list;
		}
	}

	// Token: 0x06000967 RID: 2407 RVA: 0x0002EF00 File Offset: 0x0002D100
	public float GetCostMultiplier(TIGameState targetState)
	{
		switch (this.costMultiplier)
		{
		case CostMultiplier.region_popMillions:
		{
			TIRegionState ref_region = targetState.ref_region;
			if (ref_region == null)
			{
				return 10f;
			}
			return ref_region.populationInMillions;
		}
		case CostMultiplier.nation_numControlPoints:
		{
			TINationState ref_nation = targetState.ref_nation;
			return (float)((ref_nation != null) ? ref_nation.numControlPoints : 1);
		}
		case CostMultiplier.nation_numRegions:
		{
			TINationState ref_nation2 = targetState.ref_nation;
			return (float)((ref_nation2 != null) ? ref_nation2.regions.Count : 1);
		}
		case CostMultiplier.nation_spaceProgramSize:
			if (targetState.ref_nation != null)
			{
				return Mathf.Max(1f, targetState.ref_nation.boostIncome_year_dekatons / 5f) + (float)targetState.ref_nation.missionControl / 2f + (float)targetState.ref_nation.regions.Count<TIRegionState>((TIRegionState x) => x.antiSpaceDefenses);
			}
			return 1f;
		case CostMultiplier.nation_democracy:
		{
			TINationState ref_nation3 = targetState.ref_nation;
			if (ref_nation3 == null)
			{
				return 10f;
			}
			return ref_nation3.democracy;
		}
		case CostMultiplier.nation_autocracy:
		{
			float num = (float)10;
			TINationState ref_nation4 = targetState.ref_nation;
			return (num - ((ref_nation4 != null) ? new float?(ref_nation4.democracy) : null)).GetValueOrDefault();
		}
		case CostMultiplier.nation_miltech:
		{
			TINationState ref_nation5 = targetState.ref_nation;
			if (ref_nation5 == null)
			{
				return 3.5f;
			}
			return ref_nation5.militaryTechLevel;
		}
		case CostMultiplier.nation_education:
		{
			TINationState ref_nation6 = targetState.ref_nation;
			if (ref_nation6 == null)
			{
				return 6f;
			}
			return ref_nation6.education;
		}
		case CostMultiplier.nation_inequality:
		{
			TINationState ref_nation7 = targetState.ref_nation;
			if (ref_nation7 == null)
			{
				return 8f;
			}
			return ref_nation7.inequality;
		}
		case CostMultiplier.nation_equality:
		{
			float num = (float)9;
			TINationState ref_nation8 = targetState.ref_nation;
			float? num2 = num - ((ref_nation8 != null) ? new float?(ref_nation8.inequality) : null);
			if (num2 == null)
			{
				return 1f;
			}
			return num2.GetValueOrDefault();
		}
		case CostMultiplier.global_campaignDuration:
			return Mathf.Max(1f, TITimeState.CampaignDuration_years_Exact() / TemplateManager.global.duration_scaling_divisor);
		case CostMultiplier.global_GDP:
			return Mathf.Pow((float)TIGlobalValuesState.globalGDP, 0.33f) / 30000f;
		case CostMultiplier.global_fissionTechLevel_discount:
			return Mathf.Max((10f - TIEffectsState.SumEffectsModifiers(Context.GlobalFissionTechLevel, targetState.ref_faction, 0f, null)) / 10f, 0.5f);
		case CostMultiplier.global_fusionTechLevel_discount:
			return Mathf.Max((10f - TIEffectsState.SumEffectsModifiers(Context.GlobalFusionTechLevel, targetState.ref_faction, 0f, null)) / 10f, 0.5f);
		case CostMultiplier.hab_tier:
		{
			TIHabState ref_hab = targetState.ref_hab;
			return (float)((ref_hab != null) ? ref_hab.tier : 1);
		}
		case CostMultiplier.hab_sectors:
		{
			TIHabState ref_hab2 = targetState.ref_hab;
			return (float)((ref_hab2 != null) ? ref_hab2.sectors.Count : 1);
		}
		case CostMultiplier.hab_modules:
		{
			TIHabState ref_hab3 = targetState.ref_hab;
			return (float)((ref_hab3 != null) ? ref_hab3.numCompletedModules : 1);
		}
		case CostMultiplier.faction_numControlPointsWeighted:
		{
			TIFactionState ref_faction = targetState.ref_faction;
			if (ref_faction == null)
			{
				return 1f;
			}
			return ref_faction.GetAnnualControlPointMaintenanceCost();
		}
		case CostMultiplier.army_miltech:
		{
			TIArmyState ref_army = targetState.ref_army;
			if (ref_army == null)
			{
				return 3.5f;
			}
			return ref_army.techLevel;
		}
		case CostMultiplier.globalTemperatureAnomaly_C:
			return Mathf.Max(1f, Mathf.Abs(TIGlobalValuesState.GlobalValues.temperatureAnomaly_C));
		case CostMultiplier.globalSeaLevelAnomaly_cm:
			return Mathf.Max(1f, TIGlobalValuesState.GlobalValues.globalSeaLevelAnomaly_cm);
		default:
			return 1f;
		}
	}

	// Token: 0x040007D3 RID: 2003
	public ResourceCostBuilder costBuilder;

	// Token: 0x040007D4 RID: 2004
	public CostMultiplier costMultiplier;

	// Token: 0x040007D5 RID: 2005
	public List<string> effectTemplateNames;

	// Token: 0x040007D6 RID: 2006
	public List<string> delayedEffectTemplateNames;

	// Token: 0x040007D7 RID: 2007
	public List<string> addNarrativeEvents;

	// Token: 0x040007D8 RID: 2008
	public List<string> removeNarrativeEvents;

	// Token: 0x040007D9 RID: 2009
	public string projectGrantedTemplateName;

	// Token: 0x040007DA RID: 2010
	public string orgGrantedTemplateName;

	// Token: 0x040007DB RID: 2011
	public float weight;

	// Token: 0x040007DC RID: 2012
	public List<NarrativeEventWeightModifier> facWtMod;

	// Token: 0x040007DD RID: 2013
	public List<NarrativeEventWeightModifier> tarWtMod;

	// Token: 0x040007DE RID: 2014
	public List<NarrativeEventWeightModifier> secWtMod;

	// Token: 0x040007DF RID: 2015
	public bool AIFavored;

	// Token: 0x040007E0 RID: 2016
	public bool forceAlert;
}
