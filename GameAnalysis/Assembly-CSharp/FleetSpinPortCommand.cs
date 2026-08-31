using System;
using System.Linq;

// Token: 0x02000395 RID: 917
public class FleetSpinPortCommand : TIFleetManueverCommandTemplate_Spin
{
	// Token: 0x060010B5 RID: 4277 RVA: 0x00055652 File Offset: 0x00053852
	public override int IconPosition()
	{
		return 15;
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x00055656 File Offset: 0x00053856
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.SpinPort;
	}

	// Token: 0x060010B7 RID: 4279 RVA: 0x00055659 File Offset: 0x00053859
	public override CombatManeuver OppositeManeuver()
	{
		return CombatManeuver.SpinStarboard;
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x0005565D File Offset: 0x0005385D
	public override CombatManeuver CancelManeuver()
	{
		return CombatManeuver.CancelSpinPort;
	}

	// Token: 0x060010B9 RID: 4281 RVA: 0x00055661 File Offset: 0x00053861
	public override CombatManeuver CancelOppositeManeuver()
	{
		return CombatManeuver.CancelSpinStarboard;
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x00055665 File Offset: 0x00053865
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(SpinPortCommand)).GetTemplate();
	}
}
