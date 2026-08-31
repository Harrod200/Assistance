using System;
using System.Linq;

// Token: 0x02000398 RID: 920
public class FleetCancelSpinPortCommand : CancelFleetSpinCommand
{
	// Token: 0x060010C9 RID: 4297 RVA: 0x00055762 File Offset: 0x00053962
	public override int IconPosition()
	{
		return 15;
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x00055766 File Offset: 0x00053966
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelSpinPort;
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x0005576A File Offset: 0x0005396A
	public override CombatManeuver CancelManeuver()
	{
		return CombatManeuver.SpinPort;
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x0005576D File Offset: 0x0005396D
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(CancelSpinPortCommand)).GetTemplate();
	}
}
