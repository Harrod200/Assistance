using System;
using System.Text;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000552 RID: 1362
	public class FacilityMarkerController : SingleMarkerController
	{
		// Token: 0x0600234F RID: 9039 RVA: 0x000BA034 File Offset: 0x000B8234
		public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
		{
			base.InitializeWithRegion(regionController, container);
			this.launchState = base.region.boostFacility;
			this.missionControlState = base.region.missionControlFacility;
			this.laserState = base.region.spaceDefenseFacility;
			TIRegionSpaceFacilityState tiregionSpaceFacilityState = this.launchState;
			TIRegionSpaceFacilityState tiregionSpaceFacilityState2 = this.missionControlState;
			this.laserState.FacilityMarkerController = this;
			tiregionSpaceFacilityState2.FacilityMarkerController = this;
			tiregionSpaceFacilityState.FacilityMarkerController = this;
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.UpdateMarker), null, null, true, false);
			GameControl.eventManager.AddListener<CouncilorTargetSpaceFacilities>(new EventManager.EventDelegate<CouncilorTargetSpaceFacilities>(this.ActivateFacilityButtons), null, null, false, false);
			GameControl.eventManager.AddListener<ArmyTargetSpaceFacilities>(new EventManager.EventDelegate<ArmyTargetSpaceFacilities>(this.ActivateFacilityButtonsForArmy), null, null, false, false);
			GameControl.eventManager.AddListener<DeTargetSpaceFacilities>(new EventManager.EventDelegate<DeTargetSpaceFacilities>(this.DeactivateFacilityButtons), null, null, false, false);
			GameControl.eventManager.AddListener<MissionTargettedEvent>(new EventManager.EventDelegate<MissionTargettedEvent>(this.OnNewTargetSelected), null, base.region, true, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.LaunchRocket), "Launch Rocket to Orbit", null, true, false);
			GameControl.eventManager.AddListener<LaunchRocketEvent>(new EventManager.EventDelegate<LaunchRocketEvent>(this.LaunchRocket), null, this.launchState, true, false);
			GameControl.eventManager.AddListener<CombatStarts>(new EventManager.EventDelegate<CombatStarts>(this.OnCombatBegins), null, null, true, false);
			this.UpdateMarker();
		}

		// Token: 0x06002350 RID: 9040 RVA: 0x000BA188 File Offset: 0x000B8388
		private void Update()
		{
			if (this.launchDataDirty)
			{
				this.UpdateBoostMarker();
				this.launchDataDirty = false;
				base.container.Refresh();
			}
			if (this.missionControlDataDirty)
			{
				this.UpdateMissionControlMarker();
				base.container.Refresh();
				this.missionControlDataDirty = false;
			}
			if (this.spaceDefensesDataDirty)
			{
				this.UpdateLaserMarker();
				base.container.Refresh();
				this.spaceDefensesDataDirty = false;
			}
		}

		// Token: 0x06002351 RID: 9041 RVA: 0x000BA1F5 File Offset: 0x000B83F5
		private void UpdateMarker(RegionDataUpdated e)
		{
			this.AttemptUpdateMarker();
		}

		// Token: 0x06002352 RID: 9042 RVA: 0x000BA200 File Offset: 0x000B8400
		private void UpdateMarker(MapActivationChangedEvent e)
		{
			if (e.active)
			{
				this.AttemptUpdateMarker();
				GameControl.eventManager.AddListener<RegionDataUpdated>(new EventManager.EventDelegate<RegionDataUpdated>(this.UpdateMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<SpaceFacilityTakesDamage>(new EventManager.EventDelegate<SpaceFacilityTakesDamage>(this.OnLaunchFacilityDamaged), null, this.launchState, true, false);
				GameControl.eventManager.AddListener<SpaceFacilityTakesDamage>(new EventManager.EventDelegate<SpaceFacilityTakesDamage>(this.OnMCFacilityDamaged), null, this.missionControlState, true, false);
				GameControl.eventManager.AddListener<SpaceFacilityTakesDamage>(new EventManager.EventDelegate<SpaceFacilityTakesDamage>(this.OnSpaceDefensesDamaged), null, this.laserState, true, false);
				GameControl.eventManager.AddListener<RegionEntityUpdated>(new EventManager.EventDelegate<RegionEntityUpdated>(this.OnLaunchDataUpdated), null, this.launchState, true, false);
				GameControl.eventManager.AddListener<RegionEntityUpdated>(new EventManager.EventDelegate<RegionEntityUpdated>(this.OnMCDataUpdated), null, this.missionControlState, true, false);
				GameControl.eventManager.AddListener<RegionEntityUpdated>(new EventManager.EventDelegate<RegionEntityUpdated>(this.OnSpaceDefenseDataUpdated), null, this.laserState, true, false);
				return;
			}
			GameControl.eventManager.RemoveListener<RegionDataUpdated>(new EventManager.EventDelegate<RegionDataUpdated>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<SpaceFacilityTakesDamage>(new EventManager.EventDelegate<SpaceFacilityTakesDamage>(this.OnLaunchFacilityDamaged), null);
			GameControl.eventManager.RemoveListener<SpaceFacilityTakesDamage>(new EventManager.EventDelegate<SpaceFacilityTakesDamage>(this.OnMCFacilityDamaged), null);
			GameControl.eventManager.RemoveListener<SpaceFacilityTakesDamage>(new EventManager.EventDelegate<SpaceFacilityTakesDamage>(this.OnSpaceDefensesDamaged), null);
			GameControl.eventManager.RemoveListener<RegionEntityUpdated>(new EventManager.EventDelegate<RegionEntityUpdated>(this.OnLaunchDataUpdated), null);
			GameControl.eventManager.RemoveListener<RegionEntityUpdated>(new EventManager.EventDelegate<RegionEntityUpdated>(this.OnMCDataUpdated), null);
			GameControl.eventManager.RemoveListener<RegionEntityUpdated>(new EventManager.EventDelegate<RegionEntityUpdated>(this.OnSpaceDefenseDataUpdated), null);
		}

		// Token: 0x06002353 RID: 9043 RVA: 0x000BA399 File Offset: 0x000B8599
		public void AttemptUpdateMarker()
		{
			if (base.gameObject.activeSelf)
			{
				this.launchDataDirty = true;
				this.missionControlDataDirty = true;
				this.spaceDefensesDataDirty = true;
				return;
			}
			this.UpdateMarker();
		}

		// Token: 0x06002354 RID: 9044 RVA: 0x000BA3C4 File Offset: 0x000B85C4
		public override void UpdateMarker()
		{
			this.UpdateBoostMarker();
			this.UpdateMissionControlMarker();
			this.UpdateLaserMarker();
			base.container.Refresh();
		}

		// Token: 0x06002355 RID: 9045 RVA: 0x000BA3E3 File Offset: 0x000B85E3
		public void OnLaunchDataUpdated(RegionEntityUpdated e)
		{
			this.launchDataDirty = true;
		}

		// Token: 0x06002356 RID: 9046 RVA: 0x000BA3EC File Offset: 0x000B85EC
		public void OnMCDataUpdated(RegionEntityUpdated e)
		{
			this.missionControlDataDirty = true;
		}

		// Token: 0x06002357 RID: 9047 RVA: 0x000BA3F5 File Offset: 0x000B85F5
		public void OnSpaceDefenseDataUpdated(RegionEntityUpdated e)
		{
			this.spaceDefensesDataDirty = true;
		}

		// Token: 0x06002358 RID: 9048 RVA: 0x000BA3FE File Offset: 0x000B85FE
		public void OnLaunchFacilityDamaged(SpaceFacilityTakesDamage e)
		{
			if (this.boostMarker != null)
			{
				this.boostMarker.TriggerExplosion();
			}
		}

		// Token: 0x06002359 RID: 9049 RVA: 0x000BA419 File Offset: 0x000B8619
		public void OnMCFacilityDamaged(SpaceFacilityTakesDamage e)
		{
			if (this.missionControlMarker != null)
			{
				this.missionControlMarker.TriggerExplosion();
			}
		}

		// Token: 0x0600235A RID: 9050 RVA: 0x000BA434 File Offset: 0x000B8634
		public void OnSpaceDefensesDamaged(SpaceFacilityTakesDamage e)
		{
			if (this.laserMarker != null)
			{
				this.laserMarker.TriggerExplosion();
			}
		}

		// Token: 0x0600235B RID: 9051 RVA: 0x000BA44F File Offset: 0x000B864F
		private void RevertBoostMarker()
		{
			this.boostMarker.highPriority = false;
		}

		// Token: 0x0600235C RID: 9052 RVA: 0x000BA45D File Offset: 0x000B865D
		public void LaunchRocket(TimeEventStart e)
		{
			if (e.eventObject == this.launchState)
			{
				this.LaunchRocket();
			}
		}

		// Token: 0x0600235D RID: 9053 RVA: 0x000BA478 File Offset: 0x000B8678
		public void LaunchRocket(LaunchRocketEvent e)
		{
			this.LaunchRocket();
		}

		// Token: 0x0600235E RID: 9054 RVA: 0x000BA480 File Offset: 0x000B8680
		public void LaunchRocket()
		{
			if (this.boostMarker == null)
			{
				this.UpdateBoostMarker();
				if (this.boostMarker != null)
				{
					this.boostMarker.highPriority = true;
					this.boostMarker.TriggerLaunch();
					base.Invoke("RevertBoostMarker", 6f);
					return;
				}
			}
			else
			{
				this.boostMarker.TriggerLaunch();
			}
		}

		// Token: 0x0600235F RID: 9055 RVA: 0x000BA4E4 File Offset: 0x000B86E4
		public void OnNewTargetSelected(MissionTargettedEvent e)
		{
			if (e.councilor.faction == base.activePlayer && e.target.isRegionSpaceFacility)
			{
				if (this.boostMarker != null)
				{
					if (base.globalCurrentTarget == this.launchState)
					{
						this.boostMarker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
						this.boostMarker.StartSelectionAnimation();
						this.boostMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					}
					else
					{
						this.boostMarker.StopSelectionAnimation();
						if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(this.launchState))
						{
							this.boostMarker.SetToHitNumber("", true, ClearFlag.TurnOn, 0);
						}
					}
				}
				if (this.laserMarker != null)
				{
					if (base.globalCurrentTarget == this.laserState)
					{
						this.laserMarker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
						this.laserMarker.StartSelectionAnimation();
						this.laserMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					}
					else
					{
						this.laserMarker.StopSelectionAnimation();
						if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(this.laserState))
						{
							this.laserMarker.SetToHitNumber("", true, ClearFlag.TurnOn, 0);
						}
					}
				}
				if (this.missionControlMarker != null)
				{
					if (base.globalCurrentTarget == this.missionControlState)
					{
						this.missionControlMarker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
						this.missionControlMarker.StartSelectionAnimation();
						this.missionControlMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
						return;
					}
					this.missionControlMarker.StopSelectionAnimation();
					if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(this.missionControlState))
					{
						this.missionControlMarker.SetToHitNumber("", true, ClearFlag.TurnOn, 0);
						return;
					}
				}
			}
			else
			{
				this.ShutDownTargetingAnimations();
			}
		}

		// Token: 0x06002360 RID: 9056 RVA: 0x000BA6A8 File Offset: 0x000B88A8
		public void ShutDownTargetingAnimations()
		{
			if (this.boostMarker != null)
			{
				this.boostMarker.StopSelectionAnimation();
			}
			if (this.laserMarker != null)
			{
				this.laserMarker.StopSelectionAnimation();
			}
			if (this.missionControlMarker != null)
			{
				this.missionControlMarker.StopSelectionAnimation();
			}
		}

		// Token: 0x06002361 RID: 9057 RVA: 0x000BA700 File Offset: 0x000B8900
		public void ActivateFacilityButtons(CouncilorTargetSpaceFacilities e)
		{
			this.targetingCouncilor = e.councilor;
			this.missionTemplate = e.missionTemplate;
			this.AttemptUpdateMarker();
		}

		// Token: 0x06002362 RID: 9058 RVA: 0x000BA720 File Offset: 0x000B8920
		public void ActivateFacilityButtonsForArmy(ArmyTargetSpaceFacilities e)
		{
			this.AttemptUpdateMarker();
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x000BA728 File Offset: 0x000B8928
		public void DeactivateFacilityButtons(DeTargetSpaceFacilities e)
		{
			this.targetingCouncilor = null;
			this.missionTemplate = null;
			this.ShutDownTargetingAnimations();
			this.AttemptUpdateMarker();
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x000BA744 File Offset: 0x000B8944
		private string LaunchSiteTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Loc.T("UI.Markers.Boost", new object[]
			{
				this.launchState.displayName,
				TemplateManager.global.boostInlineSpritePath,
				TIUtilities.FormatSmallNumber(base.region.boostPerYear_dekatons, 7, 0, true, false)
			}));
			if (this.launchState.region.numSTOFighters > 0)
			{
				stringBuilder.Append(TemplateManager.global.STO_InlineSpritePath).Append(Loc.T("UI.Nation.STOFighters", new object[]
				{
					base.region.availableSTOFighters.ToString(),
					base.region.numSTOFighters.ToString(),
					base.region.maxSTOFighters.ToString()
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002365 RID: 9061 RVA: 0x000BA820 File Offset: 0x000B8A20
		public void UpdateBoostMarker()
		{
			bool flag = this.launchState.Extant();
			this.boostMarker = base.container.ManageMarkerStack(this.boostMarker, !flag, MarkerType.HumanLaunchFacility, base.region, "boostFacility", -1, false);
			if (flag)
			{
				string text = TIUtilities.FormatSmallNumber(base.region.boostPerYear_dekatons, 1, 0, true, false);
				this.boostMarker.SetTooltip(() => this.LaunchSiteTooltip());
				if (text == "0")
				{
					this.boostMarker.SetNumber("Min", ClearFlag.TurnOff, false);
				}
				else
				{
					this.boostMarker.SetNumber(text, ClearFlag.TurnOn, false);
				}
				this.boostMarker.SetTopRightIcon(AssetCacheManager.STOFighterIcon, (base.region.numSTOFighters > 0) ? ClearFlag.TurnOn : ClearFlag.TurnOff);
				this.ShowSpaceFacilityMarker(this.boostMarker, this.launchState);
				string text2;
				switch (this.launchState.GetSize())
				{
				case 1:
					text2 = "small";
					break;
				case 2:
					text2 = "medium";
					break;
				case 3:
					text2 = "large";
					break;
				default:
					Debug.Log("<color=yellow>Launch facility size not found.</color>");
					text2 = "small";
					break;
				}
				string text3 = new StringBuilder("3dearthmodels/geoscape_space_launch_").Append(text2).ToString();
				base.container.InitializeGeoscapeModel(this.boostMarker, text3);
			}
		}

		// Token: 0x06002366 RID: 9062 RVA: 0x000BA96C File Offset: 0x000B8B6C
		public void UpdateMissionControlMarker()
		{
			bool flag = this.missionControlState.Extant();
			this.missionControlMarker = base.container.ManageMarkerStack(this.missionControlMarker, !flag, MarkerType.HumanMissionControlFacility, base.region, "missionControlFacility", -1, false);
			if (flag)
			{
				string MCValue = base.region.missionControl.ToString("N0");
				this.missionControlMarker.SetNumber(MCValue, ClearFlag.TurnOn, false);
				this.missionControlMarker.SetTooltip(() => Loc.T("UI.Markers.MissionControl", new object[]
				{
					this.missionControlState.displayName,
					TemplateManager.global.missionControlInlineSpritePath,
					MCValue,
					this.region.maxMissionControl.ToString("N0")
				}));
				this.ShowSpaceFacilityMarker(this.missionControlMarker, this.missionControlState);
				base.container.InitializeGeoscapeModel(this.missionControlMarker, "3dearthmodels/geoscape_mission_ctrl");
			}
		}

		// Token: 0x06002367 RID: 9063 RVA: 0x000BAA2C File Offset: 0x000B8C2C
		public void UpdateLaserMarker()
		{
			bool flag = this.laserState.Extant();
			this.laserMarker = base.container.ManageMarkerStack(this.laserMarker, !flag, MarkerType.HumanLaserFacility, base.region, "laserFacility", -1, false);
			if (flag)
			{
				this.laserMarker.SetTooltip(() => new StringBuilder(this.laserState.displayName).AppendLine().AppendLine((this.laserState as TISpaceDefensesFacilityState).weaponTemplate.displayName).ToString());
				this.laserMarker.SetNumber(string.Empty, ClearFlag.TurnOff, false);
				this.ShowSpaceFacilityMarker(this.laserMarker, this.laserState);
				base.container.InitializeGeoscapeModel(this.laserMarker, "3dearthmodels/geoscape_laser_orbit");
			}
		}

		// Token: 0x06002368 RID: 9064 RVA: 0x000BAAC4 File Offset: 0x000B8CC4
		public void ShowSpaceFacilityMarker(MarkerController marker, TIRegionSpaceFacilityState facility)
		{
			marker.associatedState = facility;
			marker.SetCentralIcon(facility.GetIcon(base.activePlayer));
			marker.enabled = true;
			marker.SetCentralIconShadow(false);
			marker.centralIcon.raycastTarget = true;
			marker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnMarkerClicked));
			if (this.InTargetingFacilityMode())
			{
				if (this.targetingCouncilor != null)
				{
					if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(facility))
					{
						marker.SetHoverSprite(1);
						this.UpdateFacilitySuccessChanceString(marker, facility);
						return;
					}
					marker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					marker.SetTooltip(() => MarkerController.BuildInvalidTargetTooltip(this.missionTemplate.target.ValidateSingleTarget(this.missionTemplate, this.targetingCouncilor, facility)));
					return;
				}
				else
				{
					marker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(facility))
					{
						marker.SetHoverSprite(1);
						return;
					}
					marker.SetTooltip(() => Loc.T("TIMissionTargeting_InvalidTarget", new object[] { facility.displayName }));
					return;
				}
			}
			else
			{
				marker.SetHoverSprite(0);
				marker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				if (GeneralControlsController.UIOtherSelectedState == facility)
				{
					marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.CyanSquare);
					marker.StartSelectionAnimation();
					return;
				}
				marker.StopSelectionAnimation();
				return;
			}
		}

		// Token: 0x06002369 RID: 9065 RVA: 0x000BAC18 File Offset: 0x000B8E18
		public void UpdateFacilitySuccessChanceString(MarkerController marker, TIGameState target)
		{
			string successChanceString = this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.targetingCouncilor, target, 0f, false, 2);
			marker.SetToHitNumber(successChanceString, this.missionTemplate.resolutionMethod.automaticSuccess, ClearFlag.TurnOn, 0);
		}

		// Token: 0x0600236A RID: 9066 RVA: 0x000BAC63 File Offset: 0x000B8E63
		private bool InTargetingFacilityMode()
		{
			return GeneralControlsController.CurrentlyTargetingStateType(typeof(TIRegionSpaceFacilityState));
		}

		// Token: 0x0600236B RID: 9067 RVA: 0x000BAC74 File Offset: 0x000B8E74
		private void OnMarkerClicked(MarkerController controller)
		{
			if (this.InTargetingFacilityMode())
			{
				if (!GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(controller.associatedState))
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
					controller.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.CyanSquare);
					controller.StartSelectionAnimation();
				}
				else
				{
					SoundEffectController.PlaySelectSound(controller.associatedState);
				}
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
				controller.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.CyanSquare);
				controller.StartSelectionAnimation();
			}
			TIGameState associatedState = controller.associatedState;
			SpaceFacilityType? spaceFacilityType;
			if (associatedState == null)
			{
				spaceFacilityType = null;
			}
			else
			{
				TIRegionSpaceFacilityState ref_regionSpaceFacility = associatedState.ref_regionSpaceFacility;
				spaceFacilityType = ((ref_regionSpaceFacility != null) ? new SpaceFacilityType?(ref_regionSpaceFacility.spaceFacilityType) : null);
			}
			SpaceFacilityType? spaceFacilityType2 = spaceFacilityType;
			if (spaceFacilityType2 != null)
			{
				switch (spaceFacilityType2.GetValueOrDefault())
				{
				case SpaceFacilityType.launchFacility:
					AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Rocket_Launch", false, false);
					break;
				case SpaceFacilityType.missionControlFacility:
					AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Mission_Control", false, false);
					break;
				case SpaceFacilityType.spaceDefenseFacility:
					AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Surface_to_Orbit", false, false);
					break;
				}
			}
			TIGameState associatedState2 = controller.associatedState;
			bool flag;
			if (associatedState2 == null)
			{
				flag = false;
			}
			else
			{
				TIRegionSpaceFacilityState ref_regionSpaceFacility2 = associatedState2.ref_regionSpaceFacility;
				spaceFacilityType2 = ((ref_regionSpaceFacility2 != null) ? new SpaceFacilityType?(ref_regionSpaceFacility2.spaceFacilityType) : null);
				SpaceFacilityType spaceFacilityType3 = SpaceFacilityType.missionControlFacility;
				flag = (spaceFacilityType2.GetValueOrDefault() == spaceFacilityType3) & (spaceFacilityType2 != null);
			}
			if (flag)
			{
				AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Mission_Control", false, false);
			}
			GeneralControlsController.SetSelectedState(controller.associatedState, true);
			GameControl.eventManager.TriggerEvent(new SpaceFacilityMapObjectSelected(controller.associatedState.ref_regionSpaceFacility), null, Array.Empty<object>());
		}

		// Token: 0x0600236C RID: 9068 RVA: 0x000BADDC File Offset: 0x000B8FDC
		public Vector3 BombardmentTargetPosition_Display(TISpaceFleetState fleet)
		{
			return base.cameraManager.ScaledPosition_DoNotTouchCache(base.region.GetGlobalPosition(TITimeState.Now()));
		}

		// Token: 0x0600236D RID: 9069 RVA: 0x000BAE00 File Offset: 0x000B9000
		public void DisplaySTOBeam(TISpaceShipState target, TIDateTime shotTime)
		{
			if (this.weaponController != null)
			{
				return;
			}
			if (this.modelController == null && base.region != null && base.region.ref_spaceBody.controller.modelLink != null)
			{
				this.modelController = base.region.ref_spaceBody.controller.modelLink.GetComponent<SpaceBodyController>();
			}
			if (this.modelController != null)
			{
				SpaceBodyController spaceBodyController = this.modelController;
				Func<TISpaceFleetState, Vector3> func = new Func<TISpaceFleetState, Vector3>(this.BombardmentTargetPosition_Display);
				TIRegionState region = base.region;
				TISpaceObjectState ref_spaceBody = base.region.ref_spaceBody;
				TISpaceDefensesFacilityState tispaceDefensesFacilityState = this.laserState as TISpaceDefensesFacilityState;
				this.weaponController = spaceBodyController.RequestSTOBeam(func, region, target, ref_spaceBody, shotTime, (tispaceDefensesFacilityState != null) ? tispaceDefensesFacilityState.weaponTemplate : null);
				base.Invoke("CeaseDisplayingSTOBeam", 1f);
			}
		}

		// Token: 0x0600236E RID: 9070 RVA: 0x000BAEDA File Offset: 0x000B90DA
		public void CeaseDisplayingSTOBeam()
		{
			if (this.modelController != null)
			{
				this.modelController.ReleaseSTOBeamController(this.weaponController);
				this.weaponController = null;
			}
		}

		// Token: 0x0600236F RID: 9071 RVA: 0x000BAF02 File Offset: 0x000B9102
		public void OnCombatBegins(CombatStarts e)
		{
			if (this.weaponController != null)
			{
				this.CeaseDisplayingSTOBeam();
			}
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x000BAF18 File Offset: 0x000B9118
		public void Facility3DModelHandler(string assetPath, TIRegionSpaceFacilityState facility)
		{
		}

		// Token: 0x04001AAC RID: 6828
		public MarkerController boostMarker;

		// Token: 0x04001AAD RID: 6829
		public MarkerController missionControlMarker;

		// Token: 0x04001AAE RID: 6830
		public MarkerController laserMarker;

		// Token: 0x04001AAF RID: 6831
		public TIRegionSpaceFacilityState launchState;

		// Token: 0x04001AB0 RID: 6832
		public TIRegionSpaceFacilityState missionControlState;

		// Token: 0x04001AB1 RID: 6833
		public TIRegionSpaceFacilityState laserState;

		// Token: 0x04001AB2 RID: 6834
		private TICouncilorState targetingCouncilor;

		// Token: 0x04001AB3 RID: 6835
		private TIMissionTemplate missionTemplate;

		// Token: 0x04001AB4 RID: 6836
		private EventInstance eventInstance;

		// Token: 0x04001AB5 RID: 6837
		private BeamWeaponController weaponController;

		// Token: 0x04001AB6 RID: 6838
		private BeamWeapon weapon;

		// Token: 0x04001AB7 RID: 6839
		private SpaceBodyController modelController;

		// Token: 0x04001AB8 RID: 6840
		private bool launchDataDirty;

		// Token: 0x04001AB9 RID: 6841
		private bool missionControlDataDirty;

		// Token: 0x04001ABA RID: 6842
		private bool spaceDefensesDataDirty;

		// Token: 0x04001ABB RID: 6843
		private const string RevertBoostMarkerStr = "RevertBoostMarker";
	}
}
