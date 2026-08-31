using System;
using System.Linq;

// Token: 0x02000399 RID: 921
public class FleetCancelSpinStarboardCommand : CancelFleetSpinCommand
{
	// Token: 0x060010CE RID: 4302 RVA: 0x000557A5 File Offset: 0x000539A5
	public override int IconPosition()
	{
		return 17;
	}

	// Token: 0x060010CF RID: 4303 RVA: 0x000557A9 File Offset: 0x000539A9
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelSpinStarboard;
	}

	// Token: 0x060010D0 RID: 4304 RVA: 0x000557AD File Offset: 0x000539AD
	public override CombatManeuver CancelManeuver()
	{
		return CombatManeuver.SpinStarboard;
	}

	// Token: 0x060010D1 RID: 4305 RVA: 0x000557B1 File Offset: 0x000539B1
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(CancelSpinStarboardCommand)).GetTemplate();
	}
}
