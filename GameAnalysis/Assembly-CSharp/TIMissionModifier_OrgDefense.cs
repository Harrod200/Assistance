using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000206 RID: 518
public class TIMissionModifier_OrgDefense : TIMissionModifier
{
	// Token: 0x0600070B RID: 1803 RVA: 0x00021FEC File Offset: 0x000201EC
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TIOrgState tiorgState = target as TIOrgState;
		if (tiorgState != null)
		{
			num = tiorgState.takeoverDefense;
		}
		return num;
	}
}
