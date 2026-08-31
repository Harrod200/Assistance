using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200054D RID: 1357
	public class AlienMarkerController : SingleMarkerController
	{
		// Token: 0x060022C5 RID: 8901 RVA: 0x000B4470 File Offset: 0x000B2670
		public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
		{
			base.InitializeWithRegion(regionController, container);
			this.alienFacility = base.region.alienFacility;
			this.alienLanding = base.region.alienLanding;
			this.alienCrashdown = base.region.alienCrashdown;
			this.alienActivity = base.region.alienActivity;
			this.xenoforming = base.region.xenoforming;
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.UpdateAllMarkersForMapActivation), null, null, true, false);
			GameControl.eventManager.AddListener<AlienCrashdownInRegion>(new EventManager.EventDelegate<AlienCrashdownInRegion>(this.UpdateForCrashdown), null, base.region, true, false);
			GameControl.eventManager.AddListener<ArmyTargetAlienAsset>(new EventManager.EventDelegate<ArmyTargetAlienAsset>(this.ActivateAssetTargetsForArmy), null, null, true, false);
			GameControl.eventManager.AddListener<CouncilorTargetAlienActivity>(new EventManager.EventDelegate<CouncilorTargetAlienActivity>(this.ActivateActivityTargetsForCouncilor), null, null, true, false);
			GameControl.eventManager.AddListener<CouncilorTargetAlienAsset>(new EventManager.EventDelegate<CouncilorTargetAlienAsset>(this.ActivateAssetTargetsForCouncilor), null, null, true, false);
			GameControl.eventManager.AddListener<DeTargetAlienActivity>(new EventManager.EventDelegate<DeTargetAlienActivity>(this.DeactivateActivityTargets), null, null, true, false);
			GameControl.eventManager.AddListener<DeTargetAlienAssets>(new EventManager.EventDelegate<DeTargetAlienAssets>(this.DeactivateAssetTargets), null, null, true, false);
			GameControl.eventManager.AddListener<MissionTargettedEvent>(new EventManager.EventDelegate<MissionTargettedEvent>(this.OnNewTargetSelected), null, base.region, true, false);
			this.UpdateAllMarkers();
			this.currentTargetList = new List<TIGameState>();
		}

		// Token: 0x060022C6 RID: 8902 RVA: 0x000B45D5 File Offset: 0x000B27D5
		public void Update()
		{
			if (this.markerDataDirty)
			{
				this.UpdateMarker();
				this.markerDataDirty = false;
			}
		}

		// Token: 0x060022C7 RID: 8903 RVA: 0x000B45EC File Offset: 0x000B27EC
		public void AttemptUpdateMarker()
		{
			if (base.gameObject.activeSelf)
			{
				this.markerDataDirty = true;
				return;
			}
			this.UpdateMarker();
		}

		// Token: 0x060022C8 RID: 8904 RVA: 0x000B4609 File Offset: 0x000B2809
		public override void UpdateMarker()
		{
			this.UpdateAllMarkers();
		}

		// Token: 0x060022C9 RID: 8905 RVA: 0x000B4611 File Offset: 0x000B2811
		public void UpdateAllMarkersForActivityEvent(AlienRegionEntityUpdated e)
		{
			this.UpdateAllMarkers();
		}

		// Token: 0x060022CA RID: 8906 RVA: 0x000B4619 File Offset: 0x000B2819
		public void UpdateForCrashdown(AlienCrashdownInRegion e)
		{
			this.crashdownVisualizationFired = false;
			this.UpdateAlienCrashdownMarker();
			if (this.alienCrashdownMarker != null)
			{
				this.alienCrashdownMarker.TriggerTouchdown(this);
			}
		}

		// Token: 0x060022CB RID: 8907 RVA: 0x000B4644 File Offset: 0x000B2844
		public void UpdateAllMarkersForMapActivation(MapActivationChangedEvent e)
		{
			if (e.active)
			{
				this.AttemptUpdateMarker();
				GameControl.eventManager.AddListener<AlienRegionEntityUpdated>(new EventManager.EventDelegate<AlienRegionEntityUpdated>(this.UpdateAllMarkersForActivityEvent), null, base.region, true, false);
				GameControl.eventManager.AddListener<RegionXenoformingIntelUpdate>(new EventManager.EventDelegate<RegionXenoformingIntelUpdate>(this.UpdateXenoformingMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<AlienFacilityDamaged>(new EventManager.EventDelegate<AlienFacilityDamaged>(this.OnAlienFacilityDamaged), null, this.alienFacility, false, false);
				GameControl.eventManager.AddListener<AlienLandingDamaged>(new EventManager.EventDelegate<AlienLandingDamaged>(this.OnAlienLandingDamaged), null, this.alienLanding, false, false);
				GameControl.eventManager.AddListener<XenoformingDamaged>(new EventManager.EventDelegate<XenoformingDamaged>(this.OnXenoformingDamaged), null, this.xenoforming, false, false);
				GameControl.eventManager.AddListener<XenoformingDestroyed>(new EventManager.EventDelegate<XenoformingDestroyed>(this.OnXenoformingDestroyed), null, this.xenoforming, false, false);
				GameControl.eventManager.AddListener<TIGameStateAttacking>(new EventManager.EventDelegate<TIGameStateAttacking>(this.OnXenoformingAttacking), null, this.xenoforming, false, false);
				return;
			}
			GameControl.eventManager.RemoveListener<AlienRegionEntityUpdated>(new EventManager.EventDelegate<AlienRegionEntityUpdated>(this.UpdateAllMarkersForActivityEvent), null);
			GameControl.eventManager.RemoveListener<RegionXenoformingIntelUpdate>(new EventManager.EventDelegate<RegionXenoformingIntelUpdate>(this.UpdateXenoformingMarker), null);
			GameControl.eventManager.RemoveListener<AlienFacilityDamaged>(new EventManager.EventDelegate<AlienFacilityDamaged>(this.OnAlienFacilityDamaged), null);
			GameControl.eventManager.RemoveListener<AlienLandingDamaged>(new EventManager.EventDelegate<AlienLandingDamaged>(this.OnAlienLandingDamaged), null);
			GameControl.eventManager.RemoveListener<XenoformingDamaged>(new EventManager.EventDelegate<XenoformingDamaged>(this.OnXenoformingDamaged), null);
			GameControl.eventManager.RemoveListener<XenoformingDestroyed>(new EventManager.EventDelegate<XenoformingDestroyed>(this.OnXenoformingDestroyed), null);
			GameControl.eventManager.RemoveListener<TIGameStateAttacking>(new EventManager.EventDelegate<TIGameStateAttacking>(this.OnXenoformingAttacking), null);
		}

		// Token: 0x060022CC RID: 8908 RVA: 0x000B47DD File Offset: 0x000B29DD
		public void UpdateAllMarkers()
		{
			this.UpdateAlienFacilityMarker();
			this.UpdateAlienActivityMarker();
			this.UpdateAlienLandingMarker();
			this.UpdateAlienCrashdownMarker();
			this.UpdateXenoformingMarker();
		}

		// Token: 0x060022CD RID: 8909 RVA: 0x000B4800 File Offset: 0x000B2A00
		public void ActivateAssetTargetsForCouncilor(CouncilorTargetAlienAsset e)
		{
			this.councilorTargetingAlienSurfaceAssetMode = true;
			this.targetingCouncilor = e.councilor;
			this.missionTemplate = e.missionTemplate;
			this.currentTargetList = (List<TIGameState>)this.missionTemplate.GetValidTargets(this.targetingCouncilor);
			if (this.currentTargetList.Contains(this.alienFacility))
			{
				this.UpdateAlienFacilityMarker();
			}
			if (this.currentTargetList.Contains(this.alienLanding))
			{
				this.UpdateAlienLandingMarker();
			}
			if (this.currentTargetList.Contains(this.xenoforming))
			{
				this.UpdateXenoformingMarker();
			}
		}

		// Token: 0x060022CE RID: 8910 RVA: 0x000B4893 File Offset: 0x000B2A93
		public void ActivateAssetTargetsForArmy(ArmyTargetAlienAsset e)
		{
			this.armyTargetingAlienSurfaceAssetMode = true;
			this.targetingArmy = e.army;
			this.operationTemplate = e.operationTemplate;
			this.currentTargetList = this.operationTemplate.GetPossibleTargets(this.targetingArmy, null);
			this.UpdateMarker();
		}

		// Token: 0x060022CF RID: 8911 RVA: 0x000B48D4 File Offset: 0x000B2AD4
		public void ActivateActivityTargetsForCouncilor(CouncilorTargetAlienActivity e)
		{
			this.councilorTargetingAlienActivity = true;
			this.targetingCouncilor = e.councilor;
			this.missionTemplate = e.missionTemplate;
			this.currentTargetList = (List<TIGameState>)this.missionTemplate.GetValidTargets(this.targetingCouncilor);
			if (this.currentTargetList.Contains(this.alienActivity))
			{
				this.UpdateAlienActivityMarker();
			}
			if (this.currentTargetList.Contains(this.alienCrashdown))
			{
				this.UpdateAlienCrashdownMarker();
			}
		}

		// Token: 0x060022D0 RID: 8912 RVA: 0x000B4950 File Offset: 0x000B2B50
		public void DeactivateActivityTargets(DeTargetAlienActivity e)
		{
			if (this.councilorTargetingAlienActivity)
			{
				this.councilorTargetingAlienActivity = false;
				this.ShutdownAllTargetingAnimations();
				if (this.currentTargetList.Contains(this.alienActivity))
				{
					this.UpdateAlienActivityMarker();
				}
				if (this.currentTargetList.Contains(this.alienCrashdown))
				{
					this.UpdateAlienCrashdownMarker();
				}
				this.currentTargetList.Clear();
			}
		}

		// Token: 0x060022D1 RID: 8913 RVA: 0x000B49B0 File Offset: 0x000B2BB0
		public void DeactivateAssetTargets(DeTargetAlienAssets e)
		{
			this.armyTargetingAlienSurfaceAssetMode = false;
			this.councilorTargetingAlienSurfaceAssetMode = false;
			this.ShutdownAllTargetingAnimations();
			if (this.currentTargetList.Contains(this.alienFacility))
			{
				this.UpdateAlienFacilityMarker();
			}
			if (this.currentTargetList.Contains(this.alienLanding))
			{
				this.UpdateAlienLandingMarker();
			}
			if (this.currentTargetList.Contains(this.xenoforming))
			{
				this.UpdateXenoformingMarker();
			}
			this.currentTargetList.Clear();
		}

		// Token: 0x060022D2 RID: 8914 RVA: 0x000B4A27 File Offset: 0x000B2C27
		private void OnAlienFacilityDamaged(AlienFacilityDamaged e)
		{
			if (this.alienFacilityMarker != null && this.alienFacility.VisibleToFaction(base.activePlayer))
			{
				this.alienFacilityMarker.TriggerExplosion();
			}
		}

		// Token: 0x060022D3 RID: 8915 RVA: 0x000B4A55 File Offset: 0x000B2C55
		private void OnAlienLandingDamaged(AlienLandingDamaged e)
		{
			if (this.alienLandingMarker != null)
			{
				this.alienLandingMarker.TriggerExplosion();
			}
		}

		// Token: 0x060022D4 RID: 8916 RVA: 0x000B4A70 File Offset: 0x000B2C70
		private void OnXenoformingDamaged(XenoformingDamaged e)
		{
			if (this.xenoformingMarker != null)
			{
				this.xenoformingMarker.TriggerExplosion();
				this.UpdateXenoformingMarker();
			}
		}

		// Token: 0x060022D5 RID: 8917 RVA: 0x000B4A91 File Offset: 0x000B2C91
		private void OnXenoformingDestroyed(XenoformingDestroyed e)
		{
			if (this.xenoformingMarker != null)
			{
				this.xenoformingMarker.TriggerExplosion();
				this.xenoformingMarker.TriggerDestruction();
				this.UpdateXenoformingMarker();
			}
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x000B4ABD File Offset: 0x000B2CBD
		private void OnXenoformingAttacking(TIGameStateAttacking e)
		{
			if (this.xenoformingMarker != null)
			{
				this.xenoformingMarker.TriggerAttacking();
			}
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x000B4AD8 File Offset: 0x000B2CD8
		public void OnNewTargetSelected(MissionTargettedEvent e)
		{
			if (!(e.councilor.faction == base.activePlayer))
			{
				this.ShutdownAllTargetingAnimations();
				return;
			}
			if (e.target.isRegionAlienEntity)
			{
				this.SetCouncilorTargeting(this.alienCrashdownMarker);
				this.SetCouncilorTargeting(this.alienLandingMarker);
				this.SetCouncilorTargeting(this.alienActivityMarker);
				this.SetCouncilorTargeting(this.alienFacilityMarker);
				this.SetCouncilorTargeting(this.xenoformingMarker);
				this.SetAnimationOnMarker(this.alienCrashdownMarker);
				this.SetAnimationOnMarker(this.alienLandingMarker);
				this.SetAnimationOnMarker(this.alienActivityMarker);
				this.SetAnimationOnMarker(this.alienFacilityMarker);
				this.SetAnimationOnMarker(this.xenoformingMarker);
				return;
			}
			this.ShutdownAllTargetingAnimations();
		}

		// Token: 0x060022D8 RID: 8920 RVA: 0x000B4B98 File Offset: 0x000B2D98
		public void UpdateXenoformingMarker(RegionXenoformingIntelUpdate e)
		{
			if (e.faction == base.activePlayer)
			{
				this.UpdateXenoformingMarker();
			}
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x000B4BB4 File Offset: 0x000B2DB4
		public void ShutdownAllTargetingAnimations()
		{
			if (this.alienActivityMarker != null)
			{
				this.alienActivityMarker.StopSelectionAnimation();
				this.alienActivityMarker.SetTooltip(() => this.alienActivityMarker.BuildTooltipText(this.alienActivity.displayName, base.activePlayer, TIMissionPhaseState.InMissionPhase(), this.alienActivity));
				this.SetAnimationOnMarker(this.alienActivityMarker);
			}
			if (this.alienFacilityMarker != null)
			{
				this.alienFacilityMarker.StopSelectionAnimation();
				this.alienFacilityMarker.SetTooltip(() => this.alienFacilityMarker.BuildTooltipText(this.alienFacility.displayName, base.activePlayer, TIMissionPhaseState.InMissionPhase(), this.alienFacility));
				this.SetAnimationOnMarker(this.alienFacilityMarker);
			}
			if (this.alienCrashdownMarker != null)
			{
				this.alienCrashdownMarker.StopSelectionAnimation();
				this.alienCrashdownMarker.SetTooltip(() => this.alienCrashdownMarker.BuildTooltipText(this.alienCrashdown.displayName, base.activePlayer, TIMissionPhaseState.InMissionPhase(), this.alienCrashdown));
				this.SetAnimationOnMarker(this.alienCrashdownMarker);
			}
			if (this.alienLandingMarker != null)
			{
				this.alienLandingMarker.StopSelectionAnimation();
				this.alienLandingMarker.SetTooltip(() => this.alienLandingMarker.BuildTooltipText(this.alienLanding.displayName, base.activePlayer, TIMissionPhaseState.InMissionPhase(), this.alienLanding));
				this.SetAnimationOnMarker(this.alienLandingMarker);
			}
			if (this.xenoformingMarker != null && this.xenoformingMarker.centralIcon.isActiveAndEnabled)
			{
				this.xenoformingMarker.StopSelectionAnimation();
				this.xenoformingMarker.SetTooltip(() => this.xenoformingMarker.BuildTooltipText(this.xenoforming.description, base.activePlayer, TIMissionPhaseState.InMissionPhase(), this.xenoforming));
				this.SetAnimationOnMarker(this.xenoformingMarker);
			}
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x000B4CFF File Offset: 0x000B2EFF
		private void AlertAnimation(MarkerController marker)
		{
			marker.selectionAnim.gameObject.SetActive(true);
			marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.AlienChevron);
			marker.StartSelectionAnimation();
		}

		// Token: 0x060022DB RID: 8923 RVA: 0x000B4D1F File Offset: 0x000B2F1F
		private void TargetingAnimation(MarkerController marker)
		{
			marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
			marker.StartSelectionAnimation();
		}

		// Token: 0x060022DC RID: 8924 RVA: 0x000B4D30 File Offset: 0x000B2F30
		private void SetAnimationOnMarker(MarkerController marker)
		{
			if (marker != null)
			{
				if (marker.associatedState == base.globalCurrentTarget)
				{
					this.TargetingAnimation(marker);
					return;
				}
				marker.StopSelectionAnimation();
				if (marker == this.xenoformingMarker)
				{
					if (this.xenoformingMarker.centralIcon.gameObject.activeInHierarchy && this.xenoforming.xenoformingLevel >= TIRegionXenoformingState.stage3Xenoforming)
					{
						this.AlertAnimation(this.xenoformingMarker);
						return;
					}
					this.xenoformingMarker.StopCentralIconAnimation();
					return;
				}
				else
				{
					this.AlertAnimation(marker);
				}
			}
		}

		// Token: 0x060022DD RID: 8925 RVA: 0x000B4DC0 File Offset: 0x000B2FC0
		public void UpdateAlienMarker(MarkerController marker, TIRegionAlienEntityState alienEntity)
		{
			marker.associatedState = alienEntity;
			marker.SetCentralIcon(alienEntity.GetIcon(base.activePlayer));
			marker.centralIcon.raycastTarget = true;
			this.SetAnimationOnMarker(marker);
			marker.centralButton.enabled = true;
			marker.SetTooltip(() => marker.BuildTooltipText(alienEntity.isRegionXenoformingState ? new StringBuilder(alienEntity.displayName).AppendLine().AppendLine(alienEntity.ref_xenoforming.severityDescription).ToString() : alienEntity.displayName, this.activePlayer, TIMissionPhaseState.InMissionPhase(), alienEntity));
			marker.SetHoverSprite(base.activePlayer.shouldNeverAttackAliens ? 2 : 1);
		}

		// Token: 0x060022DE RID: 8926 RVA: 0x000B4E78 File Offset: 0x000B3078
		public void UpdateAlienActivityMarker()
		{
			bool flag = this.alienActivity != null && this.alienActivity.VisibleToFaction(base.activePlayer);
			this.alienActivityMarker = base.container.ManageMarkerStack(this.alienActivityMarker, !flag, MarkerType.AlienActivity, base.region, "alienActivity", -1, false);
			if (flag)
			{
				this.UpdateAlienMarker(this.alienActivityMarker, this.alienActivity);
				this.alienActivityMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnAlienActivityClicked));
				this.alienActivityMarker.TriggerAlienLights(5);
				if (!this.SetCouncilorTargeting(this.alienActivityMarker))
				{
					this.alienActivityMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				}
			}
		}

		// Token: 0x060022DF RID: 8927 RVA: 0x000B4F2C File Offset: 0x000B312C
		private void UpdateAlienLandingMarker()
		{
			bool flag = this.alienLanding != null && this.alienLanding.VisibleToFaction(base.activePlayer);
			this.alienLandingMarker = base.container.ManageMarkerStack(this.alienLandingMarker, !flag, MarkerType.AlienLanding, base.region, "alienLanding", -1, false);
			if (flag)
			{
				this.UpdateAlienMarker(this.alienLandingMarker, this.alienLanding);
				this.alienLandingMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnAlienLandingClicked));
				this.alienLandingMarker.TriggerAlienLights(8);
				if (!this.SetCouncilorTargeting(this.alienLandingMarker) && !this.SetArmyTargeting(this.alienLandingMarker))
				{
					this.alienLandingMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				}
				base.container.InitializeGeoscapeModel(this.alienLandingMarker, "3dearthmodels/geoscape_alien_assault_carrier");
			}
		}

		// Token: 0x060022E0 RID: 8928 RVA: 0x000B5004 File Offset: 0x000B3204
		private void UpdateAlienCrashdownMarker()
		{
			bool flag = this.alienCrashdown != null && this.alienCrashdown.VisibleToFaction(base.activePlayer);
			this.alienCrashdownMarker = base.container.ManageMarkerStack(this.alienCrashdownMarker, !flag, MarkerType.AlienCrashdown, base.region, "alienCrashdown", -1, false);
			if (flag)
			{
				this.UpdateAlienMarker(this.alienCrashdownMarker, this.alienCrashdown);
				this.alienCrashdownMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnAlienCrashdownClicked));
				TIDateTime crashdownTime = this.alienCrashdown.crashdownTime;
				if (crashdownTime != null && crashdownTime.DifferenceInSeconds(this.gameTime.currentTime) < (double)60 && !this.crashdownVisualizationFired)
				{
					this.alienCrashdownMarker.TriggerTouchdown(this);
				}
				else
				{
					TIDateTime crashdownTime2 = this.alienCrashdown.crashdownTime;
					if (crashdownTime2 != null && crashdownTime2.DifferenceInDays(this.gameTime.currentTime) < (double)60)
					{
						this.alienCrashdownMarker.TriggerLinearFires();
					}
				}
				if (!this.SetCouncilorTargeting(this.alienCrashdownMarker))
				{
					this.alienCrashdownMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				}
				base.container.InitializeGeoscapeModel(this.alienCrashdownMarker, "3dearthmodels/geoscape_alien_UFO");
			}
		}

		// Token: 0x060022E1 RID: 8929 RVA: 0x000B5138 File Offset: 0x000B3338
		private void UpdateAlienFacilityMarker()
		{
			bool flag = this.alienFacility != null && this.alienFacility.built && this.alienFacility.VisibleToFaction(base.activePlayer);
			this.alienFacilityMarker = base.container.ManageMarkerStack(this.alienFacilityMarker, !flag, MarkerType.AlienFacility, base.region, "alienFacility", -1, false);
			if (flag)
			{
				this.UpdateAlienMarker(this.alienFacilityMarker, this.alienFacility);
				this.alienFacilityMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnAlienFacilityClicked));
				this.alienFacilityMarker.TriggerAlienLights(7);
				if (!this.SetCouncilorTargeting(this.alienFacilityMarker) && !this.SetArmyTargeting(this.alienFacilityMarker))
				{
					this.alienFacilityMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				}
				base.container.InitializeGeoscapeModel(this.alienFacilityMarker, "3dearthmodels/geoscape_alien_facilities");
			}
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x000B521C File Offset: 0x000B341C
		private void UpdateXenoformingMarker()
		{
			bool flag = this.xenoforming != null && this.xenoforming.xenoformingLevel > 0f && this.xenoforming.VisibleToFaction(base.activePlayer);
			this.xenoformingMarker = base.container.ManageMarkerStack(this.xenoformingMarker, !flag, MarkerType.Xenoforming, base.region, "xenoforming", -1, false);
			if (flag)
			{
				this.UpdateAlienMarker(this.xenoformingMarker, this.xenoforming);
				this.xenoformingMarker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnXenoformingMarkerClicked));
				if (!this.SetCouncilorTargeting(this.xenoformingMarker) && !this.SetArmyTargeting(this.xenoformingMarker))
				{
					this.xenoformingMarker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
				}
			}
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x000B52E4 File Offset: 0x000B34E4
		private bool SetCouncilorTargeting(MarkerController marker)
		{
			if (marker != null && ((this.councilorTargetingAlienActivity && (marker == this.alienActivityMarker || marker == this.alienCrashdownMarker)) || (this.councilorTargetingAlienSurfaceAssetMode && (marker == this.alienLandingMarker || marker == this.xenoformingMarker || marker == this.alienFacilityMarker))))
			{
				TIGameState state = marker.associatedState;
				if (this.currentTargetList.Contains(state))
				{
					marker.SetToHitNumber(this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.targetingCouncilor, state, 0f, false, 2), this.missionTemplate.resolutionMethod.automaticSuccess, (base.globalCurrentTarget == state) ? ClearFlag.TurnOff : ClearFlag.TurnOn, 0);
				}
				else
				{
					marker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
					marker.SetTooltip(() => MarkerController.BuildInvalidTargetTooltip(this.missionTemplate.target.ValidateSingleTarget(this.missionTemplate, this.targetingCouncilor, state)));
				}
				return true;
			}
			return false;
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x000B5404 File Offset: 0x000B3604
		private bool SetArmyTargeting(MarkerController marker)
		{
			if (marker != null && this.armyTargetingAlienSurfaceAssetMode && (marker == this.alienLandingMarker || marker == this.xenoformingMarker || marker == this.alienFacilityMarker))
			{
				if (!this.currentTargetList.Contains(marker.associatedState))
				{
					marker.SetTooltip(() => Loc.T("TIMissionTargeting_InvalidTarget", new object[] { marker.associatedState.displayName }));
				}
				return true;
			}
			return false;
		}

		// Token: 0x170004F1 RID: 1265
		// (get) Token: 0x060022E5 RID: 8933 RVA: 0x000B549F File Offset: 0x000B369F
		private bool Targeting
		{
			get
			{
				return this.armyTargetingAlienSurfaceAssetMode || this.councilorTargetingAlienActivity || this.councilorTargetingAlienSurfaceAssetMode;
			}
		}

		// Token: 0x060022E6 RID: 8934 RVA: 0x000B54BC File Offset: 0x000B36BC
		private bool TargetingButInvalidTarget(MarkerController marker)
		{
			return !this.currentTargetList.Contains(marker.associatedState) && ((this.armyTargetingAlienSurfaceAssetMode && (marker == this.alienLandingMarker || marker == this.xenoformingMarker || marker == this.alienFacilityMarker)) || (this.councilorTargetingAlienActivity && (marker == this.alienActivityMarker || marker == this.alienCrashdownMarker)) || (this.councilorTargetingAlienSurfaceAssetMode && (marker == this.alienLandingMarker || marker == this.xenoformingMarker || marker == this.alienFacilityMarker)));
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x000B5570 File Offset: 0x000B3770
		private void TriggerUIEffects(MarkerController controller)
		{
			if (this.Targeting)
			{
				if (this.TargetingButInvalidTarget(controller))
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienEarthObjectSelect", false, false);
				}
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienEarthObjectSelect", false, false);
			}
			GeneralControlsController.SetSelectedState(controller.associatedState, true);
		}

		// Token: 0x060022E8 RID: 8936 RVA: 0x000B55C4 File Offset: 0x000B37C4
		private void OnAlienFacilityClicked(MarkerController controller)
		{
			this.TriggerUIEffects(controller);
			AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Alien_Activity_Earth", false, false);
			GameControl.eventManager.TriggerEvent(new AlienAssetTargetSelected(this.alienFacility), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new AlienRegionMapEntitySelected(this.alienFacility), null, Array.Empty<object>());
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x000B561A File Offset: 0x000B381A
		private void OnAlienActivityClicked(MarkerController controller)
		{
			this.TriggerUIEffects(controller);
			AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Alien_Activity_Earth", false, false);
			GameControl.eventManager.TriggerEvent(new AlienRegionMapEntitySelected(this.alienActivity), null, Array.Empty<object>());
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x000B564A File Offset: 0x000B384A
		private void OnAlienLandingClicked(MarkerController controller)
		{
			this.TriggerUIEffects(controller);
			GameControl.eventManager.TriggerEvent(new AlienAssetTargetSelected(this.alienLanding), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new AlienRegionMapEntitySelected(this.alienLanding), null, Array.Empty<object>());
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x000B5689 File Offset: 0x000B3889
		private void OnAlienCrashdownClicked(MarkerController controller)
		{
			this.TriggerUIEffects(controller);
			AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Alien_Activity_Earth", false, false);
			GameControl.eventManager.TriggerEvent(new AlienRegionMapEntitySelected(this.alienCrashdown), null, Array.Empty<object>());
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x000B56BC File Offset: 0x000B38BC
		private void OnXenoformingMarkerClicked(MarkerController controller)
		{
			this.TriggerUIEffects(controller);
			AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Alien_Xenoforming", false, false);
			GameControl.eventManager.TriggerEvent(new AlienAssetTargetSelected(this.xenoforming), null, Array.Empty<object>());
			GameControl.eventManager.TriggerEvent(new AlienRegionMapEntitySelected(this.xenoforming), null, Array.Empty<object>());
		}

		// Token: 0x04001A64 RID: 6756
		public MarkerController alienFacilityMarker;

		// Token: 0x04001A65 RID: 6757
		private TIRegionAlienFacilityState alienFacility;

		// Token: 0x04001A66 RID: 6758
		public MarkerController alienActivityMarker;

		// Token: 0x04001A67 RID: 6759
		private TIRegionAlienActivityState alienActivity;

		// Token: 0x04001A68 RID: 6760
		public MarkerController alienLandingMarker;

		// Token: 0x04001A69 RID: 6761
		private TIRegionUFOLandingState alienLanding;

		// Token: 0x04001A6A RID: 6762
		public MarkerController alienCrashdownMarker;

		// Token: 0x04001A6B RID: 6763
		private TIRegionUFOCrashdownState alienCrashdown;

		// Token: 0x04001A6C RID: 6764
		public MarkerController xenoformingMarker;

		// Token: 0x04001A6D RID: 6765
		private TIRegionXenoformingState xenoforming;

		// Token: 0x04001A6E RID: 6766
		private bool councilorTargetingAlienSurfaceAssetMode;

		// Token: 0x04001A6F RID: 6767
		private bool councilorTargetingAlienActivity;

		// Token: 0x04001A70 RID: 6768
		private bool armyTargetingAlienSurfaceAssetMode;

		// Token: 0x04001A71 RID: 6769
		private TICouncilorState targetingCouncilor;

		// Token: 0x04001A72 RID: 6770
		private TIMissionTemplate missionTemplate;

		// Token: 0x04001A73 RID: 6771
		private List<TIGameState> currentTargetList;

		// Token: 0x04001A74 RID: 6772
		private TIArmyState targetingArmy;

		// Token: 0x04001A75 RID: 6773
		private IOperation operationTemplate;

		// Token: 0x04001A76 RID: 6774
		private GameTimeManager gameTime;

		// Token: 0x04001A77 RID: 6775
		private bool markerDataDirty;

		// Token: 0x04001A78 RID: 6776
		public bool crashdownVisualizationFired;
	}
}
