using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D9 RID: 217
public class TICouncilorCondition_bInHomeNation : TICouncilorCondition
{
	// Token: 0x060003C6 RID: 966 RVA: 0x000137AE File Offset: 0x000119AE
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x000137B8 File Offset: 0x000119B8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.homeNation == state.ref_councilor.ref_nation, TIUtilities.GetBoolValue(this.strValue));
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00013808 File Offset: 0x00011A08
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
		return state.ref_councilor != null && tinationState2 != null && TICondition.PassesComparison(this.sign, state.ref_councilor.homeNation == tinationState2, TIUtilities.GetBoolValue(this.strValue));
	}
}
