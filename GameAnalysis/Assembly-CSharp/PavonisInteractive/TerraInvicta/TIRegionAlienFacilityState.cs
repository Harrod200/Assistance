using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000771 RID: 1905
	public class TIRegionAlienFacilityState : TIRegionAlienAssetState
	{
		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x060039D7 RID: 14807 RVA: 0x00155D0F File Offset: 0x00153F0F
		// (set) Token: 0x060039D8 RID: 14808 RVA: 0x00155D17 File Offset: 0x00153F17
		public bool built { get; private set; }

		// Token: 0x060039D9 RID: 14809 RVA: 0x00155D20 File Offset: 0x00153F20
		public override bool Extant()
		{
			return this.built;
		}

		// Token: 0x060039DA RID: 14810 RVA: 0x00155D28 File Offset: 0x00153F28
		public override string GetIconResourcePath(TIFactionState faction)
		{
			return TemplateManager.global.pathGeoscapeAlienFacility;
		}

		// Token: 0x060039DB RID: 14811 RVA: 0x00155D34 File Offset: 0x00153F34
		public override string GetIllustrationPath(TIFactionState faction)
		{
			return TemplateManager.global.illus_alienFacility;
		}

		// Token: 0x17000A01 RID: 2561
		// (get) Token: 0x060039DC RID: 14812 RVA: 0x00155D40 File Offset: 0x00153F40
		public override bool isRegionAlienFacility
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A02 RID: 2562
		// (get) Token: 0x060039DD RID: 14813 RVA: 0x00155D43 File Offset: 0x00153F43
		public override TIRegionAlienFacilityState ref_alienFacility
		{
			get
			{
				return this;
			}
		}

		// Token: 0x060039DE RID: 14814 RVA: 0x00155D48 File Offset: 0x00153F48
		public void InitWithRegionState(TIRegionState region)
		{
			if (!this.gameStateSubjectCreated)
			{
				if (region.template == null)
				{
					return;
				}
				this.templateName = region.template.dataName;
				base.region = region;
				this.built = false;
				this.gameStateSubjectCreated = true;
				if (TemplateManager.global.debug_advancedFactionStart && region.templateName == "RockyMountains")
				{
					this.built = true;
					this.currentHP = 80f;
				}
			}
		}

		// Token: 0x060039DF RID: 14815 RVA: 0x00155DBC File Offset: 0x00153FBC
		public override void PostInitializationInit_4()
		{
			if (TemplateManager.global.debug_advancedFactionStart && this.built)
			{
				GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
				{
					x.SetIntel(this, 1f, null, false);
				});
			}
		}

		// Token: 0x060039E0 RID: 14816 RVA: 0x00155DF0 File Offset: 0x00153FF0
		public void BuildFacility()
		{
			this.built = true;
			this.currentHP = 80f;
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				if (tifactionState.IsAlienProxy || tifactionState.IsAlienFaction)
				{
					tifactionState.SetIntel(this, 1f, null, false);
				}
				else
				{
					tifactionState.SetIntel(this, 0f, null, false);
				}
			}
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { base.region });
		}

		// Token: 0x060039E1 RID: 14817 RVA: 0x00155E7C File Offset: 0x0015407C
		public void SightedByFaction(TIFactionState council)
		{
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { this, base.region });
			if (council.proAlien)
			{
				council.CompleteMilestone(CampaignMilestone.AlienInfrastructureExists);
				if (council.veryProAlien)
				{
					council.CompleteMilestone(CampaignMilestone.AlienDiplomacy);
				}
			}
			if (council.GetObjectivesByStatus(ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTarget == ObjectiveMissionTargetType.Abductions))
			{
				base.region.alienActivity.ActivitySightedByFaction(council, TIFactionState.abductionsMission, null, null, null);
			}
			else if (council.GetObjectivesByStatus(ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTarget == ObjectiveMissionTargetType.EnthrallMission))
			{
				base.region.alienActivity.ActivitySightedByFaction(council, TIFactionState.enthrallPublicMission, null, null, null);
			}
			TINotificationQueueState.LogAlienFacilityDetected(council, base.region.alienFacility);
		}

		// Token: 0x060039E2 RID: 14818 RVA: 0x00155F72 File Offset: 0x00154172
		public override float GetArmyAssaultDefenseScore()
		{
			return 4.5f + ((base.region.terrain == TerrainType.Rugged) ? 1.5f : 0f);
		}

		// Token: 0x060039E3 RID: 14819 RVA: 0x00155F94 File Offset: 0x00154194
		public override string ResolveAssault(TIGameState assaultingState, TIFactionState assaultingFaction, TIMissionOutcome outcome)
		{
			string empty = string.Empty;
			assaultingFaction.CompleteMilestone(CampaignMilestone.AssaultedAlienFacility);
			GameControl.eventManager.TriggerEvent(new TIGameStateAttacking(this), null, new object[] { assaultingState });
			GameControl.eventManager.TriggerEvent(new AlienFacilityDamaged(this), null, new object[] { this });
			if (outcome >= TIMissionOutcome.Success)
			{
				assaultingFaction.CompleteMilestone(CampaignMilestone.DestoyedAlienFacility);
				this.built = false;
				int num = (int)((float)base.region.abductions * TemplateManager.global.abductionsCancelledFactorOnFacilityAssault * (float)((outcome == TIMissionOutcome.CriticalSuccess) ? 2 : 1));
				float num2 = TemplateManager.global.exoticsFromAlienFacilityRaid * TIUtilities.RandomRange(0.75f, 1.25f) * (float)((outcome == TIMissionOutcome.CriticalSuccess) ? 2 : 1);
				TINotificationQueueState.LogAlienFacilityAssaulted(assaultingState, assaultingFaction, this, num2, num);
				if (assaultingFaction != null && (assaultingState.ref_councilor == assaultingState || assaultingState.ref_army == assaultingState))
				{
					foreach (CampaignMilestone campaignMilestone in this.CampaignMilestonesGrantedOnCapture(assaultingFaction, outcome))
					{
						assaultingFaction.CompleteMilestone(campaignMilestone);
					}
					assaultingFaction.AddToCurrentResource(num2, FactionResource.Exotics, false, "Alien Facility Assault");
				}
				if (!assaultingState.isCouncilorState)
				{
					this.ref_faction.GainFactionHate(assaultingFaction, TemplateManager.global.factionHateForDestroyAlienFacility, false, "Alien Facility Assault", true);
				}
				base.region.ConductAbductions(this.ref_faction, -num);
				this.OnDestruction();
				TIFactionState tifactionState = GameStateManager.AlienProxy();
				if (tifactionState != null && tifactionState.GetIntel(this) > 0f)
				{
					GameStateManager.AlienProxy().AddSuspicionForMajorReversal(20f, null);
				}
			}
			else if (outcome == TIMissionOutcome.Failure)
			{
				this.currentHP = Mathf.Clamp(this.currentHP - TIUtilities.RandomFloatValue() * 10f, 1f, 80f);
			}
			return empty;
		}

		// Token: 0x060039E4 RID: 14820 RVA: 0x00156164 File Offset: 0x00154364
		public bool Bombed(TISpaceFleetState fleet, float damageValue)
		{
			this.currentHP -= damageValue;
			if (damageValue > 0f)
			{
				GameControl.eventManager.TriggerEvent(new AlienFacilityDamaged(this), null, new object[] { this });
			}
			if (this.currentHP <= 0f)
			{
				this.built = false;
				TINotificationQueueState.LogAlienFacilityBombed(fleet, this);
				this.OnDestruction();
				this.ref_faction.GainFactionHate(fleet.faction, TemplateManager.global.factionHateForDestroyAlienFacility, false, "Alien Facility Bombed", true);
				return true;
			}
			return false;
		}

		// Token: 0x060039E5 RID: 14821 RVA: 0x001561E8 File Offset: 0x001543E8
		public void OnDestruction()
		{
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ExpireIntel(this, true);
			}
			foreach (TIArmyState tiarmyState in base.region.armies)
			{
				using (List<OperationData>.Enumerator enumerator2 = tiarmyState.currentOperations.ToList<OperationData>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.target == this)
						{
							tiarmyState.ClearOperations();
						}
					}
				}
			}
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { base.region });
		}

		// Token: 0x060039E6 RID: 14822 RVA: 0x001562D0 File Offset: 0x001544D0
		public override List<CampaignMilestone> CampaignMilestonesGrantedOnCapture(TIFactionState capturingFaction, TIMissionOutcome outcome)
		{
			List<CampaignMilestone> list = new List<CampaignMilestone>();
			if (outcome >= TIMissionOutcome.Success)
			{
				list.Add(CampaignMilestone.AccessAlienTech);
				if (TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, this.ref_faction))
				{
					list.Add(CampaignMilestone.AccessSalamanderCorpus);
					if (capturingFaction.CanDetectAlien && TIUtilities.RandomFloatValue() <= 0.5f)
					{
						list.Add(CampaignMilestone.AccessHydraCorpus);
					}
				}
			}
			if (outcome >= TIMissionOutcome.CriticalSuccess && TIEffectsState.CheckForAnyEffectInContext(Context.ManyAliensOnEarth, this.ref_faction))
			{
				list.Add(CampaignMilestone.AccessLiveSalamander);
				if (capturingFaction.CanCaptureAlien && TIUtilities.RandomFloatValue() <= 0.5f)
				{
					list.Add(CampaignMilestone.AccessLiveHydra);
				}
				else if (capturingFaction.CanDetectAlien)
				{
					list.Add(CampaignMilestone.AccessHydraCorpus);
				}
			}
			return list;
		}

		// Token: 0x04002570 RID: 9584
		private const int maxHP = 80;

		// Token: 0x04002571 RID: 9585
		public float currentHP;
	}
}
