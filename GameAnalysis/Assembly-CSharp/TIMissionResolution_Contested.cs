using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000262 RID: 610
public class TIMissionResolution_Contested : TIMissionResolution
{
	// Token: 0x17000104 RID: 260
	// (get) Token: 0x060007DE RID: 2014 RVA: 0x00024CCA File Offset: 0x00022ECA
	public override bool automaticSuccess
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x00024CD0 File Offset: 0x00022ED0
	public override TIMissionResult GetMissionOutcome(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f)
	{
		float successChance = this.GetSuccessChance(mission, councilor, target, resourcesSpent, false);
		float num = TIUtilities.RandomFloatValue();
		if (councilor.turned && ((successChance <= councilor.autofailMissionsValue && num <= successChance) || councilor.AutofailTurnedCouncilor(mission, target)))
		{
			float num2 = successChance + 0.01f;
			float num3 = successChance + 0.9f * (1f - successChance);
			num = TIUtilities.RandomRange(num2, num3);
		}
		num = (float)Math.Truncate((double)(num * 1000f)) / 1000f;
		TIMissionResult timissionResult = new TIMissionResult
		{
			roll = num
		};
		if (num <= successChance / 10f)
		{
			timissionResult.outcome = TIMissionOutcome.CriticalSuccess;
		}
		else if (num <= successChance)
		{
			timissionResult.outcome = TIMissionOutcome.Success;
		}
		else
		{
			float num4 = 1f - (1f - successChance) / 10f;
			if (num >= num4)
			{
				timissionResult.outcome = TIMissionOutcome.CriticalFailure;
			}
			else
			{
				timissionResult.outcome = TIMissionOutcome.Failure;
			}
		}
		return timissionResult;
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x00024DA4 File Offset: 0x00022FA4
	public override float GetSuccessChance(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f, bool reValidateTarget = false)
	{
		if ((reValidateTarget && !mission.target.ValidTarget(mission.target.ValidateSingleTarget(mission, councilor, target))) || !councilor.active)
		{
			return 0f;
		}
		float num = this.Difficulty(mission, councilor, target, resourcesSpent);
		float num2 = 0.5f * Mathf.Pow(0.775f, Mathf.Abs(num));
		if (num >= 0f)
		{
			num2 = 1f - num2;
		}
		return num2;
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x00024E12 File Offset: 0x00023012
	public List<TIMissionModifier> GetAttackingNonZeroModifiers(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f)
	{
		return this.GetNonZeroModifiers(mission, true, councilor, target, resourcesSpent);
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x00024E20 File Offset: 0x00023020
	public List<TIMissionModifier> GetDefendingNonZeroModifiers(TIMissionTemplate mission, TICouncilorState councilor = null, TIGameState target = null, float resourcesSpent = 0f)
	{
		return this.GetNonZeroModifiers(mission, false, councilor, target, resourcesSpent);
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x00024E30 File Offset: 0x00023030
	public List<TIMissionModifier> GetAllModifiers(TIMissionTemplate mission, bool attacking, TICouncilorState councilor, TIGameState target, float resourcesSpent)
	{
		List<TIMissionModifier> list = new List<TIMissionModifier>();
		if (attacking)
		{
			list.AddRange(this.attackingModifiers);
			CouncilorAttribute primaryAttackerStat = mission.primaryAttackerStat;
			foreach (TITraitTemplate titraitTemplate in councilor.traits)
			{
				TIMissionModifier_TraitModifier timissionModifier_TraitModifier = new TIMissionModifier_TraitModifier
				{
					trait = titraitTemplate,
					attacking = true,
					attribute = primaryAttackerStat
				};
				if (!list.Contains(timissionModifier_TraitModifier))
				{
					list.Add(timissionModifier_TraitModifier);
				}
			}
			foreach (Context context in mission.attackerContexts)
			{
				if (context != Context.None)
				{
					TIMissionModifier_ContextBased_Attacker timissionModifier_ContextBased_Attacker = new TIMissionModifier_ContextBased_Attacker
					{
						context = context,
						sourceFaction = councilor.faction
					};
					list.Add(timissionModifier_ContextBased_Attacker);
				}
			}
			list.Add(new TIMissionModifier_CampaignDifficulty_Attacker());
		}
		else
		{
			list.AddRange(this.defendingModifiers);
			CouncilorAttribute councilorAttribute = mission.primaryDefenderStat();
			if (target.ref_councilor != null)
			{
				foreach (TITraitTemplate titraitTemplate2 in target.ref_councilor.traits)
				{
					TIMissionModifier_TraitModifier timissionModifier_TraitModifier2 = new TIMissionModifier_TraitModifier
					{
						trait = titraitTemplate2,
						attacking = false,
						attribute = councilorAttribute
					};
					if (!list.Contains(timissionModifier_TraitModifier2))
					{
						list.Add(timissionModifier_TraitModifier2);
					}
				}
			}
			foreach (Context context2 in mission.defenderContexts)
			{
				if (context2 != Context.None)
				{
					list.Add(new TIMissionModifier_ContextBased_Defender
					{
						context = context2,
						sourceFaction = target.ref_faction
					});
				}
			}
			list.Add(new TIMissionModifier_CampaignDifficulty_Defender());
		}
		return list;
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x00025048 File Offset: 0x00023248
	protected List<TIMissionModifier> GetNonZeroModifiers(TIMissionTemplate mission, bool attacking, TICouncilorState councilor, TIGameState target, float resourcesSpent)
	{
		return (from x in this.GetAllModifiers(mission, attacking, councilor, target, resourcesSpent)
			where x.GetModifier(councilor, target, resourcesSpent, FactionResource.None) != 0f
			select x).ToList<TIMissionModifier>();
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x000250A2 File Offset: 0x000232A2
	public float SumAttackingModifiers(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target, float resourcesSpent)
	{
		return this.SumModifiers(mission, this.GetAllModifiers(mission, true, councilor, target, resourcesSpent), councilor, target, resourcesSpent);
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x000250BB File Offset: 0x000232BB
	public float SumDefendingModifiers(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target, float resourcesSpent)
	{
		return this.SumModifiers(mission, this.GetAllModifiers(mission, false, councilor, target, resourcesSpent), councilor, target, resourcesSpent);
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x000250D4 File Offset: 0x000232D4
	private float SumModifiers(TIMissionTemplate mission, List<TIMissionModifier> modifiers, TICouncilorState councilor, TIGameState target, float resourcesSpent)
	{
		float num = 0f;
		foreach (TIMissionModifier timissionModifier in modifiers)
		{
			float num2 = num;
			TIMissionModifier timissionModifier2 = timissionModifier;
			TIMissionCost cost = mission.cost;
			num = num2 + timissionModifier2.GetModifier(councilor, target, resourcesSpent, (cost != null) ? cost.resourceType : FactionResource.None);
		}
		return num;
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x00025144 File Offset: 0x00023344
	public float Difficulty(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target, float resourcesSpent)
	{
		return this.SumAttackingModifiers(mission, councilor, target, resourcesSpent) - this.SumDefendingModifiers(mission, councilor, target, resourcesSpent);
	}

	// Token: 0x0400062E RID: 1582
	public const float scaling = 0.775f;

	// Token: 0x0400062F RID: 1583
	public const float failureChanceAtBalance = 0.5f;

	// Token: 0x04000630 RID: 1584
	public const float criticalCutPoint = 10f;
}
