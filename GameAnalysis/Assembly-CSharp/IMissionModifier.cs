using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F6 RID: 502
public interface IMissionModifier
{
	// Token: 0x060006E1 RID: 1761
	float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None);
}
