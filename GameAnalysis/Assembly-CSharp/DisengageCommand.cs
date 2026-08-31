using System;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003B1 RID: 945
public class DisengageCommand : TIShipCommandTemplate
{
	// Token: 0x06001182 RID: 4482 RVA: 0x00056738 File Offset: 0x00054938
	public override int IconPosition()
	{
		return 10;
	}

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x06001183 RID: 4483 RVA: 0x0005673C File Offset: 0x0005493C
	public override bool TriggersManeuver
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001184 RID: 4484 RVA: 0x00056740 File Offset: 0x00054940
	public override string GetDescription(TISpaceShipState ship = null)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[]
		{
			2000f.ToString("N0"),
			30f.ToString("N0")
		});
	}

	// Token: 0x06001185 RID: 4485 RVA: 0x000567A1 File Offset: 0x000549A1
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return !ship.combatAIControl && !ship.ShipDestroyed() && !ship.disengageFromCombat;
	}

	// Token: 0x06001186 RID: 4486 RVA: 0x000567BE File Offset: 0x000549BE
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return !ship.ShipDestroyed() && !ship.disengageFromCombat;
	}

	// Token: 0x06001187 RID: 4487 RVA: 0x000567D4 File Offset: 0x000549D4
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new SetDisengageStatus(ship, true));
		ship.faction.playerControl.StartAction(new ClearPrimaryTargetAction(ship));
		foreach (IWeapon weapon in GameControl.spaceCombat.combatantLookup[ship].ref_shipController.hull.IterateByClass<IWeapon>())
		{
			if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Defense))
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Defense));
			}
		}
		base.OnExecuteCommand(ship);
	}

	// Token: 0x040010BB RID: 4283
	public const float combatDurationTillAllowed_min = 30f;

	// Token: 0x040010BC RID: 4284
	public const float minDistanceToTrigger_km = 2000f;
}
