using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;

// Token: 0x0200035F RID: 863
public class TICommandTargetableTargeting : TICommandTargeting
{
	// Token: 0x06000F2E RID: 3886 RVA: 0x0004D46E File Offset: 0x0004B66E
	public override List<Type> TargetedGameStates()
	{
		return new List<Type> { typeof(CombatTargetableState) };
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x0004D488 File Offset: 0x0004B688
	public override void Initialize(TISpaceShipState ship, IShipCommandWithTarget command)
	{
		this.ship = ship;
		this.command = command;
		this.possibleTargets = this.GetPossibleTargets(command.IncludeFriendlyTargets(), command.OnlyFriendlyTargets());
		GameControl.eventManager.AddListener<CombatTargetedableStateSelected>(new EventManager.EventDelegate<CombatTargetedableStateSelected>(this.ValidTargetSelectedForTargeting), null, null, false, false);
		GameControl.eventManager.AddListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.CleanupListeners), null, null, true, false);
		this.fleetTargeting = false;
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x0004D4F8 File Offset: 0x0004B6F8
	public override void Initialize(List<TISpaceShipState> ships, IFleetCommandWithTarget command)
	{
		this.ships = ships;
		this.ship = ships[0];
		this.fleetCommand = command;
		this.possibleTargets = this.GetPossibleTargets(command.IncludeFriendlyTargets(), command.OnlyFriendlyTargets());
		GameControl.eventManager.AddListener<CombatTargetedableStateSelected>(new EventManager.EventDelegate<CombatTargetedableStateSelected>(this.ValidTargetSelectedForTargeting), null, null, false, false);
		GameControl.eventManager.AddListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.CleanupListeners), null, null, true, false);
		this.fleetTargeting = true;
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x0004D573 File Offset: 0x0004B773
	private void CleanupListeners(CombatEnds e)
	{
		GameControl.eventManager.RemoveListener<CombatTargetedableStateSelected>(new EventManager.EventDelegate<CombatTargetedableStateSelected>(this.ValidTargetSelectedForTargeting), null);
		GameControl.eventManager.RemoveListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.CleanupListeners), null);
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x0004D5A3 File Offset: 0x0004B7A3
	private void CleanupListeners()
	{
		GameControl.eventManager.RemoveListener<CombatTargetedableStateSelected>(new EventManager.EventDelegate<CombatTargetedableStateSelected>(this.ValidTargetSelectedForTargeting), null);
		GameControl.eventManager.RemoveListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.CleanupListeners), null);
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x0004D5D4 File Offset: 0x0004B7D4
	private bool ValidShipTarget(CombatantShipController shipController, bool allIncludingFriendlies, bool onlyFriendlies)
	{
		if (!(shipController != null) || !(shipController.ShipState != null))
		{
			return false;
		}
		if (shipController.isDestroyed || shipController.destructionTriggered)
		{
			return false;
		}
		if (shipController.ShipState.faction.permanentAlly(GameControl.control.activePlayer))
		{
			return onlyFriendlies || allIncludingFriendlies;
		}
		return !onlyFriendlies;
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x0004D63C File Offset: 0x0004B83C
	private bool ValidHabModuleTarget(CombatHabModuleController moduleController, bool allIncludingFriendlies, bool onlyFriendlies)
	{
		if (!(moduleController != null) || !(moduleController.habModule != null))
		{
			return false;
		}
		if (moduleController.destructionTriggered)
		{
			return false;
		}
		if (moduleController.habModule.GetFaction().permanentAlly(GameControl.control.activePlayer))
		{
			return onlyFriendlies || allIncludingFriendlies;
		}
		return !onlyFriendlies;
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x0004D69A File Offset: 0x0004B89A
	private bool ValidTarget(CombatantController target)
	{
		if (target.damageableType == IDamageableType.Ship)
		{
			return this.ValidShipTarget(target.ref_shipController, false, false);
		}
		return target.damageableType == IDamageableType.StationModule && this.ValidHabModuleTarget(target.ref_habModuleController, false, false);
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x0004D6CC File Offset: 0x0004B8CC
	public new List<CombatTargetableState> GetPossibleTargets(bool includeFriendlies, bool onlyFriendlies)
	{
		List<CombatTargetableState> list = new List<CombatTargetableState>();
		foreach (CombatantShipController combatantShipController in GameControl.spaceCombat.activeShips)
		{
			if (this.ValidShipTarget(combatantShipController, includeFriendlies, onlyFriendlies))
			{
				list.Add(combatantShipController.ShipState);
			}
		}
		foreach (CombatHabModuleController combatHabModuleController in GameControl.spaceCombat.combatHabModuleControllers)
		{
			if (this.ValidHabModuleTarget(combatHabModuleController, includeFriendlies, onlyFriendlies))
			{
				list.Add(combatHabModuleController.habModule);
			}
		}
		return list;
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x0004D794 File Offset: 0x0004B994
	private void ValidTargetSelectedForTargeting(CombatTargetedableStateSelected e)
	{
		if (this.fleetTargeting && this.fleetCommand != null)
		{
			if (this.GetPossibleTargets(this.fleetCommand.IncludeFriendlyTargets(), this.fleetCommand.OnlyFriendlyTargets()).Contains(e.target))
			{
				this.fleetCommand.OnExecuteFleetCommand(this.ships, e.target);
			}
			else
			{
				this.fleetCommand.EndTargeting(this.ships[0].faction);
			}
		}
		else if (this.command != null)
		{
			if (this.GetPossibleTargets(this.command.IncludeFriendlyTargets(), this.command.OnlyFriendlyTargets()).Contains(e.target))
			{
				this.command.OnCommandExecute(this.ship, e.target);
			}
			else
			{
				this.command.EndTargeting(this.ship.faction);
			}
		}
		this.CleanupListeners();
	}

	// Token: 0x04000F42 RID: 3906
	private TISpaceShipState ship;

	// Token: 0x04000F43 RID: 3907
	private List<TISpaceShipState> ships;

	// Token: 0x04000F44 RID: 3908
	private IShipCommandWithTarget command;

	// Token: 0x04000F45 RID: 3909
	private IFleetCommandWithTarget fleetCommand;

	// Token: 0x04000F46 RID: 3910
	private new List<CombatTargetableState> possibleTargets;

	// Token: 0x04000F47 RID: 3911
	private bool fleetTargeting;
}
