using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000763 RID: 1891
	public class TIMissionState : TIGameState
	{
		// Token: 0x0600350E RID: 13582 RVA: 0x0012F3D4 File Offset: 0x0012D5D4
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			TIMissionTemplate timissionTemplate = template as TIMissionTemplate;
			if (timissionTemplate == null)
			{
				return;
			}
			this.templateName = timissionTemplate.dataName;
			this.displayName = template.displayName;
		}

		// Token: 0x0600350F RID: 13583 RVA: 0x0012F40B File Offset: 0x0012D60B
		public override void PostGlobalGameStateCreateInit_2()
		{
			base.PostGlobalGameStateCreateInit_2();
			if (this.resolveTimeAssigned)
			{
				this.ListenForResolutionTime();
			}
		}

		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x06003510 RID: 13584 RVA: 0x0012F421 File Offset: 0x0012D621
		public override TIFactionState ref_faction
		{
			get
			{
				return this.councilor.faction;
			}
		}

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x06003511 RID: 13585 RVA: 0x0012F42E File Offset: 0x0012D62E
		public override TIHabState ref_hab
		{
			get
			{
				return this.target.ref_hab;
			}
		}

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x06003512 RID: 13586 RVA: 0x0012F43B File Offset: 0x0012D63B
		public override TINationState ref_nation
		{
			get
			{
				return this.target.ref_nation;
			}
		}

		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x06003513 RID: 13587 RVA: 0x0012F448 File Offset: 0x0012D648
		public override TIRegionState ref_region
		{
			get
			{
				return this.target.ref_region;
			}
		}

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x06003514 RID: 13588 RVA: 0x0012F455 File Offset: 0x0012D655
		public override TISpaceFleetState ref_fleet
		{
			get
			{
				return this.target.ref_fleet;
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x06003515 RID: 13589 RVA: 0x0012F462 File Offset: 0x0012D662
		public override TIOrbitState ref_orbit
		{
			get
			{
				return this.target.ref_orbit;
			}
		}

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x06003516 RID: 13590 RVA: 0x0012F46F File Offset: 0x0012D66F
		public override TIControlPoint ref_controlPoint
		{
			get
			{
				return this.target.ref_controlPoint;
			}
		}

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x06003517 RID: 13591 RVA: 0x0012F47C File Offset: 0x0012D67C
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.target.ref_spaceBody;
			}
		}

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x06003518 RID: 13592 RVA: 0x0012F489 File Offset: 0x0012D689
		public override TICouncilorState ref_councilor
		{
			get
			{
				return this.councilor;
			}
		}

		// Token: 0x06003519 RID: 13593 RVA: 0x0012F491 File Offset: 0x0012D691
		public new TIMissionTemplate GetMyTemplate()
		{
			return this.missionTemplate;
		}

		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x0600351A RID: 13594 RVA: 0x0012F499 File Offset: 0x0012D699
		public string getMissionEventName
		{
			get
			{
				return new StringBuilder("ResolveMissionOrder").Append((int)base.ID).ToString();
			}
		}

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x0600351B RID: 13595 RVA: 0x0012F4BA File Offset: 0x0012D6BA
		public string getDetectEventName
		{
			get
			{
				return new StringBuilder("DetectMissionOrder").Append((int)base.ID).ToString();
			}
		}

		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x0600351C RID: 13596 RVA: 0x0012F4DB File Offset: 0x0012D6DB
		public TIMissionTemplate missionTemplate
		{
			get
			{
				if (this._missionTemplate == null)
				{
					this._missionTemplate = this.GetMyTemplate<TIMissionTemplate>();
				}
				return this._missionTemplate;
			}
		}

		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x0600351D RID: 13597 RVA: 0x0012F4F8 File Offset: 0x0012D6F8
		public float getResolutionOrder
		{
			get
			{
				float num = (float)this.missionTemplate.resolutionOrder;
				if (this.missionTemplate.resolutionMethod != null)
				{
					float num2 = Mathf.Clamp(this.missionTemplate.resolutionMethod.GetSuccessChance(this.missionTemplate, this.councilor, this.target, this.resources, false), 0.0001f, 0.9999f);
					if (num > 0f)
					{
						num += 1f - num2;
					}
				}
				return num;
			}
		}

		// Token: 0x0600351E RID: 13598 RVA: 0x0012F56C File Offset: 0x0012D76C
		public TIGameState GetInitialMissionLocation()
		{
			switch (this.missionTemplate.movementRule)
			{
			case MissionMovementRule.MoveToTarget:
				return this.targetLocation;
			case MissionMovementRule.MoveToLaunchSite:
				if (this.councilor.OnEarth)
				{
					TIRegionSpaceFacilityState tiregionSpaceFacilityState = this.councilor.faction.SelectRandomLaunchSite();
					return ((tiregionSpaceFacilityState != null) ? tiregionSpaceFacilityState.ref_region : null) ?? GameStateManager.MapRegionLookup("map_Astana");
				}
				return this.councilor.location;
			}
			return this.councilor.location;
		}

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x0600351F RID: 13599 RVA: 0x0012F5F8 File Offset: 0x0012D7F8
		public TIGameState targetLocation
		{
			get
			{
				if (this.target.isCouncilorState)
				{
					return TIUtilities.ObjectToExactLocation(TIMissionPhaseState.CouncilorLastKnownLocation(this.councilor.faction, this.target.ref_councilor));
				}
				if (!this.target.isSpaceFleetState)
				{
					return TIUtilities.ObjectToExactLocation(this.target);
				}
				if (this.councilor.OnAShip && this.councilor.location.ref_fleet == this.target)
				{
					return this.councilor.location;
				}
				return (from x in this.target.ref_fleet.ships
					orderby x.HasSpecialModuleRule(SpecialModuleRule.ReduceFleetMCConsumption, false) descending, x.dryMass_tons descending
					select x).First<TISpaceShipState>();
			}
		}

		// Token: 0x06003520 RID: 13600 RVA: 0x0012F6E2 File Offset: 0x0012D8E2
		public void ListenForResolutionTime()
		{
			this.resolveTimeAssigned = true;
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ResolveMissionOrder), this.getMissionEventName, null, false, false);
		}

		// Token: 0x06003521 RID: 13601 RVA: 0x0012F70C File Offset: 0x0012D90C
		public void MissionResolved()
		{
			if (TIGameState.Valid(this.councilor))
			{
				this.councilor.ClearActiveMission();
				this.councilor.SetCompletedMission(this);
				this.councilor.SetPriorMission(this.missionTemplate, this.target);
			}
			this.resolveTimeAssigned = false;
			this.resolveTime = null;
			this.startTime = null;
			GameStateManager.MissionPhase().currentlyResolvingMissions.Remove(this);
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.ResolveMissionOrder), this.getMissionEventName);
		}

		// Token: 0x06003522 RID: 13602 RVA: 0x0012F796 File Offset: 0x0012D996
		public void ResolveMissionOrder(TimeEventStart e)
		{
			this.ResolveMission(TIMissionState.AbortReason.None, "");
		}

		// Token: 0x06003523 RID: 13603 RVA: 0x0012F7A8 File Offset: 0x0012D9A8
		public float GetSuccessChance()
		{
			if (this.missionTemplate != null && this.councilor != null && this.councilor.location != null && this.target != null)
			{
				return this.missionTemplate.resolutionMethod.GetSuccessChance(this.missionTemplate, this.councilor, this.target, this.resources, false);
			}
			return 0f;
		}

		// Token: 0x06003524 RID: 13604 RVA: 0x0012F81C File Offset: 0x0012DA1C
		public static string DumpModifiers(TIMissionTemplate missionTemplate, TICouncilorState councilor, TIGameState target, float resources, string missionTemplateName = "")
		{
			if (missionTemplateName == "" || missionTemplate.dataName == missionTemplateName)
			{
				TIMissionResolution_Contested timissionResolution_Contested = missionTemplate.resolutionMethod as TIMissionResolution_Contested;
				if (timissionResolution_Contested != null)
				{
					StringBuilder stringBuilder = new StringBuilder(string.Concat(new string[]
					{
						missionTemplate.displayName,
						" ",
						councilor.displayName,
						"(",
						councilor.faction.displayName,
						"): ",
						target.displayName,
						" ",
						missionTemplate.resolutionMethod.GetSuccessChanceString(missionTemplate, councilor, target, resources, false, 2)
					})).AppendLine();
					stringBuilder.AppendLine("#Attacking Modifiers: " + timissionResolution_Contested.SumAttackingModifiers(missionTemplate, councilor, target, resources).ToString());
					foreach (TIMissionModifier timissionModifier in timissionResolution_Contested.GetAllModifiers(missionTemplate, true, councilor, target, resources))
					{
						stringBuilder.AppendLine(timissionModifier.displayName + ": " + timissionModifier.GetModifier(councilor, target, resources, missionTemplate.primaryResource).ToString());
					}
					stringBuilder.AppendLine("#Defending Modifiers: " + timissionResolution_Contested.SumDefendingModifiers(missionTemplate, councilor, target, resources).ToString());
					foreach (TIMissionModifier timissionModifier2 in timissionResolution_Contested.GetAllModifiers(missionTemplate, false, councilor, target, resources))
					{
						stringBuilder.AppendLine(timissionModifier2.displayName + ": " + timissionModifier2.GetModifier(councilor, target, resources, missionTemplate.primaryResource).ToString());
					}
					stringBuilder.AppendLine("Net Difficulty: " + (-timissionResolution_Contested.Difficulty(missionTemplate, councilor, target, resources)).ToString("N3"));
					return stringBuilder.AppendLine().ToString();
				}
			}
			return "";
		}

		// Token: 0x06003525 RID: 13605 RVA: 0x0012FA38 File Offset: 0x0012DC38
		public MissionResult ResolveMission(TIMissionState.AbortReason abortReason = TIMissionState.AbortReason.None, string abortReasonDetail = "")
		{
			if (!GameStateManager.MissionPhase().currentlyResolvingMissions.Contains(this))
			{
				GameStateManager.MissionPhase().currentlyResolvingMissions.Add(this);
			}
			MissionResult missionResult = new MissionResult
			{
				councilor = this.councilor,
				missionTemplate = this.missionTemplate,
				noiseModifier = 0f
			};
			TIFactionState faction = this.councilor.faction;
			if (this.councilor.status != CouncilorStatus.Active || !TIGameState.Valid(this.councilor.activeMission) || !TIGameState.Valid(faction))
			{
				missionResult.missionOutcome = TIMissionOutcome.None;
				this.missionOutcome = missionResult.missionOutcome;
				this.MissionResolved();
				return missionResult;
			}
			bool flag = abortReason > TIMissionState.AbortReason.None;
			if (!flag && this.councilor.ref_faction.player.isAI && AIEvaluators.AI_ShouldAbortBadMission(this))
			{
				flag = true;
				abortReason = TIMissionState.AbortReason.VoluntaryAbort;
			}
			TICouncilorState ref_councilor = this.target.ref_councilor;
			if (!flag && !this.councilor.CheckAndChaseMissionTarget())
			{
				missionResult.missionOutcome = TIMissionOutcome.Aborted;
				this.missionOutcome = missionResult.missionOutcome;
				this.MissionResolved();
				return missionResult;
			}
			TIMissionResolution resolutionMethod = this.missionTemplate.resolutionMethod;
			if (!flag)
			{
				if (!this.councilor.active)
				{
					flag = true;
					abortReason = ((this.councilor.status == CouncilorStatus.Dead) ? TIMissionState.AbortReason.CouncilorDead : TIMissionState.AbortReason.CouncilorUnavailable);
				}
				else if (!TIGameState.Valid(this.target))
				{
					flag = true;
					abortReason = TIMissionState.AbortReason.TargetInvalid;
				}
				else
				{
					List<string> list = this.missionTemplate.target.ValidateSingleTarget(this.missionTemplate, this.councilor, this.target);
					if (!this.missionTemplate.target.ValidTarget(list))
					{
						flag = true;
						abortReason = TIMissionState.AbortReason.UseDetail;
						abortReasonDetail = MarkerController.BuildInvalidTargetTooltip(list);
					}
				}
			}
			if (flag && !this.missionTemplate.debugForced)
			{
				missionResult.missionOutcome = TIMissionOutcome.Aborted;
				this.missionOutcome = missionResult.missionOutcome;
				if (this.missionTemplate.hasCost)
				{
					faction.AddToCurrentResource(this.resources, this.missionTemplate.cost.resourceType, false, null);
				}
			}
			else
			{
				float successChance = resolutionMethod.GetSuccessChance(this.missionTemplate, this.councilor, this.target, this.resources, false);
				missionResult.successChance = successChance;
				TIMissionResult timissionResult = resolutionMethod.GetMissionOutcome(this.missionTemplate, this.councilor, this.target, this.resources);
				if (faction.isActivePlayer && !resolutionMethod.automaticSuccess)
				{
					TIMissionOutcome outcome = timissionResult.outcome;
					if (outcome - TIMissionOutcome.CriticalFailure > 1)
					{
						if (outcome - TIMissionOutcome.Success <= 1)
						{
							Mood.GoodNews();
						}
					}
					else
					{
						Mood.BadNews();
					}
				}
				if (faction != null && faction.isActivePlayer && successChance >= 0.99f && (timissionResult.outcome == TIMissionOutcome.Failure || timissionResult.outcome == TIMissionOutcome.CriticalFailure))
				{
					faction.UnlockAchievement("failEasyMission");
				}
				if (TemplateManager.global.debug_noMissionFail)
				{
					timissionResult = new TIMissionResult
					{
						outcome = TIMissionOutcome.Success,
						roll = 0f
					};
				}
				else if (TemplateManager.global.debug_alwaysCritFail)
				{
					timissionResult = new TIMissionResult
					{
						outcome = TIMissionOutcome.CriticalFailure,
						roll = 1f
					};
				}
				missionResult.missionOutcome = timissionResult.outcome;
				if (resolutionMethod.automaticSuccess)
				{
					missionResult.roll = 0f;
				}
				else
				{
					missionResult.roll = timissionResult.roll;
				}
				missionResult.target = this.target;
			}
			List<TIGameState> list2 = new List<TIGameState>();
			List<TIGameState> list3 = new List<TIGameState>();
			TIFactionState ref_faction = this.target.ref_faction;
			TICouncilorState ref_councilor2 = this.target.ref_councilor;
			if (missionResult.Attempted)
			{
				MissionMovementRule movementRule = this.missionTemplate.movementRule;
				if (movementRule != MissionMovementRule.MoveUponAttempt)
				{
					if (movementRule == MissionMovementRule.MoveWhenSuccessful)
					{
						string text;
						if (missionResult.Success && this.councilor.ValidDestination(this.targetLocation, out text))
						{
							this.councilor.ChangeLocation(this.targetLocation);
						}
					}
				}
				else
				{
					this.councilor.ChangeLocation(this.targetLocation);
				}
				TINationState ref_nation = this.target.ref_nation;
				List<TIFactionState> list4;
				if (!this.target.isCouncilorState)
				{
					list4 = this.target.ref_factions;
				}
				else
				{
					(list4 = new List<TIFactionState>()).Add(this.target.ref_faction);
				}
				List<TIFactionState> list5 = list4;
				if (ref_nation != null)
				{
					list2 = ref_nation.controlPointOwnersByPoint;
				}
				if (missionResult.missionOutcome == TIMissionOutcome.CriticalFailure)
				{
					TITraitTemplate.ProcessLoyaltyChangeFromTraits(faction, SpecialTraitRule.LoyaltyLossOnFactionCriticalFailure, 1);
					TITraitTemplate.ProcessLoyaltyChangeFromTraits(this.councilor, SpecialTraitRule.LoyaltyLossOnPersonalCritFailure, 1);
				}
				if (this.missionTemplate.targetEffects != null)
				{
					foreach (TIMissionEffect timissionEffect in this.missionTemplate.targetEffects)
					{
						string text2 = timissionEffect.ApplyEffect(this, this.target, missionResult.missionOutcome);
						if (string.IsNullOrEmpty(missionResult.valueChange) && text2 != string.Empty)
						{
							missionResult.valueChange = text2;
						}
					}
				}
				if (this.missionTemplate.councilorEffects != null)
				{
					foreach (TIMissionEffect timissionEffect2 in this.missionTemplate.councilorEffects)
					{
						string text3 = timissionEffect2.ApplyEffect(this, this.councilor, missionResult.missionOutcome);
						if (string.IsNullOrEmpty(missionResult.valueChange) && text3 != string.Empty)
						{
							missionResult.valueChange = text3;
						}
					}
				}
				if (ref_nation != null)
				{
					list3 = this.target.ref_nation.controlPointOwnersByPoint;
				}
				int num;
				if (this.missionTemplate.specialPost && this.target.isRegionXenoformingState)
				{
					num = (missionResult.Success ? 2 : 1);
				}
				else
				{
					num = (missionResult.Success ? this.missionTemplate.XPonSuccess : (this.missionTemplate.XPonSuccess / 2));
				}
				float phasesPerMonth = TIMissionPhaseState.phasesPerMonth;
				if (phasesPerMonth == 1f)
				{
					num *= 2;
				}
				else if (phasesPerMonth > 1f && phasesPerMonth < 2f)
				{
					num = (int)Math.Ceiling((double)((float)num * 4f / 3f));
				}
				this.councilor.ChangeXP(num);
				float num2 = this.missionTemplate.hate[(int)missionResult.missionOutcome];
				Func<TICouncilorState, bool> <>9__0;
				foreach (TIFactionState tifactionState in list5)
				{
					if (tifactionState != faction && missionResult.Attempted)
					{
						if (this.missionTemplate.specialPost && this.target.isRegionXenoformingState)
						{
							tifactionState.GainFactionHate(faction, TemplateManager.global.factionHateForBurnXenoforming, false, "Xenoforming burned", true);
						}
						else if (faction.IsAlienFaction && !tifactionState.CanDetectAlienMission(this.missionTemplate))
						{
							tifactionState.GainFactionHate(GameStateManager.AlienProxy(), num2, false, "Blaming Servants for Alien Mission", true);
						}
						else if (num2 == 0f && tifactionState.intelSharingFactions.Contains(faction))
						{
							num2 = this.missionTemplate.hate.Max();
							tifactionState.GainFactionHate(faction, num2, false, this.missionTemplate.displayName + " targets faction", true);
						}
						else
						{
							if (num2 == 0f)
							{
								IEnumerable<TICouncilorState> turnedCouncilors = tifactionState.turnedCouncilors;
								Func<TICouncilorState, bool> func;
								if ((func = <>9__0) == null)
								{
									func = (<>9__0 = (TICouncilorState x) => x.faction == faction);
								}
								if (turnedCouncilors.Any<TICouncilorState>(func))
								{
									if (missionResult.Success)
									{
										num2 = Mathf.Max(this.missionTemplate.hate[4], this.missionTemplate.hate[5]);
									}
									else if (missionResult.Failed)
									{
										num2 = Mathf.Max(this.missionTemplate.hate[2], this.missionTemplate.hate[1]);
									}
									tifactionState.GainFactionHate(faction, num2, false, this.missionTemplate.displayName + " targets faction", true);
									continue;
								}
							}
							tifactionState.GainFactionHate(faction, num2, false, this.missionTemplate.displayName + " targets faction", true);
						}
					}
				}
			}
			if (this.missionOutcome == TIMissionOutcome.Aborted && abortReasonDetail == string.Empty)
			{
				abortReasonDetail = Loc.T(new StringBuilder("TIMissionAbort_").Append(abortReason.ToString()).ToString());
			}
			TINotificationQueueState.LogMissionOutcome(this, missionResult, ref_faction, list3, list2, false, abortReasonDetail);
			if (this.councilor.agentForFaction != null)
			{
				TINotificationQueueState.LogMissionOutcome(this, missionResult, ref_faction, list3, list2, true, abortReasonDetail);
			}
			faction.CheckForObjectivesCompleteViaMission(this, missionResult);
			if (missionResult.Attempted)
			{
				if (this.missionTemplate.targetEffects != null)
				{
					foreach (TIMissionEffect timissionEffect3 in this.missionTemplate.targetEffects.Where<TIMissionEffect>((TIMissionEffect x) => x.HasDelayedEffect()))
					{
						timissionEffect3.ApplyDelayedEffect(this, this.target, missionResult.missionOutcome, "");
					}
				}
				if (this.missionTemplate.councilorEffects != null)
				{
					foreach (TIMissionEffect timissionEffect4 in this.missionTemplate.councilorEffects.Where<TIMissionEffect>((TIMissionEffect x) => x.HasDelayedEffect()))
					{
						timissionEffect4.ApplyDelayedEffect(this, this.councilor, missionResult.missionOutcome, "");
					}
				}
			}
			if (!this.councilor.deleted && missionResult.Attempted)
			{
				this.DetectionPhase(this.councilor, missionResult, ref_councilor2, ref_faction);
				if (missionResult.Failed)
				{
					this.councilor.faction.AddSuspicionForFailure(missionResult);
				}
				if (!this.target.deleted && this.target.isCouncilorState && this.target.ref_faction != null && (!this.councilor.isAlien || this.target.ref_faction.CanDetectAlienMission(this.missionTemplate)) && this.MissionNoise(missionResult.missionOutcome) > 0f && this.missionTemplate.hate[(int)missionResult.missionOutcome] > 0f)
				{
					this.target.ref_councilor.AddToParanoia(this.councilor.faction);
					if (!this.councilor.ref_faction.permanentAlly(this.target.ref_faction))
					{
						this.target.ref_councilor.imBeingTargeted = true;
					}
				}
				if (missionResult.Failed && this.missionTemplate.hate[(int)missionResult.missionOutcome] > 0f && !this.councilor.isAlien)
				{
					TINotificationQueueState.LogEnemyMissionFailure(this, missionResult);
				}
			}
			this.missionOutcome = missionResult.missionOutcome;
			this.MissionResolved();
			return missionResult;
		}

		// Token: 0x06003526 RID: 13606 RVA: 0x001305D4 File Offset: 0x0012E7D4
		public float MissionNoise(TIMissionOutcome outcome)
		{
			return this.missionTemplate.noise[(int)outcome];
		}

		// Token: 0x06003527 RID: 13607 RVA: 0x001305E4 File Offset: 0x0012E7E4
		private void DetectionPhase(TICouncilorState thisMissionCouncilor, MissionResult result, TICouncilorState missionTargetCouncilor, TIFactionState missionTargetFaction)
		{
			float num = thisMissionCouncilor.HideScore - this.MissionNoise(result.missionOutcome);
			Func<TICouncilorState, bool> <>9__0;
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions().Except<TIFactionState>(new TIFactionState[] { thisMissionCouncilor.faction }))
			{
				float num2 = 0f;
				float num3 = num;
				for (int i = 0; i < tifactionState.councilors.Count; i++)
				{
					TICouncilorState ticouncilorState = tifactionState.councilors[i];
					if (ticouncilorState.active)
					{
						if (ticouncilorState.location == thisMissionCouncilor.location)
						{
							num2 += ticouncilorState.DetectCouncilorScore;
						}
						else if (ticouncilorState.OnEarth && thisMissionCouncilor.OnEarth)
						{
							if (ticouncilorState.currentNation == thisMissionCouncilor.currentNation)
							{
								num2 += ticouncilorState.DetectCouncilorScore / 2f;
							}
							else if (ticouncilorState.ref_region.AdjacentRegions(false).Contains(thisMissionCouncilor.ref_region))
							{
								num2 += ticouncilorState.DetectCouncilorScore / 4f;
							}
						}
					}
				}
				if (num2 > 0f)
				{
					num2 /= 6f;
				}
				if (thisMissionCouncilor.OnEarth)
				{
					num2 += 2f * thisMissionCouncilor.currentNation.CouncilControlPointFraction(tifactionState, false, false);
					if (thisMissionCouncilor.isAlien)
					{
						num2 += (float)tifactionState.AlienDetectionBonus;
						num2 += TIEffectsState.SumEffectsModifiers(Context.DetectAliensOnEarth, tifactionState, num2, null);
						num2 -= thisMissionCouncilor.ref_region.xenoforming.xenoformingLevel / 5f;
						num2 -= (float)(thisMissionCouncilor.ref_region.alienFacility.Extant() ? 6 : 0);
					}
					else
					{
						num2 += (float)tifactionState.HumanDetectionBonus;
						num2 += TIEffectsState.SumEffectsModifiers(Context.DetectHumanCouncilorsOnEarth, tifactionState, num2, null);
					}
				}
				else
				{
					if (thisMissionCouncilor.location.isSpaceAssetState)
					{
						if (!thisMissionCouncilor.location.ref_spaceAsset.ref_factions.Contains(tifactionState))
						{
							IEnumerable<TICouncilorState> councilors = tifactionState.councilors;
							Func<TICouncilorState, bool> func;
							if ((func = <>9__0) == null)
							{
								func = (<>9__0 = (TICouncilorState x) => x.location.ref_spaceAsset == thisMissionCouncilor.location.ref_spaceAsset);
							}
							if (!councilors.Any<TICouncilorState>(func))
							{
								goto IL_0282;
							}
						}
						num2 += (float)(6 - thisMissionCouncilor.location.ref_hab.tier);
						goto IL_0289;
					}
					IL_0282:
					num2 = -9999f;
				}
				IL_0289:
				if (tifactionState.SufficientIntel(TIUtilities.ObjectToScannableLocation(thisMissionCouncilor.location), 1f))
				{
					if (thisMissionCouncilor.isHuman)
					{
						num2 += 12f;
					}
					else
					{
						num2 += (float)Mathf.Clamp(tifactionState.alienInvestigations, 1, 12);
					}
				}
				float num4 = num2 - num3;
				float num5 = -1f;
				if (num2 > -100f)
				{
					num5 = 0.5f * Mathf.Pow(0.775f, Mathf.Abs(num4));
					if (num4 >= 0f)
					{
						num5 = 1f - num5;
					}
				}
				float num6 = TIUtilities.RandomFloatValue();
				if (num6 < num5)
				{
					if (thisMissionCouncilor.isAlien && thisMissionCouncilor.location.isRegionState)
					{
						if (tifactionState.CanDetectAlienMission(this.missionTemplate))
						{
							thisMissionCouncilor.location.ref_region.alienActivity.ActivitySightedByFaction(tifactionState, this.missionTemplate, missionTargetCouncilor, missionTargetFaction, this);
							if (tifactionState.CanDetectAlien)
							{
								this.DetectCouncilor(tifactionState, thisMissionCouncilor, num6, num5, result);
							}
						}
					}
					else
					{
						this.DetectCouncilor(tifactionState, thisMissionCouncilor, num6, num5, result);
					}
				}
			}
		}

		// Token: 0x06003528 RID: 13608 RVA: 0x001309CC File Offset: 0x0012EBCC
		public void DetectCouncilor(TIFactionState detectingFaction, TICouncilorState detectedCouncilor, float roll, float chance, MissionResult result)
		{
			float num = ((roll <= chance / 10f) ? TemplateManager.global.intelToSeeCouncilorBasicData : TemplateManager.global.intelToSeeNeutralPawn);
			float intel = detectingFaction.GetIntel(detectedCouncilor);
			float num2 = detectingFaction.GainIntelToMinimum(detectedCouncilor, num, num, detectingFaction, TIGlobalConfig.globalConfig.intelToSeeCouncilorMission);
			if (num2 > intel && num2 >= TemplateManager.global.intelToSeeNeutralPawn && num2 < TemplateManager.global.intelToSeeCouncilorBasicData)
			{
				TINotificationQueueState.LogEnemyCouncilorLocationDetected(detectingFaction, detectedCouncilor);
			}
			else if (num2 >= TemplateManager.global.intelToSeeCouncilorBasicData)
			{
				if (num2 > intel)
				{
					if (this.councilor.isAlien)
					{
						TINotificationQueueState.LogAlienCouncilorDetected(detectingFaction, detectedCouncilor);
					}
					else
					{
						TINotificationQueueState.LogEnemyCouncilorLocationDetected(detectingFaction, detectedCouncilor);
					}
				}
				if (result.Success && this.missionTemplate.hate[(int)result.missionOutcome] == 0f && this.missionTemplate.hate[4] > 0f && this.target.ref_factions.Contains(detectingFaction))
				{
					detectingFaction.GainFactionHate(detectedCouncilor.ref_faction, this.missionTemplate.hate[4] / 2f, false, "Detected enemy councilor targeting our interests", true);
				}
			}
			float num3 = (float)detectedCouncilor.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false) - detectingFaction.GetAggregateStat(CouncilorAttribute.Espionage, false, null);
			float num4 = 0.5f * Mathf.Pow(0.775f, Mathf.Abs(num3));
			if (num3 >= 0f)
			{
				num4 = 1f - num4;
			}
			if (TIUtilities.RandomFloatValue() < num4)
			{
				TINotificationQueueState.LogMyCouncilorDetected(detectingFaction, detectedCouncilor);
				detectedCouncilor.AddToParanoia(detectingFaction);
			}
		}

		// Token: 0x040023D2 RID: 9170
		public float resources;

		// Token: 0x040023D3 RID: 9171
		public bool resolveTimeAssigned;

		// Token: 0x040023D4 RID: 9172
		public TIDateTime startTime;

		// Token: 0x040023D5 RID: 9173
		public TIDateTime resolveTime;

		// Token: 0x040023D6 RID: 9174
		public TIGameState target;

		// Token: 0x040023D7 RID: 9175
		public TICouncilorState councilor;

		// Token: 0x040023D8 RID: 9176
		public TIMissionOutcome missionOutcome;

		// Token: 0x040023D9 RID: 9177
		[fsIgnore]
		protected TIMissionTemplate _missionTemplate;

		// Token: 0x040023DA RID: 9178
		private const float detectionScaling = 0.775f;

		// Token: 0x040023DB RID: 9179
		private const float failureChanceAtBalance = 0.5f;

		// Token: 0x02000E0D RID: 3597
		public enum AbortReason
		{
			// Token: 0x04005625 RID: 22053
			None,
			// Token: 0x04005626 RID: 22054
			UseDetail,
			// Token: 0x04005627 RID: 22055
			ControlPointRemoved,
			// Token: 0x04005628 RID: 22056
			CouncilorRetired,
			// Token: 0x04005629 RID: 22057
			CouncilorUnavailable,
			// Token: 0x0400562A RID: 22058
			CouncilorDetained,
			// Token: 0x0400562B RID: 22059
			CouncilorDead,
			// Token: 0x0400562C RID: 22060
			TurnedCouncilorQuit,
			// Token: 0x0400562D RID: 22061
			MissionOrgLost,
			// Token: 0x0400562E RID: 22062
			ControlPointAlreadyPurged,
			// Token: 0x0400562F RID: 22063
			BlanketCancel_ProbableError,
			// Token: 0x04005630 RID: 22064
			NationAlreadyCouped,
			// Token: 0x04005631 RID: 22065
			TargetFleetDestroyed,
			// Token: 0x04005632 RID: 22066
			TargetShipDestroyed,
			// Token: 0x04005633 RID: 22067
			TargetHabDestroyed,
			// Token: 0x04005634 RID: 22068
			TargetInvalid,
			// Token: 0x04005635 RID: 22069
			VoluntaryAbort,
			// Token: 0x04005636 RID: 22070
			OrgAlreadyTaken
		}
	}
}
