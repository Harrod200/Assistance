using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000DC RID: 220
public class TICouncilorCondition_bInWarEnemyNation : TICouncilorCondition
{
	// Token: 0x060003D0 RID: 976 RVA: 0x00013A8C File Offset: 0x00011C8C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && state.ref_councilor.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_councilor.ref_nation.wars.Contains(state.ref_councilor.homeNation), TIUtilities.GetBoolValue(this.strValue));
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00013AF4 File Offset: 0x00011CF4
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
		return state.ref_councilor != null && tinationState2 != null && TICondition.PassesComparison(this.sign, tinationState2.wars.Contains(state.ref_councilor.homeNation), TIUtilities.GetBoolValue(this.strValue));
	}
}
