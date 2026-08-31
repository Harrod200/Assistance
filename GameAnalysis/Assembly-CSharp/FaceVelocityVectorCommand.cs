using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003CB RID: 971
public class FaceVelocityVectorCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x06001211 RID: 4625 RVA: 0x000576DC File Offset: 0x000558DC
	public override int IconPosition()
	{
		return 19;
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x000576E0 File Offset: 0x000558E0
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.FaceVelocityVector;
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x000576E4 File Offset: 0x000558E4
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.CanRotateAndRoll() && !ship.activeCombatManeuvers.Contains(this.Maneuver());
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x0005771A File Offset: 0x0005591A
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.activeCombatManeuvers.Clear();
		base.OnCommandExecute(ship, target);
	}
}
