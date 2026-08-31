using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003BA RID: 954
public abstract class TIShipManueverCommandTemplate_Roll : TIShipManeuverCommandTemplate
{
	// Token: 0x060011AD RID: 4525 RVA: 0x00057096 File Offset: 0x00055296
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && !ship.SystemDestroyed(ShipSystem.VectorThrusters) && !ship.Rolling() && !ship.PerformingCombatManeuver(TIShipManeuverCommandTemplate.exclusiveManeuvers);
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x000570C3 File Offset: 0x000552C3
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new AddCombatManeuverAction(ship, this.Maneuver()));
		base.OnExecuteCommand(ship);
	}
}
