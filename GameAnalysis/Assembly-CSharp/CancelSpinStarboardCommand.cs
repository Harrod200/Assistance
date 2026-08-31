using System;

// Token: 0x020003C0 RID: 960
public class CancelSpinStarboardCommand : CancelSpinCommand
{
	// Token: 0x060011CC RID: 4556 RVA: 0x0005723D File Offset: 0x0005543D
	public override int IconPosition()
	{
		return 17;
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x00057241 File Offset: 0x00055441
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelSpinStarboard;
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x00057245 File Offset: 0x00055445
	public override CombatManeuver CancelManeuver()
	{
		return CombatManeuver.SpinStarboard;
	}
}
