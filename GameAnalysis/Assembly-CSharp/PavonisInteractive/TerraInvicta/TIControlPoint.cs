using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000767 RID: 1895
	public class TIControlPoint : TIGameState
	{
		// Token: 0x170008D8 RID: 2264
		// (get) Token: 0x06003653 RID: 13907 RVA: 0x0013BC8A File Offset: 0x00139E8A
		// (set) Token: 0x06003654 RID: 13908 RVA: 0x0013BC92 File Offset: 0x00139E92
		public TINationState nation { get; private set; }

		// Token: 0x170008D9 RID: 2265
		// (get) Token: 0x06003655 RID: 13909 RVA: 0x0013BC9B File Offset: 0x00139E9B
		// (set) Token: 0x06003656 RID: 13910 RVA: 0x0013BCA3 File Offset: 0x00139EA3
		public TIFactionState faction { get; private set; }

		// Token: 0x170008DA RID: 2266
		// (get) Token: 0x06003657 RID: 13911 RVA: 0x0013BCAC File Offset: 0x00139EAC
		// (set) Token: 0x06003658 RID: 13912 RVA: 0x0013BCB4 File Offset: 0x00139EB4
		public bool benefitsDisabled { get; private set; }

		// Token: 0x170008DB RID: 2267
		// (get) Token: 0x06003659 RID: 13913 RVA: 0x0013BCBD File Offset: 0x00139EBD
		// (set) Token: 0x0600365A RID: 13914 RVA: 0x0013BCC5 File Offset: 0x00139EC5
		public bool defended { get; private set; }

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x0600365B RID: 13915 RVA: 0x0013BCCE File Offset: 0x00139ECE
		// (set) Token: 0x0600365C RID: 13916 RVA: 0x0013BCD6 File Offset: 0x00139ED6
		public TIDateTime crackdownExpiration { get; private set; }

		// Token: 0x170008DD RID: 2269
		// (get) Token: 0x0600365D RID: 13917 RVA: 0x0013BCDF File Offset: 0x00139EDF
		// (set) Token: 0x0600365E RID: 13918 RVA: 0x0013BCE7 File Offset: 0x00139EE7
		public TIDateTime defendExpiration { get; private set; }

		// Token: 0x170008DE RID: 2270
		// (get) Token: 0x0600365F RID: 13919 RVA: 0x0013BCF0 File Offset: 0x00139EF0
		// (set) Token: 0x06003660 RID: 13920 RVA: 0x0013BCF8 File Offset: 0x00139EF8
		public ControlPointType controlPointType { get; private set; }

		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x06003661 RID: 13921 RVA: 0x0013BD01 File Offset: 0x00139F01
		public override bool isControlPointState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x06003662 RID: 13922 RVA: 0x0013BD04 File Offset: 0x00139F04
		public override TIFactionState ref_faction
		{
			get
			{
				return this.faction;
			}
		}

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06003663 RID: 13923 RVA: 0x0013BD0C File Offset: 0x00139F0C
		public override TINationState ref_nation
		{
			get
			{
				return this.nation;
			}
		}

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06003664 RID: 13924 RVA: 0x0013BD14 File Offset: 0x00139F14
		public override TIRegionState ref_region
		{
			get
			{
				return this.nation.capital;
			}
		}

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06003665 RID: 13925 RVA: 0x0013BD21 File Offset: 0x00139F21
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.ref_region.spaceBody;
			}
		}

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x06003666 RID: 13926 RVA: 0x0013BD2E File Offset: 0x00139F2E
		public override TIControlPoint ref_controlPoint
		{
			get
			{
				return this;
			}
		}

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06003667 RID: 13927 RVA: 0x0013BD31 File Offset: 0x00139F31
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06003668 RID: 13928 RVA: 0x0013BD34 File Offset: 0x00139F34
		public bool owned
		{
			get
			{
				return this.faction != null;
			}
		}

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06003669 RID: 13929 RVA: 0x0013BD42 File Offset: 0x00139F42
		public bool executive
		{
			get
			{
				return this.positionInNation == this.nation.maxControlPointIndex;
			}
		}

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x0600366A RID: 13930 RVA: 0x0013BD57 File Offset: 0x00139F57
		public string description
		{
			get
			{
				return this.controlPointTypeDisplayName;
			}
		}

		// Token: 0x0600366B RID: 13931 RVA: 0x0013BD5F File Offset: 0x00139F5F
		public bool EnemyFactionControlPoint(TIFactionState otherFaction)
		{
			return this.owned && !this.faction.permanentAlly(otherFaction);
		}

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x0600366C RID: 13932 RVA: 0x0013BD7A File Offset: 0x00139F7A
		public bool nextOpenControlPoint
		{
			get
			{
				return this.nation.FirstNativeControlPoint() == this;
			}
		}

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x0600366D RID: 13933 RVA: 0x0013BD8D File Offset: 0x00139F8D
		public bool ExecutiveImmunity
		{
			get
			{
				return this.executive && this.nation.numControlPoints > 1;
			}
		}

		// Token: 0x0600366E RID: 13934 RVA: 0x0013BDA7 File Offset: 0x00139FA7
		public bool CanBeAttacked(TIFactionState faction)
		{
			return !this.executive || this.nation.maxControlPointIndex == 0 || this.nation.FactionsWithControlPoint.Contains(faction);
		}

		// Token: 0x0600366F RID: 13935 RVA: 0x0013BDD4 File Offset: 0x00139FD4
		public bool CanBeEnthralled()
		{
			return (!this.executive || this.nation.maxControlPointIndex == 0 || this.nation.FactionsWithControlPoint.Contains(GameStateManager.AlienProxy())) && !this.faction.permanentAlly(GameStateManager.AlienFaction());
		}

		// Token: 0x06003670 RID: 13936 RVA: 0x0013BE24 File Offset: 0x0013A024
		public bool CanBeTerrorized()
		{
			return (!this.executive || this.nation.maxControlPointIndex == 0) && (this.faction == null || (!this.faction.IsAlienProxy && !this.faction.isAlienAppeaser && !this.faction.IsAlienFaction));
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x06003671 RID: 13937 RVA: 0x0013BE82 File Offset: 0x0013A082
		public List<TIArmyState> armies
		{
			get
			{
				return this.nation.armies.Where<TIArmyState>((TIArmyState army) => army.controlPointIdx == this.positionInNation).ToList<TIArmyState>();
			}
		}

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x06003672 RID: 13938 RVA: 0x0013BEA5 File Offset: 0x0013A0A5
		public int numArmies
		{
			get
			{
				return this.armies.Count;
			}
		}

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x06003673 RID: 13939 RVA: 0x0013BEB2 File Offset: 0x0013A0B2
		public FactionIdeology ideology
		{
			get
			{
				TIFactionState faction = this.faction;
				if (faction == null)
				{
					return GameStateManager.UndecidedIdeology().ideology;
				}
				return faction.ideology.ideology;
			}
		}

		// Token: 0x06003674 RID: 13940 RVA: 0x0013BED4 File Offset: 0x0013A0D4
		public void InitWithNationState(TINationState nation, int position)
		{
			if (!this.gameStateSubjectCreated)
			{
				TINationTemplate template = nation.template;
				this.templateName = template.dataName;
				this.positionInNation = position;
				this.nation = nation;
				this.controlPointPriorities = new Dictionary<PriorityType, int>(Enums.PriorityTypes.Length);
				for (int i = 0; i < Enums.PriorityTypes.Length; i++)
				{
					this.SetControlPointPriority(Enums.PriorityTypes[i], 0, true, true, false);
				}
				this.SetControlPointType();
			}
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}

		// Token: 0x06003675 RID: 13941 RVA: 0x0013BF58 File Offset: 0x0013A158
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			TIFactionState initialFaction = this.nation.template.initialFaction;
			if (initialFaction != null)
			{
				this.SetFaction(initialFaction, true);
			}
			if (TIGlobalValuesState.Customizations.usingCustomizations && TIGlobalValuesState.Customizations.customFactionStartingNationGroup.Count > 0)
			{
				foreach (KeyValuePair<string, int> keyValuePair in TIGlobalValuesState.Customizations.customFactionStartingNationGroup)
				{
					if (this.nation.template.group == keyValuePair.Value)
					{
						this.SetFaction(GameStateManager.FindByTemplate<TIFactionState>(keyValuePair.Key, false), true);
						this.ResolveDefendControlPointEffect(TIGlobalConfig.globalConfig.defendInterestDistributableDuration_days / this.nation.numControlPoints);
					}
				}
			}
			if (TemplateManager.global.debug_advancedFactionStart)
			{
				if (this.nation.templateName == "USA")
				{
					this.SetFaction(GameStateManager.FindByTemplate<TIFactionState>("ExploitCouncil", false), false);
				}
				if (this.nation.templateName == "EUA" || this.nation.templateName == "DEU")
				{
					this.SetFaction(GameStateManager.FindByTemplate<TIFactionState>("CooperateCouncil", false), false);
				}
				if (this.nation.templateName == "RUS" || this.nation.templateName == "BLR" || this.nation.templateName == "KAZ")
				{
					this.SetFaction(GameStateManager.FindByTemplate<TIFactionState>("AppeaseCouncil", false), false);
				}
				if (this.nation.templateName == "CHN")
				{
					this.SetFaction(GameStateManager.FindByTemplate<TIFactionState>("SubmitCouncil", false), false);
				}
				if (this.nation.templateName == "JPN" || this.nation.templateName == "KOR" || this.nation.templateName == "TWN" || this.nation.templateName == "SGP")
				{
					this.SetFaction(GameStateManager.FindByTemplate<TIFactionState>("DestroyCouncil", false), false);
				}
				if (this.nation.templateName == "GBR" || this.nation.templateName == "AUS" || this.nation.templateName == "CAN" || this.nation.templateName == "NZL")
				{
					this.SetFaction(GameStateManager.FindByTemplate<TIFactionState>("ResistCouncil", false), false);
				}
				if (this.nation.templateName == "IND" || this.nation.templateName == "IDN")
				{
					this.SetFaction(GameStateManager.FindByTemplate<TIFactionState>("EscapeCouncil", false), false);
				}
			}
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x06003676 RID: 13942 RVA: 0x0013C250 File Offset: 0x0013A450
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (this.nation == null)
			{
				this.RemoveControlPointFromNation();
				return;
			}
			if (this.controlPointPriorities == null)
			{
				this.controlPointPriorities = new Dictionary<PriorityType, int>(Enums.PriorityTypes.Length);
				for (int i = 0; i < Enums.PriorityTypes.Length; i++)
				{
					this.controlPointPriorities[Enums.PriorityTypes[i]] = Mathf.Clamp(0, 0, 3);
				}
				return;
			}
			foreach (PriorityType priorityType in Enums.PriorityTypes)
			{
				if (!this.controlPointPriorities.ContainsKey(priorityType))
				{
					this.controlPointPriorities.Add(priorityType, 0);
				}
			}
		}

		// Token: 0x06003677 RID: 13943 RVA: 0x0013C300 File Offset: 0x0013A500
		public override void PostVisualizerCreationInit_6()
		{
			if (this.defended && this.defendExpiration < this.gameTime.currentTime)
			{
				this.EndControlPointDefense();
			}
			if (this.benefitsDisabled && this.crackdownExpiration < this.gameTime.currentTime)
			{
				this.EnableBenefits();
			}
			this.RepairOwnership();
			this.SetDisplayName();
			this.RecordAndFixControlPointValues(false);
		}

		// Token: 0x06003678 RID: 13944 RVA: 0x0013C36C File Offset: 0x0013A56C
		private void RepairOwnership()
		{
			foreach (TIFactionState tifactionState in from x in GameStateManager.AllFactions()
				where x.controlPoints.Contains(this)
				select x)
			{
				if (tifactionState != this.faction)
				{
					if (tifactionState != null)
					{
						tifactionState.controlPoints.Remove(this);
					}
					Log.Error("CP owner record out of sync with faction: " + ((tifactionState != null) ? tifactionState.displayName : null), Array.Empty<object>());
				}
			}
		}

		// Token: 0x06003679 RID: 13945 RVA: 0x0013C400 File Offset: 0x0013A600
		public void SetFaction(TIFactionState newFaction, bool newCampaign = false)
		{
			if (newFaction != this.faction)
			{
				TIFactionState faction = this.faction;
				if (faction != null)
				{
					faction.controlPoints.Remove(this);
					if (!faction.lostControlPoints.ContainsKey(this))
					{
						faction.lostControlPoints.Add(this, TITimeState.Now());
					}
					else
					{
						faction.lostControlPoints[this] = TITimeState.Now();
					}
					faction.ValidateAllOrgs(false);
				}
				this.faction = newFaction;
				GameControl.eventManager.TriggerEvent(new NationControlPointOwnerChanged(this.nation, this), null, new object[]
				{
					this,
					this.nation.capital,
					faction
				}.Where<object>((object x) => x != null).ToArray<object>());
				TIFactionState faction2 = this.faction;
				if (faction2 != null)
				{
					faction2.controlPoints.Add(this);
				}
			}
			if (!newCampaign)
			{
				this.RecordAndFixControlPointValues(false);
				AIDailyFactionPlanner.AIReaction(AIReactionEvent.CheckForCPTrouble, this, newFaction);
			}
		}

		// Token: 0x0600367A RID: 13946 RVA: 0x0013C504 File Offset: 0x0013A704
		public List<TIArmyState> RemoveControlPointFromNation()
		{
			List<TIArmyState> armies = this.armies;
			foreach (TIMissionState timissionState in GameStateManager.AllActiveMissions())
			{
				if (timissionState.target == this)
				{
					timissionState.ResolveMission(TIMissionState.AbortReason.ControlPointRemoved, "");
				}
			}
			this.SetFaction(null, false);
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				if (tifactionState.lostControlPoints.ContainsKey(this))
				{
					tifactionState.lostControlPoints.Remove(this);
				}
			}
			this.nation.RemoveControlPointFromNation(this);
			this.nation = null;
			base.ArchiveState(true);
			GameStateManager.RemoveGameState<TIControlPoint>(base.ID, false);
			return armies;
		}

		// Token: 0x0600367B RID: 13947 RVA: 0x0013C5DC File Offset: 0x0013A7DC
		public void SetControlPointType()
		{
			int num = this.nation.numControlPoints - this.positionInNation - 1;
			if (this.nation.alienNation)
			{
				this.controlPointType = ControlPointType.Alien;
				return;
			}
			switch (num)
			{
			case 0:
				this.controlPointType = ControlPointType.Executive;
				return;
			case 1:
				if (this.nation.democracy >= 5f)
				{
					this.controlPointType = ControlPointType.Legislature;
					return;
				}
				if (this.nation.education >= 6f && this.nation.cohesion >= 7f)
				{
					this.controlPointType = ControlPointType.TheParty;
					return;
				}
				if (this.nation.education >= 6f && this.nation.cohesion < 7f)
				{
					this.controlPointType = ControlPointType.Oligarchs;
					return;
				}
				this.controlPointType = ControlPointType.Aristocracy;
				return;
			case 2:
				if (this.nation.democracy < 5f)
				{
					this.controlPointType = ControlPointType.SecurityApparatus;
					return;
				}
				if (this.nation.education >= 7f)
				{
					this.controlPointType = ControlPointType.MassMedia;
					return;
				}
				this.controlPointType = ControlPointType.Religion;
				return;
			case 3:
				if (this.nation.democracy < 4f)
				{
					this.controlPointType = ControlPointType.NationalIndustries;
					return;
				}
				if ((double)this.nation.inequality < 3.5 && ((this.nation.perCapitaGDP > 10000f) & (this.nation.education > 7f)))
				{
					this.controlPointType = ControlPointType.TradeUnions;
					return;
				}
				this.controlPointType = ControlPointType.Corporations;
				return;
			case 4:
				if (this.nation.cohesion > 6f)
				{
					this.controlPointType = ControlPointType.Bureaucracy;
					return;
				}
				if (this.nation.cohesion > 3f)
				{
					this.controlPointType = ControlPointType.RegionalAuthorities;
					return;
				}
				if (this.nation.cohesion > 1f || this.nation.unrest < 7f)
				{
					this.controlPointType = ControlPointType.IdentityBlocs;
					return;
				}
				this.controlPointType = ControlPointType.Warlords;
				return;
			case 5:
				if (this.nation.democracy > 9f && this.nation.education > 9f)
				{
					this.controlPointType = ControlPointType.KnowledgeSector;
					return;
				}
				if (this.nation.cohesion > 6f && this.nation.enemies.Any<TINationState>((TINationState x) => x.numControlPoints >= this.nation.numControlPoints))
				{
					this.controlPointType = ControlPointType.DefenseSector;
					return;
				}
				if (this.nation.democracy > 6f && this.nation.education > 7f)
				{
					this.controlPointType = ControlPointType.FinancialSector;
					return;
				}
				if (this.nation.resourceRegions > 0)
				{
					this.controlPointType = ControlPointType.ExtractiveSector;
					return;
				}
				this.controlPointType = ControlPointType.AgriculturalSector;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600367C RID: 13948 RVA: 0x0013C87A File Offset: 0x0013AA7A
		public void SetDisplayName()
		{
			this.displayName = Loc.T("UI.Nation.CP.FullName", new object[]
			{
				this.nation.displayName,
				this.controlPointTypeDisplayName
			});
		}

		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x0600367D RID: 13949 RVA: 0x0013C8AC File Offset: 0x0013AAAC
		public string controlPointTypeDisplayName
		{
			get
			{
				return Loc.T(new StringBuilder("UI.Nation.CP.").Append(this.controlPointType.ToString()).ToString());
			}
		}

		// Token: 0x0600367E RID: 13950 RVA: 0x0013C8E6 File Offset: 0x0013AAE6
		public static TIDateTime FindMissionPhaseAfter(TIDateTime inputDate)
		{
			return GameStateManager.FindByTemplate<TITimeEvent>("CouncilorMissionUpdate", false).GetNextEventTime(inputDate);
		}

		// Token: 0x0600367F RID: 13951 RVA: 0x0013C8FC File Offset: 0x0013AAFC
		public TIDateTime ResolveCrackdownEffect(int duration_months, TIFactionState crackingFaction, bool voluntary = false, bool skipLogging = false, float hate = 0f)
		{
			this.EndControlPointDefense();
			if (this.benefitsDisabled && TITimeState.Now() > this.crackdownExpiration)
			{
				this.EnableBenefits();
			}
			if (this.nation.alienNation)
			{
				return null;
			}
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddMonths(TemplateManager.global.selfDisableControlPointDuration_months);
			TIDateTime tidateTime2;
			if (this.benefitsDisabled && (!voluntary || this.crackdownExpiration >= tidateTime))
			{
				tidateTime2 = this.crackdownExpiration;
			}
			else
			{
				tidateTime2 = TITimeState.Now();
			}
			tidateTime2.AddMonths(duration_months);
			this.SetCrackdownExpiry(tidateTime2);
			this.benefitsDisabled = true;
			TIFactionState faction = this.faction;
			if (faction != null)
			{
				faction.SetResourceIncomeDataDirty(TINationState.NationalResources);
			}
			GameControl.eventManager.TriggerEvent(new ControlPointDataUpdated(this), null, new object[]
			{
				this.nation,
				this.nation.capital
			});
			if (!voluntary || skipLogging)
			{
				TINotificationQueueState.LogMyControlPointCrackedDown(this, tidateTime2, crackingFaction, hate);
			}
			return this.crackdownExpiration;
		}

		// Token: 0x06003680 RID: 13952 RVA: 0x0013C9F0 File Offset: 0x0013ABF0
		public void ReenableBenefits()
		{
			TIFactionState faction = this.faction;
			bool flag = faction != null && faction.permaAbandonedNations.Contains(this.nation);
			if (this.benefitsDisabled && this.owned)
			{
				if (flag)
				{
					this.nation.SelfDisableControlPoints(this.faction);
				}
				else
				{
					TINotificationQueueState.LogCrackdownExpires(this);
				}
				AIDailyFactionPlanner.AIReaction(AIReactionEvent.CheckForCPTrouble, this, this.faction);
			}
			if (!flag)
			{
				this.EnableBenefits();
			}
		}

		// Token: 0x06003681 RID: 13953 RVA: 0x0013CA60 File Offset: 0x0013AC60
		public void EnableBenefits()
		{
			if (this.benefitsDisabled)
			{
				this.benefitsDisabled = false;
				TIFactionState faction = this.faction;
				if (faction != null)
				{
					faction.SetResourceIncomeDataDirty(TINationState.NationalResources);
				}
				GameControl.eventManager.TriggerEvent(new NationDataUpdated(this.nation), null, new object[] { this.nation });
				GameControl.eventManager.TriggerEvent(new ControlPointDataUpdated(this), null, new object[]
				{
					this.nation,
					this.nation.capital
				});
			}
			this.crackdownExpiration = null;
		}

		// Token: 0x06003682 RID: 13954 RVA: 0x0013CAEC File Offset: 0x0013ACEC
		private void SetCrackdownExpiry(TIDateTime expiry)
		{
			this.crackdownExpiration = TIControlPoint.FindMissionPhaseAfter(expiry);
			this.crackdownExpiration.AddSeconds(-60.0);
		}

		// Token: 0x06003683 RID: 13955 RVA: 0x0013CB10 File Offset: 0x0013AD10
		public string ResolveDefendControlPointEffect(int duration_days)
		{
			if (this.defended && TITimeState.Now() >= this.defendExpiration)
			{
				this.EndControlPointDefense();
			}
			TIDateTime tidateTime;
			if (!this.defended || this.defendExpiration == null)
			{
				tidateTime = TITimeState.Now();
			}
			else
			{
				tidateTime = this.defendExpiration;
			}
			this.defended = true;
			tidateTime.AddDays((float)duration_days);
			TITimeEvent titimeEvent = GameStateManager.FindByTemplate<TITimeEvent>("CouncilorMissionUpdate", false);
			this.defendExpiration = titimeEvent.GetNextEventTime(tidateTime);
			this.defendExpiration.AddSeconds(-60.0);
			GameControl.eventManager.TriggerEvent(new ControlPointDataUpdated(this), null, new object[]
			{
				this.nation,
				this.nation.capital
			});
			return this.defendExpiration.ToCustomDateString();
		}

		// Token: 0x06003684 RID: 13956 RVA: 0x0013CBD8 File Offset: 0x0013ADD8
		public void ExpireDefense()
		{
			if (this.owned && this.defended)
			{
				if (this.nation.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.faction == this.faction && x.defendExpiration == this.defendExpiration).ToList<TIControlPoint>().MaxBy<TIControlPoint, int>((TIControlPoint x) => x.positionInNation) == this)
				{
					TINotificationQueueState.LogControlPointDefenseExpires(this);
				}
			}
			this.EndControlPointDefense();
		}

		// Token: 0x06003685 RID: 13957 RVA: 0x0013CC50 File Offset: 0x0013AE50
		public void EndControlPointDefense()
		{
			if (this.defended)
			{
				this.defended = false;
				GameControl.eventManager.TriggerEvent(new ControlPointDataUpdated(this), null, new object[]
				{
					this.nation,
					this.nation.capital
				});
			}
			this.defendExpiration = null;
		}

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06003686 RID: 13958 RVA: 0x0013CCA1 File Offset: 0x0013AEA1
		public float BaselineMaintenanceCost
		{
			get
			{
				return this.nation.ControlPointMaintenanceCost;
			}
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06003687 RID: 13959 RVA: 0x0013CCAE File Offset: 0x0013AEAE
		public float CurrentMaintenanceCost
		{
			get
			{
				if (!this.benefitsDisabled)
				{
					return this.BaselineMaintenanceCost;
				}
				return 0f;
			}
		}

		// Token: 0x06003688 RID: 13960 RVA: 0x0013CCC4 File Offset: 0x0013AEC4
		public int GetControlPointPriority(PriorityType priority, bool checkValid)
		{
			if (!checkValid || this.nation.ValidPriority(priority))
			{
				return this.controlPointPriorities[priority];
			}
			return 0;
		}

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06003689 RID: 13961 RVA: 0x0013CCE5 File Offset: 0x0013AEE5
		// (set) Token: 0x0600368A RID: 13962 RVA: 0x0013CCED File Offset: 0x0013AEED
		public int totalWeightsForControlPoint { get; private set; }

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x0600368B RID: 13963 RVA: 0x0013CCF6 File Offset: 0x0013AEF6
		// (set) Token: 0x0600368C RID: 13964 RVA: 0x0013CCFE File Offset: 0x0013AEFE
		public int numPrioritiesWithWeight { get; private set; }

		// Token: 0x0600368D RID: 13965 RVA: 0x0013CD08 File Offset: 0x0013AF08
		public void SyncAllPriorities(TIControlPoint sourceCP)
		{
			int num = Enums.PriorityTypes.Length - 1;
			for (int i = 0; i < Enums.PriorityTypes.Length; i++)
			{
				PriorityType priorityType = Enums.PriorityTypes[i];
				this.controlPointPriorities[priorityType] = this.SetControlPointPriority(priorityType, sourceCP.GetControlPointPriority(priorityType, false), i == num, true, false);
			}
			this.RecordAndFixControlPointValues(false);
			GameControl.eventManager.TriggerEvent(new ControlPointDataUpdated(this), null, new object[] { this.nation });
		}

		// Token: 0x0600368E RID: 13966 RVA: 0x0013CD84 File Offset: 0x0013AF84
		public void RecordAndFixControlPointValues(bool alertReset)
		{
			this.totalWeightsForControlPoint = this.controlPointPriorities.Sum<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => this.GetControlPointPriority(x.Key, true));
			this.numPrioritiesWithWeight = this.controlPointPriorities.Count<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => this.GetControlPointPriority(x.Key, true) > 0);
			if (this.totalWeightsForControlPoint == 0)
			{
				this.controlPointPriorities[PriorityType.Economy] = 1;
				this.totalWeightsForControlPoint = 1;
				this.numPrioritiesWithWeight = 1;
				if (alertReset)
				{
					TINotificationQueueState.LogNationsCPPrioritiesReset(this, PriorityType.Economy);
				}
			}
			if (this.numPrioritiesWithWeight > 1)
			{
				foreach (PriorityType priorityType in Enums.PriorityTypes)
				{
					this.diversityBonus[priorityType] = 0f;
					foreach (PriorityType priorityType2 in TIControlPoint.priorityDiversityBonus.Keys)
					{
						if (priorityType2 != priorityType)
						{
							float num = this.nation.NationalPriorityBonuses(priorityType);
							float num2;
							if (this.faction != null && this.faction.cachedPriorityBonuses != null && this.faction.cachedPriorityBonuses.TryGetValue(priorityType, out num2))
							{
								num += num2;
							}
							if (num > -1f)
							{
								Dictionary<PriorityType, float> dictionary = this.diversityBonus;
								PriorityType priorityType3 = priorityType;
								dictionary[priorityType3] += TIControlPoint.priorityDiversityBonus[priorityType2] * (float)this.GetControlPointPriority(priorityType2, true) / (float)this.totalWeightsForControlPoint;
							}
						}
					}
				}
				return;
			}
			this.diversityBonus = Enums.PriorityTypes.ToDictionary<PriorityType, PriorityType, float>((PriorityType x) => x, (PriorityType x) => 0f);
		}

		// Token: 0x0600368F RID: 13967 RVA: 0x0013CF60 File Offset: 0x0013B160
		public int SetControlPointPriority(PriorityType priority, int value, bool skipUpdate = false, bool bulkUpdate = false, bool alertReset = false)
		{
			this.controlPointPriorities[priority] = Mathf.Clamp(value, 0, 3);
			if (!bulkUpdate)
			{
				this.RecordAndFixControlPointValues(!skipUpdate && alertReset);
			}
			if (!skipUpdate)
			{
				GameControl.eventManager.TriggerEvent(new ControlPointDataUpdated(this), null, new object[] { this.nation });
			}
			return this.controlPointPriorities[priority];
		}

		// Token: 0x06003690 RID: 13968 RVA: 0x0013CFC4 File Offset: 0x0013B1C4
		private void ChangeControlPointPriority(PriorityType priority, int delta, bool cycle)
		{
			if (cycle)
			{
				if (this.controlPointPriorities[priority] + delta > 3)
				{
					this.SetControlPointPriority(priority, 0, false, false, false);
					return;
				}
				if (this.controlPointPriorities[priority] + delta < 0)
				{
					this.SetControlPointPriority(priority, 3, false, false, false);
					return;
				}
			}
			this.SetControlPointPriority(priority, this.controlPointPriorities[priority] + delta, false, false, false);
		}

		// Token: 0x06003691 RID: 13969 RVA: 0x0013D029 File Offset: 0x0013B229
		public void IncrementControlPointPriority(PriorityType priority)
		{
			if (!this.controlPointPriorities.ContainsKey(priority))
			{
				this.controlPointPriorities[priority] = 0;
			}
			this.ChangeControlPointPriority(priority, 1, true);
		}

		// Token: 0x06003692 RID: 13970 RVA: 0x0013D04F File Offset: 0x0013B24F
		public void DecrementControlPointPriority(PriorityType priority)
		{
			if (!this.controlPointPriorities.ContainsKey(priority))
			{
				this.controlPointPriorities[priority] = 0;
			}
			this.ChangeControlPointPriority(priority, -1, true);
		}

		// Token: 0x06003693 RID: 13971 RVA: 0x0013D075 File Offset: 0x0013B275
		public string GetIconPath(bool small64)
		{
			if (!this.owned)
			{
				return TemplateManager.global.pathEmptyControlPoint;
			}
			if (small64)
			{
				return this.faction.factionIcon64path;
			}
			return this.faction.factionIcon128path;
		}

		// Token: 0x06003694 RID: 13972 RVA: 0x0013D0A4 File Offset: 0x0013B2A4
		public Sprite GetIcon(bool forUI, bool largeUI)
		{
			if (!this.owned)
			{
				return AssetCacheManager.controlPointCircle;
			}
			if (!forUI)
			{
				return this.faction.factionIcon128;
			}
			if (!largeUI)
			{
				return this.faction.factionIcon64;
			}
			return this.faction.factionIcon256;
		}

		// Token: 0x06003695 RID: 13973 RVA: 0x0013D0E0 File Offset: 0x0013B2E0
		public string GetIllustrationPath()
		{
			string text = TemplateManager.global.illus_controlPointPaths[(int)this.controlPointType];
			if (string.IsNullOrEmpty(text))
			{
				return "illustrations/ControlPoint_TheParty";
			}
			return text;
		}

		// Token: 0x0400245B RID: 9307
		public const int minPriorityValue = 0;

		// Token: 0x0400245C RID: 9308
		public const int maxPriorityValue = 3;

		// Token: 0x0400245D RID: 9309
		public int positionInNation;

		// Token: 0x04002460 RID: 9312
		public Dictionary<PriorityType, int> controlPointPriorities = new Dictionary<PriorityType, int>();

		// Token: 0x04002466 RID: 9318
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x04002467 RID: 9319
		private GameTimeManager gameTime;

		// Token: 0x0400246A RID: 9322
		[fsIgnore]
		public Dictionary<PriorityType, float> diversityBonus = new Dictionary<PriorityType, float>();

		// Token: 0x0400246B RID: 9323
		public static readonly Dictionary<PriorityType, float> priorityDiversityBonus = new Dictionary<PriorityType, float>
		{
			{
				PriorityType.Economy,
				0.5f
			},
			{
				PriorityType.Welfare,
				0.2f
			},
			{
				PriorityType.Environment,
				0.2f
			},
			{
				PriorityType.Knowledge,
				0.2f
			},
			{
				PriorityType.Government,
				0.2f
			},
			{
				PriorityType.Unity,
				0.2f
			},
			{
				PriorityType.Military,
				0.2f
			}
		};
	}
}
