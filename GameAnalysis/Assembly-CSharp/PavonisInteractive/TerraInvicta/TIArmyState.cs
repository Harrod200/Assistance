using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000766 RID: 1894
	public class TIArmyState : TIGameState, IOperationCapableState
	{
		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x0600359F RID: 13727 RVA: 0x00134DD2 File Offset: 0x00132FD2
		// (set) Token: 0x060035A0 RID: 13728 RVA: 0x00134DDA File Offset: 0x00132FDA
		public TIRegionState currentRegion { get; protected set; }

		// Token: 0x1700089C RID: 2204
		// (get) Token: 0x060035A1 RID: 13729 RVA: 0x00134DE3 File Offset: 0x00132FE3
		// (set) Token: 0x060035A2 RID: 13730 RVA: 0x00134DEB File Offset: 0x00132FEB
		public TIDateTime embarkDate { get; protected set; }

		// Token: 0x1700089D RID: 2205
		// (get) Token: 0x060035A3 RID: 13731 RVA: 0x00134DF4 File Offset: 0x00132FF4
		// (set) Token: 0x060035A4 RID: 13732 RVA: 0x00134DFC File Offset: 0x00132FFC
		public TIDateTime destinationSeaDate { get; protected set; }

		// Token: 0x1700089E RID: 2206
		// (get) Token: 0x060035A5 RID: 13733 RVA: 0x00134E05 File Offset: 0x00133005
		// (set) Token: 0x060035A6 RID: 13734 RVA: 0x00134E0D File Offset: 0x0013300D
		public bool huntingXenofauna { get; private set; }

		// Token: 0x1700089F RID: 2207
		// (get) Token: 0x060035A7 RID: 13735 RVA: 0x00134E16 File Offset: 0x00133016
		// (set) Token: 0x060035A8 RID: 13736 RVA: 0x00134E1E File Offset: 0x0013301E
		public string armyDamageEventName { get; private set; }

		// Token: 0x170008A0 RID: 2208
		// (get) Token: 0x060035A9 RID: 13737 RVA: 0x00134E27 File Offset: 0x00133027
		// (set) Token: 0x060035AA RID: 13738 RVA: 0x00134E2F File Offset: 0x0013302F
		public string armyStatusUpdateEventName { get; private set; }

		// Token: 0x170008A1 RID: 2209
		// (get) Token: 0x060035AB RID: 13739 RVA: 0x00134E38 File Offset: 0x00133038
		// (set) Token: 0x060035AC RID: 13740 RVA: 0x00134E40 File Offset: 0x00133040
		public string armyOperationCompleteEventName { get; private set; }

		// Token: 0x170008A2 RID: 2210
		// (get) Token: 0x060035AD RID: 13741 RVA: 0x00134E49 File Offset: 0x00133049
		public override bool isArmyState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170008A3 RID: 2211
		// (get) Token: 0x060035AE RID: 13742 RVA: 0x00134E4C File Offset: 0x0013304C
		public override Searchable searchable
		{
			get
			{
				return Searchable.always;
			}
		}

		// Token: 0x170008A4 RID: 2212
		// (get) Token: 0x060035AF RID: 13743 RVA: 0x00134E4F File Offset: 0x0013304F
		public override TIFactionState ref_faction
		{
			get
			{
				return this.faction;
			}
		}

		// Token: 0x170008A5 RID: 2213
		// (get) Token: 0x060035B0 RID: 13744 RVA: 0x00134E57 File Offset: 0x00133057
		public override TIRegionState ref_region
		{
			get
			{
				return this.currentRegion;
			}
		}

		// Token: 0x170008A6 RID: 2214
		// (get) Token: 0x060035B1 RID: 13745 RVA: 0x00134E5F File Offset: 0x0013305F
		public override TINationState ref_nation
		{
			get
			{
				TIRegionState currentRegion = this.currentRegion;
				if (currentRegion == null)
				{
					return null;
				}
				return currentRegion.nation;
			}
		}

		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x060035B2 RID: 13746 RVA: 0x00134E72 File Offset: 0x00133072
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				TIRegionState currentRegion = this.currentRegion;
				if (currentRegion == null)
				{
					return null;
				}
				return currentRegion.spaceBody;
			}
		}

		// Token: 0x170008A8 RID: 2216
		// (get) Token: 0x060035B3 RID: 13747 RVA: 0x00134E85 File Offset: 0x00133085
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return this.ref_naturalSpaceObject;
			}
		}

		// Token: 0x170008A9 RID: 2217
		// (get) Token: 0x060035B4 RID: 13748 RVA: 0x00134E8D File Offset: 0x0013308D
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this.ref_spaceBody;
			}
		}

		// Token: 0x170008AA RID: 2218
		// (get) Token: 0x060035B5 RID: 13749 RVA: 0x00134E95 File Offset: 0x00133095
		public override TIArmyState ref_army
		{
			get
			{
				return this;
			}
		}

		// Token: 0x170008AB RID: 2219
		// (get) Token: 0x060035B6 RID: 13750 RVA: 0x00134E98 File Offset: 0x00133098
		public override TIControlPoint ref_controlPoint
		{
			get
			{
				return this.homeNation.controlPoints[this.controlPointIdx];
			}
		}

		// Token: 0x170008AC RID: 2220
		// (get) Token: 0x060035B7 RID: 13751 RVA: 0x00134EB0 File Offset: 0x001330B0
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170008AD RID: 2221
		// (get) Token: 0x060035B8 RID: 13752 RVA: 0x00134EB3 File Offset: 0x001330B3
		public override bool hasEarthMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x060035B9 RID: 13753 RVA: 0x00134EB6 File Offset: 0x001330B6
		public virtual TIMegafaunaArmyState ref_megafaunaArmyState
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x060035BA RID: 13754 RVA: 0x00134EB9 File Offset: 0x001330B9
		public virtual TIAlienArmyState ref_alienArmyState
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060035BB RID: 13755 RVA: 0x00134EBC File Offset: 0x001330BC
		public override bool Initialize()
		{
			return true;
		}

		// Token: 0x060035BC RID: 13756 RVA: 0x00134EC0 File Offset: 0x001330C0
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			TIArmyTemplate tiarmyTemplate = template as TIArmyTemplate;
			if (tiarmyTemplate == null)
			{
				return;
			}
			this.templateName = tiarmyTemplate.dataName;
			this.deploymentType = tiarmyTemplate.deploymentType;
			this.faction = null;
			this.controlPointIdx = -1;
			this.createdFromTemplate = true;
		}

		// Token: 0x060035BD RID: 13757 RVA: 0x00134F0C File Offset: 0x0013310C
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			if (this.createdFromTemplate && !this.gameStateSubjectCreated)
			{
				this.homeRegion = this.template.homeRegion;
				this.currentRegion = this.template.startRegion;
				this.currentRegion.armies.Add(this);
				this.NewArmy(ArmyType.Human, 0, 1f);
				this.strength = this.template.startingStrength;
			}
		}

		// Token: 0x060035BE RID: 13758 RVA: 0x00134F7C File Offset: 0x0013317C
		public override void PostGlobalGameStateCreateInit_2()
		{
			foreach (OperationData operationData in this.currentOperations)
			{
				if (operationData.operationDataName == "DeployArmyOperation")
				{
					operationData.RepairOperation("DeployArmyOperation_OpenTarget");
				}
			}
		}

		// Token: 0x060035BF RID: 13759 RVA: 0x00134FE8 File Offset: 0x001331E8
		public override void PostCanvasManagerCreateInit_3()
		{
			if (!this.gameStateSubjectCreated)
			{
				this.homeNation.AddArmy(this);
				this.controlPointIdx = this.homeNation.GetNextArmyControlPointIdx();
				this.SetGameStateCreated();
				return;
			}
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnTimedOperationComplete), this.armyOperationCompleteEventName, null, true, false);
		}

		// Token: 0x060035C0 RID: 13760 RVA: 0x00135044 File Offset: 0x00133244
		public override void PostAllStartUpInit_5()
		{
			if (this.destroyed)
			{
				Log.Debug("PostAllStartUpInit_5() called on destroyed army : " + base.ID.ToString(), Array.Empty<object>());
				return;
			}
			if (this.strength <= 0f)
			{
				Log.Error("PostAllStartUpInit_5() called on army with 0 strength: " + base.ID.ToString() + " " + this.currentRegion.displayName, Array.Empty<object>());
				try
				{
					this.Disband();
					return;
				}
				catch
				{
					this.strength = 0.1f;
				}
			}
			if (this.currentOperations.Count > 0)
			{
				List<OperationData> list = new List<OperationData>();
				foreach (OperationData operationData in this.currentOperations)
				{
					if (World.Active.GetExistingManager<GameTimeManager>().Now > operationData.completionDate.ExportTime())
					{
						list.Add(operationData);
					}
				}
				foreach (OperationData operationData2 in list)
				{
					operationData2.operation.OnOperationExecute(this, operationData2.target);
					this.RemoveOperation(operationData2);
					GameControl.eventManager.TriggerEvent(new TimeEventComplete(this, null), this.armyOperationCompleteEventName, Array.Empty<object>());
				}
				this.UpdateIsMoving();
				return;
			}
			if (this.currentOperations.Count != this.CurrentOperations().Count)
			{
				this.currentOperations.RemoveAll((OperationData x) => x.operation == null);
			}
		}

		// Token: 0x060035C1 RID: 13761 RVA: 0x00135224 File Offset: 0x00133424
		public void NewArmy(ArmyType armyType, int value = 0, float startingStrength = 1f)
		{
			this.armyDamageEventName = new StringBuilder("VisualizeArmyDamage").Append(base.ID.ToString()).ToString();
			this.armyStatusUpdateEventName = new StringBuilder("ArmyStatusUpdate").Append(base.ID.ToString()).ToString();
			this.armyOperationCompleteEventName = new StringBuilder("ArmyOperationComplete").Append(base.ID.ToString()).ToString();
			this.strength = startingStrength;
			this.currentOperations = new List<OperationData>();
			this.armyType = armyType;
			switch (armyType)
			{
			case ArmyType.Human:
			{
				string localizationName = this.homeRegion.template.localizationName;
				this.displayName = Loc.T(new StringBuilder("TIArmyTemplate.displayName.").Append(localizationName).Append(".").Append(value.ToString())
					.ToString());
				this.displayNameWithArticle = Loc.T(new StringBuilder("TIArmyTemplate.displayNameWithArticle.").Append(localizationName).Append(".").Append(value.ToString())
					.ToString());
				break;
			}
			case ArmyType.AlienMegafauna:
				this.displayName = Loc.T("TIArmyTemplate.displayName.AlienMegafauna");
				this.displayNameWithArticle = Loc.T("TIArmyTemplate.displayNameWithArticle.AlienMegafauna");
				break;
			case ArmyType.AlienInvader:
				TIGlobalValuesState.GlobalValues.alienInvaderArmies++;
				this.displayName = Loc.T("TIArmyTemplate.displayName.AlienInvader", new object[] { TIGlobalValuesState.GlobalValues.alienInvaderArmies.ToString() });
				this.displayNameWithArticle = Loc.T("TIArmyTemplate.displayNameWithArticle.AlienInvader", new object[] { TIGlobalValuesState.GlobalValues.alienInvaderArmies.ToString() });
				break;
			}
			this.SetArmyDataDirty();
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnTimedOperationComplete), this.armyOperationCompleteEventName, null, true, false);
			EventManager eventManager = GameControl.eventManager;
			GameEvent gameEvent = new ArmyMajorStatusUpdate(this);
			string text = null;
			object[] array = new object[4];
			array[0] = this;
			array[1] = this.currentRegion;
			array[2] = this.homeRegion;
			int num = 3;
			TIRegionState tiregionState = this.homeRegion;
			array[num] = ((tiregionState != null) ? tiregionState.nation : null);
			eventManager.TriggerEvent(gameEvent, text, (from x in array.Distinct<object>()
				where x != null
				select x).ToArray<object>());
		}

		// Token: 0x060035C2 RID: 13762 RVA: 0x00135487 File Offset: 0x00133687
		public void SetGameStateCreated()
		{
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x060035C3 RID: 13763 RVA: 0x00135490 File Offset: 0x00133690
		public void Disband()
		{
			TIRegionState currentRegion = this.currentRegion;
			TIRegionState tiregionState = this.homeRegion;
			TIRegionState tiregionState2 = this.homeRegion;
			TINationState tinationState = ((tiregionState2 != null) ? tiregionState2.nation : null);
			TIRegionState tiregionState3 = null;
			if (this.currentOperations.Count > 0 && this.currentOperations[0].operation is DeployArmyOperation)
			{
				tiregionState3 = this.currentOperations[0].target.ref_region;
			}
			this.ClearOperations();
			this.destroyed = true;
			base.ArchiveState(false);
			TINotificationQueueState.CleanQueueOfArchivedState(this, this.currentRegion);
			TISpaceBodyState ref_spaceBody = this.ref_spaceBody;
			foreach (TISpaceFleetState tispaceFleetState in ((ref_spaceBody != null) ? ref_spaceBody.fleetsInInterfaceOrbits : null))
			{
				if (tispaceFleetState.bombardmentTarget == this)
				{
					tispaceFleetState.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.NotForDisplay);
				}
			}
			if (!this.homeNation.RemoveArmy(this) && !this.homeRegion.nation.RemoveArmy(this))
			{
				TINationState[] array = GameStateManager.AllNations();
				int i = 0;
				while (i < array.Length && !array[i].RemoveArmy(this))
				{
					i++;
				}
			}
			this.currentRegion.armies.Remove(this);
			TIFactionState tifactionState = this.faction;
			if (tifactionState != null)
			{
				tifactionState.armies.Remove(this);
			}
			this.strength = 0f;
			TIFactionState[] array2 = GameStateManager.AllFactions();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].CleanStateFromGoalTargets(this);
			}
			this.currentRegion = null;
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnTimedOperationComplete), null);
			World.Active.GetExistingManager<GameTimeManager>().CancelAllTimeEventsForObject(this);
			array2 = GameStateManager.AllFactions();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].ExpireIntel(this, true);
			}
			this.SetArmyDataDirty();
			this.homeRegion = null;
			GameControl.eventManager.TriggerEvent(new ArmyMajorStatusUpdate(this), null, (from x in new object[] { this, currentRegion, tiregionState, tinationState, tiregionState3 }.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			base.ArchiveState(true);
			this.faction = null;
			switch (this.armyType)
			{
			case ArmyType.Human:
				GameStateManager.RemoveGameState<TIArmyState>(base.ID, false);
				return;
			case ArmyType.AlienMegafauna:
				GameStateManager.RemoveGameState<TIMegafaunaArmyState>(base.ID, false);
				return;
			case ArmyType.AlienInvader:
				GameStateManager.RemoveGameState<TIAlienArmyState>(base.ID, false);
				return;
			default:
				return;
			}
		}

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x060035C4 RID: 13764 RVA: 0x00135734 File Offset: 0x00133934
		public TIArmyTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIArmyTemplate>();
			}
		}

		// Token: 0x170008B1 RID: 2225
		// (get) Token: 0x060035C5 RID: 13765 RVA: 0x0013573C File Offset: 0x0013393C
		public virtual TINationState homeNation
		{
			get
			{
				TIRegionState tiregionState = this.homeRegion;
				if (tiregionState == null)
				{
					return null;
				}
				return tiregionState.nation;
			}
		}

		// Token: 0x170008B2 RID: 2226
		// (get) Token: 0x060035C6 RID: 13766 RVA: 0x0013574F File Offset: 0x0013394F
		public TINationState currentNation
		{
			get
			{
				return this.currentRegion.nation;
			}
		}

		// Token: 0x170008B3 RID: 2227
		// (get) Token: 0x060035C7 RID: 13767 RVA: 0x0013575C File Offset: 0x0013395C
		public virtual float techLevel
		{
			get
			{
				return this.homeNation.militaryTechLevel;
			}
		}

		// Token: 0x170008B4 RID: 2228
		// (get) Token: 0x060035C8 RID: 13768 RVA: 0x00135769 File Offset: 0x00133969
		private int baseTechLevel
		{
			get
			{
				return Mathf.Clamp((int)Math.Truncate((double)this.techLevel), TIGlobalConfig.globalConfig.minArmyBaseTechLevel, TIGlobalConfig.globalConfig.maxArmyBaseTechLevel);
			}
		}

		// Token: 0x170008B5 RID: 2229
		// (get) Token: 0x060035C9 RID: 13769 RVA: 0x00135791 File Offset: 0x00133991
		public virtual bool InFriendlyRegion
		{
			get
			{
				return this.FriendlyRegion(this.currentRegion);
			}
		}

		// Token: 0x170008B6 RID: 2230
		// (get) Token: 0x060035CA RID: 13770 RVA: 0x0013579F File Offset: 0x0013399F
		public bool InLegalRegion
		{
			get
			{
				return this.LegalRegion(this.currentRegion);
			}
		}

		// Token: 0x060035CB RID: 13771 RVA: 0x001357AD File Offset: 0x001339AD
		public virtual bool LegalRegion(TIRegionState region)
		{
			return this.FriendlyRegion(region) || region.nation.IsAtWarWith(this.homeNation);
		}

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x060035CC RID: 13772 RVA: 0x001357CB File Offset: 0x001339CB
		public virtual bool CanTakeOffensiveAction
		{
			get
			{
				return this.strength >= 0.5f && this.InLegalRegion;
			}
		}

		// Token: 0x060035CD RID: 13773 RVA: 0x001357E2 File Offset: 0x001339E2
		public virtual bool IsAttacking()
		{
			return !this.InFriendlyRegion;
		}

		// Token: 0x060035CE RID: 13774 RVA: 0x001357ED File Offset: 0x001339ED
		public virtual bool InBattleWithArmies()
		{
			return this.GetEnemyArmiesInRegion().Count > 0;
		}

		// Token: 0x060035CF RID: 13775 RVA: 0x001357FD File Offset: 0x001339FD
		public virtual bool InBattleWithArmiesOrRegionDefenses()
		{
			return this.InBattleWithArmies() || this.OccupyingRegion(true);
		}

		// Token: 0x060035D0 RID: 13776 RVA: 0x00135810 File Offset: 0x00133A10
		public virtual bool CanHeal()
		{
			return this.strength > 0f && this.strength < 1f && this.InFriendlyRegion && !this.currentRegion.OccupiedOrOccupationUnderway() && !this.InBattleWithArmies() && this.CurrentOperations().Count == 0;
		}

		// Token: 0x170008B8 RID: 2232
		// (get) Token: 0x060035D1 RID: 13777 RVA: 0x00135864 File Offset: 0x00133A64
		public virtual bool HumanArmy
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170008B9 RID: 2233
		// (get) Token: 0x060035D2 RID: 13778 RVA: 0x00135867 File Offset: 0x00133A67
		public virtual bool AlienMegafaunaArmy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170008BA RID: 2234
		// (get) Token: 0x060035D3 RID: 13779 RVA: 0x0013586A File Offset: 0x00133A6A
		public virtual bool AlienRegularArmy
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170008BB RID: 2235
		// (get) Token: 0x060035D4 RID: 13780 RVA: 0x0013586D File Offset: 0x00133A6D
		public float techLevelSpeedModifier
		{
			get
			{
				return 4f / this.techLevel;
			}
		}

		// Token: 0x170008BC RID: 2236
		// (get) Token: 0x060035D5 RID: 13781 RVA: 0x0013587B File Offset: 0x00133A7B
		public bool useHomeInvestmentFactor
		{
			get
			{
				return this.currentRegion == this.homeRegion && this.CurrentOperations().Count == 0 && !this.InBattleWithArmies() && !this.currentRegion.IsFullyOccupied();
			}
		}

		// Token: 0x170008BD RID: 2237
		// (get) Token: 0x060035D6 RID: 13782 RVA: 0x001358B5 File Offset: 0x00133AB5
		public virtual float investmentArmyFactor
		{
			get
			{
				if (!this.useHomeInvestmentFactor)
				{
					return TemplateManager.global.nationalInvestmentArmyFactorAway;
				}
				return TemplateManager.global.nationalInvestmentArmyFactorHome;
			}
		}

		// Token: 0x170008BE RID: 2238
		// (get) Token: 0x060035D7 RID: 13783 RVA: 0x001358D4 File Offset: 0x00133AD4
		public virtual float investmentNavyFactor
		{
			get
			{
				return TemplateManager.global.nationalInvestmentNavyFactor;
			}
		}

		// Token: 0x170008BF RID: 2239
		// (get) Token: 0x060035D8 RID: 13784 RVA: 0x001358E0 File Offset: 0x00133AE0
		public bool atSea
		{
			get
			{
				ArmySeaTransitStage armySeaTransitStage = this.SeaTransitStage();
				return armySeaTransitStage - ArmySeaTransitStage.Sea_HomeRegion <= 1;
			}
		}

		// Token: 0x060035D9 RID: 13785 RVA: 0x00135900 File Offset: 0x00133B00
		public static float baseSeaMovementSpeed_days(TIRegionState origin, TIRegionState destination, TIArmyState army, bool onlyConsiderDistance = false)
		{
			float num = origin.DistanceToRegion_km(destination) / 792f;
			if (!onlyConsiderDistance)
			{
				num *= TIRegionState.SeaTravelMultiplier((army != null) ? army.homeNation : null, origin, destination);
			}
			return 14f + num;
		}

		// Token: 0x060035DA RID: 13786 RVA: 0x0013593C File Offset: 0x00133B3C
		public static float baseLandMovementSpeed_days(TIRegionState origin, TIRegionState destination, TIArmyState army, bool onlyConsiderDistance = false)
		{
			float num = origin.DistanceToRegion_km(destination) / 300f;
			if (onlyConsiderDistance)
			{
				return num;
			}
			bool flag = origin.mapRegionTemplate.smallRegion && !destination.mapRegionTemplate.smallRegion;
			bool flag2 = !origin.mapRegionTemplate.smallRegion && destination.mapRegionTemplate.smallRegion;
			if (army != null)
			{
				if (!army.FriendlyRegion(origin) || army.InBattleWithArmies())
				{
					if (flag)
					{
						num *= TIArmyState.small_enemyRegion;
					}
					else if (flag2)
					{
						num *= TIArmyState.large_enemyRegion;
					}
					else
					{
						num *= TIArmyState.normal_enemyRegion;
					}
					if (origin.terrain == TerrainType.Rugged)
					{
						if (flag)
						{
							num *= TIArmyState.small_enemyRegion_rugged;
						}
						else if (flag2)
						{
							num *= TIArmyState.large_enemyRegion_rugged;
						}
						else
						{
							num *= TIArmyState.normal_enemyRegion_rugged;
						}
					}
					if (!army.FriendlyRegionIncludingFullyOccupied(destination))
					{
						List<TIRegionState> list = destination.AdjacentRegions(true);
						list.RemoveAll((TIRegionState x) => !army.CanEnter(x));
						if (list.Count > 0 && list.All<TIRegionState>((TIRegionState x) => !army.FriendlyRegionIncludingFullyOccupied(x)))
						{
							num *= 4f;
						}
					}
				}
				else
				{
					if (origin.terrain == TerrainType.Rugged)
					{
						if (flag)
						{
							num *= TIArmyState.small_friendlyRegion_rugged;
						}
						else if (flag2)
						{
							num *= TIArmyState.large_friendlyRegion_rugged;
						}
						else
						{
							num *= TIArmyState.normal_friendlyRegion_rugged;
						}
					}
					if (origin.nation.unrest > 5f)
					{
						if (flag)
						{
							num *= Mathf.Pow(1f + (origin.nation.unrest - 5f) / 6.5f, 0.2f);
						}
						else if (flag2)
						{
							num *= Mathf.Pow(1f + (origin.nation.unrest - 5f) / 6.5f, 0.8f);
						}
						else
						{
							num *= Mathf.Pow(1f + (origin.nation.unrest - 5f) / 6.5f, 0.5f);
						}
					}
				}
				if (origin.colonyRegion)
				{
					if (flag)
					{
						num *= TIArmyState.small_colonyRegion;
					}
					else if (flag2)
					{
						num *= TIArmyState.large_colonyRegion;
					}
					else
					{
						num *= TIArmyState.normal_colonyRegion;
					}
				}
				if (!army.FriendlyRegion(destination))
				{
					if (flag)
					{
						num *= TIArmyState.large_enemyRegion;
					}
					else if (flag2)
					{
						num *= TIArmyState.small_enemyRegion;
					}
					else
					{
						num *= TIArmyState.normal_enemyRegion;
					}
					if (destination.terrain == TerrainType.Rugged)
					{
						if (flag)
						{
							num *= TIArmyState.large_enemyRegion_rugged;
						}
						else if (flag2)
						{
							num *= TIArmyState.small_enemyRegion_rugged;
						}
						else
						{
							num *= TIArmyState.normal_enemyRegion_rugged;
						}
					}
				}
				else
				{
					if (destination.terrain == TerrainType.Rugged)
					{
						if (flag)
						{
							num *= TIArmyState.large_friendlyRegion_rugged;
						}
						else if (flag2)
						{
							num *= TIArmyState.small_friendlyRegion_rugged;
						}
						else
						{
							num *= TIArmyState.normal_friendlyRegion_rugged;
						}
					}
					if (destination.nation.unrest > 5f)
					{
						if (flag)
						{
							num *= Mathf.Pow(1f + (destination.nation.unrest - 5f) / 6.5f, 0.8f);
						}
						else if (flag2)
						{
							num *= Mathf.Pow(1f + (destination.nation.unrest - 5f) / 6.5f, 0.2f);
						}
						else
						{
							num *= Mathf.Pow(1f + (origin.nation.unrest - 5f) / 6.5f, 0.5f);
						}
					}
				}
				if (destination.colonyRegion)
				{
					if (flag)
					{
						num *= TIArmyState.large_colonyRegion;
					}
					else if (flag2)
					{
						num *= TIArmyState.small_colonyRegion;
					}
					else
					{
						num *= TIArmyState.normal_colonyRegion;
					}
				}
			}
			else
			{
				if (origin.terrain == TerrainType.Rugged)
				{
					if (flag)
					{
						num *= TIArmyState.small_friendlyRegion_rugged;
					}
					else if (flag2)
					{
						num *= TIArmyState.large_friendlyRegion_rugged;
					}
					else
					{
						num *= TIArmyState.normal_friendlyRegion_rugged;
					}
				}
				if (destination.terrain == TerrainType.Rugged)
				{
					if (flag)
					{
						num *= TIArmyState.large_friendlyRegion_rugged;
					}
					else if (flag2)
					{
						num *= TIArmyState.small_friendlyRegion_rugged;
					}
					else
					{
						num *= TIArmyState.normal_friendlyRegion_rugged;
					}
				}
			}
			return num;
		}

		// Token: 0x060035DB RID: 13787 RVA: 0x00135D26 File Offset: 0x00133F26
		public void SetIsMoving()
		{
			this._isMoving = true;
		}

		// Token: 0x060035DC RID: 13788 RVA: 0x00135D2F File Offset: 0x00133F2F
		public void SetNotMoving()
		{
			this._isMoving = false;
		}

		// Token: 0x170008C0 RID: 2240
		// (get) Token: 0x060035DD RID: 13789 RVA: 0x00135D38 File Offset: 0x00133F38
		public bool IsMoving
		{
			get
			{
				return this._isMoving;
			}
		}

		// Token: 0x060035DE RID: 13790 RVA: 0x00135D40 File Offset: 0x00133F40
		public bool IsFighting(bool forceUpdate)
		{
			if (forceUpdate || TIFrameCounter.FrameCount != this._lastIsFightingFrame)
			{
				bool flag;
				if (!this.destroyed)
				{
					if (!this.InBattleWithArmiesOrRegionDefenses())
					{
						List<OperationData> list = this.currentOperations;
						if (list == null)
						{
							flag = false;
						}
						else
						{
							flag = list.Any<OperationData>(delegate(OperationData x)
							{
								IOperation operation = x.operation;
								TIArmyOperationTemplate tiarmyOperationTemplate = ((operation != null) ? operation.GetTemplate() : null) as TIArmyOperationTemplate;
								return tiarmyOperationTemplate != null && tiarmyOperationTemplate.IsCombatOperation();
							});
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = false;
				}
				this._isFighting = flag;
				this._lastIsFightingFrame = TIFrameCounter.FrameCount;
			}
			return this._isFighting;
		}

		// Token: 0x060035DF RID: 13791 RVA: 0x00135DBC File Offset: 0x00133FBC
		public List<TIArmyState> GetEnemyArmiesInRegion()
		{
			if (this.currentRegion == null || this.currentRegion.armies == null)
			{
				return new List<TIArmyState>();
			}
			if (this.AlienMegafaunaArmy)
			{
				return this.currentRegion.armies.Where<TIArmyState>((TIArmyState x) => x.faction != this.faction).ToList<TIArmyState>();
			}
			return this.currentRegion.armies.Where<TIArmyState>((TIArmyState x) => x.homeNation.wars.Contains(this.homeNation) || (x.AlienMegafaunaArmy && x.faction != this.faction)).ToList<TIArmyState>();
		}

		// Token: 0x060035E0 RID: 13792 RVA: 0x00135E35 File Offset: 0x00134035
		public bool FriendlyRegion(TIRegionState region)
		{
			if (!(region != null))
			{
				return false;
			}
			if (!(region.nation == this.homeNation))
			{
				TINationState nation = region.nation;
				return nation != null && nation.IsAlliedWith(this.homeNation, false);
			}
			return true;
		}

		// Token: 0x060035E1 RID: 13793 RVA: 0x00135E6F File Offset: 0x0013406F
		public bool OccupierInCurrentRegion()
		{
			return this.currentRegion.GetOccupyingAlliance(false).Contains(this.homeNation);
		}

		// Token: 0x060035E2 RID: 13794 RVA: 0x00135E88 File Offset: 0x00134088
		public bool FriendlyRegionIncludingFullyOccupied(TIRegionState region)
		{
			return region != null && (this.FriendlyRegion(region) || region.armies.Any<TIArmyState>((TIArmyState x) => x.homeNation == this.homeNation || this.homeNation.allies.Contains(x.homeNation)) || (region.IsFullyOccupied() && region.GetOccupyingAlliance(false).Contains(this.homeNation)));
		}

		// Token: 0x060035E3 RID: 13795 RVA: 0x00135EE0 File Offset: 0x001340E0
		public bool CanReduceOccupation()
		{
			return this.InFriendlyRegion && this.currentOperations.Count == 0 && this.currentRegion.OccupiedOrOccupationUnderway() && this.currentRegion.occupations.Any<KeyValuePair<TINationState, float>>((KeyValuePair<TINationState, float> x) => this.homeNation.wars.Contains(x.Key));
		}

		// Token: 0x060035E4 RID: 13796 RVA: 0x00135F30 File Offset: 0x00134130
		public virtual bool OccupyingRegion(bool includeLiberation = true)
		{
			return (includeLiberation && this.CanReduceOccupation()) || (!this.InBattleWithArmies() && (this.homeNation.wars.Contains(this.currentNation) || !this.homeNation.extant) && this.currentOperations.Count == 0 && (!this.currentRegion.IsFullyOccupied() || (this.AlienRegularArmy && !this.OccupierInCurrentRegion())));
		}

		// Token: 0x060035E5 RID: 13797 RVA: 0x00135FA8 File Offset: 0x001341A8
		public bool InEnemyCapital()
		{
			return !this.InFriendlyRegion && this.currentRegion == this.currentNation.capital;
		}

		// Token: 0x060035E6 RID: 13798 RVA: 0x00135FCA File Offset: 0x001341CA
		public float OccupationValue()
		{
			if (this.OccupyingRegion(true) && this.currentRegion.occupations.ContainsKey(this.homeNation))
			{
				return this.currentRegion.occupations[this.homeNation];
			}
			return 0f;
		}

		// Token: 0x060035E7 RID: 13799 RVA: 0x0013600C File Offset: 0x0013420C
		public bool InBattleWithOtherArmiesAndWinningByALot()
		{
			List<TIArmyState> list = this.currentRegion.FilteredArmiesPresent(true, false, false, false, true);
			List<TIArmyState> list2 = this.currentRegion.FilteredArmiesPresent(false, false, true, false, false);
			if (list2.Count > 0 && list.Count > 0)
			{
				if (list.Contains(this))
				{
					if (list.Sum<TIArmyState>((TIArmyState x) => x.strength) <= list2.Sum<TIArmyState>((TIArmyState x) => x.strength) * 2f)
					{
						if (!list2.All<TIArmyState>((TIArmyState x) => x.strength < 0.2f))
						{
							return false;
						}
					}
					return true;
				}
				if (list2.Contains(this))
				{
					if (list2.Sum<TIArmyState>((TIArmyState x) => x.strength) <= list.Sum<TIArmyState>((TIArmyState x) => x.strength) * 3f)
					{
						if (!list.All<TIArmyState>((TIArmyState x) => x.strength < 0.2f))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x170008C1 RID: 2241
		// (get) Token: 0x060035E8 RID: 13800 RVA: 0x00136160 File Offset: 0x00134360
		public virtual float dailyHealRate
		{
			get
			{
				TINationState homeNation = this.homeNation;
				TIFactionState tifactionState = ((homeNation != null) ? homeNation.GetControlPointTypeOwner(ControlPointType.DefenseSector) : null);
				float num = 0.005f + ((tifactionState != null && tifactionState == this.faction) ? TemplateManager.global.defenseSectorHealBonus : 0f);
				if (this.homeRegion == this.currentRegion)
				{
					num += 0.01f;
				}
				else if (this.currentNation == this.homeNation)
				{
					num += 0.005f;
				}
				float num2 = (float)this.homeNation.armies.Count<TIArmyState>((TIArmyState x) => x.armyType == this.armyType && x.CanHeal());
				if (num2 > 1f)
				{
					float num3 = (float)this.homeNation.armies.Count<TIArmyState>((TIArmyState x) => x.armyType == this.armyType);
					num *= 1f - 0.5f * (num2 / num3);
				}
				num += TIEffectsState.SumEffectsModifiers(Context.ArmyHealRate, this.faction, num, null);
				if (num < 0f)
				{
					Debug.LogError("Negative DailyHealRate for " + base.ID.ToString() + this.displayName);
					num = 0f;
				}
				return num;
			}
		}

		// Token: 0x060035E9 RID: 13801 RVA: 0x0013628C File Offset: 0x0013448C
		public static IList<TIRegionState> OneStepValidDestinationRegions(TIArmyState army, TIRegionState currentRegion, bool includeCurrentRegion)
		{
			List<TIRegionState> list = new List<TIRegionState>();
			if (army.AlienMegafaunaArmy)
			{
				list.AddRange(currentRegion.AdjacentRegions(true));
				if (!includeCurrentRegion && list.Contains(currentRegion))
				{
					list.Remove(currentRegion);
				}
			}
			else
			{
				bool flag = army.homeNation.navalFreedom && army.deploymentType == DeploymentType.Naval && currentRegion.onTheWater;
				List<TINationState> list2 = new List<TINationState>();
				list2.Add(army.homeNation);
				list2.AddRange(army.homeNation.allies);
				if (includeCurrentRegion)
				{
					list.Add(currentRegion);
				}
				foreach (TINationState tinationState in list2)
				{
					foreach (TIRegionState tiregionState in tinationState.regions)
					{
						if (tiregionState.IsAdjacent(currentRegion, false) || (flag && tiregionState != currentRegion && tiregionState.onTheWater && TIArmyState.IsTraversible(currentRegion, tiregionState, army)))
						{
							list.Add(tiregionState);
						}
					}
				}
				foreach (TINationState tinationState2 in army.homeNation.wars.Distinct<TINationState>())
				{
					foreach (TIRegionState tiregionState2 in tinationState2.regions)
					{
						if (tiregionState2.IsAdjacent(currentRegion, true) || (tiregionState2 != currentRegion && flag && tiregionState2.onTheWater && TIArmyState.IsTraversible(currentRegion, tiregionState2, army)))
						{
							list.Add(tiregionState2);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060035EA RID: 13802 RVA: 0x00136478 File Offset: 0x00134678
		public static List<TIRegionState> AllValidDestinationRegions(TIArmyState army, TIRegionState currentRegion, bool includeCurrentRegion)
		{
			List<TIRegionState> list = new List<TIRegionState>();
			bool navalFreedom = army.homeNation.navalFreedom;
			List<TINationState> list2 = new List<TINationState>();
			list2.Add(army.homeNation);
			list2.AddRange(army.homeNation.allies);
			IEnumerable<TINationState> enumerable = army.homeNation.wars.Distinct<TINationState>();
			list2.AddRange(enumerable);
			if (includeCurrentRegion)
			{
				list.Add(currentRegion);
			}
			List<TIRegionState> list3 = new List<TIRegionState>();
			foreach (TINationState tinationState in list2)
			{
				bool flag = enumerable.Contains(tinationState);
				foreach (TIRegionState tiregionState in tinationState.regions)
				{
					if (tiregionState.IsAdjacent(currentRegion, flag) || (tiregionState != currentRegion && navalFreedom && army.deploymentType == DeploymentType.Naval && currentRegion.onTheWater && tiregionState.onTheWater))
					{
						list.Add(tiregionState);
					}
					else
					{
						list3.Add(tiregionState);
					}
				}
			}
			int num = 0;
			bool flag2;
			do
			{
				flag2 = false;
				foreach (TIRegionState tiregionState2 in list3.ToList<TIRegionState>())
				{
					if (tiregionState2.AdjacentRegions(enumerable.Contains(tiregionState2.nation)).Intersect<TIRegionState>(list).Any<TIRegionState>())
					{
						list.Add(tiregionState2);
						list3.Remove(tiregionState2);
						flag2 = true;
					}
				}
				num++;
			}
			while (flag2 && num < 1000);
			if (num >= 1000)
			{
				Log.Error("Something wrong with the loop", Array.Empty<object>());
			}
			return list;
		}

		// Token: 0x170008C2 RID: 2242
		// (get) Token: 0x060035EB RID: 13803 RVA: 0x00136654 File Offset: 0x00134854
		public IEnumerable<TIRegionState> ReachableRegions
		{
			get
			{
				if (this.reachableRegionsCachedFrame != TIFrameCounter.FrameCount && (!this.AlienMegafaunaArmy || !this.cachedReachableRegions.Contains(this.currentRegion)))
				{
					this.cachedReachableRegions.Clear();
					Dictionary<TIRegionState, bool> canGetToCache = new Dictionary<TIRegionState, bool>();
					this.cachedReachableRegions.UnionWith(TIRegionState.Regions.Where<TIRegionState>((TIRegionState x) => this.CanGetTo(x, null, null, canGetToCache)));
					this.reachableRegionsCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedReachableRegions;
			}
		}

		// Token: 0x170008C3 RID: 2243
		// (get) Token: 0x060035EC RID: 13804 RVA: 0x001366E0 File Offset: 0x001348E0
		public IEnumerable<TIRegionState> ReachableRegions_Fast
		{
			get
			{
				if (this.AlienMegafaunaArmy)
				{
					return this.ReachableRegions;
				}
				float num = float.PositiveInfinity;
				if (this.reachableRegionsCachedDate_Fast != null)
				{
					num = (float)(TITimeState.Now() - this.reachableRegionsCachedDate_Fast).TotalDays;
				}
				if (num > 7f)
				{
					this.cachedReachableRegions_Fast.Clear();
					Dictionary<TIRegionState, bool> canGetToCache = new Dictionary<TIRegionState, bool>();
					this.cachedReachableRegions_Fast.UnionWith(TIRegionState.Regions.Where<TIRegionState>((TIRegionState x) => this.CanGetTo(x, null, null, canGetToCache)));
					this.reachableRegionsCachedDate_Fast = TITimeState.Now();
				}
				return this.cachedReachableRegions_Fast;
			}
		}

		// Token: 0x060035ED RID: 13805 RVA: 0x00136788 File Offset: 0x00134988
		public static DeploymentType GetRequiredDeploymentType(TIRegionState origin, TIRegionState destination, TIArmyState army = null)
		{
			DeploymentType deploymentType;
			TIArmyState.IsTraversible(origin, destination, out deploymentType, army);
			return deploymentType;
		}

		// Token: 0x060035EE RID: 13806 RVA: 0x001367A4 File Offset: 0x001349A4
		public static bool IsTraversible(TIRegionState origin, TIRegionState destination, out DeploymentType deploymentTypeRequired, TIArmyState army = null)
		{
			deploymentTypeRequired = DeploymentType.None;
			bool flag = origin.Neighbors.Contains(destination);
			if (flag && army != null)
			{
				switch (origin.GetAdjacencyType(destination))
				{
				case TerrestrialAdjacencyType.None:
					flag = false;
					break;
				case TerrestrialAdjacencyType.FriendlyCrossingOnly:
					flag = !army.AlienMegafaunaArmy && !army.homeNation.wars.Contains(destination.nation) && !army.homeNation.wars.Contains(origin.nation);
					break;
				}
			}
			if (flag)
			{
				deploymentTypeRequired = DeploymentType.Standard;
				return true;
			}
			bool flag2 = origin.onTheWater && destination.onTheWater;
			if (flag2 && army != null)
			{
				flag2 = army.deploymentType == DeploymentType.Naval && army.homeNation.navalFreedom && origin.GetAccessibleWaterBodies(army.homeNation).Intersect<WaterBody>(destination.GetAccessibleWaterBodies(army.homeNation)).Any<WaterBody>();
			}
			if (flag2)
			{
				deploymentTypeRequired = DeploymentType.Naval;
				return true;
			}
			return false;
		}

		// Token: 0x060035EF RID: 13807 RVA: 0x00136898 File Offset: 0x00134A98
		public static bool IsTraversible(TIRegionState origin, TIRegionState destination, TIArmyState army = null)
		{
			DeploymentType deploymentType;
			return TIArmyState.IsTraversible(origin, destination, out deploymentType, army);
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x060035F0 RID: 13808 RVA: 0x001368B0 File Offset: 0x00134AB0
		public IEnumerable<TIRegionState> EnterableRegions
		{
			get
			{
				if (this.AlienMegafaunaArmy)
				{
					return TIRegionState.Regions;
				}
				if (this.enterableRegionsCachedFrame != TIFrameCounter.FrameCount)
				{
					this.cachedEnterableRegions = new HashSet<TIRegionState>();
					this.cachedEnterableRegions.UnionWith(TIRegionState.Regions.Where<TIRegionState>((TIRegionState destination) => destination.nation == this.homeNation || this.homeNation.allies.Contains(destination.nation) || this.homeNation.wars.Contains(destination.nation)));
					this.enterableRegionsCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedEnterableRegions;
			}
		}

		// Token: 0x060035F1 RID: 13809 RVA: 0x00136915 File Offset: 0x00134B15
		public bool CanEnter(TIRegionState destination)
		{
			return this.AlienMegafaunaArmy || this.EnterableRegions.Contains(destination);
		}

		// Token: 0x060035F2 RID: 13810 RVA: 0x00136930 File Offset: 0x00134B30
		public static float GetDeploymentToAdjacentRegionDuration_Days(TIRegionState origin, TIRegionState destination, TIArmyState army = null)
		{
			if (origin == destination)
			{
				return 0f;
			}
			DeploymentType deploymentType;
			if (!TIArmyState.IsTraversible(origin, destination, out deploymentType, army))
			{
				return -1f;
			}
			float num;
			if (deploymentType == DeploymentType.Naval)
			{
				num = TIArmyState.baseSeaMovementSpeed_days(origin, destination, army, false);
			}
			else
			{
				num = TIArmyState.baseLandMovementSpeed_days(origin, destination, army, false);
			}
			return num * ((army != null) ? army.techLevelSpeedModifier : 1f);
		}

		// Token: 0x060035F3 RID: 13811 RVA: 0x0013698A File Offset: 0x00134B8A
		public float GetDeploymentToAdjacentRegionDuration_Days(TIRegionState destination)
		{
			return TIArmyState.GetDeploymentToAdjacentRegionDuration_Days(this.currentRegion, destination, this);
		}

		// Token: 0x060035F4 RID: 13812 RVA: 0x0013699C File Offset: 0x00134B9C
		public static float GetAdmissableHeuristicOfJourneyDurationInDays(TIRegionState origin, TIRegionState destination, TIArmyState army = null)
		{
			if (origin == destination)
			{
				return 0f;
			}
			float num = TIArmyState.baseLandMovementSpeed_days(origin, destination, army, true);
			float num2 = float.PositiveInfinity;
			if (army == null || (army.deploymentType == DeploymentType.Naval && army.homeNation.navalFreedom))
			{
				num2 = TIArmyState.baseSeaMovementSpeed_days(origin, destination, army, true);
			}
			if (num < 0f)
			{
				num = float.PositiveInfinity;
			}
			if (num2 < 0f)
			{
				num2 = float.PositiveInfinity;
			}
			float num3 = Mathf.Min(num, num2);
			if (army != null)
			{
				num3 *= army.techLevelSpeedModifier;
			}
			return num3;
		}

		// Token: 0x060035F5 RID: 13813 RVA: 0x00136A28 File Offset: 0x00134C28
		private static IEnumerable<TIRegionState> GetConnectedRegions(TIRegionState origin, TIArmyState army = null)
		{
			if (army != null)
			{
				int num = -1;
				HashSet<TIRegionState> hashSet;
				bool flag;
				if (army.cachedConnectedRegions.ContainsKey(origin))
				{
					ValueTuple<HashSet<TIRegionState>, int> valueTuple = army.cachedConnectedRegions[origin];
					hashSet = valueTuple.Item1;
					num = valueTuple.Item2;
					flag = true;
				}
				else
				{
					hashSet = new HashSet<TIRegionState>();
					flag = false;
				}
				if (!flag || num != TIFrameCounter.FrameCount)
				{
					hashSet.Clear();
					hashSet.UnionWith(origin.ConnectedRegions.Where<TIRegionState>(delegate(TIRegionState destination)
					{
						TIArmyState army2 = army;
						return (army2 == null || army2.CanEnter(destination)) && TIArmyState.IsTraversible(origin, destination, army);
					}));
					army.cachedConnectedRegions[origin] = new ValueTuple<HashSet<TIRegionState>, int>(hashSet, TIFrameCounter.FrameCount);
				}
				return hashSet;
			}
			return origin.ConnectedRegions;
		}

		// Token: 0x060035F6 RID: 13814 RVA: 0x00136B04 File Offset: 0x00134D04
		public static List<TIRegionState> GetJourney(TIRegionState origin, TIRegionState destination, out float durationInDays, Func<TIRegionState, bool> IsRegionAllowed = null, TIArmyState army = null)
		{
			Func<TIRegionState, IEnumerable<TIRegionState>> func;
			if (IsRegionAllowed == null)
			{
				func = (TIRegionState region) => TIArmyState.GetConnectedRegions(region, army);
			}
			else
			{
				func = (TIRegionState region) => region.ConnectedRegions.Where<TIRegionState>((TIRegionState x) => IsRegionAllowed(x) && TIArmyState.IsTraversible(region, x, army));
			}
			if (IsRegionAllowed == null)
			{
				IsRegionAllowed = delegate(TIRegionState region)
				{
					TIArmyState army2 = army;
					return army2 == null || army2.CanEnter(region);
				};
			}
			if (!IsRegionAllowed(destination))
			{
				durationInDays = float.PositiveInfinity;
				return null;
			}
			Dictionary<TIRegionState, TIRegionState> previousNode = new Dictionary<TIRegionState, TIRegionState>();
			Dictionary<TIRegionState, float> costToSource = new Dictionary<TIRegionState, float>();
			Dictionary<TIRegionState, float> costToDestinationEstimate = new Dictionary<TIRegionState, float>();
			HashSet<TIRegionState> hashSet = new HashSet<TIRegionState> { origin };
			previousNode[origin] = null;
			costToSource[origin] = 0f;
			costToDestinationEstimate[origin] = 0f;
			bool flag = false;
			Func<TIRegionState, float> <>9__7;
			Func<TIRegionState, bool> <>9__10;
			while (hashSet.Count > 0)
			{
				IEnumerable<TIRegionState> enumerable = hashSet;
				Func<TIRegionState, TIRegionState> func2 = (TIRegionState x) => x;
				Func<TIRegionState, float> func3;
				if ((func3 = <>9__7) == null)
				{
					func3 = (<>9__7 = (TIRegionState x) => base.<GetJourney>g__GetPathCostEstimate|3(x));
				}
				Dictionary<TIRegionState, float> dictionary = enumerable.ToDictionary<TIRegionState, TIRegionState, float>(func2, func3);
				float minimumPathCostEstimate = dictionary.Values.Min();
				IEnumerable<TIRegionState> enumerable2 = from x in dictionary
					where x.Value == minimumPathCostEstimate
					select x.Key;
				Func<TIRegionState, bool> func4;
				if ((func4 = <>9__10) == null)
				{
					func4 = (<>9__10 = (TIRegionState x) => x == destination);
				}
				TIRegionState tiregionState = enumerable2.OrderByDescending<TIRegionState, bool>(func4).First<TIRegionState>();
				hashSet.Remove(tiregionState);
				if (tiregionState == destination)
				{
					flag = true;
					break;
				}
				foreach (TIRegionState tiregionState2 in func(tiregionState))
				{
					float num = TIArmyState.GetDeploymentToAdjacentRegionDuration_Days(tiregionState, tiregionState2, army) + costToSource[tiregionState];
					if (num < 0f)
					{
						num = float.PositiveInfinity;
					}
					if (!costToSource.ContainsKey(tiregionState2) || num < costToSource[tiregionState2])
					{
						previousNode[tiregionState2] = tiregionState;
						costToSource[tiregionState2] = num;
						costToDestinationEstimate[tiregionState2] = TIArmyState.GetAdmissableHeuristicOfJourneyDurationInDays(tiregionState2, destination, army);
						hashSet.Add(tiregionState2);
					}
				}
			}
			if (!flag)
			{
				durationInDays = float.PositiveInfinity;
				return null;
			}
			List<TIRegionState> list = Utilities.LinkedList<TIRegionState>(destination, (TIRegionState node) => previousNode[node]).Reverse<TIRegionState>().ToList<TIRegionState>();
			durationInDays = costToSource[destination];
			return list;
		}

		// Token: 0x060035F7 RID: 13815 RVA: 0x00136DFC File Offset: 0x00134FFC
		public static List<TIRegionState> GetJourney(TIRegionState origin, TIRegionState destination, TIArmyState army = null)
		{
			float num;
			return TIArmyState.GetJourney(origin, destination, out num, null, army);
		}

		// Token: 0x060035F8 RID: 13816 RVA: 0x00136E14 File Offset: 0x00135014
		public List<TIRegionState> GetJourney(TIRegionState origin, TIRegionState destination, Func<TIRegionState, bool> ShouldAvoidRegion)
		{
			TIArmyState.<>c__DisplayClass185_0 CS$<>8__locals1 = new TIArmyState.<>c__DisplayClass185_0();
			CS$<>8__locals1.ShouldAvoidRegion = ShouldAvoidRegion;
			CS$<>8__locals1.<>4__this = this;
			if (origin == destination)
			{
				return new List<TIRegionState>();
			}
			List<TIRegionState> list = null;
			if (!CS$<>8__locals1.ShouldAvoidRegion(destination))
			{
				float num;
				list = TIArmyState.GetJourney(origin, destination, out num, new Func<TIRegionState, bool>(CS$<>8__locals1.<GetJourney>g__IsRegionAllowed|0), this);
			}
			if (list == null || list.Count == 0)
			{
				float num;
				list = TIArmyState.GetJourney(origin, destination, out num, null, this);
			}
			return list;
		}

		// Token: 0x060035F9 RID: 13817 RVA: 0x00136E83 File Offset: 0x00135083
		public List<TIRegionState> GetJourney_AvoidEnemyRegions(TIRegionState origin, TIRegionState destination)
		{
			return this.GetJourney(origin, destination, (TIRegionState x) => this.homeNation.wars.Contains(x.nation));
		}

		// Token: 0x060035FA RID: 13818 RVA: 0x00136E99 File Offset: 0x00135099
		public List<TIRegionState> GetJourney(TIRegionState origin, TIRegionState destination, out float durationInDays)
		{
			return TIArmyState.GetJourney(origin, destination, out durationInDays, null, this);
		}

		// Token: 0x060035FB RID: 13819 RVA: 0x00136EA8 File Offset: 0x001350A8
		public List<TIRegionState> GetJourney(TIRegionState origin, TIRegionState destination)
		{
			float num;
			return TIArmyState.GetJourney(origin, destination, out num, null, this);
		}

		// Token: 0x060035FC RID: 13820 RVA: 0x00136EC0 File Offset: 0x001350C0
		public List<TIRegionState> GetJourney(TIRegionState destination)
		{
			return TIArmyState.GetJourney(this.currentRegion, destination, this);
		}

		// Token: 0x060035FD RID: 13821 RVA: 0x00136ED0 File Offset: 0x001350D0
		public static float GetJourneyDurationInDays(TIRegionState origin, TIRegionState destination, TIArmyState army = null)
		{
			float num;
			TIArmyState.GetJourney(origin, destination, out num, null, army);
			return num;
		}

		// Token: 0x060035FE RID: 13822 RVA: 0x00136EEA File Offset: 0x001350EA
		public float GetJourneyDurationInDays(TIRegionState destination)
		{
			return TIArmyState.GetJourneyDurationInDays(this.currentRegion, destination, this);
		}

		// Token: 0x060035FF RID: 13823 RVA: 0x00136EFC File Offset: 0x001350FC
		public bool CanGetTo(TIRegionState destination, Func<TIRegionState, bool> IsRegionAllowed = null, TIRegionState origin = null, Dictionary<TIRegionState, bool> canGetToCache = null)
		{
			TIArmyState.<>c__DisplayClass192_0 CS$<>8__locals1 = new TIArmyState.<>c__DisplayClass192_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.canGetToCache = canGetToCache;
			CS$<>8__locals1.IsRegionAllowed = IsRegionAllowed;
			CS$<>8__locals1.origin = origin;
			CS$<>8__locals1.destination = destination;
			if (CS$<>8__locals1.IsRegionAllowed == null)
			{
				CS$<>8__locals1.IsRegionAllowed = (TIRegionState region) => CS$<>8__locals1.<>4__this.CanEnter(region);
			}
			if (CS$<>8__locals1.origin == null)
			{
				CS$<>8__locals1.origin = this.currentRegion;
			}
			if (CS$<>8__locals1.canGetToCache != null)
			{
				bool flag;
				if (CS$<>8__locals1.canGetToCache.TryGetValue(CS$<>8__locals1.destination, out flag))
				{
					return flag;
				}
				if (!CS$<>8__locals1.IsRegionAllowed(CS$<>8__locals1.destination))
				{
					return CS$<>8__locals1.canGetToCache[CS$<>8__locals1.destination] = false;
				}
				Func<TIRegionState, bool> IsRegionAllowed_ = CS$<>8__locals1.IsRegionAllowed;
				CS$<>8__locals1.IsRegionAllowed = delegate(TIRegionState region)
				{
					bool flag5;
					if (CS$<>8__locals1.canGetToCache.TryGetValue(region, out flag5))
					{
						return flag5;
					}
					return IsRegionAllowed_(region) || (CS$<>8__locals1.canGetToCache[region] = false);
				};
			}
			else if (!CS$<>8__locals1.IsRegionAllowed(CS$<>8__locals1.destination))
			{
				return false;
			}
			bool? flag2 = null;
			CS$<>8__locals1.originWaterBodies = new List<WaterBody>();
			CS$<>8__locals1.destinationWaterBodies = new List<WaterBody>();
			CS$<>8__locals1.canTravelNavally = this.deploymentType == DeploymentType.Naval && this.homeNation.navalFreedom;
			if (CS$<>8__locals1.canTravelNavally)
			{
				CS$<>8__locals1.originWaterBodies = CS$<>8__locals1.origin.GetAccessibleWaterBodies(this.homeNation).ToList<WaterBody>();
				CS$<>8__locals1.destinationWaterBodies = CS$<>8__locals1.destination.GetAccessibleWaterBodies(this.homeNation).ToList<WaterBody>();
			}
			if (CS$<>8__locals1.<CanGetTo>g__NavalConnectionExists|2())
			{
				flag2 = new bool?(true);
			}
			Queue<TIRegionState> queue = new Queue<TIRegionState>();
			Queue<TIRegionState> queue2 = new Queue<TIRegionState>();
			HashSet<TIRegionState> hashSet = new HashSet<TIRegionState>();
			HashSet<TIRegionState> hashSet2 = new HashSet<TIRegionState>();
			hashSet.Add(CS$<>8__locals1.origin);
			hashSet2.Add(CS$<>8__locals1.destination);
			queue.Enqueue(CS$<>8__locals1.origin);
			queue2.Enqueue(CS$<>8__locals1.destination);
			bool flag3 = true;
			while (flag2 == null && (queue.Count > 0 || queue2.Count > 0) && (queue.Count != 0 || (CS$<>8__locals1.canTravelNavally && CS$<>8__locals1.originWaterBodies.Any<WaterBody>())) && (queue2.Count != 0 || (CS$<>8__locals1.canTravelNavally && CS$<>8__locals1.destinationWaterBodies.Any<WaterBody>())))
			{
				Queue<TIRegionState> queue3 = queue;
				HashSet<TIRegionState> hashSet3 = hashSet;
				HashSet<TIRegionState> hashSet4 = hashSet2;
				if (!flag3)
				{
					queue3 = queue2;
					hashSet3 = hashSet2;
					hashSet4 = hashSet;
				}
				TIRegionState tiregionState = queue3.Dequeue();
				foreach (TIRegionState tiregionState2 in CS$<>8__locals1.<CanGetTo>g__GetValidNeighbors|1(tiregionState))
				{
					if (hashSet4.Contains(tiregionState2))
					{
						flag2 = new bool?(true);
						break;
					}
					if (!hashSet3.Contains(tiregionState2))
					{
						if (CS$<>8__locals1.canTravelNavally && tiregionState2.onTheWater)
						{
							List<WaterBody> list = tiregionState2.GetAccessibleWaterBodies(this.homeNation).ToList<WaterBody>();
							if (flag3)
							{
								CS$<>8__locals1.originWaterBodies = CS$<>8__locals1.originWaterBodies.Union<WaterBody>(list).ToList<WaterBody>();
							}
							else
							{
								CS$<>8__locals1.destinationWaterBodies = CS$<>8__locals1.destinationWaterBodies.Union<WaterBody>(list).ToList<WaterBody>();
							}
							if (CS$<>8__locals1.<CanGetTo>g__NavalConnectionExists|2())
							{
								flag2 = new bool?(true);
								break;
							}
						}
						hashSet3.Add(tiregionState2);
						queue3.Enqueue(tiregionState2);
					}
				}
				if (flag3 ? (queue2.Count > 0) : (queue.Count > 0))
				{
					flag3 = !flag3;
				}
			}
			if (flag2 == null)
			{
				if (!CS$<>8__locals1.canTravelNavally)
				{
					flag2 = new bool?(false);
				}
				foreach (IEnumerable<WaterBody> enumerable in Enumerable.Empty<IEnumerable<WaterBody>>().Append(CS$<>8__locals1.originWaterBodies).Append(CS$<>8__locals1.destinationWaterBodies))
				{
					if (flag2 != null)
					{
						break;
					}
					using (IEnumerator<WaterBody> enumerator3 = enumerable.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							WaterBody waterBody = enumerator3.Current;
							if (flag2 != null)
							{
								break;
							}
							if (waterBody != WaterBody.Ocean)
							{
								IEnumerable<TIRegionState> enterableRegions = this.EnterableRegions;
								Func<TIRegionState, bool> func;
								if ((func = CS$<>8__locals1.<>9__5) == null)
								{
									func = (CS$<>8__locals1.<>9__5 = (TIRegionState x) => x != CS$<>8__locals1.origin && x != CS$<>8__locals1.destination);
								}
								Func<TIRegionState, bool> <>9__7;
								foreach (TIRegionState tiregionState3 in (from x in enterableRegions.Where<TIRegionState>(func).Where<TIRegionState>(CS$<>8__locals1.IsRegionAllowed)
									where x.GetBorderingWaterBodies().Contains(waterBody)
									select x).ToList<TIRegionState>())
								{
									TIRegionState tiregionState4 = CS$<>8__locals1.destination;
									if (enumerable == CS$<>8__locals1.destinationWaterBodies)
									{
										tiregionState4 = CS$<>8__locals1.origin;
									}
									TIRegionState tiregionState5 = tiregionState4;
									Func<TIRegionState, bool> func2;
									if ((func2 = <>9__7) == null)
									{
										func2 = (<>9__7 = (TIRegionState x) => !x.GetBorderingWaterBodies().Contains(waterBody) && CS$<>8__locals1.IsRegionAllowed(x));
									}
									if (this.CanGetTo(tiregionState5, func2, tiregionState3, null))
									{
										flag2 = new bool?(true);
										break;
									}
								}
							}
						}
					}
				}
			}
			bool flag4 = flag2.GetValueOrDefault();
			if (flag2 == null)
			{
				flag4 = false;
				flag2 = new bool?(flag4);
			}
			if (CS$<>8__locals1.canGetToCache != null)
			{
				foreach (TIRegionState tiregionState6 in hashSet)
				{
					CS$<>8__locals1.canGetToCache[tiregionState6] = true;
				}
				foreach (TIRegionState tiregionState7 in hashSet2)
				{
					CS$<>8__locals1.canGetToCache[tiregionState7] = flag2.Value;
				}
			}
			return flag2.Value;
		}

		// Token: 0x06003600 RID: 13824 RVA: 0x00137598 File Offset: 0x00135798
		public bool CanGetTo(TIRegionState destination, bool doNotEnterEnemyRegions)
		{
			Func<TIRegionState, bool> func = null;
			if (doNotEnterEnemyRegions)
			{
				func = (TIRegionState region) => !this.homeNation.wars.Contains(region.nation) && this.CanEnter(region);
			}
			return this.CanGetTo(destination, func, null, null);
		}

		// Token: 0x06003601 RID: 13825 RVA: 0x001375C4 File Offset: 0x001357C4
		public static void BakeJourneyHeuristic()
		{
		}

		// Token: 0x06003602 RID: 13826 RVA: 0x001375D8 File Offset: 0x001357D8
		public static void FinishBakingJourneyHeuristic()
		{
			if (TIArmyState.journeyHeuristicThreads == null)
			{
				return;
			}
			foreach (Thread thread in TIArmyState.journeyHeuristicThreads)
			{
				thread.Abort();
			}
		}

		// Token: 0x06003603 RID: 13827 RVA: 0x00137630 File Offset: 0x00135830
		public void SetSeaTransitStages(TIDateTime startDate, TIDateTime completionDate, TIRegionState destinationRegion)
		{
			double num = completionDate.DifferenceInSeconds(startDate);
			this.embarkDate = new TIDateTime(startDate);
			this.embarkDate.AddSeconds(num * 0.33000001311302185);
			this.destinationSeaDate = new TIDateTime(startDate);
			this.destinationSeaDate.AddSeconds(num * 0.6700000166893005);
			TITimeEvent.CreateNewTimeEvent(this.embarkDate, this, destinationRegion, null, this.currentRegion.ArmyEmbarkEventName, true, false, TITimeQueueRepeatType.None, 1, true, false);
			TITimeEvent.CreateNewTimeEvent(this.destinationSeaDate, this, this.currentRegion, null, destinationRegion.ArmySeaTransitEventName, true, false, TITimeQueueRepeatType.None, 1, true, false);
		}

		// Token: 0x06003604 RID: 13828 RVA: 0x001376CC File Offset: 0x001358CC
		public void CancelSeaTransit()
		{
			World.Active.GetExistingManager<GameTimeManager>().CancelTimeEvent(this.currentRegion.ArmyEmbarkEventName, this, this.currentOperations[0].target, null, this.embarkDate);
			World.Active.GetExistingManager<GameTimeManager>().CancelTimeEvent(this.currentOperations[0].target.ref_region.ArmySeaTransitEventName, this, this.currentRegion, null, this.destinationSeaDate);
			this.embarkDate = null;
			this.destinationSeaDate = null;
			GameControl.eventManager.TriggerEvent(new ArmySeaTransitCancelled(this), null, new object[]
			{
				this.currentRegion,
				this.currentOperations[0].target
			});
		}

		// Token: 0x06003605 RID: 13829 RVA: 0x00137788 File Offset: 0x00135988
		public ArmySeaTransitStage SeaTransitStage()
		{
			if (this.CurrentOperations().Count > 0)
			{
				OperationData operationData = this.CurrentOperations()[0];
				if (operationData.operation.GetTemplate() is DeployArmyOperation && operationData.target != this.currentRegion && !operationData.target.ref_region.AdjacentRegions(true).Contains(this.currentRegion) && this.embarkDate != null && this.destinationSeaDate != null)
				{
					if (TITimeState.Now() < this.embarkDate)
					{
						return ArmySeaTransitStage.Embarking;
					}
					if (TITimeState.Now() < this.destinationSeaDate)
					{
						return ArmySeaTransitStage.Sea_HomeRegion;
					}
					return ArmySeaTransitStage.Sea_DestinationRegion;
				}
			}
			return ArmySeaTransitStage.None;
		}

		// Token: 0x06003606 RID: 13830 RVA: 0x00137840 File Offset: 0x00135A40
		public static TIRegionState ScoreAndSelectRegion(TIArmyState army, List<TIRegionState> regions, AIArmyDestination destination)
		{
			if (regions.Count == 0)
			{
				return null;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			switch (destination)
			{
			case AIArmyDestination.NearestSafeRegion:
				flag5 = true;
				break;
			case AIArmyDestination.NearestSafeHomeNationRegion:
				flag5 = true;
				break;
			case AIArmyDestination.NearestHomeNationRegion:
				flag5 = true;
				break;
			case AIArmyDestination.NearestEnemyRegion:
				flag3 = true;
				flag2 = true;
				flag5 = true;
				break;
			case AIArmyDestination.NearestBattle:
				flag3 = true;
				flag4 = true;
				flag5 = true;
				break;
			case AIArmyDestination.NearestOffensiveBattle:
				flag2 = true;
				flag5 = true;
				break;
			case AIArmyDestination.NearestDefensiveBattle:
				flag3 = true;
				flag = true;
				flag4 = true;
				flag5 = true;
				break;
			case AIArmyDestination.NearestBorderWithEnemy:
			case AIArmyDestination.NearestAlliedBorderWithEnemy:
				flag = true;
				flag3 = true;
				flag4 = true;
				flag2 = true;
				flag5 = true;
				break;
			case AIArmyDestination.NearestOccupiedFriendlyRegion:
				flag2 = true;
				flag4 = true;
				flag5 = true;
				break;
			case AIArmyDestination.IntermediateDestination:
				flag6 = true;
				break;
			}
			if (flag5)
			{
				Dictionary<TIRegionState, float> durations = regions.ToDictionary<TIRegionState, TIRegionState, float>((TIRegionState x) => x, (TIRegionState x) => army.currentRegion.DistanceToRegion_km(x));
				float min = durations.Values.Min();
				regions = regions.Where<TIRegionState>((TIRegionState x) => durations[x] <= min).ToList<TIRegionState>();
			}
			Dictionary<TIRegionState, float> dictionary = new Dictionary<TIRegionState, float>();
			Func<TIArmyState, bool> <>9__4;
			foreach (TIRegionState tiregionState in regions)
			{
				TINationState homeNation = army.homeNation;
				if (homeNation == null || !homeNation.wars.Contains(tiregionState.nation) || tiregionState.nation.NumNuclearWeaponsDefendingMeAgainst(army.homeNation) <= ((tiregionState.nation.capital == tiregionState) ? 0 : 1) || (!(army.faction == null) && army.faction.extremist))
				{
					float num = 1f;
					num += (float)tiregionState.NumArmiesPresent(true, true, false, false);
					if (!flag5)
					{
						float num2 = army.currentRegion.DistanceToRegion_km(tiregionState);
						num *= 200f / Mathf.Pow(num2 / 500f, 2f);
					}
					if (flag && tiregionState.terrain == TerrainType.Rugged)
					{
						num *= 100f;
					}
					if (flag2 && tiregionState.NumArmiesPresent(false, false, true, false) == 0)
					{
						num *= 100f;
					}
					if (flag6 && army.homeNation != null && (tiregionState.nation == army.homeNation || tiregionState.nation.allies.Contains(army.homeNation)))
					{
						IEnumerable<TIArmyState> armies = tiregionState.armies;
						Func<TIArmyState, bool> func;
						if ((func = <>9__4) == null)
						{
							func = (<>9__4 = delegate(TIArmyState x)
							{
								TINationState homeNation2 = x.homeNation;
								return homeNation2 != null && homeNation2.wars.Contains(army.homeNation);
							});
						}
						if (armies.None<TIArmyState>(func))
						{
							num *= 500f;
						}
					}
					if (flag3 && tiregionState.nation.capital == tiregionState)
					{
						num *= 500f;
					}
					if (tiregionState.nation == army.homeNation)
					{
						num *= 10f;
						if (flag4)
						{
							num *= 50f;
						}
					}
					dictionary.Add(tiregionState, num);
				}
			}
			if (dictionary.Count > 0)
			{
				return dictionary.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> o) => o.Value, -1f, 1E-37f).Key;
			}
			return null;
		}

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x06003607 RID: 13831 RVA: 0x00137BFC File Offset: 0x00135DFC
		public TIRegionState finalDestination
		{
			get
			{
				if (TIFrameCounter.FrameCount != this._finalDestinationFrame)
				{
					this._finalDestination = this.destinationQueue.LastOrDefault<TIRegionState>() ?? ((this.CurrentOperations().Count == 1 && this.CurrentOperations()[0].operation.GetTemplate() is DeployArmyOperation) ? this.CurrentOperations()[0].target.ref_region : null);
					this._finalDestinationFrame = TIFrameCounter.FrameCount;
				}
				return this._finalDestination;
			}
		}

		// Token: 0x06003608 RID: 13832 RVA: 0x00137C80 File Offset: 0x00135E80
		private IEnumerable<TIArmyState> EnemyArmiesInRegion(TIRegionState region)
		{
			if (TIFrameCounter.FrameCount != this._enemyArmiesCacheFrame || !this._enemyArmiesInRegion.ContainsKey(region))
			{
				this._enemyArmiesInRegion[region] = region.armies.Where<TIArmyState>((TIArmyState x) => x.homeNation.wars.Contains(this.homeNation));
				this._enemyArmiesCacheFrame = TIFrameCounter.FrameCount;
			}
			return this._enemyArmiesInRegion[region];
		}

		// Token: 0x06003609 RID: 13833 RVA: 0x00137CE4 File Offset: 0x00135EE4
		public static bool RegionMeetsDestinationCriteria(TIArmyState army, TIRegionState region, AIArmyDestination destinationType)
		{
			TIArmyState.<>c__DisplayClass209_0 CS$<>8__locals1 = new TIArmyState.<>c__DisplayClass209_0();
			CS$<>8__locals1.army = army;
			CS$<>8__locals1.region = region;
			CS$<>8__locals1.enemyArmiesInRegion = CS$<>8__locals1.army.EnemyArmiesInRegion(CS$<>8__locals1.region);
			CS$<>8__locals1.friendlyArmiesInRegion = CS$<>8__locals1.region.armies.Except<TIArmyState>(CS$<>8__locals1.enemyArmiesInRegion);
			switch (destinationType)
			{
			case AIArmyDestination.MyCapital:
				return CS$<>8__locals1.region == CS$<>8__locals1.army.homeNation.capital && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1();
			case AIArmyDestination.MyHome:
				return !CS$<>8__locals1.army.AlienMegafaunaArmy && CS$<>8__locals1.region == CS$<>8__locals1.army.homeRegion && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1();
			case AIArmyDestination.RandomAdjacentRegion:
				return CS$<>8__locals1.region.IsAdjacent(CS$<>8__locals1.army.currentRegion, true);
			case AIArmyDestination.NearestSafeRegion:
				return !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && !CS$<>8__locals1.region.Battle() && ((CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation && !CS$<>8__locals1.region.IsFullyOccupied()) || (CS$<>8__locals1.army.homeNation.allies.Contains(CS$<>8__locals1.region.nation) && !CS$<>8__locals1.region.IsFullyOccupied()));
			case AIArmyDestination.NearestSafeHomeNationRegion:
				return CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && !CS$<>8__locals1.region.Battle() && !CS$<>8__locals1.region.IsFullyOccupied();
			case AIArmyDestination.NearestHomeNationRegion:
				return CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation;
			case AIArmyDestination.NearestEnemyRegion:
				return !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyRegion|0() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__warringForTheEnemy|2() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__dontBunchUpHere|4() && (!CS$<>8__locals1.region.IsFullyOccupied() || CS$<>8__locals1.region.Battle()) && (CS$<>8__locals1.army.faction == null || CS$<>8__locals1.region.nation.executiveFaction == null || !CS$<>8__locals1.region.nation.executiveFaction.permanentAlly(CS$<>8__locals1.army.faction));
			case AIArmyDestination.NearestBattle:
				return !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__warringForTheEnemy|2() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__dontBunchUpHere|4() && (CS$<>8__locals1.region.Battle() || CS$<>8__locals1.region.OccupationUnderwayButNotComplete());
			case AIArmyDestination.NearestOffensiveBattle:
				return !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__warringForTheEnemy|2() && CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyRegion|0() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__dontBunchUpHere|4() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && (CS$<>8__locals1.region.Battle() || CS$<>8__locals1.region.OccupationUnderwayButNotComplete());
			case AIArmyDestination.NearestDefensiveBattle:
				return (CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation || CS$<>8__locals1.army.homeNation.allies.Contains(CS$<>8__locals1.region.nation)) && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__dontBunchUpHere|4() && (CS$<>8__locals1.region.Battle() || CS$<>8__locals1.region.OccupiedOrOccupationUnderway()) && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.region.armies.Any<TIArmyState>((TIArmyState x) => CS$<>8__locals1.army.homeNation.wars.Contains(x.homeNation) || (x.AlienMegafaunaArmy && x.faction != CS$<>8__locals1.army.faction));
			case AIArmyDestination.NearestBorderWithEnemy:
				if (CS$<>8__locals1.army.homeNation.atWar)
				{
					return CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.region.AdjacentRegions(true).Any<TIRegionState>((TIRegionState adjacentRegion) => CS$<>8__locals1.army.homeNation.wars.Contains(adjacentRegion.nation) && (adjacentRegion.nation.executiveFaction == null || !adjacentRegion.nation.executiveFaction.permanentAlly(CS$<>8__locals1.army.faction)));
				}
				return CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.region.AdjacentRegions(true).Any<TIRegionState>((TIRegionState adjacentRegion) => CS$<>8__locals1.army.homeNation.rivals.Contains(adjacentRegion.nation) && (adjacentRegion.nation.executiveFaction == null || !adjacentRegion.nation.executiveFaction.permanentAlly(CS$<>8__locals1.army.faction)));
			case AIArmyDestination.NearestAlliedBorderWithEnemy:
				if (CS$<>8__locals1.army.homeNation.atWar)
				{
					return CS$<>8__locals1.army.homeNation.CurrentWarAllies_AllWars().Contains(CS$<>8__locals1.region.nation) && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && (from adjacentRegion in CS$<>8__locals1.region.AdjacentRegions(true)
						where CS$<>8__locals1.army.homeNation.wars.Contains(adjacentRegion.nation) && (adjacentRegion.nation.executiveFaction == null || !adjacentRegion.nation.executiveFaction.permanentAlly(CS$<>8__locals1.army.faction))
						select adjacentRegion).Any<TIRegionState>();
				}
				return (CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation || CS$<>8__locals1.army.homeNation.allies.Contains(CS$<>8__locals1.region.nation)) && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && (from adjacentRegion in CS$<>8__locals1.region.AdjacentRegions(true)
					where CS$<>8__locals1.army.homeNation.rivals.Contains(adjacentRegion.nation) && (adjacentRegion.nation.executiveFaction == null || !adjacentRegion.nation.executiveFaction.permanentAlly(CS$<>8__locals1.army.faction))
					select adjacentRegion).Any<TIRegionState>();
			case AIArmyDestination.NearestAlliedBorderWithEnemyArmy:
				if (CS$<>8__locals1.army.homeNation.atWar)
				{
					return CS$<>8__locals1.army.homeNation.CurrentWarAllies_AllWars().Contains(CS$<>8__locals1.region.nation) && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__dontBunchUpHere|4() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.region.AdjacentRegions(true).Any<TIRegionState>((TIRegionState adjacentRegion) => CS$<>8__locals1.army.homeNation.wars.Contains(adjacentRegion.nation) && adjacentRegion.NumArmiesPresent(true, false, false, true) > 0 && (adjacentRegion.nation.executiveFaction == null || !adjacentRegion.nation.executiveFaction.permanentAlly(CS$<>8__locals1.army.faction)));
				}
				return (CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation || CS$<>8__locals1.army.homeNation.allies.Contains(CS$<>8__locals1.region.nation)) && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__dontBunchUpHere|4() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.region.AdjacentRegions(true).Any<TIRegionState>((TIRegionState adjacentRegion) => CS$<>8__locals1.army.homeNation.rivals.Contains(adjacentRegion.nation) && adjacentRegion.NumArmiesPresent(true, true, false, false) > 0 && (adjacentRegion.nation.executiveFaction == null || !adjacentRegion.nation.executiveFaction.permanentAlly(CS$<>8__locals1.army.faction)));
			case AIArmyDestination.NearestOccupiedFriendlyRegion:
				return !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__dontBunchUpHere|4() && (CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation || CS$<>8__locals1.army.homeNation.allies.Contains(CS$<>8__locals1.region.nation)) && CS$<>8__locals1.region.OccupiedOrOccupationUnderway();
			case AIArmyDestination.NearestCoast:
				return !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.region.onTheWater;
			case AIArmyDestination.NearestPotentialBreakaway:
				return !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.region.NationsWithClaim(false, true, false, false).Count > 0;
			case AIArmyDestination.IntermediateDestination:
				return CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__expectedFriendlyMass|3() > 0 || CS$<>8__locals1.enemyArmiesInRegion.Count<TIArmyState>() == 0;
			case AIArmyDestination.MegafaunaDestination:
				return CS$<>8__locals1.army.ref_megafaunaArmyState.AI_DesiredRegion(CS$<>8__locals1.region);
			case AIArmyDestination.NearestAlienFacility:
				return CS$<>8__locals1.army.faction != null && CS$<>8__locals1.region.hasAlienFacility && CS$<>8__locals1.region.alienFacility.VisibleToFaction(CS$<>8__locals1.army.faction);
			case AIArmyDestination.NearestAlienXenoformingThreat:
				if (CS$<>8__locals1.region.xenoforming.Extant() && (CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation || CS$<>8__locals1.army.homeNation.allies.Contains(CS$<>8__locals1.region.nation)) && ((CS$<>8__locals1.army.faction == null) ? (CS$<>8__locals1.region.xenoforming.xenoformingLevel >= TIRegionXenoformingState.autodetectThreshold) : CS$<>8__locals1.region.xenoforming.VisibleToFaction(CS$<>8__locals1.army.faction)) && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1())
				{
					return CS$<>8__locals1.region.armies.None<TIArmyState>((TIArmyState x) => (from x in x.CurrentOperations()
						select x.operationDataName).ToList<string>().Contains(OperationsManager.operationsLookup[typeof(AssaultAlienAssetOperation)].GetTemplate().dataName));
				}
				return false;
			case AIArmyDestination.NearestMegafaunaArmyThreat:
				return (CS$<>8__locals1.region.nation == CS$<>8__locals1.army.homeNation || CS$<>8__locals1.army.homeNation.allies.Contains(CS$<>8__locals1.region.nation)) && (CS$<>8__locals1.region.Battle() || CS$<>8__locals1.region.OccupiedOrOccupationUnderway()) && !CS$<>8__locals1.<RegionMeetsDestinationCriteria>g__enemyArmiesFromMyFactionPresent|1() && CS$<>8__locals1.region.armies.Any<TIArmyState>((TIArmyState x) => x.AlienMegafaunaArmy && x.faction != CS$<>8__locals1.army.faction);
			}
			return true;
		}

		// Token: 0x0600360A RID: 13834 RVA: 0x00138510 File Offset: 0x00136710
		public static TIRegionState FindArmyDestination(TIArmyState army, AIArmyDestination destinationType)
		{
			if (TIArmyState.RegionMeetsDestinationCriteria(army, army.currentRegion, destinationType))
			{
				return army.currentRegion;
			}
			List<TIRegionState> list = (from x in army.currentRegion.Neighbors
				where army.CanEnter(x)
				where TIArmyState.IsTraversible(army.currentRegion, x, army)
				select x).ToList<TIRegionState>();
			List<TIRegionState> list2 = list.Where<TIRegionState>((TIRegionState x) => TIArmyState.RegionMeetsDestinationCriteria(army, x, destinationType)).ToList<TIRegionState>();
			if (list2.Count > 0)
			{
				return TIArmyState.ScoreAndSelectRegion(army, list2, destinationType);
			}
			Dictionary<TIRegionState, float> dictionary = army.ReachableRegions_Fast.Where<TIRegionState>((TIRegionState x) => TIArmyState.RegionMeetsDestinationCriteria(army, x, destinationType)).ToDictionary<TIRegionState, TIRegionState, float>((TIRegionState x) => x, (TIRegionState x) => x.DistanceToRegion_km(army.currentRegion));
			TIRegionState selectedRegion = TIArmyState.ScoreAndSelectRegion(army, dictionary.Keys.ToList<TIRegionState>(), destinationType);
			if (selectedRegion == null)
			{
				return null;
			}
			if (army.AlienMegafaunaArmy)
			{
				return list.MinBy<TIRegionState, float>((TIRegionState x) => selectedRegion.GetDistanceEstimate_km(x));
			}
			List<TIRegionState> journey = TIArmyState.GetJourney(army.currentRegion, selectedRegion, army);
			if (journey == null)
			{
				return null;
			}
			return journey[1];
		}

		// Token: 0x0600360B RID: 13835 RVA: 0x00138694 File Offset: 0x00136894
		public static TIRegionState GetArmyDestination(TIArmyState army, AIArmyDestination destinationType, int numAlternatesToConsider = 4)
		{
			TIRegionState tiregionState = TIArmyState.FindArmyDestination(army, destinationType);
			int num = 0;
			while (tiregionState == null && num < numAlternatesToConsider)
			{
				AIArmyDestination aiarmyDestination;
				switch (destinationType)
				{
				case AIArmyDestination.None:
				case AIArmyDestination.MyCapital:
				case AIArmyDestination.RandomAdjacentRegion:
				case AIArmyDestination.NearestCoast:
					goto IL_0087;
				case AIArmyDestination.MyHome:
					aiarmyDestination = AIArmyDestination.MyCapital;
					break;
				case AIArmyDestination.NearestSafeRegion:
					aiarmyDestination = AIArmyDestination.MyHome;
					break;
				case AIArmyDestination.NearestSafeHomeNationRegion:
					aiarmyDestination = AIArmyDestination.NearestSafeRegion;
					break;
				case AIArmyDestination.NearestHomeNationRegion:
					aiarmyDestination = AIArmyDestination.MyHome;
					break;
				case AIArmyDestination.NearestEnemyRegion:
					aiarmyDestination = AIArmyDestination.NearestBattle;
					break;
				case AIArmyDestination.NearestBattle:
					aiarmyDestination = AIArmyDestination.NearestAlliedBorderWithEnemy;
					break;
				case AIArmyDestination.NearestOffensiveBattle:
					aiarmyDestination = AIArmyDestination.NearestEnemyRegion;
					break;
				case AIArmyDestination.NearestDefensiveBattle:
					aiarmyDestination = AIArmyDestination.NearestOccupiedFriendlyRegion;
					break;
				case AIArmyDestination.NearestBorderWithEnemy:
				case AIArmyDestination.NearestAlliedBorderWithEnemy:
					aiarmyDestination = AIArmyDestination.MyHome;
					break;
				case AIArmyDestination.NearestAlliedBorderWithEnemyArmy:
					aiarmyDestination = AIArmyDestination.NearestAlliedBorderWithEnemy;
					break;
				case AIArmyDestination.NearestOccupiedFriendlyRegion:
					aiarmyDestination = AIArmyDestination.NearestAlliedBorderWithEnemyArmy;
					break;
				default:
					goto IL_0087;
				}
				tiregionState = TIArmyState.FindArmyDestination(army, aiarmyDestination);
				destinationType = aiarmyDestination;
				num++;
				continue;
				IL_0087:
				return null;
			}
			return tiregionState;
		}

		// Token: 0x170008C6 RID: 2246
		// (get) Token: 0x0600360C RID: 13836 RVA: 0x0013874A File Offset: 0x0013694A
		public string displayNameWithArticleCapitalized
		{
			get
			{
				return Utilities.Capitalize(this.displayNameWithArticle);
			}
		}

		// Token: 0x170008C7 RID: 2247
		// (get) Token: 0x0600360D RID: 13837 RVA: 0x00138757 File Offset: 0x00136957
		public string displayNameWithNation
		{
			get
			{
				return Loc.T("TIArmyTemplate.withNation", new object[]
				{
					this.displayName,
					this.homeNation.displayName
				});
			}
		}

		// Token: 0x170008C8 RID: 2248
		// (get) Token: 0x0600360E RID: 13838 RVA: 0x00138780 File Offset: 0x00136980
		public string displayNameWithNationAndArticle
		{
			get
			{
				return Loc.T("TIArmyTemplate.withNation", new object[]
				{
					this.displayNameWithArticle,
					this.homeNation.displayName
				});
			}
		}

		// Token: 0x170008C9 RID: 2249
		// (get) Token: 0x0600360F RID: 13839 RVA: 0x001387A9 File Offset: 0x001369A9
		public string displayNameWithNationAndArticleCapitalized
		{
			get
			{
				return Utilities.Capitalize(this.displayNameWithNationAndArticle);
			}
		}

		// Token: 0x06003610 RID: 13840 RVA: 0x001387B8 File Offset: 0x001369B8
		public virtual Sprite GetTransportIcon()
		{
			int baseTechLevel = this.baseTechLevel;
			if (baseTechLevel <= 1)
			{
				return AssetCacheManager.navyTransportIcon_0;
			}
			return AssetCacheManager.navyTransportIcon_2;
		}

		// Token: 0x170008CA RID: 2250
		// (get) Token: 0x06003611 RID: 13841 RVA: 0x001387DB File Offset: 0x001369DB
		public bool UseAttackingVisuals
		{
			get
			{
				return this.IsAttacking();
			}
		}

		// Token: 0x06003612 RID: 13842 RVA: 0x001387E4 File Offset: 0x001369E4
		public virtual Sprite GetForegroundIcon()
		{
			if (this.UseAttackingVisuals)
			{
				switch (this.baseTechLevel)
				{
				case 0:
					return AssetCacheManager.humanArmy0_att;
				case 1:
					return AssetCacheManager.humanArmy1_att;
				case 2:
					return AssetCacheManager.humanArmy2_att;
				case 3:
					return AssetCacheManager.humanArmy3_att;
				case 4:
					return AssetCacheManager.humanArmy4_att;
				case 5:
					return AssetCacheManager.humanArmy5_att;
				case 6:
					return AssetCacheManager.humanArmy6_att;
				}
				return AssetCacheManager.humanArmy7_att;
			}
			switch (this.baseTechLevel)
			{
			case 0:
				return AssetCacheManager.humanArmy0_def;
			case 1:
				return AssetCacheManager.humanArmy1_def;
			case 2:
				return AssetCacheManager.humanArmy2_def;
			case 3:
				return AssetCacheManager.humanArmy3_def;
			case 4:
				return AssetCacheManager.humanArmy4_def;
			case 5:
				return AssetCacheManager.humanArmy5_def;
			case 6:
				return AssetCacheManager.humanArmy6_def;
			}
			return AssetCacheManager.humanArmy7_def;
		}

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x06003613 RID: 13843 RVA: 0x001388B8 File Offset: 0x00136AB8
		public virtual string GetIconForegroundResource
		{
			get
			{
				if (this.UseAttackingVisuals)
				{
					switch (this.baseTechLevel)
					{
					case 0:
						return TIGlobalConfig.globalConfig.pathArmy0_attacking;
					case 1:
						return TIGlobalConfig.globalConfig.pathArmy1_attacking;
					case 2:
						return TIGlobalConfig.globalConfig.pathArmy2_attacking;
					case 3:
						return TIGlobalConfig.globalConfig.pathArmy3_attacking;
					case 4:
						return TIGlobalConfig.globalConfig.pathArmy4_attacking;
					case 5:
						return TIGlobalConfig.globalConfig.pathArmy5_attacking;
					case 6:
						return TIGlobalConfig.globalConfig.pathArmy6_attacking;
					}
					return TIGlobalConfig.globalConfig.pathArmy7_attacking;
				}
				switch (this.baseTechLevel)
				{
				case 0:
					return TIGlobalConfig.globalConfig.pathArmy0_defending;
				case 1:
					return TIGlobalConfig.globalConfig.pathArmy1_defending;
				case 2:
					return TIGlobalConfig.globalConfig.pathArmy2_defending;
				case 3:
					return TIGlobalConfig.globalConfig.pathArmy3_defending;
				case 4:
					return TIGlobalConfig.globalConfig.pathArmy4_defending;
				case 5:
					return TIGlobalConfig.globalConfig.pathArmy5_defending;
				case 6:
					return TIGlobalConfig.globalConfig.pathArmy6_defending;
				}
				return TIGlobalConfig.globalConfig.pathArmy7_defending;
			}
		}

		// Token: 0x170008CC RID: 2252
		// (get) Token: 0x06003614 RID: 13844 RVA: 0x001389DD File Offset: 0x00136BDD
		public Sprite GetIconBackgroundSprite
		{
			get
			{
				return AssetCacheManager.armyIconBackground;
			}
		}

		// Token: 0x170008CD RID: 2253
		// (get) Token: 0x06003615 RID: 13845 RVA: 0x001389E4 File Offset: 0x00136BE4
		public string GetIconBackgroundResource
		{
			get
			{
				return TemplateManager.global.pathArmyIconBackground;
			}
		}

		// Token: 0x06003616 RID: 13846 RVA: 0x001389F0 File Offset: 0x00136BF0
		public virtual string GetModelResource()
		{
			if (this.faction != null)
			{
				if (this.homeNation.alienNation || (this.faction.IsAlienFaction && this.armyType == ArmyType.Human))
				{
					if (GameStateManager.AlienNation() != null)
					{
						return new StringBuilder("3dearthmodels/Tech_LVL_").Append(this.baseTechLevel).Append("_").Append(GameStateManager.AlienNation().template.tankSeries[this.baseTechLevel])
							.Append(GameStateManager.AlienProxy().template.armySkinBase)
							.ToString();
					}
					return new StringBuilder("3dearthmodels/Tech_LVL_").Append(this.baseTechLevel).Append("_RUS").Append(GameStateManager.AlienProxy().template.armySkinBase)
						.ToString();
				}
				else
				{
					if (this.homeRegion != null && this.homeRegion.nation != null)
					{
						return new StringBuilder("3dearthmodels/Tech_LVL_").Append(this.baseTechLevel).Append("_").Append(this.homeRegion.nation.template.tankSeries[this.baseTechLevel])
							.Append(this.faction.template.armySkinBase)
							.ToString();
					}
					Log.Error("Tank series not properly defined, defaulting to _RUS", Array.Empty<object>());
					return new StringBuilder("3dearthmodels/Tech_LVL_").Append(this.baseTechLevel).Append("_RUS").Append(this.faction.template.armySkinBase)
						.ToString();
				}
			}
			else
			{
				if (this.homeRegion != null && this.homeRegion.nation != null)
				{
					return new StringBuilder("3dearthmodels/Tech_LVL_").Append(this.baseTechLevel).Append("_").Append(this.homeRegion.nation.template.tankSeries[this.baseTechLevel])
						.Append("_undefined")
						.ToString();
				}
				Log.Error("Tank series not properly defined, defaulting to _RUS", Array.Empty<object>());
				return new StringBuilder("3dearthmodels/Tech_LVL_").Append(this.baseTechLevel).Append("_RUS").Append("_undefined")
					.ToString();
			}
		}

		// Token: 0x170008CE RID: 2254
		// (get) Token: 0x06003617 RID: 13847 RVA: 0x00138C43 File Offset: 0x00136E43
		public Color GetIconBackgroundResourceColor
		{
			get
			{
				if (!(this.faction == null))
				{
					return this.faction.template.color;
				}
				return TIArmyState.genericBackgroundColor;
			}
		}

		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x06003618 RID: 13848 RVA: 0x00138C6C File Offset: 0x00136E6C
		public virtual string AnimatorResource
		{
			get
			{
				int num;
				string text;
				if (this.UseAttackingVisuals)
				{
					num = this.baseTechLevel;
					if (num != 5)
					{
						if (num != 6)
						{
							text = new StringBuilder("TechLvl").Append(this.baseTechLevel.ToString()).Append("_army_att_animator").ToString();
						}
						else
						{
							text = new StringBuilder("TechLvl5_army_att_animator").ToString();
						}
					}
					else
					{
						text = new StringBuilder("TechLvl6_army_att_animator").ToString();
					}
					return text;
				}
				num = this.baseTechLevel;
				if (num != 5)
				{
					if (num != 6)
					{
						text = new StringBuilder("TechLvl").Append(this.baseTechLevel.ToString()).Append("_army_def_animator").ToString();
					}
					else
					{
						text = new StringBuilder("TechLvl5_army_def_animator").ToString();
					}
				}
				else
				{
					text = new StringBuilder("TechLvl6_army_def_animator").ToString();
				}
				return text;
			}
		}

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x06003619 RID: 13849 RVA: 0x00138D48 File Offset: 0x00136F48
		public virtual string FightingSpriteSheet
		{
			get
			{
				int num;
				string text;
				if (this.UseAttackingVisuals)
				{
					num = this.baseTechLevel;
					if (num != 5)
					{
						if (num != 6)
						{
							text = new StringBuilder("SpriteSheet_TechLvl").Append(this.baseTechLevel.ToString()).Append("_army_att").ToString();
						}
						else
						{
							text = new StringBuilder("SpriteSheet_TechLvl5_army_att").ToString();
						}
					}
					else
					{
						text = new StringBuilder("SpriteSheet_TechLvl6_army_att").ToString();
					}
					return text;
				}
				num = this.baseTechLevel;
				if (num != 5)
				{
					if (num != 6)
					{
						text = new StringBuilder("SpriteSheet_TechLvl").Append(this.baseTechLevel.ToString()).Append("_army_def2").ToString();
					}
					else
					{
						text = new StringBuilder("SpriteSheet_TechLvl5_army_def2").ToString();
					}
				}
				else
				{
					text = new StringBuilder("SpriteSheet_TechLvl6_army_def2").ToString();
				}
				return text;
			}
		}

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x0600361A RID: 13850 RVA: 0x00138E24 File Offset: 0x00137024
		public virtual string MovingSpriteSheet
		{
			get
			{
				int num;
				string text;
				if (this.UseAttackingVisuals)
				{
					num = this.baseTechLevel;
					if (num != 5)
					{
						if (num != 6)
						{
							text = new StringBuilder("SpriteSheet_TechLvl").Append(this.baseTechLevel.ToString()).Append("_army_att2").ToString();
						}
						else
						{
							text = new StringBuilder("SpriteSheet_TechLvl5_army_att2").ToString();
						}
					}
					else
					{
						text = new StringBuilder("SpriteSheet_TechLvl6_army_att2").ToString();
					}
					return text;
				}
				num = this.baseTechLevel;
				if (num != 5)
				{
					if (num != 6)
					{
						text = new StringBuilder("SpriteSheet_TechLvl").Append(this.baseTechLevel.ToString()).Append("_army_def").ToString();
					}
					else
					{
						text = new StringBuilder("SpriteSheet_TechLvl5_army_def").ToString();
					}
				}
				else
				{
					text = new StringBuilder("SpriteSheet_TechLvl6_army_def").ToString();
				}
				return text;
			}
		}

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x0600361B RID: 13851 RVA: 0x00138F00 File Offset: 0x00137100
		public virtual string illustration
		{
			get
			{
				if (this.AlienMegafaunaArmy)
				{
					return TemplateManager.global.illus_xenofaunaArmy;
				}
				if (this.armyType == ArmyType.AlienInvader)
				{
					if (!this.atSea)
					{
						return TemplateManager.global.illus_alienArmy;
					}
					return TemplateManager.global.illus_alienNavyTransport;
				}
				else
				{
					if (this.atSea)
					{
						switch (this.baseTechLevel)
						{
						case 0:
						case 1:
							return TemplateManager.global.illus_humanNavyTransport0;
						case 2:
						case 3:
						case 4:
						case 5:
							return TemplateManager.global.illus_humanNavyTransport2;
						}
						return TemplateManager.global.illus_humanNavyTransport6;
					}
					switch (this.baseTechLevel)
					{
					case 0:
						return TemplateManager.global.illus_humanArmy0;
					case 1:
						return TemplateManager.global.illus_humanArmy1;
					case 2:
						return TemplateManager.global.illus_humanArmy2;
					case 3:
						return TemplateManager.global.illus_humanArmy3;
					case 4:
						return TemplateManager.global.illus_humanArmy4;
					case 5:
						return TemplateManager.global.illus_humanArmy5;
					case 6:
						return TemplateManager.global.illus_humanArmy6;
					}
					return TemplateManager.global.illus_humanArmy7;
				}
			}
		}

		// Token: 0x0600361C RID: 13852 RVA: 0x00139028 File Offset: 0x00137228
		public void SetArmyDataDirty()
		{
			EventManager eventManager = GameControl.eventManager;
			GameEvent gameEvent = new ArmyStatusUpdate(this, null);
			string armyStatusUpdateEventName = this.armyStatusUpdateEventName;
			object[] array = new object[4];
			array[0] = this;
			array[1] = this.currentRegion;
			int num = 2;
			TIRegionState tiregionState = this.homeRegion;
			array[num] = ((tiregionState != null) ? tiregionState.nation : null);
			array[3] = this.priorRegion;
			eventManager.TriggerEvent(gameEvent, armyStatusUpdateEventName, array);
		}

		// Token: 0x0600361D RID: 13853 RVA: 0x00139080 File Offset: 0x00137280
		public void AssignToFaction(TIFactionState newCouncil, bool alienMegafaunaTakeover = false)
		{
			if (this.armyType == ArmyType.Human || newCouncil == GameStateManager.AlienFaction() || alienMegafaunaTakeover)
			{
				TIFactionState tifactionState = this.faction;
				if (newCouncil != this.faction)
				{
					this.faction = newCouncil;
					TIFactionState tifactionState2 = this.faction;
					if (tifactionState2 != null && tifactionState2.isActivePlayer && alienMegafaunaTakeover)
					{
						this.faction.UnlockAchievement("controlMegafauna");
					}
					if (newCouncil != null)
					{
						newCouncil.armies.Add(this);
					}
					if (tifactionState != null)
					{
						tifactionState.armies.Remove(this);
					}
					this.SetHuntingXenofauna(false);
					this.SetArmyDataDirty();
					GameControl.eventManager.TriggerEvent(new ArmyAssignedToFaction(this, this.faction), null, new object[] { this, this.faction, tifactionState });
					GameControl.eventManager.TriggerEvent(new ArmyStatusUpdate(this, null), null, new object[] { this, this.faction, tifactionState });
					TINotificationQueueState.LogArmyAssignedToFaction(this, tifactionState);
					AIDailyFactionPlanner.AIReaction(AIReactionEvent.NewArmyGained, this, null);
				}
			}
		}

		// Token: 0x0600361E RID: 13854 RVA: 0x00139187 File Offset: 0x00137387
		public void AddNavy()
		{
			this.deploymentType = DeploymentType.Naval;
			TINationState homeNation = this.homeNation;
			if (homeNation != null)
			{
				homeNation.SetArmyAccessibilityDirty();
			}
			this.SetArmyDataDirty();
		}

		// Token: 0x0600361F RID: 13855 RVA: 0x001391A7 File Offset: 0x001373A7
		public void Rename(string newName, string newNameWithArticle)
		{
			if (this.displayName != newName)
			{
				this.displayName = newName;
				this.displayNameWithArticle = newNameWithArticle;
			}
		}

		// Token: 0x06003620 RID: 13856 RVA: 0x001391C8 File Offset: 0x001373C8
		public bool TakeDamage(float amount, TIFactionState attacker, TINationState attackingNation, bool allowReformingOfHumanArmies)
		{
			if (this.strength <= 0f)
			{
				return true;
			}
			float num = this.strength;
			this.strength -= amount;
			this.strength = Mathf.Clamp(this.strength, 0f, 1f);
			if (amount > 0f)
			{
				GameControl.eventManager.TriggerEvent(new ArmyTakesDamage(this), this.armyDamageEventName, new object[] { this, this.currentRegion });
				if (attackingNation != null)
				{
					foreach (TIWarState tiwarState in this.homeNation.currentWarStates.Where<TIWarState>((TIWarState x) => x.allBelligerents.Contains(attackingNation)))
					{
						tiwarState.FightingOccurs();
					}
				}
			}
			if (this.strength <= 0f)
			{
				TINotificationQueueState.LogArmyIsDestroyed(this, this.currentRegion, attacker);
				if (this.faction != null)
				{
					if (!this.faction.armiesLost.ContainsKey(this.armyType))
					{
						this.faction.armiesLost.Add(this.armyType, 1);
					}
					else
					{
						Dictionary<ArmyType, int> armiesLost = this.faction.armiesLost;
						ArmyType armyType = this.armyType;
						armiesLost[armyType]++;
					}
				}
				if ((this.faction == null || this.faction.IsActiveHumanFaction) && this.homeNation != null)
				{
					this.homeNation.AddToCohesion(-this.homeNation.democracy / 10f, TINationState.CohesionChangeReason.CohesionReason_ArmyLost);
					if (this.faction != null)
					{
						this.homeNation.PropagandaOnPop(this.faction.ideology, -this.homeNation.democracy, false);
					}
				}
				if (this.armyType == ArmyType.Human)
				{
					if (this.currentRegion.nation == this.homeNation)
					{
						if (allowReformingOfHumanArmies)
						{
							this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildArmy, 0.3f + TIUtilities.RandomFloatValue() * 0.2f, true);
						}
						if (this.deploymentType == DeploymentType.Naval)
						{
							if (this.homeNation.navalFreedom)
							{
								if (this.currentRegion.onTheWater)
								{
									this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.7f + TIUtilities.RandomFloatValue() * 0.2f, true);
								}
								else
								{
									this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.9f + TIUtilities.RandomFloatValue() * 0.2f, true);
								}
							}
							else
							{
								this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.2f + TIUtilities.RandomFloatValue() * 0.1f, true);
							}
						}
					}
					else
					{
						if (allowReformingOfHumanArmies)
						{
							this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildArmy, 0.15f + TIUtilities.RandomFloatValue() * 0.1f, true);
						}
						if (this.deploymentType == DeploymentType.Naval)
						{
							if (this.homeNation.navalFreedom)
							{
								if (this.currentRegion.onTheWater)
								{
									this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.6f + TIUtilities.RandomFloatValue() * 0.2f, true);
								}
								else
								{
									this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.8f + TIUtilities.RandomFloatValue() * 0.2f, true);
								}
							}
							else
							{
								this.homeNation.ModifyAccumulatedInvestmentFractional(PriorityType.Military_BuildNavy, 0.1f + TIUtilities.RandomFloatValue() * 0.1f, true);
							}
						}
					}
				}
				if (attacker != null)
				{
					switch (this.armyType)
					{
					case ArmyType.Human:
						if (this.homeNation.alienNation && this.techLevel >= 6f)
						{
							attacker.CompleteMilestone(CampaignMilestone.AccessAlienTech);
							if (TIUtilities.RandomFloatValue() < 0.5f)
							{
								attacker.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
							}
							else if (TIUtilities.RandomFloatValue() < 0.1f)
							{
								attacker.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
							}
						}
						break;
					case ArmyType.AlienMegafauna:
						attacker.CompleteMilestone(CampaignMilestone.AccessAlienMegafauna);
						if (attacker.isActivePlayer)
						{
							attacker.UnlockAchievement("destroyMegafauna");
						}
						break;
					case ArmyType.AlienInvader:
						attacker.CompleteMilestone(CampaignMilestone.AccessAlienTech);
						attacker.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
						attacker.CompleteMilestone(CampaignMilestone.AccessWarDogCorpus);
						attacker.CompleteMilestone(CampaignMilestone.AlienArmyDestroyed);
						if (TIUtilities.RandomFloatValue() < 0.4f)
						{
							attacker.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
						}
						if (attacker.isActivePlayer)
						{
							attacker.UnlockAchievement("destroyAlienArmy");
						}
						break;
					}
				}
				if (this.armyType == ArmyType.AlienInvader && GameStateManager.AlienNation().regions.Count == 0)
				{
					if (GameStateManager.AlienFaction().armies.Count<TIArmyState>((TIArmyState x) => x.armyType == ArmyType.AlienInvader) <= 1)
					{
						foreach (TIWarState tiwarState2 in this.homeNation.currentWarStates)
						{
							TINationState.EndFullWar(GameStateManager.AlienFaction(), tiwarState2, true, false);
						}
					}
				}
				if (attacker != null)
				{
					attacker.RegisterKill(this, 1f);
				}
				this.Disband();
				return true;
			}
			if (this.armyType == ArmyType.Human && num >= 0.25f && this.strength < 0.25f)
			{
				TINotificationQueueState.LogMyArmyBadlyDamaged(this);
			}
			this.SetArmyDataDirty();
			return false;
		}

		// Token: 0x06003621 RID: 13857 RVA: 0x001396F0 File Offset: 0x001378F0
		public void HealDamage()
		{
			if (this.strength < 1f)
			{
				this.strength += this.dailyHealRate;
				this.strength = Mathf.Clamp(this.strength, 0f, 1f);
				this.SetArmyDataDirty();
			}
		}

		// Token: 0x06003622 RID: 13858 RVA: 0x00139740 File Offset: 0x00137940
		public float AttemptRepair(float amountToAttempt)
		{
			float num = Mathf.Min(1f - this.strength, amountToAttempt);
			if (num > 0f)
			{
				this.SetStrength(this.strength + num);
			}
			return num;
		}

		// Token: 0x06003623 RID: 13859 RVA: 0x00139777 File Offset: 0x00137977
		public void SetStrength(float value)
		{
			this.strength = Mathf.Clamp(value, 0f, 1f);
			this.SetArmyDataDirty();
		}

		// Token: 0x06003624 RID: 13860 RVA: 0x00139798 File Offset: 0x00137998
		public void MoveArmyToRegion(TIRegionState newRegion, bool newArmy = false)
		{
			TIRegionState tiregionState = this.priorRegion;
			if (!newArmy)
			{
				this.SetArmyDataDirty();
				this.priorRegion = this.currentRegion;
				this.currentRegion.armies.Remove(this);
				this.ClearOperations();
			}
			else
			{
				this.priorRegion = newRegion;
			}
			this.currentRegion = newRegion;
			this.embarkDate = null;
			this.destinationSeaDate = null;
			this.SetNotMoving();
			newRegion.armies.Add(this);
			if (this.AI_targetEnemyRegion == newRegion)
			{
				this.AI_targetEnemyRegion = null;
			}
			GameControl.eventManager.TriggerEvent(new ArmyArrivesInRegion(this, newRegion), null, new object[] { this, tiregionState, this.priorRegion, newRegion });
			this.CheckAndPromptIfInIllegalRegion(true, false);
			if (this.currentRegion == newRegion)
			{
				AIDailyFactionPlanner.AIReaction(AIReactionEvent.ArmyEntersMyRegion, newRegion, this);
				if (this.InBattleWithArmies())
				{
					TINotificationQueueState.LogEnemyArmyJoinsBattle(this);
				}
				if (newRegion.antiSpaceDefenses && this.FriendlyRegion(newRegion))
				{
					foreach (TISpaceFleetState tispaceFleetState in newRegion.ref_spaceBody.fleetsInInterfaceOrbits)
					{
						if (tispaceFleetState.bombarding && tispaceFleetState.bombardmentTarget == this)
						{
							AIDailyFactionPlanner.AIReaction(AIReactionEvent.BombardmentTargetEntersDangerZone, tispaceFleetState, this);
							TINotificationQueueState.LogBombardingFleetEntersDangerZone(tispaceFleetState);
						}
					}
				}
			}
		}

		// Token: 0x06003625 RID: 13861 RVA: 0x001398F4 File Offset: 0x00137AF4
		public void GoHome()
		{
			this.MoveArmyToRegion(this.homeRegion, false);
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06003626 RID: 13862 RVA: 0x00139903 File Offset: 0x00137B03
		public float LEOHabBonus
		{
			get
			{
				TIFactionState tifactionState = this.faction;
				if (tifactionState == null)
				{
					return 0f;
				}
				return tifactionState.ArmyCombatBonus;
			}
		}

		// Token: 0x170008D4 RID: 2260
		// (get) Token: 0x06003627 RID: 13863 RVA: 0x0013991A File Offset: 0x00137B1A
		public virtual float adjustedTechLevel
		{
			get
			{
				return this.techLevel + this.homeNation.adviserCommandBonus + this.LEOHabBonus;
			}
		}

		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06003628 RID: 13864 RVA: 0x00139935 File Offset: 0x00137B35
		public float combatEffectiveness
		{
			get
			{
				return 1f - (1f - this.strength) * TemplateManager.global.battleDamageEffectivenessFactor;
			}
		}

		// Token: 0x06003629 RID: 13865 RVA: 0x00139954 File Offset: 0x00137B54
		public float GetEffectiveCombatStrength()
		{
			return this.adjustedTechLevel * this.combatEffectiveness;
		}

		// Token: 0x0600362A RID: 13866 RVA: 0x00139964 File Offset: 0x00137B64
		public string CombatBreakdown_Army()
		{
			StringBuilder stringBuilder = new StringBuilder();
			float attackValue = this.GetAttackValue();
			stringBuilder.AppendLine(Loc.T("UI.Army.BD_TotalAttackValue", new object[] { attackValue.ToString("N3") }));
			stringBuilder.AppendLine(Loc.T("UI.Army.BD_Miltech", new object[] { this.techLevel.ToString("N3") }));
			if (this.homeNation.adviserCommandBonus > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_Advisers", new object[] { this.homeNation.adviserCommandBonus }));
			}
			if (this.LEOHabBonus > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_LEOHabBonus", new object[] { this.LEOHabBonus }));
			}
			float num = this.adjustedTechLevel;
			TIControlPoint ref_controlPoint = this.ref_controlPoint;
			if (ref_controlPoint != null && ref_controlPoint.benefitsDisabled)
			{
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_Crackdown", new object[] { TemplateManager.global.armyCrackdownMalus }));
				num -= TemplateManager.global.armyCrackdownMalus;
			}
			if (this.homeNation.regions.Contains(this.currentRegion))
			{
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_HomeAdvantage", new object[] { TemplateManager.global.armyRegionDefenseBonus }));
				num += TemplateManager.global.armyRegionDefenseBonus;
				if (this.currentRegion.terrain == TerrainType.Rugged)
				{
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_Rugged", new object[] { TemplateManager.global.ruggedTerrainDefenseBonus }));
					num += TemplateManager.global.ruggedTerrainDefenseBonus;
				}
				if (this.currentRegion.coreEconomicRegion)
				{
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_CoreEco", new object[] { TemplateManager.global.coreEconomicRegionDefenseBonus }));
					num += TemplateManager.global.coreEconomicRegionDefenseBonus;
				}
			}
			float num2 = TIEffectsState.SumEffectsModifiers(Context.ArmyRuggedWarfare, this.faction, num, null);
			if (this.currentRegion.terrain == TerrainType.Rugged && num2 > 0f)
			{
				num += num2;
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_RuggedProjects", new object[] { num2 }));
			}
			float num3 = TIEffectsState.SumEffectsModifiers(Context.ArmyUrbanWarfare, this.faction, num, null);
			if (this.currentRegion.coreEconomicRegion && num3 > 0f)
			{
				num += num3;
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_UrbanProjects", new object[] { num3 }));
			}
			float fightingInFriendlyRegionBonus = this.FightingInFriendlyRegionBonus;
			if (fightingInFriendlyRegionBonus > 0f)
			{
				num += fightingInFriendlyRegionBonus;
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_Cohesion", new object[] { fightingInFriendlyRegionBonus.ToString("N2") }));
			}
			if (num - attackValue > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_Losses", new object[] { (num - attackValue).ToString("N3") }));
			}
			if (TIGameState.Valid(this.lastEnemyArmy) && this.InBattleWithArmies())
			{
				float enemyDefendValue = this.GetEnemyDefendValue(this.lastEnemyArmy);
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_HitChance", new object[] { this.GetCombatSuccessChance(attackValue, enemyDefendValue).ToPercent("P0") }));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_LastShot", new object[] { this.lastEnemyArmy.displayName }));
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_TotalDefendValue", new object[] { enemyDefendValue.ToString("N3") }));
				stringBuilder.AppendLine(Loc.T("UI.Army.BD_Miltech", new object[] { this.lastEnemyArmy.techLevel.ToString("N3") }));
				if (this.lastEnemyArmy.homeNation.adviserCommandBonus > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_Advisers", new object[] { this.lastEnemyArmy.homeNation.adviserCommandBonus }));
				}
				if (this.lastEnemyArmy.LEOHabBonus > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_LEOHabBonus", new object[] { this.lastEnemyArmy.LEOHabBonus }));
				}
				float num4 = this.lastEnemyArmy.adjustedTechLevel;
				TIControlPoint ref_controlPoint2 = this.lastEnemyArmy.ref_controlPoint;
				if (ref_controlPoint2 != null && ref_controlPoint2.benefitsDisabled)
				{
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_Crackdown", new object[] { TemplateManager.global.armyCrackdownMalus }));
					num4 -= TemplateManager.global.armyCrackdownMalus;
				}
				if (this.lastEnemyArmy.homeNation.regions.Contains(this.currentRegion))
				{
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_HomeAdvantage", new object[] { TemplateManager.global.armyRegionDefenseBonus }));
					num4 += TemplateManager.global.armyRegionDefenseBonus;
					if (this.currentRegion.terrain == TerrainType.Rugged)
					{
						stringBuilder.AppendLine(Loc.T("UI.Army.BD_Rugged", new object[] { TemplateManager.global.ruggedTerrainDefenseBonus }));
						num4 += TemplateManager.global.ruggedTerrainDefenseBonus;
					}
					if (this.currentRegion.coreEconomicRegion)
					{
						stringBuilder.AppendLine(Loc.T("UI.Army.BD_CoreEco", new object[] { TemplateManager.global.coreEconomicRegionDefenseBonus }));
						num4 += TemplateManager.global.coreEconomicRegionDefenseBonus;
					}
				}
				float num5 = TIEffectsState.SumEffectsModifiers(Context.ArmyRuggedWarfare, this.lastEnemyArmy.faction, num4, null);
				if (this.currentRegion.terrain == TerrainType.Rugged && num5 > 0f)
				{
					num4 += num5;
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_RuggedProjects", new object[] { num5 }));
				}
				float num6 = TIEffectsState.SumEffectsModifiers(Context.ArmyUrbanWarfare, this.lastEnemyArmy.faction, num4, null);
				if (this.currentRegion.coreEconomicRegion && num6 > 0f)
				{
					num4 += num6;
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_UrbanProjects", new object[] { num6 }));
				}
				if (num4 - enemyDefendValue > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_Losses", new object[] { (num4 - enemyDefendValue).ToString("N3") }));
				}
			}
			else if (this.OccupyingRegion(true))
			{
				TINationState tinationState;
				if ((tinationState = this.currentRegion.leadOccupier) == null)
				{
					tinationState = this.currentRegion.occupations.MaxBy<KeyValuePair<TINationState, float>, float>((KeyValuePair<TINationState, float> x) => x.Value).Key;
				}
				TINationState tinationState2 = tinationState;
				float num7 = this.LocalForcesBaseDefenseLevel(true, tinationState2);
				bool flag = this.FriendlyRegion(this.currentRegion);
				stringBuilder.AppendLine(Loc.T(flag ? "UI.Army.OccupationDecreaseChance" : "UI.Army.OccupationIncreaseChance", new object[] { this.GetCombatSuccessChance(attackValue, num7).ToPercent("P0") }));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(Loc.T(flag ? "UI.Army.BD_OccupationForces" : "UI.Army.BD_LocalForces", new object[] { num7.ToString("N3") }));
				TINationState getOccupierNation = this.currentRegion.GetOccupierNation;
				if (num7 > 0f)
				{
					float num8 = (flag ? getOccupierNation.militaryTechLevel : this.currentNation.militaryTechLevel);
					stringBuilder.AppendLine(Loc.T("UI.Army.BD_Miltech", new object[] { num8.ToString("N3") }));
					float num9 = (flag ? getOccupierNation.adviserCommandBonus : this.currentNation.adviserCommandBonus);
					if (num9 > 0f)
					{
						stringBuilder.AppendLine(Loc.T("UI.Army.BD_Advisers", new object[] { num9 }));
						num8 += num9;
					}
					if (!flag)
					{
						stringBuilder.AppendLine(Loc.T("UI.Army.BD_Defender", new object[] { TemplateManager.global.baseRegionDefenseBonus.ToString("N3") }));
						num8 += TemplateManager.global.baseRegionDefenseBonus;
						if (this.currentRegion.terrain == TerrainType.Rugged)
						{
							stringBuilder.AppendLine(Loc.T("UI.Army.BD_Rugged", new object[] { TemplateManager.global.ruggedTerrainDefenseBonus.ToString("N2") }));
							num8 += TemplateManager.global.ruggedTerrainDefenseBonus;
							float num10 = TIEffectsState.SumEffectsModifiers(Context.ArmyRuggedWarfare, this.currentNation.executiveFaction, 0f, null);
							if (num10 > 0f)
							{
								stringBuilder.AppendLine(Loc.T("UI.Army.BD_RuggedProjects", new object[] { num10 }));
								num8 += num10;
							}
						}
						if (this.currentRegion.coreEconomicRegion)
						{
							stringBuilder.AppendLine(Loc.T("UI.Army.BD_CoreEco", new object[] { TemplateManager.global.coreEconomicRegionDefenseBonus.ToString("N3") }));
							float num11 = TIEffectsState.SumEffectsModifiers(Context.ArmyUrbanWarfare, this.currentNation.executiveFaction, 0f, null);
							if (num11 > 0f)
							{
								stringBuilder.AppendLine(Loc.T("UI.Army.BD_UrbanProjects", new object[] { num11 }));
								num8 += num11;
							}
						}
						float num12 = this.currentNation.cohesion * TemplateManager.global.defenseCohesionMultiplier;
						num8 += num12;
						if (num12 != 0f)
						{
							stringBuilder.AppendLine(Loc.T("UI.Army.BD_Cohesion", new object[] { num12.ToString("N2") }));
						}
						float num13 = this.currentNation.unrest * TemplateManager.global.defenseUnrestMultiplier;
						num8 += num13;
						if (num13 != 0f)
						{
							stringBuilder.AppendLine(Loc.T("UI.Army.BD_Unrest", new object[] { num13.ToString("N2") }));
						}
						float num14 = TIArmyState.LocalForcesAdjacentRegionsBonus(this.currentRegion);
						if (num14 != 0f)
						{
							stringBuilder.AppendLine(Loc.T("UI.Army.BD_AdjacentForces", new object[] { num14.ToString("N2") }));
							num8 += num14;
						}
					}
					if (num8 - num7 > 0f)
					{
						stringBuilder.AppendLine(Loc.T("UI.Army.BD_LFLosses", new object[] { (num8 - num7).ToString("N3") }));
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x170008D6 RID: 2262
		// (get) Token: 0x0600362B RID: 13867 RVA: 0x0013A3FE File Offset: 0x001385FE
		public float FightingInFriendlyRegionBonus
		{
			get
			{
				if (!this.InFriendlyRegion)
				{
					return 0f;
				}
				return this.currentNation.cohesion * TemplateManager.global.defenseCohesionMultiplier;
			}
		}

		// Token: 0x0600362C RID: 13868 RVA: 0x0013A424 File Offset: 0x00138624
		public float GetAttackValue()
		{
			float num = this.adjustedTechLevel;
			if (this.homeNation.regions.Contains(this.currentRegion))
			{
				num += TemplateManager.global.armyRegionDefenseBonus;
				if (this.currentRegion.terrain == TerrainType.Rugged)
				{
					num += TemplateManager.global.ruggedTerrainDefenseBonus;
				}
				if (this.currentRegion.coreEconomicRegion)
				{
					num += TemplateManager.global.coreEconomicRegionDefenseBonus;
				}
			}
			TIControlPoint ref_controlPoint = this.ref_controlPoint;
			if (ref_controlPoint != null && ref_controlPoint.benefitsDisabled)
			{
				num -= TemplateManager.global.armyCrackdownMalus;
			}
			if (this.currentRegion.terrain == TerrainType.Rugged)
			{
				num += TIEffectsState.SumEffectsModifiers(Context.ArmyRuggedWarfare, this.faction, num, null);
			}
			if (this.currentRegion.coreEconomicRegion)
			{
				num += TIEffectsState.SumEffectsModifiers(Context.ArmyUrbanWarfare, this.faction, num, null);
			}
			num += this.FightingInFriendlyRegionBonus;
			return num * this.combatEffectiveness;
		}

		// Token: 0x0600362D RID: 13869 RVA: 0x0013A508 File Offset: 0x00138708
		public float GetEnemyDefendValue(TIArmyState defendingArmy)
		{
			float num = defendingArmy.adjustedTechLevel;
			if (defendingArmy.homeNation.regions.Contains(this.currentRegion))
			{
				num += TemplateManager.global.armyRegionDefenseBonus;
				if (this.currentRegion.terrain == TerrainType.Rugged)
				{
					num += TemplateManager.global.ruggedTerrainDefenseBonus;
				}
				if (this.currentRegion.coreEconomicRegion)
				{
					num += TemplateManager.global.coreEconomicRegionDefenseBonus;
				}
			}
			TIControlPoint ref_controlPoint = defendingArmy.ref_controlPoint;
			if (ref_controlPoint != null && ref_controlPoint.benefitsDisabled)
			{
				num -= TemplateManager.global.armyCrackdownMalus;
			}
			if (this.currentRegion.terrain == TerrainType.Rugged)
			{
				num += TIEffectsState.SumEffectsModifiers(Context.ArmyRuggedWarfare, defendingArmy.faction, num, null);
			}
			if (this.currentRegion.coreEconomicRegion)
			{
				num += TIEffectsState.SumEffectsModifiers(Context.ArmyUrbanWarfare, defendingArmy.faction, num, null);
			}
			return num * defendingArmy.combatEffectiveness;
		}

		// Token: 0x0600362E RID: 13870 RVA: 0x0013A5E0 File Offset: 0x001387E0
		public float GetCombatSuccessChance(float attackValue, float enemyValue)
		{
			float num = 1.5f * (attackValue - enemyValue);
			float num2 = 0.5f * Mathf.Pow(0.775f, Mathf.Abs(num));
			if (num > 0f)
			{
				num2 = 1f - num2;
			}
			return num2;
		}

		// Token: 0x0600362F RID: 13871 RVA: 0x0013A620 File Offset: 0x00138820
		public void FireAtEnemyArmy(TIArmyState defendingArmy)
		{
			if (this.atSea)
			{
				return;
			}
			this.lastEnemyArmy = defendingArmy;
			float attackValue = this.GetAttackValue();
			float enemyDefendValue = this.GetEnemyDefendValue(defendingArmy);
			float combatSuccessChance = this.GetCombatSuccessChance(attackValue, enemyDefendValue);
			int num = this.currentRegion.NumArmiesPresent(true, false, true, true);
			float num2 = 1f;
			if (!this.AlienMegafaunaArmy && !defendingArmy.AlienMegafaunaArmy && this.strength == 1f)
			{
				if (this.currentRegion.occupations.Count<KeyValuePair<TINationState, float>>() != 0)
				{
					if (!this.currentRegion.occupations.All<KeyValuePair<TINationState, float>>((KeyValuePair<TINationState, float> x) => x.Value <= 0f))
					{
						goto IL_00A8;
					}
				}
				num2 = 3f;
				goto IL_00C1;
			}
			IL_00A8:
			if (TIUtilities.RandomFloatValue() < 0.01f * (float)num * (float)num)
			{
				num2 = 3f;
			}
			IL_00C1:
			defendingArmy.currentRegion.ApplyDamageToRegion((10f - attackValue) * this.regionDamageScaling * num2, this.faction, this.homeNation, false, false, false, false);
			if (TIUtilities.RandomFloatValue() < combatSuccessChance)
			{
				if (!this.AlienMegafaunaArmy && defendingArmy.AlienMegafaunaArmy && TIUtilities.RandomFloatValue() < TIEffectsState.SumEffectsModifiers(Context.MegafaunaMastery, this.faction, 0f, null))
				{
					defendingArmy.AssignToFaction(this.faction, true);
					return;
				}
				float num3 = attackValue * 0.001f * num2 * (0.8f + TIUtilities.RandomRange(0f, 0.4f));
				num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoAllArmies, this.faction, num3, null);
				switch (defendingArmy.armyType)
				{
				case ArmyType.Human:
					num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoHumanArmy, this.faction, num3, null);
					break;
				case ArmyType.AlienMegafauna:
					num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoMegafauna, this.faction, num3, null);
					break;
				case ArmyType.AlienInvader:
					num3 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoInvaderArmy, this.faction, num3, null);
					break;
				}
				defendingArmy.TakeDamage(num3, this.faction, this.homeNation, true);
			}
		}

		// Token: 0x06003630 RID: 13872 RVA: 0x0013A80C File Offset: 0x00138A0C
		public static float LocalForcesAdjacentRegionsBonus(TIRegionState currentRegion)
		{
			float num = 0f;
			IEnumerable<TIRegionState> enumerable = from x in currentRegion.AdjacentRegions(false)
				where x.nation == currentRegion.nation
				select x;
			float num2 = (float)enumerable.Count<TIRegionState>();
			if (num2 > 0f)
			{
				foreach (TIRegionState tiregionState in enumerable)
				{
					if (tiregionState.occupations.Count != 0)
					{
						if (!tiregionState.occupations.All<KeyValuePair<TINationState, float>>((KeyValuePair<TINationState, float> x) => x.Value <= 0f))
						{
							continue;
						}
					}
					if (tiregionState.NumArmiesPresent(false, false, true, false) == 0)
					{
						num += 1f;
					}
				}
				return num / num2 * currentRegion.nation.militaryTechLevel * TemplateManager.global.adjacentFriendlyForcesRegionMiltechMultiplier * (1f + currentRegion.nation.unrest * TemplateManager.global.defenseUnrestMultiplier);
			}
			return 0f;
		}

		// Token: 0x06003631 RID: 13873 RVA: 0x0013A930 File Offset: 0x00138B30
		public float LocalForcesBaseDefenseLevel(bool modifyForCohesionAndUnrest, TINationState occupierWereFighting)
		{
			if (this.InFriendlyRegion)
			{
				if (occupierWereFighting == null && this.currentRegion.occupations.Count > 0)
				{
					occupierWereFighting = this.currentRegion.occupations.MaxBy<KeyValuePair<TINationState, float>, float>((KeyValuePair<TINationState, float> x) => x.Value).Key;
				}
				return occupierWereFighting.militaryTechLevel + occupierWereFighting.adviserCommandBonus;
			}
			if (this.currentNation.military)
			{
				float num = this.currentNation.militaryTechLevel + this.currentRegion.nation.adviserCommandBonus + TemplateManager.global.baseRegionDefenseBonus;
				if (this.currentRegion.terrain == TerrainType.Rugged)
				{
					num += TemplateManager.global.ruggedTerrainDefenseBonus;
					num += TIEffectsState.SumEffectsModifiers(Context.ArmyRuggedWarfare, this.currentNation.executiveFaction, num, null);
				}
				if (this.currentRegion.coreEconomicRegion)
				{
					num += TemplateManager.global.coreEconomicRegionDefenseBonus;
					num += TIEffectsState.SumEffectsModifiers(Context.ArmyUrbanWarfare, this.currentNation.executiveFaction, num, null);
				}
				if (modifyForCohesionAndUnrest)
				{
					num += this.currentNation.cohesion * TemplateManager.global.defenseCohesionMultiplier;
					num += this.currentNation.unrest * TemplateManager.global.defenseUnrestMultiplier;
				}
				num += TIArmyState.LocalForcesAdjacentRegionsBonus(this.currentRegion);
				return num * (this.currentRegion.occupations.ContainsKey(this.homeNation) ? (1f - this.currentRegion.occupations[this.homeNation] * TemplateManager.global.localDefensesDamageEffectivenessFactor) : 1f);
			}
			return 0f;
		}

		// Token: 0x170008D7 RID: 2263
		// (get) Token: 0x06003632 RID: 13874 RVA: 0x0013AAD6 File Offset: 0x00138CD6
		private float regionDamageScaling
		{
			get
			{
				if (this.armyType != ArmyType.AlienMegafauna)
				{
					return 2E-05f;
				}
				return 0.002f;
			}
		}

		// Token: 0x06003633 RID: 13875 RVA: 0x0013AAEC File Offset: 0x00138CEC
		public virtual void EngageLocalForcesAndOccupy(bool regionReturnFireOnly = false)
		{
			if (this.atSea)
			{
				return;
			}
			List<TIArmyState> list = (from x in this.currentRegion.FilteredArmiesPresent(false, false, true, false, false)
				where x.CurrentOperations().Count == 0
				select x).ToList<TIArmyState>();
			int count = list.Count;
			List<TIArmyState> list2 = this.currentRegion.FilteredArmiesPresent(true, false, false, false, true).Where<TIArmyState>(delegate(TIArmyState x)
			{
				TINationState homeNation = x.homeNation;
				return homeNation != null && homeNation.wars.Contains(this.homeNation) && x.CurrentOperations().Count == 0;
			}).ToList<TIArmyState>();
			int count2 = list2.Count;
			float num = list.Sum<TIArmyState>((TIArmyState x) => x.combatEffectiveness);
			float num2 = list2.Sum<TIArmyState>((TIArmyState x) => x.combatEffectiveness);
			bool inFriendlyRegion = this.InFriendlyRegion;
			bool flag = !inFriendlyRegion;
			List<TINationState> occupyingAlliance;
			TINationState tinationState;
			float highestWarAllianceOccupationValue = this.currentRegion.GetHighestWarAllianceOccupationValue(out tinationState, out occupyingAlliance);
			TINationState tinationState2 = ((inFriendlyRegion && highestWarAllianceOccupationValue > 0f) ? this.currentRegion.occupations.Where<KeyValuePair<TINationState, float>>((KeyValuePair<TINationState, float> x) => occupyingAlliance.Contains(x.Key)).SelectRandomWeightedItem<KeyValuePair<TINationState, float>>((KeyValuePair<TINationState, float> x) => this.currentRegion.occupations[x.Key], -1f, 1E-37f).Key : null);
			float num3;
			if (inFriendlyRegion)
			{
				num3 = 0.01f * (float)count2 * (float)count2 * (float)Mathf.Max(count, 1);
			}
			else
			{
				num3 = 0.01f * (float)count * (float)count * (float)Mathf.Max(count2, 1);
			}
			float num4 = (((this.strength == 1f && (!this.currentRegion.occupations.ContainsKey(this.homeNation) || this.currentRegion.occupations[this.homeNation] <= 0f)) || TIUtilities.RandomFloatValue() < num3) ? 40f : 1f);
			if (this.strength == 1f)
			{
				if (this.currentRegion.occupations.Count<KeyValuePair<TINationState, float>>() != 0)
				{
					if (!this.currentRegion.occupations.All<KeyValuePair<TINationState, float>>((KeyValuePair<TINationState, float> x) => x.Value <= 0f))
					{
						goto IL_022B;
					}
				}
				num4 = 100f;
				goto IL_0254;
			}
			IL_022B:
			float num5 = TIUtilities.RandomFloatValue();
			if (num5 < num3)
			{
				num4 = 40f;
			}
			else if (num5 < num3 * 2f)
			{
				num4 = 5f;
			}
			IL_0254:
			float attackValue = this.GetAttackValue();
			float num6 = this.LocalForcesBaseDefenseLevel(true, tinationState2);
			float combatSuccessChance = this.GetCombatSuccessChance(num6, attackValue);
			this.currentRegion.ApplyDamageToRegion(Mathf.Max(1f, 12f - attackValue) * this.regionDamageScaling * num4, this.faction, this.homeNation, false, false, false, false);
			float num7 = this.currentRegion.RegionArmyActionMultiplier(true);
			if (TIUtilities.RandomFloatValue() >= combatSuccessChance)
			{
				if (!regionReturnFireOnly)
				{
					if (flag)
					{
						if (count2 == 0)
						{
							float num8 = 0.8f + TIUtilities.RandomRange(0f, 0.4f);
							float num9 = Mathf.Max(0f, attackValue - this.currentNation.militaryTechLevel - this.currentNation.adviserCommandBonus);
							float num10 = (attackValue + num9) * 0.000225f * TemplateManager.global.occupationSpeed * num7 * num4 * num8;
							float num11 = (float)count - this.currentRegion.mapRegionTemplate.area_km2 / 100000f;
							if (num11 > 0f)
							{
								num10 *= Mathf.Max(0.5f, 1f - num11 * 0.02f);
							}
							num10 = Mathf.Min(0.1f, num10);
							this.currentRegion.IncreaseOccupationValue(this.homeNation, num10, this);
						}
					}
					else if (count == 0)
					{
						if (tinationState2 != null)
						{
							float num12 = 0.8f + TIUtilities.RandomRange(0f, 0.4f);
							float[] array = new float[3];
							array[1] = attackValue - tinationState2.militaryTechLevel - tinationState2.adviserCommandBonus;
							float num13 = Mathf.Max(array);
							float num14 = (attackValue + num13) * 0.000225f * TemplateManager.global.occupationSpeed * num7 * num4 * ((count == 0) ? (1f + num12) : num12);
							num14 = Mathf.Min(0.1f, num14);
							this.currentRegion.IncreaseOccupationValue(tinationState2, -num14, this);
						}
						else
						{
							this.currentRegion.ValidateAndCleanOccupations();
						}
					}
					if (this.armyType == ArmyType.AlienInvader && num4 > 1f)
					{
						this.currentRegion.ConductAbductions(this.faction, 1);
					}
				}
				return;
			}
			float num15 = num6 * 0.000225f * num4 * (0.8f + TIUtilities.RandomRange(0f, 0.4f));
			num15 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoAllArmies, this.currentRegion.nation.executiveFaction, num15, null);
			if (flag)
			{
				ArmyType armyType = this.armyType;
				if (armyType != ArmyType.Human)
				{
					if (armyType == ArmyType.AlienInvader)
					{
						num15 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoInvaderArmy, this.currentNation.executiveFaction, num15, null);
					}
				}
				else
				{
					num15 += TIEffectsState.SumEffectsModifiers(Context.ArmyDamageBonustoHumanArmy, this.currentNation.executiveFaction, num15, null);
				}
			}
			float num16 = (inFriendlyRegion ? (num2 - num) : (num - num2));
			if (num16 > 0f)
			{
				num15 *= 1f - num16 / (num16 + 2f);
			}
			if (!flag)
			{
				this.TakeDamage((2f - this.currentNation.cohesion / 10f + this.currentNation.unrest / 10f) * num15, (tinationState2 != null) ? tinationState2.ref_faction : null, tinationState2, true);
				return;
			}
			float num17;
			if (!this.currentRegion.occupations.ContainsKey(this.homeNation) || this.currentRegion.occupations[this.homeNation] <= 0f)
			{
				num17 = 0f;
			}
			else if (count == 0)
			{
				num17 = 1f;
			}
			else
			{
				float num18 = this.currentRegion.occupations[this.homeNation];
				float num19 = num;
				num17 = num18 / num19;
			}
			if (TIUtilities.RandomFloatValue() < num17)
			{
				float num20 = 0.8f + TIUtilities.RandomRange(0f, 0.4f);
				float num21 = Mathf.Clamp(num6 * 0.000225f * TemplateManager.global.occupationSpeed * num4 * num7 * num20 / (float)Mathf.Max(1, count), 0f, 0.1f);
				this.currentRegion.IncreaseOccupationValue(this.homeNation, -num21, null);
				this.TakeDamage(num15, this.currentRegion.ref_faction, this.currentRegion.nation, true);
				return;
			}
			this.TakeDamage(2f * num15, this.currentRegion.ref_faction, this.currentRegion.nation, true);
		}

		// Token: 0x06003634 RID: 13876 RVA: 0x0013B18C File Offset: 0x0013938C
		public void CheckAndPromptIfInIllegalRegion(bool autoTeleport, bool justMadePeace)
		{
			if (!this.InLegalRegion && !this.AlienMegafaunaArmy)
			{
				if (autoTeleport && !this.AlienRegularArmy)
				{
					this.TeleportArmyFromIllegalRegion();
					return;
				}
				if (!TIPromptQueueState.HasPromptStatic(this.homeNation, this.currentNation, this.currentNation, "PromptArmyOrderedToDepart", 0) && !TIPromptQueueState.HasPromptStatic(this.homeNation, this.currentNation, null, "PromptArmyOrderedToDepart", 0))
				{
					TIPromptQueueState.AddPromptStatic(new Prompt(this.homeNation, this.currentNation, justMadePeace ? this.currentNation : null, "PromptArmyOrderedToDepart", 0));
				}
			}
		}

		// Token: 0x06003635 RID: 13877 RVA: 0x0013B220 File Offset: 0x00139420
		public bool TeleportArmyFromIllegalRegion()
		{
			TIRegionState currentRegion = this.currentRegion;
			List<TIRegionState> list = new List<TIRegionState>();
			List<TIRegionState> list2 = new List<TIRegionState>();
			foreach (TIRegionState tiregionState in GameStateManager.IterateByClass<TIRegionState>(false))
			{
				if (this.LegalRegion(tiregionState))
				{
					if (this.FriendlyRegionIncludingFullyOccupied(tiregionState) && !tiregionState.IsFullyOccupied())
					{
						list.Add(tiregionState);
					}
					else
					{
						list2.Add(tiregionState);
					}
				}
			}
			Func<List<TIRegionState>, TIRegionState> func = delegate(List<TIRegionState> regions)
			{
				if (regions.Count == 0)
				{
					return null;
				}
				Dictionary<TIRegionState, float> dictionary = regions.Intersect<TIRegionState>(this.currentRegion.ConnectedRegions).ToDictionary<TIRegionState, TIRegionState, float>((TIRegionState x) => x, delegate(TIRegionState region)
				{
					float num = this.GetDeploymentToAdjacentRegionDuration_Days(region);
					if (num < 0f)
					{
						num = float.PositiveInfinity;
					}
					return num;
				});
				TIRegionState tiregionState2 = dictionary.MinBy<KeyValuePair<TIRegionState, float>, float>((KeyValuePair<TIRegionState, float> x) => x.Value).Key;
				if (tiregionState2 == null || dictionary[tiregionState2] == float.PositiveInfinity)
				{
					tiregionState2 = regions.MinBy<TIRegionState, float>(delegate(TIRegionState x)
					{
						float num2 = Mathf.Abs(x.longitude - this.currentRegion.longitude);
						if (num2 > 180f)
						{
							num2 = 360f - num2;
						}
						float num3 = Mathf.Abs(x.latitude - this.currentRegion.latitude);
						return Mathf.Pow(num2, 2f) + Mathf.Pow(num3, 2f);
					});
				}
				return tiregionState2;
			};
			bool flag = false;
			if (list.Count > 0)
			{
				this.MoveArmyToRegion(func(list), false);
				flag = true;
			}
			else if (list2.Count > 0)
			{
				this.MoveArmyToRegion(func(list2), false);
				flag = true;
			}
			if (!flag)
			{
				this.MoveArmyToRegion(this.homeRegion, false);
			}
			TINotificationQueueState.LogArmyTeleportedToLegalRegion(this, currentRegion, this.currentRegion);
			return flag;
		}

		// Token: 0x06003636 RID: 13878 RVA: 0x0013B314 File Offset: 0x00139514
		public List<IOperation> VisibleOperationList(TINaturalSpaceObjectState naturalSpaceObject = null)
		{
			return OperationsManager.armyOperations.Where<IOperation>((IOperation x) => x.OpVisibleToActor(this, null)).ToList<IOperation>();
		}

		// Token: 0x06003637 RID: 13879 RVA: 0x0013B334 File Offset: 0x00139534
		public List<IOperation> AvailableOperationList(TINaturalSpaceObjectState naturalSpaceObject = null)
		{
			if (this.IsMoving)
			{
				return OperationsManager.LegalArmyOperationsWhileMoving.ToList<IOperation>();
			}
			if (this.currentOperations.Any<OperationData>((OperationData x) => x.operation.IsBlockingOperation()))
			{
				return OperationsManager.CancelArmyOperation.ToList<IOperation>();
			}
			return OperationsManager.armyOperations.Where<IOperation>((IOperation x) => x.ActorCanPerformOperation(this, null)).ToList<IOperation>();
		}

		// Token: 0x06003638 RID: 13880 RVA: 0x0013B3A8 File Offset: 0x001395A8
		public List<IOperation> SubstantiveAvailableOperationList()
		{
			if (this.IsMoving)
			{
				return OperationsManager.LegalArmyOperationsWhileMoving.ToList<IOperation>();
			}
			if (this.currentOperations.Any<OperationData>((OperationData x) => x.operation.IsBlockingOperation()))
			{
				return OperationsManager.CancelArmyOperation.ToList<IOperation>();
			}
			return OperationsManager.AIArmyOperations.Where<IOperation>((IOperation x) => x.ActorCanPerformOperation(this, null)).ToList<IOperation>();
		}

		// Token: 0x06003639 RID: 13881 RVA: 0x0013B41C File Offset: 0x0013961C
		private void UpdateIsMoving()
		{
			this._isMoving = this.currentOperations.Count > 0 && this.currentOperations[0].operation is DeployArmyOperation && this.currentRegion != this.currentOperations[0].target;
		}

		// Token: 0x0600363A RID: 13882 RVA: 0x0013B474 File Offset: 0x00139674
		public void RemoveOperation(OperationData data)
		{
			if (!this.currentOperations.Remove(data))
			{
				Log.Warn("Failed to remove operation: " + data.operationDataName, Array.Empty<object>());
			}
			this.UpdateIsMoving();
		}

		// Token: 0x0600363B RID: 13883 RVA: 0x0013B4A4 File Offset: 0x001396A4
		public void ClearOperations()
		{
			if (this.SeaTransitStage() != ArmySeaTransitStage.None)
			{
				this.CancelSeaTransit();
			}
			foreach (OperationData operationData in new List<OperationData>(this.currentOperations))
			{
				operationData.OnOperationCancel(this);
				this.RemoveOperation(operationData);
			}
			this.SetNotMoving();
		}

		// Token: 0x0600363C RID: 13884 RVA: 0x0013B518 File Offset: 0x00139718
		public List<OperationData> CurrentOperations()
		{
			return this.currentOperations;
		}

		// Token: 0x0600363D RID: 13885 RVA: 0x0013B520 File Offset: 0x00139720
		public TIMissionOutcome AssaultAlienAsset(TIRegionAlienAssetState alienAsset, TIMissionOutcome baseOutcome)
		{
			float num = 0f;
			float num2 = 0.5f;
			switch (baseOutcome)
			{
			case TIMissionOutcome.CriticalFailure:
				num = 0.5f;
				num2 = 1f;
				break;
			case TIMissionOutcome.Failure:
				num = 0.25f;
				num2 = 0.75f;
				break;
			case TIMissionOutcome.Success:
				num = 0f;
				num2 = 0.5f;
				break;
			case TIMissionOutcome.CriticalSuccess:
				num = 0f;
				num2 = 0.1f;
				break;
			}
			if (alienAsset.isRegionXenoformingState)
			{
				num /= 5f;
				num2 /= 2f;
			}
			this.TakeDamage(TIUtilities.RandomRange(num, num2) / (this.techLevel / 3f), GameStateManager.AlienFaction(), null, true);
			TIMissionOutcome timissionOutcome = (this.destroyed ? TIMissionOutcome.Failure : baseOutcome);
			alienAsset.ResolveAssault(this, this.faction, timissionOutcome);
			return timissionOutcome;
		}

		// Token: 0x0600363E RID: 13886 RVA: 0x0013B5E4 File Offset: 0x001397E4
		public void OnTimedOperationComplete(TimeEventStart e)
		{
			if (this.destroyed)
			{
				Log.Debug("OnTimedOperationComplete called but army was already destroyed", Array.Empty<object>());
				return;
			}
			if (e.eventObject == this)
			{
				IOperation operation = e.eventDataTemplate as IOperation;
				TIGameState eventObject = e.eventObject2;
				OperationData operationData = null;
				foreach (OperationData operationData2 in this.currentOperations)
				{
					if (operationData2.operation == operation && operationData2.target == eventObject)
					{
						operationData = operationData2;
					}
				}
				if (operationData != null)
				{
					this.RemoveOperation(operationData);
					operation.OnOperationExecute(this, e.eventObject2);
					GameControl.eventManager.TriggerEvent(new TimeEventComplete(this, null), this.armyOperationCompleteEventName, Array.Empty<object>());
				}
			}
		}

		// Token: 0x0600363F RID: 13887 RVA: 0x0013B6C0 File Offset: 0x001398C0
		public string OperationDescription()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.currentOperations.Count > 0)
			{
				OperationData operationData = this.currentOperations[0];
				IOperation operation = operationData.operation;
				TIGameState target = operationData.target;
				TIDateTime completionDate = operationData.completionDate;
				if (operation.GetTemplate() is DeployArmyOperation)
				{
					stringBuilder.Append(Loc.T("UI.Army.DeployDescription", new object[]
					{
						target.displayName,
						completionDate.ToCustomDateString()
					}));
				}
				else
				{
					stringBuilder.Append(operation.GetDisplayName());
					if (target != null)
					{
						stringBuilder.Append(Loc.T("UI.Army.OperationTarget", new object[] { target.displayName }));
					}
					if (completionDate != null)
					{
						stringBuilder.Append(Loc.T("UI.Army.CompletionDate", new object[] { completionDate.ToCustomDateString() }));
					}
				}
			}
			else if (this.InBattleWithArmies())
			{
				stringBuilder.Append(Loc.T("UI.Army.InBattle"));
			}
			else if (this.OccupyingRegion(true))
			{
				stringBuilder.Append(Loc.T("UI.Army.Occupying", new object[] { this.OccupationValue().ToPercent("P0") }));
			}
			else if (this.huntingXenofauna)
			{
				stringBuilder.Append(Loc.T("UI.Army.HuntingXenos"));
			}
			else
			{
				stringBuilder.Append(Loc.T("UI.Army.Idle"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003640 RID: 13888 RVA: 0x0013B825 File Offset: 0x00139A25
		public void SetHuntingXenofauna(bool setting)
		{
			bool huntingXenofauna = this.huntingXenofauna;
			this.huntingXenofauna = setting;
			if (huntingXenofauna != this.huntingXenofauna)
			{
				this.SetArmyDataDirty();
			}
		}

		// Token: 0x04002412 RID: 9234
		public TIFactionState faction;

		// Token: 0x04002414 RID: 9236
		public TIRegionState homeRegion;

		// Token: 0x04002415 RID: 9237
		public TIRegionState priorRegion;

		// Token: 0x04002416 RID: 9238
		public DeploymentType deploymentType;

		// Token: 0x04002417 RID: 9239
		public float strength;

		// Token: 0x04002418 RID: 9240
		public int controlPointIdx;

		// Token: 0x04002419 RID: 9241
		public bool createdFromTemplate;

		// Token: 0x0400241A RID: 9242
		public List<OperationData> currentOperations;

		// Token: 0x0400241B RID: 9243
		public TIGameState operationTarget;

		// Token: 0x0400241E RID: 9246
		public bool destroyed;

		// Token: 0x04002420 RID: 9248
		public ArmyType armyType;

		// Token: 0x04002421 RID: 9249
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x04002422 RID: 9250
		public string displayNameWithArticle;

		// Token: 0x04002426 RID: 9254
		public TIRegionState AI_targetEnemyRegion;

		// Token: 0x04002427 RID: 9255
		public const float baselineNavalFleetSpeed_kph = 33f;

		// Token: 0x04002428 RID: 9256
		public const float armyMovement_km_day = 300f;

		// Token: 0x04002429 RID: 9257
		public const float smallMovementPower = 0.2f;

		// Token: 0x0400242A RID: 9258
		public const float evenSplitMovementPower = 0.5f;

		// Token: 0x0400242B RID: 9259
		public const float largeMovementPower = 0.8f;

		// Token: 0x0400242C RID: 9260
		public const float enemyRegionBaseMultiplier = 5f;

		// Token: 0x0400242D RID: 9261
		public const float enemyRegionRuggedMultiplier = 1.5f;

		// Token: 0x0400242E RID: 9262
		public const float friendlyRegionRuggedMultipler = 1.25f;

		// Token: 0x0400242F RID: 9263
		public const float colonyRegionMultiplier = 1.5f;

		// Token: 0x04002430 RID: 9264
		public const float hostileToHostileMultiplier = 4f;

		// Token: 0x04002431 RID: 9265
		public static readonly float normal_enemyRegion = Mathf.Pow(5f, 0.5f);

		// Token: 0x04002432 RID: 9266
		public static readonly float small_enemyRegion = Mathf.Pow(5f, 0.2f);

		// Token: 0x04002433 RID: 9267
		public static readonly float large_enemyRegion = Mathf.Pow(5f, 0.8f);

		// Token: 0x04002434 RID: 9268
		public static readonly float normal_enemyRegion_rugged = Mathf.Pow(1.5f, 0.5f);

		// Token: 0x04002435 RID: 9269
		public static readonly float small_enemyRegion_rugged = Mathf.Pow(1.5f, 0.2f);

		// Token: 0x04002436 RID: 9270
		public static readonly float large_enemyRegion_rugged = Mathf.Pow(1.5f, 0.8f);

		// Token: 0x04002437 RID: 9271
		public static readonly float normal_friendlyRegion_rugged = Mathf.Pow(1.25f, 0.5f);

		// Token: 0x04002438 RID: 9272
		public static readonly float small_friendlyRegion_rugged = Mathf.Pow(1.25f, 0.2f);

		// Token: 0x04002439 RID: 9273
		public static readonly float large_friendlyRegion_rugged = Mathf.Pow(1.25f, 0.8f);

		// Token: 0x0400243A RID: 9274
		public static readonly float normal_colonyRegion = Mathf.Pow(1.5f, 0.5f);

		// Token: 0x0400243B RID: 9275
		public static readonly float small_colonyRegion = Mathf.Pow(1.5f, 0.2f);

		// Token: 0x0400243C RID: 9276
		public static readonly float large_colonyRegion = Mathf.Pow(1.5f, 0.8f);

		// Token: 0x0400243D RID: 9277
		private bool _isMoving;

		// Token: 0x0400243E RID: 9278
		private bool _isFighting;

		// Token: 0x0400243F RID: 9279
		private int _lastIsFightingFrame = -1;

		// Token: 0x04002440 RID: 9280
		private HashSet<TIRegionState> cachedReachableRegions = new HashSet<TIRegionState>();

		// Token: 0x04002441 RID: 9281
		private int reachableRegionsCachedFrame = -1;

		// Token: 0x04002442 RID: 9282
		private HashSet<TIRegionState> cachedReachableRegions_Fast = new HashSet<TIRegionState>();

		// Token: 0x04002443 RID: 9283
		private TIDateTime reachableRegionsCachedDate_Fast;

		// Token: 0x04002444 RID: 9284
		private HashSet<TIRegionState> cachedEnterableRegions = new HashSet<TIRegionState>();

		// Token: 0x04002445 RID: 9285
		private int enterableRegionsCachedFrame = -1;

		// Token: 0x04002446 RID: 9286
		[TupleElementNames(new string[] { "ConnectedRegions", "CachedFrame" })]
		private Dictionary<TIRegionState, ValueTuple<HashSet<TIRegionState>, int>> cachedConnectedRegions = new Dictionary<TIRegionState, ValueTuple<HashSet<TIRegionState>, int>>();

		// Token: 0x04002447 RID: 9287
		public List<TIRegionState> destinationQueue = new List<TIRegionState>();

		// Token: 0x04002448 RID: 9288
		private static Dictionary<string, Dictionary<string, float>> journeyHeuristic = null;

		// Token: 0x04002449 RID: 9289
		private static List<Thread> journeyHeuristicThreads;

		// Token: 0x0400244A RID: 9290
		private TIRegionState _finalDestination;

		// Token: 0x0400244B RID: 9291
		private int _finalDestinationFrame;

		// Token: 0x0400244C RID: 9292
		private Dictionary<TIRegionState, IEnumerable<TIArmyState>> _enemyArmiesInRegion = new Dictionary<TIRegionState, IEnumerable<TIArmyState>>();

		// Token: 0x0400244D RID: 9293
		private int _enemyArmiesCacheFrame;

		// Token: 0x0400244E RID: 9294
		private static readonly Color genericBackgroundColor = new Color(0.69803923f, 0.69803923f, 0.69803923f, 1f);

		// Token: 0x0400244F RID: 9295
		public const float combatScaling = 0.775f;

		// Token: 0x04002450 RID: 9296
		public const float failureChanceAtBalance = 0.5f;

		// Token: 0x04002451 RID: 9297
		private TIArmyState lastEnemyArmy;

		// Token: 0x04002452 RID: 9298
		private const float battleScaling = 0.001f;

		// Token: 0x04002453 RID: 9299
		private const float occupationScaling = 0.000225f;

		// Token: 0x04002454 RID: 9300
		private const float majorBattleChanceBase = 0.01f;

		// Token: 0x04002455 RID: 9301
		private const float majorArmyEngagementModifer = 3f;

		// Token: 0x04002456 RID: 9302
		private const float minorBattleModifier = 5f;

		// Token: 0x04002457 RID: 9303
		private const float majorBattleModifier = 40f;

		// Token: 0x04002458 RID: 9304
		private const float decisiveBattleModifier = 100f;

		// Token: 0x04002459 RID: 9305
		public const float maxOccupationChangePerArmyPerDay = 0.1f;

		// Token: 0x0400245A RID: 9306
		private const int LocalDefensesDamageReductionFactor = 2;
	}
}
