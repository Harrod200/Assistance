using System;

// Token: 0x020003BC RID: 956
public class SpinPortCommand : TIShipManueverCommandTemplate_Spin
{
	// Token: 0x060011B7 RID: 4535 RVA: 0x0005717F File Offset: 0x0005537F
	public override int IconPosition()
	{
		return 15;
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x00057183 File Offset: 0x00055383
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.SpinPort;
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x00057186 File Offset: 0x00055386
	public override CombatManeuver OppositeManeuver()
	{
		return CombatManeuver.SpinStarboard;
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0005718A File Offset: 0x0005538A
	public override CombatManeuver CancelManeuver()
	{
		return CombatManeuver.CancelSpinPort;
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0005718E File Offset: 0x0005538E
	public override CombatManeuver CancelOppositeManeuver()
	{
		return CombatManeuver.CancelSpinStarboard;
	}
}
