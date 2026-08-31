using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200020B RID: 523
public class TIMissionModifier_AttackerUnrestIdeology : TIMissionModifier
{
	// Token: 0x06000715 RID: 1813 RVA: 0x00022158 File Offset: 0x00020358
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		TIFactionState ref_faction = attackingCouncilor.ref_faction;
		if (tinationState != null)
		{
			int councilUnrestAttempts = tinationState.GetCouncilUnrestAttempts(ref_faction);
			if (councilUnrestAttempts > 0)
			{
				return (float)councilUnrestAttempts;
			}
		}
		return 0f;
	}
}
