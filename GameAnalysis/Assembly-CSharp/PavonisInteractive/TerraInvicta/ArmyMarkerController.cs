using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Animations;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200054E RID: 1358
	public class ArmyMarkerController : SingleMarkerController
	{
		// Token: 0x060022F3 RID: 8947 RVA: 0x000B57E8 File Offset: 0x000B39E8
		private string ambientSFXPath(TIArmyState army)
		{
			string text = null;
			if (!army.AlienRegularArmy && !army.AlienMegafaunaArmy)
			{
				if (army.InBattleWithArmies())
				{
					if (army.techLevel < 2f)
					{
						text = "event:/SFX/Environment/trig_SFX_WW2_Tanks_In_Battle";
					}
					else if (army.techLevel < 4f)
					{
						text = "event:/SFX/Environment/trig_SFX_Modern_Tanks_In_Battle";
					}
					else if (army.techLevel < 6f)
					{
						text = "event:/SFX/Environment/trig_SFX_SciFi_Tanks_In_Battle";
					}
					else if (army.techLevel < 8f)
					{
						text = "event:/SFX/Environment/trig_SFX_Hover_Tanks_In_Battle";
					}
				}
				else if (army.IsMoving)
				{
					if (army.techLevel < 2f)
					{
						text = "event:/SFX/Environment/trig_SFX_WW2_Tanks_Moving";
					}
					else if (army.techLevel < 4f)
					{
						text = "event:/SFX/Environment/trig_SFX_Modern_Tanks_Moving";
					}
					else if (army.techLevel < 6f)
					{
						text = "event:/SFX/Environment/trig_SFX_WW2_Tanks_Moving";
					}
					else if (army.techLevel < 8f)
					{
						text = "event:/SFX/Environment/trig_SFX_Hover_Tanks_Moving";
					}
				}
			}
			else if (army.InBattleWithArmies())
			{
				if (army.AlienRegularArmy)
				{
					text = "event:/SFX/Environment/trig_SFX_Alien_Mecha_Tanks_In_Battle";
				}
				else if (army.AlienMegafaunaArmy)
				{
					text = "event:/SFX/Environment/trig_SFX_Kaiju_Monster_In_Battle";
				}
			}
			else if (army.IsMoving)
			{
				if (army.AlienRegularArmy)
				{
					text = "event:/SFX/Environment/trig_SFX_Alien_Mecha_Tanks_Moving";
				}
				else if (army.AlienMegafaunaArmy)
				{
					text = "event:/SFX/Environment/trig_SFX_Kaiju_Monster_Moving";
				}
			}
			return text;
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x000B5929 File Offset: 0x000B3B29
		public override void InitializeWithRegion(RegionController regionController, MarkerContainerController container)
		{
			base.InitializeWithRegion(regionController, container);
			this.UpdateMarker();
			GameControl.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.UpdateMarker), null, null, true, false);
			base.name = base.region.displayName;
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x000B5964 File Offset: 0x000B3B64
		private void Update()
		{
			if (this.armyDataDirty)
			{
				this.UpdateMarker();
				this.armyDataDirty = false;
				this.defendingArmyDataDirty = false;
				this.attackingArmyDataDirty = false;
				this.megafaunaArmyDataDirty = false;
			}
			else
			{
				if (this.attackingArmyDataDirty)
				{
					this.UpdateAttackingArmyMarker();
					this.attackingArmyDataDirty = false;
					base.container.Refresh();
				}
				else if (TIFrameCounter.FrameCount % 100 == (int)base.region.ID % 100)
				{
					this.VisualizeBattle(this.attackingMarker);
				}
				if (this.defendingArmyDataDirty)
				{
					this.UpdateDefendingArmyMarker();
					this.defendingArmyDataDirty = false;
					base.container.Refresh();
				}
				else if (TIFrameCounter.FrameCount % 100 == (int)base.region.ID % 100)
				{
					this.VisualizeBattle(this.defendingMarker);
				}
				if (this.megafaunaArmyDataDirty)
				{
					this.UpdateMegafaunaArmyMarker();
					this.megafaunaArmyDataDirty = false;
					base.container.Refresh();
				}
				else if (TIFrameCounter.FrameCount % 100 == (int)base.region.ID % 100)
				{
					this.VisualizeBattle(this.alienMegafaunaMarker);
				}
			}
			if (this.topMarker != null && this.topMarker.ambientSFX.isValid() && GameControl.control.viewMgr.currentView == ViewType.PoliticalMap)
			{
				this.topMarker.SetModelSFXVolume(base.container.DistanceForCamera());
			}
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x000B5AC9 File Offset: 0x000B3CC9
		private void AttemptUpdateMarker()
		{
			if (base.gameObject.activeSelf)
			{
				this.armyDataDirty = true;
				return;
			}
			this.UpdateMarker();
		}

		// Token: 0x060022F7 RID: 8951 RVA: 0x000B5AE8 File Offset: 0x000B3CE8
		private void UpdateMarker(MapActivationChangedEvent e)
		{
			if (e.active)
			{
				GameControl.eventManager.AddListener<ArmyArrivesInRegion>(new EventManager.EventDelegate<ArmyArrivesInRegion>(this.UpdateMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<RegionControlChanged>(new EventManager.EventDelegate<RegionControlChanged>(this.UpdateMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<ForceAllArmyUpdateInRegion>(new EventManager.EventDelegate<ForceAllArmyUpdateInRegion>(this.UpdateMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<ArmySeaTransitCancelled>(new EventManager.EventDelegate<ArmySeaTransitCancelled>(this.UpdateMarker), null, base.region, true, false);
				GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdateMarker), base.region.ArmyEmbarkEventName, null, true, false);
				GameControl.eventManager.AddListener<OccupationStatusChange>(new EventManager.EventDelegate<OccupationStatusChange>(this.UpdateMarker), null, base.region, true, false);
				this.AttemptUpdateMarker();
				return;
			}
			if (this.topMarker != null)
			{
				this.topMarker.TurnOffAmbientVolume();
			}
			GameControl.eventManager.RemoveListener<ArmyArrivesInRegion>(new EventManager.EventDelegate<ArmyArrivesInRegion>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<RegionControlChanged>(new EventManager.EventDelegate<RegionControlChanged>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<ForceAllArmyUpdateInRegion>(new EventManager.EventDelegate<ForceAllArmyUpdateInRegion>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<ArmySeaTransitCancelled>(new EventManager.EventDelegate<ArmySeaTransitCancelled>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdateMarker), base.region.ArmyEmbarkEventName);
			GameControl.eventManager.RemoveListener<OccupationStatusChange>(new EventManager.EventDelegate<OccupationStatusChange>(this.UpdateMarker), null);
		}

		// Token: 0x060022F8 RID: 8952 RVA: 0x000B5C73 File Offset: 0x000B3E73
		private void UpdateMarker(TimeEventStart e)
		{
			this.AttemptUpdateMarker();
		}

		// Token: 0x060022F9 RID: 8953 RVA: 0x000B5C7B File Offset: 0x000B3E7B
		private void UpdateMarker(ArmyArrivesInRegion e)
		{
			this.AttemptUpdateMarker();
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x000B5C83 File Offset: 0x000B3E83
		private void UpdateMarker(RegionControlChanged e)
		{
			this.AttemptUpdateMarker();
		}

		// Token: 0x060022FB RID: 8955 RVA: 0x000B5C8B File Offset: 0x000B3E8B
		private void UpdateMarker(ForceAllArmyUpdateInRegion e)
		{
			this.AttemptUpdateMarker();
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x000B5C93 File Offset: 0x000B3E93
		private void UpdateMarker(ArmySeaTransitCancelled e)
		{
			this.AttemptUpdateMarker();
		}

		// Token: 0x060022FD RID: 8957 RVA: 0x000B5C9B File Offset: 0x000B3E9B
		private void UpdateMarker(OccupationStatusChange e)
		{
			this.AttemptUpdateMarker();
		}

		// Token: 0x060022FE RID: 8958 RVA: 0x000B5CA3 File Offset: 0x000B3EA3
		public override void UpdateMarker()
		{
			this.UpdateDefendingArmyMarker();
			this.UpdateAttackingArmyMarker();
			this.UpdateMegafaunaArmyMarker();
			base.container.Refresh();
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x000B5CC4 File Offset: 0x000B3EC4
		private void VisualizeBattle(MarkerController marker)
		{
			if (marker != null)
			{
				if (marker.associatedState.ref_army.IsFighting(false))
				{
					if (marker.primaryCentralIconObject.activeSelf && !marker.animating)
					{
						marker.StartAnimations("Fire");
						return;
					}
					if (marker.modelActive && marker.modelAnimatorController.GetAnimationState != ModelAnimatorController.AnimationState.Attack)
					{
						marker.TriggerAttacking();
						return;
					}
				}
				else
				{
					if (marker.primaryCentralIconObject.activeSelf && marker.animating && marker.cachedAnimTrigger == "Fire")
					{
						marker.StopCentralIconAnimation();
						return;
					}
					if (marker.modelActive && marker.modelAnimatorController.GetAnimationState == ModelAnimatorController.AnimationState.Attack)
					{
						marker.modelAnimatorController.PlayAnimationState(ModelAnimatorController.AnimationState.Idle);
					}
				}
			}
		}

		// Token: 0x06002300 RID: 8960 RVA: 0x000B5D80 File Offset: 0x000B3F80
		private void VisualizeAttackingArmyDamage(ArmyTakesDamage e)
		{
			if (this.attackingMarker != null)
			{
				if (e.state == this.topAttackingArmy && e.state.strength <= 0f)
				{
					this.attackingMarker.TriggerDestruction();
					return;
				}
				this.attackingMarker.TriggerExplosion();
			}
		}

		// Token: 0x06002301 RID: 8961 RVA: 0x000B5DD8 File Offset: 0x000B3FD8
		private void VisualizeDefendingArmyDamage(ArmyTakesDamage e)
		{
			if (this.defendingMarker != null)
			{
				if (e.state == this.topDefendingArmy && e.state.strength <= 0f)
				{
					this.defendingMarker.TriggerDestruction();
					return;
				}
				this.defendingMarker.TriggerExplosion();
			}
		}

		// Token: 0x06002302 RID: 8962 RVA: 0x000B5E30 File Offset: 0x000B4030
		private void VisualizeMegafaunaArmyDamage(ArmyTakesDamage e)
		{
			if (this.alienMegafaunaMarker != null)
			{
				if (e.state == this.topMegafaunaArmy && e.state.strength <= 0f)
				{
					this.alienMegafaunaMarker.TriggerDestruction();
					return;
				}
				this.alienMegafaunaMarker.TriggerExplosion();
			}
		}

		// Token: 0x06002303 RID: 8963 RVA: 0x000B5E88 File Offset: 0x000B4088
		private void FrontNewAttackingArmy()
		{
			if (this.attackingArmies[this.topAttackingArmyIndex].deleted)
			{
				this.UpdateAttackingArmyMarker();
				return;
			}
			this.UpdateTopArmyInfo(this.attackingMarker, this.attackingArmies[this.topAttackingArmyIndex], this.attackingArmies.Count);
			this.attackingMarker.markerTooltipTrigger.ForceRefreshTooltip();
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x000B5EEC File Offset: 0x000B40EC
		private void FrontNewDefendingArmy()
		{
			if (this.defendingArmies[this.topDefendingArmyIndex].deleted)
			{
				this.UpdateDefendingArmyMarker();
				return;
			}
			this.UpdateTopArmyInfo(this.defendingMarker, this.defendingArmies[this.topDefendingArmyIndex], this.defendingArmies.Count);
			this.defendingMarker.markerTooltipTrigger.ForceRefreshTooltip();
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x000B5F50 File Offset: 0x000B4150
		private void FrontNewMegaFaunaArmy()
		{
			if (this.megafaunaArmies[this.topMegafaunaArmyIndex].deleted)
			{
				this.UpdateMegafaunaArmyMarker();
				return;
			}
			this.UpdateTopArmyInfo(this.alienMegafaunaMarker, this.megafaunaArmies[this.topMegafaunaArmyIndex], this.megafaunaArmies.Count);
			this.alienMegafaunaMarker.markerTooltipTrigger.ForceRefreshTooltip();
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x000B5FB4 File Offset: 0x000B41B4
		public void MoveToFront(TIArmyState army)
		{
			if (this.attackingArmies.Contains(army))
			{
				this.topAttackingArmy = army;
				this.topAttackingArmyIndex = this.attackingArmies.IndexOf(this.topAttackingArmy);
				this.UpdateTopArmyInfo(this.attackingMarker, army, this.attackingArmies.Count);
				return;
			}
			if (this.defendingArmies.Contains(army))
			{
				this.topDefendingArmy = army;
				this.topDefendingArmyIndex = this.defendingArmies.IndexOf(this.topDefendingArmy);
				this.UpdateTopArmyInfo(this.defendingMarker, army, this.defendingArmies.Count);
				return;
			}
			if (this.megafaunaArmies.Contains(army))
			{
				this.topMegafaunaArmy = army;
				this.topMegafaunaArmyIndex = this.megafaunaArmies.IndexOf(this.topMegafaunaArmy);
				this.UpdateTopArmyInfo(this.alienMegafaunaMarker, army, this.megafaunaArmies.Count);
			}
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x000B6090 File Offset: 0x000B4290
		private string ArmyStackTooltip(IList<TIArmyState> armies, TIArmyState top)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (armies.Count > 1)
			{
				armies.Remove(top);
				armies.Insert(0, top);
			}
			foreach (TIArmyState tiarmyState in armies)
			{
				if (!tiarmyState.destroyed)
				{
					float attackValue = tiarmyState.GetAttackValue();
					if (tiarmyState.AlienMegafaunaArmy)
					{
						stringBuilder.AppendLine(Loc.T("UI.Markers.MonsterArmy", new object[] { tiarmyState.CanHeal() ? TIUtilities.GreenLine(tiarmyState.strength.ToPercent("P0")) : tiarmyState.strength.ToPercent("P0") }));
					}
					else
					{
						if (tiarmyState.deploymentType == DeploymentType.Naval)
						{
							stringBuilder.Append(TemplateManager.global.navyInlineSpritePath);
						}
						if (tiarmyState.faction == null)
						{
							stringBuilder.AppendLine(Loc.T("UI.Markers.NoFactionArmy", new object[]
							{
								tiarmyState.displayName,
								tiarmyState.homeNation.displayName,
								tiarmyState.CanHeal() ? TIUtilities.GreenLine(tiarmyState.strength.ToPercent("P0")) : tiarmyState.strength.ToPercent("P0"),
								attackValue.ToString("N3")
							}));
						}
						else
						{
							stringBuilder.AppendLine(Loc.T("UI.Markers.FactionArmy", new object[]
							{
								tiarmyState.displayName,
								tiarmyState.homeNation.displayName,
								tiarmyState.faction.displayNameCapitalizedWithColor,
								tiarmyState.CanHeal() ? TIUtilities.GreenLine(tiarmyState.strength.ToPercent("P0")) : tiarmyState.strength.ToPercent("P0"),
								attackValue.ToString("N3")
							}));
						}
						if (tiarmyState.CurrentOperations().Count > 0)
						{
							stringBuilder.Append("    ").AppendLine(tiarmyState.OperationDescription());
						}
						if (tiarmyState == top)
						{
							stringBuilder.Append(tiarmyState.CombatBreakdown_Army());
							if (tiarmyState.CanHeal())
							{
								StringBuilder stringBuilder2 = new StringBuilder(Loc.T("UI.Army.HealRate", new object[] { TIUtilities.GreenLine(tiarmyState.dailyHealRate.ToPercent("P2")) }));
								if (tiarmyState.HumanArmy)
								{
									stringBuilder2.Append(Loc.T("UI.Army.HealRate_Human"));
								}
								stringBuilder.AppendLine().AppendLine(stringBuilder2.ToString());
							}
							if (armies.Count > 1)
							{
								stringBuilder.AppendLine();
							}
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x000B6338 File Offset: 0x000B4538
		private void UpdateTopArmyInfo(MarkerController marker, TIArmyState army, int stackSize)
		{
			this.topMarker = marker;
			marker.SetCentralIcon(army.GetForegroundIcon());
			marker.SetPrimaryIconBackground(army.GetIconBackgroundSprite, army.GetIconBackgroundResourceColor, ClearFlag.TurnOn);
			marker.SetCentralIconShadow(stackSize > 1);
			marker.SetNumber((stackSize == 1) ? string.Empty : stackSize.ToString(), (stackSize == 1) ? ClearFlag.TurnOff : ClearFlag.TurnOn, false);
			marker.SetPercentage(army.strength, (army.strength < 1f) ? ClearFlag.TurnOn : ClearFlag.TurnOff);
			marker.SetPercentColor((army.strength < 0.5f) ? Color.red : Color.green);
			marker.SetButtonPressed(new MarkerController.OnMarkerButtonPressed(this.OnArmyButtonClick));
			marker.associatedState = army;
			marker.SetHoverSpriteByFaction(army.faction);
			GameObject gameObject = GameControl.assetLoader.LoadAsset<GameObject>(army.GetModelResource());
			marker.cachedModel = gameObject;
			marker.SetMarkerModel(this.ambientSFXPath(army));
			marker.SetModelSFXVolume(base.container.DistanceForCamera());
			if (!this.rotationInitialized && !army.UseAttackingVisuals)
			{
				marker.model.transform.Rotate(0f, 180f, 0f);
				this.rotationInitialized = true;
			}
			if (!army.AlienMegafaunaArmy)
			{
				marker.SetNationImage(army.homeNation.flag, ClearFlag.TurnOn);
				if (army.faction != null)
				{
					marker.SetArmyFactionImage(army.faction.factionIcon128, ClearFlag.TurnOn);
				}
				else
				{
					marker.SetArmyFactionImage(null, ClearFlag.TurnOff);
				}
			}
			else
			{
				marker.SetNationImage(null, ClearFlag.TurnOff);
				marker.SetArmyFactionImage(null, ClearFlag.TurnOff);
			}
			if (army.deploymentType == DeploymentType.Naval)
			{
				marker.SetTopRightIcon(AssetCacheManager.navalArmyIcon, ClearFlag.TurnOn);
			}
			else
			{
				marker.SetTopRightIcon(null, ClearFlag.TurnOff);
			}
			marker.SetTooltip(() => this.ArmyStackTooltip((marker == this.attackingMarker) ? this.attackingArmies : ((marker == this.defendingMarker) ? this.defendingArmies : this.megafaunaArmies), (marker == this.attackingMarker) ? this.topAttackingArmy : ((marker == this.defendingMarker) ? this.topDefendingArmy : this.topMegafaunaArmy)));
			if (army.IsMoving)
			{
				marker.AssignAnimationToCentralIconSprite(army, false, false);
				marker.StartAnimations("Move");
				marker.armyMovementArrowImage.enabled = true;
				TIRegionState tiregionState = army.currentOperations[0].target as TIRegionState;
				this.UpdateHeading(marker, army);
				marker.armyMovementArrowImage.color = (army.FriendlyRegion(tiregionState) ? new Color32(91, 109, 133, byte.MaxValue) : new Color32(236, 33, 0, byte.MaxValue));
				if (marker.model.activeSelf)
				{
					marker.backgroundIcon.color = Color.clear;
					marker.StopCentralIconAnimation();
				}
			}
			else
			{
				marker.armyMovementArrowImage.enabled = false;
				if (army.IsFighting(true))
				{
					marker.AssignAnimationToCentralIconSprite(army, true, false);
					marker.StartAnimations("Fire");
					if (marker.model.activeSelf)
					{
						marker.backgroundIcon.color = Color.clear;
						marker.StopCentralIconAnimation();
					}
				}
				else
				{
					if (marker.model.activeSelf)
					{
						marker.backgroundIcon.color = Color.clear;
					}
					else
					{
						marker.centralIcon.enabled = true;
					}
					marker.StopCentralIconAnimation();
				}
			}
			if (!this.attackingArmies.Any<TIArmyState>((TIArmyState x) => x.IsFighting(true)))
			{
				if (!this.defendingArmies.Any<TIArmyState>((TIArmyState x) => x.IsFighting(true)))
				{
					if (!this.megafaunaArmies.Any<TIArmyState>((TIArmyState x) => x.IsFighting(true)))
					{
						marker.StopArtilleryFlashes();
						goto IL_0451;
					}
				}
			}
			marker.TriggerArtilleryFlashes();
			IL_0451:
			if (GeneralControlsController.UISelectedAssetState == army)
			{
				marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.GreenSquare);
				marker.StartSelectionAnimation();
				return;
			}
			if (GeneralControlsController.UIOtherSelectedState == army)
			{
				if (army.faction != null)
				{
					marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.RedSquare);
				}
				else
				{
					marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.CyanSquare);
				}
				marker.StartSelectionAnimation();
				return;
			}
			if (GeneralControlsController.UITargetedState == army)
			{
				marker.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.Targeting);
				marker.StartSelectionAnimation();
				return;
			}
			marker.StopSelectionAnimation();
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x000B682C File Offset: 0x000B4A2C
		public static Vector3 GetHeading(TIArmyState army)
		{
			RegionController controller = army.currentRegion.Controller;
			RegionController controller2 = (army.CurrentOperations()[0].target as TIRegionState).Controller;
			if (controller == null || controller2 == null)
			{
				return new Vector3(1f, 0f, 0f);
			}
			Vector3 vector2;
			if (!army.atSea)
			{
				Vector3 vector;
				controller.GetArmyLocation(out vector);
				vector2 = controller.transform.TransformPoint(vector);
			}
			else if (army.SeaTransitStage() == ArmySeaTransitStage.Sea_HomeRegion)
			{
				Vector3 vector3;
				controller.GetSeaLocation(out vector3);
				vector2 = controller.transform.TransformPoint(vector3);
			}
			else
			{
				Vector3 vector4;
				controller2.GetSeaLocation(out vector4);
				vector2 = controller2.transform.TransformPoint(vector4);
			}
			Vector3 normalized = (vector2 - army.ref_spaceBody.controller.transform.position).normalized;
			Vector3 vector5;
			controller2.GetArmyLocation(out vector5);
			Vector3 vector6 = controller2.transform.TransformPoint(vector5) - vector2;
			return vector6 - Vector3.Dot(vector6, normalized) * normalized;
		}

		// Token: 0x0600230A RID: 8970 RVA: 0x000B6938 File Offset: 0x000B4B38
		private void UpdateHeading(MarkerController marker, TIArmyState army)
		{
			TerrestrialUnitModel unitModel = marker.model.GetComponent<ModelAnimatorController>().unitModel;
			Vector3 heading = ArmyMarkerController.GetHeading(army);
			unitModel.transform.localRotation = Quaternion.identity;
			float num = Vector3.SignedAngle(unitModel.transform.rotation * Vector3.forward, heading, unitModel.transform.rotation * Vector3.up);
			unitModel.transform.localRotation = Quaternion.AngleAxis(num, Vector3.up);
			marker.armyMovementArrow.transform.localRotation = Quaternion.identity;
			num = Vector3.SignedAngle(marker.armyMovementArrow.transform.rotation * Vector3.up, heading, marker.armyMovementArrow.transform.rotation * Vector3.forward);
			marker.armyMovementArrow.transform.localRotation = Quaternion.AngleAxis(num, Vector3.forward);
		}

		// Token: 0x0600230B RID: 8971 RVA: 0x000B6A1F File Offset: 0x000B4C1F
		private void OnAttackingArmyStatusUpdate(ArmyStatusUpdate e)
		{
			this.attackingArmyDataDirty = true;
		}

		// Token: 0x0600230C RID: 8972 RVA: 0x000B6A28 File Offset: 0x000B4C28
		private void UpdateAttackingArmyMarker()
		{
			foreach (TIArmyState tiarmyState in this.attackingArmies)
			{
				GameControl.eventManager.RemoveListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnAttackingArmyStatusUpdate), tiarmyState.armyStatusUpdateEventName);
				GameControl.eventManager.RemoveListener<ArmyTakesDamage>(new EventManager.EventDelegate<ArmyTakesDamage>(this.VisualizeAttackingArmyDamage), tiarmyState.armyDamageEventName);
			}
			this.attackingArmies.Clear();
			base.region.IsFullyOccupied();
			this.attackingArmies = base.region.armies.Where<TIArmyState>((TIArmyState x) => x.UseAttackingVisuals && !x.AlienMegafaunaArmy && x.strength > 0f && !x.atSea).ToList<TIArmyState>();
			if (this.attackingArmies.Count > 0)
			{
				if (this.topAttackingArmy == null || this.topAttackingArmy.currentRegion != base.region || this.topAttackingArmy.atSea || !this.attackingArmies.Contains(this.topAttackingArmy))
				{
					this.topAttackingArmy = this.attackingArmies[0];
				}
			}
			else
			{
				this.topAttackingArmy = null;
			}
			foreach (TIArmyState tiarmyState2 in this.attackingArmies)
			{
				GameControl.eventManager.AddListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnAttackingArmyStatusUpdate), tiarmyState2.armyStatusUpdateEventName, tiarmyState2, true, false);
				GameControl.eventManager.AddListener<ArmyTakesDamage>(new EventManager.EventDelegate<ArmyTakesDamage>(this.VisualizeAttackingArmyDamage), tiarmyState2.armyDamageEventName, tiarmyState2, true, false);
			}
			this.attackingMarker = base.container.ManageMarkerStack(this.attackingMarker, this.attackingArmies.Count == 0, MarkerType.Army, base.region, "attackingArmy", 1, false);
			if (this.attackingMarker != null)
			{
				this.UpdateTopArmyInfo(this.attackingMarker, this.topAttackingArmy, this.attackingArmies.Count);
			}
		}

		// Token: 0x0600230D RID: 8973 RVA: 0x000B6C44 File Offset: 0x000B4E44
		private void OnDefendingArmyStatusUpdate(ArmyStatusUpdate e)
		{
			this.defendingArmyDataDirty = true;
		}

		// Token: 0x0600230E RID: 8974 RVA: 0x000B6C50 File Offset: 0x000B4E50
		private void UpdateDefendingArmyMarker()
		{
			foreach (TIArmyState tiarmyState in this.defendingArmies)
			{
				GameControl.eventManager.RemoveListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnDefendingArmyStatusUpdate), tiarmyState.armyStatusUpdateEventName);
				GameControl.eventManager.RemoveListener<ArmyTakesDamage>(new EventManager.EventDelegate<ArmyTakesDamage>(this.VisualizeDefendingArmyDamage), tiarmyState.armyDamageEventName);
			}
			this.defendingArmies.Clear();
			base.region.IsFullyOccupied();
			this.defendingArmies = base.region.armies.Where<TIArmyState>((TIArmyState x) => !x.UseAttackingVisuals && !x.AlienMegafaunaArmy && x.strength > 0f && !x.atSea).ToList<TIArmyState>();
			if (this.defendingArmies.Count > 0)
			{
				if (this.topDefendingArmy == null || this.topDefendingArmy.currentRegion != base.region || this.topDefendingArmy.atSea || !this.defendingArmies.Contains(this.topDefendingArmy))
				{
					this.topDefendingArmy = this.defendingArmies[0];
				}
			}
			else
			{
				this.topDefendingArmy = null;
			}
			foreach (TIArmyState tiarmyState2 in this.defendingArmies)
			{
				GameControl.eventManager.AddListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnDefendingArmyStatusUpdate), tiarmyState2.armyStatusUpdateEventName, tiarmyState2, true, false);
				GameControl.eventManager.AddListener<ArmyTakesDamage>(new EventManager.EventDelegate<ArmyTakesDamage>(this.VisualizeDefendingArmyDamage), tiarmyState2.armyDamageEventName, tiarmyState2, true, false);
			}
			this.defendingMarker = base.container.ManageMarkerStack(this.defendingMarker, this.defendingArmies.Count == 0, MarkerType.Army, base.region, "defendingArmy", 0, false);
			if (this.defendingMarker != null)
			{
				this.UpdateTopArmyInfo(this.defendingMarker, this.topDefendingArmy, this.defendingArmies.Count);
			}
		}

		// Token: 0x0600230F RID: 8975 RVA: 0x000B6E6C File Offset: 0x000B506C
		private void OnAlienMegafaunaArmyStatusUpdate(ArmyStatusUpdate e)
		{
			this.megafaunaArmyDataDirty = true;
		}

		// Token: 0x06002310 RID: 8976 RVA: 0x000B6E78 File Offset: 0x000B5078
		private void UpdateMegafaunaArmyMarker()
		{
			foreach (TIArmyState tiarmyState in this.megafaunaArmies)
			{
				GameControl.eventManager.RemoveListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnAlienMegafaunaArmyStatusUpdate), tiarmyState.armyStatusUpdateEventName);
				GameControl.eventManager.RemoveListener<ArmyTakesDamage>(new EventManager.EventDelegate<ArmyTakesDamage>(this.VisualizeMegafaunaArmyDamage), tiarmyState.armyDamageEventName);
			}
			this.megafaunaArmies.Clear();
			int num = 0;
			foreach (TIArmyState tiarmyState2 in from x in base.region.MegafaunaArmiesPresent()
				where x.strength > 0f
				select x)
			{
				TIMegafaunaArmyState timegafaunaArmyState = tiarmyState2 as TIMegafaunaArmyState;
				if (this.topMegafaunaArmy == null || this.topMegafaunaArmy.currentRegion != base.region || this.topMegafaunaArmy.atSea)
				{
					this.topMegafaunaArmy = timegafaunaArmyState;
				}
				this.megafaunaArmies.Add(timegafaunaArmyState);
				GameControl.eventManager.AddListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.OnAlienMegafaunaArmyStatusUpdate), tiarmyState2.armyStatusUpdateEventName, timegafaunaArmyState, true, false);
				GameControl.eventManager.AddListener<ArmyTakesDamage>(new EventManager.EventDelegate<ArmyTakesDamage>(this.VisualizeMegafaunaArmyDamage), tiarmyState2.armyDamageEventName, timegafaunaArmyState, true, false);
				num++;
			}
			this.alienMegafaunaMarker = base.container.ManageMarkerStack(this.alienMegafaunaMarker, num == 0, MarkerType.Army, base.region, "megafaunaArmy", 2, false);
			if (this.alienMegafaunaMarker != null)
			{
				this.UpdateTopArmyInfo(this.alienMegafaunaMarker, this.topMegafaunaArmy, num);
			}
		}

		// Token: 0x06002311 RID: 8977 RVA: 0x000B7050 File Offset: 0x000B5250
		private void OnArmyButtonClick(MarkerController controller)
		{
			if (controller == this.attackingMarker && this.attackingArmies.Count > 0)
			{
				this.topAttackingArmyIndex++;
				if (this.topAttackingArmyIndex < 0 || this.topAttackingArmyIndex >= this.attackingArmies.Count)
				{
					this.topAttackingArmyIndex = 0;
				}
				this.topAttackingArmy = this.attackingArmies[this.topAttackingArmyIndex];
				SoundEffectController.PlaySelectSound(this.topAttackingArmy);
				TIUtilities.GotoGameState(this.topAttackingArmy, false, true, true, true, false, -1f);
				GameControl.eventManager.TriggerEvent(new ArmyMapItemSelected(this.topAttackingArmy), null, Array.Empty<object>());
				this.FrontNewAttackingArmy();
			}
			if (controller == this.defendingMarker)
			{
				this.topDefendingArmyIndex++;
				if (this.topDefendingArmyIndex < 0 || this.topDefendingArmyIndex >= this.defendingArmies.Count)
				{
					this.topDefendingArmyIndex = 0;
				}
				this.topDefendingArmy = this.defendingArmies[this.topDefendingArmyIndex];
				SoundEffectController.PlaySelectSound(this.topDefendingArmy);
				TIUtilities.GotoGameState(this.topDefendingArmy, false, true, true, true, false, -1f);
				GameControl.eventManager.TriggerEvent(new ArmyMapItemSelected(this.topDefendingArmy), null, Array.Empty<object>());
				this.FrontNewDefendingArmy();
			}
			if (controller == this.alienMegafaunaMarker)
			{
				this.topMegafaunaArmyIndex++;
				if (this.topMegafaunaArmyIndex < 0 || this.topMegafaunaArmyIndex >= this.megafaunaArmies.Count)
				{
					this.topMegafaunaArmyIndex = 0;
				}
				this.topMegafaunaArmy = this.megafaunaArmies[this.topMegafaunaArmyIndex];
				SoundEffectController.PlaySelectSound(this.topMegafaunaArmy);
				TIUtilities.GotoGameState(this.topMegafaunaArmy, false, true, true, true, false, -1f);
				GameControl.eventManager.TriggerEvent(new ArmyMapItemSelected(this.topMegafaunaArmy), null, Array.Empty<object>());
				this.FrontNewMegaFaunaArmy();
			}
		}

		// Token: 0x04001A79 RID: 6777
		public MarkerController defendingMarker;

		// Token: 0x04001A7A RID: 6778
		public TIArmyState topDefendingArmy;

		// Token: 0x04001A7B RID: 6779
		public List<TIArmyState> defendingArmies = new List<TIArmyState>();

		// Token: 0x04001A7C RID: 6780
		public int topDefendingArmyIndex;

		// Token: 0x04001A7D RID: 6781
		public MarkerController attackingMarker;

		// Token: 0x04001A7E RID: 6782
		public TIArmyState topAttackingArmy;

		// Token: 0x04001A7F RID: 6783
		public List<TIArmyState> attackingArmies = new List<TIArmyState>();

		// Token: 0x04001A80 RID: 6784
		public int topAttackingArmyIndex;

		// Token: 0x04001A81 RID: 6785
		public MarkerController alienMegafaunaMarker;

		// Token: 0x04001A82 RID: 6786
		public TIArmyState topMegafaunaArmy;

		// Token: 0x04001A83 RID: 6787
		public List<TIArmyState> megafaunaArmies = new List<TIArmyState>();

		// Token: 0x04001A84 RID: 6788
		public int topMegafaunaArmyIndex;

		// Token: 0x04001A85 RID: 6789
		private bool armyDataDirty;

		// Token: 0x04001A86 RID: 6790
		private bool defendingArmyDataDirty;

		// Token: 0x04001A87 RID: 6791
		private bool attackingArmyDataDirty;

		// Token: 0x04001A88 RID: 6792
		private bool megafaunaArmyDataDirty;

		// Token: 0x04001A89 RID: 6793
		private MarkerController topMarker;

		// Token: 0x04001A8A RID: 6794
		private bool rotationInitialized;
	}
}
