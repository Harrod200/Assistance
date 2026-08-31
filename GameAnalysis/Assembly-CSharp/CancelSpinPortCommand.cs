using System;

// Token: 0x020003BF RID: 959
public class CancelSpinPortCommand : CancelSpinCommand
{
	// Token: 0x060011C8 RID: 4552 RVA: 0x0005722A File Offset: 0x0005542A
	public override int IconPosition()
	{
		return 15;
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x0005722E File Offset: 0x0005542E
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelSpinPort;
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x00057232 File Offset: 0x00055432
	public override CombatManeuver CancelManeuver()
	{
		return CombatManeuver.SpinPort;
	}
}
