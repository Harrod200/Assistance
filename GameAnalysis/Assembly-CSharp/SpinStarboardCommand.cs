using System;

// Token: 0x020003BD RID: 957
public class SpinStarboardCommand : TIShipManueverCommandTemplate_Spin
{
	// Token: 0x060011BD RID: 4541 RVA: 0x0005719A File Offset: 0x0005539A
	public override int IconPosition()
	{
		return 17;
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0005719E File Offset: 0x0005539E
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.SpinStarboard;
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x000571A2 File Offset: 0x000553A2
	public override CombatManeuver OppositeManeuver()
	{
		return CombatManeuver.SpinPort;
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x000571A5 File Offset: 0x000553A5
	public override CombatManeuver CancelManeuver()
	{
		return CombatManeuver.CancelSpinStarboard;
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x000571A9 File Offset: 0x000553A9
	public override CombatManeuver CancelOppositeManeuver()
	{
		return CombatManeuver.CancelSpinPort;
	}
}
