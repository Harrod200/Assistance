using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000DB RID: 219
public class TICouncilorCondition_bInRivalOrWarEnemyNation : TICouncilorCondition
{
	// Token: 0x060003CD RID: 973 RVA: 0x000139A0 File Offset: 0x00011BA0
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && state.ref_councilor.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_councilor.ref_nation.enemies.Contains(state.ref_councilor.homeNation), TIUtilities.GetBoolValue(this.strValue));
	}

	// Token: 0x060003CE RID: 974 RVA: 0x00013A08 File Offset: 0x00011C08
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
		return state.ref_councilor != null && tinationState2 != null && TICondition.PassesComparison(this.sign, targetedState.ref_nation.enemies.Contains(tinationState2), TIUtilities.GetBoolValue(this.strValue));
	}
}
