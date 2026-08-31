using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200023D RID: 573
public class TIMissionModifier_DefenderHabSize : TIMissionModifier
{
	// Token: 0x06000788 RID: 1928 RVA: 0x00023A80 File Offset: 0x00021C80
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIHabState ref_hab = target.ref_hab;
		int? num = ((ref_hab != null) ? new int?(ref_hab.numCompletedModules) : null);
		if (num == null)
		{
			return 0f;
		}
		return (float)num.GetValueOrDefault();
	}
}
