using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.UI;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000906 RID: 2310
	public static class UITutorialAction
	{
		// Token: 0x0600585D RID: 22621 RVA: 0x002884DC File Offset: 0x002866DC
		public static void Execute(UITutorialActionType actionType)
		{
			switch (actionType)
			{
			case UITutorialActionType.None:
				return;
			case UITutorialActionType.GoToGeoscape:
				GameControl.control.viewMgr.GotoView(ViewType.PoliticalMap);
				return;
			case UITutorialActionType.GoToSolarSystem:
				GameControl.control.viewMgr.GotoView(ViewType.SolarSystem);
				World.Active.GetExistingManager<CameraManager>().Zoom(1047185094900.0, true);
				return;
			case UITutorialActionType.GeneralControls_Initialize:
			{
				UITutorialController.disallowHidingTutorialTipObject = true;
				CanvasManager existingManager = World.Active.GetExistingManager<CanvasManager>();
				existingManager.CloseActiveInfoScreen();
				existingManager.SetActiveAssetPanel(AssetPanel.None, 0f);
				existingManager.SetActiveInfoPanel(InfoPanel.None, 0f);
				(existingManager.Codex as CodexController).HideCodex();
				GeneralControlsController generalControlsController = existingManager.StrategyHud as GeneralControlsController;
				if (generalControlsController != null)
				{
					generalControlsController.alarmPanel.SetActive(false);
				}
				UITutorialController.disallowHidingTutorialTipObject = false;
				return;
			}
			case UITutorialActionType.GeneralControls_OpenFinder:
			{
				GeneralControlsController generalControlsController2 = World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController;
				if (generalControlsController2 != null)
				{
					generalControlsController2.EnableFinderCanvas(true);
					return;
				}
				return;
			}
			case UITutorialActionType.GeneralControls_OpenFakeTutorialTip:
			{
				GeneralControlsController generalControlsController3 = World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController;
				if (generalControlsController3 != null)
				{
					generalControlsController3.fakeTutorialWindow.SetActive(true);
					return;
				}
				return;
			}
			case UITutorialActionType.GeneralControls_CloseFakeTutorialTip:
			{
				GeneralControlsController generalControlsController4 = World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController;
				if (generalControlsController4 != null)
				{
					generalControlsController4.fakeTutorialWindow.SetActive(false);
					return;
				}
				return;
			}
			case UITutorialActionType.GeneralControls_OpenEarthInterfaceStation:
			{
				TIFactionState activePlayer = GameControl.control.activePlayer;
				using (IEnumerator<TIHabState> enumerator = GameStateManager.IterateByClass<TIHabState>(false).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIHabState tihabState = enumerator.Current;
						if (tihabState.ref_faction == activePlayer && tihabState.IsStation && tihabState.orbitState.interfaceOrbit && tihabState.orbitState.ref_spaceBody.isEarth)
						{
							TIUtilities.GotoGameState(tihabState, false, true, true, false, false, -1f);
							break;
						}
					}
					return;
				}
				break;
			}
			case UITutorialActionType.GeneralControls_OpenSellResourcesWindow:
				break;
			case UITutorialActionType.Habs_SelectFirstHab:
			{
				HabitatsScreenController infoScreen = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<HabitatsScreenController>();
				if (infoScreen != null)
				{
					infoScreen.Tutorial_SelectFirstPlayerHab();
					return;
				}
				return;
			}
			case UITutorialActionType.Fleets_ExpandFirstFleet:
			{
				FleetsScreenController infoScreen2 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<FleetsScreenController>();
				if (infoScreen2 != null)
				{
					infoScreen2.Tutorial_ExpandFirstFleet();
					return;
				}
				return;
			}
			case UITutorialActionType.Fleets_UnexpandAllFleets:
			{
				FleetsScreenController infoScreen3 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<FleetsScreenController>();
				if (infoScreen3 != null)
				{
					infoScreen3.Tutorial_UnExpandFirstFleet();
					return;
				}
				return;
			}
			case UITutorialActionType.Intel_ConfigureExampleTransfer:
			{
				IntelScreenController intelScreenController = World.Active.GetExistingManager<CanvasManager>().ShowInfoScreen<IntelScreenController>() as IntelScreenController;
				if (intelScreenController != null)
				{
					intelScreenController.Tutorial_ConfigureExampleTransfer();
					return;
				}
				return;
			}
			case UITutorialActionType.NationInfo_OpenPrioritiesTab:
			{
				NationInfoController nationInfoController = World.Active.GetExistingManager<CanvasManager>().NationInfo as NationInfoController;
				if (nationInfoController != null)
				{
					nationInfoController.OpenPrioritiesTab();
					return;
				}
				return;
			}
			case UITutorialActionType.NationInfo_OpenPolicyTab:
			{
				NationInfoController nationInfoController2 = World.Active.GetExistingManager<CanvasManager>().NationInfo as NationInfoController;
				if (nationInfoController2 != null)
				{
					nationInfoController2.OpenPolicyTab();
					return;
				}
				return;
			}
			case UITutorialActionType.NationInfo_OpenRegionsTab:
			{
				NationInfoController nationInfoController3 = World.Active.GetExistingManager<CanvasManager>().NationInfo as NationInfoController;
				if (nationInfoController3 != null)
				{
					nationInfoController3.OpenRegionsTab();
					return;
				}
				return;
			}
			case UITutorialActionType.NationInfo_OpenRelationsTab:
			{
				NationInfoController nationInfoController4 = World.Active.GetExistingManager<CanvasManager>().NationInfo as NationInfoController;
				if (nationInfoController4 != null)
				{
					nationInfoController4.OpenRelationsTab();
					return;
				}
				return;
			}
			case UITutorialActionType.NationInfo_OpenArmiesTab:
			{
				NationInfoController nationInfoController5 = World.Active.GetExistingManager<CanvasManager>().NationInfo as NationInfoController;
				if (nationInfoController5 != null)
				{
					nationInfoController5.OpenArmiesTab();
					return;
				}
				return;
			}
			case UITutorialActionType.NationInfo_OpenCouncilorsTab:
			{
				NationInfoController nationInfoController6 = World.Active.GetExistingManager<CanvasManager>().NationInfo as NationInfoController;
				if (nationInfoController6 != null)
				{
					nationInfoController6.OpenCouncilorsTab();
					return;
				}
				return;
			}
			case UITutorialActionType.NationInfo_SelectExoCapableNation:
			{
				if (!(World.Active.GetExistingManager<CanvasManager>().NationInfo as NationInfoController != null))
				{
					return;
				}
				TIMapRegionTemplate dixieRegionTemplate = TemplateManager.Find<TIMapRegionTemplate>("map_Dixie", false);
				TIRegionState tiregionState = GameStateManager.IterateByClass<TIRegionState>(false).First<TIRegionState>((TIRegionState x) => x.mapRegionTemplate == dixieRegionTemplate);
				if (tiregionState != null)
				{
					TIUtilities.GotoGameState(tiregionState, true, true, true, true, false, -1f);
					return;
				}
				return;
			}
			case UITutorialActionType.NationInfo_TargetConstructOrbitalFighter:
			{
				NationInfoController nationInfoController7 = World.Active.GetExistingManager<CanvasManager>().NationInfo as NationInfoController;
				if (nationInfoController7 != null)
				{
					nationInfoController7.Tutorial_TargetConstructOrbitalFightersPriority();
					return;
				}
				return;
			}
			case UITutorialActionType.Nations_Initialize:
			{
				NationsScreenController infoScreen4 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<NationsScreenController>();
				if (infoScreen4 != null)
				{
					infoScreen4.OnCloseCPBreakdown();
					infoScreen4.nationListAdapter.ScrollTo(0, 0f, 0f);
					return;
				}
				return;
			}
			case UITutorialActionType.Nations_ShowAllNations:
			{
				NationsScreenController infoScreen5 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<NationsScreenController>();
				if (infoScreen5 != null && (infoScreen5.nationListAdapter == null || infoScreen5.nationListAdapter.VisibleItemsCount == 0))
				{
					infoScreen5.filterFactionToggle.isOn = true;
					return;
				}
				return;
			}
			case UITutorialActionType.Council_OpenOrgMarketplace:
			{
				CouncilGridController infoScreen6 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<CouncilGridController>();
				if (infoScreen6 != null)
				{
					infoScreen6.Tutorial_OpenOrgMarketplace();
					return;
				}
				return;
			}
			case UITutorialActionType.Council_OpenUnassignedOrgs:
			{
				CouncilGridController infoScreen7 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<CouncilGridController>();
				if (infoScreen7 != null)
				{
					infoScreen7.Tutorial_OpenUnassignedOrgs();
					return;
				}
				return;
			}
			case UITutorialActionType.Council_OpenCandidateDossier:
			{
				CouncilGridController infoScreen8 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<CouncilGridController>();
				if (infoScreen8 != null)
				{
					infoScreen8.Tutorial_SelectFirstCandidate();
					return;
				}
				return;
			}
			case UITutorialActionType.Council_InitializeOrgManager:
			{
				CouncilGridController infoScreen9 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<CouncilGridController>();
				if (infoScreen9 != null)
				{
					infoScreen9.Tutorial_InitOrgManager();
					return;
				}
				return;
			}
			case UITutorialActionType.Council_HighlightFirstClock:
			{
				CouncilGridController infoScreen10 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<CouncilGridController>();
				if (infoScreen10 != null)
				{
					infoScreen10.Tutorial_HighlightFirstCalendarClock();
					return;
				}
				return;
			}
			case UITutorialActionType.Councilor_OpenTargetingPanel:
			{
				TIMapRegionTemplate almatyRegionTemplate = TemplateManager.Find<TIMapRegionTemplate>("map_Almaty", false);
				TIRegionState tiregionState2 = GameStateManager.IterateByClass<TIRegionState>(false).First<TIRegionState>((TIRegionState x) => x.mapRegionTemplate == almatyRegionTemplate);
				if (tiregionState2 != null)
				{
					GameControl.eventManager.TriggerEvent(new MissionOptionsForTargetRequested(tiregionState2), null, Array.Empty<object>());
					return;
				}
				return;
			}
			case UITutorialActionType.Operations_InitializeTransferOp:
			{
				OperationCanvasController operationCanvasController = World.Active.GetExistingManager<CanvasManager>().OperationCanvasController as OperationCanvasController;
				if (operationCanvasController != null && !operationCanvasController.targetSelectionTool.gameObject.activeInHierarchy)
				{
					operationCanvasController.MaximizeTargetPanel();
					return;
				}
				return;
			}
			case UITutorialActionType.Operations_HighlightLaunchExofighterOp:
			{
				OperationCanvasController operationCanvasController2 = World.Active.GetExistingManager<CanvasManager>().OperationCanvasController as OperationCanvasController;
				if (operationCanvasController2 != null)
				{
					operationCanvasController2.Tutorial_HighlightLaunchExofighterOp();
					return;
				}
				return;
			}
			case UITutorialActionType.Research_InitializeTechTree:
			{
				ResearchScreenController infoScreen11 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<ResearchScreenController>();
				if (infoScreen11 != null)
				{
					if (infoScreen11.selectiveTechTreeCanvas.enabled)
					{
						infoScreen11.CloseSelectiveTechTree();
					}
					infoScreen11.ShowNoProjectTechTree();
					return;
				}
				return;
			}
			case UITutorialActionType.Research_OpenInfoPanel:
			{
				ResearchScreenController infoScreen12 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<ResearchScreenController>();
				if (infoScreen12 != null)
				{
					if (infoScreen12.selectiveTechTreeCanvas.enabled)
					{
						infoScreen12.CloseSelectiveTechTree();
					}
					ChildTechGridItemController childTechGridItemController = infoScreen12.noProjectTechList.First<ChildTechGridItemController>((ChildTechGridItemController x) => x.tech.dataName.Equals("MissiontotheMoon"));
					infoScreen12.SetSelectedTechEntry(childTechGridItemController.tech.dataName);
					infoScreen12.DisplayTechTree(childTechGridItemController.tech);
					return;
				}
				return;
			}
			case UITutorialActionType.Research_OpenSubtree:
			{
				ResearchScreenController infoScreen13 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<ResearchScreenController>();
				if (infoScreen13 != null)
				{
					ChildTechGridItemController childTechGridItemController2 = infoScreen13.noProjectTechList.First<ChildTechGridItemController>((ChildTechGridItemController x) => x.tech.dataName.Equals("MissiontotheMoon"));
					infoScreen13.InitializeSelectiveTechTree(childTechGridItemController2.tech.dataName, childTechGridItemController2.tech.displayName, false);
					infoScreen13.selectiveTechList.First<ChildTechGridItemController>((ChildTechGridItemController x) => x.tech.dataName.Equals("MissiontotheMoon")).SelectFullTechItem(false);
					return;
				}
				return;
			}
			case UITutorialActionType.Research_ExitSubtree:
			{
				ResearchScreenController infoScreen14 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<ResearchScreenController>();
				if (infoScreen14 != null && infoScreen14.selectiveTechTreeCanvas.enabled)
				{
					infoScreen14.CloseSelectiveTechTree();
					return;
				}
				return;
			}
			case UITutorialActionType.Research_OpenFullTree:
			{
				ResearchScreenController infoScreen15 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<ResearchScreenController>();
				if (infoScreen15 != null)
				{
					ChildTechGridItemController childTechGridItemController3 = infoScreen15.mainTechObjectList.First<ChildTechGridItemController>((ChildTechGridItemController x) => x.tech.dataName.Equals("Project_RapidRepairingMaterials"));
					infoScreen15.SetSelectedProjectEntry(childTechGridItemController3.tech.dataName);
					infoScreen15.ShowFullTechTree();
					return;
				}
				return;
			}
			case UITutorialActionType.SpaceCombat_GoToMiddleShip:
			{
				SpaceCombatCameraController component = GameControl.control.mainCamera.GetComponent<SpaceCombatCameraController>();
				if (!(component != null))
				{
					return;
				}
				GameObject gameObject = GameControl.spaceCombat.activeShips[0].gameObject;
				if (gameObject != null)
				{
					component.LookAtObject(gameObject);
					return;
				}
				return;
			}
			case UITutorialActionType.SpaceCombat_GoToWaypoint1:
			{
				SpaceCombatCameraController component2 = GameControl.control.mainCamera.GetComponent<SpaceCombatCameraController>();
				if (!(component2 != null))
				{
					return;
				}
				GameObject gameObject2 = GameControl.spaceCombat.activeShips[0]._waypointNavigationController.WaypointContainer.transform.GetChild(1).gameObject;
				if (gameObject2 != null)
				{
					component2.LookAtObject(gameObject2);
					return;
				}
				return;
			}
			case UITutorialActionType.SpaceCombat_GoToWaypoint2:
			{
				SpaceCombatCameraController component3 = GameControl.control.mainCamera.GetComponent<SpaceCombatCameraController>();
				if (!(component3 != null))
				{
					return;
				}
				GameObject gameObject3 = GameControl.spaceCombat.activeShips[0]._waypointNavigationController.WaypointContainer.transform.GetChild(2).gameObject;
				if (gameObject3 != null)
				{
					component3.LookAtObject(gameObject3);
					return;
				}
				return;
			}
			case UITutorialActionType.SpaceCombat_GoToWaypoint3:
			{
				SpaceCombatCameraController component4 = GameControl.control.mainCamera.GetComponent<SpaceCombatCameraController>();
				if (!(component4 != null))
				{
					return;
				}
				GameObject gameObject4 = GameControl.spaceCombat.activeShips[0]._waypointNavigationController.WaypointContainer.transform.GetChild(3).gameObject;
				if (gameObject4 != null)
				{
					component4.LookAtObject(gameObject4);
					return;
				}
				return;
			}
			case UITutorialActionType.SpaceCombat_PauseSpaceCombat:
			{
				SpaceCombatCanvasController spaceCombatCanvasController = GameControl.canvasStack.CombatHud as SpaceCombatCanvasController;
				if (spaceCombatCanvasController != null)
				{
					spaceCombatCanvasController.clockController.PauseNoToggle();
					return;
				}
				return;
			}
			case UITutorialActionType.SpaceCombat_RevokeAIControl:
			{
				SpaceCombatCanvasController spaceCombatCanvasController2 = GameControl.canvasStack.CombatHud as SpaceCombatCanvasController;
				if (!(spaceCombatCanvasController2 != null))
				{
					return;
				}
				List<TISpaceShipState> list = new List<TISpaceShipState>();
				list = (from x in spaceCombatCanvasController2.leftHandCombatants.Keys
					select x.GetCombatantState() as TISpaceShipState into y
					where y != null && !y.ShipDestroyed() && !y.hasDisengaged
					select y).ToList<TISpaceShipState>();
				IFleetCommand fleetCommand = null;
				foreach (IFleetCommand fleetCommand2 in ShipCommandsManager.fleetCommands)
				{
					if (fleetCommand2 is FleetReleaseAIControlCommand)
					{
						fleetCommand = fleetCommand2;
						break;
					}
				}
				if (fleetCommand != null)
				{
					fleetCommand.OnExecuteFleetCommand(list, null);
					return;
				}
				return;
			}
			case UITutorialActionType.SpaceCombat_DeselectShips:
			{
				SpaceCombatCanvasController spaceCombatCanvasController3 = GameControl.canvasStack.CombatHud as SpaceCombatCanvasController;
				if (!(spaceCombatCanvasController3 != null))
				{
					return;
				}
				spaceCombatCanvasController3.ClearGroupSelect();
				if (!(spaceCombatCanvasController3.selectedFriendlyShip != null) || !spaceCombatCanvasController3.combatMgr.combatantLookup.ContainsKey(spaceCombatCanvasController3.selectedFriendlyShip.ShipState))
				{
					return;
				}
				CombatantController combatantController = spaceCombatCanvasController3.combatMgr.combatantLookup[spaceCombatCanvasController3.selectedFriendlyShip.ShipState];
				if (combatantController != null)
				{
					spaceCombatCanvasController3.DeselectShip(combatantController);
					return;
				}
				return;
			}
			case UITutorialActionType.Council_InitializeSingleCouncilorScreen:
			{
				CouncilGridController infoScreen16 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<CouncilGridController>();
				if (infoScreen16 != null)
				{
					infoScreen16.Tutorial_InitializeSingleCouncilorScreen();
					return;
				}
				return;
			}
			case UITutorialActionType.Council_InitializeCouncilRecruitScreen:
			{
				CouncilGridController infoScreen17 = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<CouncilGridController>();
				if (infoScreen17 != null)
				{
					infoScreen17.Tutorial_InitializeCouncilRecruitScreen();
					return;
				}
				return;
			}
			case UITutorialActionType.GeneralControls_IntroHighlight:
				World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController != null;
				return;
			default:
				return;
			}
			GeneralControlsController generalControlsController5 = World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController;
			if (generalControlsController5 != null)
			{
				generalControlsController5.OnClickResources();
				return;
			}
		}
	}
}
