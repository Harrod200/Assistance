using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.UI;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000911 RID: 2321
	public class TerminalCouncilorCommands
	{
		// Token: 0x060058B8 RID: 22712 RVA: 0x0028A1C9 File Offset: 0x002883C9
		public TerminalCouncilorCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x060058B9 RID: 22713 RVA: 0x0028A1E0 File Offset: 0x002883E0
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("giveorg", new CommandHandler(this.GiveOrg), "Add councilor first and last name OR faction dataName and unique org dataName to give an org to a councilor/faction: 'giveorg John Thomas,CentralIntelligenceAgency'");
			this.terminalController.RegisterCommand("killstate", new CommandHandler(this.KillState), "Kill the current UI Selected State (army, councilor, fleet, hab)");
			this.terminalController.RegisterCommand("killasset", new CommandHandler(this.KillAsset), "Kill the current UI Selected Asset (army, councilor, fleet");
			this.terminalController.RegisterCommand("forcemissions", new CommandHandler(this.ForceMissions), "Force Missions to complete(using for testing victory missions, may break other things; Use again to toggle)");
			this.terminalController.RegisterCommand("nosecrets", new CommandHandler(this.NoSecrets), "Set active player intel on all factions to max");
			this.terminalController.RegisterCommand("watchfactions", new CommandHandler(this.WatchHumanFactions), "Set active player intel on top-level human faction data to max");
			this.terminalController.RegisterCommand("designships", new CommandHandler(this.DesignShips), "Force all AI factions to update their ship designs");
			this.terminalController.RegisterCommand("dumpdesigns", new CommandHandler(this.DumpDesigns), "Summarize All ship designs in AI Log, option include faction");
			this.terminalController.RegisterCommand("setfaction", new CommandHandler(this.SetFaction), "Change active player faction. Not fully supported; can inspect some updated UIs but save and reload to play.");
			this.terminalController.RegisterCommand("detainme", new CommandHandler(this.DetainCouncilor), "Detain selected councilor to the assigned faction: detainme ResistCouncil");
			this.terminalController.RegisterCommand("addtrait", new CommandHandler(this.AddTrait), "Give trait to selected councilor");
			this.terminalController.RegisterCommand("removetrait", new CommandHandler(this.RemoveTrait), "Remove trait from selected councilor");
			this.terminalController.RegisterCommand("sight", new CommandHandler(this.SightAlienMissions), "Sight all alien missions");
			this.terminalController.RegisterCommand("teleport", new CommandHandler(this.TeleportCouncilor), "Teleport councilor (full name) to selected asset");
			this.terminalController.RegisterCommand("givexp", new CommandHandler(this.GiveXP), "Give XP to selected councilor: giveXP 200");
			this.terminalController.RegisterCommand("turnfaction", new CommandHandler(this.TurnFaction), "Compromise Faction: turnfaction SubmitCouncil");
		}

		// Token: 0x060058BA RID: 22714 RVA: 0x0028A400 File Offset: 0x00288600
		public void TurnFaction(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("Requires a faction dataName");
				return;
			}
			TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
			if (tifactionState != null && tifactionState.councilors.Count > 0)
			{
				tifactionState.councilors.SelectRandomItem<TICouncilorState>().TurnCouncilor(GameControl.control.activePlayer);
			}
		}

		// Token: 0x060058BB RID: 22715 RVA: 0x0028A460 File Offset: 0x00288660
		private void TeleportCouncilor(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("Requires councilor first and last name");
			}
			TICouncilorState ticouncilorState = GameStateManager.IterateByClass<TICouncilorState>(false).FirstOrDefault<TICouncilorState>((TICouncilorState x) => x.displayName == args[0]);
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState != null)
			{
				if (uiselectedAssetState.hasEarthMapObject)
				{
					ticouncilorState.SetLocation(uiselectedAssetState.ref_region);
					return;
				}
				if (uiselectedAssetState.isSpaceFleetState)
				{
					ticouncilorState.SetLocation(uiselectedAssetState.ref_fleet.ships[0]);
					return;
				}
				if (uiselectedAssetState.ref_hab != null)
				{
					ticouncilorState.SetLocation(uiselectedAssetState.ref_hab);
					return;
				}
			}
			else
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				if (uiotherSelectedState != null)
				{
					if (uiotherSelectedState.hasEarthMapObject)
					{
						ticouncilorState.SetLocation(uiotherSelectedState.ref_region);
						return;
					}
					if (uiotherSelectedState.isSpaceFleetState)
					{
						ticouncilorState.SetLocation(uiotherSelectedState.ref_fleet.ships[0]);
						return;
					}
					if (uiotherSelectedState.ref_hab != null)
					{
						ticouncilorState.SetLocation(uiotherSelectedState.ref_hab);
					}
				}
			}
		}

		// Token: 0x060058BC RID: 22716 RVA: 0x0028A56C File Offset: 0x0028876C
		private void GiveOrg(string[] args)
		{
			if (args.Length < 2)
			{
				this.terminalController.OutputError("Requires councilor first and last name or faction name and org dataName, separated by a comma.");
				return;
			}
			TICouncilorState ticouncilorState = GameStateManager.IterateByClass<TICouncilorState>(false).FirstOrDefault<TICouncilorState>((TICouncilorState x) => x.displayName == args[0]);
			TIFactionState tifactionState = ((ticouncilorState != null) ? ticouncilorState.faction : null);
			if (ticouncilorState == null)
			{
				tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
				if (tifactionState == null)
				{
					this.terminalController.OutputError("Bad councilor / faction name " + args[0] + " entered.");
					return;
				}
			}
			TIOrgState tiorgState = GameStateManager.IterateByClass<TIOrgState>(false).FirstOrDefault<TIOrgState>((TIOrgState x) => x.templateName == args[1]);
			if (tiorgState == null)
			{
				TIOrgTemplate tiorgTemplate = TemplateManager.Find<TIOrgTemplate>(args[1], false);
				if (tiorgTemplate != null)
				{
					tifactionState.GrantNewOrgToCouncilor(ticouncilorState ?? tifactionState.councilors[0], tiorgTemplate.dataName);
					return;
				}
				this.terminalController.OutputError("Could not find org " + args[1]);
				return;
			}
			else
			{
				TIFactionState factionOrbit = tiorgState.factionOrbit;
				if (factionOrbit != null)
				{
					factionOrbit.LoseOrg(tiorgState);
				}
				if (ticouncilorState == null)
				{
					tifactionState.AddOrgToFactionPool(tiorgState, null, false);
					foreach (TIFactionState tifactionState2 in GameStateManager.AllFactions())
					{
						if (tifactionState2.availableOrgs.Contains(tiorgState))
						{
							tifactionState2.availableOrgs.Remove(tiorgState);
							Debug.LogError("Removing cheated org from market of faction: " + tifactionState2.displayName);
						}
					}
				}
				else
				{
					List<TIOrgState> list;
					ticouncilorState.StealOrg(tiorgState, out list);
				}
				if (tiorgState == null)
				{
					this.terminalController.OutputError("Could not find org " + args[1]);
					return;
				}
				if (tiorgState.factionOrbit == null)
				{
					Log.Error("No faction for cheated org", Array.Empty<object>());
					tiorgState.SetFactionOrbit(((ticouncilorState != null) ? ticouncilorState.faction : null) ?? tifactionState);
				}
				return;
			}
		}

		// Token: 0x060058BD RID: 22717 RVA: 0x0028A760 File Offset: 0x00288960
		private void KillState(string[] args)
		{
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			if (uiotherSelectedState != null)
			{
				if (uiotherSelectedState.isArmyState)
				{
					uiotherSelectedState.ref_army.TakeDamage(1f, GameControl.control.activePlayer, null, false);
					return;
				}
				if (uiotherSelectedState.isCouncilorState)
				{
					uiotherSelectedState.ref_councilor.KillCouncilor(false, GameControl.control.activePlayer);
					return;
				}
				if (uiotherSelectedState.isSpaceFleetState)
				{
					using (List<TISpaceShipState>.Enumerator enumerator = new List<TISpaceShipState>(uiotherSelectedState.ref_fleet.ships).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TISpaceShipState tispaceShipState = enumerator.Current;
							tispaceShipState.DestroyShip(true, null);
						}
						return;
					}
				}
				if (uiotherSelectedState.isHabState)
				{
					uiotherSelectedState.ref_hab.DestroyHab((uiotherSelectedState.ref_faction == GameControl.control.activePlayer) ? GameStateManager.AlienProxy() : GameControl.control.activePlayer, 0.25f, false, null, 0f);
					return;
				}
				if (uiotherSelectedState.isRegionSpaceFacility)
				{
					uiotherSelectedState.ref_regionSpaceFacility.region.DestroySpaceFacility(uiotherSelectedState.ref_regionSpaceFacility.spaceFacilityType, false);
					return;
				}
			}
			else
			{
				this.terminalController.OutputError("No selected state to kill");
			}
		}

		// Token: 0x060058BE RID: 22718 RVA: 0x0028A89C File Offset: 0x00288A9C
		private void KillAsset(string[] args)
		{
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState != null)
			{
				if (uiselectedAssetState.isArmyState)
				{
					uiselectedAssetState.ref_army.TakeDamage(1f, GameStateManager.AlienProxy(), null, false);
				}
				if (uiselectedAssetState.isCouncilorState)
				{
					uiselectedAssetState.ref_councilor.KillCouncilor(false, null);
				}
				if (!uiselectedAssetState.isSpaceFleetState)
				{
					return;
				}
				using (List<TISpaceShipState>.Enumerator enumerator = new List<TISpaceShipState>(uiselectedAssetState.ref_fleet.ships).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipState tispaceShipState = enumerator.Current;
						tispaceShipState.DestroyShip(true, null);
					}
					return;
				}
			}
			this.terminalController.OutputError("No selected asset to kill");
		}

		// Token: 0x060058BF RID: 22719 RVA: 0x0028A954 File Offset: 0x00288B54
		private void ForceMissions(string[] args)
		{
			CouncilorMissionCanvasController component = GameControl.control._canvasStack.CouncilorMissionController.GameObject.GetComponent<CouncilorMissionCanvasController>();
			if (!component.forceAllowMissions)
			{
				component.forceAllowMissions = true;
				return;
			}
			component.forceAllowMissions = false;
		}

		// Token: 0x060058C0 RID: 22720 RVA: 0x0028A994 File Offset: 0x00288B94
		private void WatchHumanFactions(string[] args)
		{
			foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
			{
				GameControl.control.activePlayer.SetIntel(tifactionState, 1f, null, false);
			}
		}

		// Token: 0x060058C1 RID: 22721 RVA: 0x0028A9D0 File Offset: 0x00288BD0
		private void NoSecrets(string[] args)
		{
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				GameControl.control.activePlayer.SetIntel(tifactionState, 1f, null, false);
				foreach (TICouncilorState ticouncilorState in tifactionState.councilors)
				{
					GameControl.control.activePlayer.SetIntel(ticouncilorState, 1f, null, false);
				}
			}
			if (args.Length != 0 && args[0] == "1")
			{
				foreach (TISpaceBodyState tispaceBodyState in GameStateManager.IterateByClass<TISpaceBodyState>(false))
				{
					GameControl.control.activePlayer.ProspectSpaceBody(tispaceBodyState);
				}
			}
		}

		// Token: 0x060058C2 RID: 22722 RVA: 0x0028AAC4 File Offset: 0x00288CC4
		private void DesignShips(string[] args)
		{
			TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
			if (tifactionState != null && tifactionState.player.isAI)
			{
				List<string> list = tifactionState.shipDesigns.Select<TISpaceShipTemplate, string>((TISpaceShipTemplate x) => x.dataName).ToList<string>();
				AIDailyFactionPlanner.DesignShips(tifactionState, null);
				using (IEnumerator<TISpaceShipTemplate> enumerator = tifactionState.shipDesigns.OrderBy<TISpaceShipTemplate, ShipRole>((TISpaceShipTemplate x) => x.role).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipTemplate tispaceShipTemplate = enumerator.Current;
						if (!list.Contains(tispaceShipTemplate.dataName))
						{
							TIFactionState.LogAI("NEW: " + tispaceShipTemplate.DebugSummary(), false);
						}
						else
						{
							TIFactionState.LogAI("OLD: " + tispaceShipTemplate.DebugSummary(), false);
						}
					}
					return;
				}
			}
			this.terminalController.OutputError("Could not find faction " + args[0]);
		}

		// Token: 0x060058C3 RID: 22723 RVA: 0x0028ABDC File Offset: 0x00288DDC
		private void DumpDesigns(string[] args)
		{
			List<TIFactionState> list = new List<TIFactionState>();
			if (args.Length != 0)
			{
				TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
				if (tifactionState != null)
				{
					list.Add(tifactionState);
				}
			}
			if (list.Count == 0)
			{
				list.AddRange(GameStateManager.AllFactions());
			}
			foreach (TIFactionState tifactionState2 in list)
			{
				foreach (TISpaceShipTemplate tispaceShipTemplate in tifactionState2.shipDesigns)
				{
					TIFactionState.LogAI(tispaceShipTemplate.DebugSummary(), false);
				}
			}
		}

		// Token: 0x060058C4 RID: 22724 RVA: 0x0028ACA0 File Offset: 0x00288EA0
		private void SetFaction(string[] args)
		{
			TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
			if (tifactionState != null && tifactionState.player.isAI)
			{
				GameControl.SetActivePlayer(tifactionState);
				World.Active.GetExistingManager<CanvasManager>().ResetActivePlayerDuringRunTime();
				SpaceObjectSymbolController[] array = global::UnityEngine.Object.FindObjectsOfType<SpaceObjectSymbolController>(true);
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActivePlayer();
				}
				SingleMarkerController[] array2 = global::UnityEngine.Object.FindObjectsOfType<SingleMarkerController>(true);
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].SetActivePlayer(false);
				}
				HabSiteController[] array3 = global::UnityEngine.Object.FindObjectsOfType<HabSiteController>(true);
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].SetActivePlayer(false);
				}
				return;
			}
			this.terminalController.OutputError("Could not find AI faction " + args[0]);
		}

		// Token: 0x060058C5 RID: 22725 RVA: 0x0028AD58 File Offset: 0x00288F58
		private void DetainCouncilor(string[] args)
		{
			TIFactionState tifactionState = null;
			if (args.Length >= 1)
			{
				tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
			}
			if (tifactionState == null)
			{
				tifactionState = GameControl.control.activePlayer;
			}
			if (!(tifactionState != null))
			{
				this.terminalController.OutputError("No faction selected");
				return;
			}
			if (GeneralControlsController.UISelectedAssetState.isCouncilorState && tifactionState != GeneralControlsController.UISelectedAssetState.ref_faction)
			{
				GeneralControlsController.UISelectedAssetState.ref_councilor.DetainCouncilor(tifactionState, 2f, 1f, true);
				return;
			}
			if (GeneralControlsController.UIOtherSelectedState.isCouncilorState && tifactionState != GeneralControlsController.UIOtherSelectedState.ref_faction)
			{
				GeneralControlsController.UIOtherSelectedState.ref_councilor.DetainCouncilor(tifactionState, 2f, 1f, true);
				return;
			}
			this.terminalController.OutputError("No councilor targeted");
		}

		// Token: 0x060058C6 RID: 22726 RVA: 0x0028AE30 File Offset: 0x00289030
		private void AddTrait(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("No trait template name specified");
				return;
			}
			TITraitTemplate titraitTemplate = TemplateManager.Find<TITraitTemplate>(args[0], false);
			if (titraitTemplate == null)
			{
				this.terminalController.OutputError("No trait found");
				return;
			}
			TICouncilorState ticouncilorState = GeneralControlsController.UISelectedAssetState as TICouncilorState;
			if (ticouncilorState == null)
			{
				ticouncilorState = GeneralControlsController.UIOtherSelectedState as TICouncilorState;
			}
			if (ticouncilorState != null)
			{
				ticouncilorState.AddTrait(titraitTemplate, false);
				return;
			}
			this.terminalController.OutputError("No councilor selected");
		}

		// Token: 0x060058C7 RID: 22727 RVA: 0x0028AEB8 File Offset: 0x002890B8
		private void RemoveTrait(string[] args)
		{
			if (args.Length >= 1)
			{
				TITraitTemplate titraitTemplate = TemplateManager.Find<TITraitTemplate>(args[0], false);
				if (titraitTemplate != null)
				{
					TICouncilorState ticouncilorState = GeneralControlsController.UISelectedAssetState as TICouncilorState;
					if (ticouncilorState == null)
					{
						ticouncilorState = GeneralControlsController.UIOtherSelectedState as TICouncilorState;
					}
					if (!(ticouncilorState != null))
					{
						this.terminalController.OutputError("No councilor selected");
						return;
					}
					if (!ticouncilorState.RemoveTrait(titraitTemplate))
					{
						this.terminalController.OutputError("Trait not found on councilor");
						return;
					}
				}
				else
				{
					this.terminalController.OutputError("No trait found");
				}
				return;
			}
			this.terminalController.OutputError("No trait template name specified");
		}

		// Token: 0x060058C8 RID: 22728 RVA: 0x0028AF50 File Offset: 0x00289150
		private void SightAlienMissions(string[] args)
		{
			foreach (TICouncilorState ticouncilorState in GameStateManager.AlienFaction().councilors)
			{
				if (ticouncilorState.OnEarth && ticouncilorState.HasMission)
				{
					ticouncilorState.ref_region.ref_regionAlienActivity.ActivitySightedByFaction(GameControl.control.activePlayer, ticouncilorState.activeMission.missionTemplate, ticouncilorState.activeMission.target.ref_councilor, ticouncilorState.activeMission.target.ref_faction, ticouncilorState.activeMission);
				}
			}
		}

		// Token: 0x060058C9 RID: 22729 RVA: 0x0028AFFC File Offset: 0x002891FC
		private void GiveXP(string[] args)
		{
			int num = 0;
			if (args.Length >= 1)
			{
				num = int.Parse(args[0]);
			}
			TICouncilorState ticouncilorState = GeneralControlsController.UISelectedAssetState as TICouncilorState;
			if (ticouncilorState == null)
			{
				ticouncilorState = GeneralControlsController.UIOtherSelectedState as TICouncilorState;
			}
			if (ticouncilorState != null)
			{
				ticouncilorState.ChangeXP(num);
			}
		}

		// Token: 0x0400406E RID: 16494
		private TerminalController terminalController;
	}
}
