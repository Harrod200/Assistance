using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000919 RID: 2329
	public class TerminalResourceCommands
	{
		// Token: 0x06005915 RID: 22805 RVA: 0x0028DBE3 File Offset: 0x0028BDE3
		public TerminalResourceCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x06005916 RID: 22806 RVA: 0x0028DBF8 File Offset: 0x0028BDF8
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("dumpTime", new CommandHandler(this.DumpPendingTimeEvents), "Dump Pending Time Events");
			this.terminalController.RegisterCommand("addresource", new CommandHandler(this.AddResource), "Add amount to council resource by name eg; 'addresource Money,100,SubmitCouncil'");
			this.terminalController.RegisterCommand("giveresource", new CommandHandler(this.AddResource), "Add amount to council resource by name eg; 'giveresource Money,100,SubmitCouncil'");
			this.terminalController.RegisterCommand("addcouncilresources", new CommandHandler(this.AddCouncilResources), "Add amount to all council resources eg; 'addcouncilresources 100'");
			this.terminalController.RegisterCommand("addspaceresources", new CommandHandler(this.AddSpaceResources), "Add amount to all space resources eg; 'addspaceresources 100'");
			this.terminalController.RegisterCommand("prospect", new CommandHandler(this.Prospect), "Prospect Spacebody, 'Prospect Ganymede'");
			this.terminalController.RegisterCommand("revealsites", new CommandHandler(this.RevealSites), "Reveals all hab site data'");
			this.terminalController.RegisterCommand("alieninfo", new CommandHandler(this.AlienInfo), "See alien resources and saving target");
			this.terminalController.RegisterCommand("dumpfleetgoal", new CommandHandler(this.DumpFleetGoal), "Log selected goal for AI Fleet");
			this.terminalController.RegisterCommand("propaganda", new CommandHandler(this.PR), "Propaganda on nation(s), optional strength and nation 'propaganda ResistCouncil (10) (GBR)");
			this.terminalController.RegisterCommand("aerosols", new CommandHandler(this.AddAerosols), "Add Aerosols to atmosphere in ppm, 'aerosols .05'");
			this.terminalController.RegisterCommand("addco2", new CommandHandler(this.AddCO2), "Add CO2 to atmosphere in ppm, 'addco2 50'");
			this.terminalController.RegisterCommand("givemetonsofresources", new CommandHandler(this.GiveTonsOfResources), "Give a lot of everything to player faction");
		}

		// Token: 0x06005917 RID: 22807 RVA: 0x0028DDB4 File Offset: 0x0028BFB4
		public void GiveTonsOfResources(string[] args)
		{
			float num = 100000000f;
			if (args.Length != 0)
			{
				float.TryParse(args[0], out num);
				if (num == 0f)
				{
					num = 100000000f;
				}
			}
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Money, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Influence, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Operations, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Research, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Projects, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Boost, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.MissionControl, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Water, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Metals, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.NobleMetals, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Volatiles, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Fissiles, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Antimatter, false, null);
			GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Exotics, false, null);
		}

		// Token: 0x06005918 RID: 22808 RVA: 0x0028DF04 File Offset: 0x0028C104
		public void AddResource(string[] args)
		{
			if (args.Length <= 2)
			{
				this.terminalController.OutputError("Requires Resource,Amount,Faction");
				return;
			}
			float num;
			if (!float.TryParse(args[1], out num))
			{
				this.terminalController.OutputError("add resource error: couldn't parse amount: " + args[1]);
				return;
			}
			FactionResource factionResource;
			if (!Enum.TryParse<FactionResource>(args[0], out factionResource))
			{
				this.terminalController.OutputError("add resource error: couldn't parse resource: " + args[0]);
				return;
			}
			if (args.Length <= 1 || !(args[2] != string.Empty))
			{
				GameControl.control.activePlayer.AddToCurrentResource(num, factionResource, false, null);
				return;
			}
			TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[2], false);
			if (tifactionState == null)
			{
				this.terminalController.OutputError("add resource error: couldn't parse faction dataname: " + args[2]);
				return;
			}
			if (factionResource == FactionResource.MissionControl)
			{
				tifactionState.ChangeBaseResourceIncome(FactionResource.MissionControl, num);
				return;
			}
			tifactionState.AddToCurrentResource(num, factionResource, false, null);
		}

		// Token: 0x06005919 RID: 22809 RVA: 0x0028DFE4 File Offset: 0x0028C1E4
		public void AddCouncilResources(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("add council resources error: missing amount");
				return;
			}
			float num;
			if (float.TryParse(args[0], out num))
			{
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Money, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Influence, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Operations, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Research, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Projects, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Boost, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.MissionControl, false, null);
				return;
			}
			this.terminalController.OutputError("add council resources error: couldn't parse amount: " + args[0]);
		}

		// Token: 0x0600591A RID: 22810 RVA: 0x0028E0BC File Offset: 0x0028C2BC
		public void AddSpaceResources(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("add space resources error: missing amount");
				return;
			}
			float num;
			if (float.TryParse(args[0], out num))
			{
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Water, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Metals, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.NobleMetals, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Volatiles, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Fissiles, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Antimatter, false, null);
				GameControl.control.activePlayer.AddToCurrentResource(num, FactionResource.Exotics, false, null);
				return;
			}
			this.terminalController.OutputError("add space resources error: couldn't parse amount: " + args[1]);
		}

		// Token: 0x0600591B RID: 22811 RVA: 0x0028E19C File Offset: 0x0028C39C
		public void Prospect(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("need spacebody");
				return;
			}
			TISpaceBodyState tispaceBodyState = GameStateManager.IterateByClass<TISpaceBodyState>(false).FirstOrDefault<TISpaceBodyState>((TISpaceBodyState x) => x.displayName == args[0] || x.templateName == args[0]);
			if (tispaceBodyState != null)
			{
				GameControl.control.activePlayer.ProspectSpaceBody(tispaceBodyState);
				TINotificationQueueState.LogProbeArrived(GameControl.control.activePlayer, tispaceBodyState);
				return;
			}
			this.terminalController.OutputError("bad spacebody");
		}

		// Token: 0x0600591C RID: 22812 RVA: 0x0028E224 File Offset: 0x0028C424
		public void RevealSites(string[] args)
		{
			foreach (TISpaceBodyState tispaceBodyState in GameStateManager.IterateByClass<TISpaceBodyState>(false))
			{
				GameControl.control.activePlayer.ProspectSpaceBody(tispaceBodyState);
			}
		}

		// Token: 0x0600591D RID: 22813 RVA: 0x0028E27C File Offset: 0x0028C47C
		public void AlienInfo(string[] args)
		{
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			TIGlobalConfig global = TemplateManager.global;
			AISavingData aisavingTarget = tifactionState.AISavingTarget;
			this.terminalController.Output(string.Concat(new string[]
			{
				global.moneyInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Money).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Money, false, false).ToString(),
				") ",
				global.influenceInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Influence).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Influence, false, false).ToString(),
				") ",
				global.opsInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Operations).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Operations, false, false).ToString(),
				") ",
				global.boostInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Boost).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Boost, false, false).ToString(),
				") ",
				global.missionControlInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.MissionControl).ToString(),
				" ",
				global.waterInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Water).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Water, false, false).ToString(),
				") ",
				global.volatilesInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Volatiles).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Volatiles, false, false).ToString(),
				") ",
				global.metalsInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Metals).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Metals, false, false).ToString(),
				") ",
				global.noblesInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.NobleMetals).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.NobleMetals, false, false).ToString(),
				") ",
				global.fissilesInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Fissiles).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Fissiles, false, false).ToString(),
				") ",
				global.antimatterInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Antimatter).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Antimatter, false, false).ToString(),
				") ",
				global.exoticsInlineSpritePath,
				tifactionState.GetCurrentResourceAmount(FactionResource.Exotics).ToString(),
				" (",
				tifactionState.GetDailyIncome(FactionResource.Exotics, false, false).ToString(),
				") \n",
				aisavingTarget.active ? string.Concat(new string[]
				{
					"Saving for: ",
					aisavingTarget.desiredPurchase.displayName,
					" ",
					aisavingTarget.location.displayName,
					" ",
					aisavingTarget.relatedGoal.description,
					"\nResources Required: ",
					aisavingTarget.GetResourcesToSave().ToString("Relevant", false, false, null, false, FactionResource.None)
				}) : ""
			}));
		}

		// Token: 0x0600591E RID: 22814 RVA: 0x0028E648 File Offset: 0x0028C848
		public void DumpFleetGoal(string[] args)
		{
			TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
			if (uiotherSelectedState != null && uiotherSelectedState.isSpaceFleetState)
			{
				TerminalController terminalController = this.terminalController;
				FactionGoal_Fleet factionGoal_Fleet = GeneralControlsController.UIOtherSelectedState.ref_fleet.AssignedGoal();
				terminalController.Output(((factionGoal_Fleet != null) ? factionGoal_Fleet.description : null) ?? "No goal");
				return;
			}
			this.terminalController.OutputError("select AI fleet");
		}

		// Token: 0x0600591F RID: 22815 RVA: 0x0028E6A8 File Offset: 0x0028C8A8
		public void DumpPendingTimeEvents(string[] args)
		{
			TIFactionState.LogAI("# events:" + World.Active.GetExistingManager<GameTimeManager>().timeQueue.events.Count.ToString(), false);
			foreach (TITimeEvent titimeEvent in World.Active.GetExistingManager<GameTimeManager>().timeQueue.events)
			{
				string[] array = new string[5];
				array[0] = titimeEvent.eventName;
				array[1] = " ";
				array[2] = titimeEvent.time.ToCustomTimeString();
				array[3] = " ";
				int num = 4;
				TIGameState eventObject = titimeEvent.eventObject;
				array[num] = ((eventObject != null) ? eventObject.displayName : null) ?? "null EO";
				TIFactionState.LogAI(string.Concat(array), false);
			}
			if (World.Active.GetExistingManager<GameTimeManager>().timeQueue.events.Count != GameStateManager.IterateByClass<TITimeEvent>(false).Count<TITimeEvent>())
			{
				TIFactionState.LogAI("Events in State but not in queue:", false);
				foreach (TITimeEvent titimeEvent2 in from x in GameStateManager.IterateByClass<TITimeEvent>(false)
					where !World.Active.GetExistingManager<GameTimeManager>().timeQueue.events.Contains(x)
					select x)
				{
					TIFactionState.LogAI(titimeEvent2.eventName + " " + titimeEvent2.time.ToCustomTimeString(), false);
				}
			}
		}

		// Token: 0x06005920 RID: 22816 RVA: 0x0028E838 File Offset: 0x0028CA38
		public void PR(string[] args)
		{
			TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
			if (tifactionState != null)
			{
				float num = 1f;
				if (args.Length > 1)
				{
					num = TIUtilities.GetFloatValue(args[1]);
				}
				TINationState tinationState = null;
				if (args.Length > 2)
				{
					tinationState = GameStateManager.FindByTemplate<TINationState>(args[2], false);
				}
				if (tinationState != null)
				{
					tinationState.PropagandaOnPop(tifactionState.ideology, num, false);
					return;
				}
				using (IEnumerator<TINationState> enumerator = GameStateManager.AllExtantHumanNations().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TINationState tinationState2 = enumerator.Current;
						tinationState2.PropagandaOnPop(tifactionState.ideology, num, false);
					}
					return;
				}
			}
			this.terminalController.OutputError("need faction dataname, optionally strength value and nation dataname");
		}

		// Token: 0x06005921 RID: 22817 RVA: 0x0028E8F0 File Offset: 0x0028CAF0
		public void AddAerosols(string[] args)
		{
			if (args.Length >= 1)
			{
				float floatValue = TIUtilities.GetFloatValue(args[0]);
				TIGlobalValuesState.GlobalValues.AddStratosphericAerosols_ppm(floatValue, false);
				return;
			}
			this.terminalController.OutputError("enter amount");
		}

		// Token: 0x06005922 RID: 22818 RVA: 0x0028E92C File Offset: 0x0028CB2C
		public void AddCO2(string[] args)
		{
			if (args.Length >= 1)
			{
				float floatValue = TIUtilities.GetFloatValue(args[0]);
				TIGlobalValuesState.GlobalValues.AddCO2_ppm(floatValue, GHGSources.Effect);
				return;
			}
			this.terminalController.OutputError("enter amount");
		}

		// Token: 0x04004076 RID: 16502
		private TerminalController terminalController;
	}
}
