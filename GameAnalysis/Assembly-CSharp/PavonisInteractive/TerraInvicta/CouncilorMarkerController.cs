using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000551 RID: 1361
	public class CouncilorMarkerController : SingleMarkerController
	{
		// Token: 0x06002320 RID: 8992 RVA: 0x000B819C File Offset: 0x000B639C
		public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
		{
			base.InitializeWithRegion(regionController, container);
			this.spaceObjectSelection = World.Active.GetExistingManager<SpaceObjectSelection>();
			this.UpdateMarker();
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.UpdateMarker), null, null, true, false);
			GameControl.eventManager.AddListener<TargetCouncilors>(new EventManager.EventDelegate<TargetCouncilors>(this.ActivateCouncilorTargets), null, null, true, false);
			GameControl.eventManager.AddListener<DeTargetCouncilors>(new EventManager.EventDelegate<DeTargetCouncilors>(this.DeactivateCouncilorTargets), null, null, true, false);
			GameControl.eventManager.AddListener<CouncilorSelectedOffMap>(new EventManager.EventDelegate<CouncilorSelectedOffMap>(this.UpdateMarker), null, base.region, false, false);
			GameControl.eventManager.AddListener<TargetOrgs>(new EventManager.EventDelegate<TargetOrgs>(this.ActivateOrgTargets), null, null, true, false);
			GameControl.eventManager.AddListener<DeTargetOrgs>(new EventManager.EventDelegate<DeTargetOrgs>(this.DeactivateOrgTargets), null, null, true, false);
			this.lastUpdateDate = TITimeState.Now();
		}

		// Token: 0x06002321 RID: 8993 RVA: 0x000B8274 File Offset: 0x000B6474
		public void Update()
		{
			if (this.councilorDataDirty)
			{
				this.UpdateMarker();
				this.councilorDataDirty = false;
			}
			if (TITimeState.Now().DifferenceInDays(this.lastUpdateDate) >= (double)this.updateRateInDays)
			{
				this.councilorDataDirty = true;
				this.lastUpdateDate = TITimeState.Now();
			}
		}

		// Token: 0x06002322 RID: 8994 RVA: 0x000B82C1 File Offset: 0x000B64C1
		public void TryUpdateMarker()
		{
			if (base.gameObject.activeInHierarchy)
			{
				this.councilorDataDirty = true;
				return;
			}
			this.UpdateMarker();
		}

		// Token: 0x06002323 RID: 8995 RVA: 0x000B82DE File Offset: 0x000B64DE
		public void UpdateMarker(MissionPhaseStart e)
		{
			this.TryUpdateMarker();
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x000B82E8 File Offset: 0x000B64E8
		public void UpdateMarker(MapActivationChangedEvent e)
		{
			if (e.active)
			{
				this.UpdateMarker();
				GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateMarker), null, base.region, false, false);
				GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateMarker), null, base.region, false, false);
				GameControl.eventManager.AddListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateMarker), null, base.region, false, false);
				GameControl.eventManager.AddListener<CouncilorDepartsRegion>(new EventManager.EventDelegate<CouncilorDepartsRegion>(this.UpdateMarker), null, base.region, false, false);
				GameControl.eventManager.AddListener<MissionPhaseStart>(new EventManager.EventDelegate<MissionPhaseStart>(this.UpdateMarker), null, null, true, false);
				return;
			}
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<CouncilorDepartsRegion>(new EventManager.EventDelegate<CouncilorDepartsRegion>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<MissionPhaseStart>(new EventManager.EventDelegate<MissionPhaseStart>(this.UpdateMarker), null);
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x000B8410 File Offset: 0x000B6610
		public void UpdateMarker(CouncilorPositionUpdated e)
		{
			if (base.activePlayer.HasIntelOnCouncilorLocation(e.councilor))
			{
				this.TryUpdateMarker();
			}
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x000B842B File Offset: 0x000B662B
		public void UpdateMarker(CouncilorDepartsRegion e)
		{
			if (base.activePlayer.HasIntelOnCouncilorLocation(e.councilor))
			{
				this.TryUpdateMarker();
			}
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x000B8448 File Offset: 0x000B6648
		public void OnEnable()
		{
			if (this.friendlyMarker != null && this.friendlyMarker.cachedAnimTrigger != string.Empty)
			{
				this.friendlyMarker.StartAnimations(this.friendlyMarker.cachedAnimTrigger);
			}
			if (this.alienMarker != null && this.alienMarker.cachedAnimTrigger != string.Empty)
			{
				this.alienMarker.StartAnimations(this.alienMarker.cachedAnimTrigger);
			}
			if (this.opposedMarker != null && this.opposedMarker.cachedAnimTrigger != string.Empty)
			{
				this.opposedMarker.StartAnimations(this.opposedMarker.cachedAnimTrigger);
			}
		}

		// Token: 0x06002328 RID: 9000 RVA: 0x000B8508 File Offset: 0x000B6708
		public void UpdateMarker(CouncilorSelectedOffMap e)
		{
			if (!this.unwindStacks && !this.councilorDataDirty)
			{
				if (this.friendlyCouncilors.Contains(e.councilor))
				{
					this.topFriendlyCouncilor = e.councilor;
					this.topFriendlyCouncilorIndex = this.friendlyCouncilors.IndexOf(e.councilor);
					this.FrontNewFriendlyCouncilor();
					return;
				}
				if (this.opposedCouncilors.Contains(e.councilor))
				{
					this.topOpposedCouncilor = e.councilor;
					this.topOpposedCouncilorIndex = this.opposedCouncilors.IndexOf(e.councilor);
					this.FrontNewOpposingCouncilor();
					return;
				}
				if (this.alienCouncilors.Contains(e.councilor))
				{
					this.topAlienCouncilor = e.councilor;
					this.topAlienCouncilorIndex = this.alienCouncilors.IndexOf(e.councilor);
					this.FrontNewAlienCouncilor();
					return;
				}
			}
			else
			{
				this.UpdateMarker();
			}
		}

		// Token: 0x06002329 RID: 9001 RVA: 0x000B85E8 File Offset: 0x000B67E8
		public void UpdateMarker(CouncilorVisibilityChanged e)
		{
			if (GameControl.control.activePlayer == e.viewingFaction)
			{
				this.newlyDiscoveredCouncilor = e.councilor;
			}
			if (e.viewingFaction == base.activePlayer)
			{
				this.TryUpdateMarker();
			}
		}

		// Token: 0x0600232A RID: 9002 RVA: 0x000B8628 File Offset: 0x000B6828
		public void UpdateMarker(CouncilorMissionUpdated e)
		{
			if (base.activePlayer.HasIntelOnCouncilorLocation(e.councilor))
			{
				if (!this.friendlyCouncilors.Contains(e.councilor) && !this.opposedCouncilors.Contains(e.councilor) && !this.alienCouncilors.Contains(e.councilor))
				{
					List<TIGameState> list = this.currentTargetList;
					bool flag;
					if (list == null)
					{
						flag = false;
					}
					else
					{
						TIMissionState mission = e.mission;
						flag = list.Contains((mission != null) ? mission.target : null);
					}
					if (!flag)
					{
						return;
					}
				}
				this.TryUpdateMarker();
			}
		}

		// Token: 0x0600232B RID: 9003 RVA: 0x000B86AD File Offset: 0x000B68AD
		public override void UpdateMarker()
		{
			if (!this.unwindStacks)
			{
				this.UpdateMarkerStacks();
			}
			else
			{
				this.ShowIndividualCouncilors();
			}
			base.container.Refresh();
		}

		// Token: 0x0600232C RID: 9004 RVA: 0x000B86D0 File Offset: 0x000B68D0
		public void ShutDownAllTargetingAnimations()
		{
			if (this.unwindStacks)
			{
				using (List<MarkerController>.Enumerator enumerator = this.individualMarkers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MarkerController markerController = enumerator.Current;
						if (markerController != null)
						{
							markerController.StopSelectionAnimation();
						}
					}
					goto IL_0091;
				}
			}
			if (this.friendlyMarker != null)
			{
				this.friendlyMarker.StopSelectionAnimation();
			}
			if (this.opposedMarker != null)
			{
				this.opposedMarker.StopSelectionAnimation();
			}
			if (this.alienMarker != null)
			{
				this.alienMarker.StopSelectionAnimation();
			}
			IL_0091:
			if (this.orgMarkers != null)
			{
				foreach (MarkerController markerController2 in this.orgMarkers)
				{
					if (markerController2 != null)
					{
						markerController2.StopSelectionAnimation();
					}
				}
			}
		}

		// Token: 0x0600232D RID: 9005 RVA: 0x000B87D0 File Offset: 0x000B69D0
		public void ActivateCouncilorTargets(TargetCouncilors e)
		{
			if (this.targetingMode)
			{
				this.DeactivateCouncilorTargets();
			}
			this.unwindStacks = true;
			this.targetingMode = true;
			GameControl.eventManager.AddListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.OnCouncilorClickedDuringTargeting), null, base.region, true, false);
			this.targetingCouncilor = e.councilor;
			this.missionTemplate = e.missionTemplate;
			this.currentTargetList = (List<TIGameState>)this.missionTemplate.GetValidTargets(this.targetingCouncilor);
			this.UpdateMarker();
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x000B8852 File Offset: 0x000B6A52
		public void DeactivateCouncilorTargets(DeTargetCouncilors e)
		{
			this.DeactivateCouncilorTargets();
		}

		// Token: 0x0600232F RID: 9007 RVA: 0x000B885C File Offset: 0x000B6A5C
		public void DeactivateCouncilorTargets()
		{
			if (this.targetingMode)
			{
				this.unwindStacks = false;
				this.targetingMode = false;
				GameControl.eventManager.RemoveListener<CouncilorMapItemSelected>(new EventManager.EventDelegate<CouncilorMapItemSelected>(this.OnCouncilorClickedDuringTargeting), null);
				this.currentTargetList.Clear();
				this.ShutDownAllTargetingAnimations();
				this.UpdateMarker();
			}
		}

		// Token: 0x06002330 RID: 9008 RVA: 0x000B88AD File Offset: 0x000B6AAD
		public void OnCouncilorClickedDuringTargeting(CouncilorMapItemSelected e)
		{
			if (this.targetingMode && this.currentTargetList.Contains(e.councilor))
			{
				this.TryUpdateMarker();
			}
		}

		// Token: 0x06002331 RID: 9009 RVA: 0x000B88D0 File Offset: 0x000B6AD0
		public void DisableGroupMarkers()
		{
			if (this.friendlyMarker != null)
			{
				this.friendlyMarker = base.container.ManageMarkerStack(this.friendlyMarker, true, MarkerType.Councilor, base.region, "", -1, false);
			}
			if (this.opposedMarker != null)
			{
				this.opposedMarker = base.container.ManageMarkerStack(this.opposedMarker, true, MarkerType.Councilor, base.region, "", -1, false);
			}
			if (this.alienMarker != null)
			{
				this.alienMarker = base.container.ManageMarkerStack(this.alienMarker, true, MarkerType.Councilor, base.region, "", -1, false);
			}
		}

		// Token: 0x06002332 RID: 9010 RVA: 0x000B897C File Offset: 0x000B6B7C
		public void DisableIndividualMarkers()
		{
			for (int i = this.individualMarkers.Count - 1; i >= 0; i--)
			{
				this.individualMarkers[i] = base.container.ManageMarkerStack(this.individualMarkers[i], true, MarkerType.Councilor, base.region, "", -1, false);
			}
			this.individualMarkers.Clear();
			this.localCouncilors.Clear();
		}

		// Token: 0x06002333 RID: 9011 RVA: 0x000B89EC File Offset: 0x000B6BEC
		public void ShowIndividualCouncilors()
		{
			this.DisableGroupMarkers();
			this.localCouncilors.Clear();
			this.localCouncilors.AddRange(this.friendlyCouncilorStack);
			this.localCouncilors.AddRange(this.enemyCouncilorStack);
			this.localCouncilors.AddRange(this.alienCouncilorStack);
			int count = this.individualMarkers.Count;
			int num = 0;
			using (List<TICouncilorState>.Enumerator enumerator = this.localCouncilors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TICouncilorState councilor = enumerator.Current;
					if (num >= this.individualMarkers.Count)
					{
						this.individualMarkers.Add(new MarkerController());
					}
					this.individualMarkers[num] = base.container.ManageMarkerStack(this.individualMarkers[num], false, MarkerType.Councilor, base.region, "indivCouncilor", -1, false);
					CouncilorView viewofCouncilor = base.activePlayer.GetViewofCouncilor(councilor);
					TIFactionState factionCurrent = viewofCouncilor.factionCurrent;
					this.individualMarkers[num].SetHoverSpriteByFaction(factionCurrent);
					this.individualMarkers[num].associatedState = councilor;
					this.individualMarkers[num].SetCentralIcon(viewofCouncilor.mapIconResourcePathCurrent);
					this.individualMarkers[num].SetPrimaryIconBackground(AssetCacheManager.councilorIconBackground, (factionCurrent == null) ? Color.white : factionCurrent.template.color, (factionCurrent == null) ? ClearFlag.TurnOff : ClearFlag.TurnOn);
					this.individualMarkers[num].SetFactionImage(councilor.faction.factionIcon128, (factionCurrent == null) ? ClearFlag.TurnOff : ClearFlag.TurnOn);
					this.individualMarkers[num].SetCentralIconShadow(false);
					this.individualMarkers[num].SetNumber(string.Empty, ClearFlag.TurnOff, false);
					this.individualMarkers[num].SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnIndividualCouncilorMarkerClicked));
					this.individualMarkers[num].SetMissionTimer(councilor);
					if (this.targetingMode)
					{
						if (this.currentTargetList.Contains(this.localCouncilors[num]))
						{
							if (GeneralControlsController.UITargetedState == this.localCouncilors[num])
							{
								this.individualMarkers[num].AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
								this.individualMarkers[num].StartSelectionAnimation();
								this.individualMarkers[num].SetToHitNumber("", true, ClearFlag.TurnOff, 0);
							}
							else
							{
								this.individualMarkers[num].StopSelectionAnimation();
								string successChanceString = this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.targetingCouncilor, this.localCouncilors[num], 0f, false, 2);
								this.individualMarkers[num].SetToHitNumber(successChanceString, this.missionTemplate.resolutionMethod.automaticSuccess, this.missionTemplate.resolutionMethod.automaticSuccess ? ClearFlag.TurnOff : ClearFlag.TurnOn, 0);
							}
							this.individualMarkers[num].SetTooltip(() => this.SetStackTooltip(new List<TICouncilorState> { councilor }));
						}
						else
						{
							string str = MarkerController.BuildInvalidTargetTooltip(this.missionTemplate.target.ValidateSingleTarget(this.missionTemplate, this.targetingCouncilor, this.localCouncilors[num]));
							this.individualMarkers[num].SetTooltip(() => this.SetStackTooltip(str));
							this.individualMarkers[num].SetToHitNumber("", true, ClearFlag.TurnOff, 0);
						}
					}
					else
					{
						this.individualMarkers[num].SetTooltip(() => this.SetStackTooltip(new List<TICouncilorState> { councilor }));
						this.individualMarkers[num].StopSelectionAnimation();
					}
					base.container.ScaleMarker(base.container.GetNewScale(), this.individualMarkers[num]);
					num++;
				}
			}
			for (int i = num; i < count; i++)
			{
				this.individualMarkers[num] = base.container.ManageMarkerStack(this.individualMarkers[num], true, MarkerType.Councilor, base.region, "indivCouncilor", -1, false);
				this.individualMarkers.RemoveAt(num);
			}
		}

		// Token: 0x06002334 RID: 9012 RVA: 0x000B8E64 File Offset: 0x000B7064
		public void OnIndividualCouncilorMarkerClicked(MarkerController controller)
		{
			this.spaceObjectSelection.BlockThisFrame = true;
			TICouncilorState ticouncilorState = this.localCouncilors[this.individualMarkers.IndexOf(controller)];
			if (ticouncilorState != null)
			{
				SoundEffectController.PlaySelectSound(ticouncilorState);
				TIUtilities.GotoGameState(ticouncilorState, false, true, true);
			}
			GameControl.eventManager.TriggerEvent(new CouncilorMapItemSelected(ticouncilorState), null, CouncilorMapItemSelected.MakeSourceObjects(ticouncilorState));
		}

		// Token: 0x06002335 RID: 9013 RVA: 0x000B8EC4 File Offset: 0x000B70C4
		public void UpdateMarkerStacks()
		{
			this.DisableIndividualMarkers();
			this.UpdateFriendlyCouncilorMarkerStackData();
			this.UpdateOpposedCouncilorMarkerStackData();
			this.UpdateAlienCouncilorMarkerStackData();
			this.newlyDiscoveredCouncilor = null;
		}

		// Token: 0x06002336 RID: 9014 RVA: 0x000B8EE8 File Offset: 0x000B70E8
		private void FrontNewFriendlyCouncilor()
		{
			if (this.topFriendlyCouncilorIndex >= this.friendlyCouncilors.Count)
			{
				this.topFriendlyCouncilorIndex = 0;
			}
			this.UpdateCouncilorStackMarker(this.friendlyMarker, this.friendlyCouncilors[this.topFriendlyCouncilorIndex], this.friendlyCouncilors, this.friendlyCouncilors.Count);
		}

		// Token: 0x06002337 RID: 9015 RVA: 0x000B8F40 File Offset: 0x000B7140
		private void FrontNewOpposingCouncilor()
		{
			if (this.topOpposedCouncilorIndex >= this.opposedCouncilors.Count)
			{
				this.topOpposedCouncilorIndex = 0;
			}
			this.UpdateCouncilorStackMarker(this.opposedMarker, this.opposedCouncilors[this.topOpposedCouncilorIndex], this.opposedCouncilors, this.opposedCouncilors.Count);
		}

		// Token: 0x06002338 RID: 9016 RVA: 0x000B8F98 File Offset: 0x000B7198
		private void FrontNewAlienCouncilor()
		{
			if (this.topAlienCouncilorIndex >= this.alienCouncilors.Count)
			{
				this.topAlienCouncilorIndex = 0;
			}
			this.UpdateCouncilorStackMarker(this.alienMarker, this.alienCouncilors[this.topAlienCouncilorIndex], this.alienCouncilors, this.alienCouncilors.Count);
		}

		// Token: 0x06002339 RID: 9017 RVA: 0x000B8FED File Offset: 0x000B71ED
		private string SetStackTooltip(string tooltip)
		{
			return tooltip;
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x000B8FF0 File Offset: 0x000B71F0
		private string SetStackTooltip(IEnumerable<TICouncilorState> councilors)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TICouncilorState ticouncilorState in councilors)
			{
				CouncilorView viewofCouncilor = base.activePlayer.GetViewofCouncilor(ticouncilorState);
				if (councilors.Count<TICouncilorState>() == 1)
				{
					stringBuilder.AppendLine(ticouncilorState.VisibleSummary(base.activePlayer));
				}
				else if (viewofCouncilor.HasMission)
				{
					if (viewofCouncilor.GetActiveMission.resolveTimeAssigned)
					{
						stringBuilder.AppendLine(Loc.T("UI.Markers.CouncilorMarker.TooltipWithMissionResolveTime", new object[]
						{
							ticouncilorState.faction.template.inlineColorString,
							viewofCouncilor.displayNameCurrent,
							viewofCouncilor.councilorJobStringCurrent,
							viewofCouncilor.currentMissionDisplayName,
							viewofCouncilor.currentMissionTargetDisplayName,
							viewofCouncilor.currentMissionResolveTime
						}));
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Markers.CouncilorMarker.TooltipWithMission", new object[]
						{
							ticouncilorState.faction.template.inlineColorString,
							viewofCouncilor.displayNameCurrent,
							viewofCouncilor.councilorJobStringCurrent,
							viewofCouncilor.currentMissionDisplayName,
							viewofCouncilor.currentMissionTargetDisplayName
						}));
					}
				}
				else if (base.activePlayer.HasIntelOnCouncilorBasicData(ticouncilorState))
				{
					stringBuilder.AppendLine(Loc.T("UI.Markers.CouncilorMarker.Tooltip", new object[]
					{
						ticouncilorState.faction.template.inlineColorString,
						viewofCouncilor.displayNameCurrent,
						viewofCouncilor.councilorJobStringCurrent
					}));
				}
				else
				{
					stringBuilder.AppendLine(viewofCouncilor.displayNameCurrent);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x000B91A8 File Offset: 0x000B73A8
		private void UpdateSuccessValue(MarkerController marker, TICouncilorState topCouncilorState)
		{
			CouncilorView viewofCouncilor = base.activePlayer.GetViewofCouncilor(topCouncilorState);
			if (viewofCouncilor.HasMission)
			{
				TIMissionState getActiveMission = viewofCouncilor.GetActiveMission;
				string successChanceString = getActiveMission.missionTemplate.resolutionMethod.GetSuccessChanceString(getActiveMission.missionTemplate, topCouncilorState, getActiveMission.target, getActiveMission.resources, true, 2);
				marker.SetToHitNumber(successChanceString, getActiveMission.missionTemplate.resolutionMethod.automaticSuccess, ClearFlag.TurnOn, 0);
				return;
			}
			marker.SetToHitNumber("", true, ClearFlag.TurnOff, 0);
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x000B9224 File Offset: 0x000B7424
		private void UpdateCouncilorStackMarker(MarkerController marker, TICouncilorState topCouncilorState, List<TICouncilorState> councilorsInStack, int councilorCount)
		{
			CouncilorView viewofCouncilor = base.activePlayer.GetViewofCouncilor(topCouncilorState);
			TIFactionState factionCurrent = viewofCouncilor.factionCurrent;
			marker.SetCentralIcon(viewofCouncilor.mapIconResourcePathCurrent);
			marker.associatedState = topCouncilorState;
			marker.SetHoverSpriteByFaction(viewofCouncilor.factionCurrent);
			marker.SetPrimaryIconBackground(AssetCacheManager.councilorIconBackground, (factionCurrent == null) ? Color.white : factionCurrent.template.color, (factionCurrent == null) ? ClearFlag.TurnOff : ClearFlag.TurnOn);
			marker.SetFactionImage((factionCurrent != null) ? factionCurrent.factionIcon128 : null, (factionCurrent != null) ? ClearFlag.TurnOn : ClearFlag.TurnOff);
			marker.SetMissionTimer(topCouncilorState);
			if (viewofCouncilor.HasMission)
			{
				marker.AssignAnimationToCentralIconSprite(viewofCouncilor.GetActiveMission.missionTemplate, true);
				marker.StartAnimations("Pending");
			}
			else if (viewofCouncilor.GetCompletedMission != null && viewofCouncilor.GetCompletedMission.missionTemplate.persistentEffect)
			{
				marker.AssignAnimationToCentralIconSprite(viewofCouncilor.GetCompletedMission.missionTemplate, false);
				marker.StartAnimations("Resolving");
			}
			else
			{
				marker.StopCentralIconAnimation();
			}
			GameControl.eventManager.RemoveListener<CurrentAssetDeSelected>(new EventManager.EventDelegate<CurrentAssetDeSelected>(this.OnCouncilorAssetDeselected), null);
			GameControl.eventManager.RemoveListener<CurrentOtherStateDeselected>(new EventManager.EventDelegate<CurrentOtherStateDeselected>(this.OnCouncilorOtherStateDeselected), null);
			if (GeneralControlsController.UISelectedAssetState == topCouncilorState)
			{
				marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.GreenSquare);
				marker.StartSelectionAnimation();
				GameControl.eventManager.AddListener<CurrentAssetDeSelected>(new EventManager.EventDelegate<CurrentAssetDeSelected>(this.OnCouncilorAssetDeselected), null, null, true, false);
			}
			else if (GeneralControlsController.UIOtherSelectedState == topCouncilorState)
			{
				if (topCouncilorState.agentForFaction == base.activePlayer)
				{
					marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.GreenSquare);
				}
				else
				{
					marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.RedSquare);
				}
				marker.StartSelectionAnimation();
				GameControl.eventManager.AddListener<CurrentOtherStateDeselected>(new EventManager.EventDelegate<CurrentOtherStateDeselected>(this.OnCouncilorOtherStateDeselected), null, null, true, false);
			}
			else
			{
				marker.StopSelectionAnimation();
			}
			marker.SetCentralIconShadow(councilorCount > 1);
			marker.SetTooltip(() => this.SetStackTooltip(councilorsInStack));
			marker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnCouncilorStackButtonClick));
			marker.SetNumber((councilorCount == 1) ? string.Empty : councilorCount.ToString(), (councilorCount == 1) ? ClearFlag.TurnOff : ClearFlag.TurnOn, false);
			this.UpdateSuccessValue(marker, topCouncilorState);
			base.container.ScaleMarker(base.container.GetNewScale(), marker);
		}

		// Token: 0x170004F5 RID: 1269
		// (get) Token: 0x0600233D RID: 9021 RVA: 0x000B9472 File Offset: 0x000B7672
		private List<TICouncilorState> friendlyCouncilorStack
		{
			get
			{
				return base.activePlayer.councilors.Where<TICouncilorState>((TICouncilorState x) => TIMissionPhaseState.CouncilorLastKnownLocation(base.activePlayer, x) == base.region && !x.InTransit()).ToList<TICouncilorState>();
			}
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x000B9498 File Offset: 0x000B7698
		private void UpdateFriendlyCouncilorMarkerStackData()
		{
			this.friendlyCouncilors.Clear();
			int num = 0;
			foreach (TICouncilorState ticouncilorState in this.friendlyCouncilorStack)
			{
				this.friendlyCouncilors.Add(ticouncilorState);
				num++;
				if (this.topFriendlyCouncilor == null || this.topFriendlyCouncilor.faction != base.activePlayer || this.topFriendlyCouncilor.archived || this.topFriendlyCouncilor.location != base.region)
				{
					this.topFriendlyCouncilor = ticouncilorState;
					this.topFriendlyCouncilorIndex = 0;
				}
			}
			this.friendlyMarker = base.container.ManageMarkerStack(this.friendlyMarker, num == 0, MarkerType.Councilor, base.region, "friendlyCouncilor", -1, false);
			if (this.friendlyMarker != null)
			{
				this.UpdateCouncilorStackMarker(this.friendlyMarker, this.topFriendlyCouncilor, this.friendlyCouncilors, num);
			}
		}

		// Token: 0x170004F6 RID: 1270
		// (get) Token: 0x0600233F RID: 9023 RVA: 0x000B95AC File Offset: 0x000B77AC
		private List<TICouncilorState> enemyCouncilorStack
		{
			get
			{
				return (from x in TIMissionPhaseState.GetVisibleCouncilorsAtLocation(base.activePlayer, base.region, TemplateManager.global.intelToSeeNeutralPawn, 1f, false)
					where x.faction != base.activePlayer && (!x.isAlien || base.activePlayer.GetIntel(x) < TemplateManager.global.intelToSeeCouncilorBasicData)
					select x).ToList<TICouncilorState>();
			}
		}

		// Token: 0x06002340 RID: 9024 RVA: 0x000B95E8 File Offset: 0x000B77E8
		private void UpdateOpposedCouncilorMarkerStackData()
		{
			this.opposedCouncilors.Clear();
			int num = 0;
			foreach (TICouncilorState ticouncilorState in this.enemyCouncilorStack)
			{
				this.opposedCouncilors.Add(ticouncilorState);
				num++;
				if (this.topOpposedCouncilor == null || this.topOpposedCouncilor.faction == null || this.topOpposedCouncilor.faction == base.activePlayer || this.topOpposedCouncilor.archived || !this.enemyCouncilorStack.Contains(this.topOpposedCouncilor) || ticouncilorState == this.newlyDiscoveredCouncilor || this.alienCouncilorStack.Contains(this.topOpposedCouncilor))
				{
					this.topOpposedCouncilor = ticouncilorState;
					this.topOpposedCouncilorIndex = 0;
				}
			}
			this.opposedMarker = base.container.ManageMarkerStack(this.opposedMarker, num == 0, MarkerType.Councilor, base.region, "opposedCouncilor", -1, false);
			if (this.opposedMarker != null)
			{
				this.UpdateCouncilorStackMarker(this.opposedMarker, this.topOpposedCouncilor, this.opposedCouncilors, num);
			}
		}

		// Token: 0x170004F7 RID: 1271
		// (get) Token: 0x06002341 RID: 9025 RVA: 0x000B9730 File Offset: 0x000B7930
		private List<TICouncilorState> alienCouncilorStack
		{
			get
			{
				return (from x in TIMissionPhaseState.GetVisibleCouncilorsAtLocation(base.activePlayer, base.region, TemplateManager.global.intelToSeeCouncilorBasicData, 1f, false)
					where x.faction != base.activePlayer && x.isAlien
					select x).ToList<TICouncilorState>();
			}
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x000B976C File Offset: 0x000B796C
		private void UpdateAlienCouncilorMarkerStackData()
		{
			this.alienCouncilors.Clear();
			int num = 0;
			foreach (TICouncilorState ticouncilorState in this.alienCouncilorStack)
			{
				if (ticouncilorState.faction != base.activePlayer && ticouncilorState.isAlien)
				{
					this.alienCouncilors.Add(ticouncilorState);
					num++;
					if (this.topAlienCouncilor == null || this.topAlienCouncilor.archived || this.topAlienCouncilor.faction == null || !this.alienCouncilorStack.Contains(this.topAlienCouncilor) || ticouncilorState == this.newlyDiscoveredCouncilor)
					{
						this.topAlienCouncilor = ticouncilorState;
						this.topAlienCouncilorIndex = 0;
					}
				}
			}
			this.alienMarker = base.container.ManageMarkerStack(this.alienMarker, num == 0, MarkerType.Councilor, base.region, "alienCouncilor", -1, false);
			if (this.alienMarker != null)
			{
				this.UpdateCouncilorStackMarker(this.alienMarker, this.topAlienCouncilor, this.alienCouncilors, num);
			}
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x000B98A4 File Offset: 0x000B7AA4
		private void OnCouncilorStackButtonClick(MarkerController controller)
		{
			if (controller == this.friendlyMarker)
			{
				this.topFriendlyCouncilorIndex++;
				if (this.topFriendlyCouncilorIndex < 0 || this.topFriendlyCouncilorIndex >= this.friendlyCouncilors.Count)
				{
					this.topFriendlyCouncilorIndex = 0;
				}
				this.topFriendlyCouncilor = this.friendlyCouncilors[this.topFriendlyCouncilorIndex];
				SoundEffectController.PlaySelectSound(this.topFriendlyCouncilor);
				if (this.topFriendlyCouncilor != null)
				{
					TIUtilities.GotoGameState(this.topFriendlyCouncilor, false, true, true);
				}
				GameControl.eventManager.TriggerEvent(new CouncilorMapItemSelected(this.topFriendlyCouncilor), null, CouncilorMapItemSelected.MakeSourceObjects(this.topFriendlyCouncilor));
				this.FrontNewFriendlyCouncilor();
				return;
			}
			if (controller == this.opposedMarker)
			{
				this.topOpposedCouncilorIndex++;
				if (this.topOpposedCouncilorIndex < 0 || this.topOpposedCouncilorIndex >= this.opposedCouncilors.Count)
				{
					this.topOpposedCouncilorIndex = 0;
				}
				this.topOpposedCouncilor = this.opposedCouncilors[this.topOpposedCouncilorIndex];
				SoundEffectController.PlaySelectSound(this.topOpposedCouncilor);
				if (this.topOpposedCouncilor != null)
				{
					TIUtilities.GotoGameState(this.topOpposedCouncilor, false, true, true);
				}
				GameControl.eventManager.TriggerEvent(new CouncilorMapItemSelected(this.topOpposedCouncilor), null, CouncilorMapItemSelected.MakeSourceObjects(this.topOpposedCouncilor));
				this.FrontNewOpposingCouncilor();
				return;
			}
			if (controller == this.alienMarker)
			{
				this.topAlienCouncilorIndex++;
				if (this.topAlienCouncilorIndex < 0 || this.topAlienCouncilorIndex >= this.alienCouncilors.Count)
				{
					this.topAlienCouncilorIndex = 0;
				}
				this.topAlienCouncilor = this.alienCouncilors[this.topAlienCouncilorIndex];
				SoundEffectController.PlaySelectSound(this.topAlienCouncilor);
				if (this.topAlienCouncilor != null)
				{
					TIUtilities.GotoGameState(this.topAlienCouncilor, false, true, true);
				}
				GameControl.eventManager.TriggerEvent(new CouncilorMapItemSelected(this.topAlienCouncilor), null, CouncilorMapItemSelected.MakeSourceObjects(this.topAlienCouncilor));
				this.FrontNewAlienCouncilor();
			}
		}

		// Token: 0x06002344 RID: 9028 RVA: 0x000B9AA8 File Offset: 0x000B7CA8
		private void OnCouncilorAssetDeselected(CurrentAssetDeSelected e)
		{
			if (!this.unwindStacks)
			{
				this.councilorDataDirty = true;
			}
			GameControl.eventManager.RemoveListener<CurrentAssetDeSelected>(new EventManager.EventDelegate<CurrentAssetDeSelected>(this.OnCouncilorAssetDeselected), null);
		}

		// Token: 0x06002345 RID: 9029 RVA: 0x000B9AD0 File Offset: 0x000B7CD0
		private void OnCouncilorOtherStateDeselected(CurrentOtherStateDeselected e)
		{
			if (!this.unwindStacks)
			{
				this.councilorDataDirty = true;
			}
			GameControl.eventManager.RemoveListener<CurrentOtherStateDeselected>(new EventManager.EventDelegate<CurrentOtherStateDeselected>(this.OnCouncilorOtherStateDeselected), null);
		}

		// Token: 0x06002346 RID: 9030 RVA: 0x000B9AF8 File Offset: 0x000B7CF8
		private void ActivateOrgTargets(TargetOrgs e)
		{
			this.orgTargetingMode = true;
			this.targetingCouncilor = e.councilor;
			this.missionTemplate = e.missionTemplate;
			this.localOrgs = (from x in e.validTargets
				where x.ref_org.homeRegion == base.region
				orderby x.ref_org.factionOrbit, x.ref_org.tier descending
				select x).ToList<TIGameState>().ConvertAll<TIOrgState>((TIGameState x) => x.ref_org);
			int num = 0;
			GameControl.eventManager.AddListener<OrgSelectedEvent>(new EventManager.EventDelegate<OrgSelectedEvent>(this.OnOrgTargetSelected), null, null, true, false);
			using (List<TIOrgState>.Enumerator enumerator = this.localOrgs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIOrgState org = enumerator.Current;
					if (num >= this.orgMarkers.Count)
					{
						this.orgMarkers.Add(new MarkerController());
					}
					this.orgMarkers[num] = base.container.ManageMarkerStack(this.orgMarkers[num], false, MarkerType.Org, base.region, org.displayName, -1, false);
					this.orgMarkers[num].SetHoverSpriteByFaction(org.factionOrbit);
					this.orgMarkers[num].associatedState = org;
					this.orgMarkers[num].SetCentralIcon(org.icon);
					this.orgMarkers[num].SetCentralIconShadow(true);
					this.orgMarkers[num].SetFactionImage(org.factionOrbit.factionIcon128, ClearFlag.NoChange);
					this.orgMarkers[num].SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnOrgMarkerClicked));
					this.orgMarkers[num].SetTooltip(() => org.description(true, GameControl.control.activePlayer, true, false));
					string successChanceString = this.missionTemplate.resolutionMethod.GetSuccessChanceString(this.missionTemplate, this.targetingCouncilor, org, 0f, false, 2);
					this.orgMarkers[num].SetToHitNumber(successChanceString, this.missionTemplate.resolutionMethod.automaticSuccess, ClearFlag.TurnOn, 0);
					this.orgMarkers[num].SetNumber(org.smallTierStarsInline, ClearFlag.TurnOn, true);
					if (this.orgMarkers[num].associatedState == e.starterTarget)
					{
						this.orgMarkers[num].StartSelectionAnimation();
					}
					base.container.ScaleMarker(base.container.GetNewScale(), this.orgMarkers[num]);
					num++;
				}
			}
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x000B9E00 File Offset: 0x000B8000
		private void DeactivateOrgTargets(DeTargetOrgs e)
		{
			this.orgTargetingMode = false;
			for (int i = this.orgMarkers.Count - 1; i >= 0; i--)
			{
				this.orgMarkers[i] = base.container.ManageMarkerStack(this.orgMarkers[i], true, MarkerType.Org, base.region, "", -1, false);
			}
			this.targetingCouncilor = null;
			this.missionTemplate = null;
			this.localOrgs = new List<TIOrgState>();
			this.orgMarkers.Clear();
			GameControl.eventManager.RemoveListener<OrgSelectedEvent>(new EventManager.EventDelegate<OrgSelectedEvent>(this.OnOrgTargetSelected), null);
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x000B9E9A File Offset: 0x000B809A
		public void OnOrgMarkerClicked(MarkerController controller)
		{
			SoundEffectController.PlaySelectSound(controller.associatedState);
			GameControl.eventManager.TriggerEvent(new OrgSelectedEvent(controller.associatedState.ref_org), null, Array.Empty<object>());
		}

		// Token: 0x06002349 RID: 9033 RVA: 0x000B9EC8 File Offset: 0x000B80C8
		public void OnOrgTargetSelected(OrgSelectedEvent e)
		{
			for (int i = this.orgMarkers.Count - 1; i >= 0; i--)
			{
				if (this.orgTargetingMode && this.orgMarkers[i].associatedState == e.org)
				{
					this.orgMarkers[i].StartSelectionAnimation();
				}
				else
				{
					this.orgMarkers[i].StopSelectionAnimation();
				}
			}
		}

		// Token: 0x04001A90 RID: 6800
		public MarkerController friendlyMarker;

		// Token: 0x04001A91 RID: 6801
		public List<TICouncilorState> friendlyCouncilors = new List<TICouncilorState>();

		// Token: 0x04001A92 RID: 6802
		public TICouncilorState topFriendlyCouncilor;

		// Token: 0x04001A93 RID: 6803
		public int topFriendlyCouncilorIndex;

		// Token: 0x04001A94 RID: 6804
		public MarkerController opposedMarker;

		// Token: 0x04001A95 RID: 6805
		public List<TICouncilorState> opposedCouncilors = new List<TICouncilorState>();

		// Token: 0x04001A96 RID: 6806
		public TICouncilorState topOpposedCouncilor;

		// Token: 0x04001A97 RID: 6807
		public int topOpposedCouncilorIndex;

		// Token: 0x04001A98 RID: 6808
		public MarkerController alienMarker;

		// Token: 0x04001A99 RID: 6809
		public List<TICouncilorState> alienCouncilors = new List<TICouncilorState>();

		// Token: 0x04001A9A RID: 6810
		public TICouncilorState topAlienCouncilor;

		// Token: 0x04001A9B RID: 6811
		public int topAlienCouncilorIndex;

		// Token: 0x04001A9C RID: 6812
		private bool unwindStacks;

		// Token: 0x04001A9D RID: 6813
		private bool targetingMode;

		// Token: 0x04001A9E RID: 6814
		private bool orgTargetingMode;

		// Token: 0x04001A9F RID: 6815
		private TIMissionTemplate missionTemplate;

		// Token: 0x04001AA0 RID: 6816
		private TICouncilorState targetingCouncilor;

		// Token: 0x04001AA1 RID: 6817
		private List<TIGameState> currentTargetList;

		// Token: 0x04001AA2 RID: 6818
		public List<MarkerController> individualMarkers = new List<MarkerController>();

		// Token: 0x04001AA3 RID: 6819
		public List<TICouncilorState> localCouncilors = new List<TICouncilorState>();

		// Token: 0x04001AA4 RID: 6820
		public List<MarkerController> orgMarkers = new List<MarkerController>();

		// Token: 0x04001AA5 RID: 6821
		public List<TIOrgState> localOrgs = new List<TIOrgState>();

		// Token: 0x04001AA6 RID: 6822
		private MarkerController targetedOrgMarker;

		// Token: 0x04001AA7 RID: 6823
		private SpaceObjectSelection spaceObjectSelection;

		// Token: 0x04001AA8 RID: 6824
		private bool councilorDataDirty;

		// Token: 0x04001AA9 RID: 6825
		private int updateRateInDays = 1;

		// Token: 0x04001AAA RID: 6826
		private TIDateTime lastUpdateDate;

		// Token: 0x04001AAB RID: 6827
		private TICouncilorState newlyDiscoveredCouncilor;
	}
}
