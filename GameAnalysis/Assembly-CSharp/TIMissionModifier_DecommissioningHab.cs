using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000253 RID: 595
public class TIMissionModifier_DecommissioningHab : TIMissionModifier
{
	// Token: 0x060007B6 RID: 1974 RVA: 0x00024828 File Offset: 0x00022A28
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.isHabState && target.ref_hab.decommissioning)
		{
			return -100f;
		}
		return 0f;
	}
}
