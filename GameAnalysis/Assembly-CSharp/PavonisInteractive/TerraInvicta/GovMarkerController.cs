using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000553 RID: 1363
	public class GovMarkerController : SingleMarkerController
	{
		// Token: 0x170004F8 RID: 1272
		// (get) Token: 0x06002374 RID: 9076 RVA: 0x000BAF60 File Offset: 0x000B9160
		private bool capitalRegion
		{
			get
			{
				return base.nation.capital == base.region;
			}
		}

		// Token: 0x06002375 RID: 9077 RVA: 0x000BAF78 File Offset: 0x000B9178
		public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
		{
			base.InitializeWithRegion(regionController, container);
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.ResetAnimations), null, null, true, false);
			GameControl.eventManager.AddListener<TargetControlPoints>(new EventManager.EventDelegate<TargetControlPoints>(this.ActivateControlPointButtons), null, null, false, false);
			GameControl.eventManager.AddListener<DeTargetControlPoints>(new EventManager.EventDelegate<DeTargetControlPoints>(this.DeactivateControlPointButtons), null, null, false, false);
			GameControl.eventManager.AddListener<TargetGov>(new EventManager.EventDelegate<TargetGov>(this.ActivateGovToHitValues), null, null, false, false);
			GameControl.eventManager.AddListener<DeTargetGov>(new EventManager.EventDelegate<DeTargetGov>(this.DeactivateGovToHitValues), null, null, false, false);
			GameControl.eventManager.AddListener<TargetRegions>(new EventManager.EventDelegate<TargetRegions>(this.ActivateRegionTargeting), null, null, false, false);
			GameControl.eventManager.AddListener<DeTargetRegions>(new EventManager.EventDelegate<DeTargetRegions>(this.DeactivateRegionTargeting), null, null, false, false);
			GameControl.eventManager.AddListener<MissionTargettedEvent>(new EventManager.EventDelegate<MissionTargettedEvent>(this.OnNewTargetSelected), null, base.region, true, false);
			this.UpdateMarker();
		}

		// Token: 0x06002376 RID: 9078 RVA: 0x000BB068 File Offset: 0x000B9268
		private void UpdateCapitalMarker(RegionDataUpdated e)
		{
			this.TryUpdateCapitalMarker();
		}

		// Token: 0x06002377 RID: 9079 RVA: 0x000BB070 File Offset: 0x000B9270
		private void UpdateCapitalMarker(NationControlPointOwnerChanged e)
		{
			this.TryUpdateCapitalMarker();
		}

		// Token: 0x06002378 RID: 9080 RVA: 0x000BB078 File Offset: 0x000B9278
		private void UpdateCapitalMarker(ControlPointDataUpdated e)
		{
			this.TryUpdateCapitalMarker();
		}

		// Token: 0x06002379 RID: 9081 RVA: 0x000BB080 File Offset: 0x000B9280
		private void TryUpdateCapitalMarker()
		{
			if (base.gameObject.activeSelf)
			{
				this.capitalMarkerDataDirty = true;
				return;
			}
			this.UpdateCapitalMarker();
			this.capitalMarkerDataDirty = false;
		}

		// Token: 0x0600237A RID: 9082 RVA: 0x000BB0A4 File Offset: 0x000B92A4
		private void UpdateMarker(MajorRegionStatusChange e)
		{
			this.UpdateMarkerNext();
		}

		// Token: 0x0600237B RID: 9083 RVA: 0x000BB0AC File Offset: 0x000B92AC
		private void UpdateMarker(OccupationStatusChange e)
		{
			this.UpdateMarkerNext();
		}

		// Token: 0x0600237C RID: 9084 RVA: 0x000BB0B4 File Offset: 0x000B92B4
		private void ResetAnimations(MapActivationChangedEvent e)
		{
			if (e.active)
			{
				this.TurnOffAllTargeting();
				this.UpdateMarker();
				if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TINationState)))
				{
					if (base.globalCurrentTarget == base.nation && base.region.nation.capital == base.region)
					{
						this.animationsDataDirty = true;
					}
				}
				else if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIRegionState)))
				{
					if (base.globalCurrentTarget == base.region)
					{
						this.animationsDataDirty = true;
					}
				}
				else if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIControlPoint)))
				{
					TIGameState globalCurrentTarget = base.globalCurrentTarget;
					if (((globalCurrentTarget != null) ? globalCurrentTarget.ref_nation : null) == base.nation && base.region.nation.capital == base.region)
					{
						this.animationsDataDirty = true;
					}
				}
				GameControl.eventManager.AddListener<ControlPointDataUpdated>(new EventManager.EventDelegate<ControlPointDataUpdated>(this.UpdateCapitalMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<RegionDataUpdated>(new EventManager.EventDelegate<RegionDataUpdated>(this.UpdateCapitalMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<NationControlPointOwnerChanged>(new EventManager.EventDelegate<NationControlPointOwnerChanged>(this.UpdateCapitalMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<RegionOccupationValueChange>(new EventManager.EventDelegate<RegionOccupationValueChange>(this.OnOccupationUnderway), null, base.region, true, false);
				GameControl.eventManager.AddListener<OccupationStatusChange>(new EventManager.EventDelegate<OccupationStatusChange>(this.UpdateMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<MajorRegionStatusChange>(new EventManager.EventDelegate<MajorRegionStatusChange>(this.UpdateMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<RegionDamaged>(new EventManager.EventDelegate<RegionDamaged>(this.OnRegionDamaged), null, base.region, true, false);
				GameControl.eventManager.AddListener<NuclearLaunch>(new EventManager.EventDelegate<NuclearLaunch>(this.OnNuclearLaunch), null, base.region, true, false);
				GameControl.eventManager.AddListener<NuclearStrike>(new EventManager.EventDelegate<NuclearStrike>(this.OnNuclearStrike), null, base.region, true, false);
				return;
			}
			this.TurnOffAllTargeting();
			GameControl.eventManager.RemoveListener<ControlPointDataUpdated>(new EventManager.EventDelegate<ControlPointDataUpdated>(this.UpdateCapitalMarker), null);
			GameControl.eventManager.RemoveListener<RegionDataUpdated>(new EventManager.EventDelegate<RegionDataUpdated>(this.UpdateCapitalMarker), null);
			GameControl.eventManager.RemoveListener<NationControlPointOwnerChanged>(new EventManager.EventDelegate<NationControlPointOwnerChanged>(this.UpdateCapitalMarker), null);
			GameControl.eventManager.RemoveListener<RegionOccupationValueChange>(new EventManager.EventDelegate<RegionOccupationValueChange>(this.OnOccupationUnderway), null);
			GameControl.eventManager.RemoveListener<OccupationStatusChange>(new EventManager.EventDelegate<OccupationStatusChange>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<MajorRegionStatusChange>(new EventManager.EventDelegate<MajorRegionStatusChange>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<RegionDamaged>(new EventManager.EventDelegate<RegionDamaged>(this.OnRegionDamaged), null);
			GameControl.eventManager.RemoveListener<NuclearLaunch>(new EventManager.EventDelegate<NuclearLaunch>(this.OnNuclearLaunch), null);
			GameControl.eventManager.RemoveListener<NuclearStrike>(new EventManager.EventDelegate<NuclearStrike>(this.OnNuclearStrike), null);
		}

		// Token: 0x0600237D RID: 9085 RVA: 0x000BB39C File Offset: 0x000B959C
		private void TurnOffAllTargetingAnimations()
		{
			if (this.capitalStatusMarker != null)
			{
				this.capitalStatusMarker.StopSelectionAnimation();
				foreach (TIControlPoint ticontrolPoint in base.nation.controlPoints)
				{
					int positionInNation = ticontrolPoint.positionInNation;
					this.capitalStatusMarker.StopCPTargetingAnimation(positionInNation);
				}
			}
			if (this.regionStatusMarker != null)
			{
				this.regionStatusMarker.StopSelectionAnimation();
			}
			if (this.occupationMarker != null)
			{
				this.occupationMarker.StopSelectionAnimation();
			}
		}

		// Token: 0x0600237E RID: 9086 RVA: 0x000BB44C File Offset: 0x000B964C
		private void TurnOffAllTargeting()
		{
			if (this.capitalStatusMarker != null)
			{
				this.capitalStatusMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				this.capitalStatusMarker.StopSelectionAnimation();
				foreach (TIControlPoint ticontrolPoint in base.nation.controlPoints)
				{
					int positionInNation = ticontrolPoint.positionInNation;
					this.capitalStatusMarker.StopCPTargetingAnimation(positionInNation);
					this.capitalStatusMarker.SetCPToHitNumber(ticontrolPoint.positionInNation, null, ClearFlag.TurnOff);
				}
			}
			if (this.regionStatusMarker != null)
			{
				this.regionStatusMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				this.regionStatusMarker.StopSelectionAnimation();
			}
			if (this.occupationMarker != null)
			{
				this.occupationMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				this.occupationMarker.StopSelectionAnimation();
			}
		}

		// Token: 0x0600237F RID: 9087 RVA: 0x000BB548 File Offset: 0x000B9748
		private void UpdateMarkerNext()
		{
			if (base.gameObject.activeSelf)
			{
				this.regionDataDirty = true;
				return;
			}
			this.UpdateMarker();
			this.regionDataDirty = false;
		}

		// Token: 0x06002380 RID: 9088 RVA: 0x000BB56C File Offset: 0x000B976C
		public void Update()
		{
			if (this.regionDataDirty)
			{
				this.UpdateMarker();
				this.regionDataDirty = false;
				this.capitalMarkerDataDirty = false;
			}
			else if (this.capitalMarkerDataDirty)
			{
				this.UpdateCapitalMarker();
				this.capitalMarkerDataDirty = false;
			}
			if (this.animationsDataDirty)
			{
				if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIControlPoint)))
				{
					using (List<TIControlPoint>.Enumerator enumerator = base.nation.controlPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIControlPoint ticontrolPoint = enumerator.Current;
							int positionInNation = ticontrolPoint.positionInNation;
							if (GeneralControlsController.UITargetedState == ticontrolPoint)
							{
								this.capitalStatusMarker.StartCPTargetingAnimation(positionInNation);
							}
							else
							{
								this.capitalStatusMarker.StopCPTargetingAnimation(positionInNation);
							}
						}
						goto IL_00BC;
					}
				}
				this.GetLiveMarkerForAnimations().StartSelectionAnimation();
				IL_00BC:
				this.animationsDataDirty = false;
			}
		}

		// Token: 0x06002381 RID: 9089 RVA: 0x000BB64C File Offset: 0x000B984C
		public void OnNewTargetSelected(MissionTargettedEvent e)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				this.TurnOffAllTargeting();
				return;
			}
			if (this.capitalStatusMarker != null && e.councilor.faction == base.activePlayer)
			{
				if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TINationState)) && e.target.isNationState)
				{
					if (base.globalCurrentTarget == base.nation)
					{
						this.capitalStatusMarker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
						this.capitalStatusMarker.StartSelectionAnimation();
						this.capitalStatusMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					}
					else
					{
						this.TurnOffAllTargetingAnimations();
						if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(base.nation))
						{
							this.capitalStatusMarker.SetToHitNumber("", e.mission.resolutionMethod.automaticSuccess, e.mission.resolutionMethod.automaticSuccess ? ClearFlag.TurnOff : ClearFlag.TurnOn, this.GetToHitPosition(this.capitalStatusMarker));
						}
					}
				}
				else if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIControlPoint)))
				{
					foreach (TIControlPoint ticontrolPoint in base.nation.controlPoints)
					{
						int positionInNation = ticontrolPoint.positionInNation;
						if (base.globalCurrentTarget == ticontrolPoint)
						{
							this.capitalStatusMarker.StartCPTargetingAnimation(positionInNation);
							this.capitalStatusMarker.SetCPToHitNumber(positionInNation, null, ClearFlag.TurnOff);
						}
						else
						{
							this.capitalStatusMarker.StopCPTargetingAnimation(positionInNation);
							if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(ticontrolPoint))
							{
								this.capitalStatusMarker.SetCPToHitNumber(ticontrolPoint.positionInNation, null, ClearFlag.TurnOn);
							}
						}
					}
				}
			}
			if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIRegionState)))
			{
				MarkerController liveMarkerForAnimations = this.GetLiveMarkerForAnimations();
				if (base.globalCurrentTarget == base.region)
				{
					liveMarkerForAnimations.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
					liveMarkerForAnimations.StartSelectionAnimation();
					liveMarkerForAnimations.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					return;
				}
				this.TurnOffAllTargetingAnimations();
				if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(base.region))
				{
					this.UpdateRegionSuccessChanceString();
					liveMarkerForAnimations.SetToHitNumber("", e.mission.resolutionMethod.automaticSuccess, e.mission.resolutionMethod.automaticSuccess ? ClearFlag.TurnOff : ClearFlag.TurnOn, this.GetToHitPosition(liveMarkerForAnimations));
				}
			}
		}

		// Token: 0x06002382 RID: 9090 RVA: 0x000BB8C8 File Offset: 0x000B9AC8
		public int GetToHitPosition(MarkerController marker)
		{
			int num = 0;
			if (marker != null)
			{
				if (marker == this.capitalStatusMarker)
				{
					int numControlPoints = base.region.nation.numControlPoints;
					int numNativeControlPoints = base.region.nation.NumNativeControlPoints;
					if (numControlPoints > 3 && numNativeControlPoints <= 2)
					{
						num = 2;
					}
					else if (numControlPoints == numNativeControlPoints)
					{
						num = 1;
					}
				}
				else if (marker == this.occupationMarker)
				{
					if (this.occupationMarker.centralIcon.isActiveAndEnabled)
					{
						num = 2;
					}
					else
					{
						num = 1;
					}
				}
				else if (marker == this.regionStatusMarker)
				{
					num = 2;
				}
			}
			return num;
		}

		// Token: 0x06002383 RID: 9091 RVA: 0x000BB95D File Offset: 0x000B9B5D
		public void ActivateControlPointButtons(TargetControlPoints e)
		{
			this.targetingCouncilor = e.councilor;
			this.missionTemplate = e.missionTemplate;
			this.UpdateCapitalMarker();
		}

		// Token: 0x06002384 RID: 9092 RVA: 0x000BB97D File Offset: 0x000B9B7D
		public void DeactivateControlPointButtons(DeTargetControlPoints e)
		{
			this.missionTemplate = null;
			this.targetingCouncilor = null;
			this.TurnOffAllTargeting();
			this.UpdateCapitalMarker();
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x000BB999 File Offset: 0x000B9B99
		public void ActivateGovToHitValues(TargetGov e)
		{
			this.targetingCouncilor = e.councilor;
			this.missionTemplate = e.missionTemplate;
			this.UpdateCapitalMarker();
		}

		// Token: 0x06002386 RID: 9094 RVA: 0x000BB9B9 File Offset: 0x000B9BB9
		public void DeactivateGovToHitValues(DeTargetGov e)
		{
			this.missionTemplate = null;
			this.targetingCouncilor = null;
			this.TurnOffAllTargeting();
			this.UpdateCapitalMarker();
		}

		// Token: 0x06002387 RID: 9095 RVA: 0x000BB9D5 File Offset: 0x000B9BD5
		public void ActivateRegionTargeting(TargetRegions e)
		{
			this.targetingCouncilor = e.councilor;
			this.missionTemplate = e.missionTemplate;
			this.TargetingAnimation();
		}

		// Token: 0x06002388 RID: 9096 RVA: 0x000BB9F5 File Offset: 0x000B9BF5
		public void DeactivateRegionTargeting(DeTargetRegions e)
		{
			this.missionTemplate = null;
			this.targetingCouncilor = null;
			this.TurnOffAllTargeting();
		}

		// Token: 0x06002389 RID: 9097 RVA: 0x000BBA0B File Offset: 0x000B9C0B
		public void OnNuclearLaunch(NuclearLaunch e)
		{
			TIUtilities.GotoGameState(e.launchingRegion, false, false, false, true, false, -1f);
			this.GetLiveMarkerForAnimations().TriggerNuclearLaunch();
		}

		// Token: 0x0600238A RID: 9098 RVA: 0x000BBA2D File Offset: 0x000B9C2D
		public void OnNuclearStrike(NuclearStrike e)
		{
			TIUtilities.GotoGameState(e.targetRegion, false, false, false, true, false, -1f);
			this.GetLiveMarkerForAnimations().TriggerNuclearStrike();
		}

		// Token: 0x0600238B RID: 9099 RVA: 0x000BBA4F File Offset: 0x000B9C4F
		public void OnOccupationUnderway(RegionOccupationValueChange e)
		{
			this.UpdateOccupationMarker(false);
		}

		// Token: 0x0600238C RID: 9100 RVA: 0x000BBA58 File Offset: 0x000B9C58
		public MarkerController GetLiveMarkerForAnimations()
		{
			if (this.designatedAnimationMarker != null && this.designatedAnimationMarker.isActiveAndEnabled)
			{
				return this.designatedAnimationMarker;
			}
			if (this.occupationMarker != null && this.occupationMarker.isActiveAndEnabled)
			{
				this.designatedAnimationMarker = this.occupationMarker;
				return this.occupationMarker;
			}
			if (this.regionStatusMarker != null && this.regionStatusMarker.isActiveAndEnabled)
			{
				this.designatedAnimationMarker = this.regionStatusMarker;
				return this.regionStatusMarker;
			}
			if (this.capitalStatusMarker != null && this.capitalStatusMarker.isActiveAndEnabled)
			{
				this.designatedAnimationMarker = this.capitalStatusMarker;
				return this.capitalStatusMarker;
			}
			this.UpdateOccupationMarker(true);
			this.designatedAnimationMarker = this.occupationMarker;
			return this.designatedAnimationMarker;
		}

		// Token: 0x0600238D RID: 9101 RVA: 0x000BBB2A File Offset: 0x000B9D2A
		public void OnRegionDamaged(RegionDamaged e)
		{
			this.GetLiveMarkerForAnimations().TriggerExplosion();
		}

		// Token: 0x0600238E RID: 9102 RVA: 0x000BBB37 File Offset: 0x000B9D37
		public override void UpdateMarker()
		{
			this.UpdateCapitalMarker();
			this.UpdateRegionStatusMarker();
			this.UpdateOccupationMarker(false);
			base.container.Refresh();
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x000BBB58 File Offset: 0x000B9D58
		public void TargetingAnimation()
		{
			MarkerController liveMarkerForAnimations = this.GetLiveMarkerForAnimations();
			if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIRegionState)) && this.missionTemplate != null)
			{
				if (base.globalCurrentTarget == base.region)
				{
					liveMarkerForAnimations.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
					liveMarkerForAnimations.StartSelectionAnimation();
					liveMarkerForAnimations.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					return;
				}
				if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(base.region))
				{
					liveMarkerForAnimations.StopSelectionAnimation();
					this.UpdateRegionSuccessChanceString();
					return;
				}
				liveMarkerForAnimations.StopSelectionAnimation();
				liveMarkerForAnimations.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				return;
			}
			else
			{
				if (!GeneralControlsController.CurrentlyTargetingStateType(typeof(TINationState)) || this.missionTemplate == null)
				{
					if (!GeneralControlsController.CurrentlyTargetingStateType(typeof(TIControlPoint)) || this.missionTemplate == null)
					{
						liveMarkerForAnimations.StopSelectionAnimation();
						liveMarkerForAnimations.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					}
					return;
				}
				if (base.globalCurrentTarget == base.nation && base.region.nation.capital == base.region)
				{
					liveMarkerForAnimations.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
					liveMarkerForAnimations.StartSelectionAnimation();
					liveMarkerForAnimations.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					return;
				}
				if (base.region.nation.capital == base.region && GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(base.nation))
				{
					liveMarkerForAnimations.StopSelectionAnimation();
					this.UpdateRegionSuccessChanceString();
					return;
				}
				liveMarkerForAnimations.StopSelectionAnimation();
				liveMarkerForAnimations.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				return;
			}
		}

		// Token: 0x06002390 RID: 9104 RVA: 0x000BBCDC File Offset: 0x000B9EDC
		public void UpdateOccupationMarker(bool forceCreation = false)
		{
			this.occupationMarker = base.container.ManageMarkerStack(this.occupationMarker, !base.region.OccupiedOrOccupationUnderway(), MarkerType.OccupationMarker, base.region, "occupationStatusMarker", 2, forceCreation);
			if (this.occupationMarker != null)
			{
				this.occupationMarker.associatedState = base.region;
				if (base.region.OccupiedOrOccupationUnderway())
				{
					this.ShowOccupationProgress(this.occupationMarker);
					return;
				}
				this.occupationMarker.centralIcon.enabled = false;
				this.occupationMarker.SetTooltip(() => base.region.displayName);
				this.occupationMarker.SetPercentage(0f, ClearFlag.TurnOff);
			}
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x000BBD90 File Offset: 0x000B9F90
		public void UpdateRegionStatusMarker()
		{
			bool flag = base.region.coreEconomicRegion || base.region.coreResourceRegion;
			this.regionStatusMarker = base.container.ManageMarkerStack(this.regionStatusMarker, !flag, MarkerType.RegionalStatusIcon, base.region, "regionStatusMarker", 1, false);
			if (flag)
			{
				this.regionStatusMarker.associatedState = base.region;
				if (base.region.coreEconomicRegion)
				{
					this.regionStatusMarker.SetCentralIcon(AssetCacheManager.coreEconomicRegionIcon);
					this.regionStatusMarker.SetTooltip(() => Loc.T("UI.Markers.CoreEconomicRegion"));
					base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dearthmodels/geoscape_core_eco");
					return;
				}
				if (base.region.oilRegion)
				{
					this.regionStatusMarker.SetCentralIcon(AssetCacheManager.coreResourceRegionOilIcon);
					base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dearthmodels/geoscape_core_resources");
					this.regionStatusMarker.SetTooltip(() => Loc.T("UI.Markers.CoreResourceRegion"));
					return;
				}
				if (base.region.resourceRegion)
				{
					this.regionStatusMarker.SetCentralIcon(AssetCacheManager.coreResourceRegionMiningIcon);
					base.container.InitializeGeoscapeModel(this.regionStatusMarker, "3dEarthmodels/geoscape_core_resources_mining");
					this.regionStatusMarker.SetTooltip(() => Loc.T("UI.Markers.CoreResourceRegion"));
				}
			}
		}

		// Token: 0x06002392 RID: 9106 RVA: 0x000BBF14 File Offset: 0x000BA114
		public void UpdateNationSuccessChanceString()
		{
			if (this.capitalStatusMarker != null)
			{
				TIMissionTemplate timissionTemplate = this.missionTemplate;
				if (timissionTemplate == null || timissionTemplate.resolutionMethod.automaticSuccess)
				{
					this.capitalStatusMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					return;
				}
				string successChanceString = this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.targetingCouncilor, base.region.nation, 0f, false, 2);
				this.capitalStatusMarker.SetToHitNumber(successChanceString, this.missionTemplate.resolutionMethod.automaticSuccess, ClearFlag.TurnOn, this.GetToHitPosition(this.designatedAnimationMarker));
			}
		}

		// Token: 0x06002393 RID: 9107 RVA: 0x000BBFB8 File Offset: 0x000BA1B8
		public void UpdateCPSuccessChanceString(TIControlPoint controlPoint)
		{
			if (this.capitalStatusMarker != null)
			{
				string successChanceString = this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.targetingCouncilor, controlPoint, 0f, false, 2);
				this.capitalStatusMarker.SetCPToHitNumber(controlPoint.positionInNation, successChanceString, ClearFlag.TurnOn);
			}
		}

		// Token: 0x06002394 RID: 9108 RVA: 0x000BC00C File Offset: 0x000BA20C
		public void UpdateRegionSuccessChanceString()
		{
			TIMissionTemplate timissionTemplate = this.missionTemplate;
			if (timissionTemplate == null || timissionTemplate.resolutionMethod.automaticSuccess)
			{
				this.GetLiveMarkerForAnimations().SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				return;
			}
			string successChanceString = this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.targetingCouncilor, base.region, 0f, false, 2);
			this.GetLiveMarkerForAnimations().SetToHitNumber(successChanceString, this.missionTemplate.resolutionMethod.automaticSuccess, ClearFlag.TurnOn, this.GetToHitPosition(this.designatedAnimationMarker));
		}

		// Token: 0x06002395 RID: 9109 RVA: 0x000BC09C File Offset: 0x000BA29C
		private string GetOccupationString(TINationState leader, List<TINationState> leadAlliance, float leadOccupationValue)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Markers.Occupation", new object[]
			{
				leader.displayName,
				leadOccupationValue.ToPercent("P0")
			}));
			List<TINationState> list = new List<TINationState>();
			foreach (TIArmyState tiarmyState in base.region.FilteredArmiesPresent(true, false, true, false, true))
			{
				if (!leadAlliance.Contains(tiarmyState.homeNation))
				{
					TINationState tinationState;
					float highestWarAllianceOccupationValueByNation = base.region.GetHighestWarAllianceOccupationValueByNation(tiarmyState.homeNation, out tinationState);
					if (tinationState != null && !list.Contains(tinationState))
					{
						list.Add(tinationState);
						stringBuilder.AppendLine().Append(Loc.T("UI.Markers.Occupation", new object[]
						{
							tinationState.displayName,
							highestWarAllianceOccupationValueByNation.ToPercent("P0")
						}));
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002396 RID: 9110 RVA: 0x000BC1A4 File Offset: 0x000BA3A4
		public void ShowOccupationProgress(MarkerController marker)
		{
			if (!base.region.OccupiedOrOccupationUnderway())
			{
				this.occupationMarker.centralIcon.enabled = false;
				marker.SetPercentage(0f, ClearFlag.TurnOff);
				return;
			}
			TINationState leader;
			List<TINationState> leadAlliance;
			float leadOccupation = base.region.GetHighestWarAllianceOccupationValue(out leader, out leadAlliance);
			if (leader != null)
			{
				marker.SetCentralIcon(leader.flag);
				marker.centralIcon.enabled = true;
				marker.SetPercentage(leadOccupation, ClearFlag.TurnOn);
				marker.SetTooltip(() => this.GetOccupationString(leader, leadAlliance, leadOccupation));
				return;
			}
			this.occupationMarker.centralIcon.enabled = false;
			marker.SetPercentage(0f, ClearFlag.TurnOff);
		}

		// Token: 0x06002397 RID: 9111 RVA: 0x000BC274 File Offset: 0x000BA474
		public static string CapitalTooltip(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Markers.Capital.TooltipHeader", new object[]
			{
				nation.displayName,
				nation.numControlPoints.ToString()
			})).AppendLine();
			for (int i = 0; i < nation.numControlPoints; i++)
			{
				TIControlPoint ticontrolPoint = nation.controlPoints[i];
				stringBuilder.AppendLine(Loc.T("UI.Markers.Capital.ControlPoint", new object[]
				{
					(nation.numControlPoints - i).ToString(),
					ticontrolPoint.description,
					(ticontrolPoint.faction == null) ? Loc.T("UI.Markers.Uncontrolled") : ticontrolPoint.faction.displayNameCapitalizedWithColor
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002398 RID: 9112 RVA: 0x000BC335 File Offset: 0x000BA535
		private void OnTotalControlIconPressed(MarkerController controller)
		{
			SoundEffectController.PlaySelectSound(base.region);
			TIUtilities.GotoGameState(base.region, true, true, true, true, false, -1f);
		}

		// Token: 0x06002399 RID: 9113 RVA: 0x000BC358 File Offset: 0x000BA558
		public void UpdateCapitalMarker()
		{
			bool capitalRegion = this.capitalRegion;
			this.capitalStatusMarker = base.container.ManageMarkerStack(this.capitalStatusMarker, !capitalRegion, MarkerType.Capital, base.region, "capitalStatusMarker", 0, false);
			if (capitalRegion)
			{
				TIFactionState totalOwningFaction = base.nation.TotalOwningFaction;
				this.capitalStatusMarker.SetTooltip(() => GovMarkerController.CapitalTooltip(base.nation));
				this.capitalStatusMarker.associatedState = base.nation;
				bool flag = GeneralControlsController.CurrentlyTargetingStateType(typeof(TIControlPoint));
				bool flag2;
				if (!(totalOwningFaction == null))
				{
					if (!base.nation.controlPoints.OnlySome<TIControlPoint>((TIControlPoint x) => x.defended))
					{
						flag2 = base.nation.controlPoints.OnlySome<TIControlPoint>((TIControlPoint x) => x.benefitsDisabled);
						goto IL_00E7;
					}
				}
				flag2 = true;
				IL_00E7:
				if (flag2 || flag)
				{
					this.capitalStatusMarker.centralIcon.enabled = false;
					this.capitalStatusMarker.SetCPImages(base.nation, ClearFlag.TurnOn, true, base.activePlayer);
					this.capitalStatusMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					this.capitalStatusMarker.SetTopRightIcon(null, ClearFlag.TurnOff);
				}
				else
				{
					this.capitalStatusMarker.SetCPImages(null, ClearFlag.TurnOff, true, null);
					this.capitalStatusMarker.SetCentralIcon(totalOwningFaction.factionIcon128);
					this.capitalStatusMarker.centralIcon.enabled = true;
					this.capitalStatusMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnTotalControlIconPressed));
					if (base.nation.controlPoints.All<TIControlPoint>((TIControlPoint x) => x.defended))
					{
						this.capitalStatusMarker.SetTopRightIcon(AssetCacheManager.smallDefendInterestsIcon, ClearFlag.TurnOn);
					}
					else if (base.nation.controlPoints.All<TIControlPoint>((TIControlPoint x) => x.benefitsDisabled))
					{
						this.capitalStatusMarker.SetTopRightIcon(AssetCacheManager.smallCrackdownIcon, ClearFlag.TurnOn);
					}
					else
					{
						this.capitalStatusMarker.SetTopRightIcon(null, ClearFlag.TurnOff);
					}
				}
				flag = flag && this.missionTemplate != null;
				if (flag)
				{
					for (int i = 0; i <= base.nation.maxControlPointIndex; i++)
					{
						TIControlPoint ticontrolPoint = base.nation.controlPoints[i];
						if (ticontrolPoint.owned)
						{
							if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(ticontrolPoint) && this.missionTemplate.ContestedMission)
							{
								this.UpdateCPSuccessChanceString(ticontrolPoint);
							}
							else
							{
								this.capitalStatusMarker.SetCPToHitNumber(i, null, ClearFlag.TurnOff);
							}
						}
					}
				}
				else
				{
					for (int j = 0; j <= base.nation.maxControlPointIndex; j++)
					{
						if (base.nation.controlPoints[j].owned)
						{
							this.capitalStatusMarker.SetCPToHitNumber(j, null, ClearFlag.TurnOff);
						}
					}
					if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TINationState)))
					{
						if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(base.nation))
						{
							this.UpdateNationSuccessChanceString();
						}
						else
						{
							this.capitalStatusMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
						}
					}
					else
					{
						this.capitalStatusMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					}
				}
				if (base.region.nation.alienNation)
				{
					this.capitalStatusMarker.TriggerAlienLights(1);
				}
			}
		}

		// Token: 0x04001ABC RID: 6844
		public MarkerController regionStatusMarker;

		// Token: 0x04001ABD RID: 6845
		public MarkerController capitalStatusMarker;

		// Token: 0x04001ABE RID: 6846
		public MarkerController occupationMarker;

		// Token: 0x04001ABF RID: 6847
		private TICouncilorState targetingCouncilor;

		// Token: 0x04001AC0 RID: 6848
		private TIMissionTemplate missionTemplate;

		// Token: 0x04001AC1 RID: 6849
		private bool regionDataDirty;

		// Token: 0x04001AC2 RID: 6850
		private bool capitalMarkerDataDirty;

		// Token: 0x04001AC3 RID: 6851
		private MarkerController designatedAnimationMarker;

		// Token: 0x04001AC4 RID: 6852
		private bool animationsDataDirty;
	}
}
