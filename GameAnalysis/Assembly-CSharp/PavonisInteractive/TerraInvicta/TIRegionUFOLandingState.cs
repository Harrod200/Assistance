using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Tasks;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000779 RID: 1913
	public class TIRegionUFOLandingState : TIRegionAlienAssetState
	{
		// Token: 0x17000A72 RID: 2674
		// (get) Token: 0x06003B12 RID: 15122 RVA: 0x0015C870 File Offset: 0x0015AA70
		// (set) Token: 0x06003B13 RID: 15123 RVA: 0x0015C878 File Offset: 0x0015AA78
		public bool landingPresent { get; private set; }

		// Token: 0x06003B14 RID: 15124 RVA: 0x0015C881 File Offset: 0x0015AA81
		public override string GetIconResourcePath(TIFactionState faction)
		{
			return TemplateManager.global.pathGeoscapeUFOLanding;
		}

		// Token: 0x06003B15 RID: 15125 RVA: 0x0015C88D File Offset: 0x0015AA8D
		public override string GetIllustrationPath(TIFactionState faction)
		{
			return TemplateManager.global.illus_landedUFO;
		}

		// Token: 0x17000A73 RID: 2675
		// (get) Token: 0x06003B16 RID: 15126 RVA: 0x0015C899 File Offset: 0x0015AA99
		public override bool isRegionLandedUFO
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A74 RID: 2676
		// (get) Token: 0x06003B17 RID: 15127 RVA: 0x0015C89C File Offset: 0x0015AA9C
		public override TIRegionUFOLandingState ref_UFOLanding
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06003B18 RID: 15128 RVA: 0x0015C89F File Offset: 0x0015AA9F
		public void InitWithRegionState(TIRegionState region)
		{
			if (!this.gameStateSubjectCreated)
			{
				this.templateName = region.template.dataName;
				base.region = region;
				this.gameStateSubjectCreated = true;
			}
		}

		// Token: 0x06003B19 RID: 15129 RVA: 0x0015C8C8 File Offset: 0x0015AAC8
		public override void PostInitializationInit_4()
		{
			if (this.landingPresent)
			{
				if (this.deployingArmy)
				{
					GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnAlienArmyDeployed), this.deployArmyEvent, null, true, false);
				}
				if (this.supportingArmyBuildup)
				{
					GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CompleteArmyDeployment), this.expireLandingEvent, null, true, false);
				}
			}
		}

		// Token: 0x06003B1A RID: 15130 RVA: 0x0015C92B File Offset: 0x0015AB2B
		public override bool Extant()
		{
			return this.landingPresent;
		}

		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06003B1B RID: 15131 RVA: 0x0015C933 File Offset: 0x0015AB33
		public TINationState alienNation
		{
			get
			{
				return GameStateManager.AlienNation();
			}
		}

		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x06003B1C RID: 15132 RVA: 0x0015C93C File Offset: 0x0015AB3C
		public string deployArmyEvent
		{
			get
			{
				return new StringBuilder("Aliens Deploy Army").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x06003B1D RID: 15133 RVA: 0x0015C974 File Offset: 0x0015AB74
		public string expireLandingEvent
		{
			get
			{
				return new StringBuilder("Expire Alien Landing").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x06003B1E RID: 15134 RVA: 0x0015C9AC File Offset: 0x0015ABAC
		public void TriggerLanding(float overrideTime = -1f)
		{
			this.landingPresent = true;
			this.currentHP = 1200f;
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetIntel(this, 1f, null, false);
			}
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { base.region });
			TIDateTime tidateTime = TITimeState.Now();
			if (overrideTime <= 0f)
			{
				tidateTime.AddDays((float)TemplateManager.global.daysToFieldArmyFromUFO);
			}
			else
			{
				tidateTime.AddDays(overrideTime);
			}
			TITimeEvent.CreateNewTimeEvent(tidateTime, this, null, null, this.deployArmyEvent, true, false, TITimeQueueRepeatType.None, 1, true, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnAlienArmyDeployed), this.deployArmyEvent, null, true, false);
			GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
			{
				x.CompleteMilestone(CampaignMilestone.AliensLandArmy);
			});
			TINotificationQueueState.LogUFOLanding(base.region);
			this.deployingArmy = true;
			if (!base.region.nation.alienNation && !base.region.nation.allies.Contains(this.alienNation) && !base.region.nation.wars.Contains(this.alienNation))
			{
				if (!this.alienNation.extant)
				{
					this.alienNation.SetCapital(base.region);
				}
				GameStateManager.AlienNation().DeclareFullWar(GameStateManager.AlienFaction(), base.region.nation);
			}
		}

		// Token: 0x06003B1F RID: 15135 RVA: 0x0015CB38 File Offset: 0x0015AD38
		public void OnAlienArmyDeployed(TimeEventStart e)
		{
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddDays((float)TemplateManager.global.daysToPrepareFullArmyFromUFO);
			TIAlienArmyState tialienArmyState = null;
			if (e.eventObject == this && this.Extant())
			{
				for (int i = 0; i < TemplateManager.global.alienArmiesFromLanding; i++)
				{
					tialienArmyState = GameStateManager.CreateNewGameState<TIAlienArmyState>();
					tialienArmyState.SpawnArmy(base.region);
				}
				TINotificationQueueState.LogAlienArmySpawned(tialienArmyState);
			}
			if (!base.region.nation.alienNation && !base.region.nation.allies.Contains(this.alienNation) && !base.region.nation.wars.Contains(this.alienNation))
			{
				if (!this.alienNation.extant)
				{
					this.alienNation.SetCapital(base.region);
				}
				GameStateManager.AlienNation().DeclareFullWar(GameStateManager.AlienFaction(), base.region.nation);
			}
			else
			{
				foreach (TINationState tinationState in base.region.nation.allies)
				{
					if (!tinationState.wars.Contains(GameStateManager.AlienNation()))
					{
						TIWarState tiwarState = base.region.nation.findWarsWith(GameStateManager.AlienNation()).FirstOrDefault<TIWarState>();
						if (tiwarState != null)
						{
							TIFactionState executiveFaction = tinationState.executiveFaction;
							if (executiveFaction != null && !executiveFaction.permanentAlly(GameStateManager.AlienFaction()))
							{
								tinationState.JoinWar(tinationState.executiveFaction, base.region.nation, tiwarState);
							}
						}
					}
				}
			}
			this.deployingArmy = false;
			this.supportingArmyBuildup = true;
			TITimeEvent.CreateNewTimeEvent(tidateTime, this, null, null, this.expireLandingEvent, true, false, TITimeQueueRepeatType.None, 1, true, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CompleteArmyDeployment), this.expireLandingEvent, null, true, false);
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnAlienArmyDeployed), this.deployArmyEvent);
		}

		// Token: 0x06003B20 RID: 15136 RVA: 0x0015CD48 File Offset: 0x0015AF48
		public void CompleteArmyDeployment(TimeEventStart e)
		{
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CompleteArmyDeployment), this.expireLandingEvent);
			this.supportingArmyBuildup = false;
			this.ExpireUFOLandingForAll();
		}

		// Token: 0x06003B21 RID: 15137 RVA: 0x0015CD74 File Offset: 0x0015AF74
		public void ExpireUFOLandingForAll()
		{
			this.landingPresent = false;
			foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
			{
				tifactionState.ExpireIntel(this, true);
			}
			foreach (TIArmyState tiarmyState in base.region.armies)
			{
				using (List<OperationData>.Enumerator enumerator3 = tiarmyState.currentOperations.ToList<OperationData>().GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						if (enumerator3.Current.target == this)
						{
							tiarmyState.ClearOperations();
						}
					}
				}
			}
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { base.region });
		}

		// Token: 0x06003B22 RID: 15138 RVA: 0x0015CE80 File Offset: 0x0015B080
		public override float GetArmyAssaultDefenseScore()
		{
			return 12f + ((base.region.terrain == TerrainType.Rugged) ? 3f : 0f);
		}

		// Token: 0x06003B23 RID: 15139 RVA: 0x0015CEA4 File Offset: 0x0015B0A4
		public override string ResolveAssault(TIGameState assaultingState, TIFactionState assaultingFaction, TIMissionOutcome outcome)
		{
			string empty = string.Empty;
			GameControl.eventManager.TriggerEvent(new TIGameStateAttacking(this), null, new object[] { assaultingState });
			GameControl.eventManager.TriggerEvent(new AlienLandingDamaged(this), null, new object[] { this });
			if (outcome >= TIMissionOutcome.Success)
			{
				TINotificationQueueState.LogUFOLandingAssaulted(assaultingState, assaultingFaction, this);
				if (assaultingFaction != null && (assaultingState.ref_councilor == assaultingState || assaultingState.ref_army == assaultingState))
				{
					foreach (CampaignMilestone campaignMilestone in this.CampaignMilestonesGrantedOnCapture(assaultingFaction, outcome))
					{
						assaultingFaction.CompleteMilestone(campaignMilestone);
					}
				}
				if (!assaultingState.isCouncilorState)
				{
					this.ref_faction.GainFactionHate(assaultingState.ref_faction, TemplateManager.global.factionHateForDestroyLandedUFO, false, "Landed UFO Assaulted", true);
				}
				if (assaultingFaction != null)
				{
					AIDailyFactionPlanner.AIReaction(AIReactionEvent.AlienCarrierDestroyed, this.ref_faction, assaultingFaction);
				}
				this.ExpireUFOLandingForAll();
				if (this.alienNation.extant)
				{
					return empty;
				}
				if (this.alienNation.armies.Count<TIArmyState>((TIArmyState x) => x.AlienRegularArmy) != 0)
				{
					return empty;
				}
				using (List<TIWarState>.Enumerator enumerator2 = this.alienNation.currentWarStates.ToList<TIWarState>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIWarState tiwarState = enumerator2.Current;
						this.alienNation.WhitePeace(this.ref_faction, tiwarState, false);
					}
					return empty;
				}
			}
			if (outcome == TIMissionOutcome.Failure)
			{
				this.currentHP = Mathf.Clamp(this.currentHP - TIUtilities.RandomFloatValue() * 10f, 1f, 1200f);
			}
			return empty;
		}

		// Token: 0x06003B24 RID: 15140 RVA: 0x0015D078 File Offset: 0x0015B278
		public bool Bombed(TISpaceFleetState bombingState, float damageValue)
		{
			this.currentHP -= damageValue;
			if (damageValue > 0f)
			{
				GameControl.eventManager.TriggerEvent(new AlienLandingDamaged(this), null, new object[] { this });
			}
			if (this.currentHP <= 0f)
			{
				this.ref_faction.GainFactionHate(bombingState.faction, TemplateManager.global.factionHateForDestroyLandedUFO, false, "UFO Landing Bombed", true);
				AIDailyFactionPlanner.AIReaction(AIReactionEvent.AlienCarrierDestroyed, this.ref_faction, bombingState.faction);
				TINotificationQueueState.LogUFOLandingBombed(bombingState, this);
				this.ExpireUFOLandingForAll();
				if (!this.alienNation.extant)
				{
					if (this.alienNation.armies.Count<TIArmyState>((TIArmyState x) => x.AlienRegularArmy) == 0)
					{
						foreach (TIWarState tiwarState in this.alienNation.currentWarStates.ToList<TIWarState>())
						{
							this.alienNation.WhitePeace(this.ref_faction, tiwarState, false);
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06003B25 RID: 15141 RVA: 0x0015D1A8 File Offset: 0x0015B3A8
		public override List<CampaignMilestone> CampaignMilestonesGrantedOnCapture(TIFactionState assaultingFaction, TIMissionOutcome outcome = TIMissionOutcome.Success)
		{
			List<CampaignMilestone> list = new List<CampaignMilestone>();
			if (outcome >= TIMissionOutcome.Success)
			{
				list.Add(CampaignMilestone.AccessAlienShip);
				list.Add(CampaignMilestone.AccessAlienTech);
				list.Add(CampaignMilestone.AccessGriffinCorpus);
				if (assaultingFaction.CanDetectAlien)
				{
					list.Add(CampaignMilestone.AccessHydraCorpus);
				}
			}
			if (outcome >= TIMissionOutcome.CriticalSuccess)
			{
				list.Add(CampaignMilestone.AccessLiveGriffin);
				if (assaultingFaction.CanCaptureAlien)
				{
					list.Add(CampaignMilestone.AccessLiveHydra);
				}
			}
			return list;
		}

		// Token: 0x040025B5 RID: 9653
		[SerializeField]
		private bool deployingArmy;

		// Token: 0x040025B6 RID: 9654
		[SerializeField]
		private bool supportingArmyBuildup;

		// Token: 0x040025B7 RID: 9655
		private const float maxHP = 1200f;

		// Token: 0x040025B8 RID: 9656
		public float currentHP;
	}
}
