using System;
using System.Linq;

// Token: 0x02000396 RID: 918
public class FleetSpinStarboardCommand : TIFleetManueverCommandTemplate_Spin
{
	// Token: 0x060010BC RID: 4284 RVA: 0x0005569D File Offset: 0x0005389D
	public override int IconPosition()
	{
		return 17;
	}

	// Token: 0x060010BD RID: 4285 RVA: 0x000556A1 File Offset: 0x000538A1
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.SpinStarboard;
	}

	// Token: 0x060010BE RID: 4286 RVA: 0x000556A5 File Offset: 0x000538A5
	public override CombatManeuver OppositeManeuver()
	{
		return CombatManeuver.SpinPort;
	}

	// Token: 0x060010BF RID: 4287 RVA: 0x000556A8 File Offset: 0x000538A8
	public override CombatManeuver CancelManeuver()
	{
		return CombatManeuver.CancelSpinStarboard;
	}

	// Token: 0x060010C0 RID: 4288 RVA: 0x000556AC File Offset: 0x000538AC
	public override CombatManeuver CancelOppositeManeuver()
	{
		return CombatManeuver.CancelSpinPort;
	}

	// Token: 0x060010C1 RID: 4289 RVA: 0x000556B0 File Offset: 0x000538B0
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(SpinStarboardCommand)).GetTemplate();
	}
}
