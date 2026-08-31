using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000F6 RID: 246
public class TIHabCondition_iHabTier : TIHabCondition_Numeric
{
	// Token: 0x06000419 RID: 1049 RVA: 0x00014505 File Offset: 0x00012705
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.tier, TIUtilities.GetIntValue(this.strValue));
	}
}
