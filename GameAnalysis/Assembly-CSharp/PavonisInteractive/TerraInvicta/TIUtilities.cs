using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using AssetBundles;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using PavonisInteractive.TerraInvicta.Systems.UI;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000717 RID: 1815
	public static class TIUtilities
	{
		// Token: 0x170005EF RID: 1519
		// (get) Token: 0x06002B6F RID: 11119 RVA: 0x000ECB0B File Offset: 0x000EAD0B
		public static bool IsInCombatMode
		{
			get
			{
				return TIGlobalValuesState.isSpaceCombatEnabled;
			}
		}

		// Token: 0x170005F0 RID: 1520
		// (get) Token: 0x06002B70 RID: 11120 RVA: 0x000ECB12 File Offset: 0x000EAD12
		public static bool IsInSolarSystemMode
		{
			get
			{
				return GameControl.solarSystem.enabled;
			}
		}

		// Token: 0x170005F1 RID: 1521
		// (get) Token: 0x06002B71 RID: 11121 RVA: 0x000ECB1E File Offset: 0x000EAD1E
		public static bool IsTimeFlowing
		{
			get
			{
				return GameTimeManager.Singleton.IsTimeFlowing;
			}
		}

		// Token: 0x170005F2 RID: 1522
		// (get) Token: 0x06002B72 RID: 11122 RVA: 0x000ECB2A File Offset: 0x000EAD2A
		public static bool IsThereUnresolvedCombats
		{
			get
			{
				return (from x in GameStateManager.IterateByClass<TISpaceCombatState>(false)
					where !x.archived
					select x).Any<TISpaceCombatState>();
			}
		}

		// Token: 0x06002B73 RID: 11123 RVA: 0x000ECB5C File Offset: 0x000EAD5C
		public static bool GetBoolValue(string strValue)
		{
			bool flag;
			if (bool.TryParse(strValue, out flag))
			{
				return flag;
			}
			Log.Error("GetBoolValue tried to convert bad bool value: " + strValue, Array.Empty<object>());
			return false;
		}

		// Token: 0x06002B74 RID: 11124 RVA: 0x000ECB8C File Offset: 0x000EAD8C
		public static float GetFloatValue(string strValue)
		{
			float num;
			if (float.TryParse(strValue, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
			{
				return num;
			}
			Log.Error("GetFloatValue tried to convert bad float value: " + strValue, Array.Empty<object>());
			return 0f;
		}

		// Token: 0x06002B75 RID: 11125 RVA: 0x000ECBCC File Offset: 0x000EADCC
		public static double GetDoubleValue(string strValue)
		{
			double num;
			if (double.TryParse(strValue, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
			{
				return num;
			}
			Log.Error("GetDoubleValue tried to convert bad double value: " + strValue, Array.Empty<object>());
			return 0.0;
		}

		// Token: 0x06002B76 RID: 11126 RVA: 0x000ECC10 File Offset: 0x000EAE10
		public static int GetIntValue(string strValue)
		{
			int num;
			if (int.TryParse(strValue, out num))
			{
				return num;
			}
			Log.Error("GetIntValue tried to convert bad int value: " + strValue, Array.Empty<object>());
			return 0;
		}

		// Token: 0x06002B77 RID: 11127 RVA: 0x000ECC40 File Offset: 0x000EAE40
		public static T GetTemplateValue<T>(string strValue) where T : TIDataTemplate
		{
			T t = TemplateManager.Find<T>(strValue, true);
			if (t != null)
			{
				return t;
			}
			Log.Error("GetTemplateValue tried to convert bad template value: " + strValue, Array.Empty<object>());
			return default(T);
		}

		// Token: 0x06002B78 RID: 11128 RVA: 0x000ECC80 File Offset: 0x000EAE80
		public static string GetDebugString(this TIGameState gameState, bool embedLinks = false)
		{
			if (gameState == null)
			{
				return "null";
			}
			string text = ((gameState != null) ? gameState.ToString() : null) + "/" + gameState.GetDisplayName(GameControl.control.activePlayer);
			if (embedLinks)
			{
				text = string.Concat(new string[]
				{
					"<color=#8888FF><link=\"",
					gameState.ID.ToString(),
					"\">",
					text,
					"</link></color>"
				});
			}
			return text;
		}

		// Token: 0x06002B79 RID: 11129 RVA: 0x000ECD08 File Offset: 0x000EAF08
		public static string GetLocationDebugString(this TIGameState gameState, bool embedLinks = false)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = ((gameState != null) ? gameState.ref_naturalSpaceObject : null);
			if (tinaturalSpaceObjectState == null)
			{
				return "somewhere in space";
			}
			string text = tinaturalSpaceObjectState.GetDebugString(embedLinks);
			if (tinaturalSpaceObjectState.GetSunOrbitingRelatedObject != tinaturalSpaceObjectState)
			{
				text = text + " near " + tinaturalSpaceObjectState.GetSunOrbitingRelatedObject.GetDebugString(embedLinks);
			}
			return text;
		}

		// Token: 0x06002B7A RID: 11130 RVA: 0x000ECD60 File Offset: 0x000EAF60
		public static string GetTrajectoryDebugString(this Trajectory trajectory, bool embedLinks = false)
		{
			return string.Concat(new string[]
			{
				"transferring",
				trajectory.involuntary ? " involuntarily" : "",
				" from ",
				trajectory.originOrbit.GetLocationDebugString(embedLinks),
				" to ",
				trajectory.destination.GetLocationDebugString(embedLinks)
			});
		}

		// Token: 0x06002B7B RID: 11131 RVA: 0x000ECDC8 File Offset: 0x000EAFC8
		public static TIGameState ObjectToSupraLocation(TIGameState state)
		{
			if (state == null)
			{
				throw new ArgumentException("Null Game State passed to ObjecttoSupraLocation", "state");
			}
			if (state.ref_orbit != null)
			{
				return state.ref_orbit;
			}
			if (state.ref_fleet != null)
			{
				return state.ref_fleet;
			}
			if (state.ref_spaceBody != null)
			{
				return state.ref_spaceBody;
			}
			Error.Log("Can't identify location for target in ObjecttoSupraLocation. Target: " + state.displayName, Array.Empty<object>());
			return state;
		}

		// Token: 0x06002B7C RID: 11132 RVA: 0x000ECE48 File Offset: 0x000EB048
		public static TIGameState ObjectToExactLocation(TIGameState state)
		{
			if (state == null)
			{
				Error.Log("Null state passed to ObjectToExactLocation", Array.Empty<object>());
				return null;
			}
			if (state.ref_region != null)
			{
				return state.ref_region;
			}
			if (state.isSpaceShipState)
			{
				return state.ref_ship;
			}
			if (state.isSpaceFleetState && state.ref_fleet.ships.Count > 0)
			{
				return state.ref_fleet.ships[0];
			}
			if (state.ref_hab != null)
			{
				return state.ref_hab;
			}
			if (state.isCouncilorState)
			{
				return state.ref_councilor.location;
			}
			if (state.isHabSiteState)
			{
				return state.ref_habSite;
			}
			if (!state.isOrgState)
			{
				Error.Log("Can't identify location for target in ObjectToExactLocation: " + state.GetType().ToString(), Array.Empty<object>());
				return null;
			}
			if (state.ref_org.homeRegion != null)
			{
				return state.ref_org.homeRegion;
			}
			if (state.ref_org.hasCouncilor)
			{
				return state.ref_org.assignedCouncilor.location;
			}
			return null;
		}

		// Token: 0x06002B7D RID: 11133 RVA: 0x000ECF60 File Offset: 0x000EB160
		public static TIGameState ObjectToScannableLocation(TIGameState state)
		{
			if (state.ref_region != null)
			{
				return state.ref_region;
			}
			if (state.ref_hab != null)
			{
				return state.ref_hab;
			}
			if (state.ref_fleet != null)
			{
				return state.ref_fleet;
			}
			Error.Log("Can't identify location for target in ObjectToScannableLocation", Array.Empty<object>());
			return state;
		}

		// Token: 0x06002B7E RID: 11134 RVA: 0x000ECFBC File Offset: 0x000EB1BC
		public static bool GameStateHasLatLong(TIGameState state)
		{
			return state.ref_region != null || state.ref_habSite != null;
		}

		// Token: 0x06002B7F RID: 11135 RVA: 0x000ECFDC File Offset: 0x000EB1DC
		public static Vector2 GameStateLatLong(TIGameState state)
		{
			if (state.ref_region != null)
			{
				return new Vector2
				{
					x = state.ref_region.latitude,
					y = state.ref_region.longitude
				};
			}
			if (state.ref_hab != null && state.ref_hab.IsBase)
			{
				return new Vector2
				{
					x = state.ref_hab.habSite.latitude,
					y = state.ref_hab.habSite.longitude
				};
			}
			if (state.ref_habSite != null)
			{
				return new Vector2
				{
					x = state.ref_habSite.latitude,
					y = state.ref_habSite.longitude
				};
			}
			return default(Vector2);
		}

		// Token: 0x06002B80 RID: 11136 RVA: 0x000ED0C0 File Offset: 0x000EB2C0
		private static TISpaceObjectState ObjectToSpaceObject(TIGameState state, bool landedFleetUICall)
		{
			if (state.ref_fleet != null)
			{
				if (state.ref_fleet.landed && !landedFleetUICall)
				{
					return state.ref_fleet.ref_spaceBody;
				}
				return state.ref_fleet;
			}
			else
			{
				if (state.ref_hab != null && state.ref_hab.IsStation)
				{
					return state.ref_hab;
				}
				if (state.ref_lagrangePoint != null)
				{
					return state.ref_lagrangePoint;
				}
				if (state.ref_spaceBody != null)
				{
					return state.ref_spaceBody;
				}
				if (state.isOrbitState)
				{
					return state.ref_orbit.barycenter;
				}
				return state as TISpaceObjectState;
			}
		}

		// Token: 0x06002B81 RID: 11137 RVA: 0x000ED164 File Offset: 0x000EB364
		public static bool IsIrradiated(this TIGameState gameState)
		{
			if (gameState.ref_habSite != null)
			{
				return gameState.ref_habSite.irradiated;
			}
			if (gameState.ref_orbit != null)
			{
				return gameState.ref_orbit.irradiated;
			}
			return gameState.isSpaceBodyState && gameState.ref_spaceBody.irradiated;
		}

		// Token: 0x06002B82 RID: 11138 RVA: 0x000ED1BC File Offset: 0x000EB3BC
		public static float IrradiatedMultiplier(TIGameState gameState)
		{
			if (gameState.isHabSiteState)
			{
				return gameState.ref_habSite.irradiatedValue;
			}
			if (gameState.isOrbitState)
			{
				return gameState.ref_orbit.irradiatedValue;
			}
			if (gameState.isSpaceBodyState)
			{
				return gameState.ref_spaceBody.irradiatedMultiplier;
			}
			if (gameState.ref_habSite != null)
			{
				return gameState.ref_habSite.irradiatedValue;
			}
			if (gameState.ref_orbit != null)
			{
				return gameState.ref_orbit.irradiatedValue;
			}
			if (gameState.ref_spaceBody != null)
			{
				return gameState.ref_spaceBody.irradiatedMultiplier;
			}
			return 1f;
		}

		// Token: 0x06002B83 RID: 11139 RVA: 0x000ED258 File Offset: 0x000EB458
		public static void GotoGameState(TIFactionState faction, bool select = false)
		{
			if (faction == GameControl.control.activePlayer)
			{
				World.Active.GetExistingManager<CanvasManager>().ShowInfoScreen<CouncilGridController>();
				return;
			}
			(World.Active.GetExistingManager<CanvasManager>().ShowInfoScreen<IntelScreenController>() as IntelScreenController).ForceActiveTab(faction);
		}

		// Token: 0x06002B84 RID: 11140 RVA: 0x000ED297 File Offset: 0x000EB497
		public static void GotoGameState(TIGlobalResearchState research, bool select = false)
		{
			World.Active.GetExistingManager<CanvasManager>().ShowInfoScreen<ResearchScreenController>();
		}

		// Token: 0x06002B85 RID: 11141 RVA: 0x000ED2A9 File Offset: 0x000EB4A9
		public static void GotoGameState(TIGlobalValuesState values, bool select = false)
		{
			(World.Active.GetExistingManager<CanvasManager>().ShowInfoScreen<IntelScreenController>() as IntelScreenController).ForceActiveTab(values);
		}

		// Token: 0x06002B86 RID: 11142 RVA: 0x000ED2C5 File Offset: 0x000EB4C5
		public static void GotoGameState(TIMissionPhaseState phase, bool select = false)
		{
			World.Active.GetExistingManager<CanvasManager>().ShowInfoScreen<ObjectivesScreenController>();
		}

		// Token: 0x06002B87 RID: 11143 RVA: 0x000ED2D7 File Offset: 0x000EB4D7
		public static void GotoGameState(TICouncilorState councilor, bool moveCamera = true, bool launchUI = true, bool triggerSelectionEvent = true)
		{
			if (councilor.faction == null)
			{
				TIUtilities.GotoGameState(GameControl.control.activePlayer, false);
				return;
			}
			TIUtilities.GotoGameState(new CouncilorView(councilor, GameControl.control.activePlayer), true, moveCamera, launchUI, triggerSelectionEvent);
		}

		// Token: 0x06002B88 RID: 11144 RVA: 0x000ED314 File Offset: 0x000EB514
		public static void GotoGameState(CouncilorView councilorView, bool basicDataVisible, bool moveCamera = true, bool launchUI = true, bool triggerSelectionEvent = true)
		{
			if (basicDataVisible)
			{
				if (TIMissionPhaseState.InMissionPhase() && councilorView.councilor.faction != GameControl.control.activePlayer)
				{
					TIUtilities.GotoGameState(TIMissionPhaseState.CouncilorLastKnownLocation(GameControl.control.activePlayer, councilorView.councilor), moveCamera, false, false, true, false, -1f);
					TIUtilities.GotoGameState(councilorView.councilor, false, launchUI, triggerSelectionEvent, true, false, -1f);
					return;
				}
				TIUtilities.GotoGameState(councilorView.councilor, moveCamera, launchUI, triggerSelectionEvent, true, false, -1f);
				return;
			}
			else
			{
				TIGameState location = councilorView.location;
				if (location == null)
				{
					return;
				}
				TIUtilities.GotoGameState(location, false, true, true, true, false, -1f);
				return;
			}
		}

		// Token: 0x06002B89 RID: 11145 RVA: 0x000ED3BC File Offset: 0x000EB5BC
		public static void GotoGameState(TIGameState gameState, bool moveCamera = true, bool launchUI = true, bool triggerGlobalSelectionEvent = true, bool zoomCamera = true, bool selectSpaceObject = false, float defaultScalingForEarthOverride = -1f)
		{
			if (TIGameState.Valid(gameState))
			{
				if (gameState.hasMapObject)
				{
					if (launchUI)
					{
						TIUtilities.GotoSelectedStateUI(gameState, gameState.isSpaceObjectState ? (selectSpaceObject || moveCamera || zoomCamera) : launchUI);
					}
					if (triggerGlobalSelectionEvent)
					{
						TIUtilities.TriggerSelectionEvent(gameState);
					}
					if (TIInputManager.DoubleClickedGameState(gameState, true))
					{
						moveCamera = true;
					}
					if (moveCamera)
					{
						TISpaceObjectState tispaceObjectState = TIUtilities.ObjectToSpaceObject(gameState, false);
						if (TIGameState.Valid(tispaceObjectState))
						{
							float num;
							switch (tispaceObjectState.objectType)
							{
							case SpaceObjectType.Fleet:
								num = 1.5f;
								break;
							case SpaceObjectType.Hab:
								num = 3f;
								break;
							case SpaceObjectType.LagrangePoint:
							{
								TIOrbitState tiorbitState = tispaceObjectState.ref_lagrangePoint.orbits[0];
								num = (float)((tiorbitState != null) ? (tiorbitState.semiMajorAxis_m * (double)3.25f) : 1000.0);
								break;
							}
							default:
								num = (tispaceObjectState.isEarth ? ((defaultScalingForEarthOverride > 0f) ? defaultScalingForEarthOverride : 1.7f) : 3.25f);
								break;
							}
							if (tispaceObjectState.controller != null && !double.IsNaN(tispaceObjectState.controller.SpaceObject.Position.magnitude))
							{
								GameObject gameObjectLink = tispaceObjectState.gameObjectLink;
								SpaceObjectSelection.SelectSpaceObject(gameObjectLink, false, false, false);
								SpaceObjectComponent component = gameObjectLink.GetComponent<SpaceObjectComponent>();
								if (TIUtilities.GameStateHasLatLong(gameState))
								{
									Vector2 vector = TIUtilities.GameStateLatLong(gameState);
									if (tispaceObjectState.isEarth && gameState.hasEarthMapObject)
									{
										if (GameControl.control.viewMgr.currentView != ViewType.PoliticalMap)
										{
											GameControl.control.viewMgr.GotoView(ViewType.PoliticalMap);
										}
										TIRegionState ref_region = gameState.ref_region;
										if (ref_region != null && ref_region.mapRegionTemplate.smallRegion)
										{
											num = ((defaultScalingForEarthOverride > 0f) ? defaultScalingForEarthOverride : 1.35f);
										}
										if (gameState.isArmyState && gameState.ref_army.IsMoving && gameState.ref_army.SeaTransitStage() == ArmySeaTransitStage.Sea_DestinationRegion)
										{
											vector = TIUtilities.GameStateLatLong(gameState.ref_army.currentOperations[0].target.ref_region);
										}
									}
									if (zoomCamera)
									{
										TIUtilities.camera.Zoom(tispaceObjectState.meanRadius_m * (double)num, true);
									}
									TIUtilities.camera.RotateToLatitudeLongitude((double)vector.x, (double)vector.y);
									return;
								}
								if (gameState.isOrbitState)
								{
									TIUtilities.camera.Zoom(gameState.ref_orbit.semiMajorAxis_m * 3.0, true);
								}
								else if (tispaceObjectState.ref_spaceBody == tispaceObjectState || tispaceObjectState.ref_hab == tispaceObjectState || tispaceObjectState.ref_fleet == tispaceObjectState)
								{
									if (zoomCamera)
									{
										TIUtilities.camera.Zoom(tispaceObjectState.meanRadius_m * (double)num, true);
									}
									else if (component != null && TIUtilities.camera.Spherical.radius < component.Value.MeanRadius * (double)num)
									{
										TIUtilities.camera.Zoom(component.Value.MeanRadius * (double)num, true);
									}
								}
								else if (tispaceObjectState.isLagrangePointState && zoomCamera)
								{
									TIUtilities.camera.Zoom((double)num, true);
								}
								if (zoomCamera && (tispaceObjectState.isSpaceAssetState || tispaceObjectState.isSpaceShipState))
								{
									GameTimeManager.Singleton.ResetSpeed(false);
									return;
								}
							}
						}
					}
				}
				else
				{
					if (gameState.isFactionState)
					{
						TIUtilities.GotoGameState(gameState.ref_faction, false);
						return;
					}
					if (gameState is TIGlobalResearchState)
					{
						World.Active.GetExistingManager<CanvasManager>().ShowInfoScreen<ResearchScreenController>();
						return;
					}
					if (gameState is TIMissionPhaseState)
					{
						World.Active.GetExistingManager<CanvasManager>().ShowInfoScreen<ObjectivesScreenController>();
						return;
					}
					TIGlobalValuesState tiglobalValuesState = gameState as TIGlobalValuesState;
					if (tiglobalValuesState != null)
					{
						TIUtilities.GotoGameState(tiglobalValuesState, false);
					}
				}
			}
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x000ED72C File Offset: 0x000EB92C
		public static void TriggerSelectionEvent(TIGameState gameState)
		{
			if (gameState.isSpaceFleetState)
			{
				GameControl.eventManager.TriggerEvent(new FleetSelectedEvent(gameState.ref_fleet), null, Array.Empty<object>());
				return;
			}
			if (gameState.isHabState)
			{
				GameControl.eventManager.TriggerEvent(new HabSelectedEvent(gameState.ref_hab), null, Array.Empty<object>());
				return;
			}
			if (gameState.isSpaceBodyState)
			{
				GameControl.eventManager.TriggerEvent(new SpaceBodySelectedEvent(gameState.ref_spaceBody), null, Array.Empty<object>());
				return;
			}
			if (gameState.isLagrangePointState)
			{
				GameControl.eventManager.TriggerEvent(new LagrangePointSelectedEvent(gameState.ref_lagrangePoint), null, Array.Empty<object>());
				return;
			}
			if (gameState.isHabSiteState)
			{
				GameControl.eventManager.TriggerEvent(new HabSiteSelectedEvent(gameState.ref_habSite), null, Array.Empty<object>());
				return;
			}
			if (gameState.isSpaceShipState)
			{
				GameControl.eventManager.TriggerEvent(new ShipSelectedEvent(gameState.ref_ship), null, Array.Empty<object>());
				return;
			}
			if (gameState.isArmyState)
			{
				GameControl.eventManager.TriggerEvent(new ArmyMapItemSelected(gameState.ref_army), null, Array.Empty<object>());
				return;
			}
			if (gameState.isNationState)
			{
				TINationState ref_nation = gameState.ref_nation;
				GameControl.eventManager.TriggerEvent(new RegionStateSelected(ref_nation.capital), null, new object[] { ref_nation.capital });
				GameControl.eventManager.TriggerEvent(new NationStateSelected(ref_nation), null, new object[] { ref_nation });
				return;
			}
			if (gameState.isCouncilorState)
			{
				TICouncilorState ref_councilor = gameState.ref_councilor;
				if (GeneralControlsController.UIPlayerInTargetingMode)
				{
					GameControl.eventManager.TriggerEvent(new CouncilorMapItemSelected(ref_councilor), null, CouncilorMapItemSelected.MakeSourceObjects(ref_councilor));
					return;
				}
				GameControl.eventManager.TriggerEvent(new CouncilorSelectedOffMap(ref_councilor), null, new object[] { ref_councilor.ref_region });
				return;
			}
			else
			{
				if (gameState.isRegionState)
				{
					GameControl.eventManager.TriggerEvent(new RegionStateSelected(gameState.ref_region), null, new object[] { gameState.ref_region });
					GameControl.eventManager.TriggerEvent(new NationStateSelected(gameState.ref_region.nation), null, new object[] { gameState.ref_region.nation });
					return;
				}
				if (gameState.isRegionSpaceFacility)
				{
					GameControl.eventManager.TriggerEvent(new SpaceFacilityMapObjectSelected(gameState.ref_regionSpaceFacility), null, new object[] { gameState.ref_regionSpaceFacility });
					return;
				}
				if (gameState.isRegionAlienEntity)
				{
					GameControl.eventManager.TriggerEvent(new AlienRegionMapEntitySelected(gameState.ref_regionAlienEntity), null, new object[] { gameState.ref_regionAlienEntity });
					if (gameState.isRegionAlienAsset)
					{
						GameControl.eventManager.TriggerEvent(new AlienAssetTargetSelected(gameState.ref_regionAlienAsset), null, new object[] { gameState.ref_regionAlienEntity });
					}
					return;
				}
				if (gameState.isControlPointState)
				{
					GameControl.eventManager.TriggerEvent(new ControlPointTargetSelected(gameState.ref_controlPoint), null, new object[] { gameState.ref_region, gameState.ref_nation });
					return;
				}
				if (gameState.ref_region != null)
				{
					GameControl.eventManager.TriggerEvent(new RegionStateSelected(gameState.ref_region), null, new object[] { gameState.ref_region });
					GameControl.eventManager.TriggerEvent(new NationStateSelected(gameState.ref_region.nation), null, new object[] { gameState.ref_region.nation });
					return;
				}
				return;
			}
		}

		// Token: 0x06002B8B RID: 11147 RVA: 0x000EDA54 File Offset: 0x000EBC54
		public static void LookAtGameState(TIGameState gameState)
		{
			if (gameState.isCouncilorState)
			{
				gameState = TIMissionPhaseState.CouncilorLastKnownLocation(GameControl.control.activePlayer, gameState.ref_councilor);
			}
			TIUtilities.GotoGameState(gameState, true, false, false, false, false, -1f);
		}

		// Token: 0x06002B8C RID: 11148 RVA: 0x000EDA88 File Offset: 0x000EBC88
		public static void GotoSelectedStateUI(TIGameState gameState, bool setAsGlobalSelectedState)
		{
			if (TIGameState.Valid(gameState) && gameState.hasMapObject)
			{
				if (gameState.isSpaceGameState)
				{
					TISpaceObjectState tispaceObjectState = TIUtilities.ObjectToSpaceObject(gameState, true);
					if (TIInputManager.DoubleClickedGameState(tispaceObjectState, false))
					{
						SpaceObjectSelection.SelectSpaceObject(tispaceObjectState.gameObjectLink, setAsGlobalSelectedState, false, false);
					}
					if (gameState.isHabState && gameState.ref_hab.IsBase)
					{
						GeneralControlsController.SetSelectedState(gameState, true);
						(World.Active.GetExistingManager<CanvasManager>().SpaceObjectDetail as SpaceObjectDetailController).ViewSpaceObject(gameState.ref_hab, true);
						return;
					}
					(World.Active.GetExistingManager<CanvasManager>().SpaceObjectDetail as SpaceObjectDetailController).ViewSpaceObject(tispaceObjectState.gameObjectLink);
					return;
				}
				else
				{
					if (gameState.isArmyState || gameState.isRegionSpaceFacility || gameState.isCouncilorState || gameState.isRegionAlienEntity || gameState.isSpaceShipState || gameState.isRegionState || gameState.isNationState)
					{
						GeneralControlsController.SetSelectedState(gameState, true);
						return;
					}
					if (gameState.ref_region != null)
					{
						GeneralControlsController.SetSelectedState(gameState.ref_region, true);
					}
				}
			}
		}

		// Token: 0x06002B8D RID: 11149 RVA: 0x000EDB8C File Offset: 0x000EBD8C
		public static float Median<T>(this IEnumerable<T> elements, Func<T, float> GetValue)
		{
			List<float> list = (from x in elements
				select GetValue(x) into x
				orderby x
				select x).ToList<float>();
			if (list.Count == 0)
			{
				throw new ArgumentException();
			}
			if (list.Count % 2 == 0)
			{
				return list[list.Count / 2 - 1] / 2f + list[list.Count / 2] / 2f;
			}
			return list[list.Count / 2];
		}

		// Token: 0x06002B8E RID: 11150 RVA: 0x000EDC34 File Offset: 0x000EBE34
		public static IEnumerable<U> SelectSansNulls<T, U>(this IEnumerable<T> elements, Func<T, U> Selector)
		{
			return from x in elements.Select<T, U>(Selector)
				where x != null
				select x;
		}

		// Token: 0x06002B8F RID: 11151 RVA: 0x000EDC61 File Offset: 0x000EBE61
		public static string FormatBigOrSmallNumber(float value, int bigCap = 1, int smallCap = 7, int smallExtend = 0, bool useSmallPrefixes = false, bool emptyZero = false)
		{
			if (Mathf.Abs(value) >= 1000f)
			{
				return TIUtilities.FormatBigNumber((double)value, bigCap, emptyZero);
			}
			return TIUtilities.FormatSmallNumber(value, smallCap, smallExtend, !useSmallPrefixes, emptyZero);
		}

		// Token: 0x06002B90 RID: 11152 RVA: 0x000EDC8A File Offset: 0x000EBE8A
		public static string FormatBigOrSmallNumber(double value, int bigCap = 1, int smallCap = 7, int smallExtend = 0, bool useSmallPrefixes = false, bool emptyZero = false)
		{
			if (Mathd.Abs(value) >= 1000.0)
			{
				return TIUtilities.FormatBigNumber(value, bigCap, emptyZero);
			}
			return TIUtilities.FormatSmallNumber(value, smallCap, smallExtend, !useSmallPrefixes, emptyZero);
		}

		// Token: 0x06002B91 RID: 11153 RVA: 0x000EDCB8 File Offset: 0x000EBEB8
		public static string FormatBigNumber(double value, int cap = 1, bool emptyZero = false)
		{
			if (value == 0.0 && emptyZero)
			{
				return "-";
			}
			double num = Mathd.Abs(value);
			string text = new StringBuilder("N").Append(cap.ToString()).ToString();
			if (num < 1000.0)
			{
				value = Utilities.VariableTruncate(value, cap);
				return value.ToString(text);
			}
			if (num < 1000000.0)
			{
				return Loc.T("UI.Global.Thousands", new object[] { (value / 1000.0).ToString(text) });
			}
			if (num < 1000000000.0)
			{
				return Loc.T("UI.Global.Millions", new object[] { (value / 1000000.0).ToString(text) });
			}
			if (num >= 1000000000000.0)
			{
				return Loc.T("UI.Global.Trillions", new object[] { (value / 999999995904.0).ToString(text) });
			}
			if (num > 1000000000000000.0)
			{
				return Loc.T("UI.Global.Quadrillions", new object[] { (value / 999999986991104.0).ToString(text) });
			}
			return Loc.T("UI.Global.Billions", new object[] { (value / 1000000000.0).ToString(text) });
		}

		// Token: 0x06002B92 RID: 11154 RVA: 0x000EDE1C File Offset: 0x000EC01C
		public static string FormatSmallNumber(float value, int decimalCap = 7, int extend = 0, bool avoidPrefix = true, bool emptyZero = false)
		{
			if (value == 0f && emptyZero)
			{
				return "-";
			}
			if (!avoidPrefix && !Mathd.Approximately((double)value, 0.0) && Mathd.Abs((double)value) < 0.001)
			{
				return TIUtilities.FormatSmallNumber_prefix((double)value, decimalCap, extend, false);
			}
			value = Utilities.VariableTruncate(value, decimalCap + extend);
			string text = value.ToString(TIUtilities.DecimalPlaces((double)value, decimalCap, extend));
			if (text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
			{
				text = text.TrimEnd(new char[] { '0' });
				text = text.TrimEnd(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToCharArray());
			}
			return text;
		}

		// Token: 0x06002B93 RID: 11155 RVA: 0x000EDEC8 File Offset: 0x000EC0C8
		public static string FormatSmallNumber(double value, int decimalCap = 7, int extend = 0, bool avoidPrefix = true, bool emptyZero = false)
		{
			if (value == 0.0 && emptyZero)
			{
				return "-";
			}
			if (!avoidPrefix && !Mathd.Approximately(value, 0.0) && Mathd.Abs(value) < 0.001)
			{
				return TIUtilities.FormatSmallNumber_prefix(value, decimalCap, extend, false);
			}
			value = Utilities.VariableTruncate(value, decimalCap + extend);
			string text = value.ToString(TIUtilities.DecimalPlaces(value, decimalCap, extend));
			if (text.Contains(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator))
			{
				text = text.TrimEnd(new char[] { '0' });
				text = text.TrimEnd(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToCharArray());
			}
			return text;
		}

		// Token: 0x06002B94 RID: 11156 RVA: 0x000EDF74 File Offset: 0x000EC174
		public static string FormatSmallNumber_prefix(double value, int cap = 4, int extend = 0, bool includingMilli = false)
		{
			double num = Mathd.Abs(value);
			string text = new StringBuilder("N").Append(extend.ToString()).ToString();
			if (num < 1.0)
			{
				if (num < 0.001)
				{
					if (num < 1E-06)
					{
						if (num < 1E-09)
						{
							if (num < 1E-12)
							{
								if (num < 1E-15)
								{
									if (num < 1E-18)
									{
										if (num < 1E-21)
										{
											if (value < 0.0)
											{
												return Loc.T("UI.Global.nyocto", new object[] { (num * 1E+24).ToString(new StringBuilder("N").Append(cap.ToString()).ToString()) });
											}
											return Loc.T("UI.Global.yocto", new object[] { (value * 1E+24).ToString(new StringBuilder("N").Append(cap.ToString()).ToString()) });
										}
										else
										{
											if (value < 0.0)
											{
												return Loc.T("UI.Global.nzepto", new object[] { (num * 1E+21).ToString(text) });
											}
											return Loc.T("UI.Global.zepto", new object[] { (value * 1E+21).ToString(text) });
										}
									}
									else
									{
										if (value < 0.0)
										{
											return Loc.T("UI.Global.natto", new object[] { (num * 1E+18).ToString(text) });
										}
										return Loc.T("UI.Global.atto", new object[] { (value * 1E+18).ToString(text) });
									}
								}
								else
								{
									if (value < 0.0)
									{
										return Loc.T("UI.Global.nfemto", new object[] { (num * 1000000000000000.0).ToString(text) });
									}
									return Loc.T("UI.Global.femto", new object[] { (value * 1000000000000000.0).ToString(text) });
								}
							}
							else
							{
								if (value < 0.0)
								{
									return Loc.T("UI.Global.npico", new object[] { (num * 1000000000000.0).ToString(text) });
								}
								return Loc.T("UI.Global.pico", new object[] { (value * 1000000000000.0).ToString(text) });
							}
						}
						else
						{
							if (value < 0.0)
							{
								return Loc.T("UI.Global.nnano", new object[] { (num * 1000000000.0).ToString(text) });
							}
							return Loc.T("UI.Global.nano", new object[] { (value * 1000000000.0).ToString(text) });
						}
					}
					else
					{
						if (value < 0.0)
						{
							return Loc.T("UI.Global.nmicro", new object[] { (num * 1000000.0).ToString(text) });
						}
						return Loc.T("UI.Global.micro", new object[] { (value * 1000000.0).ToString(text) });
					}
				}
				else if (includingMilli)
				{
					if (value < 0.0)
					{
						return Loc.T("UI.Global.nmilli", new object[] { (num * 1000.0).ToString(text) });
					}
					return Loc.T("UI.Global.milli", new object[] { (value * 1000.0).ToString(text) });
				}
			}
			return value.ToString(text).TrimEnd(new char[] { '0' }).TrimEnd(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.ToCharArray());
		}

		// Token: 0x06002B95 RID: 11157 RVA: 0x000EE360 File Offset: 0x000EC560
		public static string LocalizeGW(string keyGW, float GW)
		{
			float num = Mathf.Abs(GW);
			string text;
			string text2;
			if (num >= 1000f)
			{
				text = keyGW.Replace("GW", "TW");
				text2 = (GW / 1000f).ToString(TIUtilities.DecimalPlaces((double)(GW / 1000f), 7, 0));
			}
			else if (num >= 1f)
			{
				text = keyGW;
				text2 = GW.ToString(TIUtilities.DecimalPlaces((double)GW, 7, 0));
			}
			else
			{
				text = keyGW.Replace("GW", "MW");
				text2 = (GW * 1000f).ToString(TIUtilities.DecimalPlaces((double)(GW * 1000f), 7, 0));
			}
			return Loc.T(text, new object[] { text2 });
		}

		// Token: 0x06002B96 RID: 11158 RVA: 0x000EE40C File Offset: 0x000EC60C
		public static string DecimalPlaces(double value, int cap = 7, int forceExtend = 0)
		{
			double num = Mathd.Abs(value);
			double num2 = Mathd.Abs(Math.Truncate(value));
			int num3;
			if (num == 0.0 || num == num2 || num >= 1000.0)
			{
				num3 = 0;
			}
			else if (num >= 1.0)
			{
				num3 = 1;
			}
			else if (num > 0.10000000149011612)
			{
				num3 = 2;
			}
			else if (num > 0.009999999776482582)
			{
				num3 = 3;
			}
			else if (num > 0.0010000000474974513)
			{
				num3 = 4;
			}
			else if (num > 9.999999747378752E-05)
			{
				num3 = 5;
			}
			else if (num > 9.999999747378752E-06)
			{
				num3 = 6;
			}
			else
			{
				num3 = 7;
			}
			if (forceExtend > 0 && num3 > 0)
			{
				num3 += forceExtend;
			}
			if (cap < num3)
			{
				num3 = cap;
			}
			return new StringBuilder("N").Append(num3.ToString()).ToString();
		}

		// Token: 0x06002B97 RID: 11159 RVA: 0x000EE4DC File Offset: 0x000EC6DC
		public static string DecimalPlaces_P(double value, int cap = 7, int forceExtend = 0)
		{
			double num = Mathd.Abs(value) * 100.0;
			double num2 = Mathd.Abs(Math.Truncate(value));
			int num3;
			if (num == 0.0 || num == num2 || num >= 1000.0)
			{
				num3 = 0;
			}
			else if (num >= 1.0)
			{
				num3 = 1;
			}
			else if (num > 0.10000000149011612)
			{
				num3 = 2;
			}
			else if (num > 0.009999999776482582)
			{
				num3 = 3;
			}
			else if (num > 0.0010000000474974513)
			{
				num3 = 4;
			}
			else if (num > 9.999999747378752E-05)
			{
				num3 = 5;
			}
			else if (num > 9.999999747378752E-06)
			{
				num3 = 6;
			}
			else
			{
				num3 = 7;
			}
			if (forceExtend > 0 && num3 > 0)
			{
				num3 += forceExtend;
			}
			if (cap < num3)
			{
				num3 = cap;
			}
			return new StringBuilder("P").Append(num3.ToString()).Replace(" ", "").ToString();
		}

		// Token: 0x06002B98 RID: 11160 RVA: 0x000EE5C4 File Offset: 0x000EC7C4
		public static string ForceValueSign(float value, string stringToModify, bool dollars = false, bool colorize = false, NationInfoController.WhatIsGood whatIsGood = NationInfoController.WhatIsGood.upIsGood)
		{
			string text;
			if (!dollars)
			{
				if (value <= 0f)
				{
					text = stringToModify;
				}
				else
				{
					text = Loc.T("UI.Global.PositiveValueWithSign", new object[] { stringToModify });
				}
			}
			else if (value < 0f)
			{
				text = Loc.T("UI.Global.NegativeDollarValueWithSign", new object[] { stringToModify.Replace("-", string.Empty).Replace(NumberFormatInfo.CurrentInfo.NegativeSign, string.Empty) });
			}
			else if (value == 0f)
			{
				text = Loc.T("UI.Global.DollarValue", new object[] { value.ToString("N0") });
			}
			else
			{
				text = Loc.T("UI.Global.PositiveDollarValueWithSign", new object[] { stringToModify });
			}
			if (colorize)
			{
				if (whatIsGood == NationInfoController.WhatIsGood.downIsGood)
				{
					if (value < 0f)
					{
						text = TIUtilities.GreenLine(text);
					}
					else if (value > 0f)
					{
						text = TIUtilities.RedLine(text);
					}
				}
				else if (value > 0f)
				{
					text = TIUtilities.GreenLine(text);
				}
				else if (value < 0f)
				{
					text = TIUtilities.RedLine(text);
				}
			}
			return text;
		}

		// Token: 0x06002B99 RID: 11161 RVA: 0x000EE6CC File Offset: 0x000EC8CC
		public static string ForceValueSign(float value, bool dollars = false, bool percent = false, string decimalOverride = "")
		{
			if (!dollars)
			{
				if (percent)
				{
					if (value <= 0f)
					{
						return value.ToPercent((decimalOverride != string.Empty) ? decimalOverride : TIUtilities.DecimalPlaces_P((double)value, 7, 0));
					}
					return Loc.T("UI.Global.PositiveValueWithSign", new object[] { value.ToPercent((decimalOverride != string.Empty) ? decimalOverride : TIUtilities.DecimalPlaces_P((double)value, 7, 0)) });
				}
				else
				{
					if (value <= 0f)
					{
						return TIUtilities.FormatBigOrSmallNumber(value, 1, 7, 0, false, false);
					}
					return Loc.T("UI.Global.PositiveValueWithSign", new object[] { TIUtilities.FormatBigOrSmallNumber(value, 1, 7, 0, false, false) });
				}
			}
			else
			{
				if (value < 0f)
				{
					return Loc.T("UI.Global.NegativeDollarValueWithSign", new object[] { TIUtilities.FormatBigOrSmallNumber(Math.Abs(value), 1, 7, 1, false, false) });
				}
				if (value == 0f)
				{
					return Loc.T("UI.Global.DollarValue", new object[] { value.ToString("N0") });
				}
				return Loc.T("UI.Global.PositiveDollarValueWithSign", new object[] { TIUtilities.FormatBigOrSmallNumber(value, 1, 7, 1, false, false) });
			}
		}

		// Token: 0x06002B9A RID: 11162 RVA: 0x000EE7E4 File Offset: 0x000EC9E4
		public static string GetStateDisplayName(TIGameState target, TIFactionState targetingFaction = null, bool sentenceForm = false, bool capitalize = false, bool includeArticle = false, bool colorFactionName = false, bool councilorFromMemory = true)
		{
			if (target.isRegionState)
			{
				return TIUtilities.GetLocationString(target.ref_region, false, sentenceForm);
			}
			if (target.isCouncilorState)
			{
				if (!(targetingFaction != null))
				{
					return target.ref_councilor.displayName;
				}
				if (sentenceForm)
				{
					if (!councilorFromMemory)
					{
						return targetingFaction.GetViewofCouncilor(target.ref_councilor).displayNameCurrentSentence;
					}
					return targetingFaction.GetViewofCouncilor(target.ref_councilor).displayNameMemorySentence;
				}
				else
				{
					if (!councilorFromMemory)
					{
						return targetingFaction.GetViewofCouncilor(target.ref_councilor).displayNameCurrent;
					}
					return targetingFaction.GetViewofCouncilor(target.ref_councilor).displayNameMemory;
				}
			}
			else if (target.isNationState)
			{
				if (sentenceForm)
				{
					return TIUtilities.GetLocationString(target.ref_nation, false, true);
				}
				if (!includeArticle)
				{
					return target.ref_nation.displayName;
				}
				if (capitalize)
				{
					return target.ref_nation.displayNameWithArticleCapitalized;
				}
				return target.ref_nation.displayNameWithArticle;
			}
			else if (target.isFactionState)
			{
				if (!capitalize)
				{
					if (!colorFactionName)
					{
						return target.ref_faction.displayName;
					}
					return target.ref_faction.displayNameWithColor;
				}
				else
				{
					if (!colorFactionName)
					{
						return target.ref_faction.displayNameCapitalized;
					}
					return target.ref_faction.displayNameCapitalizedWithColor;
				}
			}
			else if (target.isArmyState)
			{
				if (!includeArticle && !sentenceForm)
				{
					return target.ref_army.displayName;
				}
				if (!capitalize)
				{
					return target.ref_army.displayNameWithArticle;
				}
				return target.ref_army.displayNameWithArticleCapitalized;
			}
			else
			{
				if (!target.isSpaceAssetState && !target.isSpaceShipState)
				{
					bool isControlPointState = target.isControlPointState;
					return target.displayName;
				}
				if (sentenceForm)
				{
					return TIUtilities.GetLocationString(target, false, true);
				}
				return target.GetDisplayName(targetingFaction);
			}
		}

		// Token: 0x06002B9B RID: 11163 RVA: 0x000EE970 File Offset: 0x000ECB70
		public static string GetLocationString(TIGameState location, bool expandedLocationString, bool sentenceForm)
		{
			if (location == null)
			{
				Log.Error("Null Location Passed to GetLocationString", Array.Empty<object>());
				return string.Empty;
			}
			TIFactionState activePlayer = GameControl.control.activePlayer;
			if (location.isRegionState)
			{
				TIRegionState ref_region = location.ref_region;
				if (expandedLocationString)
				{
					if (!sentenceForm)
					{
						return Loc.T("UI.Notifications.TwoPointLocation", new object[]
						{
							ref_region.displayName,
							ref_region.nation.displayName
						});
					}
					return Loc.T("UI.Notifications.ExpandedRegionWithPrep", new object[]
					{
						ref_region.displayName,
						ref_region.nation.displayNameWithArticle
					});
				}
				else
				{
					if (!sentenceForm)
					{
						return ref_region.displayName;
					}
					return Loc.T("UI.Notifications.RegionWithPrep", new object[] { ref_region.displayName });
				}
			}
			else if (location.isNationState)
			{
				if (!sentenceForm)
				{
					return location.ref_nation.displayNameWithArticleCapitalized;
				}
				return location.ref_nation.displayNameWithArticleAndPlacePrep;
			}
			else
			{
				if (location.ref_region != null)
				{
					return TIUtilities.GetLocationString(location.ref_region, expandedLocationString, sentenceForm);
				}
				if (location.isHabSiteState)
				{
					TIHabSiteState ref_habSite = location.ref_habSite;
					bool flag = ref_habSite.parentBody.displayName.Contains(ref_habSite.displayName);
					if (expandedLocationString && !flag)
					{
						if (!sentenceForm)
						{
							return Loc.T("UI.Notifications.TwoPointLocation", new object[]
							{
								ref_habSite.displayName,
								ref_habSite.parentBody.displayName
							});
						}
						return Loc.T("UI.Notifications.ExpandedHabSiteWithPrep", new object[]
						{
							ref_habSite.displayName,
							ref_habSite.parentBody.displayName
						});
					}
					else
					{
						if (!sentenceForm)
						{
							return Loc.T(ref_habSite.displayName);
						}
						return Loc.T("UI.Notifications.HabSiteWithPrep", new object[] { flag ? Loc.T("UI.Notifications.UnnamedHabSite", new object[] { ref_habSite.displayName }) : ref_habSite.displayName });
					}
				}
				else if (location.isHabState || location.isHabModuleState)
				{
					TIHabState ref_hab = location.ref_hab;
					if (expandedLocationString)
					{
						if (ref_hab.IsStation)
						{
							if (!sentenceForm)
							{
								return Loc.T("UI.Notifications.TwoPointLocation", new object[]
								{
									ref_hab.GetDisplayName(activePlayer),
									ref_hab.ref_orbit.displayName
								});
							}
							return Loc.T("UI.Notifications.ExpandedStationLocationWithPrep", new object[]
							{
								ref_hab.GetDisplayName(activePlayer),
								ref_hab.ref_orbit.displayName
							});
						}
						else
						{
							TIHabSiteState ref_habSite2 = ref_hab.ref_habSite;
							bool flag2 = ref_habSite2.parentBody.displayName.Contains(ref_habSite2.displayName);
							string text = ref_habSite2.displayName;
							if (flag2)
							{
								text = Loc.T("UI.Notifications.UnnamedHabSite", new object[] { text });
							}
							if (!sentenceForm)
							{
								if (!flag2)
								{
									return Loc.T("UI.Notifications.ThreePointLocation", new object[]
									{
										ref_hab.GetDisplayName(activePlayer),
										ref_habSite2.displayName,
										ref_habSite2.parentBody.displayName
									});
								}
								return Loc.T("UI.Notifications.TwoPointLocation", new object[]
								{
									ref_hab.GetDisplayName(activePlayer),
									ref_habSite2.displayName
								});
							}
							else
							{
								if (!flag2)
								{
									return Loc.T("UI.Notifications.FullExpandedBaseLocationWithPrep", new object[]
									{
										ref_hab.GetDisplayName(activePlayer),
										ref_habSite2.displayName,
										ref_habSite2.parentBody.displayName
									});
								}
								return Loc.T("UI.Notifications.ExpandedBaseLocationWithPrep", new object[]
								{
									ref_hab.GetDisplayName(activePlayer),
									ref_habSite2.displayName
								});
							}
						}
					}
					else if (ref_hab.IsStation)
					{
						if (!sentenceForm)
						{
							return ref_hab.GetDisplayName(activePlayer);
						}
						return Loc.T("UI.Notifications.StationLocationWithPrep", new object[] { ref_hab.GetDisplayName(activePlayer) });
					}
					else
					{
						if (!sentenceForm)
						{
							return ref_hab.GetDisplayName(activePlayer);
						}
						return Loc.T("UI.Notifications.BaseLocationWithPrep", new object[] { ref_hab.GetDisplayName(activePlayer) });
					}
				}
				else if (location.isOrbitState)
				{
					TIOrbitState ref_orbit = location.ref_orbit;
					if (!sentenceForm)
					{
						return ref_orbit.displayName;
					}
					return Loc.T("UI.Notifications.OrbitWithPrep", new object[] { ref_orbit.displayName });
				}
				else if (location.isSpaceShipState)
				{
					TISpaceShipState ref_ship = location.ref_ship;
					if (expandedLocationString)
					{
						if (!sentenceForm)
						{
							return Loc.T("UI.Notifications.TwoPointLocation", new object[]
							{
								ref_ship.GetDisplayName(activePlayer),
								ref_ship.fleet.GetDisplayName(activePlayer)
							});
						}
						return Loc.T("UI.Notifications.ExpandedShipWithPrep", new object[]
						{
							ref_ship.GetDisplayName(activePlayer),
							ref_ship.fleet.GetDisplayName(activePlayer)
						});
					}
					else
					{
						if (!sentenceForm)
						{
							return ref_ship.GetDisplayName(activePlayer);
						}
						return Loc.T("UI.Notifications.ShipWithPrep", new object[] { ref_ship.GetDisplayName(activePlayer) });
					}
				}
				else if (location.isLagrangePointState)
				{
					if (!sentenceForm)
					{
						return location.ref_lagrangePoint.displayName;
					}
					return Loc.T("UI.Notifications.LPointWithPrep", new object[] { location.ref_lagrangePoint.displayName });
				}
				else
				{
					if (!location.isSpaceBodyState)
					{
						return location.GetDisplayName(activePlayer);
					}
					if (!sentenceForm)
					{
						return location.ref_spaceBody.displayName;
					}
					return Loc.T("UI.Notifications.PlanetWithPrep", new object[] { location.ref_spaceBody.displayName });
				}
			}
		}

		// Token: 0x170005F3 RID: 1523
		// (get) Token: 0x06002B9C RID: 11164 RVA: 0x000EEE60 File Offset: 0x000ED060
		public static string separator
		{
			get
			{
				return new StringBuilder("<align=\"center\">").Append(Loc.T("UI.Global.Div")).Append("</align>").ToString();
			}
		}

		// Token: 0x06002B9D RID: 11165 RVA: 0x000EEE8C File Offset: 0x000ED08C
		public static string StripDiacriticsFromString(string inputString)
		{
			string text = inputString.Normalize(NormalizationForm.FormD);
			StringBuilder stringBuilder = new StringBuilder(text.Length);
			foreach (char c in text)
			{
				if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		// Token: 0x06002B9E RID: 11166 RVA: 0x000EEEE3 File Offset: 0x000ED0E3
		public static string StripInvalidPathCharsFromString(string filename)
		{
			return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
		}

		// Token: 0x06002B9F RID: 11167 RVA: 0x000EEEF8 File Offset: 0x000ED0F8
		public static string CombineStrings(params string[] strings)
		{
			TIUtilities.sb.Clear();
			foreach (string text in strings)
			{
				TIUtilities.sb.Append(text);
			}
			return TIUtilities.sb.ToString();
		}

		// Token: 0x06002BA0 RID: 11168 RVA: 0x000EEF3A File Offset: 0x000ED13A
		public static string LocalizedNamelistIDX(string idx)
		{
			return Loc.T(TIUtilities.CombineStrings(new string[] { "UI.StartScreen.CustomizeCampaign.Namelist.", idx }));
		}

		// Token: 0x06002BA1 RID: 11169 RVA: 0x000EEF58 File Offset: 0x000ED158
		public static string ConstructTextList(List<TIGameState> gameStates, bool noConjuction = false, bool orConjunction = false)
		{
			StringBuilder stringBuilder = new StringBuilder(TIUtilities.GetStateDisplayName(gameStates[0], null, false, false, true, false, true));
			for (int i = 1; i < gameStates.Count; i++)
			{
				if (!noConjuction && i == gameStates.Count - 1)
				{
					if (orConjunction)
					{
						if (i == 1)
						{
							stringBuilder.Append(Loc.T("UI.Global.ListConjuctionOr"));
						}
						else
						{
							stringBuilder.Append(Loc.T("UI.Global.ListSerialConjunctionOr"));
						}
					}
					else if (i == 1)
					{
						stringBuilder.Append(Loc.T("UI.Global.ListConjunction"));
					}
					else
					{
						stringBuilder.Append(Loc.T("UI.Global.ListSerialConjunction"));
					}
				}
				else
				{
					stringBuilder.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
				}
				stringBuilder.Append(TIUtilities.GetStateDisplayName(gameStates[i], null, false, false, true, false, true));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002BA2 RID: 11170 RVA: 0x000EF02C File Offset: 0x000ED22C
		public static string ConstructTextList(List<TIDataTemplate> templates, bool noConjuction = false, bool orConjunction = false)
		{
			if (templates.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(templates[0].displayName);
			for (int i = 1; i < templates.Count; i++)
			{
				if (!noConjuction && i == templates.Count - 1)
				{
					if (orConjunction)
					{
						if (i == 1)
						{
							stringBuilder.Append(Loc.T("UI.Global.ListConjuctionOr"));
						}
						else
						{
							stringBuilder.Append(Loc.T("UI.Global.ListSerialConjunctionOr"));
						}
					}
					else if (i == 1)
					{
						stringBuilder.Append(Loc.T("UI.Global.ListConjunction"));
					}
					else
					{
						stringBuilder.Append(Loc.T("UI.Global.ListSerialConjunction"));
					}
				}
				else
				{
					stringBuilder.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
				}
				stringBuilder.Append(templates[i].displayName);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002BA3 RID: 11171 RVA: 0x000EF100 File Offset: 0x000ED300
		public static string ConstructTextList(List<string> strings, bool noConjuction = false, bool orConjunction = false)
		{
			StringBuilder stringBuilder = new StringBuilder(strings[0]);
			for (int i = 1; i < strings.Count; i++)
			{
				if (!noConjuction && i == strings.Count - 1)
				{
					if (orConjunction)
					{
						if (i == 1)
						{
							stringBuilder.Append(Loc.T("UI.Global.ListConjuctionOr"));
						}
						else
						{
							stringBuilder.Append(Loc.T("UI.Global.ListSerialConjunctionOr"));
						}
					}
					else if (i == 1)
					{
						stringBuilder.Append(Loc.T("UI.Global.ListConjunction"));
					}
					else
					{
						stringBuilder.Append(Loc.T("UI.Global.ListSerialConjunction"));
					}
				}
				else
				{
					stringBuilder.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
				}
				stringBuilder.Append(strings[i]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002BA4 RID: 11172 RVA: 0x000EF1BC File Offset: 0x000ED3BC
		public static string GetPriorityString(PriorityType priority, bool icon = false)
		{
			if (!icon)
			{
				return Loc.T(new StringBuilder("UI.Nation.Priority_").Append(priority.ToString()).ToString());
			}
			return new StringBuilder(TINationState.GetInlinePriorityIcon(priority)).Append(Loc.T(new StringBuilder("UI.Nation.Priority_").Append(priority.ToString()).ToString())).ToString();
		}

		// Token: 0x06002BA5 RID: 11173 RVA: 0x000EF230 File Offset: 0x000ED430
		public static string GetColorString(Color color)
		{
			return new StringBuilder("<color=#").Append(((int)(color.r * 255f)).ToString("X2")).Append(((int)(color.g * 255f)).ToString("X2")).Append(((int)(color.b * 255f)).ToString("X2"))
				.Append("FF>")
				.ToString();
		}

		// Token: 0x06002BA6 RID: 11174 RVA: 0x000EF2B3 File Offset: 0x000ED4B3
		public static string GetResourceString(FactionResource resource)
		{
			return Loc.T(new StringBuilder("UI.Global.").Append(resource.ToString()).ToString());
		}

		// Token: 0x06002BA7 RID: 11175 RVA: 0x000EF2DB File Offset: 0x000ED4DB
		public static string GetAttributeString(CouncilorAttribute attribute)
		{
			return Loc.T(new StringBuilder("UI.Global.").Append(attribute.ToString()).ToString());
		}

		// Token: 0x06002BA8 RID: 11176 RVA: 0x000EF303 File Offset: 0x000ED503
		public static string GetControlPointString(ControlPointType CP)
		{
			return Loc.T(new StringBuilder("UI.Nation.CP.").Append(CP.ToString()).Append("Control Point").ToString());
		}

		// Token: 0x06002BA9 RID: 11177 RVA: 0x000EF335 File Offset: 0x000ED535
		public static string RedLine(string str)
		{
			return new StringBuilder("<color=#B26A60>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BAA RID: 11178 RVA: 0x000EF356 File Offset: 0x000ED556
		public static string GreenLine(string str)
		{
			return new StringBuilder("<color=#85B260>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BAB RID: 11179 RVA: 0x000EF377 File Offset: 0x000ED577
		public static string CyanLine(string str)
		{
			return new StringBuilder("<color=#9BB9C7>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BAC RID: 11180 RVA: 0x000EF398 File Offset: 0x000ED598
		public static string HeaderCyanLine(string str)
		{
			return new StringBuilder("<color=#AFD1E0>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BAD RID: 11181 RVA: 0x000EF3B9 File Offset: 0x000ED5B9
		public static string BlueLine(string str)
		{
			return new StringBuilder("<color=#1589FF>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BAE RID: 11182 RVA: 0x000EF3DA File Offset: 0x000ED5DA
		public static string BlackLine(string str)
		{
			return new StringBuilder("<color=#000000>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BAF RID: 11183 RVA: 0x000EF3FB File Offset: 0x000ED5FB
		public static string PurpleLine(string str)
		{
			return new StringBuilder("<color=#9964FF>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BB0 RID: 11184 RVA: 0x000EF41C File Offset: 0x000ED61C
		public static string YellowLine(string str)
		{
			return new StringBuilder("<color=#B29B60>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BB1 RID: 11185 RVA: 0x000EF43D File Offset: 0x000ED63D
		public static string GoldLine(string str)
		{
			return new StringBuilder("<color=#B2AD98>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BB2 RID: 11186 RVA: 0x000EF45E File Offset: 0x000ED65E
		public static string GrayLine(string str)
		{
			return new StringBuilder("<color=#6C818B>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BB3 RID: 11187 RVA: 0x000EF47F File Offset: 0x000ED67F
		public static string HighlightLine(string str)
		{
			return new StringBuilder("<color=#EC9933>").Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BB4 RID: 11188 RVA: 0x000EF4A0 File Offset: 0x000ED6A0
		public static string FactionLine(string str, TIFactionState faction)
		{
			return new StringBuilder(faction.template.brightInlineColorString).Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BB5 RID: 11189 RVA: 0x000EF4C7 File Offset: 0x000ED6C7
		public static string TechCategoryLine(string str, TechCategory techCategory)
		{
			return new StringBuilder(TIUtilities.GetColorString(TemplateManager.global.techColor[(int)techCategory])).Append(str).Append("</color>").ToString();
		}

		// Token: 0x06002BB6 RID: 11190 RVA: 0x000EF500 File Offset: 0x000ED700
		public static string BuildResourceValueString(ResourceValue[] resourceValues)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < resourceValues.Length; i++)
			{
				if (resourceValues[i].resource != FactionResource.None)
				{
					stringBuilder.Append(resourceValues[i].ToString()).Append(" ");
				}
			}
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06002BB7 RID: 11191 RVA: 0x000EF564 File Offset: 0x000ED764
		public static Sprite GetStateIcon(TIFactionState faction, TIGameState state, bool detail)
		{
			if (state.isHabState)
			{
				if (detail && !string.IsNullOrEmpty(state.ref_hab.customHabIconResource))
				{
					return GameControl.assetLoader.LoadAsset<Sprite>(state.ref_hab.customHabIconResource);
				}
				return state.ref_hab.icon;
			}
			else if (state.isCouncilorState)
			{
				CouncilorView viewofCouncilor = faction.GetViewofCouncilor(state.ref_councilor);
				if (detail)
				{
					return GameControl.assetLoader.LoadAsset<Sprite>(viewofCouncilor.mapIconResourcePathCurrent);
				}
				return GameControl.assetLoader.LoadAsset<Sprite>(viewofCouncilor.genericIconResourcePath);
			}
			else
			{
				if (state.isNationState || state.isRegionState)
				{
					return state.ref_nation.flag;
				}
				if (state.isArmyState)
				{
					return state.ref_army.GetForegroundIcon();
				}
				if (state.isSpaceFleetState || state.isSpaceShipState)
				{
					return state.ref_fleet.icon;
				}
				if (state.isControlPointState)
				{
					if (state.ref_controlPoint.faction != null)
					{
						return state.ref_controlPoint.faction.factionIcon64;
					}
					return state.ref_nation.flag;
				}
				else
				{
					if (state.isSpaceObjectState)
					{
						return state.ref_spaceObject.icon;
					}
					if (state.isFactionState)
					{
						return state.ref_faction.factionIcon64;
					}
					if (state.isRegionAlienEntity)
					{
						return state.ref_regionAlienEntity.GetIcon(faction);
					}
					if (state.isRegionSpaceFacility)
					{
						return state.ref_regionSpaceFacility.GetIcon(faction);
					}
					TIOrgState tiorgState = state as TIOrgState;
					if (tiorgState != null)
					{
						return tiorgState.icon;
					}
					return null;
				}
			}
		}

		// Token: 0x06002BB8 RID: 11192 RVA: 0x000EF6D4 File Offset: 0x000ED8D4
		public static string GetStateIconPath(TIFactionState faction, TIGameState state, bool detail)
		{
			if (state.isHabState)
			{
				if (detail)
				{
					return state.ref_hab.customHabIconResource;
				}
				return state.ref_hab.iconResource;
			}
			else if (state.isCouncilorState)
			{
				CouncilorView viewofCouncilor = faction.GetViewofCouncilor(state.ref_councilor);
				if (detail)
				{
					return viewofCouncilor.mapIconResourcePathCurrent;
				}
				return viewofCouncilor.genericIconResourcePath;
			}
			else
			{
				if (state.isNationState || state.isRegionState)
				{
					return state.ref_nation.flagResource;
				}
				if (state.isArmyState)
				{
					return state.ref_army.GetIconForegroundResource;
				}
				if (state.isSpaceFleetState || state.isSpaceShipState)
				{
					return state.ref_fleet.iconResource;
				}
				if (state.isSpaceObjectState)
				{
					return state.ref_spaceObject.iconResource;
				}
				if (state.isControlPointState)
				{
					if (state.ref_controlPoint.faction != null)
					{
						return state.ref_controlPoint.faction.factionIcon64path;
					}
					return state.ref_nation.flagResource;
				}
				else
				{
					if (state.isFactionState)
					{
						return state.ref_faction.factionIcon64path;
					}
					if (state.isRegionAlienEntity)
					{
						return state.ref_regionAlienEntity.GetIconResourcePath(faction);
					}
					if (state.isRegionSpaceFacility)
					{
						return state.ref_regionSpaceFacility.GetIconResourcePath(faction);
					}
					if (state.isOrgState)
					{
						return state.ref_org.orgIconPath;
					}
					return null;
				}
			}
		}

		// Token: 0x06002BB9 RID: 11193 RVA: 0x000EF818 File Offset: 0x000EDA18
		public static string PathResourceIcon(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Money:
				return TemplateManager.global.pathMoneyIcon;
			case FactionResource.Influence:
				return TemplateManager.global.pathInfluenceIcon;
			case FactionResource.Operations:
				return TemplateManager.global.pathOpsIcon;
			case FactionResource.Research:
				return TemplateManager.global.pathResearchIcon;
			case FactionResource.Projects:
				return TemplateManager.global.pathProjectsIcon;
			case FactionResource.Boost:
				return TemplateManager.global.pathBoostIcon;
			case FactionResource.MissionControl:
				return TemplateManager.global.pathMissionControlIcon;
			case FactionResource.Water:
				return TemplateManager.global.pathWaterIcon;
			case FactionResource.Volatiles:
				return TemplateManager.global.pathVolatilesIcon;
			case FactionResource.Metals:
				return TemplateManager.global.pathBaseMetalsIcon;
			case FactionResource.NobleMetals:
				return TemplateManager.global.pathNobleMetalsIcon;
			case FactionResource.Fissiles:
				return TemplateManager.global.pathFissilesIcon;
			case FactionResource.Antimatter:
				return TemplateManager.global.pathAntimatterIcon;
			case FactionResource.Exotics:
				return TemplateManager.global.pathExoticsIcon;
			default:
				return "";
			}
		}

		// Token: 0x06002BBA RID: 11194 RVA: 0x000EF90C File Offset: 0x000EDB0C
		public static string InlineResourceStr(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Money:
				return TemplateManager.global.moneyInlineSpritePath;
			case FactionResource.Influence:
				return TemplateManager.global.influenceInlineSpritePath;
			case FactionResource.Operations:
				return TemplateManager.global.opsInlineSpritePath;
			case FactionResource.Research:
				return TemplateManager.global.researchInlineSpritePath;
			case FactionResource.Projects:
				return TemplateManager.global.projectsInlineSpritePath;
			case FactionResource.Boost:
				return TemplateManager.global.boostInlineSpritePath;
			case FactionResource.MissionControl:
				return TemplateManager.global.missionControlInlineSpritePath;
			case FactionResource.Water:
				return TemplateManager.global.waterInlineSpritePath;
			case FactionResource.Volatiles:
				return TemplateManager.global.volatilesInlineSpritePath;
			case FactionResource.Metals:
				return TemplateManager.global.metalsInlineSpritePath;
			case FactionResource.NobleMetals:
				return TemplateManager.global.noblesInlineSpritePath;
			case FactionResource.Fissiles:
				return TemplateManager.global.fissilesInlineSpritePath;
			case FactionResource.Antimatter:
				return TemplateManager.global.antimatterInlineSpritePath;
			case FactionResource.Exotics:
				return TemplateManager.global.exoticsInlineSpritePath;
			default:
				return string.Empty;
			}
		}

		// Token: 0x06002BBB RID: 11195 RVA: 0x000EFA00 File Offset: 0x000EDC00
		public static string PathAttributeIcon(CouncilorAttribute attribute)
		{
			switch (attribute)
			{
			case CouncilorAttribute.Persuasion:
				return TemplateManager.global.pathPersuasionIcon;
			case CouncilorAttribute.Investigation:
				return TemplateManager.global.pathInvestigationIcon;
			case CouncilorAttribute.Espionage:
				return TemplateManager.global.pathEspionageIcon;
			case CouncilorAttribute.Command:
				return TemplateManager.global.pathCommandIcon;
			case CouncilorAttribute.Administration:
				return TemplateManager.global.pathAdministrationIcon;
			case CouncilorAttribute.Science:
				return TemplateManager.global.pathScienceIcon;
			case CouncilorAttribute.Security:
				return TemplateManager.global.pathSecurityIcon;
			case CouncilorAttribute.Loyalty:
				return TemplateManager.global.pathLoyaltyIcon;
			default:
				return string.Empty;
			}
		}

		// Token: 0x06002BBC RID: 11196 RVA: 0x000EFA94 File Offset: 0x000EDC94
		public static string InlineAttributeStr(CouncilorAttribute attribute)
		{
			switch (attribute)
			{
			case CouncilorAttribute.Persuasion:
				return TemplateManager.global.persuasionInlineSpritePath;
			case CouncilorAttribute.Investigation:
				return TemplateManager.global.investigationInlineSpritePath;
			case CouncilorAttribute.Espionage:
				return TemplateManager.global.espionageInlineSpritePath;
			case CouncilorAttribute.Command:
				return TemplateManager.global.commandInlineSpritePath;
			case CouncilorAttribute.Administration:
				return TemplateManager.global.administrationInlineSpritePath;
			case CouncilorAttribute.Science:
				return TemplateManager.global.scienceInlineSpritePath;
			case CouncilorAttribute.Security:
				return TemplateManager.global.securityInlineSpritePath;
			case CouncilorAttribute.Loyalty:
				return TemplateManager.global.loyaltyInlineSpritePath;
			default:
				return string.Empty;
			}
		}

		// Token: 0x06002BBD RID: 11197 RVA: 0x000EFB28 File Offset: 0x000EDD28
		public static string InlineKeyboardModifierStr(KeyCode keycode)
		{
			switch (keycode)
			{
			case KeyCode.RightShift:
			case KeyCode.LeftShift:
				return "<color=#FFFFFFFF><sprite name=\"keyboard_shift\"></color>";
			case KeyCode.RightControl:
			case KeyCode.LeftControl:
				if (Loc.CurrentLanguage == "DEU")
				{
					return "<color=#FFFFFFFF><sprite name=\"keyboard_strg\"></color>";
				}
				return "<color=#FFFFFFFF><sprite name=\"keyboard_ctrl\"></color>";
			case KeyCode.RightAlt:
			case KeyCode.LeftAlt:
				return "<color=#FFFFFFFF><sprite name=\"keyboard_alt\"></color>";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06002BBE RID: 11198 RVA: 0x000EFB89 File Offset: 0x000EDD89
		public static string InlineMouseClickStr(int button)
		{
			switch (button)
			{
			case 0:
				return "<color=#FFFFFFFF><sprite name=\"mouse_left_click\"></color>";
			case 1:
				return "<color=#FFFFFFFF><sprite name=\"mouse_right_click\"></color>";
			case 2:
				return "<color=#FFFFFFFF><sprite name=\"mouse_middle_click\"></color>";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06002BBF RID: 11199 RVA: 0x000EFBB6 File Offset: 0x000EDDB6
		public static string GetSaveFileExtension()
		{
			if (!TIPlayerProfileManager.compressSaves)
			{
				return ".json";
			}
			return ".gz";
		}

		// Token: 0x06002BC0 RID: 11200 RVA: 0x000EFBCA File Offset: 0x000EDDCA
		public static string GetSaveFilePath(string filename)
		{
			return CreateSaveFileScrollList.GetSaveFolderPath() + filename + TIUtilities.GetSaveFileExtension();
		}

		// Token: 0x06002BC1 RID: 11201 RVA: 0x000EFBDC File Offset: 0x000EDDDC
		public static string GetMostRecentSave()
		{
			FileInfo[] files = new DirectoryInfo(CreateSaveFileScrollList.GetSaveFolderPath()).GetFiles();
			string text;
			if (files == null)
			{
				text = null;
			}
			else
			{
				IEnumerable<FileInfo> enumerable = files.Where<FileInfo>((FileInfo x) => TIUtilities.GetSaveFileExtension().Contains(x.Extension));
				if (enumerable == null)
				{
					text = null;
				}
				else
				{
					IOrderedEnumerable<FileInfo> orderedEnumerable = enumerable.OrderByDescending<FileInfo, DateTime>((FileInfo x) => x.LastWriteTime);
					if (orderedEnumerable == null)
					{
						text = null;
					}
					else
					{
						FileInfo fileInfo = orderedEnumerable.FirstOrDefault<FileInfo>();
						text = ((fileInfo != null) ? fileInfo.FullName : null);
					}
				}
			}
			string text2 = text;
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			return text2;
		}

		// Token: 0x06002BC2 RID: 11202 RVA: 0x000EFC78 File Offset: 0x000EDE78
		public static string GetContentBundleSuffix(int idx)
		{
			if (idx == 2 || idx == 3)
			{
				return "_prm";
			}
			return string.Empty;
		}

		// Token: 0x06002BC3 RID: 11203 RVA: 0x000EFC8D File Offset: 0x000EDE8D
		public static string ContentBundleShipAbbreviation(int idx)
		{
			if (idx == 2)
			{
				return "_PRM";
			}
			if (idx == 3)
			{
				return "_DLCA";
			}
			return string.Empty;
		}

		// Token: 0x06002BC4 RID: 11204 RVA: 0x000EFCA8 File Offset: 0x000EDEA8
		public static int GetHullAppearanceIndex(int index)
		{
			if ((index == 2 || index == 3) && !AssetBundleManager.AreDLCBundlesLoaded(1))
			{
				index -= 2;
			}
			return index;
		}

		// Token: 0x06002BC5 RID: 11205 RVA: 0x000EFCC0 File Offset: 0x000EDEC0
		public static float GetScreenRatio()
		{
			float num = (float)Screen.width;
			float num2 = (float)Screen.height;
			return num / num2;
		}

		// Token: 0x06002BC6 RID: 11206 RVA: 0x000EFCDC File Offset: 0x000EDEDC
		public static float GetAspectRatio(float width, float height)
		{
			return width / height;
		}

		// Token: 0x06002BC7 RID: 11207 RVA: 0x000EFCE4 File Offset: 0x000EDEE4
		public static bool CanIncreaseUIScale(float width, float height)
		{
			float aspectRatio = TIUtilities.GetAspectRatio(width, height);
			return aspectRatio > 1.6f && aspectRatio < 3.55f;
		}

		// Token: 0x06002BC8 RID: 11208 RVA: 0x000EFD0B File Offset: 0x000EDF0B
		public static float UIScaleFactor()
		{
			return 1080f / (float)TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting];
		}

		// Token: 0x06002BC9 RID: 11209 RVA: 0x000EFD24 File Offset: 0x000EDF24
		public static float GetMouseHeightRelativeToRectTransformBounds(RectTransform rt)
		{
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out vector);
			return Mathf.Clamp01((vector.y + rt.rect.height / 2f) / rt.rect.height);
		}

		// Token: 0x06002BCA RID: 11210 RVA: 0x000EFD74 File Offset: 0x000EDF74
		public static Component CopyComponent(Component original, GameObject destination)
		{
			Type type = original.GetType();
			Component component = destination.AddComponent(type);
			foreach (FieldInfo fieldInfo in type.GetFields())
			{
				fieldInfo.SetValue(component, fieldInfo.GetValue(original));
			}
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				if (propertyInfo.CanWrite && propertyInfo.CanWrite && !(propertyInfo.Name == "name"))
				{
					propertyInfo.SetValue(component, propertyInfo.GetValue(original, null), null);
				}
			}
			return component;
		}

		// Token: 0x06002BCB RID: 11211 RVA: 0x000EFE10 File Offset: 0x000EE010
		public static void UpdateButtonSpritesPlusMinus(Button button, bool plus, bool gold = false)
		{
			if (plus)
			{
				button.image.sprite = (gold ? AssetCacheManager.notificationPlusButtonIcon : AssetCacheManager.MaximizeButtonIcon);
				SpriteState spriteState = button.spriteState;
				spriteState.highlightedSprite = (gold ? AssetCacheManager.notificationPlusButtonHoverIcon : AssetCacheManager.MaximizeButtonHoverIcon);
				spriteState.pressedSprite = (gold ? AssetCacheManager.notificationPlusButtonHoverIcon : AssetCacheManager.MaximizeButtonHoverIcon);
				spriteState.selectedSprite = (gold ? AssetCacheManager.notificationPlusButtonHoverIcon : AssetCacheManager.MaximizeButtonHoverIcon);
				button.spriteState = spriteState;
				return;
			}
			button.image.sprite = (gold ? AssetCacheManager.notificationMinusButtonIcon : AssetCacheManager.MinimizeButtonIcon);
			SpriteState spriteState2 = button.spriteState;
			spriteState2.highlightedSprite = (gold ? AssetCacheManager.notificationMinusButtonHoverIcon : AssetCacheManager.MinimizeButtonHoverIcon);
			spriteState2.pressedSprite = (gold ? AssetCacheManager.notificationMinusButtonHoverIcon : AssetCacheManager.MinimizeButtonHoverIcon);
			spriteState2.selectedSprite = (gold ? AssetCacheManager.notificationMinusButtonHoverIcon : AssetCacheManager.MinimizeButtonHoverIcon);
			button.spriteState = spriteState2;
		}

		// Token: 0x06002BCC RID: 11212 RVA: 0x000EFEF8 File Offset: 0x000EE0F8
		public static void UpdateButtonSpritesPlusMinusAlt(Button button, bool plus)
		{
			if (plus)
			{
				button.image.sprite = AssetCacheManager.plusButtonIcon;
				SpriteState spriteState = button.spriteState;
				spriteState.highlightedSprite = AssetCacheManager.plusButtonHoverIcon;
				spriteState.pressedSprite = AssetCacheManager.plusButtonHoverIcon;
				spriteState.selectedSprite = AssetCacheManager.plusButtonHoverIcon;
				button.spriteState = spriteState;
				return;
			}
			button.image.sprite = AssetCacheManager.minusButtonIcon;
			SpriteState spriteState2 = button.spriteState;
			spriteState2.highlightedSprite = AssetCacheManager.minusButtonHoverIcon;
			spriteState2.pressedSprite = AssetCacheManager.minusButtonHoverIcon;
			spriteState2.selectedSprite = AssetCacheManager.minusButtonHoverIcon;
			button.spriteState = spriteState2;
		}

		// Token: 0x06002BCD RID: 11213 RVA: 0x000EFF90 File Offset: 0x000EE190
		public static string RemoveWorkshopTags(string dirtyText)
		{
			return dirtyText.Replace("[hr]", "").Replace("[/hr]", "").Replace("[b]", "")
				.Replace("[/b]", "")
				.Replace("[h1]", "")
				.Replace("[/h1]", "")
				.Replace("[h2]", "")
				.Replace("[/h2]", "")
				.Replace("[h3]", "")
				.Replace("[/h3]", "")
				.Replace("[list]", "")
				.Replace("[/list]", "")
				.Replace("[code]", "")
				.Replace("[/code]", "")
				.Replace("[*]", "");
		}

		// Token: 0x06002BCE RID: 11214 RVA: 0x000F007F File Offset: 0x000EE27F
		public static bool HasRadeonGPU()
		{
			return SystemInfo.graphicsDeviceName.ToLower().Contains("radeon");
		}

		// Token: 0x06002BCF RID: 11215 RVA: 0x000F0095 File Offset: 0x000EE295
		public static bool IsSteamDeck()
		{
			return SystemInfo.processorType == "AMD Custom APU 0405" || SystemInfo.processorType == "AMD Custom APU 0932";
		}

		// Token: 0x06002BD0 RID: 11216
		[DllImport("ntdll.dll")]
		private static extern string wine_get_version();

		// Token: 0x06002BD1 RID: 11217 RVA: 0x000F00BC File Offset: 0x000EE2BC
		public static bool IsLinux()
		{
			if (Application.isEditor)
			{
				return false;
			}
			bool flag;
			try
			{
				string text = TIUtilities.wine_get_version();
				Debug.LogWarning("UserLinuxWineVersion: " + text);
				flag = true;
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.ToString());
				flag = false;
			}
			return flag;
		}

		// Token: 0x170005F4 RID: 1524
		// (get) Token: 0x06002BD2 RID: 11218 RVA: 0x000F010C File Offset: 0x000EE30C
		private static global::System.Random random
		{
			get
			{
				if (TIUtilities._threadSafeRandom == null)
				{
					TIUtilities._threadSafeRandom = new global::System.Random();
				}
				return TIUtilities._threadSafeRandom;
			}
		}

		// Token: 0x170005F5 RID: 1525
		// (get) Token: 0x06002BD3 RID: 11219 RVA: 0x000F0124 File Offset: 0x000EE324
		private static Stack<global::System.Random> storedRandomStack
		{
			get
			{
				if (TIUtilities._storedRandomStack == null)
				{
					TIUtilities._storedRandomStack = new Stack<global::System.Random>();
				}
				return TIUtilities._storedRandomStack;
			}
		}

		// Token: 0x06002BD4 RID: 11220 RVA: 0x000F013C File Offset: 0x000EE33C
		public static void InitRandom(int seed = 478154)
		{
			if (TemplateManager.global != null)
			{
				int num = (Application.isEditor ? seed : ((int)DateTime.Now.Ticks));
				TIGlobalConfig global = TemplateManager.global;
				seed = ((global == null || global.campaignStartSeed != -1) ? TemplateManager.global.campaignStartSeed : num);
			}
			TIUtilities._threadSafeRandom = new global::System.Random(seed);
		}

		// Token: 0x06002BD5 RID: 11221 RVA: 0x000F019C File Offset: 0x000EE39C
		public static void PushRandomState(int? newSeed = null)
		{
			if (newSeed == null)
			{
				newSeed = new int?(Guid.NewGuid().GetHashCode());
			}
			TIUtilities.storedRandomStack.Push(TIUtilities.random);
			TIUtilities._threadSafeRandom = new global::System.Random(newSeed.Value);
		}

		// Token: 0x06002BD6 RID: 11222 RVA: 0x000F01EC File Offset: 0x000EE3EC
		public static void PopRandomState()
		{
			if (TIUtilities.storedRandomStack.Count == 0)
			{
				throw new InvalidOperationException("A previous System.Random instance has not been stored. Must call `PushRandomState()` first before popping an old instance.");
			}
			TIUtilities._threadSafeRandom = TIUtilities.storedRandomStack.Pop();
		}

		// Token: 0x06002BD7 RID: 11223 RVA: 0x000F0214 File Offset: 0x000EE414
		public static double RandomDouble(double min, double max)
		{
			double num = TIUtilities.random.NextDouble();
			return min + num * (max - min);
		}

		// Token: 0x06002BD8 RID: 11224 RVA: 0x000F0233 File Offset: 0x000EE433
		public static float RandomFloatValue()
		{
			return (float)TIUtilities.random.NextDouble();
		}

		// Token: 0x06002BD9 RID: 11225 RVA: 0x000F0240 File Offset: 0x000EE440
		public static int RandomRange(int minInclusive, int maxExclusive)
		{
			if (maxExclusive < minInclusive)
			{
				int num = minInclusive;
				int num2 = maxExclusive;
				maxExclusive = num;
				minInclusive = num2;
			}
			return TIUtilities.random.Next(minInclusive, maxExclusive);
		}

		// Token: 0x06002BDA RID: 11226 RVA: 0x000F0268 File Offset: 0x000EE468
		public static float RandomRange(float minInclusive, float maxExclusive)
		{
			float num = Mathf.Abs(maxExclusive - minInclusive);
			return TIUtilities.RandomFloatValue() * num + Mathf.Min(minInclusive, maxExclusive);
		}

		// Token: 0x06002BDB RID: 11227 RVA: 0x000F028D File Offset: 0x000EE48D
		public static void SetMainThread(Thread thread)
		{
			TIUtilities.mainThread = thread;
		}

		// Token: 0x06002BDC RID: 11228 RVA: 0x000F0295 File Offset: 0x000EE495
		public static bool IsMainThread(Thread thread)
		{
			return thread == TIUtilities.mainThread;
		}

		// Token: 0x06002BDD RID: 11229 RVA: 0x000F02A0 File Offset: 0x000EE4A0
		public static void TryPrepareVideo(VideoPlayer videoToPrepare)
		{
			try
			{
				videoToPrepare.Prepare();
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to prepare video: " + ex.Message);
			}
		}

		// Token: 0x06002BDE RID: 11230 RVA: 0x000F02E0 File Offset: 0x000EE4E0
		public static void TryPlayVideo(VideoPlayer videoToPlay)
		{
			try
			{
				videoToPlay.Play();
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to play video: " + ex.Message);
			}
		}

		// Token: 0x06002BDF RID: 11231 RVA: 0x000F0320 File Offset: 0x000EE520
		public static void PromptPlayerForBugReport(string message, bool recommendReload = true)
		{
			if (GameControl.control.skirmishMode)
			{
				return;
			}
			NotificationScreenController.singleton.PromptPlayerForBugReport(message, recommendReload);
		}

		// Token: 0x06002BE0 RID: 11232 RVA: 0x000F033C File Offset: 0x000EE53C
		public static float GetUnityRadius_Plane(Vector3d globalPosition, float radius_m)
		{
			Vector3d position = CameraManager.Singleton.Position;
			double num = Vector3d.Distance(in position, in globalPosition);
			float num2 = (float)Mathd.AngularDiameterOfPlane((double)radius_m, num);
			return TIUtilities.GetUnityRadiusFromAngularDiameter_Plane(globalPosition, num2);
		}

		// Token: 0x06002BE1 RID: 11233 RVA: 0x000F0370 File Offset: 0x000EE570
		public static float GetUnityRadiusFromAngularDiameter_Plane(Vector3d globalPosition, float angularDiameter)
		{
			Vector3d position = CameraManager.Singleton.Position;
			double num = Vector3d.Distance(in position, in globalPosition);
			return TIUtilities.GetUnityRadiusFromAngularDiameter_Plane(CameraManager.Singleton.ScaledDistance(num), angularDiameter);
		}

		// Token: 0x06002BE2 RID: 11234 RVA: 0x000F03A4 File Offset: 0x000EE5A4
		public static float GetUnityRadiusFromAngularDiameter_Plane(float unityDistance, float angularDiameter)
		{
			double num = Mathd.Tan((double)(angularDiameter * 0.017453292f / 2f));
			return (float)((double)unityDistance * num);
		}

		// Token: 0x06002BE3 RID: 11235 RVA: 0x000F03CC File Offset: 0x000EE5CC
		public static AccelerationConstraints GetAccelerationConstraintsForGroup(List<CombatShipController> ships, bool conserveDV)
		{
			if (ships.Count == 0)
			{
				return new AccelerationConstraints(-1f, -1f, -1f, -1f);
			}
			Dictionary<CombatShipController, AccelerationConstraints> dictionary = ships.ToDictionary<CombatShipController, CombatShipController, AccelerationConstraints>((CombatShipController x) => x, delegate(CombatShipController x)
			{
				if (!conserveDV)
				{
					return x.GetAccelerationConstraints();
				}
				return x.GetDVConservingAccelerationConstraints(true);
			});
			return new AccelerationConstraints(dictionary.Min<KeyValuePair<CombatShipController, AccelerationConstraints>>((KeyValuePair<CombatShipController, AccelerationConstraints> x) => x.Value.LinearAcceleration), dictionary.Min<KeyValuePair<CombatShipController, AccelerationConstraints>>((KeyValuePair<CombatShipController, AccelerationConstraints> x) => x.Value.CruiseLinearAcceleration), dictionary.Min<KeyValuePair<CombatShipController, AccelerationConstraints>>((KeyValuePair<CombatShipController, AccelerationConstraints> x) => x.Value.AngularAcceleration), dictionary.Min<KeyValuePair<CombatShipController, AccelerationConstraints>>((KeyValuePair<CombatShipController, AccelerationConstraints> x) => x.Value.MaxAngularVelocity));
		}

		// Token: 0x06002BE4 RID: 11236 RVA: 0x000F04D4 File Offset: 0x000EE6D4
		public static bool WillHitSphere(Vector3 myPosition, Vector3 myVelocity, Vector3 projectilePosition, Vector3 projectileVelocity_u, float diameter_m)
		{
			Vector3 vector = myPosition - projectilePosition;
			float num = Vector3.Dot(vector, (myVelocity - projectileVelocity_u).normalized);
			if (num >= 0f)
			{
				return false;
			}
			float num2 = diameter_m * 0.5f * GameControl.spaceCombat.modelScalingFactor;
			return Vector3.Dot(vector, vector) - num * num <= num2 * num2;
		}

		// Token: 0x06002BE5 RID: 11237 RVA: 0x000F0530 File Offset: 0x000EE730
		public static bool MovingTowardsTarget(Vector3 myPos, Vector3 myVelocity, Vector3 targetPos, Vector3 targetVelocity)
		{
			Vector3 vector = targetVelocity - myVelocity;
			Vector3 vector2 = targetPos - myPos;
			float num = vector.Dot(vector2);
			return num <= 0f && num < 0f;
		}

		// Token: 0x06002BE6 RID: 11238 RVA: 0x000F0568 File Offset: 0x000EE768
		public static List<RaycastHit> SimpleConeCastAll(Vector3 orgin, Vector3 direction, Vector3 directionRight, int numberOfRays, float angle_deg, float maxDistance, int LayerMask)
		{
			int num = 360 / numberOfRays;
			float num2 = maxDistance / Mathf.Cos(angle_deg * 0.017453292f);
			Dictionary<GameObject, RaycastHit> dictionary = new Dictionary<GameObject, RaycastHit>();
			for (int i = 0; i < numberOfRays; i++)
			{
				Vector3 vector = Quaternion.AngleAxis((float)(i * num), direction) * Quaternion.AngleAxis(angle_deg, directionRight) * direction.normalized * num2;
				foreach (RaycastHit raycastHit in Physics.RaycastAll(orgin, vector, maxDistance, LayerMask))
				{
					if (raycastHit.rigidbody != null && !dictionary.ContainsKey(raycastHit.collider.gameObject))
					{
						dictionary[raycastHit.collider.gameObject] = raycastHit;
					}
				}
			}
			foreach (RaycastHit raycastHit2 in Physics.RaycastAll(orgin, direction, maxDistance, LayerMask))
			{
				if (raycastHit2.collider.gameObject != null && !dictionary.ContainsKey(raycastHit2.collider.gameObject))
				{
					dictionary[raycastHit2.collider.gameObject] = raycastHit2;
				}
			}
			return dictionary.Values.ToList<RaycastHit>();
		}

		// Token: 0x06002BE7 RID: 11239 RVA: 0x000F06A6 File Offset: 0x000EE8A6
		public static void OpenWebURL(string webURL)
		{
			Application.OpenURL(webURL);
		}

		// Token: 0x06002BE8 RID: 11240 RVA: 0x000F06AE File Offset: 0x000EE8AE
		public static void OpenFileSystemURL(string fileSystemURL)
		{
			Application.OpenURL(fileSystemURL);
		}

		// Token: 0x0400214D RID: 8525
		public const string blackstr = "<color=#000000>";

		// Token: 0x0400214E RID: 8526
		public const string greenstr = "<color=#85B260>";

		// Token: 0x0400214F RID: 8527
		public const string redstr = "<color=#B26A60>";

		// Token: 0x04002150 RID: 8528
		public const string cyanstr = "<color=#9BB9C7>";

		// Token: 0x04002151 RID: 8529
		public const string bluestr = "<color=#1589FF>";

		// Token: 0x04002152 RID: 8530
		public const string purplestr = "<color=#9964FF>";

		// Token: 0x04002153 RID: 8531
		public const string yellowstr = "<color=#B29B60>";

		// Token: 0x04002154 RID: 8532
		public const string goldstr = "<color=#B2AD98>";

		// Token: 0x04002155 RID: 8533
		public const string dimmedGrayTextStr = "<color=#6C818B>";

		// Token: 0x04002156 RID: 8534
		public const string textHighlightStr = "<color=#EC9933>";

		// Token: 0x04002157 RID: 8535
		public const string headerTextColor = "<color=#AFD1E0>";

		// Token: 0x04002158 RID: 8536
		public const string textColor = "<color=#9BB9C7>";

		// Token: 0x04002159 RID: 8537
		public const string closecolor = "</color>";

		// Token: 0x0400215A RID: 8538
		public static readonly Color UITextColor = new Color(0.6862745f, 0.81960785f, 0.8784314f, 1f);

		// Token: 0x0400215B RID: 8539
		public static readonly Color UITextColorTransluscent = new Color(0.6862745f, 0.81960785f, 0.8784314f, 0.25f);

		// Token: 0x0400215C RID: 8540
		public static readonly Color UIRedTextColor = new Color(0.69803923f, 0.41568628f, 0.3764706f, 1f);

		// Token: 0x0400215D RID: 8541
		public static readonly Color UIHighlightColor = new Color(0.9254902f, 0.6f, 0.2f, 1f);

		// Token: 0x0400215E RID: 8542
		public static readonly Color UIDisabled = new Color(0.42352942f, 0.5058824f, 0.54509807f, 1f);

		// Token: 0x0400215F RID: 8543
		public static readonly Color UIColorIndicatorNeutral = new Color(0.8f, 0.69803923f, 0.43137255f);

		// Token: 0x04002160 RID: 8544
		public static readonly Color UIColorIndicatorPositive = new Color(0.4862745f, 0.69803923f, 0.30980393f);

		// Token: 0x04002161 RID: 8545
		public static readonly Color UIColorIndicatorNegative = new Color(0.69803923f, 0.41568628f, 0.3764706f);

		// Token: 0x04002162 RID: 8546
		public static readonly Color UIColorIndicatorPipUnfilled = new Color(0.2627451f, 0.35686275f, 0.43137255f);

		// Token: 0x04002163 RID: 8547
		public static readonly Color UIColorIndicatorTimePipEmpty = new Color(0.08235294f, 0.11764706f, 0.13725491f);

		// Token: 0x04002164 RID: 8548
		public static AssetLoader assetLoader = new AssetLoader();

		// Token: 0x04002165 RID: 8549
		public static CameraManager camera = World.Active.GetExistingManager<CameraManager>();

		// Token: 0x04002166 RID: 8550
		private const float defaultScalingForEarth = 1.7f;

		// Token: 0x04002167 RID: 8551
		private const float defaultScalingForSmallEarthRegions = 1.35f;

		// Token: 0x04002168 RID: 8552
		private const float defaultScalingForNaturalSpaceObjects = 3.25f;

		// Token: 0x04002169 RID: 8553
		private const float defaultScalingForOrbits = 3f;

		// Token: 0x0400216A RID: 8554
		private const float defaultScalingForHabs = 3f;

		// Token: 0x0400216B RID: 8555
		private const float defaultScalingForFleets = 1.5f;

		// Token: 0x0400216C RID: 8556
		private static readonly StringBuilder sb = new StringBuilder();

		// Token: 0x0400216D RID: 8557
		[ThreadStatic]
		private static global::System.Random _threadSafeRandom = null;

		// Token: 0x0400216E RID: 8558
		[ThreadStatic]
		private static Stack<global::System.Random> _storedRandomStack = null;

		// Token: 0x0400216F RID: 8559
		private static Thread mainThread;
	}
}
