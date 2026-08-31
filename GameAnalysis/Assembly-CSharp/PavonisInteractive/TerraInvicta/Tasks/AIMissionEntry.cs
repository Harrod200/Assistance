using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000932 RID: 2354
	public class AIMissionEntry
	{
		// Token: 0x17000F48 RID: 3912
		// (get) Token: 0x06005A10 RID: 23056 RVA: 0x0029D91A File Offset: 0x0029BB1A
		public TIFactionState faction
		{
			get
			{
				TICouncilorState ticouncilorState = this.councilor;
				if (ticouncilorState == null)
				{
					return null;
				}
				return ticouncilorState.faction;
			}
		}

		// Token: 0x17000F49 RID: 3913
		// (get) Token: 0x06005A11 RID: 23057 RVA: 0x0029D92D File Offset: 0x0029BB2D
		public float estimatedFinalSuccessChance
		{
			get
			{
				return (this.successChanceLow + this.successChanceHigh) / 2f;
			}
		}

		// Token: 0x17000F4A RID: 3914
		// (get) Token: 0x06005A12 RID: 23058 RVA: 0x0029D942 File Offset: 0x0029BB42
		public bool FailureIsOk
		{
			get
			{
				return this.mission.targetEffects.Any<TIMissionEffect>((TIMissionEffect x) => x is TIMissionEffect_Dominate);
			}
		}

		// Token: 0x17000F4B RID: 3915
		// (get) Token: 0x06005A13 RID: 23059 RVA: 0x0029D978 File Offset: 0x0029BB78
		public bool isTooRisky
		{
			get
			{
				return this.successChanceHigh < this.acceptableMinimumSuccess && !this.FailureIsOk;
			}
		}

		// Token: 0x06005A14 RID: 23060 RVA: 0x0029D994 File Offset: 0x0029BB94
		public AIMissionEntry(AICouncilorMissionPlanner planner, TIMissionTemplate mission_, TICouncilorState councilor_, TIGameState target_, float riskAversion, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, bool objective_, float campaignDuration_years, List<CampaignMilestone> desiredMilestones, bool huntingForAlienActivity, float huntAbility, List<TIFactionState> warFactions, TIRegionState recentAlienSite, float timeSinceAlienSite_days, float availableResource, bool capturingNeutralNations, float basePayoff = -1f)
		{
			this.mission = mission_;
			this.target = target_;
			this.councilor = councilor_;
			this.objective = objective_;
			this.sliderSteps = this.councilor.CurrentMaxSliderSteps(this.mission, 1f);
			float num = (this.mission.hasCost ? (availableResource * 0.333f / this.councilor.faction.GetCurrentResourceAmount(this.mission.cost.resourceType)) : 1f);
			this.maxAffordableSliderSteps = this.councilor.CurrentMaxSliderSteps(this.mission, num);
			this.acceptableMinimumSuccess = 0.4f * riskAversion - Mathf.Min(this.payoff / 80000f, 0.2f);
			if (!this.objective)
			{
				this.acceptableMinimumSuccess = Mathf.Max(this.acceptableMinimumSuccess, 0.15f);
				if (campaignDuration_years < 0.25f)
				{
					this.acceptableMinimumSuccess += 0.5f - campaignDuration_years * 2f;
				}
			}
			this.successChanceLow = this.mission.resolutionMethod.GetSuccessChance(this.mission, this.councilor, this.target, 0f, false);
			if (this.maxAffordableSliderSteps > 0)
			{
				float cost = this.mission.cost.GetCost((float)this.maxAffordableSliderSteps, this.councilor, null);
				this.successChanceHigh = this.mission.resolutionMethod.GetSuccessChance(this.mission, this.councilor, this.target, cost, false);
			}
			else
			{
				this.successChanceHigh = this.successChanceLow;
			}
			this.payoff = basePayoff;
			if (this.payoff < 0f)
			{
				this.payoff = planner.GetPayoffForMissionTarget(this.councilor.faction, this.mission, this.councilor, this.target, requiredMissions, missingRequiredMissions, null, desiredMilestones, campaignDuration_years, huntingForAlienActivity, huntAbility, warFactions, recentAlienSite, timeSinceAlienSite_days, capturingNeutralNations);
			}
			if (this.payoff >= 200000f)
			{
				this.payoff *= 1f;
			}
			this.payoff = Mathf.Pow(this.payoff, 1f);
			float num2 = 1f - this.estimatedFinalSuccessChance;
			this.expectedUtility = this.payoff * this.estimatedFinalSuccessChance * (1f - num2 * (2f * riskAversion - 1f));
		}

		// Token: 0x06005A15 RID: 23061 RVA: 0x0029DBEB File Offset: 0x0029BDEB
		public AIMissionEntry()
		{
		}

		// Token: 0x04004107 RID: 16647
		private const float extraCarefulAIDuration_years = 0.25f;

		// Token: 0x04004108 RID: 16648
		public TICouncilorState councilor;

		// Token: 0x04004109 RID: 16649
		public TIMissionTemplate mission;

		// Token: 0x0400410A RID: 16650
		public TIGameState target;

		// Token: 0x0400410B RID: 16651
		public int sliderSteps;

		// Token: 0x0400410C RID: 16652
		public float payoff;

		// Token: 0x0400410D RID: 16653
		public float expectedUtility;

		// Token: 0x0400410E RID: 16654
		public float acceptableMinimumSuccess;

		// Token: 0x0400410F RID: 16655
		public float successChanceHigh;

		// Token: 0x04004110 RID: 16656
		public float successChanceLow;

		// Token: 0x04004111 RID: 16657
		public float finalSuccessChance;

		// Token: 0x04004112 RID: 16658
		public bool objective;

		// Token: 0x04004113 RID: 16659
		public int maxAffordableSliderSteps;

		// Token: 0x04004114 RID: 16660
		public bool finalized;
	}
}
