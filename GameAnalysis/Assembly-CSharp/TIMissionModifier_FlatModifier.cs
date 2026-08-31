using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001FD RID: 509
public class TIMissionModifier_FlatModifier : TIMissionModifier
{
	// Token: 0x060006F3 RID: 1779 RVA: 0x00021E0B File Offset: 0x0002000B
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		return this.flatModifier;
	}

	// Token: 0x04000624 RID: 1572
	public float flatModifier;
}
