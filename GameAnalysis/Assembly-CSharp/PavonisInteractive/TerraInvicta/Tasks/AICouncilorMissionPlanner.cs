using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000930 RID: 2352
	public class AICouncilorMissionPlanner : MonoBehaviour
	{
		// Token: 0x17000F47 RID: 3911
		// (get) Token: 0x060059CD RID: 22989 RVA: 0x00292F68 File Offset: 0x00291168
		// (set) Token: 0x060059CE RID: 22990 RVA: 0x00292F6F File Offset: 0x0029116F
		public static AICouncilorMissionPlanner singleton { get; private set; }

		// Token: 0x060059CF RID: 22991 RVA: 0x00292F77 File Offset: 0x00291177
		private void Awake()
		{
			AICouncilorMissionPlanner.singleton = this;
		}

		// Token: 0x060059D0 RID: 22992 RVA: 0x00292F80 File Offset: 0x00291180
		public void Initialize()
		{
			AICouncilorMissionPlanner.singleton = this;
			this.cameraManager = World.Active.GetExistingManager<CameraManager>();
			this.AISmoothing = TemplateManager.global.smoothAIMissionPlanning && !Application.isEditor;
			this.SmoothingMSPerFrame = TemplateManager.global.smoothingMSPerFrame;
			this.rawNationPayoffs = new Dictionary<TIFactionState, Dictionary<TINationState, float>>();
			this.rawControlPointPayoffs = new Dictionary<TIFactionState, Dictionary<TIControlPoint, float>>();
		}

		// Token: 0x060059D1 RID: 22993 RVA: 0x00292FE8 File Offset: 0x002911E8
		public float GetPayoffForMissionTarget(TIFactionState faction, TIMissionTemplate mission, TICouncilorState councilor, TIGameState target, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, TIFactionGoalState focusGoal, List<CampaignMilestone> factionDesiredMilestones, float campaignDuration_years, bool huntingForAlienActivity, float huntAbility, List<TIFactionState> warFactions, TIRegionState recentAlienSite, float timeSinceAlienSite_days, bool capturingNeutralNations)
		{
			AICachedMissionEntry aicachedMissionEntry = new AICachedMissionEntry
			{
				mission = mission,
				target = target
			};
			if (this.factionMissionDictionary.ContainsKey(aicachedMissionEntry))
			{
				return this.factionMissionDictionary[aicachedMissionEntry];
			}
			float goalMultipliers = this.GetGoalMultipliers(faction, mission, target, focusGoal);
			float num = AICouncilorMissionPlanner.GetPayoffForMissionTarget_Faction(faction, mission, target, factionDesiredMilestones, this.rawControlPointPayoffs[faction], this.controlPointPayoffs, this.rawNationPayoffs[faction], this.nationPayoffs, campaignDuration_years) * goalMultipliers;
			if (num == -999f)
			{
				return AICouncilorMissionPlanner.GetPayoffForMissionTarget_Individual(faction, mission, councilor, target, requiredMissions, missingRequiredMissions, this.nationPayoffs, huntingForAlienActivity, huntAbility, warFactions, recentAlienSite, timeSinceAlienSite_days, this.recentAlienControlPointGift, this.timeSinceAlienControlPointGift_days, capturingNeutralNations) * goalMultipliers;
			}
			this.factionMissionDictionary.Add(aicachedMissionEntry, num);
			return num;
		}

		// Token: 0x060059D2 RID: 22994 RVA: 0x002930B4 File Offset: 0x002912B4
		public float GetGoalMultipliers(TIFactionState faction, TIMissionTemplate mission, TIGameState target, TIFactionGoalState focusGoal)
		{
			if (faction.isActivePlayer)
			{
				return 1f;
			}
			if (target.isNationState || target.isRegionSpaceFacility || target.isControlPointState || target.isRegionState)
			{
				TINationState ref_nation = target.ref_nation;
				List<TIFactionGoalState> list;
				if (this.nationModifyingGoals.TryGetValue(ref_nation, out list))
				{
					list = list.ToList<TIFactionGoalState>();
				}
				else
				{
					list = faction.FindGoals(TIFactionGoalState.NationMissionModifyingGoals, faction, ref_nation, TIFactionState.GoalFilter.none, true);
				}
				if (target.ref_faction != null)
				{
					list.AddRangeUnique<TIFactionGoalState>(this.nationModifyingGoalsByFaction[target.ref_faction]);
				}
				return AICouncilorMissionPlanner.<GetGoalMultipliers>g__GetModifierFromGoals|38_0(list, mission, focusGoal);
			}
			if (target.isCouncilorState || target.isHabState || target.isHabModuleState || target.isOrgState)
			{
				TIFactionState ref_faction = target.ref_faction;
				return AICouncilorMissionPlanner.<GetGoalMultipliers>g__GetModifierFromGoals|38_0(this.factionMissionModifyingGoals[ref_faction], mission, focusGoal);
			}
			return 1f;
		}

		// Token: 0x060059D3 RID: 22995 RVA: 0x00293190 File Offset: 0x00291390
		public static float GetPayoffForMissionTarget_Individual(TIFactionState faction, TIMissionTemplate mission, TICouncilorState councilor, TIGameState target, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, Dictionary<TINationState, float> nationPayoffs, bool huntingForAlienActivity, float huntAbility, List<TIFactionState> warFactions, TIRegionState recentAlienSite, float timeSinceAlienSite_days, TINationState recentAlienControlPointGift, float timeSinceAlienControlPointGift_days, bool capturingNeutralNations)
		{
			string dataName = mission.dataName;
			if (dataName != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(dataName);
				if (num > 2357971745U)
				{
					if (num <= 2592082955U)
					{
						if (num != 2448992345U)
						{
							if (num != 2592082955U)
							{
								goto IL_0130;
							}
							if (!(dataName == "EnthrallOrg"))
							{
								goto IL_0130;
							}
						}
						else
						{
							if (!(dataName == "GoToGround"))
							{
								goto IL_0130;
							}
							return AICouncilorMissionPlanner.GoToGroundPayoff(councilor, target.ref_region);
						}
					}
					else if (num != 3520867116U)
					{
						if (num != 3808240500U)
						{
							goto IL_0130;
						}
						if (!(dataName == "HostileTakeover"))
						{
							goto IL_0130;
						}
					}
					else
					{
						if (!(dataName == "Protect"))
						{
							goto IL_0130;
						}
						return AICouncilorMissionPlanner.ProtectMissionPayoff(councilor, target);
					}
					return AICouncilorMissionPlanner.HostileTakeoverPayoff(councilor, target as TIOrgState, requiredMissions, missingRequiredMissions, capturingNeutralNations);
				}
				if (num != 478979418U)
				{
					if (num != 2009857032U)
					{
						if (num == 2357971745U)
						{
							if (dataName == "Advise")
							{
								return AICouncilorMissionPlanner.AdvisePayoff(councilor, target);
							}
						}
					}
					else if (dataName == "DetectCouncilActivity")
					{
						return AICouncilorMissionPlanner.DetectCouncilActivityPayoff(faction, councilor, target, huntingForAlienActivity, huntAbility, warFactions, recentAlienSite, timeSinceAlienSite_days, recentAlienControlPointGift, timeSinceAlienControlPointGift_days, nationPayoffs);
					}
				}
				else if (dataName == "Deorbit")
				{
					return AICouncilorMissionPlanner.DeorbitPayoff(councilor);
				}
			}
			IL_0130:
			return 0f;
		}

		// Token: 0x060059D4 RID: 22996 RVA: 0x002932D4 File Offset: 0x002914D4
		public static float GetPayoffForMissionTarget_Faction(TIFactionState faction, TIMissionTemplate mission, TIGameState target, List<CampaignMilestone> factionDesiredMilestones, Dictionary<TIControlPoint, float> rawControlPointPayoffs, Dictionary<TIControlPoint, float> controlPointPayoffs, Dictionary<TINationState, float> rawNationPayoffs, Dictionary<TINationState, float> nationPayoffs, float campaignDuration_years)
		{
			string dataName = mission.dataName;
			if (dataName != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(dataName);
				if (num <= 1494308904U)
				{
					if (num <= 516509485U)
					{
						if (num <= 334661530U)
						{
							if (num <= 182146424U)
							{
								if (num != 100120188U)
								{
									if (num == 182146424U)
									{
										if (dataName == "Unrest")
										{
											return AICouncilorMissionPlanner.IncreaseUnrestPayoff(faction, target.ref_region, target.ref_nation, rawNationPayoffs[target.ref_nation]);
										}
									}
								}
								else if (dataName == "SabotageFacilities")
								{
									return AICouncilorMissionPlanner.SabotageSpaceFacilitiesPayoff(faction, target.ref_regionSpaceFacility);
								}
							}
							else if (num != 186877998U)
							{
								if (num == 334661530U)
								{
									if (dataName == "Turn")
									{
										return AICouncilorMissionPlanner.TurnCouncilorPayoff(faction, target.ref_councilor);
									}
								}
							}
							else if (dataName == "SabotageProject")
							{
								return AICouncilorMissionPlanner.SabotageProjectPayoff(faction, target.ref_faction);
							}
						}
						else if (num <= 431564165U)
						{
							if (num != 429902756U)
							{
								if (num == 431564165U)
								{
									if (dataName == "Abductions")
									{
										return AICouncilorMissionPlanner.AbductionsPayoff(target.ref_region);
									}
								}
							}
							else if (dataName == "Purge")
							{
								return AICouncilorMissionPlanner.PurgePayoff(faction, target.ref_controlPoint, controlPointPayoffs[target.ref_controlPoint]);
							}
						}
						else if (num != 457615351U)
						{
							if (num == 516509485U)
							{
								if (dataName == "AssaultAlienAsset")
								{
									return AICouncilorMissionPlanner.AssaultAlienAssetPayoff(faction, target.ref_regionAlienAsset, factionDesiredMilestones);
								}
							}
						}
						else if (dataName == "Xenoform")
						{
							return AICouncilorMissionPlanner.XenoformingPayoff(faction, target.ref_region);
						}
					}
					else if (num <= 910456538U)
					{
						if (num <= 752956076U)
						{
							if (num != 674146076U)
							{
								if (num == 752956076U)
								{
									if (dataName == "DefendInterests")
									{
										return AICouncilorMissionPlanner.DefendInterestsPayoff(faction, target, rawControlPointPayoffs);
									}
								}
							}
							else if (dataName == "Propaganda")
							{
								return AICouncilorMissionPlanner.PublicOpinionShiftPayoff(faction, target.ref_nation, nationPayoffs[target.ref_nation]);
							}
						}
						else if (num != 810651166U)
						{
							if (num == 910456538U)
							{
								if (dataName == "Detain")
								{
									return AICouncilorMissionPlanner.DetainCouncilorPayoff(faction, target.ref_councilor, factionDesiredMilestones);
								}
							}
						}
						else if (dataName == "Extract")
						{
							return AICouncilorMissionPlanner.ExtractPayoff(faction, target.ref_councilor);
						}
					}
					else if (num <= 1386603441U)
					{
						if (num != 1003957029U)
						{
							if (num == 1386603441U)
							{
								if (dataName == "Crackdown")
								{
									return AICouncilorMissionPlanner.CrackdownPayoff(faction, target.ref_controlPoint, controlPointPayoffs[target.ref_controlPoint]);
								}
							}
						}
						else if (dataName == "Inspire")
						{
							return AICouncilorMissionPlanner.InspirePayoff(faction, target.ref_councilor);
						}
					}
					else if (num != 1412701134U)
					{
						if (num == 1494308904U)
						{
							if (dataName == "ControlSpaceAsset")
							{
								return AICouncilorMissionPlanner.SeizeAssetPayoff(faction, target) * 1.5f;
							}
						}
					}
					else if (dataName == "BuildFacility")
					{
						return AICouncilorMissionPlanner.BuildFacilityPayoff(faction, target.ref_region);
					}
				}
				else if (num <= 2276976907U)
				{
					if (num <= 1940690115U)
					{
						if (num <= 1654255196U)
						{
							if (num != 1619260818U)
							{
								if (num == 1654255196U)
								{
									if (dataName == "Assassinate")
									{
										return AICouncilorMissionPlanner.AssassinatePayoff(faction, target.ref_councilor, factionDesiredMilestones);
									}
								}
							}
							else if (dataName == "Coup")
							{
								return AICouncilorMissionPlanner.CoupPayoff(faction, target.ref_nation, rawNationPayoffs[target.ref_nation]);
							}
						}
						else if (num != 1839498144U)
						{
							if (num == 1940690115U)
							{
								if (dataName == "Contact")
								{
									return AICouncilorMissionPlanner.ContactCouncilorPayoff(faction, target.ref_faction);
								}
							}
						}
						else if (dataName == "AssumeControl")
						{
							return AICouncilorMissionPlanner.TransferControlPayoff(faction, target.ref_nation, nationPayoffs[target.ref_nation]);
						}
					}
					else if (num <= 2023719859U)
					{
						if (num != 1973091966U)
						{
							if (num == 2023719859U)
							{
								if (dataName == "DominateNation")
								{
									return AICouncilorMissionPlanner.DominatePayoff(faction, target.ref_nation, controlPointPayoffs);
								}
							}
						}
						else if (dataName == "Stabilize")
						{
							return AICouncilorMissionPlanner.StabilizePayoff(faction, target.ref_nation, rawNationPayoffs[target.ref_nation]);
						}
					}
					else if (num != 2057193530U)
					{
						if (num == 2276976907U)
						{
							if (dataName == "TerrorizeRegion")
							{
								return AICouncilorMissionPlanner.TerrorizePayoff(faction, target.ref_region, rawControlPointPayoffs);
							}
						}
					}
					else if (dataName == "PassTechnology")
					{
						return AICouncilorMissionPlanner.PassTechnologyPayoff(faction, target.ref_faction);
					}
				}
				else if (num <= 3376264837U)
				{
					if (num <= 2349948754U)
					{
						if (num != 2294428806U)
						{
							if (num == 2349948754U)
							{
								if (dataName == "EnthrallPublic")
								{
									return AICouncilorMissionPlanner.EnthrallPublicPayoff(faction, target.ref_region, nationPayoffs[target.ref_nation]);
								}
							}
						}
						else if (dataName == "InvestigateCouncilor")
						{
							return AICouncilorMissionPlanner.InvestigateCouncilorPayoff(faction, target.ref_councilor);
						}
					}
					else if (num != 3176398323U)
					{
						if (num == 3376264837U)
						{
							if (dataName == "StealProject")
							{
								return AICouncilorMissionPlanner.StealProjectPayoff(faction, target.ref_faction);
							}
						}
					}
					else if (dataName == "EnthrallElites")
					{
						return AICouncilorMissionPlanner.EnthrallFactionElitesPayoff(faction, target.ref_controlPoint, rawControlPointPayoffs[target.ref_controlPoint]);
					}
				}
				else if (num <= 3591997738U)
				{
					if (num != 3574394417U)
					{
						if (num == 3591997738U)
						{
							if (dataName == "InvestigateAlienActivity")
							{
								return AICouncilorMissionPlanner.InvestigateAlienActivityPayoff(faction);
							}
						}
					}
					else if (dataName == "GainInfluence")
					{
						return AICouncilorMissionPlanner.ControlNationPayoff(faction, target.ref_nation.FirstNativeControlPoint(), controlPointPayoffs, campaignDuration_years);
					}
				}
				else if (num != 3890190275U)
				{
					if (num == 3917615220U)
					{
						if (dataName == "EnthrallUnalignedElites")
						{
							return AICouncilorMissionPlanner.EnthrallUnalignedElitesPayoff(faction, target.ref_nation.FirstNativeControlPoint(), controlPointPayoffs, campaignDuration_years);
						}
					}
				}
				else if (dataName == "SeizeSpaceAsset")
				{
					return AICouncilorMissionPlanner.SeizeAssetPayoff(faction, target);
				}
			}
			return -999f;
		}

		// Token: 0x060059D5 RID: 22997 RVA: 0x002939F4 File Offset: 0x00291BF4
		public float NationPayoff_Current(TIFactionState faction, TINationState nation)
		{
			if (!this.rawNationPayoffs[faction].ContainsKey(nation))
			{
				this.rawNationPayoffs[faction].Add(nation, AIEvaluators.EvaluateNation(faction, nation));
			}
			return this.rawNationPayoffs[faction][nation] + (nation.unrest + nation.unrestRestState) / 2f * ((nation.unrest + nation.unrestRestState) / 2f) * -4f * faction.currentRiskAversion + nation.CouncilControlPointFraction(faction, true, true) * 5f;
		}

		// Token: 0x060059D6 RID: 22998 RVA: 0x00293A88 File Offset: 0x00291C88
		public static float AbductionsPayoff(TIRegionState region)
		{
			float num = 0f;
			int targetAbductionsForFacility = TemplateManager.global.minAbductionsinRegionForFacility;
			float maxAbductionMissionImpact = TemplateManager.global.maxAbductionMissionImpact;
			TIFactionState executiveFaction = region.nation.executiveFaction;
			if (executiveFaction != null && executiveFaction.IsAlienProxy && !region.nation.regions.Any<TIRegionState>((TIRegionState x) => x.abductions >= targetAbductionsForFacility))
			{
				num += (float)(100 * (1 + region.abductions) * region.nation.regions.Count) * region.nation.perCapitaGDP / 60000f;
			}
			if ((float)region.abductions < maxAbductionMissionImpact / 20f)
			{
				num += 2000f;
			}
			else if ((float)region.abductions < maxAbductionMissionImpact / 10f)
			{
				num += 1000f;
			}
			else if ((float)region.abductions < maxAbductionMissionImpact / 5f)
			{
				num += 500f;
			}
			else if ((float)region.abductions < maxAbductionMissionImpact / 2f)
			{
				num += 200f;
			}
			else if ((float)region.abductions < maxAbductionMissionImpact)
			{
				num += 100f;
			}
			return 0.01f + num;
		}

		// Token: 0x060059D7 RID: 22999 RVA: 0x00293BA8 File Offset: 0x00291DA8
		public static float AdvisePayoff(TICouncilorState councilor, TIGameState target)
		{
			float num = -1f;
			TINationState nation = target.ref_nation;
			if (nation != null)
			{
				int count = nation.FactionControlPoints(councilor.faction, false, false, true).Count;
				if (count > 0)
				{
					num = 0f;
					Func<TIArmyState, bool> <>9__1;
					if (nation.belligerentInActiveWar && (nation.armies.Count > 0 || nation.wars.Any<TINationState>(delegate(TINationState x)
					{
						IEnumerable<TIArmyState> armies = x.armies;
						Func<TIArmyState, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (TIArmyState x) => x.currentNation == nation);
						}
						return armies.Any<TIArmyState>(func);
					})))
					{
						int attribute = councilor.GetAttribute(CouncilorAttribute.Command, true, true, true, false, false, false);
						if (attribute > 10)
						{
							num += (float)(25 * attribute * count * Mathf.Max(1, nation.numStandardArmies));
						}
						else
						{
							num += (float)attribute * 1E-09f;
						}
					}
					if (!nation.alienNation)
					{
						int attribute2 = councilor.GetAttribute(CouncilorAttribute.Science, true, true, true, false, false, false);
						if (attribute2 > 10)
						{
							num += (float)attribute2 * councilor.faction.aiValues.gatherScience * (nation.research_month * 0.1f) * (float)count;
						}
						else
						{
							num += (float)attribute2 * 1E-09f;
						}
					}
					int attribute3 = councilor.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false);
					if (attribute3 > 10)
					{
						num += (float)attribute3 * nation.BaseInvestmentPoints_month(councilor.faction) * (float)count * (float)count;
					}
					else
					{
						num += (float)attribute3 * 1E-09f;
					}
				}
			}
			else
			{
				TIHabState ref_hab = target.ref_hab;
				if (ref_hab != null && ref_hab.faction == councilor.faction)
				{
					num = 0f;
					float num2 = 0f;
					int attribute4 = councilor.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false);
					if (attribute4 > 10)
					{
						foreach (FactionResource factionResource in TIResourcesCost.basicSpaceResources)
						{
							num2 += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, factionResource, ref_hab.GetNetCurrentMonthlyIncome(councilor.faction, factionResource, false, true));
						}
						num2 += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Money, ref_hab.GetNetCurrentMonthlyIncome(councilor.faction, FactionResource.Money, false, true));
						num += num2 * 10f;
					}
					else
					{
						num += (float)attribute4 * 1E-09f;
					}
					int attribute5 = councilor.GetAttribute(CouncilorAttribute.Science, true, true, true, false, false, false);
					if (attribute5 > 10)
					{
						num += (float)attribute5 * councilor.faction.aiValues.gatherScience * ref_hab.GetNetCurrentMonthlyIncome(councilor.faction, FactionResource.Research, false, true);
					}
					else
					{
						num += (float)attribute5 * 1E-09f;
					}
				}
			}
			return num;
		}

		// Token: 0x060059D8 RID: 23000 RVA: 0x00293E60 File Offset: 0x00292060
		public static float AssassinatePayoff(TIFactionState actingFaction, TICouncilorState targetCouncilor, List<CampaignMilestone> factionDesiredMilestones)
		{
			CouncilorView viewofCouncilor = actingFaction.GetViewofCouncilor(targetCouncilor);
			TIFactionState factionCurrent = viewofCouncilor.factionCurrent;
			if ((actingFaction.shouldNeverAttackAliens && viewofCouncilor.isKnownAlien) || actingFaction.permanentAlly(targetCouncilor.faction) || factionCurrent == null)
			{
				return -1f;
			}
			if (viewofCouncilor.isKnownAlien && factionDesiredMilestones.Contains(CampaignMilestone.AccessHydraCorpus))
			{
				return 1E+09f;
			}
			float num = viewofCouncilor.EvaluateCouncilor() * ((float)actingFaction.AI_WarWithFactionImportance(viewofCouncilor.factionCurrent) / 5f);
			TIGameState location = viewofCouncilor.location;
			bool? flag;
			if (location == null)
			{
				flag = null;
			}
			else
			{
				TINationState ref_nation = location.ref_nation;
				if (ref_nation == null)
				{
					flag = null;
				}
				else
				{
					TIFactionState executiveFaction = ref_nation.executiveFaction;
					flag = ((executiveFaction != null) ? new bool?(executiveFaction.permanentAlly(actingFaction)) : null);
				}
			}
			bool? flag2 = flag;
			if (!flag2.GetValueOrDefault())
			{
				TIGameState location2 = viewofCouncilor.location;
				TIGameState tigameState;
				if (location2 == null)
				{
					tigameState = null;
				}
				else
				{
					TIHabState ref_hab = location2.ref_hab;
					tigameState = ((ref_hab != null) ? ref_hab.faction : null);
				}
				if (!(tigameState == actingFaction))
				{
					TIGameState location3 = viewofCouncilor.location;
					bool? flag3;
					if (location3 == null)
					{
						flag3 = null;
					}
					else
					{
						TINationState ref_nation2 = location3.ref_nation;
						flag3 = ((ref_nation2 != null) ? new bool?(ref_nation2.FactionHasControlPoint(actingFaction)) : null);
					}
					flag2 = flag3;
					if (flag2.GetValueOrDefault())
					{
						num *= 1f;
						goto IL_0148;
					}
					num /= 10f;
					goto IL_0148;
				}
			}
			num *= 10f;
			IL_0148:
			if (viewofCouncilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.GlobalPropagandaIfKilled))
			{
				num /= Mathf.Max(2f, viewofCouncilor.traits.First<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.GlobalPropagandaIfKilled).specialTraitRuleValue / 10f);
			}
			if (viewofCouncilor.GrantsMarkedToAssassin())
			{
				num /= 10f;
			}
			else if (viewofCouncilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.tags.Contains("Dangerous")))
			{
				num /= 2f;
			}
			return num;
		}

		// Token: 0x060059D9 RID: 23001 RVA: 0x00294070 File Offset: 0x00292270
		public static float AssaultAlienAssetPayoff(TIFactionState faction, TIRegionAlienAssetState asset, List<CampaignMilestone> factionDesiredMilestones)
		{
			if (faction.shouldNeverAttackAliens)
			{
				return -1f;
			}
			float num = 0f;
			float num2 = 100f * (faction.aiValues.protectHumanLife - faction.aiValues.protectAlienLife);
			bool flag = asset.CampaignMilestonesGrantedOnCapture(faction, TIMissionOutcome.Success).Intersect<CampaignMilestone>(factionDesiredMilestones).Any<CampaignMilestone>();
			if (asset.isRegionXenoformingState)
			{
				num += asset.region.xenoforming.xenoformingLevel;
			}
			if (faction.ideologyCoordinates.x > 1f)
			{
				num *= faction.ideologyCoordinates.x;
			}
			if (faction.NationWithFactionInterest(asset.region.nation, false))
			{
				num *= 40f;
			}
			if (faction.AI_AtWarWithFaction(GameStateManager.AlienFaction()))
			{
				num *= 40f;
			}
			if (!faction.extremist)
			{
				TIFactionState executiveFaction = asset.region.nation.executiveFaction;
				if (executiveFaction != null && !executiveFaction.permanentAlly(GameStateManager.AlienFaction()) && faction.AI_AtWarWithFaction(asset.region.nation.executiveFaction))
				{
					num /= 20f;
				}
			}
			return num + (float)(flag ? 1000 : 50) * num2;
		}

		// Token: 0x060059DA RID: 23002 RVA: 0x00294190 File Offset: 0x00292390
		public static float TransferControlPayoff(TIFactionState faction, TINationState nation, float nationPayoff)
		{
			if (!faction.proAlien)
			{
				return -1f;
			}
			float num = nationPayoff * (-1f * faction.ideologyCoordinates.x / 2f) * 20f;
			if (faction.minorCPTrouble)
			{
				num *= 100f;
				if (faction.majorCPTrouble)
				{
					num *= 100f;
				}
			}
			if (nation.AdjacentNations(false).Contains(GameStateManager.AlienNation()) || !GameStateManager.AlienNation().extant)
			{
				num *= 100f;
			}
			else if (nation.armies.Count > 0)
			{
				num *= 10f;
			}
			float num2 = (float)faction.controlPoints.Count;
			return num * ((num2 - (float)nation.numControlPoints) / num2);
		}

		// Token: 0x060059DB RID: 23003 RVA: 0x00294248 File Offset: 0x00292448
		public static float BuildFacilityPayoff(TIFactionState faction, TIRegionState region)
		{
			float num = -1f;
			TIFactionState totalOwningFaction = region.nation.TotalOwningFaction;
			if (totalOwningFaction != null && totalOwningFaction.IsAlienProxy)
			{
				TINationState tinationState = GameStateManager.AlienNation();
				if (tinationState.extant && region.nation.AdjacentNations(false).Contains(GameStateManager.AlienNation()))
				{
					if (GameStateManager.IterateByClass<TIRegionAlienFacilityState>(false).Count<TIRegionAlienFacilityState>((TIRegionAlienFacilityState x) => x.Extant() && !x.region.nation.alienNation) <= 5)
					{
						num = 10000000f;
						goto IL_011A;
					}
				}
				if (!tinationState.extant && GameStateManager.AlienProxy().unlockedVictoryObjective)
				{
					num = 10000000f;
				}
				else if (GameStateManager.IterateByClass<TIRegionAlienFacilityState>(false).Count<TIRegionAlienFacilityState>((TIRegionAlienFacilityState x) => x.Extant()) == 0 && TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, GameStateManager.AlienFaction()))
				{
					num = 5000000f;
				}
				else if (GameStateManager.SupraRegionMembers(region.mapRegionTemplate.supraRegion).None<TIRegionState>((TIRegionState x) => x.hasAlienFacility))
				{
					num = 100000f;
				}
				IL_011A:
				if (num > 0f)
				{
					num *= Mathf.Max(0.5f, region.xenoforming.xenoformingLevel);
					if (faction.IsAlienFaction || (faction.IsAlienProxy && faction.CanContactAlien))
					{
						List<TIFactionGoalState> list = tinationState.ref_faction.FindGoals(TIFactionGoalState.CaptureNationGoals, tinationState.ref_faction, region.nation, TIFactionState.GoalFilter.none, true);
						if (list.Count > 0)
						{
							num *= (float)list.Max<TIFactionGoalState>((TIFactionGoalState x) => x.importance);
						}
					}
					if (faction.minorCPTrouble)
					{
						num *= 2f;
						if (faction.majorCPTrouble)
						{
							num *= 5f;
						}
					}
					num *= (float)Mathf.Max(region.abductions, 1);
					if (region.terrain == TerrainType.Rugged)
					{
						num *= 2f;
					}
				}
				return num;
			}
			return -1f;
		}

		// Token: 0x060059DC RID: 23004 RVA: 0x00294444 File Offset: 0x00292644
		public static float EnthrallUnalignedElitesPayoff(TIFactionState faction, TIControlPoint CP, Dictionary<TIControlPoint, float> controlPointPayoffs, float campaignDuration_years)
		{
			if (!(CP != null) || !controlPointPayoffs.ContainsKey(CP))
			{
				return -1f;
			}
			if (faction.IsAlienFaction)
			{
				faction = GameStateManager.AlienProxy();
			}
			bool significantPower = CP.nation.SignificantPower;
			float num = 3f * controlPointPayoffs[CP] * (significantPower ? Mathf.Max(campaignDuration_years * 4f, 20f) : 1f);
			if (faction.minorCPTrouble && !significantPower)
			{
				return -1f;
			}
			if (faction.majorCPTrouble && !CP.nation.MajorGlobalPower)
			{
				return -1f;
			}
			return num;
		}

		// Token: 0x060059DD RID: 23005 RVA: 0x002944DC File Offset: 0x002926DC
		public static float ControlNationPayoff(TIFactionState faction, TIControlPoint CP, Dictionary<TIControlPoint, float> controlPointPayoffs, float campaignDuration_years)
		{
			if (CP != null && controlPointPayoffs.ContainsKey(CP))
			{
				bool significantPower = CP.nation.SignificantPower;
				bool majorGlobalPower = CP.nation.MajorGlobalPower;
				float num = 3f * controlPointPayoffs[CP] * (significantPower ? Mathf.Max(campaignDuration_years * 4f, 20f) : 1f);
				if (faction.minorCPTrouble)
				{
					if (!significantPower)
					{
						return -1f;
					}
					if (faction.majorCPTrouble)
					{
						if (!majorGlobalPower)
						{
							return -1f;
						}
						num /= 2f;
					}
				}
				if (majorGlobalPower)
				{
					num *= (float)(1 + CP.nation.FactionControlPoints(faction, true, false, true).Count * 20);
				}
				else if (significantPower)
				{
					num *= (float)(1 + CP.nation.FactionControlPoints(faction, true, false, true).Count * 5);
				}
				else
				{
					num *= 1f + (float)CP.nation.FactionControlPoints(faction, true, false, true).Count / 2f;
				}
				return num;
			}
			return -1f;
		}

		// Token: 0x060059DE RID: 23006 RVA: 0x002945E0 File Offset: 0x002927E0
		public static float DominatePayoff(TIFactionState faction, TINationState nation, Dictionary<TIControlPoint, float> controlPointPayoffs)
		{
			if (nation.TotalOwningFaction == faction)
			{
				return -1f;
			}
			if (AICouncilorMissionPlanner.averageControlPointValue_Dominate == 0f)
			{
				if (faction.controlPoints.Count > 0)
				{
					AICouncilorMissionPlanner.averageControlPointValue_Dominate = faction.controlPoints.Average<TIControlPoint>((TIControlPoint x) => controlPointPayoffs[x]);
				}
				else
				{
					AICouncilorMissionPlanner.averageControlPointValue_Dominate = controlPointPayoffs.Values.Average();
				}
				AICouncilorMissionPlanner.cpCapFraction_Dominate = faction.GetBaselineControlPointMaintenanceCost(false) / faction.GetControlPointMaintenanceFreebieCap();
			}
			TIControlPoint ticontrolPoint = nation.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.faction != faction).MinBy<TIControlPoint, int>((TIControlPoint x) => x.positionInNation);
			float num = controlPointPayoffs[ticontrolPoint];
			if (AICouncilorMissionPlanner.cpCapFraction_Dominate > 0.8f && num < AICouncilorMissionPlanner.averageControlPointValue_Dominate * 0.75f)
			{
				return -1f;
			}
			TIFactionState strongestHumanFaction = AIEvaluators.GetStrongestHumanFaction(null);
			TIFactionState strongestEnemy = faction.GetMostThreateningEnemyHumanFaction();
			object obj;
			if (faction.controlPoints.Count > 0 && ticontrolPoint.faction == strongestEnemy && strongestHumanFaction != faction && controlPointPayoffs.Any<KeyValuePair<TIControlPoint, float>>((KeyValuePair<TIControlPoint, float> x) => x.Key.faction != strongestEnemy && x.Key.faction != faction))
			{
				TIFactionState faction2 = ticontrolPoint.faction;
				obj = faction2 == null || !faction2.IsAlienProxy;
			}
			else
			{
				obj = 0;
			}
			object obj2 = obj;
			if (obj2 != null)
			{
				num /= 2f;
			}
			if (obj2 == null || ticontrolPoint.nation.numControlPoints <= 3)
			{
				num *= Mathf.Pow(2f, (float)(ticontrolPoint.nation.numControlPoints - 3));
			}
			TIFactionState faction3 = ticontrolPoint.faction;
			if (faction3 != null && faction3.IsAlienProxy)
			{
				num *= 2f;
			}
			return Mathf.Pow(num, 2f);
		}

		// Token: 0x060059DF RID: 23007 RVA: 0x002947D4 File Offset: 0x002929D4
		public static float CoupPayoff(TIFactionState faction, TINationState nation, float rawNationPayoff)
		{
			if (faction.NationWithFactionInterest(nation, true))
			{
				return -1f;
			}
			if (!nation.SignificantPower && faction.IsAlienFaction && !nation.IsAdjacentToNation(GameStateManager.AlienNation(), false))
			{
				return -1f;
			}
			if (faction.IsAlienFaction)
			{
				if (!nation.controlPointOwnersByPoint.All<TIGameState>(delegate(TIGameState x)
				{
					TIFactionState ref_faction = x.ref_faction;
					return ref_faction != null && ref_faction.veryProAlien;
				}))
				{
					goto IL_009D;
				}
			}
			if (faction.GoalsWithTarget(nation, null, true).None<TIFactionGoalState>((TIFactionGoalState x) => x.GetMissionPayoffMultiplier(TIFactionState.coupMission, 0f) > 0f))
			{
				return -1f;
			}
			IL_009D:
			if (faction.majorCPTrouble)
			{
				return -1f;
			}
			float num = nation.CouncilControlPointFraction_DiscountNeutral(faction, true, true);
			if (num >= 1f)
			{
				return -1f;
			}
			float num2 = rawNationPayoff * TINationState.GetIdeologicalDistance(faction.ideology, nation.GetMeanEliteIdeology());
			if (num > 0f)
			{
				num2 *= faction.currentRiskAversion - num;
			}
			num2 *= faction.aiValues.dirtyTricks;
			num2 *= 0.35f;
			if (faction.minorCPTrouble)
			{
				if (nation.SignificantPower)
				{
					num2 *= 0.5f;
				}
				else
				{
					num2 *= 0.25f;
				}
			}
			return num2;
		}

		// Token: 0x060059E0 RID: 23008 RVA: 0x00294904 File Offset: 0x00292B04
		public static float CrackdownPayoff(TIFactionState faction, TIControlPoint controlPoint, float controlPointPayoff)
		{
			if (controlPoint.nation.numControlPoints <= 4)
			{
				List<TIFactionGoalState> list = faction.GoalsWithTarget(controlPoint.nation, null, true);
				list.AddRange(faction.GoalsWithTarget(controlPoint.faction, null, true));
				if (list.None<TIFactionGoalState>((TIFactionGoalState x) => x.GetMissionPayoffMultiplier(TIFactionState.crackdownMission, 0f) > 0f))
				{
					return -1f;
				}
			}
			float num = controlPointPayoff * (float)controlPoint.nation.numControlPoints_unclamped;
			num *= Mathf.Max(1f, TINationState.GetIdeologicalDistance(faction.ideology, controlPoint.faction.ideology) / 4f);
			num *= (float)Mathf.Max(controlPoint.nation.CountFactionControlPoints(faction, true, false, true), 1);
			bool flag = controlPoint.nation.numStandardArmies > 0 && controlPoint.nation.wars.Any<TINationState>((TINationState x) => faction.executiveNations.Contains(x));
			if (flag)
			{
				num *= (float)(3 * controlPoint.nation.numStandardArmies);
			}
			TIFactionState tifactionState = (faction.IsAlienFaction ? GameStateManager.AlienProxy() : faction);
			if (!flag && tifactionState.GetYearlyIncome(FactionResource.Influence, false, false, false) < tifactionState.GetAnnualInfluenceCostOfNextControlPoint(controlPoint.nation))
			{
				num *= 0.25f;
			}
			if (num > 1f && controlPoint.benefitsDisabled)
			{
				return 1f;
			}
			return num;
		}

		// Token: 0x060059E1 RID: 23009 RVA: 0x00294A74 File Offset: 0x00292C74
		public static float DefendInterestsPayoff(TIFactionState faction, TIGameState target, Dictionary<TIControlPoint, float> rawControlPointPayoffs)
		{
			if (target.isNationState)
			{
				TINationState ref_nation = target.ref_nation;
				List<TIControlPoint> list = ref_nation.FactionControlPoints(faction, false, false, false);
				if (list.Count == 0)
				{
					return 1E-15f;
				}
				if (ref_nation.civilWar || ref_nation.ArmiesThreateningCapital(false, true) > 0)
				{
					return 1f;
				}
				float num = 0f;
				int count = ref_nation.FactionsWithControlPoint.Count;
				int numNativeControlPoints = ref_nation.NumNativeControlPoints;
				bool significantPower = ref_nation.SignificantPower;
				if (count == 1 && numNativeControlPoints == 0 && list.Count >= (significantPower ? 1 : 3))
				{
					return (float)list.Count * 1E+15f * rawControlPointPayoffs[list.Last<TIControlPoint>()];
				}
				foreach (TIControlPoint ticontrolPoint in list)
				{
					num += rawControlPointPayoffs[ticontrolPoint] * (float)ref_nation.numControlPoints_unclamped;
				}
				if (numNativeControlPoints == 0)
				{
					num *= 5f;
				}
				if (count > 1)
				{
					num *= 5f;
				}
				if (ref_nation.executiveFaction == faction)
				{
					num *= 20f;
				}
				if (significantPower)
				{
					num *= 20f;
					if (ref_nation.MajorGlobalPower)
					{
						num *= 1000000f;
					}
				}
				return num;
			}
			else if (target.isHabState)
			{
				if (target.ref_hab.CouncilorsPresentAndKnownToFaction(faction, true, null).Count > 0)
				{
					return 5000f;
				}
				return 500f;
			}
			else
			{
				if (target.isSpaceFleetState)
				{
					return 500f;
				}
				return -1f;
			}
		}

		// Token: 0x060059E2 RID: 23010 RVA: 0x00294BF4 File Offset: 0x00292DF4
		public static float DetainCouncilorPayoff(TIFactionState faction, TICouncilorState targetCouncilor, List<CampaignMilestone> factionDesiredMilestones)
		{
			CouncilorView viewofCouncilor = faction.GetViewofCouncilor(targetCouncilor);
			TIFactionState factionCurrent = viewofCouncilor.factionCurrent;
			if ((factionCurrent != null) ? factionCurrent.IsAlienFaction : faction.shouldNeverAttackAliens)
			{
				return -1f;
			}
			if (factionDesiredMilestones.Contains(CampaignMilestone.AccessLiveHydra))
			{
				TIFactionState factionCurrent2 = viewofCouncilor.factionCurrent;
				if (factionCurrent2 != null && factionCurrent2.IsAlienFaction)
				{
					return 1E+09f;
				}
			}
			float num = viewofCouncilor.EvaluateCouncilor();
			bool flag = faction.AI_AtWarWithFaction(viewofCouncilor.factionCurrent);
			TIGameState location = viewofCouncilor.location;
			bool? flag2;
			if (location == null)
			{
				flag2 = null;
			}
			else
			{
				TINationState ref_nation = location.ref_nation;
				flag2 = ((ref_nation != null) ? new bool?(ref_nation.FactionHasControlPoint(faction)) : null);
			}
			bool? flag3 = flag2;
			if (!flag3.GetValueOrDefault())
			{
				TIGameState location2 = viewofCouncilor.location;
				TIGameState tigameState;
				if (location2 == null)
				{
					tigameState = null;
				}
				else
				{
					TIHabState ref_hab = location2.ref_hab;
					tigameState = ((ref_hab != null) ? ref_hab.faction : null);
				}
				if (!(tigameState == faction))
				{
					if (!flag)
					{
						return 0f;
					}
					return num;
				}
			}
			num *= (flag ? 20f : 1f);
			return num;
		}

		// Token: 0x060059E3 RID: 23011 RVA: 0x00294CE8 File Offset: 0x00292EE8
		public static float DeorbitPayoff(TICouncilorState councilor)
		{
			if (councilor.InAHab && councilor.ref_hab.inEarthSystem && councilor.ref_hab.faction == councilor.faction && (councilor.ref_hab.coreDefended || !councilor.GetPossibleMissionList(true, false, true, null, false).Contains(TIFactionState.defendInterestsMission)) && councilor.ref_hab.CouncilorsPresentAndKnownToFaction(councilor.faction, true, null).Count == 0)
			{
				return 50f;
			}
			return 0.001f;
		}

		// Token: 0x060059E4 RID: 23012 RVA: 0x00294D6C File Offset: 0x00292F6C
		public static float ContactCouncilorPayoff(TIFactionState faction, TIFactionState targetFaction)
		{
			if (!faction.WillingToTrade(targetFaction))
			{
				return -1f;
			}
			float num = -1f;
			TradeOffer tradeOffer = targetFaction.InitializeTradingOptions(faction);
			TradeOffer tradeOffer2 = faction.InitializeTradingOptions(targetFaction);
			if (tradeOffer.orgs.Count > 1)
			{
				num += 20f + (float)tradeOffer.orgs.Count * 1.25f;
			}
			if (tradeOffer.projects.Count > 1)
			{
				num += 20f + (float)tradeOffer.projects.Count * 1.25f;
			}
			num += (float)tradeOffer2.orgs.Count;
			num += (float)tradeOffer2.projects.Count;
			bool flag = false;
			if (faction.CanTradeTruce(targetFaction))
			{
				int willingnessToTradeTruce = AIEvaluators.GetWillingnessToTradeTruce(faction, targetFaction, true);
				if (willingnessToTradeTruce > 0)
				{
					num += (float)(willingnessToTradeTruce * 1000);
					flag = true;
				}
			}
			else if (faction.CanTradeNAP(targetFaction) && targetFaction.CanTradeNAP(faction))
			{
				int willingnessToTradeNAP = AIEvaluators.GetWillingnessToTradeNAP(faction, targetFaction, true);
				if (willingnessToTradeNAP > 0)
				{
					num += (float)(willingnessToTradeNAP * 1000);
				}
			}
			else if (faction.CanTradeIntelSharing(targetFaction, false) && targetFaction.CanTradeIntelSharing(faction, false))
			{
				int willingnessToShareIntel = AIEvaluators.GetWillingnessToShareIntel(faction, targetFaction, true, false);
				if (willingnessToShareIntel > 0)
				{
					num += (float)(willingnessToShareIntel * 1000);
				}
			}
			if (!flag && faction.GetFactionHate(targetFaction) > 1f)
			{
				num /= faction.GetFactionHate(targetFaction);
			}
			return num;
		}

		// Token: 0x060059E5 RID: 23013 RVA: 0x00294EB4 File Offset: 0x002930B4
		public static float DetectCouncilActivityPayoff(TIFactionState faction, TICouncilorState councilor, TIGameState target, bool huntingForAlienActivity, float huntAbility, List<TIFactionState> warFactions, TIRegionState recentAlienSite, float timeSinceAlienSite_days, TINationState recentAlienControlPointGift, float timeSinceAlienControlPointGift_days, Dictionary<TINationState, float> nationPayoffs)
		{
			if (faction.SufficientIntel(TIUtilities.ObjectToScannableLocation(target), 1f))
			{
				return -1f;
			}
			if (!target.isRegionState)
			{
				return 1f;
			}
			if (huntingForAlienActivity && huntAbility >= 1f)
			{
				return AICouncilorMissionPlanner.ScoreRegionForAlienSearch(faction, target.ref_region, recentAlienSite, timeSinceAlienSite_days, recentAlienControlPointGift, timeSinceAlienControlPointGift_days);
			}
			if (target.ref_nation.capital == target && warFactions.Count > 0 && (target.ref_nation.executiveFaction == faction || warFactions.Contains(target.ref_nation.executiveFaction)) && faction.CurrentKnownCouncilors(true, warFactions, true, false).Count == 0)
			{
				return nationPayoffs[target.ref_nation] * 0.1f * ((float)councilor.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false) + (float)councilor.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false) / 10f);
			}
			if (councilor.permanentDefenseMode && target.ref_nation.capital == target)
			{
				return (float)target.ref_nation.CountFactionControlPoints(faction, true, false, true);
			}
			return nationPayoffs[target.ref_nation] * 1E-05f * ((float)councilor.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false) + (float)councilor.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false) / 10f);
		}

		// Token: 0x060059E6 RID: 23014 RVA: 0x00295000 File Offset: 0x00293200
		private static float ScoreRegionForAlienSearch(TIFactionState faction, TIRegionState region, TIRegionState recentAlienSite, float timeSinceAlienSite_days, TINationState recentAlienControlPointGift, float timeSinceAlienControlPointGift_days)
		{
			float num = 0f;
			TINationState nation = region.nation;
			float publicOpinionOfFaction = nation.GetPublicOpinionOfFaction(GameStateManager.AlienProxy());
			float num2;
			nation.historyPublicOpinion[31].TryGetValue(GameStateManager.AlienProxy().ideology.ideology, out num2);
			float num3 = publicOpinionOfFaction - num2;
			float num4 = (float)nation.controlPoints.Count<TIControlPoint>(delegate(TIControlPoint x)
			{
				TIFactionState faction2 = x.faction;
				return faction2 != null && faction2.IsAlienProxy;
			}) / (float)nation.controlPoints.Count;
			if (faction.CanDetectEnthralls && region.nation.capital == region)
			{
				TIFactionState newExecutive = region.nation.lastExecutiveChange.newExecutive;
				if (newExecutive != null && newExecutive.IsAlienProxy)
				{
					TIDateTime date = region.nation.lastExecutiveChange.date;
					if (date != null && date.DifferenceInDays(TITimeState.Now()) < (double)30)
					{
						num += 20f;
					}
				}
			}
			if (publicOpinionOfFaction >= 0.7f && num3 > 0f)
			{
				float num5 = publicOpinionOfFaction * 200f;
				if (num4 > 0f)
				{
					num5 *= 0.5f;
				}
				num5 *= 0.1f + 0.9f * (1f - num4);
				num += num5;
			}
			if (num3 > 0.025f)
			{
				float num6 = num3 * 1500f;
				if (num4 > 0f)
				{
					num6 *= 0.75f;
				}
				num6 *= 0.5f + 0.5f * (1f - num4);
				num += num6;
			}
			if (recentAlienSite != null)
			{
				float num7 = 0f;
				float num8 = 0f;
				float num9 = 0f;
				float num10 = 0f;
				if (recentAlienSite == region)
				{
					num7 = Mathf.Max(700f * (40f - timeSinceAlienSite_days) / 40f, 30f);
				}
				if (recentAlienSite == region || region.IsAdjacent(recentAlienSite, false))
				{
					num8 = Mathf.Max(350f * (70f - timeSinceAlienSite_days) / 70f, 10f);
				}
				if (nation.regions.Contains(recentAlienSite))
				{
					num9 = Mathf.Max(100f * (180f - timeSinceAlienSite_days) / 180f, 10f);
				}
				if (recentAlienSite.AdjacentRegions(false).SelectMany<TIRegionState, TIRegionState>((TIRegionState x) => x.AdjacentRegions(false)).Distinct<TIRegionState>()
					.Contains(region))
				{
					num9 = 120f;
				}
				num += Mathf.Max(new float[] { num7, num8, num9, num10 });
			}
			if (recentAlienControlPointGift != null)
			{
				float num11 = 0f;
				float num12 = 0f;
				if (recentAlienControlPointGift.regions.Contains(region))
				{
					num11 = Mathf.Max(700f * (40f - timeSinceAlienControlPointGift_days) / 40f, 100f);
				}
				else if (nation.IsAdjacentToNation(recentAlienControlPointGift, false))
				{
					num12 = Mathf.Max(200f * (250f - timeSinceAlienControlPointGift_days) / 250f, 20f);
				}
				num += Mathf.Max(num11, num12);
				if (region == recentAlienControlPointGift.capital)
				{
					num += Mathf.Max(100f * (40f - timeSinceAlienControlPointGift_days) / 40f, 10f);
				}
			}
			return num;
		}

		// Token: 0x060059E7 RID: 23015 RVA: 0x00295348 File Offset: 0x00293548
		private TIRegionState BestRegionForAlienSearch(TICouncilorState councilor, List<TIRegionState> candidateRegions)
		{
			return (from x in candidateRegions.ToDictionary<TIRegionState, TIRegionState, float>((TIRegionState x) => x, (TIRegionState y) => AICouncilorMissionPlanner.ScoreRegionForAlienSearch(councilor.faction, y, this.recentAlienSite, this.timeSinceAlienSite_days, this.recentAlienControlPointGift, this.timeSinceAlienControlPointGift_days))
				orderby x.Value descending
				select x).ToList<KeyValuePair<TIRegionState, float>>().SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> x) => Mathf.Pow(x.Value, 3f), -1f, 1E-37f).Key;
		}

		// Token: 0x060059E8 RID: 23016 RVA: 0x002953FC File Offset: 0x002935FC
		public static float GoToGroundPayoff(TICouncilorState councilor, TIRegionState region)
		{
			TINationState nation = region.nation;
			float num = -1f;
			foreach (TIFactionState tifactionState in councilor.knowsIveBeenSeenBy)
			{
				if (!councilor.faction.HasNAP(tifactionState, true) && !tifactionState.permanentAlly(councilor.faction))
				{
					float num2 = 0f;
					num2 += 10f * TINationState.GetIdeologicalDistance(councilor.faction.ideology, tifactionState.ideology);
					if (councilor.faction.AI_AtWarWithFaction(tifactionState))
					{
						num2 *= 10f;
					}
					if (councilor.faction.IsAlienFaction && councilor.priorMissionTemplateName == TIFactionState.enthrallElitesMission.dataName && tifactionState.factionAssassinations.ContainsKey(councilor.faction) && tifactionState.factionAssassinations[councilor.faction] > 0)
					{
						num2 *= 5000f;
					}
					num += num2;
				}
			}
			if (num > 0f)
			{
				num *= nation.CouncilControlPointFraction(councilor.faction, true, true);
				if (region == nation.capital)
				{
					num /= 4f;
				}
				num *= councilor.faction.aiValues.protectCouncilors;
			}
			if (region == councilor.location && num <= 0f)
			{
				num = 1E-05f;
			}
			return num;
		}

		// Token: 0x060059E9 RID: 23017 RVA: 0x00295578 File Offset: 0x00293778
		public static float HostileTakeoverPayoff(TICouncilorState councilor, TIOrgState targetOrg, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, bool chasingNeutralNations)
		{
			bool currentlyDetectingHydra = councilor.faction.currentlyDetectingHydra;
			int count = councilor.faction.GoalsOfType(GoalType.WarOnFaction, false, true).Count;
			float num = 0f;
			using (List<TICouncilorState>.Enumerator enumerator = councilor.faction.councilors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TICouncilorState myCouncilor = enumerator.Current;
					num += AIEvaluators.EvaluateOrgForCouncilor(targetOrg, myCouncilor, myCouncilor.GetPossibleMissionList(false, false, true, null, false), requiredMissions, missingRequiredMissions, true, TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource x) => myCouncilor.GetMonthlyIncome(x)), currentlyDetectingHydra, count, chasingNeutralNations, false);
					num *= ((myCouncilor == councilor) ? 1f : 0.75f);
				}
			}
			return num;
		}

		// Token: 0x060059EA RID: 23018 RVA: 0x00295684 File Offset: 0x00293884
		public static float IncreaseUnrestPayoff(TIFactionState faction, TIRegionState region, TINationState nation, float rawNationPayoff)
		{
			float num = nation.CouncilControlPointFraction(faction, true, true);
			if (faction.NationWithFactionInterest(nation, true) || num > 0.2f || (nation.FactionsWithControlPoint.Count == 1 && nation.FactionHasControlPoint(faction)) || faction.permanentAlly(nation.executiveFaction))
			{
				return -1f;
			}
			if (nation.executiveFaction != null && faction.intelSharingFactions.Contains(nation.executiveFaction))
			{
				return -1f;
			}
			List<TIFactionGoalState> list = faction.GoalsWithTarget(nation, null, true);
			if (faction.IsAlienFaction)
			{
				if (!nation.controlPointOwnersByPoint.All<TIGameState>(delegate(TIGameState x)
				{
					TIFactionState ref_faction = x.ref_faction;
					return ref_faction != null && ref_faction.veryProAlien;
				}))
				{
					goto IL_0113;
				}
			}
			if (list.None<TIFactionGoalState>((TIFactionGoalState x) => x.GetMissionPayoffMultiplier(TIFactionState.unrestMission, 0f) > 0f))
			{
				return -1f;
			}
			IL_0113:
			if (nation.NumNativeControlPoints == nation.numControlPoints && !faction.cynical && nation.democracy > 6f)
			{
				return -1f;
			}
			float num2 = rawNationPayoff;
			if (num > 0f)
			{
				num2 *= 0.2f;
				num2 *= TINationState.GetIdeologicalDistance(faction.ideology, nation.GetMeanEliteIdeology());
				num2 *= faction.currentRiskAversion - num;
				num2 *= faction.aiValues.dirtyTricks;
			}
			num2 *= nation.unrest * 0.05f * faction.aiValues.dirtyTricks;
			if ((nation.unrest > nation.unrestRestState && nation.unrest < 5f) || nation.unrestRestState < 5f)
			{
				num2 *= 0.5f;
			}
			if (list.Any<TIFactionGoalState>((TIFactionGoalState x) => x.GetGoalType() == GoalType.NeutralizeNation))
			{
				if (faction.completedProjects.Any<TIProjectTemplate>((TIProjectTemplate x) => x.AI_projectRole == ProjectRole.NeutralizeNation && x.requiredNationState == nation))
				{
					num2 *= 1.4f + (float)nation.numControlPoints_unclamped;
				}
				if (region.SecessionCandidates().Count > 0)
				{
					num2 *= 10f;
				}
			}
			else if (list.Any<TIFactionGoalState>((TIFactionGoalState x) => TIFactionGoalState.CaptureNationGoals.Contains(x.GetGoalType())) && region.SecessionCandidates().Count > 0)
			{
				return -1f;
			}
			return num2;
		}

		// Token: 0x060059EB RID: 23019 RVA: 0x00295930 File Offset: 0x00293B30
		public static float InspirePayoff(TIFactionState faction, TICouncilorState targetCouncilor)
		{
			if (faction.IsActiveHumanFaction && faction == targetCouncilor.faction)
			{
				CouncilorView viewofCouncilor = faction.GetViewofCouncilor(targetCouncilor);
				float num = viewofCouncilor.EvaluateCouncilor();
				bool flag = false;
				int num2 = 1;
				if (faction.CanDetectEnthralls && !faction.IsAlienProxy)
				{
					num2++;
				}
				num2 += faction.GoalsOfType(GoalType.WarOnFaction, false, true).Count;
				if (faction.ShouldTryToRestoreCouncilorLoyalty(targetCouncilor))
				{
					if (viewofCouncilor.turned)
					{
						num2 += 50;
						flag = true;
					}
					else if (faction.AI_SuspectTurned(targetCouncilor))
					{
						num2 += 25;
						flag = true;
					}
				}
				if ((faction.HasIntelOnCouncilorSecrets(targetCouncilor) || targetCouncilor.transparentLoyalty) && viewofCouncilor.GetAttribute(CouncilorAttribute.Loyalty) < 20f)
				{
					return num * (float)num2 * (20f - viewofCouncilor.GetAttribute(CouncilorAttribute.Loyalty));
				}
				if (viewofCouncilor.GetAttribute(CouncilorAttribute.ApparentLoyalty) < 20f && targetCouncilor.elasticApparentLoyalty)
				{
					return num * (float)num2 * (20f - viewofCouncilor.GetAttribute(CouncilorAttribute.ApparentLoyalty));
				}
				if (flag)
				{
					return num * (float)num2;
				}
			}
			return -1f;
		}

		// Token: 0x060059EC RID: 23020 RVA: 0x00295A2C File Offset: 0x00293C2C
		public static float InvestigateAlienActivityPayoff(TIFactionState faction)
		{
			float num = 0f;
			if (!faction.IsAlienFaction && !faction.intelSharingFactions.Contains(GameStateManager.AlienFaction()))
			{
				num = (float)((Mathf.Max(12, GameStateManager.Time().currentQuarterSinceStart) - faction.alienInvestigations) * 10000);
				if (faction.currentlyDetectingHydra)
				{
					num *= 5f;
				}
				else if (faction.antiAlien)
				{
					num *= 2f;
				}
			}
			return num;
		}

		// Token: 0x060059ED RID: 23021 RVA: 0x00295A9C File Offset: 0x00293C9C
		public static float InvestigateCouncilorPayoff(TIFactionState investigatingFaction, TICouncilorState targetCouncilor)
		{
			if (investigatingFaction.IsAlienFaction && targetCouncilor.isAlien)
			{
				return -1f;
			}
			float num = -1f;
			if (!(targetCouncilor.faction == investigatingFaction) || investigatingFaction.HasIntelOnCouncilorSecrets(targetCouncilor))
			{
				if (targetCouncilor.OnEarth && !investigatingFaction.HasIntelOnCouncilorSecrets(targetCouncilor))
				{
					num = 100f * targetCouncilor.currentNation.CouncilControlPointFraction(investigatingFaction, true, false);
					if (investigatingFaction.currentlySearchingForHydraCouncilor)
					{
						num = 1000f;
						if (investigatingFaction.KnownAlienActivities.Select<TIRegionAlienActivityState, TIRegionState>((TIRegionAlienActivityState x) => x.region).Contains(targetCouncilor.ref_region))
						{
							num *= 1000f;
						}
					}
				}
				return num;
			}
			float num2 = investigatingFaction.Suspicion(targetCouncilor);
			if (num2 >= 10f)
			{
				return num2 * 100000f;
			}
			return num2 * 50f;
		}

		// Token: 0x060059EE RID: 23022 RVA: 0x00295B70 File Offset: 0x00293D70
		public static float ProtectMissionPayoff(TICouncilorState protector, TIGameState target)
		{
			float num = 0f;
			if (target.isCouncilorState)
			{
				TICouncilorState ref_councilor = target.ref_councilor;
				float num2 = protector.faction.GetViewofCouncilor(ref_councilor).EvaluateCouncilor();
				using (List<TIFactionState>.Enumerator enumerator = ref_councilor.knowsIveBeenSeenBy.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIFactionState tifactionState = enumerator.Current;
						if (ref_councilor.faction.FindGoals(GoalType.WarOnFaction, ref_councilor.faction, tifactionState, TIFactionState.GoalFilter.none, true).Count > 0)
						{
							num += 5f * (float)protector.GetAttribute(CouncilorAttribute.Security, true, true, true, false, false, false) * num2;
							num += (float)protector.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false);
						}
					}
					return num;
				}
			}
			if (target.isHabState)
			{
				num = AIEvaluators.EvaluateHab(protector.ref_faction, target.ref_hab, true, false) / 2f;
				num *= (float)(1 + target.ref_hab.CouncilorsPresentAndKnownToFaction(protector.ref_faction, true, null).Count);
			}
			else if (target.isRegionState)
			{
				if (protector.isAlien)
				{
					if (target.ref_region.alienLanding.Extant())
					{
						return 10000f;
					}
				}
				else
				{
					float num3 = target.ref_nation.CouncilControlPointFraction(protector.ref_faction, true, true) * (float)(1 + target.ref_nation.wars.Count) * (float)protector.faction.GoalsOfType(GoalType.WarOnFaction, false, true).Count;
					num += target.ref_region.boostPerYear_dekatons * num3 * AIEvaluators.GetAIRelativeValuation(FactionResource.Boost) * (float)(protector.ref_faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost) ? 2 : 1);
					num += (float)target.ref_region.missionControl * num3 * AIEvaluators.GetAIRelativeValuation(FactionResource.MissionControl) * (float)(protector.ref_faction.resourceIncomeDeficiencies.Contains(FactionResource.MissionControl) ? 3 : 1);
					num += (target.ref_region.antiSpaceDefenses ? (100f * num3) : 0f);
					num = Mathf.Min(num, 1000f);
				}
			}
			return num;
		}

		// Token: 0x060059EF RID: 23023 RVA: 0x00295D78 File Offset: 0x00293F78
		public static float EnthrallPublicPayoff(TIFactionState faction, TIRegionState region, float nationPayoff)
		{
			return AICouncilorMissionPlanner.PublicOpinionShiftPayoff(faction, region.nation, nationPayoff) * 4f * region.populationInMillions / region.nation.population_Millions;
		}

		// Token: 0x060059F0 RID: 23024 RVA: 0x00295DA0 File Offset: 0x00293FA0
		public static float PublicOpinionShiftPayoff(TIFactionState faction, TINationState nation, float nationPayoff)
		{
			float publicOpinionOfFaction = nation.GetPublicOpinionOfFaction(faction.ideology);
			int num = ((faction == nation.executiveFaction && faction.GetManagementGoalForNation(nation, true) != null) ? 20 : 1);
			if (num == 20 && nation.PublicOpinionMonthlyChange(faction, -10f))
			{
				num *= 10 * nation.numControlPoints_unclamped;
			}
			float num2 = ((faction.GoalsWithTarget(nation, null, true).Count == 0) ? 0.001f : 1f);
			return nationPayoff * (float)num * num2 * Mathf.Max(1f, Mathf.Sqrt(nation.population_Millions)) * 1E-05f * faction.aiValues.wantPopularity * (nation.singleIdeaCap - publicOpinionOfFaction) * (float)(faction.resourceIncomeDeficiencies.Contains(FactionResource.Influence) ? 3 : 1);
		}

		// Token: 0x060059F1 RID: 23025 RVA: 0x00295E68 File Offset: 0x00294068
		public static float ExtractPayoff(TIFactionState faction, TICouncilorState detainedCouncilor)
		{
			return faction.GetViewofCouncilor(detainedCouncilor).EvaluateCouncilor() * 10000000f;
		}

		// Token: 0x060059F2 RID: 23026 RVA: 0x00295E8C File Offset: 0x0029408C
		public static float EnthrallFactionElitesPayoff(TIFactionState faction, TIControlPoint controlPoint, float rawControlPointPayoff)
		{
			if (controlPoint.faction.permanentAlly(faction))
			{
				return -1f;
			}
			float num = rawControlPointPayoff * 2f * TINationState.GetIdeologicalDistance(faction.ideology, controlPoint.faction.ideology) / 4f * (float)(controlPoint.benefitsDisabled ? 30 : 1) * (float)((GameStateManager.AlienNation().extant && GameStateManager.AlienNation().AdjacentNations(false).Contains(controlPoint.nation)) ? 30 : 1);
			if ((faction.majorCPTrouble && !faction.alienProxyNeedsHelp) || (faction.minorCPTrouble && !controlPoint.nation.SignificantPower))
			{
				return -1f;
			}
			if (faction.minorCPTrouble)
			{
				num *= 0.5f;
			}
			if (controlPoint.faction.isAlienAppeaser && !faction.enemyWarFactions.Contains(controlPoint.faction))
			{
				if (controlPoint.faction.unlockedVictoryObjective)
				{
					return -1f;
				}
				num *= (faction.minorCPTrouble ? 0f : 0.01f);
			}
			return num;
		}

		// Token: 0x060059F3 RID: 23027 RVA: 0x00295F98 File Offset: 0x00294198
		public static float PurgePayoff(TIFactionState faction, TIControlPoint controlPoint, float controlPointPayoff)
		{
			if (faction.IsAlienFaction)
			{
				return AICouncilorMissionPlanner.EnthrallFactionElitesPayoff(faction, controlPoint, controlPointPayoff);
			}
			if (controlPoint.nation.numControlPoints <= 4)
			{
				List<TIFactionGoalState> list = faction.GoalsWithTarget(controlPoint.nation, null, true);
				list.AddRange(faction.GoalsWithTarget(controlPoint.faction, null, true));
				if (list.None<TIFactionGoalState>((TIFactionGoalState x) => x.GetMissionPayoffMultiplier(TIFactionState.purgeMission, 0f) > 0f))
				{
					return -1f;
				}
			}
			float num = -1f;
			if (!controlPoint.faction.permanentAlly(faction) && !faction.majorCPTrouble)
			{
				if (!controlPoint.nation.MajorGlobalPower)
				{
					if (faction.majorCPTrouble)
					{
						return -1f;
					}
					if (faction.minorCPTrouble && !controlPoint.nation.SignificantPower)
					{
						return -1f;
					}
				}
				num = controlPointPayoff * (float)Mathf.Max(controlPoint.nation.CountFactionControlPoints(faction, true, true, true), 1) * Mathf.Max(1f, TINationState.GetIdeologicalDistance(faction.ideology, controlPoint.faction.ideology) / 2f) * (float)(controlPoint.benefitsDisabled ? 10 : 1);
			}
			return num;
		}

		// Token: 0x060059F4 RID: 23028 RVA: 0x002960BC File Offset: 0x002942BC
		public static float SabotageProjectPayoff(TIFactionState faction, TIFactionState targetFaction)
		{
			if (targetFaction.permanentAlly(faction))
			{
				return -1f;
			}
			List<TIProjectTemplate> list = targetFaction.ProjectsVulnerableToSabotage(faction);
			return ((list != null) ? (list.Max<TIProjectTemplate>((TIProjectTemplate x) => targetFaction.GetProjectProgressValueByTemplate(x)) / 20f) : (-1f)) * faction.aiValues.dirtyTricks;
		}

		// Token: 0x060059F5 RID: 23029 RVA: 0x00296124 File Offset: 0x00294324
		public static float PassTechnologyPayoff(TIFactionState faction, TIFactionState targetFaction)
		{
			if ((targetFaction.IsAlienProxy || targetFaction.isAlienAppeaser) && !faction.AI_AtWarWithFaction(targetFaction) && !targetFaction.AI_AtWarWithFaction(faction))
			{
				float num = faction.ideologyCoordinates.x * -1000f;
				if (targetFaction.resourceIncomeDeficiencies.Contains(FactionResource.Research))
				{
					num *= 5f;
				}
				switch (targetFaction.selfAssessement)
				{
				case FactionSelfAssessment.LosingBig:
					num *= 3f;
					break;
				case FactionSelfAssessment.Losing:
					num *= 2f;
					break;
				case FactionSelfAssessment.Ahead:
					num *= 0.5f;
					break;
				case FactionSelfAssessment.WayAhead:
					num *= 0.1f;
					break;
				}
				return num;
			}
			return -1f;
		}

		// Token: 0x060059F6 RID: 23030 RVA: 0x002961D4 File Offset: 0x002943D4
		public static float SabotageSpaceFacilitiesPayoff(TIFactionState faction, TIRegionSpaceFacilityState targetFacility)
		{
			TINationState nation = targetFacility.region.nation;
			if (faction.NationWithFactionInterest(nation, true))
			{
				return 0f;
			}
			if (faction.enemyWarFactions.Intersect<TIFactionState>(nation.FactionsWithControlPoint).Count<TIFactionState>() == 0)
			{
				return 0f;
			}
			float num = 0f;
			switch (targetFacility.spaceFacilityType)
			{
			case SpaceFacilityType.launchFacility:
				if (faction.aiValues.wantSpaceFacilities >= 2f)
				{
					return 0f;
				}
				num = AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Boost, targetFacility.region.boostPerYear_dekatons) * (1f / faction.aiValues.wantSpaceFacilities);
				break;
			case SpaceFacilityType.missionControlFacility:
				if (faction.aiValues.wantSpaceFacilities >= 2f)
				{
					return 0f;
				}
				num = AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.MissionControl, (float)targetFacility.region.missionControl) * (1f / faction.aiValues.wantSpaceFacilities);
				break;
			case SpaceFacilityType.spaceDefenseFacility:
				num = faction.aiValues.wantSpaceWarCapability * 2f;
				if (faction.executiveNations.SelectMany<TINationState, TINationState>((TINationState x) => x.wars).Contains(targetFacility.ref_nation))
				{
					num *= 20f;
				}
				break;
			}
			switch (faction.selfAssessement)
			{
			case FactionSelfAssessment.Even:
				num *= 0.75f;
				break;
			case FactionSelfAssessment.Ahead:
				num *= 0.5f;
				break;
			case FactionSelfAssessment.WayAhead:
				num *= 0.25f;
				break;
			}
			if (nation.executiveFaction != null)
			{
				if (faction.IsAlienProxy && nation.executiveFaction.antiAlien)
				{
					num *= 20f * nation.executiveFaction.ideologyCoordinates.x;
				}
				if (faction.AI_AtWarWithFaction(nation.executiveFaction))
				{
					num *= 20f;
				}
			}
			return num * faction.aiValues.dirtyTricks;
		}

		// Token: 0x060059F7 RID: 23031 RVA: 0x002963B4 File Offset: 0x002945B4
		public static float SeizeAssetPayoff(TIFactionState faction, TIGameState target)
		{
			if (target.isHabState)
			{
				float num = AIEvaluators.EvaluateHab(faction, target.ref_hab, false, true);
				TIFactionGoalState tifactionGoalState = faction.GoalsWithTarget(target, GoalType.CaptureHab, true).FirstOrDefault<TIFactionGoalState>();
				num *= (float)((tifactionGoalState != null) ? tifactionGoalState.importance : 1);
				if (faction.AvailableMissionControlMinusFutureUsage <= target.ref_hab.MissionControlCost(true, null))
				{
					num *= 0.1f;
				}
				return num;
			}
			if (target.isSpaceShipState)
			{
				float num2 = 100f + target.ref_ship.SpaceCombatValue(false, 0f) * 10f;
				if (faction.AvailableMissionControlMinusFutureUsage <= target.ref_ship.missionControlConsumption)
				{
					num2 /= 3f;
				}
				return num2;
			}
			return -1f;
		}

		// Token: 0x060059F8 RID: 23032 RVA: 0x00296460 File Offset: 0x00294660
		public static float StabilizePayoff(TIFactionState faction, TINationState nation, float rawNationPayoff)
		{
			if (nation.unrest < 0.5f)
			{
				return 0f;
			}
			if (!nation.alienNation)
			{
				float num = rawNationPayoff / 50f;
				num *= (float)nation.CountFactionControlPoints(faction, true, true, true);
				num *= Mathf.Pow((nation.unrest + nation.unrestRestState) / 2f, 2f);
				if (nation.unrest < nation.unrestRestState && nation.unrest < 5f)
				{
					num *= 0.75f;
				}
				if (faction == nation.executiveFaction && nation.numControlPoints >= 4 && nation.unrest >= 1.75f)
				{
					num *= nation.unrest * nation.unrest * (float)(nation.numControlPoints - 3);
				}
				return num;
			}
			if (!faction.permanentAlly(nation.executiveFaction))
			{
				return -1f;
			}
			if (nation.unrest > 7f)
			{
				return nation.unrest * 10000000f;
			}
			if (faction.IsAlienFaction)
			{
				return 100f * nation.unrest * nation.unrest * nation.unrest * nation.unrest;
			}
			return 20f * nation.unrest * nation.unrest * nation.unrest * nation.unrest;
		}

		// Token: 0x060059F9 RID: 23033 RVA: 0x002965A0 File Offset: 0x002947A0
		public static float StealProjectPayoff(TIFactionState faction, TIFactionState targetFaction)
		{
			if (faction.IsAlienFaction)
			{
				return -1f;
			}
			bool shipBuilding = faction.shipBuilding;
			float dailyIncome = faction.GetDailyIncome(FactionResource.Research, false, false);
			float num;
			if (dailyIncome > 0f)
			{
				num = Mathf.Max(1f, targetFaction.GetDailyIncome(FactionResource.Research, false, false) / dailyIncome);
			}
			else
			{
				num = 1f;
			}
			IEnumerable<TIMissionTemplate> availableMissions = faction.GetAllPossibleMissions();
			float num2 = 20f * num;
			List<TIProjectTemplate> list = targetFaction.StealableProjects(faction);
			float? num3 = num2 * ((list != null) ? new float?(list.Max<TIProjectTemplate>((TIProjectTemplate x) => AIEvaluators.ScoreTech(faction, x, false, false, shipBuilding, availableMissions))) : null);
			if (num3 == null)
			{
				return -1f;
			}
			return num3.GetValueOrDefault();
		}

		// Token: 0x060059FA RID: 23034 RVA: 0x0029669C File Offset: 0x0029489C
		public static float TerrorizePayoff(TIFactionState faction, TIRegionState targetRegion, Dictionary<TIControlPoint, float> rawControlPointPayoffs)
		{
			if (faction.IsAlienFaction && !AIEvaluators.ShouldAliensGoLoud())
			{
				return -1f;
			}
			TIControlPoint ticontrolPoint = targetRegion.ref_nation.controlPoints.First<TIControlPoint>((TIControlPoint x) => x.CanBeTerrorized());
			float yearlyIncome = GameStateManager.AlienAppeaser().GetYearlyIncome(FactionResource.Influence, false, false, false);
			if (ticontrolPoint != null && yearlyIncome > 0f && rawControlPointPayoffs.ContainsKey(ticontrolPoint))
			{
				FactionIdeology ideology = faction.ideology.ideology;
				TIFactionState faction2 = ticontrolPoint.faction;
				float ideologicalDistance = TINationState.GetIdeologicalDistance(ideology, (faction2 != null) ? faction2.ideology.ideology : FactionIdeology.Undecided);
				float num = rawControlPointPayoffs[ticontrolPoint] * 0.6f * ideologicalDistance / 2f;
				if (GameStateManager.AlienAppeaser().unlockedVictoryObjective)
				{
					num *= 2f;
				}
				return num;
			}
			return -1f;
		}

		// Token: 0x060059FB RID: 23035 RVA: 0x00296770 File Offset: 0x00294970
		public static float TurnCouncilorPayoff(TIFactionState faction, TICouncilorState targetCouncilor)
		{
			if (faction.permanentAlly(targetCouncilor.faction))
			{
				return -1f;
			}
			List<TITraitTemplate> traits = faction.GetViewofCouncilor(targetCouncilor).traits;
			bool flag;
			if (traits == null)
			{
				flag = true;
			}
			else
			{
				flag = traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.LoyaltyMonitor);
			}
			if (flag)
			{
				return -1f;
			}
			float num = 1000f * TINationState.GetIdeologicalDistance(faction.ideology, targetCouncilor.faction.ideology);
			if (faction.turnedCouncilors.Any<TICouncilorState>((TICouncilorState x) => x.faction == targetCouncilor.faction))
			{
				num /= 5f;
			}
			return num;
		}

		// Token: 0x060059FC RID: 23036 RVA: 0x00296830 File Offset: 0x00294A30
		public static float XenoformingPayoff(TIFactionState faction, TIRegionState region)
		{
			if (!AIEvaluators.ShouldAliensXenoform())
			{
				return -1f;
			}
			if (region.xenoforming.xenoformingLevel == 0f)
			{
				return (float)(50 * (region.nation.alienNation ? 3 : 1));
			}
			if (region.xenoforming.xenoformingLevel < 90f)
			{
				return (float)(100 * (region.nation.alienNation ? 3 : 1));
			}
			return 0f;
		}

		// Token: 0x060059FD RID: 23037 RVA: 0x002968A0 File Offset: 0x00294AA0
		public void SetRawNationPayoffsByFaction(TIFactionState faction, bool force)
		{
			if (!force)
			{
				return;
			}
			if (!this.rawNationPayoffs.ContainsKey(faction))
			{
				this.rawNationPayoffs.Add(faction, new Dictionary<TINationState, float>());
			}
			else
			{
				this.rawNationPayoffs[faction].Clear();
			}
			if (!this.rawControlPointPayoffs.ContainsKey(faction))
			{
				this.rawControlPointPayoffs.Add(faction, new Dictionary<TIControlPoint, float>());
			}
			else
			{
				this.rawControlPointPayoffs[faction].Clear();
			}
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				this.rawNationPayoffs[faction].Add(tinationState, AIEvaluators.EvaluateNation(faction, tinationState));
				foreach (TIControlPoint ticontrolPoint in tinationState.controlPoints)
				{
					this.rawControlPointPayoffs[faction].Add(ticontrolPoint, AIEvaluators.EvaluateControlPoint(faction, ticontrolPoint));
				}
			}
		}

		// Token: 0x060059FE RID: 23038 RVA: 0x002969BC File Offset: 0x00294BBC
		public void SetPayoffValues(TIFactionState faction, bool forceRecalculation)
		{
			if (this.rawControlPointPayoffs.ContainsKey(faction))
			{
				if (faction == this.lastFactionPayoffValuesRecorded && !forceRecalculation)
				{
					return;
				}
			}
			else
			{
				this.SetRawNationPayoffsByFaction(faction, true);
			}
			this.controlPointPayoffs.Clear();
			this.nationPayoffs.Clear();
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				foreach (TIControlPoint ticontrolPoint in tinationState.controlPoints)
				{
					float num;
					if (!this.rawControlPointPayoffs[faction].TryGetValue(ticontrolPoint, out num))
					{
						this.rawControlPointPayoffs[faction].Add(ticontrolPoint, AIEvaluators.EvaluateControlPoint(faction, ticontrolPoint));
						num = this.rawControlPointPayoffs[faction][ticontrolPoint];
					}
					if (faction.minorCPTrouble)
					{
						if (faction.majorCPTrouble)
						{
							if (!tinationState.MajorGlobalPower)
							{
								num *= (float)tinationState.numControlPoints / 10f;
							}
						}
						else if (!tinationState.SignificantPower)
						{
							num *= (float)tinationState.numControlPoints / 10f;
						}
					}
					this.controlPointPayoffs.Add(ticontrolPoint, num);
				}
				this.nationPayoffs.Add(tinationState, this.NationPayoff_Current(faction, tinationState));
			}
			this.lastFactionPayoffValuesRecorded = faction;
			AICouncilorMissionPlanner.cachedPayoffFrame = TIFrameCounter.FrameCount;
		}

		// Token: 0x060059FF RID: 23039 RVA: 0x00296B44 File Offset: 0x00294D44
		private int SetIdealSpendForMission(AIMissionEntry missionEntry, int missionsWithSameResource)
		{
			float num = 0f;
			int num2 = 0;
			FactionResource resourceType = missionEntry.mission.cost.resourceType;
			switch (resourceType)
			{
			case FactionResource.Money:
				num = this.availableResources[FactionResource.Money];
				break;
			case FactionResource.Influence:
				if (missionEntry.objective || num > (float)AIEvaluators.AbundantValue(FactionResource.Influence))
				{
					num = this.availableResources[FactionResource.Influence];
				}
				else if (missionEntry.payoff < 500f)
				{
					num = this.availableResources[FactionResource.Influence] / 20f;
				}
				else if (missionEntry.payoff < 1000f)
				{
					num = this.availableResources[FactionResource.Influence] / 10f;
				}
				else if (missionEntry.payoff < 10000f)
				{
					num = this.availableResources[FactionResource.Influence] / 6f;
				}
				else if (missionEntry.payoff < 100000f)
				{
					num = this.availableResources[FactionResource.Influence] / 4f;
				}
				else
				{
					num = this.availableResources[FactionResource.Influence] / 2f;
				}
				if (missionEntry.councilor.faction.councilors.Count < missionEntry.councilor.faction.maxCouncilSize)
				{
					num /= 2f;
				}
				break;
			case FactionResource.Operations:
				if (!missionEntry.objective && missionEntry.faction.currentlyDetectingHydra && num < (float)AIEvaluators.AbundantValue(FactionResource.Operations))
				{
					num = Mathf.Max(this.availableResources[FactionResource.Operations] - 15f, 0f) / 3f;
				}
				else
				{
					num = this.availableResources[FactionResource.Operations];
				}
				break;
			default:
				num = this.availableResources[resourceType];
				break;
			}
			num = Mathf.Max(0f, num);
			if (!missionEntry.objective)
			{
				num /= (float)Mathf.Max(1, missionsWithSameResource);
			}
			int num3 = missionEntry.councilor.CurrentMaxSliderSteps(missionEntry.mission, 1f);
			float num4 = 0f;
			bool flag = AIEvaluators.Abundant(missionEntry.faction, resourceType, num, missionEntry.faction.GetDailyIncome(resourceType, false, false) > 0f, 1f);
			for (int i = 0; i <= num3; i++)
			{
				float cost = missionEntry.mission.cost.GetCost((float)i, missionEntry.councilor, null);
				if (cost <= num)
				{
					float num5 = ((cost > 0.05f * num && !flag && !missionEntry.objective) ? AIEvaluators.FixedResourceValue(missionEntry.councilor.faction, missionEntry.mission.cost.resourceType, cost, false) : 0f);
					float successChance = missionEntry.mission.resolutionMethod.GetSuccessChance(missionEntry.mission, missionEntry.councilor, missionEntry.target, cost, false);
					float num6 = missionEntry.payoff * successChance - num5;
					if (successChance < missionEntry.acceptableMinimumSuccess)
					{
						num6 *= 0.1f * successChance / missionEntry.acceptableMinimumSuccess;
					}
					if (num6 > num4)
					{
						num2 = (int)cost;
						num4 = num6;
					}
				}
			}
			Dictionary<FactionResource, float> dictionary = this.availableResources;
			FactionResource factionResource = resourceType;
			dictionary[factionResource] -= (float)num2;
			return num2;
		}

		// Token: 0x06005A00 RID: 23040 RVA: 0x00296E4C File Offset: 0x0029504C
		public void AddScoredPolicyOption(PolicyOptionWithTarget policyOption, float score)
		{
			if (policyOption.policyType == PolicyType.WarOption)
			{
				TIFactionState.LogAI(string.Concat(new string[]
				{
					"War: ",
					policyOption.actingNation.displayName,
					"/",
					policyOption.actingNation.executiveFaction.displayName,
					" attack ",
					policyOption.target.displayName,
					" Score: ",
					score.ToString()
				}), false);
			}
			Dictionary<PolicyOptionWithTarget, float> dictionary;
			if (policyOption.policy.RequiresTargetConfirm())
			{
				PolicyOptionWithTarget policyOptionWithTarget = new PolicyOptionWithTarget(policyOption.target.ref_nation, policyOption.policy, policyOption.actingNation);
				if (this.scoredPolicyOptions.ContainsKey(policyOptionWithTarget))
				{
					dictionary = this.scoredPolicyOptions;
					PolicyOptionWithTarget policyOptionWithTarget2 = policyOptionWithTarget;
					dictionary[policyOptionWithTarget2] *= 2f;
					return;
				}
			}
			if (!this.scoredPolicyOptions.ContainsKey(policyOption))
			{
				this.scoredPolicyOptions.Add(policyOption, score);
				return;
			}
			dictionary = this.scoredPolicyOptions;
			dictionary[policyOption] += score;
		}

		// Token: 0x06005A01 RID: 23041 RVA: 0x00296F58 File Offset: 0x00295158
		public static float ScorePolicyOption(PolicyOptionWithTarget policyOption, TIFactionState faction, int importance, Dictionary<TINationState, Dictionary<PolicyType, int>> targetNationPolicies, Dictionary<TINationState, float> nationPayoffs)
		{
			float num = 0f;
			if (policyOption.policy.DegradesRelations())
			{
				if (policyOption.policy.GetPolicyType() == PolicyType.WarOption || policyOption.policy.GetPolicyType() == PolicyType.DeclareIndependenceOption)
				{
					TIWarState tiwarState = policyOption.target as TIWarState;
					if (tiwarState != null)
					{
						if (AIEvaluators.NuclearDeterred(faction, policyOption.actingNation, tiwarState.EnemyWarLeader(policyOption.actingNation, true), importance, tiwarState))
						{
							num = -1f;
						}
						else if (policyOption.actingNation.allies.Contains(tiwarState.attacker))
						{
							num = AIEvaluators.ScoreIncreasingConflict(policyOption.actingNation, tiwarState.defender, policyOption.actingNation.executiveFaction == tiwarState.defender.executiveFaction, policyOption.policy.GetPolicyType());
							if (nationPayoffs != null && nationPayoffs.ContainsKey(tiwarState.defender))
							{
								num *= nationPayoffs[tiwarState.defender];
							}
						}
						else if (policyOption.actingNation.allies.Contains(tiwarState.defender))
						{
							num = AIEvaluators.ScoreIncreasingConflict(policyOption.actingNation, tiwarState.attacker, policyOption.actingNation.executiveFaction == tiwarState.attacker.executiveFaction, policyOption.policy.GetPolicyType());
							if (tiwarState.attacker.extant)
							{
								if (nationPayoffs != null && nationPayoffs.ContainsKey(tiwarState.attacker))
								{
									num *= nationPayoffs[tiwarState.attacker];
								}
							}
							else if (nationPayoffs != null && nationPayoffs.ContainsKey(tiwarState.defender))
							{
								num *= nationPayoffs[tiwarState.defender];
							}
						}
					}
					else
					{
						TINationState ref_nation = policyOption.target.ref_nation;
						TINationState tinationState = ref_nation.DefensiveAllianceProspectiveWarLeader();
						if ((policyOption.actingNation.executiveFaction != null && tinationState.executiveFaction == policyOption.actingNation.executiveFaction) || AIEvaluators.NuclearDeterred(faction, policyOption.actingNation, ref_nation, importance, null))
						{
							num = -1f;
						}
						else
						{
							num = AIEvaluators.ScoreIncreasingConflict(policyOption.actingNation, ref_nation, policyOption.actingNation.executiveFaction == ref_nation.executiveFaction, policyOption.policy.GetPolicyType());
							if (targetNationPolicies != null && targetNationPolicies.ContainsKey(ref_nation) && targetNationPolicies[ref_nation].ContainsKey(policyOption.policyType))
							{
								importance = Mathf.Max(importance, targetNationPolicies[ref_nation][policyOption.policyType]);
							}
							if (policyOption.policy.GetPolicyType() == PolicyType.WarOption && policyOption.actingNation.numStandardArmies == 0 && policyOption.actingNation.numNuclearWeapons > 0)
							{
								num = -1f;
							}
							else if (num >= 1f && nationPayoffs != null && nationPayoffs.ContainsKey(ref_nation))
							{
								num *= nationPayoffs[ref_nation];
							}
						}
					}
				}
				else
				{
					TINationState tinationState2 = null;
					if (policyOption.target.isNationState)
					{
						tinationState2 = policyOption.target.ref_nation;
					}
					else
					{
						TIFederationState tifederationState = policyOption.target as TIFederationState;
						if (tifederationState != null)
						{
							tinationState2 = tifederationState.leadNation;
						}
					}
					num = AIEvaluators.ScoreIncreasingConflict(policyOption.actingNation, tinationState2, policyOption.actingNation.executiveFaction == tinationState2.executiveFaction, policyOption.policy.GetPolicyType());
					if (tinationState2 != null && targetNationPolicies != null && targetNationPolicies.ContainsKey(tinationState2) && targetNationPolicies[tinationState2].ContainsKey(policyOption.policyType))
					{
						importance = Mathf.Max(importance, targetNationPolicies[tinationState2][policyOption.policyType]);
					}
					if (nationPayoffs != null && nationPayoffs.ContainsKey(tinationState2))
					{
						num *= nationPayoffs[tinationState2];
					}
					TIPolicyOptionWithConfirm tipolicyOptionWithConfirm = policyOption.policy as TIPolicyOptionWithConfirm;
					if (tipolicyOptionWithConfirm != null)
					{
						float num2 = tipolicyOptionWithConfirm.AIAgreeChance_Prospective(policyOption.actingNation, policyOption.target);
						if (num2 < 1f && policyOption.policyType == PolicyType.LeaveFederationOption && policyOption.actingNation.federation.AttemptedToLeaveDarkFederationSince(policyOption.actingNation, 3f))
						{
							num2 = 0f;
						}
						num *= num2;
					}
				}
			}
			else if (policyOption.policy.ImprovesRelations())
			{
				if (policyOption.target.isWarState)
				{
					TIWarState ref_war = policyOption.target.ref_war;
					TINationState tinationState3 = ref_war.EnemyWarLeader(policyOption.actingNation, false);
					if (policyOption.policyType == PolicyType.EndWarOption)
					{
						float num3 = policyOption.actingNation.AssessOverallWarStatus() - policyOption.actingNation.historyWarStatus[0];
						List<TIFactionGoalState> list = faction.FindGoals(TIFactionGoalState.NationManagementGoals, policyOption.actingNation, policyOption.actingNation, TIFactionState.GoalFilter.none, true);
						int num4;
						if (list.Count <= 0)
						{
							num4 = 1;
						}
						else
						{
							num4 = list.Max<TIFactionGoalState>((TIFactionGoalState x) => x.importance);
						}
						int num5 = num4;
						if (AIEvaluators.AlwaysEndConflict(policyOption.actingNation, tinationState3))
						{
							num = (float)num5 * AIEvaluators.ScoreImprovedRelations(policyOption.actingNation, tinationState3, policyOption.actingNation.executiveFaction != null && policyOption.actingNation.executiveFaction == tinationState3.executiveFaction);
						}
						else if ((!policyOption.actingNation.WinningWarAgainst(tinationState3) && num3 < 0f) || ref_war.stalemate)
						{
							float num6 = -policyOption.actingNation.WinningWarBy(ref_war.EnemyWarLeader(policyOption.actingNation, false));
							if (num6 > policyOption.actingNation.militaryStrength || (ref_war.stalemate && !policyOption.actingNation.alienNation && !tinationState3.alienNation))
							{
								float num7 = (policyOption.policy as TIPolicyOptionWithConfirm).AIAgreeChance_Prospective(policyOption.actingNation, ref_war);
								num = (float)num5 * num7 * (ref_war.stalemate ? ref_war.stalemateDuration_days : num6);
							}
						}
					}
					else if (AIEvaluators.AlwaysEndConflict(policyOption.actingNation, tinationState3))
					{
						num = AIEvaluators.ScoreImprovedRelations(policyOption.actingNation, tinationState3, policyOption.actingNation.executiveFaction != null && policyOption.actingNation.executiveFaction == tinationState3.executiveFaction);
					}
					else
					{
						num = -policyOption.actingNation.WinningWarBy(policyOption.target.ref_war.EnemyWarLeader(policyOption.actingNation, false));
					}
				}
				else if (policyOption.policy.GetPolicyType() == PolicyType.TransferRegionsOption)
				{
					if (policyOption.actingNation.alienNation && policyOption.CausesGuaranteedOneWayNationExpansion)
					{
						return 1000000f * (1f + (float)policyOption.target.ref_region.nationalGDPShareValue_bn);
					}
					if (policyOption.actingNation.ref_faction.permanentAlly(policyOption.target.ref_faction))
					{
						if (policyOption.target.ref_faction.GoalsWithTarget(policyOption.target, GoalType.PillageNation, true).Count <= 0 || policyOption.actingNation.ref_faction.GoalsWithTarget(policyOption.target, GoalType.ExpandNation, true).Count > 0)
						{
						}
						num = -1f;
					}
					else if (policyOption.actingNation.ref_faction.permanentAlly(policyOption.target.ref_faction))
					{
						if (policyOption.target.ref_faction.GoalsWithTarget(policyOption.target, GoalType.ExpandNation, true).Count > 0)
						{
							num = AIEvaluators.ScoreImprovedRelations(policyOption.actingNation, policyOption.target.ref_nation, policyOption.actingNation.executiveFaction != null && policyOption.actingNation.executiveFaction == policyOption.target.ref_nation.executiveFaction);
						}
					}
					else if (policyOption.actingNation.rivals.Contains(policyOption.target.ref_nation) && policyOption.actingNation.militaryStrength * 2f < policyOption.target.ref_nation.militaryStrength && policyOption.actingNation.NumNuclearWeaponsDefendingMe() == 0)
					{
						num = AIEvaluators.ScoreImprovedRelations(policyOption.actingNation, policyOption.target.ref_nation, policyOption.actingNation.executiveFaction != null && policyOption.actingNation.executiveFaction == policyOption.target.ref_nation.executiveFaction);
					}
				}
				else
				{
					num = AIEvaluators.ScoreImprovedRelations(policyOption.actingNation, policyOption.target.ref_nation, policyOption.actingNation.executiveFaction != null && policyOption.actingNation.executiveFaction == policyOption.target.ref_nation.executiveFaction);
				}
				if (policyOption.target.ref_nation != null)
				{
					if (policyOption.target.ref_nation.executiveFaction != faction && !faction.permanentAlly(policyOption.target.ref_nation.executiveFaction))
					{
						if (policyOption.policy is JoinFederationOption)
						{
							num *= 0.2f;
						}
						else if (policyOption.policy is UnificationOption)
						{
							num = -1f;
						}
					}
					else if ((double)policyOption.target.ref_nation.CouncilControlPointFraction(faction, false, true) < 0.5)
					{
						if (policyOption.policy is JoinFederationOption)
						{
							num *= 0.6f;
						}
						else if (policyOption.policy is UnificationOption)
						{
							num *= 0.5f;
						}
					}
				}
				if (num > 0f)
				{
					TINationState tinationState4 = null;
					if (policyOption.target.isNationState)
					{
						tinationState4 = policyOption.target.ref_nation;
						if (nationPayoffs != null && nationPayoffs.ContainsKey(tinationState4))
						{
							num = nationPayoffs[tinationState4];
						}
					}
					else
					{
						TIFederationState tifederationState2 = policyOption.target as TIFederationState;
						if (tifederationState2 != null)
						{
							tinationState4 = tifederationState2.leadNation;
							if (nationPayoffs != null && nationPayoffs.ContainsKey(tinationState4))
							{
								num = nationPayoffs[tinationState4];
							}
						}
					}
					if (tinationState4 != null && targetNationPolicies != null && targetNationPolicies.ContainsKey(tinationState4) && targetNationPolicies[tinationState4].ContainsKey(policyOption.policyType))
					{
						importance = Mathf.Max(importance, targetNationPolicies[tinationState4][policyOption.policyType]);
					}
					TIPolicyOptionWithConfirm tipolicyOptionWithConfirm2 = policyOption.policy as TIPolicyOptionWithConfirm;
					if (tipolicyOptionWithConfirm2 != null)
					{
						num *= tipolicyOptionWithConfirm2.AIAgreeChance_Prospective(policyOption.actingNation, policyOption.target);
					}
					num *= (float)importance;
				}
			}
			else if (!policyOption.actingNation.alienNation)
			{
				List<TIFactionGoalState> list2 = policyOption.actingNation.executiveFaction.GoalsWithTarget(policyOption.actingNation, GoalType.NeutralizeNation, true);
				switch (policyOption.policyType)
				{
				case PolicyType.PeacefulBreakupOption:
					if (list2.Count > 0)
					{
						num = (float)(list2.Max<TIFactionGoalState>((TIFactionGoalState x) => x.importance) * importance);
					}
					else if (policyOption.actingNation.civilWar && policyOption.actingNation.cohesion <= 2f)
					{
						num = policyOption.actingNation.unrest * (float)importance;
					}
					else if (!policyOption.target.ref_nation.extant && policyOption.actingNation.TotalOwningFaction == faction && policyOption.target.ref_nation.claims.Count > policyOption.actingNation.claims.Count)
					{
						if (faction.minorCPTrouble)
						{
							num = -1f;
						}
						else
						{
							num = (float)policyOption.target.ref_nation.claims.Count;
						}
					}
					break;
				case PolicyType.DisbandArmyOption:
					if (list2.Count > 0)
					{
						num = (float)(list2.Max<TIFactionGoalState>((TIFactionGoalState x) => x.importance) * importance);
					}
					break;
				case PolicyType.DisarmNuclearWeaponsOption:
					if (list2.Count > 0)
					{
						num = (float)list2.Max<TIFactionGoalState>((TIFactionGoalState x) => x.importance);
					}
					break;
				}
			}
			else
			{
				num = -1f;
			}
			return num;
		}

		// Token: 0x06005A02 RID: 23042 RVA: 0x00297B30 File Offset: 0x00295D30
		public void ModifyScoredPolicyOptionForCoordinatedMissions(AIMissionEntry selectedEntry, List<TICouncilorState> availableCouncilors, ref Dictionary<AIMissionEntry, float> missionDictionary)
		{
			using (IEnumerator<TICouncilorState> enumerator = availableCouncilors.Where<TICouncilorState>((TICouncilorState councilor) => !councilor.HasMission).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TICouncilorState otherCouncilor = enumerator.Current;
					if (selectedEntry.mission == TIFactionState.purgeMission)
					{
						AIMissionEntry aimissionEntry = missionDictionary.Keys.FirstOrDefault<AIMissionEntry>((AIMissionEntry x) => x.target == selectedEntry.target && x.councilor == otherCouncilor && x.mission == TIFactionState.crackdownMission);
						if (aimissionEntry != null)
						{
							Dictionary<AIMissionEntry, float> dictionary = missionDictionary;
							AIMissionEntry aimissionEntry2 = aimissionEntry;
							dictionary[aimissionEntry2] *= Mathf.Max(1f, (float)otherCouncilor.GetAttribute(aimissionEntry.mission.primaryAttackerStat, true, true, true, false, false, false) / 3f);
						}
					}
					else if (selectedEntry.mission == TIFactionState.crackdownMission)
					{
						AIMissionEntry aimissionEntry3 = missionDictionary.Keys.FirstOrDefault<AIMissionEntry>((AIMissionEntry x) => x.target == selectedEntry.target && x.councilor == otherCouncilor && x.mission == TIFactionState.purgeMission);
						if (aimissionEntry3 != null)
						{
							Dictionary<AIMissionEntry, float> dictionary = missionDictionary;
							AIMissionEntry aimissionEntry2 = aimissionEntry3;
							dictionary[aimissionEntry2] *= Mathf.Max(1f, (float)otherCouncilor.GetAttribute(aimissionEntry3.mission.primaryAttackerStat, true, true, true, false, false, false) / 3f);
						}
					}
					else if (selectedEntry.mission == TIFactionState.assaultAlienAssetMission || selectedEntry.mission == TIFactionState.seizeHabMission)
					{
						AIMissionEntry aimissionEntry4 = missionDictionary.Keys.FirstOrDefault<AIMissionEntry>((AIMissionEntry x) => x.target == selectedEntry.target && x.councilor == otherCouncilor && x.mission == TIFactionState.protectMission);
						if (aimissionEntry4 != null)
						{
							Dictionary<AIMissionEntry, float> dictionary = missionDictionary;
							AIMissionEntry aimissionEntry2 = aimissionEntry4;
							dictionary[aimissionEntry2] *= Mathf.Max(1f, (float)otherCouncilor.GetAttribute(CouncilorAttribute.Security, true, true, true, false, false, false) / 3f);
						}
					}
				}
			}
		}

		// Token: 0x06005A03 RID: 23043 RVA: 0x00297D58 File Offset: 0x00295F58
		[return: TupleElementNames(new string[] { "Suspect", "ExcessDefense" })]
		public static IEnumerable<ValueTuple<TICouncilorState, float>> FilterCouncilorsForPossibleAlien(IEnumerable<TICouncilorState> suspects, TICouncilorState exampleCouncilor, TIMissionTemplate exampleMission)
		{
			TICouncilorTypeTemplate ticouncilorTypeTemplate = TemplateManager.Find<TICouncilorTypeTemplate>("Alien", false);
			int num = ((ticouncilorTypeTemplate != null) ? ticouncilorTypeTemplate.baseEspionage : 0);
			int baseDefense = num;
			return from x in suspects.Select<TICouncilorState, ValueTuple<TICouncilorState, float>>(delegate(TICouncilorState x)
				{
					float num2 = (exampleMission.resolutionMethod as TIMissionResolution_Contested).SumDefendingModifiers(exampleMission, exampleCouncilor, x, 0f);
					return new ValueTuple<TICouncilorState, float>(x, num2 - (float)baseDefense);
				})
				where x.Item2 >= 0f
				select x;
		}

		// Token: 0x06005A04 RID: 23044 RVA: 0x00297DD0 File Offset: 0x00295FD0
		public List<TIGameState> GetHuntListForAliens(TIFactionState faction, List<TICouncilorState> candidateCouncilors, TIMissionTemplate objectiveMission, bool alwaysHunt, List<TICouncilorState> alreadyTargetedCouncilors, TINationState recentAlienControlPointGift, float timeSinceAlienControlPointGift_days)
		{
			List<TIGameState> list = new List<TIGameState>();
			List<TICouncilorState> list2 = faction.CurrentKnownUnidentifiedCouncilors().Except<TICouncilorState>(alreadyTargetedCouncilors).ToList<TICouncilorState>();
			if (list2.Count > 0)
			{
				Dictionary<TICouncilorState, float> councilorScores;
				if (candidateCouncilors.Count > 0)
				{
					councilorScores = AICouncilorMissionPlanner.FilterCouncilorsForPossibleAlien(list2, candidateCouncilors[0], objectiveMission).ToDictionary<ValueTuple<TICouncilorState, float>, TICouncilorState, float>(([TupleElementNames(new string[] { "Suspect", "ExcessDefense" })] ValueTuple<TICouncilorState, float> x) => x.Item1, ([TupleElementNames(new string[] { "Suspect", "ExcessDefense" })] ValueTuple<TICouncilorState, float> x) => x.Item2 / 10f);
					list2 = councilorScores.Select<KeyValuePair<TICouncilorState, float>, TICouncilorState>((KeyValuePair<TICouncilorState, float> x) => x.Key).ToList<TICouncilorState>();
				}
				else
				{
					councilorScores = list2.ToDictionary<TICouncilorState, TICouncilorState, float>((TICouncilorState x) => x, (TICouncilorState x) => 0f);
				}
				List<TIRegionState> list3 = faction.KnownAlienActivities.Select<TIRegionAlienActivityState, TIRegionState>((TIRegionAlienActivityState x) => x.region).ToList<TIRegionState>();
				if (faction.MostRecentAlienSiteAge_days(true) <= 45f)
				{
					list3.AddUnique(faction.MostRecentAlienSite(true).ref_region);
				}
				TICouncilorTypeTemplate ticouncilorTypeTemplate = TemplateManager.Find<TICouncilorTypeTemplate>("Alien", false);
				if (ticouncilorTypeTemplate != null)
				{
					int baseEspionage = ticouncilorTypeTemplate.baseEspionage;
				}
				using (List<TICouncilorState>.Enumerator enumerator = list2.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TICouncilorState possibleCouncilor = enumerator.Current;
						if (possibleCouncilor.OnEarth)
						{
							if (list3.Contains(faction.GetViewofCouncilor(possibleCouncilor).location))
							{
								Dictionary<TICouncilorState, float> dictionary = councilorScores;
								TICouncilorState ticouncilorState = possibleCouncilor;
								dictionary[ticouncilorState] += 3f;
							}
							else if (list3.Any<TIRegionState>((TIRegionState x) => x.AdjacentRegions(false).Contains(possibleCouncilor.location)))
							{
								Dictionary<TICouncilorState, float> dictionary = councilorScores;
								TICouncilorState ticouncilorState = possibleCouncilor;
								dictionary[ticouncilorState] += 2f;
							}
							else
							{
								float num;
								possibleCouncilor.ref_nation.historyPublicOpinion[31].TryGetValue(GameStateManager.AlienProxy().ideology.ideology, out num);
								if (possibleCouncilor.ref_nation.GetPublicOpinionOfFaction(GameStateManager.AlienProxy().ideology) - num < 10f)
								{
									TIFactionState newExecutive = possibleCouncilor.ref_nation.lastExecutiveChange.newExecutive;
									if (newExecutive == null || !newExecutive.IsAlienProxy)
									{
										goto IL_0303;
									}
									TIDateTime date = possibleCouncilor.ref_nation.lastExecutiveChange.date;
									if (date == null || date.DifferenceInDays(TITimeState.Now()) >= (double)30)
									{
										goto IL_0303;
									}
								}
								Dictionary<TICouncilorState, float> dictionary = councilorScores;
								TICouncilorState ticouncilorState = possibleCouncilor;
								dictionary[ticouncilorState] += 1f;
							}
							IL_0303:
							if (recentAlienControlPointGift != null)
							{
								if (recentAlienControlPointGift.regions.Contains(possibleCouncilor.location))
								{
									Dictionary<TICouncilorState, float> dictionary = councilorScores;
									TICouncilorState ticouncilorState = possibleCouncilor;
									dictionary[ticouncilorState] += 3f;
								}
								else if (recentAlienControlPointGift.IsAdjacentToRegion(possibleCouncilor.location.ref_region, false))
								{
									Dictionary<TICouncilorState, float> dictionary = councilorScores;
									TICouncilorState ticouncilorState = possibleCouncilor;
									dictionary[ticouncilorState] += 2f;
								}
							}
						}
					}
				}
				if (councilorScores.Any<KeyValuePair<TICouncilorState, float>>())
				{
					float highest = councilorScores.Values.Max();
					if (highest >= 0.7f || alwaysHunt || TIUtilities.RandomFloatValue() < 0.35f)
					{
						list = list2.Where<TICouncilorState>((TICouncilorState x) => councilorScores[x] == highest).ToList<TICouncilorState>().ConvertAll<TIGameState>((TICouncilorState x) => x.ref_gameState);
					}
				}
			}
			return list;
		}

		// Token: 0x06005A05 RID: 23045 RVA: 0x00298230 File Offset: 0x00296430
		public void MissionPhasePrepCoroutine(TIFactionState faction)
		{
			CoroutineDummy.Singleton.StartCoroutine(this.MissionPhasePrep(faction));
		}

		// Token: 0x06005A06 RID: 23046 RVA: 0x00298244 File Offset: 0x00296444
		public void PlanMissionsCoroutine(TIFactionState faction)
		{
			CoroutineDummy.Singleton.StartCoroutine(this.PlanMissions(faction));
		}

		// Token: 0x06005A07 RID: 23047 RVA: 0x00298258 File Offset: 0x00296458
		protected IEnumerator MissionPhasePrep(TIFactionState faction)
		{
			while (this.cameraManager.IsAnimating)
			{
				yield return null;
			}
			this.SetRawNationPayoffsByFaction(faction, true);
			if (this.AISmoothing)
			{
				yield return null;
			}
			AIDailyFactionPlanner.DisableOwnNations(faction, this.rawControlPointPayoffs[faction]);
			if (this.AISmoothing)
			{
				yield return null;
			}
			AIDailyFactionPlanner.ManageAlliancesAndRivalries(faction);
			while (TIPromptQueueState.ActivePlayerHasSaveBlockingPrompt())
			{
				yield return null;
			}
			faction.preppingForMissions = false;
			if (GameStateManager.AllFactions().All<TIFactionState>((TIFactionState x) => !x.preppingForMissions))
			{
				GameControl.eventManager.TriggerEvent(new MissionPhasePrepComplete(), null, Array.Empty<object>());
			}
			yield break;
		}

		// Token: 0x06005A08 RID: 23048 RVA: 0x0029826E File Offset: 0x0029646E
		protected IEnumerator PlanMissions(TIFactionState faction)
		{
			while (this.cameraManager.IsAnimating || TIPromptQueueState.ActivePlayerHasSaveBlockingPrompt())
			{
				yield return null;
			}
			for (;;)
			{
				if (!GameStateManager.AllFactions().Any<TIFactionState>((TIFactionState x) => x.preppingForMissions))
				{
					break;
				}
				yield return null;
			}
			for (;;)
			{
				if (!GameStateManager.AllFactions().Any<TIFactionState>((TIFactionState x) => x.planningMissions))
				{
					break;
				}
				yield return null;
			}
			faction.planningMissions = true;
			AIDailyFactionPlanner.ManagePriorityNationControlGoalsForFaction(faction);
			Task<AICouncilorMissionPlan> task = Task.Run<AICouncilorMissionPlan>(() => this.PlanMissionsTask(faction));
			yield return new WaitUntil(() => task.IsCompleted);
			if (task.IsFaulted)
			{
				Log.Warn(string.Format("AICouncilorMissionPlanner failed during PlanMissionsTask. Retrying on main thread.\nException: {0}", task.Exception), Array.Empty<object>());
				this.RunSelectedPlayerActions(this.PlanMissionsTask(faction));
			}
			else
			{
				this.RunSelectedPlayerActions(task.Result);
			}
			yield break;
		}

		// Token: 0x06005A09 RID: 23049 RVA: 0x00298284 File Offset: 0x00296484
		protected AICouncilorMissionPlan PlanMissionsTask(TIFactionState faction)
		{
			AICouncilorMissionPlanner.<>c__DisplayClass98_0 CS$<>8__locals1 = new AICouncilorMissionPlanner.<>c__DisplayClass98_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.<>4__this = this;
			this.nationModifyingGoals = GameStateManager.AllExtantNations().ToDictionary<TINationState, TINationState, List<TIFactionGoalState>>((TINationState x) => x, (TINationState x) => CS$<>8__locals1.faction.FindGoals(TIFactionGoalState.NationMissionModifyingGoals, CS$<>8__locals1.faction, x, TIFactionState.GoalFilter.none, true));
			this.nationModifyingGoalsByFaction = GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, List<TIFactionGoalState>>((TIFactionState x) => x, (TIFactionState x) => CS$<>8__locals1.faction.FindGoals(TIFactionGoalState.NationMissionModifyingGoals, CS$<>8__locals1.faction, x, TIFactionState.GoalFilter.none, true));
			this.factionMissionModifyingGoals = GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, List<TIFactionGoalState>>((TIFactionState x) => x, (TIFactionState x) => CS$<>8__locals1.faction.FindGoals(TIFactionGoalState.FactionMissionModifyingGoals, CS$<>8__locals1.faction, x, TIFactionState.GoalFilter.none, true));
			this.SetPayoffValues(CS$<>8__locals1.faction, true);
			if (CS$<>8__locals1.faction.IsAlienFaction)
			{
				AICouncilorMissionPlanner.campaignDuration_years = TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact();
			}
			else
			{
				AICouncilorMissionPlanner.campaignDuration_years = TITimeState.CampaignDuration_years_Exact();
			}
			this.warFactions = (from x in GameStateManager.AllFactions()
				where x != CS$<>8__locals1.faction && CS$<>8__locals1.faction.FindGoals(GoalType.WarOnFaction, CS$<>8__locals1.faction, x, TIFactionState.GoalFilter.none, true).Count > 0
				select x).ToList<TIFactionState>();
			this.factionDesiredMilestones = CS$<>8__locals1.faction.DesiredMilestones();
			this.availableResources = Enums.FactionResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource x) => CS$<>8__locals1.faction.GetCurrentResourceAmount(x));
			this.availableResources[FactionResource.Money] = (float)((int)(0.5f * (CS$<>8__locals1.faction.GetCurrentResourceAmount(FactionResource.Money) - CS$<>8__locals1.faction.AISavingTarget.GetBankedQuantity(FactionResource.Money))));
			if (CS$<>8__locals1.faction.GetMonthlyIncome(FactionResource.Money, false, false) < 0f)
			{
				Dictionary<FactionResource, float> dictionary = this.availableResources;
				dictionary[FactionResource.Money] = dictionary[FactionResource.Money] + CS$<>8__locals1.faction.GetMonthlyIncome(FactionResource.Money, false, false);
			}
			this.availableResources[FactionResource.Influence] = (float)((int)(CS$<>8__locals1.faction.GetCurrentResourceAmount(FactionResource.Influence) - CS$<>8__locals1.faction.AISavingTarget.GetBankedQuantity(FactionResource.Influence)));
			if (CS$<>8__locals1.faction.IsAlienProxy && CS$<>8__locals1.faction.knowsWinCondition)
			{
				Dictionary<FactionResource, float> dictionary = this.availableResources;
				dictionary[FactionResource.Influence] = dictionary[FactionResource.Influence] - (float)((int)TIFactionState.grantNationMission.cost.value);
			}
			if (CS$<>8__locals1.faction.IsActiveHumanFaction && CS$<>8__locals1.faction.numActiveCouncilors < CS$<>8__locals1.faction.maxCouncilSize && AICouncilorMissionPlanner.campaignDuration_years > 0.15f)
			{
				Dictionary<FactionResource, float> dictionary = this.availableResources;
				dictionary[FactionResource.Influence] = dictionary[FactionResource.Influence] - 60f;
			}
			if (CS$<>8__locals1.faction.IsActiveHumanFaction && CS$<>8__locals1.faction.GetDailyIncome(FactionResource.Influence, false, false) < 0f && this.availableResources[FactionResource.Influence] < 500f)
			{
				this.availableResources[FactionResource.Influence] = 0f;
			}
			this.availableResources[FactionResource.Operations] = (float)((int)(CS$<>8__locals1.faction.GetCurrentResourceAmount(FactionResource.Operations) - CS$<>8__locals1.faction.AISavingTarget.GetBankedQuantity(FactionResource.Operations)));
			CS$<>8__locals1.requiredMissions = CS$<>8__locals1.faction.RequiredMissions(true);
			CS$<>8__locals1.missingRequiredMissions = CS$<>8__locals1.faction.MissingRequiredMissions(CS$<>8__locals1.requiredMissions);
			CS$<>8__locals1.availableCouncilors = CS$<>8__locals1.faction.activeCouncilors;
			CS$<>8__locals1.selectedMissions = new List<AIMissionEntry>();
			CS$<>8__locals1.possibleMissionDictionary = new Dictionary<TICouncilorState, List<TIMissionTemplate>>();
			foreach (TICouncilorState ticouncilorState in CS$<>8__locals1.availableCouncilors)
			{
				CS$<>8__locals1.possibleMissionDictionary.Add(ticouncilorState, ticouncilorState.GetPossibleMissionList(true, false, true, null, false));
			}
			if (!CS$<>8__locals1.faction.IsAlienFaction)
			{
				this.recentAlienSite = CS$<>8__locals1.faction.MostRecentAlienSite(true).ref_region;
				this.timeSinceAlienSite_days = CS$<>8__locals1.faction.MostRecentAlienSiteAge_days(true);
				ValueTuple<TINationState, TIDateTime> valueTuple = CS$<>8__locals1.faction.AlienControlPointGiftHistory.FirstOrDefault<ValueTuple<TINationState, TIDateTime>>();
				this.recentAlienControlPointGift = valueTuple.Item1;
				this.timeSinceAlienControlPointGift_days = ((this.recentAlienControlPointGift != null) ? ((float)(TITimeState.Now() - valueTuple.Item2).TotalDays) : (-1f));
				foreach (TIObjectiveTemplate tiobjectiveTemplate in CS$<>8__locals1.faction.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked))
				{
					Dictionary<AIForcedMissionEntry, float> dictionary2 = new Dictionary<AIForcedMissionEntry, float>();
					TIMissionTemplate mission4 = tiobjectiveTemplate.targetMissionTemplate;
					ObjectiveMissionTargetType targetMissionTarget = tiobjectiveTemplate.targetMissionTarget;
					if (mission4 != null && mission4.CanAfford(CS$<>8__locals1.faction, null))
					{
						new List<TIGameState>();
						foreach (TICouncilorState ticouncilorState2 in CS$<>8__locals1.availableCouncilors)
						{
							if (!ticouncilorState2.HasMission && CS$<>8__locals1.possibleMissionDictionary[ticouncilorState2].Contains(mission4))
							{
								using (List<TIGameState>.Enumerator enumerator3 = tiobjectiveTemplate.ValidObjectiveTargets(mission4.GetValidTargets(ticouncilorState2).ToList<TIGameState>(), CS$<>8__locals1.faction).GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										TIGameState target7 = enumerator3.Current;
										AIForcedMissionEntry aiforcedMissionEntry = new AIForcedMissionEntry
										{
											councilor = ticouncilorState2,
											mission = mission4,
											target = target7
										};
										if (!dictionary2.Keys.Any<AIForcedMissionEntry>((AIForcedMissionEntry x) => x.mission == mission4 && x.target == target7))
										{
											dictionary2.Add(aiforcedMissionEntry, mission4.resolutionMethod.GetSuccessChance(mission4, ticouncilorState2, target7, 0f, false));
										}
									}
								}
							}
						}
						if (dictionary2.Count > 0)
						{
							AIForcedMissionEntry key = dictionary2.Aggregate<KeyValuePair<AIForcedMissionEntry, float>>(delegate(KeyValuePair<AIForcedMissionEntry, float> l, KeyValuePair<AIForcedMissionEntry, float> r)
							{
								if (l.Value <= r.Value)
								{
									return r;
								}
								return l;
							}).Key;
							float num = 1E+09f * dictionary2[key];
							int num2 = key.councilor.CurrentMaxSliderSteps(mission4, 1f);
							TIMissionCost cost = key.mission.cost;
							float num3 = ((cost != null) ? cost.GetCost((float)num2, key.councilor, null) : 0f);
							AIMissionEntry aimissionEntry = new AIMissionEntry
							{
								councilor = key.councilor,
								mission = key.mission,
								target = key.target,
								sliderSteps = num2,
								payoff = 1E+09f,
								expectedUtility = num,
								successChanceHigh = mission4.resolutionMethod.GetSuccessChance(mission4, key.councilor, key.target, num3, false),
								successChanceLow = dictionary2[key]
							};
							this.SelectMission(aimissionEntry, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
						}
					}
				}
			}
			TIMissionTemplate goToGroundMission = TIFactionState.goToGroundMission;
			foreach (TICouncilorState ticouncilorState3 in CS$<>8__locals1.availableCouncilors.ToList<TICouncilorState>())
			{
				if (ticouncilorState3.imBeingTargeted && CS$<>8__locals1.possibleMissionDictionary[ticouncilorState3].Contains(goToGroundMission))
				{
					List<TIRegionState> list = (from x in goToGroundMission.GetValidTargets(ticouncilorState3)
						select x.ref_region).ToList<TIRegionState>();
					if (list.Count > 0)
					{
						IEnumerable<TIRegionState> enumerable = list;
						Func<TIRegionState, bool> func;
						if ((func = CS$<>8__locals1.<>9__32) == null)
						{
							func = (CS$<>8__locals1.<>9__32 = (TIRegionState x) => x.nation.executiveFaction == CS$<>8__locals1.faction);
						}
						List<TIRegionState> list2 = enumerable.Where<TIRegionState>(func).ToList<TIRegionState>();
						TIRegionState tiregionState;
						if (list2.Count > 0)
						{
							tiregionState = list2.SelectRandomWeightedItem<TIRegionState>((TIRegionState x) => 1f / x.population, -1f, 1E-37f);
						}
						else
						{
							tiregionState = list.SelectRandomWeightedItem<TIRegionState>((TIRegionState x) => 1f / x.population, -1f, 1E-37f);
						}
						AIMissionEntry aimissionEntry2 = new AIMissionEntry
						{
							councilor = ticouncilorState3,
							mission = goToGroundMission,
							target = tiregionState,
							payoff = 1000000f,
							expectedUtility = 1000000f,
							successChanceHigh = 1f,
							successChanceLow = 1f
						};
						this.SelectMission(aimissionEntry2, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
					}
				}
			}
			float num4 = TITimeState.CampaignDuration_years_Exact() * 365.2422f;
			float num5 = 0f;
			if (CS$<>8__locals1.faction.councilors.Count > 0)
			{
				num5 = CS$<>8__locals1.faction.councilors.Max<TICouncilorState>(delegate(TICouncilorState x)
				{
					TIDateTime tidateTime2;
					if (CS$<>8__locals1.faction.lastTimeSecretsWereSeen.TryGetValue(x, out tidateTime2))
					{
						return (float)(TITimeState.Now() - tidateTime2).TotalDays;
					}
					return float.PositiveInfinity;
				});
			}
			float num6 = 1080f;
			if (CS$<>8__locals1.faction.IsAlienProxy)
			{
				num6 = 360f;
			}
			if (num4 > num6 && num5 > num6)
			{
				CS$<>8__locals1.faction.crazyIvan = true;
			}
			if (CS$<>8__locals1.faction.crazyIvan && CS$<>8__locals1.availableCouncilors.Count > 0 && CS$<>8__locals1.faction.councilors.Count >= 2)
			{
				TIMissionTemplate investigateMission = TIFactionState.investigateMission;
				TIMissionTemplate inspireMission = TIFactionState.inspireMission;
				Dictionary<TICouncilorState, ValueTuple<int, int, float, float>> dictionary3 = CS$<>8__locals1.faction.councilors.Where<TICouncilorState>((TICouncilorState x) => CS$<>8__locals1.faction.GetIntel(x) < TemplateManager.global.intelToSeeCouncilorSecrets).ToDictionary<TICouncilorState, TICouncilorState, ValueTuple<int, int, float, float>>((TICouncilorState councilor) => councilor4, delegate(TICouncilorState councilor)
				{
					int num36;
					TIDateTime tidateTime3;
					ValueTuple<int, int, float, float> valueTuple2 = new ValueTuple<int, int, float, float>(councilor4.GetAttribute(CouncilorAttribute.ApparentLoyalty, true, true, true, false, false, false), CS$<>8__locals1.faction.lastRecordedLoyalty.TryGetValue(councilor4, out num36) ? num36 : (-1), CS$<>8__locals1.faction.lastTimeSecretsWereSeen.TryGetValue(councilor4, out tidateTime3) ? ((float)(TITimeState.Now() - tidateTime3).TotalDays) : (-1f), 0f);
					valueTuple2.Item4 = Mathf.Clamp((valueTuple2.Item3 >= 0f) ? valueTuple2.Item3 : float.PositiveInfinity, 0f, 1800f) / (float)(((valueTuple2.Item2 >= 0) ? valueTuple2.Item2 : valueTuple2.Item1) + 3);
					return valueTuple2;
				});
				if (dictionary3.Count > 0)
				{
					foreach (TICouncilorState ticouncilorState4 in CS$<>8__locals1.availableCouncilors.OrderByDescending<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false)).ToList<TICouncilorState>())
					{
						if (CS$<>8__locals1.possibleMissionDictionary[ticouncilorState4].Contains(investigateMission))
						{
							IList<TIGameState> possibleTargets2 = investigateMission.GetValidTargets(ticouncilorState4);
							if (possibleTargets2.Count > 0)
							{
								List<KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>>> list3 = dictionary3.Where<KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>>>(([TupleElementNames(new string[] { "ApparentLoyalty", "LastRecordedLoyalty", "IntelAge_days", "Score" })] KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>> x) => possibleTargets2.Contains(x.Key)).ToList<KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>>>();
								if (list3.Count > 0)
								{
									TICouncilorState key2 = list3.MaxBy<KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>>, float>(([TupleElementNames(new string[] { "ApparentLoyalty", "LastRecordedLoyalty", "IntelAge_days", "Score" })] KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>> x) => x.Value.Item4).Key;
									if (key2 != null)
									{
										int num7 = Mathf.Min(ticouncilorState4.CurrentMaxSliderSteps(investigateMission, 1f), 2);
										float successChance = investigateMission.resolutionMethod.GetSuccessChance(investigateMission, ticouncilorState4, key2, (float)num7, false);
										float num8 = 0.35f;
										if (CS$<>8__locals1.faction.IsAlienProxy)
										{
											num8 -= 0.1f;
											float num9 = num6 * 1.5f;
											TIDateTime tidateTime;
											if (num4 > num9 && (!CS$<>8__locals1.faction.lastTimeSecretsWereSeen.TryGetValue(key2, out tidateTime) || (TITimeState.Now() - tidateTime).TotalDays > (double)num9))
											{
												num8 -= 0.1f;
											}
										}
										if (successChance >= num8)
										{
											AIMissionEntry aimissionEntry3 = new AIMissionEntry
											{
												councilor = ticouncilorState4,
												mission = investigateMission,
												target = key2,
												payoff = 1000000f,
												expectedUtility = 1000000f
											};
											this.SelectMission(aimissionEntry3, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
											dictionary3.Remove(key2);
										}
									}
								}
							}
						}
						else if (CS$<>8__locals1.possibleMissionDictionary[ticouncilorState4].Contains(inspireMission))
						{
							IList<TIGameState> possibleTargets = inspireMission.GetValidTargets(ticouncilorState4);
							if (possibleTargets.Count > 0)
							{
								List<KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>>> list4 = dictionary3.Where<KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>>>(([TupleElementNames(new string[] { "ApparentLoyalty", "LastRecordedLoyalty", "IntelAge_days", "Score" })] KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>> x) => possibleTargets.Contains(x.Key)).ToList<KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>>>();
								if (list4.Count > 0)
								{
									TICouncilorState key3 = list4.MaxBy<KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>>, float>(([TupleElementNames(new string[] { "ApparentLoyalty", "LastRecordedLoyalty", "IntelAge_days", "Score" })] KeyValuePair<TICouncilorState, ValueTuple<int, int, float, float>> x) => x.Value.Item4).Key;
									if (key3 != null)
									{
										int num10 = Mathf.Min(ticouncilorState4.CurrentMaxSliderSteps(inspireMission, 1f), 2);
										if (inspireMission.resolutionMethod.GetSuccessChance(inspireMission, ticouncilorState4, key3, (float)num10, false) > 0.4f)
										{
											AIMissionEntry aimissionEntry4 = new AIMissionEntry
											{
												councilor = ticouncilorState4,
												mission = inspireMission,
												target = key3,
												payoff = 1000000f,
												expectedUtility = 1000000f
											};
											this.SelectMission(aimissionEntry4, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
										}
									}
								}
							}
						}
					}
				}
			}
			new List<PolicyOptionWithTarget>();
			bool flag = false;
			if (!CS$<>8__locals1.faction.IsAlienFaction)
			{
				IEnumerable<TIObjectiveTemplate> objectivesByTypeAndStatus = CS$<>8__locals1.faction.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked);
				this.huntAbility = TIEffectsState.SumEffectsModifiers(Context.DetectAlienActivity, CS$<>8__locals1.faction, 0f, null);
				bool flag2 = false;
				bool flag3 = TITimeState.CampaignDuration_years_Exact() > 5f;
				IEnumerable<TIObjectiveTemplate> enumerable2 = objectivesByTypeAndStatus;
				bool flag4 = flag3;
				if (TITimeState.CampaignDuration_months_Exact() >= 2f && CS$<>8__locals1.faction.IsAlienProxy && TIGlobalValuesState.IsQuietAlienCampaign())
				{
					flag4 = true;
				}
				if (flag4)
				{
					enumerable2 = enumerable2.Concat<TIObjectiveTemplate>(enumerable2);
				}
				using (IEnumerator<TIObjectiveTemplate> enumerator4 = enumerable2.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						TIObjectiveTemplate tiobjectiveTemplate2 = enumerator4.Current;
						AICouncilorMissionPlanner.<>c__DisplayClass98_5 CS$<>8__locals6 = new AICouncilorMissionPlanner.<>c__DisplayClass98_5();
						CS$<>8__locals6.CS$<>8__locals2 = CS$<>8__locals1;
						CS$<>8__locals6.objectiveMission = tiobjectiveTemplate2.targetMissionTemplate;
						bool flag5 = false;
						this.huntingForAlienActivity = false;
						ObjectiveMissionTargetType targetMissionTarget2 = tiobjectiveTemplate2.targetMissionTarget;
						switch (targetMissionTarget2)
						{
						case ObjectiveMissionTargetType.Abductions:
							if (CS$<>8__locals6.CS$<>8__locals2.faction.KnownAbductions.Count == 0)
							{
								this.huntingForAlienActivity = true;
							}
							break;
						case ObjectiveMissionTargetType.Xenoforming:
							if (CS$<>8__locals6.CS$<>8__locals2.faction.KnownXenoformMissions.Count == 0)
							{
								this.huntingForAlienActivity = true;
							}
							break;
						case ObjectiveMissionTargetType.EnthrallMission:
							if (CS$<>8__locals6.CS$<>8__locals2.faction.KnownEnthralls.Count == 0)
							{
								this.huntingForAlienActivity = true;
							}
							break;
						default:
							if (targetMissionTarget2 == ObjectiveMissionTargetType.HydraCouncilor)
							{
								if (CS$<>8__locals6.CS$<>8__locals2.faction.CurrentKnownCouncilors(true, new List<TIFactionState> { GameStateManager.AlienFaction() }, false, false).Count == 0)
								{
									this.huntingForAlienActivity = true;
									flag5 = true;
								}
							}
							break;
						}
						switch (tiobjectiveTemplate2.targetMilestone)
						{
						case CampaignMilestone.AccessHydraCorpus:
							if (!CS$<>8__locals6.CS$<>8__locals2.faction.shouldNeverAttackAliens)
							{
								this.huntingForAlienActivity = true;
								CS$<>8__locals6.objectiveMission = TIFactionState.assassinateMission;
								flag5 = true;
							}
							break;
						case CampaignMilestone.AccessLiveHydra:
							if (!CS$<>8__locals6.CS$<>8__locals2.faction.shouldNeverAttackAliens)
							{
								this.huntingForAlienActivity = true;
								CS$<>8__locals6.objectiveMission = TIFactionState.detainMission;
								flag5 = true;
							}
							break;
						case CampaignMilestone.AccessAlienTech:
							if (!CS$<>8__locals6.CS$<>8__locals2.faction.shouldNeverAttackAliens)
							{
								if (GameStateManager.AlienNation().extant)
								{
									this.huntingForAlienActivity = true;
								}
								CS$<>8__locals6.objectiveMission = TIFactionState.assaultAlienAssetMission;
							}
							break;
						case CampaignMilestone.AccessAlienShip:
							if (!CS$<>8__locals6.CS$<>8__locals2.faction.shouldNeverAttackAliens)
							{
								CS$<>8__locals6.objectiveMission = TIFactionState.assaultAlienAssetMission;
							}
							break;
						}
						Dictionary<AIForcedMissionEntry, float> dictionary4 = new Dictionary<AIForcedMissionEntry, float>();
						if (CS$<>8__locals6.objectiveMission != null && CS$<>8__locals6.objectiveMission.CanAfford(CS$<>8__locals6.CS$<>8__locals2.faction, null))
						{
							AICouncilorMissionPlanner.<>c__DisplayClass98_6 CS$<>8__locals7;
							CS$<>8__locals7.councilor = null;
							CS$<>8__locals7.targetListFiltered = false;
							List<TICouncilorState> list5 = CS$<>8__locals6.CS$<>8__locals2.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => !x.HasMission && CS$<>8__locals6.CS$<>8__locals2.possibleMissionDictionary[x].Contains(CS$<>8__locals6.objectiveMission)).ToList<TICouncilorState>();
							CS$<>8__locals7.targetList = new List<TIGameState>();
							if (list5.Count > 0)
							{
								if (CS$<>8__locals6.objectiveMission.ContestedMission)
								{
									CS$<>8__locals7.councilor = list5.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CS$<>8__locals6.objectiveMission.primaryAttackerStat, true, true, true, false, false, false));
								}
								else
								{
									CS$<>8__locals7.councilor = list5.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false));
								}
								CS$<>8__locals7.targetList = tiobjectiveTemplate2.ValidObjectiveTargets(CS$<>8__locals6.objectiveMission.GetValidTargets(CS$<>8__locals7.councilor).ToList<TIGameState>(), CS$<>8__locals6.CS$<>8__locals2.faction);
								CS$<>8__locals7.targetListFiltered = true;
							}
							if (CS$<>8__locals7.targetList.Count == 0)
							{
								if (flag5)
								{
									CS$<>8__locals6.objectiveMission = TIFactionState.investigateMission;
									list5 = CS$<>8__locals6.CS$<>8__locals2.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => !x.HasMission && CS$<>8__locals6.CS$<>8__locals2.possibleMissionDictionary[x].Contains(CS$<>8__locals6.objectiveMission)).ToList<TICouncilorState>();
									if (list5.Count > 0 && CS$<>8__locals7.targetList.Count == 0)
									{
										CS$<>8__locals7.targetList = this.GetHuntListForAliens(CS$<>8__locals6.CS$<>8__locals2.faction, list5, CS$<>8__locals6.objectiveMission, flag3, new List<TICouncilorState>(), this.recentAlienControlPointGift, this.timeSinceAlienControlPointGift_days).ToList<TIGameState>();
										CS$<>8__locals7.targetList = CS$<>8__locals7.targetList.Where<TIGameState>((TIGameState x) => CS$<>8__locals6.CS$<>8__locals2.selectedMissions.None<AIMissionEntry>((AIMissionEntry y) => y.mission.GetType() == CS$<>8__locals6.objectiveMission.GetType() && y.target == x)).ToList<TIGameState>();
										CS$<>8__locals7.targetListFiltered = false;
										list5 = CS$<>8__locals6.CS$<>8__locals2.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => !x.HasMission && CS$<>8__locals6.CS$<>8__locals2.possibleMissionDictionary[x].Contains(CS$<>8__locals6.objectiveMission)).ToList<TICouncilorState>();
										CS$<>8__locals7.councilor = null;
									}
									if (CS$<>8__locals7.targetList.Count == 0)
									{
										list5 = CS$<>8__locals6.CS$<>8__locals2.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => !x.HasMission && CS$<>8__locals6.CS$<>8__locals2.possibleMissionDictionary[x].Contains(CS$<>8__locals6.objectiveMission)).ToList<TICouncilorState>();
										CS$<>8__locals6.<PlanMissionsTask>g__TrySurveil|44(list5, ref CS$<>8__locals7);
									}
								}
								else if (this.huntingForAlienActivity && ((CS$<>8__locals6.CS$<>8__locals2.availableCouncilors.Count >= 4 && !flag2) || flag3 || (CS$<>8__locals6.CS$<>8__locals2.availableCouncilors.Count >= 2 && flag4)))
								{
									CS$<>8__locals6.objectiveMission = TIFactionState.surveilMission;
									list5 = CS$<>8__locals6.CS$<>8__locals2.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => !x.HasMission && CS$<>8__locals6.CS$<>8__locals2.possibleMissionDictionary[x].Contains(CS$<>8__locals6.objectiveMission)).ToList<TICouncilorState>();
									if (list5.Count > 0)
									{
										CS$<>8__locals7.councilor = list5.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false));
										CS$<>8__locals6.<PlanMissionsTask>g__TrySurveil|44(list5, ref CS$<>8__locals7);
									}
								}
								if (CS$<>8__locals7.councilor == null && CS$<>8__locals7.targetList.Count > 0 && list5.Count > 0)
								{
									if (CS$<>8__locals6.objectiveMission.ContestedMission)
									{
										CS$<>8__locals7.councilor = list5.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CS$<>8__locals6.objectiveMission.primaryAttackerStat, true, true, true, false, false, false));
									}
									else
									{
										CS$<>8__locals7.councilor = list5.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false));
									}
								}
								if (CS$<>8__locals7.councilor != null && !CS$<>8__locals7.targetListFiltered)
								{
									CS$<>8__locals7.targetList = CS$<>8__locals6.objectiveMission.GetValidTargets(CS$<>8__locals7.councilor).Intersect<TIGameState>(CS$<>8__locals7.targetList).ToList<TIGameState>();
								}
							}
							if (CS$<>8__locals7.councilor != null)
							{
								foreach (TIGameState tigameState in CS$<>8__locals7.targetList)
								{
									AIForcedMissionEntry aiforcedMissionEntry2 = new AIForcedMissionEntry
									{
										councilor = CS$<>8__locals7.councilor,
										mission = CS$<>8__locals6.objectiveMission,
										target = tigameState
									};
									dictionary4.Add(aiforcedMissionEntry2, CS$<>8__locals6.objectiveMission.resolutionMethod.GetSuccessChance(CS$<>8__locals6.objectiveMission, CS$<>8__locals7.councilor, tigameState, 0f, false));
								}
							}
							if (dictionary4.Count > 0)
							{
								AICouncilorMissionPlanner.<>c__DisplayClass98_8 CS$<>8__locals8 = new AICouncilorMissionPlanner.<>c__DisplayClass98_8();
								CS$<>8__locals8.CS$<>8__locals4 = CS$<>8__locals6;
								CS$<>8__locals8.winner = dictionary4.Aggregate<KeyValuePair<AIForcedMissionEntry, float>>(delegate(KeyValuePair<AIForcedMissionEntry, float> l, KeyValuePair<AIForcedMissionEntry, float> r)
								{
									if (l.Value <= r.Value)
									{
										return r;
									}
									return l;
								}).Key;
								int num11 = CS$<>8__locals8.winner.councilor.CurrentMaxSliderSteps(CS$<>8__locals8.winner.mission, 1f);
								TIMissionCost cost2 = CS$<>8__locals8.CS$<>8__locals4.objectiveMission.cost;
								float num12 = ((cost2 != null) ? cost2.GetCost((float)num11, null, null) : 0f);
								CS$<>8__locals8.successChanceHigh = CS$<>8__locals8.CS$<>8__locals4.objectiveMission.resolutionMethod.GetSuccessChance(CS$<>8__locals8.CS$<>8__locals4.objectiveMission, CS$<>8__locals8.winner.councilor, CS$<>8__locals8.winner.target, num12, false);
								bool flag6 = CS$<>8__locals8.CS$<>8__locals4.objectiveMission == TIFactionState.assaultAlienAssetMission || CS$<>8__locals8.CS$<>8__locals4.objectiveMission == TIFactionState.assassinateMission;
								float num13 = 0.1f;
								if (CS$<>8__locals8.successChanceHigh >= num13)
								{
									AIMissionEntry aimissionEntry5 = new AIMissionEntry
									{
										councilor = CS$<>8__locals8.winner.councilor,
										mission = CS$<>8__locals8.winner.mission,
										target = CS$<>8__locals8.winner.target,
										sliderSteps = num11,
										payoff = 100000000f,
										expectedUtility = 100000000f * CS$<>8__locals8.successChanceHigh,
										successChanceHigh = CS$<>8__locals8.successChanceHigh,
										successChanceLow = dictionary4[CS$<>8__locals8.winner],
										acceptableMinimumSuccess = num13,
										objective = true
									};
									this.SelectMission(aimissionEntry5, ref CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.selectedMissions, ref CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.availableCouncilors);
									if (CS$<>8__locals8.winner.mission == TIFactionState.surveilMission)
									{
										flag2 = true;
									}
									if (flag6)
									{
										TICouncilorState ticouncilorState5 = CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, true, null, false).Contains(TIFactionState.protectMission)).MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Security, true, true, true, false, false, false));
										if (ticouncilorState5 != null)
										{
											CS$<>8__locals8.<PlanMissionsTask>g__SelectSupportMission|68(TIFactionState.protectMission, ticouncilorState5, CS$<>8__locals7.councilor);
										}
									}
									if (CS$<>8__locals8.CS$<>8__locals4.objectiveMission != TIFactionState.assassinateMission && CS$<>8__locals8.CS$<>8__locals4.objectiveMission != TIFactionState.detainMission)
									{
										if (!CS$<>8__locals8.CS$<>8__locals4.objectiveMission.resolutionMethod.attackingModifiers.Any<TIMissionModifier>((TIMissionModifier x) => x is TIMissionModifier_IntelonDefendingCouncilor))
										{
											if (!flag3)
											{
												continue;
											}
											if (CS$<>8__locals8.CS$<>8__locals4.objectiveMission == TIFactionState.investigateMission)
											{
												using (List<TIGameState>.Enumerator enumerator3 = CS$<>8__locals7.targetList.ToList<TIGameState>().GetEnumerator())
												{
													while (enumerator3.MoveNext())
													{
														TIGameState enemyCouncilor = enumerator3.Current;
														if (CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.selectedMissions.Any<AIMissionEntry>((AIMissionEntry x) => x.target == enemyCouncilor))
														{
															CS$<>8__locals7.targetList.Remove(enemyCouncilor);
														}
													}
												}
												if (CS$<>8__locals7.targetList.Count > 0)
												{
													using (List<TICouncilorState>.Enumerator enumerator = CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.availableCouncilors.ToList<TICouncilorState>().GetEnumerator())
													{
														while (enumerator.MoveNext())
														{
															TICouncilorState availableCouncilor = enumerator.Current;
															if (availableCouncilor.GetPossibleMissionList(false, false, true, null, false).Contains(TIFactionState.investigateMission) && TIUtilities.RandomFloatValue() < 0.5f)
															{
																IEnumerable<TIGameState> enumerable3 = CS$<>8__locals7.targetList.Where<TIGameState>((TIGameState x) => TIFactionState.investigateMission.GetValidTargets(availableCouncilor).Contains(x));
																if (enumerable3.Count<TIGameState>() > 0)
																{
																	TIGameState tigameState2 = enumerable3.SelectRandomItem<TIGameState>();
																	CS$<>8__locals8.<PlanMissionsTask>g__SelectSupportMission|68(TIFactionState.investigateMission, availableCouncilor, tigameState2);
																	CS$<>8__locals7.targetList.Remove(tigameState2);
																}
																if (CS$<>8__locals7.targetList.Count == 0)
																{
																	break;
																}
															}
														}
													}
												}
											}
											if (CS$<>8__locals8.CS$<>8__locals4.objectiveMission == TIFactionState.surveilMission || CS$<>8__locals8.CS$<>8__locals4.objectiveMission == TIFactionState.investigateMission)
											{
												using (List<TICouncilorState>.Enumerator enumerator = CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.availableCouncilors.ToList<TICouncilorState>().GetEnumerator())
												{
													while (enumerator.MoveNext())
													{
														TICouncilorState ticouncilorState6 = enumerator.Current;
														if (ticouncilorState6.GetPossibleMissionList(false, false, true, null, false).Contains(TIFactionState.surveilMission) && TIUtilities.RandomFloatValue() < 0.5f)
														{
															List<TIRegionState> list6 = (from x in TIFactionState.surveilMission.GetValidTargets(ticouncilorState6).ToList<TIGameState>()
																where x.isRegionState
																select x).ToList<TIGameState>().ConvertAll<TIRegionState>((TIGameState x) => x.ref_region);
															using (List<TIRegionState>.Enumerator enumerator5 = list6.ToList<TIRegionState>().GetEnumerator())
															{
																while (enumerator5.MoveNext())
																{
																	TIRegionState region = enumerator5.Current;
																	if (CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.selectedMissions.Any<AIMissionEntry>((AIMissionEntry x) => x.target == region))
																	{
																		list6.Remove(region);
																	}
																}
															}
															if (list6.Count > 0)
															{
																CS$<>8__locals8.<PlanMissionsTask>g__SelectSupportMission|68(TIFactionState.surveilMission, ticouncilorState6, this.BestRegionForAlienSearch(CS$<>8__locals7.councilor, list6));
															}
														}
													}
													continue;
												}
												goto IL_1A3C;
											}
											continue;
										}
									}
									TICouncilorState ticouncilorState7 = CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, true, null, false).Contains(TIFactionState.investigateMission) && TIFactionState.investigateMission.GetValidTargets(x).Contains(CS$<>8__locals8.winner.target)).MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false));
									if (ticouncilorState7 != null)
									{
										CS$<>8__locals8.<PlanMissionsTask>g__SelectSupportMission|68(TIFactionState.investigateMission, ticouncilorState7, CS$<>8__locals8.winner.target);
										continue;
									}
									continue;
								}
								IL_1A3C:
								if (CS$<>8__locals8.CS$<>8__locals4.objectiveMission == TIFactionState.assassinateMission || CS$<>8__locals8.CS$<>8__locals4.objectiveMission == TIFactionState.detainMission)
								{
									TIMissionTemplate chaseMission = TIFactionState.investigateMission;
									IEnumerable<TICouncilorState> enumerable4 = CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, true, null, false).Contains(chaseMission) && chaseMission.GetValidTargets(x).Contains(CS$<>8__locals8.winner.target));
									if (enumerable4.Any<TICouncilorState>())
									{
										TICouncilorState ticouncilorState8 = enumerable4.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false));
										TIGameState target6 = CS$<>8__locals8.winner.target;
										if (ticouncilorState8 != null)
										{
											int num14 = ticouncilorState8.CurrentMaxSliderSteps(chaseMission, 1f);
											float num15 = AICouncilorMissionPlanner.<PlanMissionsTask>g__GetSupportResourceCost|98_67(chaseMission, ticouncilorState8, target6, num14);
											float num16 = AICouncilorMissionPlanner.<PlanMissionsTask>g__GetSupportResourceCost|98_67(chaseMission, ticouncilorState8, target6, 0);
											AIMissionEntry aimissionEntry6 = new AIMissionEntry
											{
												councilor = ticouncilorState8,
												mission = TIFactionState.investigateMission,
												target = target6,
												sliderSteps = num14,
												payoff = 100000000f,
												expectedUtility = 100000000f * num15,
												successChanceHigh = num15,
												successChanceLow = num16,
												acceptableMinimumSuccess = num13,
												objective = true
											};
											this.SelectMission(aimissionEntry6, ref CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.selectedMissions, ref CS$<>8__locals8.CS$<>8__locals4.CS$<>8__locals2.availableCouncilors);
										}
									}
								}
							}
						}
					}
					goto IL_22FC;
				}
			}
			if (TIFactionState.setPolicyMission.CanAfford(CS$<>8__locals1.faction, null))
			{
				(from x in CS$<>8__locals1.faction.AllSetPolicyMissionOptionsWithTargets
					where x.actingNation.alienNation
					where x.policyType == PolicyType.TransferRegionsOption
					select x).ToList<PolicyOptionWithTarget>();
				flag = true;
			}
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			if (CS$<>8__locals1.availableCouncilors.Count > 0)
			{
				TIFactionState[] array = GameStateManager.AllHumanFactions();
				for (int i = 0; i < array.Length; i++)
				{
					using (List<TIObjectiveTemplate>.Enumerator enumerator2 = array[i].GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							switch (enumerator2.Current.targetMissionTarget)
							{
							case ObjectiveMissionTargetType.Abductions:
								flag7 = true;
								break;
							case ObjectiveMissionTargetType.Xenoforming:
								flag8 = true;
								break;
							case ObjectiveMissionTargetType.EnthrallMission:
								flag9 = true;
								break;
							}
						}
					}
					if (flag7 && flag8 && flag9)
					{
						break;
					}
				}
			}
			if (AIEvaluators.ShouldAliensXenoform())
			{
				if (!GameStateManager.AllRegions().Any<TIRegionState>((TIRegionState x) => x.xenoforming.Extant()))
				{
					flag8 = TIUtilities.RandomFloatValue() < 0.3f;
				}
			}
			else
			{
				flag8 = false;
			}
			int num17 = 0;
			if ((!flag || CS$<>8__locals1.availableCouncilors.Count >= 2 || TIUtilities.RandomFloatValue() < 0.5f) && (flag7 || flag8 || flag9))
			{
				List<AIForcedMissionEntry> list7 = new List<AIForcedMissionEntry>();
				foreach (int num18 in from x in Enumerable.Range(0, 2)
					orderby TIUtilities.RandomFloatValue()
					select x)
				{
					List<TIMissionTemplate> allowedMissions = new List<TIMissionTemplate>();
					if (num18 == 0 && flag7)
					{
						allowedMissions.Add(TIFactionState.abductionsMission);
					}
					else
					{
						if (flag8)
						{
							allowedMissions.Add(TIFactionState.xenoformMission);
						}
						if (flag9)
						{
							allowedMissions.Add(TIFactionState.enthrallPublicMission);
							allowedMissions.Add(TIFactionState.enthrallElitesMission);
							allowedMissions.Add(TIFactionState.enthrallNonAlignedElitesMission);
						}
					}
					if (allowedMissions != null)
					{
						List<TICouncilorState> list8 = new List<TICouncilorState>();
						Func<TIMissionTemplate, bool> <>9__85;
						foreach (TICouncilorState ticouncilorState9 in CS$<>8__locals1.availableCouncilors)
						{
							IEnumerable<TIMissionTemplate> enumerable5 = CS$<>8__locals1.possibleMissionDictionary[ticouncilorState9];
							Func<TIMissionTemplate, bool> func2;
							if ((func2 = <>9__85) == null)
							{
								func2 = (<>9__85 = (TIMissionTemplate x) => allowedMissions.Contains(x));
							}
							if (enumerable5.Any<TIMissionTemplate>(func2))
							{
								list8.Add(ticouncilorState9);
							}
						}
						new List<TIGameState>();
						Dictionary<AIForcedMissionEntry, float> alienForcedMissionDictionary = new Dictionary<AIForcedMissionEntry, float>();
						foreach (TICouncilorState ticouncilorState10 in list8)
						{
							using (List<TIMissionTemplate>.Enumerator enumerator7 = allowedMissions.GetEnumerator())
							{
								while (enumerator7.MoveNext())
								{
									TIMissionTemplate mission5 = enumerator7.Current;
									using (List<TIGameState>.Enumerator enumerator3 = mission5.GetValidTargets(ticouncilorState10).ToList<TIGameState>().GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											TIGameState target8 = enumerator3.Current;
											float successChance2 = mission5.resolutionMethod.GetSuccessChance(mission5, ticouncilorState10, target8, 0f, false);
											if (successChance2 > 0.25f)
											{
												float num19 = this.GetPayoffForMissionTarget(CS$<>8__locals1.faction, mission5, ticouncilorState10, target8, CS$<>8__locals1.requiredMissions, CS$<>8__locals1.missingRequiredMissions, CS$<>8__locals1.faction.focusGoal, this.factionDesiredMilestones, AICouncilorMissionPlanner.campaignDuration_years, false, this.huntAbility, this.warFactions, this.recentAlienSite, this.timeSinceAlienSite_days, false);
												if (num19 > 0f)
												{
													AIForcedMissionEntry aiforcedMissionEntry3 = new AIForcedMissionEntry
													{
														councilor = ticouncilorState10,
														target = target8,
														mission = mission5
													};
													if (list7.Any<AIForcedMissionEntry>((AIForcedMissionEntry x) => x.mission == mission5 && x.target == target8))
													{
														num19 *= 0.01f;
													}
													alienForcedMissionDictionary.Add(aiforcedMissionEntry3, num19 * successChance2);
												}
											}
										}
									}
								}
							}
						}
						if (alienForcedMissionDictionary.Count > 0)
						{
							AIForcedMissionEntry aiforcedMissionEntry4 = alienForcedMissionDictionary.Keys.MaxBy<AIForcedMissionEntry, float>((AIForcedMissionEntry x) => alienForcedMissionDictionary[x]);
							list7.Add(aiforcedMissionEntry4);
							int num20 = aiforcedMissionEntry4.councilor.CurrentMaxSliderSteps(aiforcedMissionEntry4.mission, 1f);
							float successChance3 = aiforcedMissionEntry4.mission.resolutionMethod.GetSuccessChance(aiforcedMissionEntry4.mission, aiforcedMissionEntry4.councilor, aiforcedMissionEntry4.target, 0f, false);
							TIMissionResolution resolutionMethod = aiforcedMissionEntry4.mission.resolutionMethod;
							TIMissionTemplate mission3 = aiforcedMissionEntry4.mission;
							TICouncilorState councilor3 = aiforcedMissionEntry4.councilor;
							TIGameState target2 = aiforcedMissionEntry4.target;
							TIMissionCost cost3 = aiforcedMissionEntry4.mission.cost;
							float successChance4 = resolutionMethod.GetSuccessChance(mission3, councilor3, target2, (cost3 != null) ? cost3.GetCost((float)num20, aiforcedMissionEntry4.councilor, null) : 0f, false);
							AIMissionEntry aimissionEntry7 = new AIMissionEntry
							{
								councilor = aiforcedMissionEntry4.councilor,
								mission = aiforcedMissionEntry4.mission,
								target = aiforcedMissionEntry4.target,
								sliderSteps = num20,
								payoff = alienForcedMissionDictionary[aiforcedMissionEntry4] * successChance3,
								expectedUtility = alienForcedMissionDictionary[aiforcedMissionEntry4] * ((successChance3 + successChance4) / 2f),
								successChanceHigh = successChance4,
								successChanceLow = successChance3
							};
							this.SelectMission(aimissionEntry7, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
							if (aiforcedMissionEntry4.mission.dataName == TIFactionState.abductionsMission.dataName)
							{
								flag7 = false;
							}
							else if (aiforcedMissionEntry4.mission.dataName == TIFactionState.xenoformMission.dataName)
							{
								flag8 = false;
							}
							else if (aiforcedMissionEntry4.mission.dataName == TIFactionState.enthrallElitesMission.dataName || aiforcedMissionEntry4.mission.dataName == TIFactionState.enthrallNonAlignedElitesMission.dataName || aiforcedMissionEntry4.mission.dataName == TIFactionState.enthrallElitesMission.dataName)
							{
								flag9 = false;
							}
							num17++;
							if (num17 > 0)
							{
								if (CS$<>8__locals1.availableCouncilors.Count<TICouncilorState>((TICouncilorState x) => x.OnEarth) <= 2)
								{
									break;
								}
							}
						}
					}
				}
			}
			IL_22FC:
			CS$<>8__locals1.capturingNeutralNations = AIDailyFactionPlanner.AI_ControllingNeutralPowers(CS$<>8__locals1.faction);
			CS$<>8__locals1.alienNation = GameStateManager.AlienNation();
			if (GameStateManager.AlienProxy().CanCountAbductions && (CS$<>8__locals1.faction.IsAlienFaction || CS$<>8__locals1.faction.IsAlienProxy))
			{
				if (CS$<>8__locals1.faction.IsAlienProxy)
				{
					KeyValuePair<TICouncilorState, IList<TIGameState>> keyValuePair = (from x in CS$<>8__locals1.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, false, null, false).Contains(TIFactionState.grantNationMission)).ToDictionary<TICouncilorState, TICouncilorState, IList<TIGameState>>((TICouncilorState x) => x, (TICouncilorState x) => TIFactionState.grantNationMission.GetValidTargets(x))
						where x.Value.Count > 0
						select x).MaxBy<KeyValuePair<TICouncilorState, IList<TIGameState>>, int>((KeyValuePair<TICouncilorState, IList<TIGameState>> x) => x.Key.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false));
					TICouncilorState key4 = keyValuePair.Key;
					if (key4 != null)
					{
						TIGameState tigameState3 = keyValuePair.Value.MaxBy<TIGameState, float>((TIGameState x) => AICouncilorMissionPlanner.<PlanMissionsTask>g__ScoreNationForAlienAquisition|98_89(x.ref_nation));
						AIMissionEntry aimissionEntry8 = new AIMissionEntry(this, TIFactionState.grantNationMission, key4, tigameState3, CS$<>8__locals1.faction.currentRiskAversion, CS$<>8__locals1.requiredMissions, CS$<>8__locals1.missingRequiredMissions, false, AICouncilorMissionPlanner.campaignDuration_years, this.factionDesiredMilestones, this.huntingForAlienActivity, this.huntAbility, this.warFactions, this.recentAlienSite, this.timeSinceAlienSite_days, TIFactionState.buildFacilityMission.hasCost ? this.availableResources[TIFactionState.buildFacilityMission.cost.resourceType] : 1f, CS$<>8__locals1.capturingNeutralNations, -1f);
						this.SelectMission(aimissionEntry8, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
					}
				}
				KeyValuePair<TICouncilorState, List<TIGameState>> keyValuePair2 = (from x in CS$<>8__locals1.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, false, null, false).Contains(TIFactionState.buildFacilityMission)).ToDictionary<TICouncilorState, TICouncilorState, List<TIGameState>>((TICouncilorState x) => x, delegate(TICouncilorState x)
					{
						IEnumerable<TIGameState> validTargets2 = TIFactionState.buildFacilityMission.GetValidTargets(x);
						Func<TIGameState, bool> func11;
						if ((func11 = CS$<>8__locals1.<>9__101) == null)
						{
							func11 = (CS$<>8__locals1.<>9__101 = (TIGameState x) => x != CS$<>8__locals1.alienNation);
						}
						return validTargets2.Where<TIGameState>(func11).ToList<TIGameState>();
					})
					where x.Value.Count > 0
					select x).MaxBy<KeyValuePair<TICouncilorState, List<TIGameState>>, int>((KeyValuePair<TICouncilorState, List<TIGameState>> x) => x.Key.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, false, false));
				TICouncilorState key5 = keyValuePair2.Key;
				if (key5 != null)
				{
					TIGameState tigameState4 = keyValuePair2.Value.MaxBy<TIGameState, float>((TIGameState x) => AICouncilorMissionPlanner.<PlanMissionsTask>g__ScoreNationForAlienAquisition|98_89(x.ref_nation));
					AIMissionEntry aimissionEntry9 = new AIMissionEntry(this, TIFactionState.buildFacilityMission, key5, tigameState4, CS$<>8__locals1.faction.currentRiskAversion, CS$<>8__locals1.requiredMissions, CS$<>8__locals1.missingRequiredMissions, false, AICouncilorMissionPlanner.campaignDuration_years, this.factionDesiredMilestones, this.huntingForAlienActivity, this.huntAbility, this.warFactions, this.recentAlienSite, this.timeSinceAlienSite_days, TIFactionState.buildFacilityMission.hasCost ? this.availableResources[TIFactionState.buildFacilityMission.cost.resourceType] : 1f, CS$<>8__locals1.capturingNeutralNations, -1f);
					this.SelectMission(aimissionEntry9, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
				}
				if (!GameStateManager.AlienNation().extant || TIUtilities.RandomFloatValue() < 0.35f)
				{
					IEnumerable<TICouncilorState> enumerable6 = CS$<>8__locals1.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, false, null, false).Contains(TIFactionState.abductionsMission));
					int num21 = 0;
					while (enumerable6.Any<TICouncilorState>() && num21 < 2 && CS$<>8__locals1.availableCouncilors.Count >= (CS$<>8__locals1.faction.IsAlienFaction ? 2 : 3))
					{
						TICouncilorState bestCouncilor = enumerable6.MaxBy<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(TIFactionState.abductionsMission.primaryAttackerStat, true, true, true, false, false, false));
						enumerable6 = enumerable6.Where<TICouncilorState>((TICouncilorState x) => x != bestCouncilor);
						TIRegionState bestRegionsForFacilityAbductions = AIEvaluators.GetBestRegionsForFacilityAbductions(CS$<>8__locals1.faction, bestCouncilor);
						if (!(bestRegionsForFacilityAbductions == null))
						{
							AIMissionEntry aimissionEntry10 = new AIMissionEntry(this, TIFactionState.abductionsMission, bestCouncilor, bestRegionsForFacilityAbductions, CS$<>8__locals1.faction.currentRiskAversion, CS$<>8__locals1.requiredMissions, CS$<>8__locals1.missingRequiredMissions, false, AICouncilorMissionPlanner.campaignDuration_years, this.factionDesiredMilestones, this.huntingForAlienActivity, this.huntAbility, this.warFactions, this.recentAlienSite, this.timeSinceAlienSite_days, TIFactionState.abductionsMission.hasCost ? this.availableResources[TIFactionState.abductionsMission.cost.resourceType] : 1f, CS$<>8__locals1.capturingNeutralNations, -1f);
							this.SelectMission(aimissionEntry10, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
							num21++;
						}
					}
				}
			}
			List<FactionGoal_FleetCouncilorGoal> list9 = (from x in CS$<>8__locals1.faction.factionGoals.SelectMany<KeyValuePair<GoalType, List<TIFactionGoalState>>, FactionGoal_FleetCouncilorGoal>((KeyValuePair<GoalType, List<TIFactionGoalState>> x) => x.Value.Select<TIFactionGoalState, FactionGoal_FleetCouncilorGoal>((TIFactionGoalState y) => y as FactionGoal_FleetCouncilorGoal))
				where x != null && !x.skipGoal
				orderby x.importance descending
				select x).ToList<FactionGoal_FleetCouncilorGoal>();
			foreach (FactionGoal_FleetCouncilorGoal factionGoal_FleetCouncilorGoal in list9)
			{
				foreach (TICouncilorState ticouncilorState11 in factionGoal_FleetCouncilorGoal.assignedCouncilors.ToList<TICouncilorState>())
				{
					if (factionGoal_FleetCouncilorGoal.ShouldUnassignCouncilor(ticouncilorState11))
					{
						factionGoal_FleetCouncilorGoal.assignedCouncilors.Remove(ticouncilorState11);
					}
				}
			}
			foreach (FactionGoal_FleetCouncilorGoal factionGoal_FleetCouncilorGoal2 in list9)
			{
				if (factionGoal_FleetCouncilorGoal2.WantsAdditionalCouncilors)
				{
					foreach (TIMissionTemplate timissionTemplate in factionGoal_FleetCouncilorGoal2.GetUltimateMissionOptions())
					{
						TICouncilorState bestCouncilorForJob = CS$<>8__locals1.faction.GetBestCouncilorForJob(timissionTemplate, CS$<>8__locals1.availableCouncilors);
						if (!(bestCouncilorForJob == null) && !CS$<>8__locals1.<PlanMissionsTask>g__GetFleetCouncilorGoalMissionEntry|10(bestCouncilorForJob, timissionTemplate, factionGoal_FleetCouncilorGoal2.GetMissionTarget(timissionTemplate)).isTooRisky)
						{
							factionGoal_FleetCouncilorGoal2.assignedCouncilors.Add(bestCouncilorForJob);
						}
					}
				}
			}
			foreach (FactionGoal_FleetCouncilorGoal factionGoal_FleetCouncilorGoal3 in list9)
			{
				using (List<TICouncilorState>.Enumerator enumerator = factionGoal_FleetCouncilorGoal3.assignedCouncilors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TICouncilorState councilor4 = enumerator.Current;
						if (CS$<>8__locals1.availableCouncilors.Contains(councilor4))
						{
							List<AIMissionEntry> list10 = (from x in factionGoal_FleetCouncilorGoal3.GetMissionOptions(councilor4)
								select CS$<>8__locals1.<PlanMissionsTask>g__GetFleetCouncilorGoalMissionEntry|10(councilor4, x.Item1, x.Item2) into x
								where !x.isTooRisky
								select x).ToList<AIMissionEntry>();
							if (list10.Count != 0)
							{
								AIMissionEntry aimissionEntry11 = list10.MaxBy<AIMissionEntry, float>((AIMissionEntry x) => x.estimatedFinalSuccessChance);
								this.SelectMission(aimissionEntry11, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
							}
						}
					}
				}
			}
			CS$<>8__locals1.faction.ClearPlannedPolicies();
			IEnumerable<TINationState> enumerable7 = from x in CS$<>8__locals1.faction.nationsWithInterest(false)
				where x.executiveFaction == CS$<>8__locals1.faction && !x.executiveControlPoint.benefitsDisabled
				select x;
			this.scoredPolicyOptions.Clear();
			Dictionary<PolicyOptionWithTarget, int> dictionary5 = CS$<>8__locals1.faction.AllSetPolicyMissionOptionsWithTargets.ToDictionary<PolicyOptionWithTarget, PolicyOptionWithTarget, int>((PolicyOptionWithTarget x) => x, (PolicyOptionWithTarget x) => 0);
			List<TIFactionGoalState> list11 = CS$<>8__locals1.faction.factionGoals.Values.SelectMany<List<TIFactionGoalState>, TIFactionGoalState>((List<TIFactionGoalState> x) => x.Where<TIFactionGoalState>((TIFactionGoalState y) => y.PoliciesAsNationGoal() && !y.skipGoal)).ToList<TIFactionGoalState>();
			List<TIFactionGoalState> list12 = CS$<>8__locals1.faction.factionGoals.Values.SelectMany<List<TIFactionGoalState>, TIFactionGoalState>((List<TIFactionGoalState> x) => x.Where<TIFactionGoalState>((TIFactionGoalState y) => y.PoliciesAtTargetNationGoal() && !y.skipGoal)).ToList<TIFactionGoalState>();
			List<TINationState> list13 = new List<TINationState>();
			Dictionary<TINationState, Dictionary<PolicyType, int>> dictionary6 = new Dictionary<TINationState, Dictionary<PolicyType, int>>();
			using (List<TIFactionGoalState>.Enumerator enumerator10 = list11.GetEnumerator())
			{
				while (enumerator10.MoveNext())
				{
					TIFactionGoalState goal = enumerator10.Current;
					list13.Clear();
					if (goal.actor().isNationState)
					{
						list13.Add(goal.actor().ref_nation);
					}
					else if (goal.actor() == CS$<>8__locals1.faction && goal.PoliciesAsFactionActor)
					{
						list13.AddRange(enumerable7);
					}
					Func<PolicyType, int> <>9__113;
					foreach (TINationState tinationState in list13)
					{
						if (!dictionary6.ContainsKey(tinationState))
						{
							Dictionary<TINationState, Dictionary<PolicyType, int>> dictionary7 = dictionary6;
							TINationState tinationState2 = tinationState;
							IEnumerable<PolicyType> policiesAsNation = goal.policiesAsNation;
							Func<PolicyType, PolicyType> func3 = (PolicyType x) => x;
							Func<PolicyType, int> func4;
							if ((func4 = <>9__113) == null)
							{
								func4 = (<>9__113 = (PolicyType x) => goal.importance);
							}
							dictionary7[tinationState2] = policiesAsNation.ToDictionary<PolicyType, PolicyType, int>(func3, func4);
						}
						else
						{
							foreach (PolicyType policyType in goal.policiesAsNation)
							{
								int num22 = goal.importance;
								if (goal == CS$<>8__locals1.faction.focusGoal)
								{
									num22 *= 100;
								}
								if (dictionary6[tinationState].ContainsKey(policyType))
								{
									if (dictionary6[tinationState][policyType] < goal.importance)
									{
										dictionary6[tinationState][policyType] = goal.importance;
									}
								}
								else
								{
									dictionary6[tinationState].Add(policyType, goal.importance);
								}
							}
						}
					}
				}
			}
			if (dictionary6.Count > 0)
			{
				foreach (PolicyOptionWithTarget policyOptionWithTarget in new List<PolicyOptionWithTarget>(dictionary5.Keys))
				{
					if (dictionary6.ContainsKey(policyOptionWithTarget.actingNation))
					{
						if (!dictionary6[policyOptionWithTarget.actingNation].ContainsKey(policyOptionWithTarget.policyType))
						{
							if (!policyOptionWithTarget.AllowAIToUseWithoutGoal())
							{
								dictionary5.Remove(policyOptionWithTarget);
							}
							else
							{
								dictionary5[policyOptionWithTarget] = policyOptionWithTarget.GoallessImportance;
							}
						}
						else
						{
							dictionary5[policyOptionWithTarget] = dictionary6[policyOptionWithTarget.actingNation][policyOptionWithTarget.policyType];
						}
					}
				}
			}
			List<TINationState> list14 = new List<TINationState>();
			Dictionary<TINationState, Dictionary<PolicyType, int>> dictionary8 = new Dictionary<TINationState, Dictionary<PolicyType, int>>();
			foreach (TIFactionGoalState tifactionGoalState in list12)
			{
				list14.Clear();
				if (tifactionGoalState.target().isNationState)
				{
					list14.Add(tifactionGoalState.target().ref_nation);
				}
				else if (tifactionGoalState.target().isFactionState)
				{
					list14.AddRange(tifactionGoalState.target().ref_faction.executiveNations);
				}
				foreach (TINationState tinationState3 in list14)
				{
					if (!dictionary8.ContainsKey(tinationState3))
					{
						dictionary8.Add(tinationState3, new Dictionary<PolicyType, int>());
					}
					int importance = tifactionGoalState.importance;
					if (tifactionGoalState == CS$<>8__locals1.faction.focusGoal)
					{
						importance *= 100;
					}
					foreach (KeyValuePair<PolicyType, int> keyValuePair3 in tifactionGoalState.policiesAtTarget.ToDictionary<PolicyType, PolicyType, int>((PolicyType x) => x, (PolicyType x) => importance))
					{
						if (dictionary8[tinationState3].ContainsKey(keyValuePair3.Key))
						{
							dictionary8[tinationState3][keyValuePair3.Key] = Mathf.Max(dictionary8[tinationState3][keyValuePair3.Key], keyValuePair3.Value);
						}
						else
						{
							dictionary8[tinationState3].Add(keyValuePair3.Key, keyValuePair3.Value);
						}
					}
				}
			}
			if (dictionary8.Count > 0)
			{
				foreach (PolicyOptionWithTarget policyOptionWithTarget2 in new List<PolicyOptionWithTarget>(dictionary5.Keys))
				{
					TIGameState target3 = policyOptionWithTarget2.target;
					if (target3 != null && target3.isNationState && (!dictionary8.ContainsKey(policyOptionWithTarget2.target.ref_nation) || !dictionary8[policyOptionWithTarget2.target.ref_nation].ContainsKey(policyOptionWithTarget2.policyType)))
					{
						if (!policyOptionWithTarget2.AllowAIToUseWithoutGoal())
						{
							dictionary5.Remove(policyOptionWithTarget2);
						}
						else
						{
							dictionary5[policyOptionWithTarget2] = policyOptionWithTarget2.GoallessImportance;
						}
					}
				}
			}
			foreach (PolicyOptionWithTarget policyOptionWithTarget3 in dictionary5.Keys)
			{
				float num23 = AICouncilorMissionPlanner.ScorePolicyOption(policyOptionWithTarget3, CS$<>8__locals1.faction, dictionary5[policyOptionWithTarget3], dictionary8, this.nationPayoffs);
				if (num23 > 0f)
				{
					this.AddScoredPolicyOption(policyOptionWithTarget3, num23);
				}
			}
			using (List<KeyValuePair<PolicyOptionWithTarget, float>>.Enumerator enumerator16 = this.scoredPolicyOptions.ToList<KeyValuePair<PolicyOptionWithTarget, float>>().GetEnumerator())
			{
				while (enumerator16.MoveNext())
				{
					AICouncilorMissionPlanner.<>c__DisplayClass98_21 CS$<>8__locals21 = new AICouncilorMissionPlanner.<>c__DisplayClass98_21();
					CS$<>8__locals21.CS$<>8__locals8 = CS$<>8__locals1;
					CS$<>8__locals21.policyOption = enumerator16.Current;
					if (CS$<>8__locals21.policyOption.Key.policyType == PolicyType.WarOption)
					{
						TIGameState target4 = CS$<>8__locals21.policyOption.Key.target;
						TINationState targetNation = target4 as TINationState;
						if (targetNation != null && this.scoredPolicyOptions.Where<KeyValuePair<PolicyOptionWithTarget, float>>(delegate(KeyValuePair<PolicyOptionWithTarget, float> x)
						{
							TIWarState tiwarState = x.Key.target as TIWarState;
							return tiwarState != null && tiwarState.ProspectiveEnemyAlliance(CS$<>8__locals21.policyOption.Key.actingNation).Contains(targetNation);
						}).ToList<KeyValuePair<PolicyOptionWithTarget, float>>().Any<KeyValuePair<PolicyOptionWithTarget, float>>((KeyValuePair<PolicyOptionWithTarget, float> x) => CS$<>8__locals21.CS$<>8__locals8.<>4__this.scoredPolicyOptions[x.Key] >= CS$<>8__locals21.policyOption.Value))
						{
							this.scoredPolicyOptions.Remove(CS$<>8__locals21.policyOption.Key);
						}
					}
				}
			}
			Dictionary<TICouncilorState, PolicyOptionWithTarget> dictionary9 = new Dictionary<TICouncilorState, PolicyOptionWithTarget>();
			if (this.scoredPolicyOptions.Count > 0 && this.scoredPolicyOptions.Values.Max() >= 100000f)
			{
				Dictionary<PolicyOptionWithTarget, float> dictionary10 = new Dictionary<PolicyOptionWithTarget, float>();
				foreach (PolicyOptionWithTarget policyOptionWithTarget4 in this.scoredPolicyOptions.Keys)
				{
					if (this.scoredPolicyOptions[policyOptionWithTarget4] >= 100000f)
					{
						dictionary10.Add(policyOptionWithTarget4, this.scoredPolicyOptions[policyOptionWithTarget4]);
					}
				}
				List<TICouncilorState> list15 = CS$<>8__locals1.availableCouncilors.Where<TICouncilorState>((TICouncilorState councilor) => !councilor.HasMission && CS$<>8__locals1.possibleMissionDictionary[councilor].Contains(TIFactionState.setPolicyMission)).ToList<TICouncilorState>();
				list15 = list15.OrderByDescending<TICouncilorState, int>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Security, true, true, true, false, false, false)).ToList<TICouncilorState>();
				int num24 = 0;
				while (num24 < list15.Count && TIFactionState.setPolicyMission.CanAfford(CS$<>8__locals1.faction, null) && dictionary10.Count > 0)
				{
					List<TIGameState> validTargets = TIFactionState.setPolicyMission.GetValidTargets(list15[num24]).ToList<TIGameState>();
					Dictionary<PolicyOptionWithTarget, float> dictionary11 = dictionary10.Where<KeyValuePair<PolicyOptionWithTarget, float>>((KeyValuePair<PolicyOptionWithTarget, float> x) => validTargets.Contains(x.Key.actingNation)).ToDictionary<KeyValuePair<PolicyOptionWithTarget, float>, PolicyOptionWithTarget, float>((KeyValuePair<PolicyOptionWithTarget, float> x) => x.Key, (KeyValuePair<PolicyOptionWithTarget, float> x) => x.Value);
					if (dictionary11.Count<KeyValuePair<PolicyOptionWithTarget, float>>() > 0)
					{
						PolicyOptionWithTarget key6 = dictionary11.MaxBy<KeyValuePair<PolicyOptionWithTarget, float>, float>((KeyValuePair<PolicyOptionWithTarget, float> x) => x.Value).Key;
						AIMissionEntry aimissionEntry12 = new AIMissionEntry
						{
							councilor = list15[num24],
							mission = TIFactionState.setPolicyMission,
							target = key6.actingNation,
							sliderSteps = 0,
							payoff = this.scoredPolicyOptions[key6],
							expectedUtility = this.scoredPolicyOptions[key6],
							successChanceHigh = TIFactionState.setPolicyMission.resolutionMethod.GetSuccessChance(TIFactionState.setPolicyMission, list15[num24], null, 0f, false),
							successChanceLow = TIFactionState.setPolicyMission.resolutionMethod.GetSuccessChance(TIFactionState.setPolicyMission, list15[num24], null, 0f, false)
						};
						this.SelectMission(aimissionEntry12, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
						dictionary9.Add(list15[num24], key6);
						Dictionary<FactionResource, float> dictionary = this.availableResources;
						dictionary[FactionResource.Influence] = dictionary[FactionResource.Influence] - (float)((int)TIFactionState.setPolicyMission.cost.GetCost(0f, list15[num24], null));
						dictionary10.Remove(key6);
						this.scoredPolicyOptions.Remove(key6);
						if (key6.policy is WarOption)
						{
							List<PolicyOptionWithTarget> list16 = new List<PolicyOptionWithTarget>();
							foreach (KeyValuePair<PolicyOptionWithTarget, float> keyValuePair4 in this.scoredPolicyOptions.Where<KeyValuePair<PolicyOptionWithTarget, float>>((KeyValuePair<PolicyOptionWithTarget, float> x) => x.Key.policy is WarOption))
							{
								list16.Add(keyValuePair4.Key);
							}
							using (List<PolicyOptionWithTarget>.Enumerator enumerator13 = list16.GetEnumerator())
							{
								while (enumerator13.MoveNext())
								{
									PolicyOptionWithTarget policyOptionWithTarget5 = enumerator13.Current;
									this.scoredPolicyOptions.Remove(policyOptionWithTarget5);
									dictionary10.Remove(policyOptionWithTarget5);
								}
								goto IL_3727;
							}
						}
						if (key6.policy is TransferRegionsOption)
						{
							foreach (KeyValuePair<PolicyOptionWithTarget, float> keyValuePair5 in this.scoredPolicyOptions.Where<KeyValuePair<PolicyOptionWithTarget, float>>((KeyValuePair<PolicyOptionWithTarget, float> x) => x.Key.policy is TransferRegionsOption).ToList<KeyValuePair<PolicyOptionWithTarget, float>>())
							{
								if (keyValuePair5.Key.target.ref_nation == key6.target.ref_nation)
								{
									this.scoredPolicyOptions.Remove(keyValuePair5.Key);
									dictionary10.Remove(keyValuePair5.Key);
								}
							}
						}
					}
					IL_3727:
					num24++;
				}
			}
			CS$<>8__locals1.inspireMission = TIFactionState.inspireMission;
			List<TICouncilorState> list17 = (from x in CS$<>8__locals1.faction.councilors
				select CS$<>8__locals1.faction.GetViewofCouncilor(x) into x
				where x.turned
				where CS$<>8__locals1.faction.ShouldTryToRestoreCouncilorLoyalty(x.councilor)
				orderby x.EvaluateCouncilor() descending
				select x.councilor).ToList<TICouncilorState>();
			List<TICouncilorState> list18 = (from x in CS$<>8__locals1.availableCouncilors
				where !x.HasMission
				where CS$<>8__locals1.possibleMissionDictionary[x].Contains(CS$<>8__locals1.inspireMission)
				orderby x.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) descending
				select x).ToList<TICouncilorState>();
			using (List<TICouncilorState>.Enumerator enumerator = list17.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TICouncilorState turnedCouncilor = enumerator.Current;
					TICouncilorState ticouncilorState12 = list18.FirstOrDefault<TICouncilorState>((TICouncilorState x) => x != turnedCouncilor);
					if (ticouncilorState12 == null)
					{
						break;
					}
					float num25 = 1f;
					int num26 = 0;
					if (CS$<>8__locals1.inspireMission.hasCost)
					{
						FactionResource resourceType = CS$<>8__locals1.inspireMission.cost.resourceType;
						num25 = this.availableResources[resourceType];
						float num27 = num25 / CS$<>8__locals1.faction.GetCurrentResourceAmount(resourceType);
						num26 = Mathf.Min(7, ticouncilorState12.CurrentMaxSliderSteps(CS$<>8__locals1.inspireMission, num27));
					}
					if (CS$<>8__locals1.faction.WorthTryingToUnturnCouncilor(turnedCouncilor, ticouncilorState12, num26))
					{
						AIMissionEntry aimissionEntry13 = new AIMissionEntry(this, CS$<>8__locals1.inspireMission, ticouncilorState12, turnedCouncilor, CS$<>8__locals1.faction.currentRiskAversion, CS$<>8__locals1.requiredMissions, CS$<>8__locals1.missingRequiredMissions, false, AICouncilorMissionPlanner.campaignDuration_years, this.factionDesiredMilestones, this.huntingForAlienActivity, this.huntAbility, this.warFactions, this.recentAlienSite, this.timeSinceAlienSite_days, num25, CS$<>8__locals1.capturingNeutralNations, -1f);
						this.SelectMission(aimissionEntry13, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
						list18.Remove(ticouncilorState12);
					}
				}
			}
			if (!CS$<>8__locals1.faction.IsAlienFaction)
			{
				if (CS$<>8__locals1.faction.numActiveCouncilors >= 4)
				{
					if (CS$<>8__locals1.faction.totalControlNations.None<TINationState>((TINationState x) => x.MajorGlobalPower) && (TIGlobalValuesState.GlobalValues.difficulty > 0 || TITimeState.CampaignDuration_CompleteYears() > 3))
					{
						IEnumerable<TIFactionGoalState> enumerable8 = from x in CS$<>8__locals1.faction.AllCaptureNationGoals(true)
							where x.target().ref_nation.MajorGlobalPower
							select x;
						List<TIFactionState> list19 = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.NonAggressionPact, false, true)
							select x.faction).ToList<TIFactionState>();
						list19.AddRange((from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.TruceWithFaction, false, true)
							select x.faction into x
							where x != null
							select x).Distinct<TIFactionState>());
						IEnumerable<TIFactionState> captureGoalTargets = enumerable8.Select<TIFactionGoalState, TIFactionState>((TIFactionGoalState x) => x.target().ref_faction).Distinct<TIFactionState>();
						captureGoalTargets = captureGoalTargets.Except<TIFactionState>(list19);
						if (captureGoalTargets.Count<TIFactionState>() > 0)
						{
							enumerable8 = enumerable8.Where<TIFactionGoalState>((TIFactionGoalState x) => captureGoalTargets.Contains(x.target()));
						}
						int num28 = enumerable8.Count<TIFactionGoalState>();
						if (num28 <= 0)
						{
							goto IL_4695;
						}
						IEnumerable<TIFactionGoalState> enumerable9 = enumerable8.Where<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_nation.FactionsWithControlPoint.Contains(CS$<>8__locals1.faction));
						if (enumerable9.Any<TIFactionGoalState>())
						{
							IEnumerable<TIFactionGoalState> enumerable10 = enumerable9.Where<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_nation.executiveFaction == CS$<>8__locals1.faction);
							if (enumerable10.Any<TIFactionGoalState>())
							{
								enumerable8 = enumerable10;
							}
							else
							{
								enumerable8 = enumerable9;
							}
						}
						else
						{
							enumerable9 = enumerable8.Where<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_nation.executiveFaction == null);
							if (enumerable9.Any<TIFactionGoalState>())
							{
								IEnumerable<TIFactionGoalState> enumerable11 = enumerable9.Where<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_nation.NumNativeControlPoints - 1 >= x.target().ref_nation.NumNativeControlPoints);
								if (enumerable11.Any<TIFactionGoalState>())
								{
									enumerable8 = enumerable11;
								}
								else
								{
									enumerable8 = enumerable9;
								}
							}
						}
						FactionGoal_CaptureNation factionGoal_CaptureNation = enumerable8.MaxBy<TIFactionGoalState, int>((TIFactionGoalState x) => x.importance) as FactionGoal_CaptureNation;
						TINationState nation = factionGoal_CaptureNation.nation;
						CS$<>8__locals1.faction.focusGoal = factionGoal_CaptureNation;
						int numNativeControlPoints = nation.NumNativeControlPoints;
						List<TIControlPoint> list20 = nation.FactionControlPoints(CS$<>8__locals1.faction, true, false, true);
						nation.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.faction != null && x.faction != CS$<>8__locals1.faction).ToList<TIControlPoint>();
						nation.GetPublicOpinionOfFaction(CS$<>8__locals1.faction);
						Dictionary<AIForcedMissionEntry, float> focusFireMissionDictionary = new Dictionary<AIForcedMissionEntry, float>();
						List<TIMissionTemplate> list21 = new List<TIMissionTemplate>
						{
							TIFactionState.controlNationMission,
							TIFactionState.publicCampaignMission,
							TIFactionState.defendInterestsMission
						};
						if (factionGoal_CaptureNation.GetGoalType() == GoalType.CaptureNationDirty && numNativeControlPoints == 0 && !nation.FactionHasControlPoint(CS$<>8__locals1.faction))
						{
							list21.Add(TIFactionState.coupMission);
							list21.Add(TIFactionState.unrestMission);
						}
						if (numNativeControlPoints > 1)
						{
							if (!nation.AdjacentNations(false).All<TINationState>((TINationState x) => x.NumNativeControlPoints <= 1))
							{
								goto IL_3D65;
							}
						}
						list21.Add(TIFactionState.purgeMission);
						list21.Add(TIFactionState.crackdownMission);
						IL_3D65:
						List<TIGameState> list22 = new List<TIGameState>();
						List<TICouncilorState> list23 = CS$<>8__locals1.availableCouncilors.Where<TICouncilorState>((TICouncilorState councilor) => !councilor.HasMission).ToList<TICouncilorState>();
						foreach (TIMissionTemplate timissionTemplate2 in list21)
						{
							foreach (TICouncilorState ticouncilorState13 in list23)
							{
								list22.Clear();
								if (CS$<>8__locals1.possibleMissionDictionary[ticouncilorState13].Contains(timissionTemplate2))
								{
									if (timissionTemplate2.target is TIMissionTarget_Nation || timissionTemplate2.target is TIMissionTarget_NationFleetHab)
									{
										if (timissionTemplate2 == TIFactionState.publicCampaignMission && (double)nation.GetPublicOpinionOfFaction(CS$<>8__locals1.faction) >= (double)nation.singleIdeaCap - 0.05)
										{
											continue;
										}
										if (timissionTemplate2 == TIFactionState.defendInterestsMission)
										{
											if (nation.FactionControlPoints(CS$<>8__locals1.faction, false, false, true).All<TIControlPoint>((TIControlPoint x) => x.defended))
											{
												continue;
											}
										}
										list22.Add(nation);
										if (timissionTemplate2 == TIFactionState.controlNationMission)
										{
											list22.AddRange(nation.AdjacentNations(false));
										}
									}
									else if (timissionTemplate2.target is TIMissionTarget_OwnedControlPoint)
									{
										list22.AddRange(nation.EnemyControlPoints(CS$<>8__locals1.faction));
										if (list20.Count == 0)
										{
											if (timissionTemplate2 == TIFactionState.purgeMission)
											{
												list22.AddRange(from x in nation.AdjacentNations(false).SelectMany<TINationState, TIControlPoint>((TINationState x) => x.controlPoints)
													where x.benefitsDisabled
													select x);
											}
											else if (timissionTemplate2 == TIFactionState.crackdownMission)
											{
												list22.AddRange(from x in nation.AdjacentNations(false).SelectMany<TINationState, TIControlPoint>((TINationState x) => x.controlPoints)
													where !x.benefitsDisabled
													select x);
											}
										}
									}
									else if (timissionTemplate2.target is TIMissionTarget_Region)
									{
										list22.Add(nation.capital);
									}
									foreach (TIGameState tigameState5 in list22)
									{
										if (timissionTemplate2.target.ValidTarget(timissionTemplate2.target.ValidateSingleTarget(timissionTemplate2, ticouncilorState13, tigameState5)))
										{
											float successChance5 = timissionTemplate2.resolutionMethod.GetSuccessChance(timissionTemplate2, ticouncilorState13, tigameState5, 0f, false);
											TIMissionResolution resolutionMethod2 = timissionTemplate2.resolutionMethod;
											TIMissionTemplate timissionTemplate3 = timissionTemplate2;
											TICouncilorState ticouncilorState14 = ticouncilorState13;
											TIGameState tigameState6 = tigameState5;
											TIMissionCost cost4 = timissionTemplate2.cost;
											if ((double)resolutionMethod2.GetSuccessChance(timissionTemplate3, ticouncilorState14, tigameState6, (cost4 != null) ? cost4.GetCost((float)ticouncilorState13.CurrentMaxSliderSteps(timissionTemplate2, 1f), ticouncilorState13, null) : 0f, false) > 0.15)
											{
												focusFireMissionDictionary.Add(new AIForcedMissionEntry(ticouncilorState13, tigameState5, timissionTemplate2), successChance5);
											}
										}
									}
								}
							}
						}
						int num29 = Mathf.Clamp(list23.Count, 0, list23.Count / 2 + 1);
						List<AIForcedMissionEntry> list24 = new List<AIForcedMissionEntry>();
						using (List<TIMissionTemplate>.Enumerator enumerator7 = list21.GetEnumerator())
						{
							Func<KeyValuePair<AIForcedMissionEntry, float>, float> <>9__148;
							Func<KeyValuePair<AIForcedMissionEntry, float>, float> <>9__151;
							while (enumerator7.MoveNext())
							{
								TIMissionTemplate mission = enumerator7.Current;
								if (focusFireMissionDictionary.Any<KeyValuePair<AIForcedMissionEntry, float>>((KeyValuePair<AIForcedMissionEntry, float> x) => x.Key.mission == mission))
								{
									IEnumerable<KeyValuePair<AIForcedMissionEntry, float>> enumerable12 = focusFireMissionDictionary.Where<KeyValuePair<AIForcedMissionEntry, float>>((KeyValuePair<AIForcedMissionEntry, float> x) => x.Key.mission == mission);
									Func<KeyValuePair<AIForcedMissionEntry, float>, float> func5;
									if ((func5 = <>9__148) == null)
									{
										func5 = (<>9__148 = (KeyValuePair<AIForcedMissionEntry, float> x) => focusFireMissionDictionary[x.Key]);
									}
									AIForcedMissionEntry key7 = enumerable12.MaxBy<KeyValuePair<AIForcedMissionEntry, float>, float>(func5).Key;
									list24.Add(key7);
									foreach (AIForcedMissionEntry aiforcedMissionEntry5 in focusFireMissionDictionary.Keys.ToList<AIForcedMissionEntry>())
									{
										if (aiforcedMissionEntry5.councilor == key7.councilor)
										{
											focusFireMissionDictionary.Remove(aiforcedMissionEntry5);
										}
									}
									num29--;
									if (mission.AIDoubleUpAllowed && num29 > 0)
									{
										int num30 = 1;
										if (mission == TIFactionState.publicCampaignMission)
										{
											if (nation.GetPublicOpinionOfFaction(CS$<>8__locals1.faction) > 0.5f)
											{
												num30 = 0;
											}
											else
											{
												num30 = 3;
											}
										}
										Func<KeyValuePair<AIForcedMissionEntry, float>, bool> <>9__149;
										Func<KeyValuePair<AIForcedMissionEntry, float>, bool> <>9__150;
										for (int k = 0; k < num30; k++)
										{
											IEnumerable<KeyValuePair<AIForcedMissionEntry, float>> focusFireMissionDictionary3 = focusFireMissionDictionary;
											Func<KeyValuePair<AIForcedMissionEntry, float>, bool> func6;
											if ((func6 = <>9__149) == null)
											{
												func6 = (<>9__149 = (KeyValuePair<AIForcedMissionEntry, float> x) => x.Key.mission == mission);
											}
											if (!focusFireMissionDictionary3.Any<KeyValuePair<AIForcedMissionEntry, float>>(func6))
											{
												break;
											}
											IEnumerable<KeyValuePair<AIForcedMissionEntry, float>> focusFireMissionDictionary2 = focusFireMissionDictionary;
											Func<KeyValuePair<AIForcedMissionEntry, float>, bool> func7;
											if ((func7 = <>9__150) == null)
											{
												func7 = (<>9__150 = (KeyValuePair<AIForcedMissionEntry, float> x) => x.Key.mission == mission);
											}
											IEnumerable<KeyValuePair<AIForcedMissionEntry, float>> enumerable13 = focusFireMissionDictionary2.Where<KeyValuePair<AIForcedMissionEntry, float>>(func7);
											Func<KeyValuePair<AIForcedMissionEntry, float>, float> func8;
											if ((func8 = <>9__151) == null)
											{
												func8 = (<>9__151 = (KeyValuePair<AIForcedMissionEntry, float> x) => focusFireMissionDictionary[x.Key]);
											}
											AIForcedMissionEntry key8 = enumerable13.MaxBy<KeyValuePair<AIForcedMissionEntry, float>, float>(func8).Key;
											if (focusFireMissionDictionary[key8] > 0.25f)
											{
												list24.Add(key8);
												foreach (AIForcedMissionEntry aiforcedMissionEntry6 in focusFireMissionDictionary.Keys.ToList<AIForcedMissionEntry>())
												{
													if (aiforcedMissionEntry6.councilor == key7.councilor)
													{
														focusFireMissionDictionary.Remove(aiforcedMissionEntry6);
													}
												}
												num29--;
											}
										}
									}
									if (num29 <= 0)
									{
										break;
									}
								}
							}
						}
						int num31 = 0;
						foreach (AIForcedMissionEntry aiforcedMissionEntry7 in list24)
						{
							int num32 = aiforcedMissionEntry7.councilor.CurrentMaxSliderSteps(aiforcedMissionEntry7.mission, 1f);
							float successChance6 = aiforcedMissionEntry7.mission.resolutionMethod.GetSuccessChance(aiforcedMissionEntry7.mission, aiforcedMissionEntry7.councilor, aiforcedMissionEntry7.target, 0f, false);
							TIMissionResolution resolutionMethod3 = aiforcedMissionEntry7.mission.resolutionMethod;
							TIMissionTemplate mission2 = aiforcedMissionEntry7.mission;
							TICouncilorState councilor2 = aiforcedMissionEntry7.councilor;
							TIGameState target5 = aiforcedMissionEntry7.target;
							TIMissionCost cost5 = aiforcedMissionEntry7.mission.cost;
							float successChance7 = resolutionMethod3.GetSuccessChance(mission2, councilor2, target5, (cost5 != null) ? cost5.GetCost((float)num32, aiforcedMissionEntry7.councilor, null) : 0f, false);
							AIMissionEntry aimissionEntry14 = new AIMissionEntry
							{
								councilor = aiforcedMissionEntry7.councilor,
								mission = aiforcedMissionEntry7.mission,
								target = aiforcedMissionEntry7.target,
								sliderSteps = num32,
								payoff = 1000000f * successChance6,
								expectedUtility = 1000000f * ((successChance6 + successChance7) / 2f),
								successChanceHigh = successChance7,
								successChanceLow = successChance6
							};
							num31++;
							this.SelectMission(aimissionEntry14, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
						}
						if (num28 <= 1 || num31 != 0 || nation.NumNativeControlPoints != 0 || nation.CountFactionControlPoints(CS$<>8__locals1.faction, true, false, true) != 0)
						{
							goto IL_4695;
						}
						if (factionGoal_CaptureNation.GetGoalType() == GoalType.CaptureNationClean)
						{
							factionGoal_CaptureNation.ChangeImportance(-2, 1, 20);
							goto IL_4695;
						}
						if (factionGoal_CaptureNation.GetGoalType() == GoalType.CaptureNationDirty)
						{
							factionGoal_CaptureNation.ChangeImportance(-8, 1, 20);
							goto IL_4695;
						}
						goto IL_4695;
					}
				}
				float num33 = 0.4f;
				TIFactionGoalState focusGoal = CS$<>8__locals1.faction.focusGoal;
				float num34 = num33 * (float)((focusGoal != null && focusGoal.InProgress()) ? 2 : 1);
				if (CS$<>8__locals1.faction.focusGoal != null && (TIUtilities.RandomFloatValue() > num34 || CS$<>8__locals1.faction.councilors.Count <= 3 || CS$<>8__locals1.faction.focusGoal.deleted || CS$<>8__locals1.faction.focusGoal.ShouldDiscardGoal() || CS$<>8__locals1.faction.focusGoal.GoalFulfilled()))
				{
					CS$<>8__locals1.faction.focusGoal = null;
				}
				if (CS$<>8__locals1.faction.focusGoal == null && CS$<>8__locals1.faction.councilors.Count >= 4 && TIUtilities.RandomFloatValue() < 0.2f)
				{
					List<TIFactionGoalState> list25 = CS$<>8__locals1.faction.GoalsOfType(TIFactionGoalState.NationMissionModifyingGoals, false, true);
					if (list25.Count > 0)
					{
						list25 = list25.Where<TIFactionGoalState>((TIFactionGoalState x) => x.importance >= 15).ToList<TIFactionGoalState>();
						if (list25.Count > 0)
						{
							CS$<>8__locals1.faction.focusGoal = list25.SelectRandomWeightedItem<TIFactionGoalState>((TIFactionGoalState x) => (float)x.importance, -1f, 1E-37f);
						}
					}
				}
			}
			IL_4695:
			this.factionMissionDictionary.Clear();
			foreach (TICouncilorState ticouncilorState15 in CS$<>8__locals1.availableCouncilors.Where<TICouncilorState>((TICouncilorState councilor) => !councilor.HasMission).ToList<TICouncilorState>())
			{
				this.councilorMissionDictionary.Clear();
				Dictionary<AIMissionEntry, PolicyOptionWithTarget> dictionary12 = new Dictionary<AIMissionEntry, PolicyOptionWithTarget>();
				foreach (TIMissionTemplate timissionTemplate4 in CS$<>8__locals1.possibleMissionDictionary[ticouncilorState15])
				{
					if (timissionTemplate4.CanAfford(CS$<>8__locals1.faction, null))
					{
						IEnumerable<TIGameState> validTargets3 = timissionTemplate4.GetValidTargets(ticouncilorState15);
						Dictionary<AIMissionEntry, float> dictionary13 = new Dictionary<AIMissionEntry, float>();
						using (IEnumerator<TIGameState> enumerator20 = validTargets3.GetEnumerator())
						{
							while (enumerator20.MoveNext())
							{
								TIGameState target = enumerator20.Current;
								AIMissionEntry aimissionEntry15 = null;
								if (timissionTemplate4 == TIFactionState.setPolicyMission)
								{
									IEnumerable<PolicyOptionWithTarget> enumerable14 = this.scoredPolicyOptions.Keys.Where<PolicyOptionWithTarget>((PolicyOptionWithTarget x) => x.actingNation == target);
									Func<PolicyOptionWithTarget, float> func9;
									if ((func9 = CS$<>8__locals1.<>9__160) == null)
									{
										func9 = (CS$<>8__locals1.<>9__160 = (PolicyOptionWithTarget x) => CS$<>8__locals1.<>4__this.scoredPolicyOptions[x]);
									}
									PolicyOptionWithTarget policyOptionWithTarget6 = enumerable14.MaxBy<PolicyOptionWithTarget, float>(func9);
									if (policyOptionWithTarget6 == null)
									{
										continue;
									}
									float num35 = this.scoredPolicyOptions[policyOptionWithTarget6];
									if (num35 > 0f)
									{
										float successChance8 = TIFactionState.setPolicyMission.resolutionMethod.GetSuccessChance(TIFactionState.setPolicyMission, ticouncilorState15, null, 0f, false);
										aimissionEntry15 = new AIMissionEntry
										{
											councilor = ticouncilorState15,
											mission = TIFactionState.setPolicyMission,
											target = policyOptionWithTarget6.actingNation,
											sliderSteps = 0,
											payoff = num35,
											expectedUtility = num35 * successChance8,
											successChanceHigh = successChance8,
											successChanceLow = successChance8
										};
										dictionary12[aimissionEntry15] = policyOptionWithTarget6;
									}
								}
								else
								{
									bool flag10 = false;
									foreach (AIMissionEntry aimissionEntry16 in CS$<>8__locals1.selectedMissions)
									{
										flag10 = AIEvaluators.AI_ShouldAvoidDoublingUpMissionTarget(aimissionEntry16.councilor, aimissionEntry16.mission, aimissionEntry16.target, aimissionEntry16.estimatedFinalSuccessChance, ticouncilorState15, timissionTemplate4, target);
										if (flag10)
										{
											break;
										}
									}
									if (flag10)
									{
										continue;
									}
									aimissionEntry15 = new AIMissionEntry(this, timissionTemplate4, ticouncilorState15, target, CS$<>8__locals1.faction.currentRiskAversion, CS$<>8__locals1.requiredMissions, CS$<>8__locals1.missingRequiredMissions, false, AICouncilorMissionPlanner.campaignDuration_years, this.factionDesiredMilestones, this.huntingForAlienActivity, this.huntAbility, this.warFactions, this.recentAlienSite, this.timeSinceAlienSite_days, timissionTemplate4.hasCost ? this.availableResources[timissionTemplate4.cost.resourceType] : 1f, CS$<>8__locals1.capturingNeutralNations, -1f);
								}
								if (aimissionEntry15 != null && aimissionEntry15.expectedUtility > 0f && !aimissionEntry15.isTooRisky)
								{
									dictionary13[aimissionEntry15] = aimissionEntry15.expectedUtility;
								}
							}
						}
						foreach (KeyValuePair<AIMissionEntry, float> keyValuePair6 in dictionary13.Sorted<KeyValuePair<AIMissionEntry, float>, float>((KeyValuePair<AIMissionEntry, float> x) => -x.Value).Take<KeyValuePair<AIMissionEntry, float>>(timissionTemplate4.maximumTargetOptionCount))
						{
							this.councilorMissionDictionary[keyValuePair6.Key] = keyValuePair6.Value;
						}
					}
				}
				if (!this.councilorMissionDictionary.Values.None<float>((float x) => x > 0f) && this.councilorMissionDictionary.Count > 0)
				{
					IEnumerable<AIMissionEntry> keys = this.councilorMissionDictionary.Keys;
					Func<AIMissionEntry, float> func10;
					if ((func10 = CS$<>8__locals1.<>9__161) == null)
					{
						func10 = (CS$<>8__locals1.<>9__161 = (AIMissionEntry x) => CS$<>8__locals1.<>4__this.councilorMissionDictionary[x]);
					}
					using (List<AIMissionEntry>.Enumerator enumerator21 = keys.OrderByDescending<AIMissionEntry, float>(func10).Take<AIMissionEntry>(0).ToList<AIMissionEntry>()
						.GetEnumerator())
					{
						while (enumerator21.MoveNext())
						{
							AIMissionEntry aimissionEntry17 = enumerator21.Current;
							Dictionary<AIMissionEntry, float> dictionary14 = this.councilorMissionDictionary;
							AIMissionEntry aimissionEntry18 = aimissionEntry17;
							dictionary14[aimissionEntry18] *= 50f;
						}
						goto IL_4B4B;
					}
					goto IL_4AE8;
				}
				goto IL_4AE8;
				IL_4B4B:
				AIMissionEntry key9 = this.councilorMissionDictionary.SelectRandomWeightedItem<KeyValuePair<AIMissionEntry, float>>((KeyValuePair<AIMissionEntry, float> j) => j.Value, -1f, 1E-37f).Key;
				if (key9 == null)
				{
					Log.Error(string.Concat(new string[]
					{
						"Could not select mission for ",
						ticouncilorState15.displayName,
						" (",
						CS$<>8__locals1.faction.displayName,
						", ",
						ticouncilorState15.location.displayName,
						")"
					}), Array.Empty<object>());
					break;
				}
				if (key9.mission == TIFactionState.setPolicyMission)
				{
					PolicyOptionWithTarget policyOptionWithTarget7 = dictionary12[key9];
					dictionary9[ticouncilorState15] = policyOptionWithTarget7;
					this.scoredPolicyOptions.Remove(policyOptionWithTarget7);
				}
				this.SelectMission(key9, ref CS$<>8__locals1.selectedMissions, ref CS$<>8__locals1.availableCouncilors);
				this.ModifyScoredPolicyOptionForCoordinatedMissions(key9, CS$<>8__locals1.availableCouncilors.Where<TICouncilorState>((TICouncilorState x) => !x.HasMission).ToList<TICouncilorState>(), ref this.councilorMissionDictionary);
				continue;
				IL_4AE8:
				AIMissionEntry aimissionEntry19 = new AIMissionEntry
				{
					councilor = ticouncilorState15,
					mission = TIFactionState.surveilMission,
					target = ticouncilorState15.location,
					sliderSteps = 0
				};
				if (this.councilorMissionDictionary.ContainsKey(aimissionEntry19))
				{
					this.councilorMissionDictionary[aimissionEntry19] = 1f;
					goto IL_4B4B;
				}
				this.councilorMissionDictionary.Add(aimissionEntry19, 1f);
				goto IL_4B4B;
			}
			CS$<>8__locals1.selectedMissions = CS$<>8__locals1.selectedMissions.OrderByDescending<AIMissionEntry, float>((AIMissionEntry x) => x.expectedUtility).ToList<AIMissionEntry>();
			return new AICouncilorMissionPlan(CS$<>8__locals1.selectedMissions, CS$<>8__locals1.faction, dictionary9, null, null);
		}

		// Token: 0x06005A0A RID: 23050 RVA: 0x0029D45C File Offset: 0x0029B65C
		public void RunSelectedPlayerActions(AICouncilorMissionPlan plan)
		{
			TIFactionState faction = plan.faction;
			IReadOnlyList<AIMissionEntry> selectedMissions = plan.selectedMissions;
			IReadOnlyDictionary<TICouncilorState, PolicyOptionWithTarget> councilorPolicy = plan.councilorPolicy;
			IPlayerActionRunner playerActionRunner = GameControl.playerManager.FindPlayerComponent(faction);
			using (IEnumerator<AIMissionEntry> enumerator = selectedMissions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AIMissionEntry selectedEntry = enumerator.Current;
					int num;
					if (selectedEntry.mission.hasCost)
					{
						num = selectedMissions.Where<AIMissionEntry>((AIMissionEntry x) => !x.finalized && x.mission.hasCost).Count<AIMissionEntry>((AIMissionEntry x) => x.mission.cost.resourceType == selectedEntry.mission.cost.resourceType);
					}
					else
					{
						num = 0;
					}
					float num2 = 0f;
					if (selectedEntry.sliderSteps > 0)
					{
						num2 = (float)this.SetIdealSpendForMission(selectedEntry, num);
					}
					if (selectedEntry.mission == TIFactionState.setPolicyMission)
					{
						faction.AddPlannedPolicy(councilorPolicy[selectedEntry.councilor]);
					}
					else
					{
						selectedEntry.finalSuccessChance = selectedEntry.mission.resolutionMethod.GetSuccessChance(selectedEntry.mission, selectedEntry.councilor, selectedEntry.target, num2, false);
						if ((double)selectedEntry.finalSuccessChance < 0.15 && !selectedEntry.objective)
						{
							TIFactionState.LogAI(string.Concat(new string[]
							{
								"WARNING: AI Selected mission with terrible success chance: ",
								selectedEntry.mission.displayName,
								" ",
								selectedEntry.finalSuccessChance.ToPercent("P0"),
								" ",
								selectedEntry.target.displayName
							}), false);
							num2 = (float)this.SetIdealSpendForMission(selectedEntry, num);
						}
					}
					playerActionRunner.StartAction(new AssignCouncilorToMission(selectedEntry.councilor, selectedEntry.mission, selectedEntry.target, num2, false));
					selectedEntry.finalized = true;
				}
			}
			playerActionRunner.StartAction(new FinalizeCouncilorMissions(faction));
			faction.planningMissions = false;
			faction.ClearScrambleValues();
			AICouncilorMissionPlanner.averageControlPointValue_Dominate = 0f;
			AICouncilorMissionPlanner.cpCapFraction_Dominate = 0f;
		}

		// Token: 0x06005A0B RID: 23051 RVA: 0x0029D6E8 File Offset: 0x0029B8E8
		public void SelectMission(AIMissionEntry selectedEntry, ref List<AIMissionEntry> selectedMissions, ref List<TICouncilorState> availableCouncilors)
		{
			if (selectedEntry.mission.hasCost && selectedEntry.mission.cost is TIMissionCost_Flat)
			{
				TIResourcesCost tiresourcesCost = new TIResourcesCost(selectedEntry.mission.cost.resourceType, selectedEntry.mission.cost.GetCost(0f, selectedEntry.councilor, selectedEntry.target));
				if (!tiresourcesCost.CanAfford_AI(selectedEntry.councilor.faction, null, null, 1, false, false, 1f, null, float.PositiveInfinity))
				{
					return;
				}
				tiresourcesCost.PayCost(selectedEntry.councilor.faction, "Flat Mission Cost");
			}
			selectedMissions.Add(selectedEntry);
			availableCouncilors.Remove(selectedEntry.councilor);
		}

		// Token: 0x06005A0D RID: 23053 RVA: 0x0029D834 File Offset: 0x0029BA34
		[CompilerGenerated]
		internal static float <GetGoalMultipliers>g__GetModifierFromGoals|38_0(List<TIFactionGoalState> goals, TIMissionTemplate mission, TIFactionGoalState focusGoal)
		{
			if (goals.Count == 0)
			{
				return 1f;
			}
			float[] array = goals.Select<TIFactionGoalState, float>((TIFactionGoalState x) => x.GetMissionPayoffMultiplier(mission, 1f)).ToArray<float>();
			if (array.Any<float>((float x) => x <= 0f))
			{
				return 0f;
			}
			float num = array.Average();
			if (goals.Contains(focusGoal))
			{
				num *= 100f;
			}
			return num;
		}

		// Token: 0x06005A0E RID: 23054 RVA: 0x0029D8BC File Offset: 0x0029BABC
		[CompilerGenerated]
		internal static float <PlanMissionsTask>g__GetSupportResourceCost|98_67(TIMissionTemplate supportMission, TICouncilorState supportCouncilor, TIGameState target, int supportSliderSteps)
		{
			TIMissionCost cost = supportMission.cost;
			float num = ((cost != null) ? cost.GetCost((float)supportSliderSteps, supportCouncilor, null) : 0f);
			return supportMission.resolutionMethod.GetSuccessChance(supportMission, supportCouncilor, target, num, false);
		}

		// Token: 0x06005A0F RID: 23055 RVA: 0x0029D8F4 File Offset: 0x0029BAF4
		[CompilerGenerated]
		internal static float <PlanMissionsTask>g__ScoreNationForAlienAquisition|98_89(TINationState nation)
		{
			if (nation == null)
			{
				return 0f;
			}
			return nation.BaseInvestmentPoints_month() + (float)(5 * nation.armies.Count);
		}

		// Token: 0x040040E0 RID: 16608
		private readonly IPlayerActionRunner runner;

		// Token: 0x040040E1 RID: 16609
		private List<TIFactionState> warFactions;

		// Token: 0x040040E2 RID: 16610
		private List<CampaignMilestone> factionDesiredMilestones;

		// Token: 0x040040E3 RID: 16611
		private Dictionary<AIMissionEntry, float> councilorMissionDictionary = new Dictionary<AIMissionEntry, float>();

		// Token: 0x040040E4 RID: 16612
		private Dictionary<TIFactionState, Dictionary<TIControlPoint, float>> rawControlPointPayoffs = new Dictionary<TIFactionState, Dictionary<TIControlPoint, float>>();

		// Token: 0x040040E5 RID: 16613
		private Dictionary<TIControlPoint, float> controlPointPayoffs = new Dictionary<TIControlPoint, float>();

		// Token: 0x040040E6 RID: 16614
		private Dictionary<TIFactionState, Dictionary<TINationState, float>> rawNationPayoffs = new Dictionary<TIFactionState, Dictionary<TINationState, float>>();

		// Token: 0x040040E7 RID: 16615
		private Dictionary<TINationState, float> nationPayoffs = new Dictionary<TINationState, float>();

		// Token: 0x040040E8 RID: 16616
		private Dictionary<TINationState, List<TIFactionGoalState>> nationModifyingGoals = new Dictionary<TINationState, List<TIFactionGoalState>>();

		// Token: 0x040040E9 RID: 16617
		private Dictionary<TIFactionState, List<TIFactionGoalState>> nationModifyingGoalsByFaction = new Dictionary<TIFactionState, List<TIFactionGoalState>>();

		// Token: 0x040040EA RID: 16618
		private Dictionary<TIFactionState, List<TIFactionGoalState>> factionMissionModifyingGoals = new Dictionary<TIFactionState, List<TIFactionGoalState>>();

		// Token: 0x040040EB RID: 16619
		private TIRegionState recentAlienSite;

		// Token: 0x040040EC RID: 16620
		private float timeSinceAlienSite_days;

		// Token: 0x040040ED RID: 16621
		private TINationState recentAlienControlPointGift;

		// Token: 0x040040EE RID: 16622
		private float timeSinceAlienControlPointGift_days;

		// Token: 0x040040EF RID: 16623
		private Dictionary<FactionResource, float> availableResources = new Dictionary<FactionResource, float>();

		// Token: 0x040040F0 RID: 16624
		private bool huntingForAlienActivity;

		// Token: 0x040040F1 RID: 16625
		private float huntAbility;

		// Token: 0x040040F2 RID: 16626
		private TIFactionState lastFactionPayoffValuesRecorded;

		// Token: 0x040040F3 RID: 16627
		public const int predictability_favoritesCount = 0;

		// Token: 0x040040F4 RID: 16628
		public const float predictability = 50f;

		// Token: 0x040040F5 RID: 16629
		public const float favoritesThreshold = 200000f;

		// Token: 0x040040F6 RID: 16630
		public const float favoritesMultiplier = 1f;

		// Token: 0x040040F7 RID: 16631
		public const float predictabilityPower = 1f;

		// Token: 0x040040F8 RID: 16632
		private const int focusGoalPolicyMultiplier = 100;

		// Token: 0x040040F9 RID: 16633
		private const int focusGoalMissionMultiplier = 100;

		// Token: 0x040040FB RID: 16635
		public static int cachedPayoffFrame;

		// Token: 0x040040FC RID: 16636
		private CameraManager cameraManager;

		// Token: 0x040040FD RID: 16637
		private bool AISmoothing;

		// Token: 0x040040FE RID: 16638
		private int SmoothingMSPerFrame = 16;

		// Token: 0x040040FF RID: 16639
		private Dictionary<AICachedMissionEntry, float> factionMissionDictionary = new Dictionary<AICachedMissionEntry, float>();

		// Token: 0x04004100 RID: 16640
		private static float averageControlPointValue_Dominate;

		// Token: 0x04004101 RID: 16641
		private static float cpCapFraction_Dominate;

		// Token: 0x04004102 RID: 16642
		private const int CPCountForNoAttacksWithoutGoal = 4;

		// Token: 0x04004103 RID: 16643
		private Dictionary<PolicyOptionWithTarget, float> scoredPolicyOptions = new Dictionary<PolicyOptionWithTarget, float>();

		// Token: 0x04004104 RID: 16644
		public static float campaignDuration_years;
	}
}
