using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;

// Token: 0x020003AA RID: 938
public class SelectSalvoTargetCommand : TIShipCommandTemplate, IShipCommandWithTarget
{
	// Token: 0x06001154 RID: 4436 RVA: 0x00056105 File Offset: 0x00054305
	public override int IconPosition()
	{
		return 2;
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x00056108 File Offset: 0x00054308
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && !ship.disengageFromCombat && ship.AnyOffensiveMissileWeaponCanFire();
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x00056123 File Offset: 0x00054323
	public override bool RequiresTarget()
	{
		return true;
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x00056126 File Offset: 0x00054326
	public bool IncludeFriendlyTargets()
	{
		return false;
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x00056129 File Offset: 0x00054329
	public bool OnlyFriendlyTargets()
	{
		return false;
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x0005612C File Offset: 0x0005432C
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target)
	{
		CombatShipController ref_shipController = GameControl.spaceCombat.combatantLookup[ship].ref_shipController;
		ship.faction.playerControl.StartAction(new SetCombatPrimaryTargetAction(ship, target));
		foreach (IWeapon weapon in ref_shipController.hull.IterateByClass<IWeapon>())
		{
			if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Salvo))
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Salvo));
			}
		}
		base.OnExecuteCommand(ship);
		this.EndTargeting(ship.faction);
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x00056200 File Offset: 0x00054400
	public Type GetTargetingMethod()
	{
		return typeof(TICommandTargetableTargeting);
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0005620C File Offset: 0x0005440C
	public void InitiateTargeting(TISpaceShipState ship)
	{
		TICommandTargeting ticommandTargeting = Activator.CreateInstance(this.GetTargetingMethod()) as TICommandTargeting;
		ticommandTargeting.Initialize(ship, this);
		GeneralControlsController.SetUIGlobalTargetingMode(ship, ticommandTargeting);
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x00056239 File Offset: 0x00054439
	public void EndTargeting(TIFactionState faction)
	{
		GeneralControlsController.ShutdownUIGlobalTargetingMode(faction);
	}
}
