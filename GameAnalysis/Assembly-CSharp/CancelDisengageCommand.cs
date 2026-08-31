using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003B2 RID: 946
public class CancelDisengageCommand : TIShipCommandTemplate
{
	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x06001189 RID: 4489 RVA: 0x000568B8 File Offset: 0x00054AB8
	public override bool TriggersManeuver
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x000568BB File Offset: 0x00054ABB
	public override int IconPosition()
	{
		return 10;
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x000568C0 File Offset: 0x00054AC0
	public override string GetDescription(TISpaceShipState ship = null)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[]
		{
			2000f.ToString("N0"),
			30f.ToString("N0")
		});
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x00056921 File Offset: 0x00054B21
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return !ship.combatAIControl && !ship.ShipDestroyed() && ship.disengageFromCombat;
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x0005693B File Offset: 0x00054B3B
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return !ship.ShipDestroyed() && ship.disengageFromCombat;
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x0005694D File Offset: 0x00054B4D
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new SetDisengageStatus(ship, false));
		base.OnExecuteCommand(ship);
	}

	// Token: 0x040010BD RID: 4285
	public const float combatDurationTillAllowed_min = 30f;

	// Token: 0x040010BE RID: 4286
	public const float minDistanceToTrigger_km = 2000f;
}
