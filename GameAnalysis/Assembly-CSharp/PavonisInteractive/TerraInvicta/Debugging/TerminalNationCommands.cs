using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000917 RID: 2327
	public class TerminalNationCommands
	{
		// Token: 0x060058EB RID: 22763 RVA: 0x0028C4BA File Offset: 0x0028A6BA
		public TerminalNationCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x060058EC RID: 22764 RVA: 0x0028C4D0 File Offset: 0x0028A6D0
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("ModifyGDP", new CommandHandler(this.ModifyGDP), "Add nation and GDP change in billions: 'ModifyGDP Afghanistan,3'");
			this.terminalController.RegisterCommand("GiveCP", new CommandHandler(this.GiveCP), "Add nation and faction: 'GiveCP Afghanistan,ResistCouncil'");
			this.terminalController.RegisterCommand("GiveAllCPs", new CommandHandler(this.GiveAllCPs), "Add nation and faction: 'GiveAllCPs Afghanistan,ResistCouncil'");
			this.terminalController.RegisterCommand("Revolution", new CommandHandler(this.Revolution), "Trigger a revolution in the selected nation");
			this.terminalController.RegisterCommand("RegimeChange", new CommandHandler(this.RegimeChange), "Trigger a regime change in the selected nation");
			this.terminalController.RegisterCommand("StartCoup", new CommandHandler(this.Coup), "Trigger a coup in the selected nation");
			this.terminalController.RegisterCommand("Secede", new CommandHandler(this.Secede), "Trigger nation to secede");
			this.terminalController.RegisterCommand("IndependenceDay", new CommandHandler(this.IndependenceDay), "Secede capital of all nonextant nations");
			this.terminalController.RegisterCommand("Absorb", new CommandHandler(this.AbsorbNation), "Trigger nation to absorb: absorbing, absorbed datanames");
			this.terminalController.RegisterCommand("Unify", new CommandHandler(this.Unify), "Trigger nation to unify with: absorbing, absorbed datanames");
			this.terminalController.RegisterCommand("SetMiltech", new CommandHandler(this.SetMiltech), "Will set selected nation's miltech to value, 'SetMiltech 7.5' or 'SetMiltech 3.2,USA'");
			this.terminalController.RegisterCommand("Nuke", new CommandHandler(this.Nuke), "Nuke Selected Region");
			this.terminalController.RegisterCommand("GiveIPs", new CommandHandler(this.GiveIPs), "Will give # of IPs to selected nation: GiveIPs BuildArmy,20");
			this.terminalController.RegisterCommand("SetArmyHealth", new CommandHandler(this.SetArmyHealth), "Set selected army's health to value");
			this.terminalController.RegisterCommand("ArmyGoHome", new CommandHandler(this.ArmyGoHome), "Teleport selected army to its home region");
			this.terminalController.RegisterCommand("TransferRegion", new CommandHandler(this.TransferRegion), "Transfer selected region to nationTemplateName");
			this.terminalController.RegisterCommand("SpawnLanding", new CommandHandler(this.SpawnLanding), "Spawn Alien Landing in selected region");
			this.terminalController.RegisterCommand("SpawnXenofauna", new CommandHandler(this.SpawnXenofauna), "Spawn Alien Megafauna in selected region");
			this.terminalController.RegisterCommand("PeaceOut", new CommandHandler(this.PeaceOut), "End all wars involving the selected nation");
			this.terminalController.RegisterCommand("DeclareWar", new CommandHandler(this.DeclareWar), "Declare war with selected nation on dataName/displayName");
			this.terminalController.RegisterCommand("ChangeUnrest", new CommandHandler(this.ChangeUnrest), "Add Unrest value in selected nation");
			this.terminalController.RegisterCommand("GiveMeAllCPs", new CommandHandler(this.GiveMeAllCPs), "Give all CPs to the activeplayer");
			this.terminalController.RegisterCommand("RandomizeAllCPs", new CommandHandler(this.RandomizeAllCPs), "Randomize CP ownership globally");
			this.terminalController.RegisterCommand("SetSustainability", new CommandHandler(this.SetSustainability), "Set National Sustainability for selected nation");
			this.terminalController.RegisterCommand("SetNuclearWeapons", new CommandHandler(this.SetNuclearWeapons), "Set # nuclear weapons for selecting nation");
			this.terminalController.RegisterCommand("AddSTO", new CommandHandler(this.AddSTOFighter), "Give selected nation 1+ STO Fighters");
			this.terminalController.RegisterCommand("Occupy", new CommandHandler(this.Occupy), "Set all of selected nation's armies' regions to 100% Occupation");
			this.terminalController.RegisterCommand("SetOccupied", new CommandHandler(this.SetOccupied), "Set selected region to 100% Occupation by nation's war enemy");
			this.terminalController.RegisterCommand("ClearRelationsCooldowns", new CommandHandler(this.ClearRelationsCooldowns), "Clear improve relations cooldowns for all extant nations");
			this.terminalController.RegisterCommand("TakeAllClaims", new CommandHandler(this.TakeAllClaims), "Annex all claimed regions to selected nation");
		}

		// Token: 0x060058ED RID: 22765 RVA: 0x0028C8BC File Offset: 0x0028AABC
		private void TakeAllClaims(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			if (ref_nation != null)
			{
				foreach (TIRegionState tiregionState in ref_nation.claims)
				{
					if (!ref_nation.regions.Contains(tiregionState))
					{
						tiregionState.nation.TransferRegionsControlTo(new List<TIRegionState> { tiregionState }, ref_nation, false, true, false, false, false);
					}
				}
			}
		}

		// Token: 0x060058EE RID: 22766 RVA: 0x0028C948 File Offset: 0x0028AB48
		private void Occupy(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			if (ref_nation != null)
			{
				foreach (TIArmyState tiarmyState in ref_nation.armies)
				{
					if (tiarmyState.OccupyingRegion(true))
					{
						tiarmyState.currentRegion.IncreaseOccupationValue(tiarmyState.homeNation, 1f, tiarmyState);
					}
				}
			}
		}

		// Token: 0x060058EF RID: 22767 RVA: 0x0028C9C8 File Offset: 0x0028ABC8
		private void SetOccupied(string[] args)
		{
			TIRegionState ref_region = GeneralControlsController.UIOtherSelectedState.ref_region;
			if (ref_region.nation.wars.Count > 0)
			{
				TINationState tinationState = ref_region.nation.wars.FirstOrDefault<TINationState>((TINationState x) => x.standardArmies.Count > 0);
				if (tinationState == null)
				{
					tinationState = ref_region.nation.wars.SelectRandomItem<TINationState>();
				}
				if (tinationState != null)
				{
					ref_region.IncreaseOccupationValue(tinationState, 1f, null);
					return;
				}
			}
			else
			{
				this.terminalController.OutputError("Select a region whose nation is at war");
			}
		}

		// Token: 0x060058F0 RID: 22768 RVA: 0x0028CA60 File Offset: 0x0028AC60
		private void AddSTOFighter(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			if (ref_nation != null)
			{
				int num = 1;
				if (args.Length >= 1)
				{
					int.TryParse(args[0], out num);
				}
				for (int i = 0; i < num; i++)
				{
					ref_nation.OnBuildSTOSquadronPriorityComplete();
					TIFactionState executiveFaction = ref_nation.executiveFaction;
					if (executiveFaction != null)
					{
						executiveFaction.CacheSTOFighterMass();
					}
				}
				return;
			}
			this.terminalController.OutputError("Select a nation and optionally enter a value to increase STO fighters in the nation");
		}

		// Token: 0x060058F1 RID: 22769 RVA: 0x0028CAC8 File Offset: 0x0028ACC8
		private void SetNuclearWeapons(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			if (ref_nation != null && args.Length >= 1)
			{
				int num;
				if (int.TryParse(args[0], out num))
				{
					ref_nation.ChangeNumNuclearWeapons(num - ref_nation.numNuclearWeapons);
					return;
				}
			}
			else
			{
				this.terminalController.OutputError("Select a nation and enter a value to set #nukes");
			}
		}

		// Token: 0x060058F2 RID: 22770 RVA: 0x0028CB1C File Offset: 0x0028AD1C
		private void ChangeUnrest(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			float num;
			if (ref_nation != null && args.Length >= 1 && float.TryParse(args[0], out num))
			{
				ref_nation.AddToUnrest(num, TINationState.UnrestChangeReason.UnrestReason_EventEffect, 10f);
				return;
			}
			this.terminalController.OutputError("Select a nation and enter a value to increase unrest");
		}

		// Token: 0x060058F3 RID: 22771 RVA: 0x0028CB70 File Offset: 0x0028AD70
		private void DeclareWar(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			if (ref_nation != null && args.Length >= 1)
			{
				TINationState tinationState = GameStateManager.FindByTemplate<TINationState>(args[0], false);
				if (tinationState == null)
				{
					tinationState = GameStateManager.AllNations().FirstOrDefault<TINationState>((TINationState x) => x.displayName == args[0]);
				}
				if (tinationState != null)
				{
					if (ref_nation.federation != null && ref_nation.federation == tinationState.federation)
					{
						ref_nation.LeaveFederation(null, true);
					}
					ref_nation.EndAlliance(null, tinationState);
					ref_nation.DeclareFullWar(ref_nation.executiveFaction, tinationState);
					return;
				}
			}
			this.terminalController.OutputError("Requires selected nation and valid target nation.");
		}

		// Token: 0x060058F4 RID: 22772 RVA: 0x0028CC34 File Offset: 0x0028AE34
		private void PeaceOut(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			if (ref_nation != null)
			{
				foreach (TIWarState tiwarState in ref_nation.currentWarStates)
				{
					ref_nation.WhitePeace(ref_nation.executiveFaction, tiwarState, true);
				}
			}
		}

		// Token: 0x060058F5 RID: 22773 RVA: 0x0028CCA4 File Offset: 0x0028AEA4
		private void SpawnLanding(string[] args)
		{
			TIRegionState ref_region = GeneralControlsController.UIOtherSelectedState.ref_region;
			if (ref_region == null)
			{
				return;
			}
			ref_region.alienLanding.TriggerLanding(-1f);
		}

		// Token: 0x060058F6 RID: 22774 RVA: 0x0028CCC4 File Offset: 0x0028AEC4
		private void SpawnXenofauna(string[] args)
		{
			TIRegionState ref_region = GeneralControlsController.UIOtherSelectedState.ref_region;
			if (ref_region == null)
			{
				return;
			}
			ref_region.xenoforming.SpawnMegafaunaArmy();
		}

		// Token: 0x060058F7 RID: 22775 RVA: 0x0028CCE0 File Offset: 0x0028AEE0
		private void TransferRegion(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("Requires nation name.");
				return;
			}
			TIRegionState ref_region = GeneralControlsController.UIOtherSelectedState.ref_region;
			if (!(ref_region != null))
			{
				this.terminalController.OutputError("Requires region selected.");
				return;
			}
			TINationState tinationState = GameStateManager.IterateByClass<TINationState>(false).FirstOrDefault<TINationState>((TINationState x) => x.displayName == args[0] || x.templateName == args[0]);
			if (tinationState != null)
			{
				ref_region.nation.TransferRegionsControlTo(new List<TIRegionState> { ref_region }, tinationState, false, true, false, false, false);
				return;
			}
			this.terminalController.OutputError("Requires nation name.");
		}

		// Token: 0x060058F8 RID: 22776 RVA: 0x0028CD8C File Offset: 0x0028AF8C
		private void GiveCP(string[] args)
		{
			if (args.Length < 2)
			{
				this.terminalController.OutputError("Requires nation first and faction second, separated by a comma. Will give first open control point if available, otherwise will give executive control point.");
				return;
			}
			TINationState tinationState = GameStateManager.IterateByClass<TINationState>(false).FirstOrDefault<TINationState>((TINationState x) => x.displayName == args[0] || x.templateName == args[0]);
			if (!(tinationState != null))
			{
				this.terminalController.OutputError("Nation not found: " + args[0]);
				return;
			}
			TIFactionState tifactionState = GameStateManager.IterateByClass<TIFactionState>(false).FirstOrDefault<TIFactionState>((TIFactionState x) => x.displayName == args[1] || x.templateName == args[1]);
			if (tifactionState != null)
			{
				TIControlPoint ticontrolPoint = tinationState.FirstNativeControlPoint();
				int num = ((ticontrolPoint != null) ? ticontrolPoint.positionInNation : tinationState.maxControlPointIndex);
				tinationState.ChangeControlPointOwner(num, ControlPointChangeCause.None, tifactionState);
				return;
			}
			this.terminalController.OutputError("Faction not found: " + args[1]);
		}

		// Token: 0x060058F9 RID: 22777 RVA: 0x0028CE64 File Offset: 0x0028B064
		private void GiveAllCPs(string[] args)
		{
			TINationState tinationState = GameStateManager.IterateByClass<TINationState>(false).FirstOrDefault<TINationState>((TINationState x) => x.displayName == args[0] || x.templateName == args[0]);
			TIFactionState tifactionState = GameControl.control.activePlayer;
			if (tinationState == null)
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				tinationState = ((uiotherSelectedState != null) ? uiotherSelectedState.ref_nation : null);
			}
			if (tinationState != null)
			{
				if (args.Length == 2)
				{
					tifactionState = GameStateManager.IterateByClass<TIFactionState>(false).FirstOrDefault<TIFactionState>((TIFactionState x) => x.displayName == args[1] || x.templateName == args[1]);
				}
				if (tifactionState != null)
				{
					using (List<TIControlPoint>.Enumerator enumerator = tinationState.controlPoints.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIControlPoint ticontrolPoint = enumerator.Current;
							tinationState.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.None, tifactionState);
						}
						return;
					}
				}
				this.terminalController.OutputError("Faction not found.");
				return;
			}
			this.terminalController.OutputError("Nation not found.");
		}

		// Token: 0x060058FA RID: 22778 RVA: 0x0028CF64 File Offset: 0x0028B164
		private void GiveMeAllCPs(string[] args)
		{
			foreach (TINationState tinationState in GameStateManager.AllExtantHumanNations())
			{
				foreach (TIControlPoint ticontrolPoint in tinationState.controlPoints)
				{
					tinationState.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.None, GameControl.control.activePlayer);
				}
			}
		}

		// Token: 0x060058FB RID: 22779 RVA: 0x0028CFFC File Offset: 0x0028B1FC
		private void RandomizeAllCPs(string[] args)
		{
			foreach (TINationState tinationState in GameStateManager.AllExtantHumanNations())
			{
				foreach (TIControlPoint ticontrolPoint in tinationState.controlPoints)
				{
					TIFactionState tifactionState = GameStateManager.AllHumanFactions().SelectRandomItem<TIFactionState>();
					tinationState.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.None, tifactionState);
				}
			}
		}

		// Token: 0x060058FC RID: 22780 RVA: 0x0028D098 File Offset: 0x0028B298
		private void SetSustainability(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			float num;
			if (ref_nation != null && float.TryParse(args[0], out num))
			{
				ref_nation.SetSustainability(num, false);
			}
		}

		// Token: 0x060058FD RID: 22781 RVA: 0x0028D0D0 File Offset: 0x0028B2D0
		private void ModifyGDP(string[] args)
		{
			if (args.Length < 2)
			{
				this.terminalController.OutputError("Requires nation first and value in billions second, separated by a comma.");
				return;
			}
			TINationState tinationState = GameStateManager.IterateByClass<TINationState>(false).FirstOrDefault<TINationState>((TINationState x) => x.displayName == args[0] || x.templateName == args[0]);
			if (!(tinationState != null))
			{
				this.terminalController.OutputError("Could not get nation from " + args[0]);
				return;
			}
			float num;
			if (float.TryParse(args[1], out num))
			{
				tinationState.ModifyGDP((double)num * 1000000000.0, TINationState.GDPChangeReason.GDPReason_EventEffect);
				return;
			}
			this.terminalController.OutputError("Could not get value from " + args[1]);
		}

		// Token: 0x060058FE RID: 22782 RVA: 0x0028D188 File Offset: 0x0028B388
		private void Coup(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			if (ref_nation != null)
			{
				ref_nation.Coup(null, 0);
				return;
			}
			this.terminalController.OutputError("Select a nation");
		}

		// Token: 0x060058FF RID: 22783 RVA: 0x0028D1C4 File Offset: 0x0028B3C4
		private void Revolution(string[] args)
		{
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			TINationState tinationState;
			if ((tinationState = ((uiotherSelectedState != null) ? uiotherSelectedState.ref_nation : null)) == null)
			{
				TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
				tinationState = ((uiselectedAssetState != null) ? uiselectedAssetState.ref_nation : null);
			}
			TINationState tinationState2 = tinationState;
			if (tinationState2 != null)
			{
				tinationState2.Revolution();
				return;
			}
			this.terminalController.OutputError("Select a nation");
		}

		// Token: 0x06005900 RID: 22784 RVA: 0x0028D218 File Offset: 0x0028B418
		private void RegimeChange(string[] args)
		{
			TINationState ref_nation = GeneralControlsController.UIOtherSelectedState.ref_nation;
			if (ref_nation != null)
			{
				TIArmyState tiarmyState;
				if ((tiarmyState = GameControl.control.activePlayer.armies.FirstOrDefault<TIArmyState>()) == null)
				{
					tiarmyState = (from x in GameStateManager.IterateByClass<TIArmyState>(false)
						where x.armyType == ArmyType.Human
						select x).SelectRandomItem<TIArmyState>();
				}
				TIArmyState tiarmyState2 = tiarmyState;
				ref_nation.RegimeChange(tiarmyState2.homeNation, new List<TINationState> { tiarmyState2.homeNation }, tiarmyState2);
				return;
			}
			this.terminalController.OutputError("Select a nation");
		}

		// Token: 0x06005901 RID: 22785 RVA: 0x0028D2B0 File Offset: 0x0028B4B0
		private void ClearRelationsCooldowns(string[] args)
		{
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				tinationState.ClearRelationsCooldowns();
			}
		}

		// Token: 0x06005902 RID: 22786 RVA: 0x0028D2FC File Offset: 0x0028B4FC
		private void Secede(string[] args)
		{
			TINationState nation = GameStateManager.FindByTemplate<TINationState>(args[0], false);
			if (!(nation != null) || nation.extant)
			{
				this.terminalController.OutputError("Requires dataname for non-existing nation");
				return;
			}
			TIBilateralTemplate tibilateralTemplate = TemplateManager.IterateByClass<TIBilateralTemplate>(true).FirstOrDefault<TIBilateralTemplate>((TIBilateralTemplate x) => x.BilateralIsInScenario() && x.capitalClaim && x.nationState1 == nation);
			TIRegionState tiregionState = ((tibilateralTemplate != null) ? tibilateralTemplate.regionState1 : null) ?? null;
			if (tiregionState != null)
			{
				tiregionState.nation.Secession(GameControl.control.activePlayer, nation, new List<TIRegionState> { tiregionState }, null);
				return;
			}
			this.terminalController.OutputError("Could not find capital claim");
		}

		// Token: 0x06005903 RID: 22787 RVA: 0x0028D3B8 File Offset: 0x0028B5B8
		private void IndependenceDay(string[] args)
		{
			using (IEnumerator<TINationState> enumerator = GameStateManager.AllNonExtantHumanNations().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TINationState nation = enumerator.Current;
					if (nation.capital == null)
					{
						TIBilateralTemplate tibilateralTemplate = TemplateManager.IterateByClass<TIBilateralTemplate>(true).FirstOrDefault<TIBilateralTemplate>((TIBilateralTemplate x) => x.BilateralIsInScenario() && x.capitalClaim && x.nationState1 == nation);
						if (tibilateralTemplate != null && tibilateralTemplate.projectUnlock != null)
						{
							GameControl.control.activePlayer.OnProjectComplete(tibilateralTemplate.projectUnlock, -1, true, false);
						}
					}
					TIRegionState tiregionState = nation.capital ?? nation.originalCapital;
					if (tiregionState != null && (tiregionState != tiregionState.nation.capital || tiregionState.nation.regions.Count > 0))
					{
						tiregionState.nation.Secession(GameControl.control.activePlayer, nation, new List<TIRegionState> { tiregionState }, null);
					}
				}
			}
		}

		// Token: 0x06005904 RID: 22788 RVA: 0x0028D4D0 File Offset: 0x0028B6D0
		private void AbsorbNation(string[] args)
		{
			if (args.Length < 2)
			{
				this.terminalController.OutputError("not enough datanames");
				return;
			}
			TINationState tinationState = GameStateManager.FindByTemplate<TINationState>(args[0], false);
			TINationState tinationState2 = GameStateManager.FindByTemplate<TINationState>(args[1], false);
			if (tinationState != null && tinationState2 != null)
			{
				tinationState.AbsorbNation(GameControl.control.activePlayer, tinationState2);
				return;
			}
			this.terminalController.OutputError("bad datanames");
		}

		// Token: 0x06005905 RID: 22789 RVA: 0x0028D53C File Offset: 0x0028B73C
		private void Unify(string[] args)
		{
			if (args.Length < 2)
			{
				this.terminalController.OutputError("not enough datanames");
				return;
			}
			TINationState tinationState = GameStateManager.FindByTemplate<TINationState>(args[0], false);
			TINationState tinationState2 = GameStateManager.FindByTemplate<TINationState>(args[1], false);
			if (tinationState != null && tinationState2 != null)
			{
				new UnificationOption().EnactPolicy(tinationState, tinationState2);
				return;
			}
			this.terminalController.OutputError("bad datanames");
		}

		// Token: 0x06005906 RID: 22790 RVA: 0x0028D5A4 File Offset: 0x0028B7A4
		private void SetMiltech(string[] args)
		{
			TINationState tinationState = null;
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState != null && uiselectedAssetState.isArmyState)
			{
				tinationState = GeneralControlsController.UISelectedAssetState.ref_army.homeNation;
			}
			if (tinationState == null)
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				if (uiotherSelectedState != null && (uiotherSelectedState.isArmyState || uiotherSelectedState.isNationState || uiotherSelectedState.isRegionState))
				{
					tinationState = GeneralControlsController.UIOtherSelectedState.ref_nation;
				}
				if (tinationState == null)
				{
					tinationState = GameStateManager.FindByTemplate<TINationState>(args[1], false);
				}
			}
			if (tinationState != null)
			{
				float militaryTechLevel = tinationState.militaryTechLevel;
				float floatValue = TIUtilities.GetFloatValue(args[0]);
				float num = floatValue - militaryTechLevel;
				if (floatValue > tinationState.maxMilitaryTechLevel)
				{
					float num2 = floatValue - tinationState.maxMilitaryTechLevel;
					tinationState.AddToMaxMilitaryTechLevel(num2);
				}
				tinationState.AddToMilitaryTechLevel(num);
			}
		}

		// Token: 0x06005907 RID: 22791 RVA: 0x0028D668 File Offset: 0x0028B868
		private void Nuke(string[] args)
		{
			TIRegionState tiregionState = null;
			if (GeneralControlsController.UIOtherSelectedState != null)
			{
				tiregionState = GeneralControlsController.UIOtherSelectedState.ref_region;
			}
			if (tiregionState == null && GeneralControlsController.UISelectedAssetState != null)
			{
				tiregionState = GeneralControlsController.UISelectedAssetState.ref_region;
			}
			if (tiregionState != null)
			{
				tiregionState.OnNuclearAttackArrives(GameControl.control.activePlayer, null);
				return;
			}
			this.terminalController.OutputError("Requires you have something selected attached to a region");
		}

		// Token: 0x06005908 RID: 22792 RVA: 0x0028D6DC File Offset: 0x0028B8DC
		private void GiveIPs(string[] args)
		{
			TINationState tinationState = null;
			if (GeneralControlsController.UIOtherSelectedState != null && (GeneralControlsController.UIOtherSelectedState.isRegionState || GeneralControlsController.UIOtherSelectedState.isNationState))
			{
				tinationState = GeneralControlsController.UIOtherSelectedState.ref_region.nation;
			}
			if (tinationState != null)
			{
				if (args.Length >= 2)
				{
					PriorityType priorityType = args[0].ToEnum(PriorityType.Economy);
					float num;
					if (float.TryParse(args[1], out num))
					{
						tinationState.DirectInvestment(priorityType, num);
					}
					else
					{
						this.terminalController.OutputError("Bad Value");
					}
					tinationState.ProcessPrioritySpending();
					tinationState.SetDataDirty();
					return;
				}
				float num2;
				if (args.Length == 1 && float.TryParse(args[0], out num2))
				{
					for (int i = 0; i < Enums.PriorityTypes.Length; i++)
					{
						PriorityType priorityType2 = Enums.PriorityTypes[i];
						tinationState.ModifyAccumulatedInvestment(priorityType2, num2, false, false);
					}
					tinationState.ProcessPrioritySpending();
					tinationState.SetDataDirty();
					return;
				}
			}
			else
			{
				this.terminalController.OutputError("Select Nation, enter priority name, amount");
			}
		}

		// Token: 0x06005909 RID: 22793 RVA: 0x0028D7CC File Offset: 0x0028B9CC
		private void SetArmyHealth(string[] args)
		{
			TIArmyState tiarmyState = null;
			float num = 1f;
			float num2;
			if (args.Length == 1 && float.TryParse(args[0], out num2))
			{
				num = Mathf.Clamp(num2, 0f, 1f);
			}
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState != null && uiselectedAssetState.isArmyState)
			{
				tiarmyState = GeneralControlsController.UISelectedAssetState.ref_army;
			}
			else
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				if (uiotherSelectedState != null && uiotherSelectedState.isArmyState)
				{
					tiarmyState = GeneralControlsController.UIOtherSelectedState.ref_army;
				}
			}
			if (tiarmyState != null)
			{
				tiarmyState.SetStrength(num);
				return;
			}
			this.terminalController.OutputError("Select An Army");
		}

		// Token: 0x0600590A RID: 22794 RVA: 0x0028D864 File Offset: 0x0028BA64
		private void ArmyGoHome(string[] args)
		{
			TIArmyState tiarmyState = null;
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			if (uiselectedAssetState != null && uiselectedAssetState.isArmyState)
			{
				tiarmyState = GeneralControlsController.UISelectedAssetState.ref_army;
			}
			else
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				if (uiotherSelectedState != null && uiotherSelectedState.isArmyState)
				{
					tiarmyState = GeneralControlsController.UIOtherSelectedState.ref_army;
				}
			}
			if (tiarmyState != null)
			{
				tiarmyState.GoHome();
				return;
			}
			this.terminalController.OutputError("Select An Army");
		}

		// Token: 0x04004074 RID: 16500
		private TerminalController terminalController;
	}
}
