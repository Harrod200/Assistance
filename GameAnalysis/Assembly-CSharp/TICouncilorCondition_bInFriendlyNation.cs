using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000DA RID: 218
public class TICouncilorCondition_bInFriendlyNation : TICouncilorCondition
{
	// Token: 0x060003CA RID: 970 RVA: 0x0001388C File Offset: 0x00011A8C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.homeNation == state.ref_councilor.ref_nation || state.ref_councilor.homeNation.allies.Contains(state.ref_councilor.ref_nation), TIUtilities.GetBoolValue(this.strValue));
	}

	// Token: 0x060003CB RID: 971 RVA: 0x00013900 File Offset: 0x00011B00
	public override bool TargetPassesCondition(TIGameState state, TIGameState targetedState)
	{
		TINationState tinationState;
		if (!targetedState.isCouncilorState)
		{
			tinationState = targetedState.ref_nation;
		}
		else
		{
			TIGameState tigameState = TIMissionPhaseState.CouncilorLastKnownLocation(state.ref_faction, targetedState.ref_councilor);
			tinationState = ((tigameState != null) ? tigameState.ref_nation : null);
		}
		TINationState tinationState2 = tinationState;
		return state.ref_councilor != null && tinationState2 != null && TICondition.PassesComparison(this.sign, state.ref_councilor.homeNation == tinationState2 || state.ref_councilor.homeNation.allies.Contains(tinationState2), TIUtilities.GetBoolValue(this.strValue));
	}
}
